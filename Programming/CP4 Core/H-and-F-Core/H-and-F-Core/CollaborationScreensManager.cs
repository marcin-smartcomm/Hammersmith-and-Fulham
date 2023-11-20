using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronConnected;
using Crestron.SimplSharpPro.DM.Streaming;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace H_and_F_Core
{
    public class ColabVideoReceivers
    {
        public List<ColabVideoReceiver> receivers;
    }
    public class ColabVideoReceiver
    {
        public uint IPID { get; set; }
        public string receiverName { get; set; }
        public string transmitterAssigned { get; set; }
    }
    public class ColabTransmitters
    {
        public List<ColabTransmitter> transmitters;
    }
    public class ColabTransmitter
    {
        public uint IPID { get; set; }
        public string transmitterName { get; set; }
    }
    public class ColabTVs
    {
        public List<ColabTV> tvs;
    }
    public class ColabTV
    {
        public uint IPID { get; set; }
        public string TVName { get; set; }
    }

    public class CollaborationScreensManager
    {
        ControlSystem _cs;

        //Video
        RoomViewConnectedDisplay[] _displays;
        DmNvx360[] _transmitters;
        DmNvxD30[] _receivers;

        public CollaborationScreensManager(ControlSystem cs)
        {
            _cs = cs;
            InitializeCollaborationScreens();
        }

        void InitializeCollaborationScreens()
        {
            try
            {
                ColabVideoReceivers receivers = JsonConvert.DeserializeObject<ColabVideoReceivers>(FileOperations.loadJson("ColabScreens/VideoReceivers"));
                ColabTVs TVs = JsonConvert.DeserializeObject<ColabTVs>(FileOperations.loadJson("ColabScreens/TVs"));
                ColabTransmitters transmitter = JsonConvert.DeserializeObject<ColabTransmitters>(FileOperations.loadJson("ColabScreens/Transmitter"));

                if (TVs.tvs.Count > 0)
                    _displays = new RoomViewConnectedDisplay[TVs.tvs.Count];
                if (receivers.receivers.Count > 0)
                    _receivers = new DmNvxD30[receivers.receivers.Count];
                if (transmitter.transmitters.Count > 0)
                    _transmitters = new DmNvx360[transmitter.transmitters.Count];

                if (_displays.Length > 0)
                {
                    for (int i = 0; i < _displays.Length; i++)
                    {
                        _displays[i] = new RoomViewConnectedDisplay(TVs.tvs[i].IPID, _cs);
                        _displays[i].Description = TVs.tvs[i].TVName;
                        _displays[i].Register();
                        _displays[i].OnlineStatusChange += _display_onlineStatusChange;
                        ConsoleLogger.WriteLine("Registering Colab TV with IPID: " + TVs.tvs[i].IPID);
                    }
                }

                if (_transmitters.Length > 0)
                {
                    for (int i = 0; i < _transmitters.Length; i++)
                    {
                        _transmitters[i] = new DmNvx360(transmitter.transmitters[i].IPID, _cs);
                        _transmitters[i].Description = transmitter.transmitters[i].transmitterName;
                        _transmitters[i].Register();
                        _transmitters[i].OnlineStatusChange += _trasmitter_onlineStatusChange;
                        ConsoleLogger.WriteLine("Registering Colab Tx with IPID: " + transmitter.transmitters[i].IPID);
                    }
                }

                if (_receivers.Length > 0)
                {
                    for (int i = 0; i < _receivers.Length; i++)
                    {
                        _receivers[i] = new DmNvxD30(receivers.receivers[i].IPID, _cs);
                        _receivers[i].Description = receivers.receivers[i].receiverName;
                        _receivers[i].Register();
                        _receivers[i].OnlineStatusChange += _receiver_onlineStatusChange;
                        ConsoleLogger.WriteLine("Registering Colab Rx with IPID: " + receivers.receivers[i].IPID);
                    }

                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in InitializeCollaborationScreens() while registering AV Equipment: " + ex);
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

        public string GetColabScreens() => FileOperations.loadJson("ColabScreens/VideoReceivers");
        public string GetColabSources() => FileOperations.loadJson("ColabScreens/Transmitter");

        public void SetColabScreenSource(int receiverIPID, int sourceIPID)
        {
            foreach(var transmitter in _transmitters)
            {
                ConsoleLogger.WriteLine("Transmitter Name: " + transmitter.Description);
            }

            foreach (var receiver in _receivers)
                if (receiver.ID == receiverIPID)
                    foreach (var transmitter in _transmitters)
                        if (transmitter.ID == sourceIPID)
                        {
                            receiver.Control.ServerUrl.StringValue = transmitter.Control.ServerUrlFeedback.StringValue;
                            ColabVideoReceivers cvr = JsonConvert.DeserializeObject<ColabVideoReceivers>(FileOperations.loadJson("ColabScreens/VideoReceivers"));
                            foreach (var videoReceiver in cvr.receivers)
                                if (videoReceiver.IPID == receiverIPID)
                                {
                                    videoReceiver.transmitterAssigned = transmitter.Description;
                                    FileOperations.saveColabReceiverData(cvr);
                                }
                        }
        }
    }
}
