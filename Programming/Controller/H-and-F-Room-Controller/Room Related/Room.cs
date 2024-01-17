using Crestron.SimplSharp.CrestronSockets;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronConnected;
using Crestron.SimplSharpPro.DM.Streaming;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace H_and_F_Room_Controller
{
    public class Room
    {
        ControlSystem _cs;
        RoomManager _mgr;
        int _id;
        string _email;

        //Video
        DmNvx360[] _nvxTransmitters;
        DmNvxD30[] _nvxReceivers;

        List<Display> _allDisplays;
        List<Display> _roomViewDisplays;
        List<Display> _cecDisplays;
        List<Display> _rs232Displays;

        List<MotorizedScreen> _motorizedScreens;

        List<Camera> _cameras;

        public enum DisplayControlOption
        { 
            Both,
            TVOnly,
            ProjectorOnly
        }
        DisplayControlOption _dco;

        //Audio
        TCPClient _audioProcessorComms;
        TesiraForte _audioProcessor;
        int _volRampSpeedInMs = 75;
        AudioProcessor _audioProcessorSetup;

        //Climate
        decimal _upperSetpointLimit = 24.0M;
        decimal _lowerSetpointLimit = 20.0M;

        //Events
        public event Action<AVSource> sourceChanged;

        //Schedule
        public bool roomPreppedforMeeting = false;

        public Room(ControlSystem cs, int id, RoomManager mgr)
        {
            ConsoleLogger.WriteLine($"--------------------- Initializing Room{id} ---------------------");

            _cs = cs;
            _id = id;
            _mgr = mgr;
            _dco = DisplayControlOption.Both;

            _allDisplays = new List<Display>();
            _cecDisplays = new List<Display>();
            _rs232Displays = new List<Display>();
            _roomViewDisplays = new List<Display>();
            _cameras = new List<Camera>();
            _motorizedScreens = new List<MotorizedScreen>();

            RoomCoreInfo roomCoreInfo = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));
            _email = roomCoreInfo.emailAddress;

            InitializeRoomAV();
        }

        void InitializeRoomAV()
        {
            try
            {
                RoomAVData aVData = JsonConvert.DeserializeObject<RoomAVData>(FileOperations.loadRoomJson(_id, "AV"));

                foreach (var tv in aVData.Displays)
                {
                    _allDisplays.Add(tv);

                    if (tv.controlType == DisplayControlType.roomView)
                    {
                        tv.connectedDisplay = new RoomViewConnectedDisplay(tv.ipid, _cs);
                        _roomViewDisplays.Add(tv);
                    }
                    if (tv.controlType == DisplayControlType.cec) _cecDisplays.Add(tv);
                    if (tv.controlType == DisplayControlType.rs232) _rs232Displays.Add(tv);
                }

                if (_roomViewDisplays.Count > 0)
                {
                    foreach (var roomViewDisplay in _roomViewDisplays)
                    {
                        roomViewDisplay.connectedDisplay.Register();
                        roomViewDisplay.connectedDisplay.OnlineStatusChange += _display_onlineStatusChange;
                    }
                }

                foreach (var motorizedScreen in aVData.motorizedScreens) _motorizedScreens.Add(motorizedScreen);

                ConsoleLogger.WriteLine($"RoomView Displays: {_roomViewDisplays.Count} || RS232 Displays: {_rs232Displays.Count} || CEC Displays: {_cecDisplays.Count}");

                if (aVData.NVXTxs.Length >= 0)
                    _nvxTransmitters = new DmNvx360[aVData.NVXTxs.Length];
                if (aVData.NVXRxs.Length >= 0)
                    _nvxReceivers = new DmNvxD30[aVData.NVXRxs.Length];

                if (_nvxTransmitters.Length > 0)
                {
                    for (int i = 0; i < _nvxTransmitters.Length; i++)
                    {
                        _nvxTransmitters[i] = new DmNvx360(aVData.NVXTxs[i], _cs);
                        _nvxTransmitters[i].Register();
                        _nvxTransmitters[i].OnlineStatusChange += _trasmitter_onlineStatusChange;
                        ConsoleLogger.WriteLine("Registering Tx with IPID: " + aVData.NVXTxs[i]);
                    }
                }

                if (_nvxReceivers.Length > 0)
                {
                    for (int i = 0; i < _nvxReceivers.Length; i++)
                    {
                        _nvxReceivers[i] = new DmNvxD30(aVData.NVXRxs[i], _cs);
                        _nvxReceivers[i].Register();
                        _nvxReceivers[i].OnlineStatusChange += _receiver_onlineStatusChange;
                        ConsoleLogger.WriteLine("Registering Rx with IPID: " + aVData.NVXRxs[i]);
                    }
                }

                if (aVData.AudioProcessor != null)
                {
                    if(aVData.AudioProcessor.controlType == AudioProcessorControlType.ip)
                    {
                        _audioProcessorComms = new TCPClient(aVData.AudioProcessor.ip, aVData.AudioProcessor.TCPport, 4096);
                        _audioProcessor = new TesiraForte(_cs, _audioProcessorComms, $"Room{_id} Audio Processor");
                    }
                    if (aVData.AudioProcessor.controlType == AudioProcessorControlType.rs232)
                        _audioProcessor = new TesiraForte(_cs, aVData.AudioProcessor, $"Room{_id} Audio Processor");

                    _audioProcessorSetup = aVData.AudioProcessor;
                }

                foreach (var cam in aVData.Cameras)
                {
                    if(cam.controlType == CameraControlType.http) _cameras.Add(cam);
                    if(cam.controlType == CameraControlType.viscaIP)
                    {
                        cam.ipComms = new IPConnectionHandler(_cs, new TCPClient(cam.ip, 5500, 4096), cam.name);
                        _cameras.Add(cam);
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in InitializeRoomAV() while registering AV Equipment: " + ex);
            }
        }

        public List<Camera> GetCameras() => _cameras;
        public string GetEmailAddress() => _email;
        public int GetRoomID() => _id;

        #region Room Monitoring

        private void _receiver_onlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            ConsoleLogger.WriteLine("NVX Rx IPID " + currentDevice.ID + " | Online Status: " + args.DeviceOnLine);
            if (!args.DeviceOnLine)
                CreateSystemAlert("NVX Receiver Offline");
            else
                ClearSystemAlert("NVX Receiver Offline");
        }

        private void _trasmitter_onlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            ConsoleLogger.WriteLine("NVX Tx IPID " + currentDevice.ID + " | Online Status: " + args.DeviceOnLine);
            if (!args.DeviceOnLine)
                CreateSystemAlert("NVX Transmitter Offline");
            else
                ClearSystemAlert("NVX Transmitter Offline");
        }

        private void _display_onlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            ConsoleLogger.WriteLine("TV IPID " + currentDevice.ID + " | Online Status: " + args.DeviceOnLine);
            if (!args.DeviceOnLine)
                CreateSystemAlert("TV Offline");
            else
                ClearSystemAlert("TV Offline");
        }

        void CreateSystemAlert(string issue)
        {
            RoomCoreInfo roomCoreInfo = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));
            _cs.SystemAlert(true, roomCoreInfo.floor, roomCoreInfo.roomName, issue);
        }

        void ClearSystemAlert(string issue)
        {
            RoomCoreInfo roomCoreInfo = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));
            _cs.SystemAlert(false, roomCoreInfo.floor, roomCoreInfo.roomName, issue);
        }

        #endregion

        #region Source Change

        public string GetCurrentSource()
        {
            AVSources allSources = JsonConvert.DeserializeObject<AVSources>(FileOperations.loadRoomJson(_id, "AVSources"));
            RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));

            foreach (var source in allSources.sources)
                if (source.sourceName == rci.sourceSelected)
                {
                    ConsoleLogger.WriteLine($"{source.sourceName} : {rci.sourceSelected}");
                    return JsonConvert.SerializeObject(source);
                }

            return JsonConvert.SerializeObject(new AVSource() { sourceName = "Off" });
        }

        public void ChangeSourceSelected(string itemSelected)
        {
            AVSources sources = JsonConvert.DeserializeObject<AVSources>(FileOperations.loadRoomJson(_id, "AVSources"));

            foreach (AVSource source in sources.sources)
                if (source.sourceName == itemSelected)
                {
                    RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));
                    rci.sourceSelected = itemSelected;
                    FileOperations.saveRoomData(_id.ToString(), rci);

                    UpdateAudioEquipment(source);
                    UpdateVideoEquipment(source);

                    Task.Run(() =>
                    {
                        Thread.Sleep(200);
                        _cs.sse.UpdateAllConnected(_id, "Source");
                    });
                    UpdateCoreOnNewSource(source);
                    if (sourceChanged != null) sourceChanged(source);
                }
        }

        private void UpdateAudioEquipment(AVSource source)
        {
            if (source.sourceType.Contains("A"))
            {
                if (_audioProcessorSetup.controlType == AudioProcessorControlType.ip)
                    _audioProcessor.Connect();
            }
            else
            {
                if (_audioProcessorSetup.controlType == AudioProcessorControlType.ip)
                    _audioProcessor.Disconnect();
            }
        }
        void UpdateVideoEquipment(AVSource newSource)
        {
            if (newSource.sourceType.Contains("V"))
            {
                PowerOnDisplays();
                UpdateReceiversStream(newSource);
            }
            else
            {
                PowerOffDisplays();
                UpdateReceiversStream(new AVSource { sourceStreamAddress = "" });
            }

            if (newSource.camerasRequired) ConnectToCameras();
            else DisconnectFromCameras();
        }
        void UpdateCoreOnNewSource(AVSource newSource)
        {
            CoreProcessorInfo cpi = JsonConvert.DeserializeObject<CoreProcessorInfo>(FileOperations.loadCoreInfo("CoreProcessorInfo"));

            Task.Run(() =>
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + cpi.IPAddress + ":50000/api/NewSourceSelected?" + _id);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 5000;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(newSource));
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ConsoleLogger.WriteLine("Received Response from " + cpi.IPAddress + ": " + result.ToString());
                }
                ConsoleLogger.WriteLine("Command sent");
            });
        }

        #endregion


        #region NVX Control

        void UpdateReceiversStream(AVSource newSource)
        {
            ConsoleLogger.WriteLine("Updating Receivers with following stream: " + newSource.sourceStreamAddress);
            foreach (var display in _allDisplays)
            {
                if (display.displayType != DisplayType.ProductionUnit)
                    foreach (var receiver in _nvxReceivers)
                        if (receiver.ID == display.nvxRXConnected)
                            receiver.Control.ServerUrl.StringValue = newSource.sourceStreamAddress;
            }
        }

        public string UpdateProductionUnitStream(string itemSelected)
        {
            try
            {
                RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));
                AVSources sources = JsonConvert.DeserializeObject<AVSources>(FileOperations.loadRoomJson(_id, "AVSources"));
                AVSource selectedSource = null;

                foreach (var source in sources.sources)
                    if (source.sourceName == itemSelected)
                        selectedSource = source;

                foreach (var display in _allDisplays)
                {
                    if (display.displayType == DisplayType.ProductionUnit)
                        foreach (var receiver in _nvxReceivers)
                        {
                            ConsoleLogger.WriteLine(receiver.ID + ":" + display.nvxRXConnected);
                            if (receiver.ID == display.nvxRXConnected)
                            {
                                ConsoleLogger.WriteLine("Updating Production unit with following stream: " + selectedSource.sourceStreamAddress);
                                receiver.Control.ServerUrl.StringValue = selectedSource.sourceStreamAddress;
                                rci.videoProductionStreamSelected = selectedSource.sourceName;
                            }
                        }
                }

                FileOperations.saveRoomData(_id.ToString(), rci);

                return rci.videoProductionStreamSelected;
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Problem in UpdateProductionUnitStream(): " + ex);
                return "";
            }
        }

        public string GetCurrentProductionStream()
        {
            RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));
            return rci.videoProductionStreamSelected;
        }

        #endregion

        #region Camera Controls
        void ConnectToCameras()
        {
            foreach (var cam in _cameras)
                if (cam.controlType == CameraControlType.viscaIP)
                    cam.ipComms.Connect();
        }
        void DisconnectFromCameras()
        {
            foreach (var cam in _cameras)
                if (cam.controlType == CameraControlType.viscaIP)
                    cam.ipComms.Disconnect();
        }

        public void ProcessCameraControlCommand(string camName, string command, string presetNum)
        {
            Camera camToControl = null;

            foreach (var cam in _cameras)
                if (camName == cam.name) camToControl = cam;

            if (camToControl == null) return;

            ConsoleLogger.WriteLine($"Sending {command} to: {camName} ({camToControl.ip})");
            /////////////////////////////
            if (command == "zoom+|start")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "zoomadd_start", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x04, 0x07, 0x02, 0xFF });
            }
            if (command == "zoom+|stop")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "zoomadd_stop", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x04, 0x07, 0x00, 0xFF });
            }
            /////////////////////////////
            if (command == "zoom-|start")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "zoomdec_start", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x04, 0x07, 0x03, 0xFF });
            }
            if (command == "zoom-|stop")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "zoomdec_stop", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x04, 0x07, 0x00, 0xFF });
            }
            /////////////////////////////
            if (command == "up|start")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "up_start", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x03, 0x01, 0xFF });
            }
            if (command == "up|stop")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "up_stop", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x03, 0x03, 0xFF });
            }
            /////////////////////////////
            if (command == "down|start")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "down_start", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x03, 0x02, 0xFF });
            }
            if (command == "down|stop")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "down_stop", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x03, 0x03, 0xFF });
            }
            /////////////////////////////
            if (command == "left|start")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "left_start", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x01, 0x03, 0xFF });
            }
            if (command == "left|stop")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "left_stop", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x03, 0x03, 0xFF });
            }
            /////////////////////////////
            if (command == "right|start")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "right_start", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x02, 0x03, 0xFF });
            }
            if (command == "right|stop")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "right_stop", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x03, 0x03, 0xFF });
            }
            /////////////////////////////
            if (command == "leftup|start")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "leftup_start", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x01, 0x01, 0xFF });
            }
            if (command == "leftup|stop")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "leftup_stop", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x03, 0x03, 0xFF });
            }
            /////////////////////////////
            if (command == "rightup|start")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "rightup_start", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x01, 0x01, 0xFF });
            }
            if (command == "rightup|stop")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "rightup_stop", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x03, 0x01, 0xFF });
            }
            /////////////////////////////
            if (command == "leftdown|start")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "leftdown_start", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x01, 0x02, 0xFF });
            }
            if (command == "leftdown|stop")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "leftdown_stop", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x03, 0x03, 0xFF });
            }
            /////////////////////////////
            if (command == "rightdown|start")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "rightdown_start", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x02, 0x02, 0xFF });
            }
            if (command == "rightdown|stop")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "rightdown_stop", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x06, 0x01, 0x0C, 0x0A, 0x03, 0x01, 0xFF });
            }
            /////////////////////////////
            if (command == "preset|recall")
            {
                if (camToControl.controlType == CameraControlType.http) CameraHTTPControlCommand(camToControl, "preset_call", presetNum);
                if (camToControl.controlType == CameraControlType.viscaIP) camToControl.ipComms.SendByteMessage(new byte[] { 0x81, 0x01, 0x04, 0x3F, 0x02, byte.Parse(presetNum), 0xFF });
            }
        }

        public void CameraHTTPControlCommand(Camera cam, string command, string byValue)
        {
            Task.Run(() =>
            {
                try
                {
                    string commandURL =
                        "http://" + cam.ip + "/ajaxcom?szCmd={\"SysCtrl\":{\"PtzCtrl\":{\"nChanel\":0,\"szPtzCmd\":\"" + command + "\",\"byValue\":" + byValue + "}}}";

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(commandURL);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Timeout = 100;

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        ConsoleLogger.WriteLine($"Received Response from {cam.name}: " + result.ToString());
                    }
                }
                catch (Exception ex)
                {
                    if (ControlSystem.debugEnabled)
                        ConsoleLogger.WriteLine("Exception in CameraControlCommand(): " + ex.Message);
                }
            });
        }
        #endregion

        #region Motorized Screens
        void RollDownMotorizedScreens()
        {
            foreach (var screen in _motorizedScreens)
            {
                if (screen.controlType == MotorizedScreenControlType.relay) _cs.ChangeRelayState(screen.relay, screen.screenDownRelayState);
                if (screen.controlType == MotorizedScreenControlType.rs232) _cs.SendSerialData(screen.rs232Port, screen.rs232Commands[1]);
            }
        }

        void RollUpMotorizedScreens()
        {
            foreach (var screen in _motorizedScreens)
            {
                if (screen.controlType == MotorizedScreenControlType.relay) _cs.ChangeRelayState(screen.relay, screen.screenUpRelayState);
                if (screen.controlType == MotorizedScreenControlType.rs232) _cs.SendSerialData(screen.rs232Port, screen.rs232Commands[0]);
            }
        }
        #endregion

        #region Display Control
        void PowerOnDisplays()
        {
            foreach (var display in _roomViewDisplays)
            {
                if (_dco == DisplayControlOption.Both) display.connectedDisplay.PowerOn();

                else if (_dco == DisplayControlOption.TVOnly)
                { if (display.displayType == DisplayType.TV) display.connectedDisplay.PowerOn(); }

                else if (_dco == DisplayControlOption.ProjectorOnly)
                    if (display.displayType == DisplayType.Projector) display.connectedDisplay.PowerOn();
            }

            foreach (var display in _cecDisplays) 
            {
                if (_dco == DisplayControlOption.Both) SendCECCommand(display.nvxRXConnected, DisplayCommand.PowerOn);

                else if (_dco == DisplayControlOption.TVOnly)
                { if (display.displayType == DisplayType.TV) SendCECCommand(display.nvxRXConnected, DisplayCommand.PowerOn); }

                else if (_dco == DisplayControlOption.ProjectorOnly)
                    if (display.displayType == DisplayType.Projector) SendCECCommand(display.nvxRXConnected, DisplayCommand.PowerOn);
            }

            foreach (var display in _rs232Displays) 
            {
                if (_dco == DisplayControlOption.Both) SendRS232Command(display.rs232Commands, display.nvxRXConnected, DisplayCommand.PowerOn);

                else if (_dco == DisplayControlOption.TVOnly)
                { if (display.displayType == DisplayType.TV) SendRS232Command(display.rs232Commands, display.nvxRXConnected, DisplayCommand.PowerOn); }

                else if (_dco == DisplayControlOption.ProjectorOnly)
                    if (display.displayType == DisplayType.Projector) SendRS232Command(display.rs232Commands, display.nvxRXConnected, DisplayCommand.PowerOn);
            }

            if (_dco == DisplayControlOption.Both || _dco == DisplayControlOption.ProjectorOnly) RollDownMotorizedScreens();
        }

        void PowerOffDisplays()
        {
            foreach (var display in _roomViewDisplays)
            {
                ConsoleLogger.WriteLine("roomview: " + display.nvxRXConnected.ToString());
                if (_dco == DisplayControlOption.Both) display.connectedDisplay.PowerOff();

                else if (_dco == DisplayControlOption.TVOnly)
                { if (display.displayType == DisplayType.TV) display.connectedDisplay.PowerOff(); }

                else if (_dco == DisplayControlOption.ProjectorOnly)
                    if (display.displayType == DisplayType.Projector) display.connectedDisplay.PowerOff();
            }

            foreach (var display in _cecDisplays)
            {
                ConsoleLogger.WriteLine("cec: " + display.nvxRXConnected.ToString());
                if (_dco == DisplayControlOption.Both) SendCECCommand(display.nvxRXConnected, DisplayCommand.PowerOff);

                else if (_dco == DisplayControlOption.TVOnly)
                { if (display.displayType == DisplayType.TV) SendCECCommand(display.nvxRXConnected, DisplayCommand.PowerOff); }

                else if (_dco == DisplayControlOption.ProjectorOnly)
                    if (display.displayType == DisplayType.Projector) SendCECCommand(display.nvxRXConnected, DisplayCommand.PowerOff);
            }

            foreach (var display in _rs232Displays)
            {
                ConsoleLogger.WriteLine("rs232: " + display.nvxRXConnected.ToString());
                if (_dco == DisplayControlOption.Both) SendRS232Command(display.rs232Commands, display.nvxRXConnected, DisplayCommand.PowerOff);

                else if (_dco == DisplayControlOption.TVOnly)
                { if (display.displayType == DisplayType.TV) SendRS232Command(display.rs232Commands, display.nvxRXConnected, DisplayCommand.PowerOff); }

                else if (_dco == DisplayControlOption.ProjectorOnly)
                    if (display.displayType == DisplayType.Projector) SendRS232Command(display.rs232Commands, display.nvxRXConnected, DisplayCommand.PowerOff);
            }

            if (_dco == DisplayControlOption.Both || _dco == DisplayControlOption.ProjectorOnly) RollUpMotorizedScreens();
        }

        void SendCECCommand(uint nvxConnected, DisplayCommand command)
        {
            foreach (var receiver in _nvxReceivers)
                if (receiver.ID == nvxConnected)
                {
                    switch(command)
                    {
                        case DisplayCommand.PowerOn:
                            receiver.HdmiOut.StreamCec.Send.StringValue = "\x40\x44\x6D";
                            ConsoleLogger.WriteLine("Sending CEC On");
                            break;
                        case DisplayCommand.PowerOff:
                            receiver.HdmiOut.StreamCec.Send.StringValue = "\x40\x44\x6C";
                            ConsoleLogger.WriteLine("Sending CEC Off");
                            break;
                        case DisplayCommand.HDMI1:
                            receiver.HdmiOut.StreamCec.Send.StringValue = "\x1F\x82\x00\x00";
                            ConsoleLogger.WriteLine("Sending CEC HDMI 1");
                            break;
                        case DisplayCommand.HDMI2:
                            receiver.HdmiOut.StreamCec.Send.StringValue = "\x2F\x82\x00\x00";
                            ConsoleLogger.WriteLine("Sending CEC HDMI 2");
                            break;
                    }
                }
        }

        void SendRS232Command(string[] commands, uint nvxConnected, DisplayCommand command)
        {
            foreach (var receiver in _nvxReceivers)
                if (receiver.ID == nvxConnected)
                {
                    switch(command)
                    {
                        case DisplayCommand.PowerOn:
                            receiver.ComPorts[1].Send(commands[0]);
                            ConsoleLogger.WriteLine($"Sending RS232: {commands[0]}");
                            break;
                        case DisplayCommand.PowerOff:
                            receiver.ComPorts[1].Send(commands[1]);
                            ConsoleLogger.WriteLine($"Sending RS232: {commands[1]}");
                            break;
                        case DisplayCommand.HDMI1:
                            receiver.ComPorts[1].Send(commands[2]);
                            ConsoleLogger.WriteLine($"Sending RS232: {commands[2]}");
                            break;
                        case DisplayCommand.HDMI2:
                            receiver.ComPorts[1].Send(commands[3]);
                            ConsoleLogger.WriteLine($"Sending RS232: {commands[3]}");
                            break;
                    }
                }
        }

        public DisplayControlOption setDisplayControlOption(string newOption)
        {
            DisplayControlOption newControlOption = DisplayControlOption.Both;

            if (newOption == "1") newControlOption = DisplayControlOption.TVOnly;
            if (newOption == "2") newControlOption = DisplayControlOption.ProjectorOnly;

            _dco = newControlOption;
            return _dco;
        }

        public DisplayControlOption getDisplayControlOption() => _dco;

        #endregion

        #region Audio Equipment Control

        //FAKE FB VARIABLES, DELETE WHEN FB CONNECTED TO AUDIO DSP
        bool volMuted = false;
        int volLevel = 0;
        //FAKE FB VARIABLES, DELETE WHEN FB CONNECTED TO AUDIO DSP
        CancellationTokenSource RampingVolTokenSource = new CancellationTokenSource();
        public void ChangeVolumeLevel(string direction, bool state)
        {
            if (!state && RampingVolTokenSource != null) RampingVolTokenSource.Cancel();

            if (state)
            {
                //Cancel Previous Task and start a new one
                if (RampingVolTokenSource != null) RampingVolTokenSource.Cancel();
                RampingVolTokenSource = new CancellationTokenSource();

                Task.Run(() =>
                {
                    _audioProcessor.SubscribeToComponent(_audioProcessorSetup.controlComponents[0][0], _audioProcessorSetup.controlComponents[0][1], "level", "volLevel");

                    bool keepRamping = true;
                    while (keepRamping)
                    {
                        if (direction == "up") { _audioProcessor.SliderLevelUp(_audioProcessorSetup.controlComponents[0][0], _audioProcessorSetup.controlComponents[0][1]); volLevel++; }
                        if (direction == "down") { _audioProcessor.SliderLevelDown(_audioProcessorSetup.controlComponents[0][0], _audioProcessorSetup.controlComponents[0][1]); volLevel--; }
                        Thread.Sleep(_volRampSpeedInMs);

                        if(volLevel > 100) volLevel = 100;
                        if(volLevel < 0) volLevel = 0;

                        _cs.sse.UpdateAllConnected(_id, "VolLevel:"+volLevel);

                        if (RampingVolTokenSource.Token.IsCancellationRequested) { keepRamping = false; }
                    }

                    Thread.Sleep(_volRampSpeedInMs);

                    _audioProcessor.UnsbscribeFromComponent(_audioProcessorSetup.controlComponents[0][0], _audioProcessorSetup.controlComponents[0][1], "level", "volLevel");

                }, RampingVolTokenSource.Token);
            }
        }
        public void MuteVolume()
        {
            _audioProcessor.SubscribeToComponent(_audioProcessorSetup.controlComponents[1][0], _audioProcessorSetup.controlComponents[1][1], "mute", "volMuteState");

            volMuted = !volMuted;
            _audioProcessor.MuteToggle(_audioProcessorSetup.controlComponents[1][0], _audioProcessorSetup.controlComponents[1][1]);
            _cs.sse.UpdateAllConnected(_id, "VolMute:" + volMuted);

            Task.Run(() => { Thread.Sleep(500); _audioProcessor.UnsbscribeFromComponent(_audioProcessorSetup.controlComponents[1][0], _audioProcessorSetup.controlComponents[1][1], "mute", "volMuteState"); });
        }   
        public int GetVolLevel() => volLevel;
        public bool GetVolMuteState() => volMuted;



        //FAKE FB VARIABLES, DELETE WHEN FB CONNECTED TO AUDIO DSP
        bool micMuted = false;
        int micLevel = 0;
        //FAKE FB VARIABLES, DELETE WHEN FB CONNECTED TO AUDIO DSP
        CancellationTokenSource RampingMicTokenSource = new CancellationTokenSource();
        public void ChangeMicLevel(string direction, bool state)
        {
            if (!state && RampingMicTokenSource != null) RampingMicTokenSource.Cancel();

            if (state)
            {
                //Cancel Previous Task and start a new one
                if (RampingMicTokenSource != null) RampingMicTokenSource.Cancel();
                RampingMicTokenSource = new CancellationTokenSource();

                Task.Run(() =>
                {
                    _audioProcessor.SubscribeToComponent(_audioProcessorSetup.controlComponents[2][0], _audioProcessorSetup.controlComponents[2][1], "level", "micLevel");

                    bool keepRamping = true;
                    while (keepRamping)
                    {
                        if (direction == "up") { _audioProcessor.SliderLevelUp(_audioProcessorSetup.controlComponents[2][0], _audioProcessorSetup.controlComponents[2][1]); micLevel++; }
                        if (direction == "down") { _audioProcessor.SliderLevelDown(_audioProcessorSetup.controlComponents[2][0], _audioProcessorSetup.controlComponents[2][1]); micLevel--; }
                        Thread.Sleep(_volRampSpeedInMs);

                        if (micLevel > 100) micLevel = 100;
                        if (micLevel < 0) micLevel = 0;

                        _cs.sse.UpdateAllConnected(_id, "MicLevel:"+micLevel);

                        if (RampingMicTokenSource.Token.IsCancellationRequested) { keepRamping = false; }
                    }

                    Thread.Sleep(_volRampSpeedInMs);

                    _audioProcessor.UnsbscribeFromComponent(_audioProcessorSetup.controlComponents[2][0], _audioProcessorSetup.controlComponents[2][1], "level", "volLevel");

                }, RampingMicTokenSource.Token);
            }
        }
        public void MuteMic()
        {
            _audioProcessor.SubscribeToComponent(_audioProcessorSetup.controlComponents[3][0], _audioProcessorSetup.controlComponents[3][1], "mute", "volMuteState");

            micMuted = !micMuted;
            _audioProcessor.MuteToggle(_audioProcessorSetup.controlComponents[3][0], _audioProcessorSetup.controlComponents[3][1]);
            _cs.sse.UpdateAllConnected(_id, "MicMute:" + micMuted);

            Task.Run(() => { Thread.Sleep(500); _audioProcessor.UnsbscribeFromComponent(_audioProcessorSetup.controlComponents[3][0], _audioProcessorSetup.controlComponents[3][1], "mute", "volMuteState"); });
        }
        public int GetMicLevel() => micLevel;
        public bool GetMicMuteState() => micMuted;

        #endregion

        #region Portable Equipment

        public void AttachPortableVideoInput(AVSource portableTransmitter)
        {
            RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));
            AVSources sources = JsonConvert.DeserializeObject<AVSources>(FileOperations.loadRoomJson(_id, "AVSources"));

            RoomMenuItem newItem = new RoomMenuItem() { menuItemName = portableTransmitter.sourceName, menuItemIcon = "fa-solid fa-network-wired", menuItemPageAssigned = "Video-Input" };
            rci.menuItems.Add(newItem);
            sources.sources.Add(portableTransmitter);

            FileOperations.saveRoomData(_id.ToString(), rci);
            FileOperations.saveRoomSources(_id.ToString(), sources);

            _cs.sse.UpdateAllConnected(_id, "SourceNumber");
        }

        public void RemovePortableVideoInput(AVSource portableTransmitter)
        {
            RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));
            AVSources sources = JsonConvert.DeserializeObject<AVSources>(FileOperations.loadRoomJson(_id, "AVSources"));

            List<RoomMenuItem> itemsToRemove = new List<RoomMenuItem>();
            foreach (var menuItem in rci.menuItems)
                if(menuItem.menuItemName == portableTransmitter.sourceName)
                {
                    itemsToRemove.Add(menuItem);
                }

            foreach (var menuItem in itemsToRemove)
                rci.menuItems.Remove(menuItem);

            foreach (var source in sources.sources)
                if(source.sourceName == portableTransmitter.sourceName)
                {
                    sources.sources.Remove(source);
                    break;
                }

            FileOperations.saveRoomData(_id.ToString(), rci);
            FileOperations.saveRoomSources(_id.ToString(), sources);

            _cs.sse.UpdateAllConnected(_id, "SourceNumber");
        }

        #endregion

        #region Virtual Grouping Section

        public string AssignGroupMaster(GroupMasterRoom masterRoom)
        {
            string masterRoomConnectionStatus = "";
            if (masterRoom.ipAddress != ControlSystem.GetLocalIPAddress())
            {
                masterRoomConnectionStatus = TryToGetMasterSource(masterRoom);
            }
            else
            {
                Room masterRoomFound = _cs.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(masterRoom.roomId));
                AVSource masterSource = JsonConvert.DeserializeObject<AVSource>(masterRoomFound.GetCurrentSource());
                GroupMasterSourceChanged(masterSource);
                masterRoomConnectionStatus = "Success";
            }

            if (masterRoomConnectionStatus == "Success")
                Task.Run(() =>
                {
                    RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));
                    rci.isGrouped = true;
                    FileOperations.saveRoomData(_id.ToString(), rci);
                    _cs.sse.UpdateAllConnected(_id, "GroupMaster");

                    FileOperations.saveMasterRoomInfo(_id, masterRoom);
                });

            return ("Slave: " + masterRoomConnectionStatus);
        }

        string TryToGetMasterSource(GroupMasterRoom masterRoom)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + masterRoom.ipAddress + ":50000/api/GetCurrentSource?" + masterRoom.roomId);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 2500;

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ConsoleLogger.WriteLine(result);
                    AVSource masterSource = JsonConvert.DeserializeObject<AVSource>(result);
                    GroupMasterSourceChanged(masterSource);
                    ConsoleLogger.WriteLine("Received Response from " + masterRoom.ipAddress + ": " + result.ToString());
                }

                return "Success";
            }
            catch (WebException ex)
            {
                ConsoleLogger.WriteLine("WebException Caught: " + ex.Message);
                return ex.Message;
            }
        }

        public void ClearGroupMaster()
        {
            RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));
            rci.isGrouped = false;
            FileOperations.saveRoomData(_id.ToString(), rci);
            _cs.sse.UpdateAllConnected(_id, "GroupMaster");
        }

        public void RestoreRoom()
        {
            ClearGroupMaster();

            Task.Run(() =>
            {
                CoreProcessorInfo cpi = JsonConvert.DeserializeObject<CoreProcessorInfo>(FileOperations.loadCoreInfo("CoreProcessorInfo"));

                try
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + cpi.IPAddress + ":50000/api/BreakRoomFromGroup?" + _id);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Timeout = 5000;

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        ConsoleLogger.WriteLine("Received Response from " + cpi.IPAddress + ": " + result.ToString());
                    }
                }
                catch (WebException ex)
                {
                    ConsoleLogger.WriteLine("WebException Caught: " + ex.Message);
                }
            });
        }

        public void GroupMasterSourceChanged(AVSource newSource)
        {
            if (newSource.sourceType.Contains("V"))
                UpdateReceiversStream(newSource);
        }

        public bool GetRoomMasterStatus() => JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core")).isGrouped;
        public string GetRoomMasterDetails() => FileOperations.loadRoomJson(_id, "MasterRoom");

        #endregion

        #region Physically Divisible Section

        int _masterID = 0;
        bool _wasSlave = false;

        public void DivisionScenarioChanged(DivisionScenario currentDivisionScenario)
        {
            BreakOutOfDivisionMaster();

            foreach (var slaveRoomID in currentDivisionScenario.slaveRoomIDs)
                if (_id == slaveRoomID)
                {
                    _wasSlave = true;
                    _masterID = currentDivisionScenario.masterRoomID;
                    SubscribeToMasterRoomChanges(_masterID);
                }

            _cs.sse.UpdateAllConnected(_id, "DivisionChanged");
        }

        void BreakOutOfDivisionMaster()
        {
            if (_wasSlave)
            {
                _wasSlave = false;
                UnsubscribeFromMasterRoomChanges(_masterID);
            }
        }

        void SubscribeToMasterRoomChanges(int masterID)
        {
            _mgr.rooms[masterID - 1].sourceChanged += MasterRoom_sourceChanged;
        }

        void UnsubscribeFromMasterRoomChanges(int masterID)
        {
            _mgr.rooms[masterID - 1].sourceChanged -= MasterRoom_sourceChanged;
        }

        private void MasterRoom_sourceChanged(AVSource newSource)
        {
            if(newSource.sourceType.Contains("V")) UpdateReceiversStream(newSource);
        }

        #endregion

        #region Climate Control

        public decimal TempUp()
        {
            if (_mgr.bacnetRefreshTask.IsCompleted)
            {
                ClimateControlValues ccv = JsonConvert.DeserializeObject<ClimateControlValues>(FileOperations.loadRoomJson(_id, "ClimateControl"));

                if(ccv.setpoint >= _upperSetpointLimit)
                    ccv.setpoint = _upperSetpointLimit;
                else
                    ccv.setpoint = Math.Round((ccv.setpoint+0.1M), 1);

                FileOperations.saveRoomClimateValues(_id, ccv);

                return ccv.setpoint;
            }

            return -100.0M;
        }
        public decimal TempDown()
        {
            if (_mgr.bacnetRefreshTask.IsCompleted)
            {
                ClimateControlValues ccv = JsonConvert.DeserializeObject<ClimateControlValues>(FileOperations.loadRoomJson(_id, "ClimateControl"));

                if (ccv.setpoint <= _lowerSetpointLimit)
                    ccv.setpoint = _lowerSetpointLimit;
                else
                    ccv.setpoint = Math.Round((ccv.setpoint - 0.1M), 1);

                FileOperations.saveRoomClimateValues(_id, ccv);

                return ccv.setpoint;
            }

            return -100.0M;
        }

        public string GetClimateData()
        {
            while(!_mgr.bacnetRefreshTask.IsCompleted) { }
            return FileOperations.loadRoomJson(_id, "ClimateControl");
        }

        #endregion

        #region Lighting Control

        public void ChangeLightingScene(string newScene)
        {
            RoomLighting roomLighting = JsonConvert.DeserializeObject<RoomLighting>(FileOperations.loadRoomJson(_id, "Lighting"));

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + roomLighting.LightingProcessorIP + ":50000/api/SetNewScene?" + roomLighting.LightingAreaNumber + ":" + newScene);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 3000;

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ConsoleLogger.WriteLine("Received Response from " + roomLighting.LightingProcessorIP + ": " + result.ToString());
                }
            }
            catch (WebException ex)
            {
                ConsoleLogger.WriteLine("Exception in ChangeLightingScene(): \n" + ex.Message);
            }
        }

        #endregion

        public void PrepRoomForMeeting()
        {
            ConsoleLogger.WriteLine($"Prepping Room{_id} for Meeting");
        }

        public void Shutdown()
        {
            ConsoleLogger.WriteLine($"Switching Room{_id} Off, no Meetings within next 10min");

            ChangeSourceSelected("Off");
            Task.Run(() => { ChangeLightingScene("Off"); });

            _cs.sse.UpdateAllConnected(_id, "RoomOff");
        }
    }
}