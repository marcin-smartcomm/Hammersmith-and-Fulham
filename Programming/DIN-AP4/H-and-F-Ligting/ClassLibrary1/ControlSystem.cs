using System;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.DeviceSupport;             // For Generic Device Support
using Crestron.SimplSharpPro.EthernetCommunication;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace H_and_F_Lighting
{
    public class ControlSystem : CrestronControlSystem
    {
        ThreeSeriesTcpIpEthernetIntersystemCommunications _simplWindowsComms;

        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(_ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(_ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(_ControllerEthernetEventHandler);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        public override void InitializeSystem()
        {
            try
            {
                ConsoleLogger cs = new ConsoleLogger();
                cs.ConsoleLoggerStart(55555, this);

                WebServer ws = new WebServer(this);

                _simplWindowsComms = new ThreeSeriesTcpIpEthernetIntersystemCommunications(0x10, "127.0.0.2", this);
                _simplWindowsComms.Register();
                _simplWindowsComms.SigChange += _simplWindowsComms_SigChange;
                _simplWindowsComms.OnlineStatusChange += _simplWindowsComms_OnlineStatusChange;
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        private void _simplWindowsComms_OnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            if(args.DeviceOnLine)
                SendAreaInfo();
        }

        private void _simplWindowsComms_SigChange(BasicTriList currentDevice, SigEventArgs args)
        {
            if (args.Sig.Type == eSigType.Bool)
                if(args.Sig.BoolValue == true)
                    UpdateSceneInFile(args);
        }

        void UpdateSceneInFile(SigEventArgs args)
        {
            try
            {
                AreaList myAreaList = JsonConvert.DeserializeObject<AreaList>(FileOperations.loadAreasInfo());
                ScenesList myScenes = JsonConvert.DeserializeObject<ScenesList>(FileOperations.loadScenes());

                foreach (Area area in myAreaList.areaList)
                    if (area.AreaPresetsDigitalJoinStart <= args.Sig.Number && area.AreaPresetsDigitalJoinStart + 20 >= args.Sig.Number)
                    {
                        uint scenePressed = args.Sig.Number - area.AreaPresetsDigitalJoinStart;
                        foreach(var scene in myScenes.availableScenes)
                            if(scene.sceneNum == scenePressed)
                                area.currentScene = scene.sceneName;
                    }

                FileOperations.saveAreaInfo(myAreaList);
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in UpdateSceneInFile()" + ex);
            }
        }

        public string GetCurrentLightScene(ushort areaNum)
        {
            AreaList myAreaList = JsonConvert.DeserializeObject<AreaList>(FileOperations.loadAreasInfo());
            foreach(var area in myAreaList.areaList)
            {
                ConsoleLogger.WriteLine("Looking for area match: " + areaNum + " - " + area.AreaNum);
                if (area.AreaNum == areaNum)
                    return JsonConvert.SerializeObject(area);
            }

            return "";
        }

        void SendAreaInfo()
        {
            try
            {
                AreaList myAreaList = JsonConvert.DeserializeObject<AreaList>(FileOperations.loadAreasInfo());
                foreach (Area area in myAreaList.areaList)
                    _simplWindowsComms.UShortInput[area.AreaNumAnalogJoin].UShortValue = area.AreaNum;
            }catch(Exception e)
            {
                ConsoleLogger.WriteLine("Exception in SendAreaInfo(): \n" + e);
            }
        }

        Task processingFileTask;
        public async void SetNewScene(string newSceneName, ushort areaNum)
        {
            if (processingFileTask != null)
                await processingFileTask;

            processingFileTask = Task.Run(() =>
            {
                try
                {
                    ScenesList myScenes = JsonConvert.DeserializeObject<ScenesList>(FileOperations.loadScenes());
                    AreaList myAreaList = JsonConvert.DeserializeObject<AreaList>(FileOperations.loadAreasInfo());

                    Scene matchingScene = new Scene();
                    foreach(var scene in myScenes.availableScenes)
                        if(scene.sceneName == newSceneName) matchingScene = scene;
                    Area matchingArea = new Area();
                    foreach (var area in myAreaList.areaList)
                        if (area.AreaNum == areaNum) matchingArea = area;

                    ConsoleLogger.WriteLine("Pulsing Input: " + (matchingArea.AreaPresetsDigitalJoinStart + matchingScene.sceneNum));

                    _simplWindowsComms.BooleanInput[matchingArea.AreaPresetsDigitalJoinStart + matchingScene.sceneNum].Pulse();
                }
                catch (Exception e)
                {
                    ConsoleLogger.WriteLine("Exception in SetNewScene(): \n" + e);
                }
            });
        }

        void _ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {//Determine the event type Link Up or Link Down
                case (eEthernetEventType.LinkDown):
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        //
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