using Crestron.SimplSharpPro.CrestronConnected;
using Crestron.SimplSharpPro.DM.Streaming;
using Crestron.SimplSharpPro;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace H_and_F_Core
{
    public class PortableAVStation
    {
        ControlSystem _cs;

        //Video
        RoomViewConnectedDisplay[] _displays;
        DmNvx360  _transmitter;
        DmNvxD30[] _receivers;

        public PortableAVStation(ControlSystem cs)
        {
            _cs = cs;

            InitializePortableAVStation();
        }

        void InitializePortableAVStation()
        {
            try
            {
                PortableAVVideoReceivers receivers = JsonConvert.DeserializeObject<PortableAVVideoReceivers>(FileOperations.loadJson("PortableEquipment/VideoReceivers"));
                PortableTVs TVs = JsonConvert.DeserializeObject<PortableTVs>(FileOperations.loadJson("PortableEquipment/TVs"));
                PortableTransmitter transmitter = JsonConvert.DeserializeObject<PortableTransmitter>(FileOperations.loadJson("PortableEquipment/Transmitter"));

                if (TVs.tvs.Count > 0)
                    _displays = new RoomViewConnectedDisplay[TVs.tvs.Count];
                if (receivers.receivers.Count > 0)
                    _receivers = new DmNvxD30[receivers.receivers.Count];

                if (_displays.Length > 0)
                {
                    for (int i = 0; i < _displays.Length; i++)
                    {
                        _displays[i] = new RoomViewConnectedDisplay(TVs.tvs[i].IPID, _cs);
                        _displays[i].Description = TVs.tvs[i].TVName;
                        _displays[i].Register();
                        _displays[i].OnlineStatusChange += _display_onlineStatusChange;
                        ConsoleLogger.WriteLine("Registering Portable TV with IPID: " + TVs.tvs[i].IPID);
                    }
                }

                _transmitter = new DmNvx360(transmitter.IPID, _cs);
                _transmitter.Description = transmitter.transmitterName;
                _transmitter.Register();
                _transmitter.OnlineStatusChange += _trasmitter_onlineStatusChange;
                ConsoleLogger.WriteLine("Registering Portable Tx with IPID: " + transmitter.IPID);

                if (_receivers.Length > 0)
                {
                    for (int i = 0; i < _receivers.Length; i++)
                    {
                        _receivers[i] = new DmNvxD30(receivers.receivers[i].IPID, _cs);
                        _receivers[i].Description = receivers.receivers[i].receiverName;
                        _receivers[i].Register();
                        _receivers[i].OnlineStatusChange += _receiver_onlineStatusChange;
                        ConsoleLogger.WriteLine("Registering Portable Rx with IPID: " + receivers.receivers[i].IPID);
                    }

                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in InitializeRoomAV() while registering AV Equipment: " + ex);
            }
        }

        private void _receiver_onlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            ConsoleLogger.WriteLine($"Portable Receiver {currentDevice.ID}: " + args.DeviceOnLine);
        }

        private void _trasmitter_onlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            ConsoleLogger.WriteLine($"Portable Transmitter {currentDevice.ID}: " + args.DeviceOnLine);
        }

        private void _display_onlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            ConsoleLogger.WriteLine($"Portable Display {currentDevice.ID}: " + args.DeviceOnLine);
        }

        public void AssignVideoEquipmentToRoom(string allOrSpecificPart, string serverIP, string roomID)
        {
            ConsoleLogger.WriteLine("All Or Specific Part: " + allOrSpecificPart);
            PortableAVVideoReceivers receivers = JsonConvert.DeserializeObject<PortableAVVideoReceivers>(FileOperations.loadJson("PortableEquipment/VideoReceivers"));
            PortableTVs TVs = JsonConvert.DeserializeObject<PortableTVs>(FileOperations.loadJson("PortableEquipment/TVs"));

            if (allOrSpecificPart == "All")
            {
                foreach(PortableAVVideoReceiver receiver in receivers.receivers)
                {
                    receiver.currentRoomServerAttached = serverIP;
                    receiver.currentRoomIDAttached = roomID;
                }
                foreach (PortableTV tv in TVs.tvs)
                {
                    tv.currentRoomServerAttached = serverIP;
                    tv.currentRoomIDAttached = roomID;
                }

                AssignVideoInputToRoom(serverIP, roomID);
            }
            else
            {
                PortableTV tv = TVs.tvs.Find(x => x.TVName == allOrSpecificPart);
                tv.currentRoomServerAttached = serverIP;
                tv.currentRoomIDAttached = roomID;

                foreach (PortableAVVideoReceiver receiver in receivers.receivers)
                {
                    if (receiver.receiverName.Replace("Receiver", "Display") == tv.TVName)
                    {
                        receiver.currentRoomServerAttached = serverIP;
                        receiver.currentRoomIDAttached = roomID;
                    }
                }

                if (allOrSpecificPart == "Video Input") AssignVideoInputToRoom(serverIP, roomID);
            }

            FileOperations.saveVideoReceivers(receivers);
            FileOperations.saveTVs(TVs);

            GetCurrentSource(serverIP, roomID);
        }

        public void AssignVideoInputToRoom(string serverIP, string roomID)
        {
            PortableTransmitter transmitter = JsonConvert.DeserializeObject<PortableTransmitter>(FileOperations.loadJson("PortableEquipment/Transmitter"));

            AVSource videoInputSource = new AVSource();
            videoInputSource.sourceName = transmitter.transmitterName;
            videoInputSource.sourceType = transmitter.transmitterType;
            videoInputSource.sourceStreamAddress = transmitter.transmitterStreamAddress;

            RemoveVideoInputFromCurrentlyAssignedRoom(videoInputSource);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + serverIP + ":50000/api/AttachVideoInput?" + roomID);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 5000;

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(videoInputSource);
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ConsoleLogger.WriteLine("Received Response from " + serverIP + ": " + result.ToString());
                }

                transmitter.currentRoomServerAttached = serverIP;
                transmitter.currentRoomIDAttached = roomID;

                FileOperations.saveTransmitter(transmitter);
            }catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Problem during video Tx update: " + ex);
            }
        }

        public void RemoveVideoInputFromCurrentlyAssignedRoom(AVSource source)
        {
            PortableTransmitter transmitter = JsonConvert.DeserializeObject<PortableTransmitter>(FileOperations.loadJson("PortableEquipment/Transmitter"));

            ConsoleLogger.WriteLine("Removing Transmitter from Room: " + transmitter.currentRoomServerAttached + ":" + transmitter.currentRoomIDAttached);

            if (transmitter.currentRoomServerAttached == "") return;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + transmitter.currentRoomServerAttached + ":50000/api/RemoveVideoInput?" + transmitter.currentRoomIDAttached);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 5000;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(source);
                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                ConsoleLogger.WriteLine("Remove Request Response from " + transmitter.currentRoomServerAttached + ": " + result.ToString());
            }
        }

        public void SourceChanged(string serverIP, string roomID, AVSource newSource)
        {
            PortableAVVideoReceivers receivers = JsonConvert.DeserializeObject<PortableAVVideoReceivers>(FileOperations.loadJson("PortableEquipment/VideoReceivers"));
            PortableTVs TVs = JsonConvert.DeserializeObject<PortableTVs>(FileOperations.loadJson("PortableEquipment/TVs"));

            foreach (var receiver in receivers.receivers)
            {
                if (receiver.currentRoomServerAttached == serverIP && receiver.currentRoomIDAttached == roomID)
                {
                    foreach (var nvx in _receivers)
                    {
                        if (nvx.ID == receiver.IPID)
                        {
                            ConsoleLogger.WriteLine(receiver.receiverName + " changing stream to: " + newSource.sourceStreamAddress);
                            nvx.Control.ServerUrl.StringValue = newSource.sourceStreamAddress;
                        }
                    }
                }
            }

            foreach (var tv in TVs.tvs)
                if (tv.currentRoomServerAttached == serverIP && tv.currentRoomIDAttached == roomID)
                    foreach (var display in _displays)
                        if (display.ID == tv.IPID)
                        {
                            if(newSource.sourceName == "Off" || !newSource.sourceType.Contains("V"))
                            {
                                ConsoleLogger.WriteLine(tv.TVName + "Changing State to Off");
                                display.PowerOff();
                            }
                            else
                            {
                                ConsoleLogger.WriteLine(tv.TVName + " Changing State to On");
                                display.PowerOn();
                            }
                        }
        }

        public void GetCurrentSource(string serverIP, string roomID)
        {
            Task.Run(() =>
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + serverIP + ":50000/api/GetCuurentSource?"+roomID);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 5000;

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    ConsoleLogger.WriteLine("Received Response from " + serverIP + ": " + result.ToString());
                    SourceChanged(serverIP, roomID, JsonConvert.DeserializeObject<AVSource>(result));
                }
            });
        }
    }
}