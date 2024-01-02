using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Crestron.SimplSharp;                              // For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;            // For Threading
using Crestron.SimplSharpPro.UI;
using System.Runtime.CompilerServices;

namespace H_and_F_Room_Controller
{
    public class ControlSystem : CrestronControlSystem
    {
        public static bool debugEnabled = false;
        PerformanceCounter _cpuCounter;

        public bool fireAlarmState = false;

        public event Action<int, string> serialDataReceived;

        public int refreshCalendarAfterMinutes = 1;
        public DateAndTime dateAndTime;
        public BookingManager bookingManager;
        public string microsoftAuthToken;

        public RoomManager roomManager;

        public Tsw770 testPanel;

        public SSE_Server sse;
        MicrosoftAppIntegrationInfo _microsoftAccountInfo;
        System.Timers.Timer _tokenRefreshTimer;
        System.Timers.Timer _currentTimeTimer;

        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(_ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(_ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(_ControllerEthernetEventHandler);

                if(this.SupportsEthernet)
                {
                    try
                    {
                        ConsoleLogger cs = new ConsoleLogger();
                        cs.ConsoleLoggerStart(55555, this); 

                        sse = new SSE_Server(this);
                        WebServer ws = new WebServer(this, sse);
                        bookingManager= new BookingManager(sse, this);
                        dateAndTime = new DateAndTime();

                        _microsoftAccountInfo = JsonConvert.DeserializeObject<MicrosoftAppIntegrationInfo>(FileOperations.loadCoreInfo("MicrosoftAccountInfo"));

                        _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

                        _tokenRefreshTimer = new System.Timers.Timer();
                        _tokenRefreshTimer.Interval = 3000000;
                        _tokenRefreshTimer.Elapsed += _tokenRefreshTimer_Elapsed;
                        _tokenRefreshTimer.AutoReset = true;

                        _currentTimeTimer = new System.Timers.Timer();
                        _currentTimeTimer.Interval = 1000;
                        _currentTimeTimer.Elapsed += _currentTimeTimer_Elapsed;
                        _currentTimeTimer.AutoReset = true;

                        _tokenRefreshTimer.Start();
                        _currentTimeTimer.Start();
                        microsoftAuthToken = string.Empty;
                        Task.Run(() => { microsoftAuthToken = GetNewToken(); });

                        roomManager = new RoomManager(this);

                        testPanel = new Tsw770(64, this);
                        testPanel.Register();
                    }
                    catch (Exception ex)
                    {
                        ConsoleLogger.WriteLine("Problem Starting Web Server: " + ex);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        #region Microsoft Outlook Comms

        int _previousMinute = -1;
        int _minutesPassed = 0;
        private void _currentTimeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime timeNow = DateTime.Now;

            if(timeNow.Minute < 10)
                dateAndTime.currentMinute = "0"+timeNow.Minute;
            else
                dateAndTime.currentMinute = ""+timeNow.Minute;

            if (timeNow.Hour < 10)
                dateAndTime.currentHour = "0" + timeNow.Hour;
            else
                dateAndTime.currentHour = "" + timeNow.Hour;

            dateAndTime.DayOfWeek = timeNow.DayOfWeek.ToString();
            dateAndTime.currentDay = timeNow.Day.ToString();
            dateAndTime.currentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(timeNow.Month);
            dateAndTime.currentYear = timeNow.Year.ToString();

            if(_previousMinute == -1) _previousMinute = timeNow.Minute;
            
            if(_previousMinute == timeNow.Minute) { }
            if (_previousMinute < timeNow.Minute || (_previousMinute == 59 && timeNow.Minute == 0))
            {
                _previousMinute = timeNow.Minute;
                sse.SendTimeToAllConnected(dateAndTime);

                _minutesPassed++;
                if (_minutesPassed >= refreshCalendarAfterMinutes)
                {
                    if (microsoftAuthToken != string.Empty) GetCalendarBookings();
                    else if (microsoftAuthToken == string.Empty) ConsoleLogger.WriteLine("Cannot Fetch Calendar Info, Microsoft Authentication Token is Empty");

                    _minutesPassed = 0;
                }
                else
                {
                    foreach(var room in roomManager.rooms)
                        if(room.GetEmailAddress() != "")
                            bookingManager.DecreaseTimeUntilNextMeetingMinutes(room.GetRoomID());
                }

                CheckGlobalTemperatureCall();
            }

            if(debugEnabled)
                ConsoleLogger.WriteLine("CPU Usage: " + _cpuCounter.NextValue() + "%");
        }

        private void _tokenRefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Task.Run(() => { microsoftAuthToken = GetNewToken(); });
        }

        string GetNewToken()
        {
            try
            {
                ConsoleLogger.WriteLine("Getting Token...");

                AccessToken newToken = new AccessToken();

                var resource = "https://graph.microsoft.com/.default";
                var url = "https://login.microsoftonline.com/" + _microsoftAccountInfo.tenantID + "/oauth2/v2.0/token";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 5000;

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "grant_type=client_credentials&client_id=" + _microsoftAccountInfo.clientID + "&scope=" + resource + "&client_secret=" + _microsoftAccountInfo.clientSecret;

                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    newToken = JsonConvert.DeserializeObject<AccessToken>(result);
                    ConsoleLogger.WriteLine("Received Response: " + result.ToString());
                }

                GetCalendarBookings();

                ConsoleLogger.WriteLine("New Access Token = " + newToken.access_token.Substring(0, 30) + "...");
                return newToken.access_token;
            }
            catch(Exception e)
            {
                ErrorLog.Error("Error in GetNewToken: {0}", e.Message);

                return string.Empty;
            }
        }

        Task GettingCalendarBookings = null;
        public void GetCalendarBookings()
        {
            if (microsoftAuthToken == null) return;
            if (GettingCalendarBookings != null) if (!GettingCalendarBookings.IsCompleted) return;

            GettingCalendarBookings = Task.Run(() =>
            {
                foreach (var directory in FileOperations.GetRoomDirectories())
                {
                    int roomFolderNamePos = directory.Split('/').Length - 1;
                    string roomFolderName = directory.Split('/')[roomFolderNamePos];
                    int roomID = int.Parse(roomFolderName.Replace("Room", ""));

                    RoomCoreInfo roomData = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(roomID, "Core"));
                    string emailsAddressToFetch = roomData.emailAddress;

                    if (emailsAddressToFetch != "")
                    {
                        ConsoleLogger.WriteLine("-----------------------------------------------------------------------------------");
                        ConsoleLogger.WriteLine("Fetching Calendar Info for " + emailsAddressToFetch);
                        FetchUserInfo(emailsAddressToFetch, roomID);
                        FetchCalendarBookings(emailsAddressToFetch, roomID);
                    }else
                    {
                        if(FileOperations.GetRoomDirectories().Count == 1)
                        {
                            _tokenRefreshTimer.Stop();
                        }
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        void FetchUserInfo(string emailAddressToFetch, int roomID)
        {
            UserInfo userInfo = new UserInfo();
            ConsoleLogger.WriteLine("Fetching User Info");

            var url = "https://graph.microsoft.com/v1.0/users/" + emailAddressToFetch;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 5000;
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", "Bearer " + microsoftAuthToken);

            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                try
                {
                    var result = streamReader.ReadToEnd();
                    userInfo = JsonConvert.DeserializeObject<UserInfo>(result);
                    ConsoleLogger.WriteLine("User Name: " + userInfo.displayName);
                    ConsoleLogger.WriteLine("User Email: " + userInfo.mail);
                }
                catch (Exception ex)
                { ConsoleLogger.WriteLine("Exception in FetchUserInfo() 1: " + ex.ToString()); }
            }

            try
            {
                FileOperations.saveUserInfo(roomID, "UserInfo", userInfo);
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in FetchUserInfo() 2: " + ex);
            }
        }

        void FetchCalendarBookings(string emailAddressToFetch, int roomID)
        {
            Bookings bookings = new Bookings();
            ConsoleLogger.WriteLine("Fetching Bookings for: " + emailAddressToFetch);

            var url = "https://graph.microsoft.com/v1.0/users/" + emailAddressToFetch + "/calendarview?" +
                "startdatetime=" + DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day + "T00:00:00.000Z&" +
                "enddatetime=" + DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day + "T23:00:00.000Z";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 5000;
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", "Bearer " + microsoftAuthToken);

            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                try
                {
                    var result = streamReader.ReadToEnd();
                    bookings = JsonConvert.DeserializeObject<Bookings>(result);
                    ConsoleLogger.WriteLine("Bookings fetched: " + bookings.value.Count);
                }
                catch (Exception ex)
                { ConsoleLogger.WriteLine("Exception in FetchCalendarBookings() 1: " + ex.ToString()); }
            }

            try
            {
                if(bookings.value.Count > 0) bookingManager.ProcessBookings(roomID, bookings);
            }
            catch(Exception ex) { ConsoleLogger.WriteLine("Exception in FetchCalendarBookings() 2: " + ex.ToString()); }
        }

        #endregion

        private void FireAlarmRelay_VersiportChange(Versiport port, VersiportEventArgs args)
        {
            ConsoleLogger.WriteLine("Port" + port.DeviceName + "state changed to: " + args.Event + "Digital In State: " + port.DigitalIn);

            FireAlarmStateChanged(!port.DigitalIn);

            if (!port.DigitalIn)
                ConsoleLogger.WriteLine("FireAlarm recorded at: " + DateTime.Now);
        }
        public void FireAlarmStateChanged(bool state)
        {
            fireAlarmState = state;

            try
            {
                if (state)
                {
                    ConsoleLogger.WriteLine("FireAlarm");
                    foreach (var directory in FileOperations.GetRoomDirectories())
                    {
                        int roomFolderNamePos = directory.Split('/').Length - 1;
                        string roomFolderName = directory.Split('/')[roomFolderNamePos];
                        int roomID = int.Parse(roomFolderName.Replace("Room", ""));

                        sse.UpdateAllConnected(roomID, "FireAlarm:1");
                    }
                }
                else
                {
                    ConsoleLogger.WriteLine("FireAlarm Cleared");
                    foreach (var directory in FileOperations.GetRoomDirectories())
                    {
                        int roomFolderNamePos = directory.Split('/').Length - 1;
                        string roomFolderName = directory.Split('/')[roomFolderNamePos];
                        int roomID = int.Parse(roomFolderName.Replace("Room", ""));

                        sse.UpdateAllConnected(roomID, "FireAlarm:0");
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in FireAlarmStateChanged(): " + ex);
            }
        }

        public void SystemAlert(bool createAlert, int floor, string roomName, string issue)
        {
            CoreProcessorInfo processorInfo = JsonConvert.DeserializeObject<CoreProcessorInfo>(FileOperations.loadCoreInfo("CoreProcessorInfo"));

            Task.Run(() =>
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://"+processorInfo.IPAddress+ ":50000/api/SystemAlert");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 10000;

                ConsoleLogger.WriteLine("Sending system alert to Core...");
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"createAlert\": \""+createAlert+"\", \"floor\": \""+floor+"\", \"roomName\": \""+roomName+"\", \"issue\": \""+issue+"\" }";
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ConsoleLogger.WriteLine("Received Response from Core: " + result.ToString());
                }
                ConsoleLogger.WriteLine("SystemAlert sent");
            });
        }

        void CheckGlobalTemperatureCall()
        {
            CoreProcessorInfo processorInfo = JsonConvert.DeserializeObject<CoreProcessorInfo>(FileOperations.loadCoreInfo("CoreProcessorInfo"));

            Task.Run(() =>
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + processorInfo.IPAddress + ":50000/api/GetMasterTemp");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 10000;

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    GlobalTemp localGlobalTempCopy = JsonConvert.DeserializeObject<GlobalTemp>(FileOperations.loadCoreInfo("GlobalTemp"));
                    GlobalTemp coreGlobalTempCopy = JsonConvert.DeserializeObject<GlobalTemp>(result);

                    if (localGlobalTempCopy.globalTemp != coreGlobalTempCopy.globalTemp)
                    {
                        localGlobalTempCopy.globalTemp = coreGlobalTempCopy.globalTemp;
                        FileOperations.saveGlobalTemp(localGlobalTempCopy);

                        roomManager.GlobalTempChanged(coreGlobalTempCopy);
                    }

                    ConsoleLogger.WriteLine("Received Response from Core: " + result.ToString());
                }
            });
        }

        public override void InitializeSystem()
        {
            try
            {
                DivisibleInfo di = JsonConvert.DeserializeObject<DivisibleInfo>(FileOperations.loadCoreInfo("DivisibleInfo"));

                if(this.SupportsVersiport)
                {
                    this.VersiPorts[1].Register();
                    this.VersiPorts[1].SetVersiportConfiguration(eVersiportConfiguration.DigitalInput);
                    this.VersiPorts[2].Register();
                    this.VersiPorts[2].SetVersiportConfiguration(eVersiportConfiguration.DigitalInput);

                    if (di.hasDivisibleRooms)
                    {
                        if (di.numberOfWalls > 1)
                        {
                            this.VersiPorts[1].VersiportChange += WallPartitionStateChange;
                            this.VersiPorts[2].VersiportChange += WallPartitionStateChange;
                        }
                        else
                            this.VersiPorts[1].VersiportChange += WallPartitionStateChange;
                    }
                    else
                        this.VersiPorts[1].VersiportChange += FireAlarmRelay_VersiportChange;
                }

                if(this.SupportsComPort)
                {
                    for(uint i = 0; i < this.ComPorts.Count; i++)
                    {
                        this.ComPorts[i+1].Register();
                        this.ComPorts[i+1].SerialDataReceived += ControlSystem_SerialDataReceived;

                        this.ComPorts[i+1].SetComPortSpec(
                            ComPort.eComBaudRates.ComspecBaudRate9600,
                            ComPort.eComDataBits.ComspecDataBits8,
                            ComPort.eComParityType.ComspecParityNone,
                            ComPort.eComStopBits.ComspecStopBits1,
                            ComPort.eComProtocolType.ComspecProtocolRS232,
                            ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
                            ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone,
                            false
                            );
                    }
                }

                if(this.SupportsRelay)
                    for(uint i = 1; i < this.RelayPorts.Count; i++)
                        this.RelayPorts[i].Register();
                
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        public void ChangeRelayState(uint port, bool newState)
        {
            if(newState) this.RelayPorts[port].Close();
            if(!newState) this.RelayPorts[port].Open();
            ConsoleLogger.WriteLine($"Changed Relay {port} to {newState}");
        }

        public void SendSerialData(uint comPort, string data) => this.ComPorts[comPort].Send(data);

        private void ControlSystem_SerialDataReceived(ComPort ReceivingComPort, ComPortSerialDataEventArgs args)
        {
            if (serialDataReceived != null)
                serialDataReceived((int)ReceivingComPort.ID, ReceivingComPort.RcvdString);
        }

        private void WallPartitionStateChange(Versiport port, VersiportEventArgs args)
        {
            ConsoleLogger.WriteLine("IO Port " + port.ID + " changed state to: " + port.DigitalIn);

            roomManager.WallStateChanged((int)port.ID, port.DigitalIn);
        }

        public bool CheckIfAllWallsOpen(DivisibleInfo di)
        {
            if (di.numberOfWalls > 1)
            {
                if (!this.VersiPorts[1].DigitalIn && !this.VersiPorts[2].DigitalIn) return true;
            }
            else if (!this.VersiPorts[1].DigitalIn) return true;

            return false;
        }

        public bool CheckIfAllWallsClosed(DivisibleInfo di)
        {
            if (di.numberOfWalls > 1)
            {
                if (this.VersiPorts[1].DigitalIn && this.VersiPorts[2].DigitalIn) return true;
            }
            else if (this.VersiPorts[1].DigitalIn) return true;

            return false;
        }

        public static string GetLocalIPAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        void _ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {
                case (eEthernetEventType.LinkDown):

                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {

                    }
                    break;
                case (eEthernetEventType.LinkUp):
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {

                    }
                    break;
            }
        }
        void _ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case (eProgramStatusEventType.Paused):
                    break;
                case (eProgramStatusEventType.Resumed):
                    break;
                case (eProgramStatusEventType.Stopping):
                    break;
            }

        }
        void _ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case (eSystemEventType.DiskInserted):
                    break;
                case (eSystemEventType.DiskRemoved):
                    break;
                case (eSystemEventType.Rebooting):
                    break;
            }

        }
    }
}