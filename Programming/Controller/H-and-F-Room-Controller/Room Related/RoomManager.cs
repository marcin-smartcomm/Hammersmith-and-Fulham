using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace H_and_F_Room_Controller
{
    public class RoomManager
    {
        ControlSystem _cs;
        DivisionScenario _currentDivisionScenario = new DivisionScenario();

        public Task bacnetRefreshTask;
        List<int> roomIDsWithClimateControl;
        BACnetComms _bacnetComms;
        System.Timers.Timer _bacnetRefreshTimer;
        
        public List<Room> rooms;

        public RoomManager(ControlSystem cs)
        {
            _cs = cs;
            rooms = new List<Room>();

            bool shouldInitialiseBACnet = false;
            roomIDsWithClimateControl = new List<int>();

            foreach(var directory in FileOperations.GetRoomDirectories())
            {
                int roomFolderNamePos = directory.Split('/').Length - 1;
                string roomFolderName = directory.Split('/')[roomFolderNamePos];
                int roomID = int.Parse(roomFolderName.Replace("Room", ""));


                RoomCoreInfo roomData = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(roomID, "Core"));
                rooms.Add(new Room(_cs, roomID, this));

                //Check if Rooms have Temp Control
                foreach (var menuItem in roomData.menuItems)
                    if (menuItem.menuItemName.Contains("Temperature"))
                    {
                        shouldInitialiseBACnet = true;
                        roomIDsWithClimateControl.Add(roomID);
                    }
            }

            if(shouldInitialiseBACnet)
                StartBacnetComms();
        }

        void StartBacnetComms()
        {
            BACnetController bc = JsonConvert.DeserializeObject<BACnetController>(FileOperations.loadCoreInfo("BACnetController"));
            _bacnetComms = new BACnetComms(bc.controllerID);

            _bacnetRefreshTimer = new System.Timers.Timer();
            _bacnetRefreshTimer.Interval = 10000;
            _bacnetRefreshTimer.Elapsed += _bacnetRefreshTimer_Elapsed;
            _bacnetRefreshTimer.AutoReset = true;

            _bacnetRefreshTimer.Start();
        }

        void StopBacnetComms()
        {
            _bacnetRefreshTimer.Stop();
            _bacnetComms = null;
        }

        public async void GlobalTempChanged(GlobalTemp newTemp)
        {
            await bacnetRefreshTask;
            foreach(var roomID in roomIDsWithClimateControl)
            {
                ClimateControlValues ccv = JsonConvert.DeserializeObject<ClimateControlValues>(FileOperations.loadRoomJson(roomID, "ClimateControl"));
                ccv.setpoint = newTemp.globalTemp;

                FileOperations.saveRoomClimateValues(roomID, ccv);

                _cs.sse.UpdateAllConnected(roomID, "Climate");
            }
        }

        private void _bacnetRefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            bacnetRefreshTask = Task.Run(() =>
            {
                try
                {
                    foreach (var roomID in roomIDsWithClimateControl)
                    {
                        ClimateControlValues ccv = JsonConvert.DeserializeObject<ClimateControlValues>(FileOperations.loadRoomJson(roomID, "ClimateControl"));

                        if (ControlSystem.debugEnabled)
                        {
                            ConsoleLogger.WriteLine($"------------------------------------ Climate Control bacnet exercise for Room{roomID} ------------------------------------");
                            //First Write previously saved values
                            ConsoleLogger.WriteLine("Writing Setpoint: " + ccv.setpoint);
                            _bacnetComms.SetNewSetpointValue(ccv.setpoint_AV_ID, ccv.setpoint);
                            ConsoleLogger.WriteLine("Writing Occupancy: " + ccv.occupancy);
                            _bacnetComms.SetNewOccupancyMode(ccv.occupancy_MSV_ID, ccv.occupancy);

                            ccv.currentTemp = _bacnetComms.ReadSpaceTempValue(ccv.currentTemp_AV_ID);
                            ConsoleLogger.WriteLine("Curent Temp: " + ccv.currentTemp);
                            ccv.setpoint = _bacnetComms.ReadSetpointValue(ccv.setpoint_AV_ID);
                            ConsoleLogger.WriteLine("Curent SetPoint: " + ccv.setpoint);
                            ccv.spaceCO2 = _bacnetComms.ReadCo2Value(ccv.spaceCO2_AV_ID);
                            ConsoleLogger.WriteLine("Curent CO2: " + ccv.spaceCO2);
                        }

                        FileOperations.saveRoomClimateValues(roomID, ccv);

                        _cs.sse.UpdateAllConnected(roomID, "Climate");
                    }
                }catch(Exception ex)
                {
                    if (ControlSystem.debugEnabled)
                        ConsoleLogger.WriteLine("Problem in _bacnetRefreshTimer_Elapsed():\n" + ex);
                    _bacnetComms.StopActivity();
                    StopBacnetComms();
                    StartBacnetComms();
                }
            });
        }

        public void WallStateChanged(int portID, bool portState)
        {
            DivisibleInfo di = JsonConvert.DeserializeObject<DivisibleInfo>(FileOperations.loadCoreInfo("DivisibleInfo"));

            bool allWallsOpen = _cs.CheckIfAllWallsOpen(di);
            bool allWallsClosed = _cs.CheckIfAllWallsClosed(di);

            if (allWallsOpen) // Scenario 1
            {
                ConsoleLogger.WriteLine("All Walls Openned");
                ProcessNewScenario(di, 0);
            }
            else if (allWallsClosed) // Scenario 4
            {
                ConsoleLogger.WriteLine("All Walls Closed");
                ProcessNewScenario(di, 3);
            }
            else if (!portState && portID == 1) // Scenario 2
            {
                ConsoleLogger.WriteLine("Scenario 2");
                ProcessNewScenario(di, 1);
            }
            else if (!portState && portID == 2) // Scenario 3
            {
                ConsoleLogger.WriteLine("Scenario 3");
                ProcessNewScenario(di, 2);
            }

            else if (portState && portID == 1) // Scenario 3
            {
                ConsoleLogger.WriteLine("Scenario 3");
                ProcessNewScenario(di, 2);
            }
            else if (portState && portID == 2) // Scenario 2
            {
                ConsoleLogger.WriteLine("Scenario 2");
                ProcessNewScenario(di, 1);
            }
        }

        void ProcessNewScenario(DivisibleInfo di, int scenarioID)
        {
            try
            {
                _currentDivisionScenario = di.divisionScenarios[scenarioID];

                foreach (var roomID in di.actors)
                    rooms[roomID - 1].DivisionScenarioChanged(_currentDivisionScenario);
            }catch(Exception e)
            {
                ConsoleLogger.WriteLine("Exception in RomManager.ProcessNewScenario(): " + e);
            }
        }


        public DivisionScenario GetCurrentDivisionScenario() => _currentDivisionScenario;
    }
}