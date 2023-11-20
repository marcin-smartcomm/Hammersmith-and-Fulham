using Crestron.SimplSharp.CrestronSockets;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronConnected;
using Crestron.SimplSharpPro.DM.Streaming;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace H_and_F_Room_Controller
{
    public class Room
    {
        ControlSystem _cs;
        RoomManager _mgr;
        int _id;

        //Video
        DmNvx360[] _transmitters;
        DmNvxD30[] _videoReceivers;

        List<RoomViewConnectedDisplay> _roomViewDisplays;
        List<TV> _cecDisplays;
        List<TV> _rs232Displays;

        List<Camera> _cameras;

        //Audio
        TCPClient _audioProcessorComms;
        TesiraForte _audioProcessor;

        //Climate
        decimal _upperSetpointLimit = 24.0M;
        decimal _lowerSetpointLimit = 20.0M;

        //Events
        public event Action<AVSource> sourceChanged;

        public Room(ControlSystem cs, int id, RoomManager mgr)
        {
            ConsoleLogger.WriteLine($"--------------------- Initializing Room{_id} ---------------------");

            _cs = cs;
            _id = id;
            _mgr = mgr;

            _cecDisplays = new List<TV>();
            _rs232Displays = new List<TV>();
            _roomViewDisplays = new List<RoomViewConnectedDisplay>();
            _cameras = new List<Camera>();

            InitializeRoomAV();
        }

        void InitializeRoomAV()
        {
            RoomAVData aVData = JsonConvert.DeserializeObject<RoomAVData>(FileOperations.loadRoomJson(_id, "AV"));

            if (aVData.TVs.Length > 0)
            {
                foreach (var tv in aVData.TVs)
                {
                    if (tv.controlType == TVControlType.roomView) _roomViewDisplays.Add(new RoomViewConnectedDisplay(tv.ipid, _cs));
                    if (tv.controlType == TVControlType.cec) _cecDisplays.Add(tv);
                    if (tv.controlType == TVControlType.rs232) _rs232Displays.Add(tv);
                }
                ConsoleLogger.WriteLine($"RoomView Displays: {_roomViewDisplays.Count} || RS232 Displays: {_rs232Displays.Count} || CEC Displays: {_cecDisplays.Count}");
            }
            if (aVData.NVXTxs.Length > 0)
                _transmitters = new DmNvx360[aVData.NVXTxs.Length];
            if (aVData.NVXRxs.Length > 0)
                _videoReceivers = new DmNvxD30[aVData.NVXRxs.Length];
            if (aVData.BiampIP != "")
            {
                _audioProcessorComms = new TCPClient(aVData.BiampIP, aVData.BiampPort, 4096);
                _audioProcessor = new TesiraForte(_cs, _audioProcessorComms, $"Room{_id} Audio Processor");
            }

            foreach (var cam in aVData.Cameras)
                _cameras.Add(cam);

            try
            {
                if (_roomViewDisplays.Count > 0)
                {
                    foreach (var roomViewDisplay in _roomViewDisplays)
                    {
                        roomViewDisplay.Register();
                        roomViewDisplay.OnlineStatusChange += _display_onlineStatusChange;
                    }
                }

                if (_transmitters.Length > 0)
                {
                    for (int i = 0; i < _transmitters.Length; i++)
                    {
                        _transmitters[i] = new DmNvx360(aVData.NVXTxs[i], _cs);
                        _transmitters[i].Register();
                        _transmitters[i].OnlineStatusChange += _trasmitter_onlineStatusChange;
                        ConsoleLogger.WriteLine("Registering Tx with IPID: " + aVData.NVXTxs[i]);
                    }
                }

                if (_videoReceivers.Length > 0)
                {
                    for (int i = 0; i < _videoReceivers.Length; i++)
                    {
                        _videoReceivers[i] = new DmNvxD30(aVData.NVXRxs[i], _cs);
                        _videoReceivers[i].Register();
                        _videoReceivers[i].OnlineStatusChange += _receiver_onlineStatusChange;
                        ConsoleLogger.WriteLine("Registering Rx with IPID: " + aVData.NVXRxs[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in InitializeRoomAV() while registering AV Equipment: " + ex);
            }
        }

        public List<Camera> GetCameras() => _cameras;

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
                ClearSystemAlert("NVX Receiver Offline");
        }

        private void _display_onlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            ConsoleLogger.WriteLine("TV IPID " + currentDevice.ID + " | Online Status: " + args.DeviceOnLine);
            if (!args.DeviceOnLine)
                CreateSystemAlert("TV Offline");
            else
                ClearSystemAlert("NVX Receiver Offline");
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
                    FileOperations.saveRoomData(_id.ToString(), "Core", rci);

                    if (source.sourceType.Contains("V"))
                    {
                        PowerOnDisplays();
                        UpdateReceiverStreams(source);
                    }
                    else
                        PowerOffDisplays();

                    if (source.sourceType.Contains("A"))
                        _audioProcessor.Conncet();

                    UpdateCoreOnNewSource(source);
                    if (sourceChanged != null) sourceChanged(source);
                }
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

        #region Equipment Control

        void UpdateReceiverStreams(AVSource newSource)
        {
            foreach (DmNvxD30 receiver in _videoReceivers)
                receiver.Control.ServerUrl.StringValue = newSource.sourceStreamAddress;
        }

        void PowerOnDisplays()
        {
            foreach (var display in _roomViewDisplays) display.PowerOn();
            foreach (var display in _cecDisplays) SendCECCommand(display.nvxRXConnected, TVCommand.PowerOn);
            foreach (var display in _rs232Displays) SendRS232Command(display.rs232Commands, display.nvxRXConnected, TVCommand.PowerOn);
        }

        void PowerOffDisplays()
        {
            foreach (var display in _roomViewDisplays) display.PowerOff();
            foreach (var display in _cecDisplays) SendCECCommand(display.nvxRXConnected, TVCommand.PowerOff);
            foreach (var display in _rs232Displays) SendRS232Command(display.rs232Commands, display.nvxRXConnected, TVCommand.PowerOff);
        }

        void SendCECCommand(uint nvxConnected, TVCommand command)
        {
            foreach (var receiver in _videoReceivers)
                if (receiver.ID == nvxConnected)
                {
                    if (command == TVCommand.PowerOn) { receiver.HdmiOut.StreamCec.Send.StringValue = "\x40\x44\x6D"; ConsoleLogger.WriteLine("Sending 40 44 6D"); }
                    if (command == TVCommand.PowerOff) { receiver.HdmiOut.StreamCec.Send.StringValue = "\x40\x44\x6C"; ConsoleLogger.WriteLine("Sending 40 44 6C");
                    }
                    if (command == TVCommand.HDMI1) { receiver.HdmiOut.StreamCec.Send.StringValue = "\x1F\x82\x00\x00"; ConsoleLogger.WriteLine("Sending 1F 82 00"); }
                    if (command == TVCommand.HDMI2) { receiver.HdmiOut.StreamCec.Send.StringValue = "\x2F\x82\x00\x00"; ConsoleLogger.WriteLine("Sending 2F 82 00"); }
                }
        }

        void SendRS232Command(string[] commands, uint nvxConnected, TVCommand command)
        {
            foreach (var receiver in _videoReceivers)
                if (receiver.ID == nvxConnected)
                {
                    if (command == TVCommand.PowerOn) {receiver.ComPorts[1].Send(commands[0]); ConsoleLogger.WriteLine($"Sending {commands[0]}"); }
                    if (command == TVCommand.PowerOff) {receiver.ComPorts[1].Send(commands[1]); ConsoleLogger.WriteLine($"Sending {commands[1]}"); }
                    if (command == TVCommand.HDMI1) {receiver.ComPorts[1].Send(commands[2]); ConsoleLogger.WriteLine($"Sending {commands[2]}");}
                    if (command == TVCommand.HDMI2) {receiver.ComPorts[1].Send(commands[3]); ConsoleLogger.WriteLine($"Sending {commands[3]}");}
                }
        }

        public void CameraControlCommand(string camName, string command, string byValue)
        {
            Task.Run(() =>
            {
                try
                {
                    string camIP = "";
                    
                    foreach (var cam in _cameras)
                        if(camName == cam.name) camIP = cam.ip;

                    string commandURL = 
                        "http://"+camIP+"/ajaxcom?szCmd={\"SysCtrl\":{\"PtzCtrl\":{\"nChanel\":0,\"szPtzCmd\":\""+command+"\",\"byValue\":"+byValue+"}}}";

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(commandURL);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Timeout = 100;

                    ConsoleLogger.WriteLine("Sending control command to Camera: " + camName);
                    ConsoleLogger.WriteLine(
                        "http://"+camIP+"/ajaxcom?szCmd={\"SysCtrl\":{\"PtzCtrl\":{\"nChanel\":0,\"szPtzCmd\":\""+command+"\",\"byValue\":"+byValue+"}}}"
                        );

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        ConsoleLogger.WriteLine("Received Response from Camera: " + result.ToString());
                    }
                    ConsoleLogger.WriteLine("Control command sent");
                }catch(Exception ex)
                {
                    ConsoleLogger.WriteLine("Exception in CameraControlCommand()");
                }
            });
        }
        #endregion

        #region Portable Equipment

        public void AttachPortableVideoInput(AVSource portableTransmitter)
        {
            RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));
            AVSources sources = JsonConvert.DeserializeObject<AVSources>(FileOperations.loadRoomJson(_id, "AVSources"));

            RoomMenuItem newItem = new RoomMenuItem() { menuItemName = portableTransmitter.sourceName, menuItemIcon = "fa-solid fa-network-wired" };
            rci.menuItems.Add(newItem);
            sources.sources.Add(portableTransmitter);

            FileOperations.saveRoomData(_id.ToString(), "Core", rci);
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

            FileOperations.saveRoomData(_id.ToString(), "Core", rci);
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
                AVSource masterSource = JsonConvert.DeserializeObject<AVSource>(_cs._roomManager.rooms[int.Parse(masterRoom.roomId) - 1].GetCurrentSource());
                GroupMasterSourceChanged(masterSource);
                masterRoomConnectionStatus = "Success";
            }

            if (masterRoomConnectionStatus == "Success")
                Task.Run(() =>
                {
                    RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core"));
                    rci.isGrouped = true;
                    FileOperations.saveRoomData(_id.ToString(), "Core", rci);
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
            FileOperations.saveRoomData(_id.ToString(), "Core", rci);
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
                UpdateReceiverStreams(newSource);
        }

        public bool GetRoomMasterStatus() => JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(_id, "Core")).isGrouped;
        public string GetRoomMasterDetails() => FileOperations.loadRoomJson(_id, "MasterRoom");

        #endregion

        #region Physically Divisible Section

        int _masterID = 0;
        bool _wasSlave = false;

        public int GetRoomID() => _id;

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
            if(newSource.sourceType.Contains("V")) UpdateReceiverStreams(newSource);
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
    }
}