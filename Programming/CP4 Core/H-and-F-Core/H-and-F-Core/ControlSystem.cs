using System;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.UI;
using Crestron.SimplSharp.CrestronIO;

namespace H_and_F_Core
{
    public class ControlSystem : CrestronControlSystem
    {
        private static CrestronOne cone;
        public iPadMaster ipadMaster;
        public PortableAVStation protableAVStation;
        public GroupManager groupManager;
        public CollaborationScreensManager colabScreenManager;
        System.Timers.Timer _currentTimeTimer;

        public SSE_Server sse;

        public ControlSystem()
            : base()
        {
            try
            {
                if(this.SupportsEthernet)
                {
                    ConsoleLogger.ConsoleLoggerStart(55555);

                    sse = new SSE_Server(this);
                    WebServer webServer = new WebServer(this, sse);

                    ipadMaster = new iPadMaster();
                    protableAVStation = new PortableAVStation(this);
                    groupManager = new GroupManager(this);
                    colabScreenManager = new CollaborationScreensManager(this);

                    _currentTimeTimer = new System.Timers.Timer();
                    _currentTimeTimer.Interval = 1000;
                    _currentTimeTimer.Elapsed += _currentTimeTimer_Elapsed;
                    _currentTimeTimer.AutoReset = true;

                    _currentTimeTimer.Start();

                    try { CreateCrestronOneApp(0x03); }
                    catch (Exception e) { ErrorLog.Error("Error in ControlSystem Constructor: {0}", e.Message); }
                }
                if(this.SupportsIROut)
                {
                    string manhattanIRPath = string.Format("{0}/user/Manhattan.ir", Directory.GetDirectoryRoot(Directory.GetApplicationDirectory()));
                    if (ControllerIROutputSlot.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                        ConsoleLogger.WriteLine("Problem Registering IR Devices: " + ControllerIROutputSlot.DeviceRegistrationFailureReason);
                    else
                    {
                        IROutputPorts[1].LoadIRDriver(manhattanIRPath);
                        IROutputPorts[2].LoadIRDriver(manhattanIRPath);
                        IROutputPorts[3].LoadIRDriver(manhattanIRPath);
                        IROutputPorts[4].LoadIRDriver(manhattanIRPath);
                    }
                }

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

        int previousMinute = -1;
        private void _currentTimeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime timeNow = DateTime.Now;

            if (timeNow.Minute == 0 || timeNow.Minute == 15 || timeNow.Minute == 30 || timeNow.Minute == 45)
                if(timeNow.Second == 0)
                    DigitalSignageManager.RunThroughScheduledOffTimes(timeNow.Hour, timeNow.Minute);

            if (previousMinute == -1) previousMinute = timeNow.Minute;
            if (previousMinute < timeNow.Minute || (previousMinute == 59 && timeNow.Minute == 0))
            {
                previousMinute = timeNow.Minute;
                ConsoleLogger.WriteLine("Time Now " + timeNow.Hour + ":" + timeNow.Minute);
            }
        }

        private void CreateCrestronOneApp(uint ipId)
        {
            cone = new CrestronOne(ipId, this);
            // This must match project name on control system exactly
            cone.ParameterProjectName.Value = "H-and-F-Main-GUI";



            cone.Register();
        }

        public override void InitializeSystem()
        {
            try
            {

            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
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

        public void FreeviewButtonPress(int irPort, int btnPress)
        {
            try
            {
                switch (btnPress)
                {
                    case 0: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 1: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 2: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 3: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 4: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 5: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 6: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 7: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 8: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 9: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 10: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 11: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 12: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 13: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 14: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 15: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 16: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 17: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 18: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 19: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 20: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 21: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                    case 22: IROutputPorts[(uint)irPort].PressAndRelease("Button1", 25); break;
                }
            }
            catch (Exception e)
            {
                ConsoleLogger.WriteLine("Error in the FreeviewButtonPress: {0}", e);
            }
        }
    }
}