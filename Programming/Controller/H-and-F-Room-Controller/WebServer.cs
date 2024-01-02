using System.Net;
using System.Text;
using System;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using static H_and_F_Room_Controller.Room;

namespace H_and_F_Room_Controller
{ 
    class WebServer
    {
        ControlSystem _controlSystem;
        SSE_Server _eventServer;

        public WebServer(ControlSystem cs, SSE_Server eventServer)
        {
            try
            {
                _controlSystem = cs;
                _eventServer = eventServer;
                ListenAsync();
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Problem in WebServer Constructor: " + ex.Message);
            }
        }

        public async void ListenAsync()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:50000/api/");
            listener.Start();

            ConsoleLogger.WriteLine("Server Started...");

            while (true)
            {
                try
                {
                    //Await Client Request
                    HttpListenerContext context = await listener.GetContextAsync();
                    await Task.Run(() => ProcessRequestAsync(context));
                }
                catch (HttpListenerException) { break; }
                catch (InvalidOperationException) { break; }
            }

            listener.Stop();
        }

        async void ProcessRequestAsync(HttpListenerContext context)
        {
            try
            {
                //Respond to Request
                string response = "";
                string incomingRequest = context.Request.RawUrl;
                ConsoleLogger.WriteLine("Request Coming on " + context.Request.RawUrl + " || from: " + context.Request.RemoteEndPoint.Address.ToString());

                if (incomingRequest.Contains("/RoomData"))
                {
                    string clientIP = context.Request.RemoteEndPoint.Address.ToString();
                    string roomID = incomingRequest.Split('?')[1];

                    if (roomID.Contains("999"))
                        response = FileOperations.loadRoomJson(GetRoomAssigned(clientIP), "Core");
                    else
                        response = FileOperations.loadRoomJson(Int32.Parse(roomID), "Core");
                }

                #region ScheduleCalls

                else if (incomingRequest.Contains("/CalendarBookings"))
                {
                    string roomID = incomingRequest.Split('?')[1];
                    response = FileOperations.loadRoomJson(Int32.Parse(roomID), "BookingStats");
                }
                else if (incomingRequest.Contains("/MeetingDurations"))
                {
                    string roomID = incomingRequest.Split('?')[1];
                    response = FileOperations.loadRoomJson(Int32.Parse(roomID), "MeetingDurations");
                }
                else if (incomingRequest.Contains("/SpecificMeeting"))
                {
                    string roomID = incomingRequest.Split('?')[1].Split(':')[0];
                    int meetingIndex = Int32.Parse(incomingRequest.Split('?')[1].Split(':')[1]);

                    MeetingInfoCardCollection bookings = JsonConvert.DeserializeObject<MeetingInfoCardCollection>(FileOperations.loadRoomJson(Int32.Parse(roomID), "MeetingInfoCards"));

                    response = JsonConvert.SerializeObject(bookings.meetingInfoCards[meetingIndex]);
                }

                else if (incomingRequest.Contains("/NewMeeting"))
                {
                    string roomID = incomingRequest.Split('?')[1].Split(':')[0];
                    int requestedTime = Int32.Parse(incomingRequest.Split('?')[1].Split(':')[1]);

                    response = _controlSystem.bookingManager.NewMeetingRequest(roomID, requestedTime);
                }
                else if (incomingRequest.Contains("/CreateNewMeeting"))
                {
                    string roomID = incomingRequest.Split('?')[1].Split('|')[0];
                    string meetingSubject = incomingRequest.Split('?')[1].Split('|')[1];
                    string startTime = incomingRequest.Split('?')[1].Split('|')[2];
                    string endTime = incomingRequest.Split('?')[1].Split('|')[3];

                    ConsoleLogger.WriteLine("new meeting request: " + roomID + "|" + meetingSubject + "|" + startTime + "|" + endTime);

                    Task.Run(() =>
                    {
                        _controlSystem.bookingManager.CreateNewMeeting(roomID, meetingSubject, startTime, endTime);
                    });

                    response = "{\"response\": \"Ok\"}";
                }

                else if (incomingRequest.Contains("/EndMeetingRequest"))
                {
                    string roomID = incomingRequest.Split('?')[1];

                    response = "{ \"minutesLeft\": "+JsonConvert.DeserializeObject<CurrentAndNextMeetingInfo>(FileOperations.loadRoomJson(int.Parse(roomID), "BookingStats")).currentMinutesRemaining+"}";
                }
                else if (incomingRequest.Contains("/EndMeetingNow"))
                {
                    string roomID = incomingRequest.Split('?')[1];

                    Task.Run(() =>
                    {
                        _controlSystem.bookingManager.EndCurrentMeeting(roomID);
                    });

                    response = "{\"response\": \"Ok\"}";
                }

                else if (incomingRequest.Contains("/ExtendMeetingRequest"))
                {
                    string roomID = incomingRequest.Split('?')[1];

                    response = _controlSystem.bookingManager.ExtendMeetingRequest(roomID);
                }
                else if (incomingRequest.Contains("/ExtendMeeting"))
                {
                    string roomID = incomingRequest.Split('?')[1].Split('|')[0];
                    string timeRequested = incomingRequest.Split('?')[1].Split('|')[1];

                    Task.Run(() =>
                    {
                        _controlSystem.bookingManager.ExtendCurrentMeeting(roomID, timeRequested);
                    });

                    response = "{\"response\": \"Ok\"}";
                }
                #endregion

                #region ClimateCalls
                else if (incomingRequest.Contains("/RoomTemperatures"))
                {
                    string roomID = incomingRequest.Split('?')[1];

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    response = foundRoom.GetClimateData();
                }
                else if (incomingRequest.Contains("/ChangeTemp"))
                {
                    string roomID = incomingRequest.Split('?')[1].Split(':')[0];
                    string direction = incomingRequest.Split('?')[1].Split(':')[1];

                    decimal newSetpoint = -100.0M;
                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    if (direction == "Up") newSetpoint = foundRoom.TempUp();
                    if (direction == "Down") newSetpoint = foundRoom.TempDown();

                    response = "{ \"setpoint\": " + newSetpoint + " }";
                }
                #endregion

                #region SourceSelection
                
                else if (incomingRequest.Contains("NewMenuItemSelected"))
                {
                    string roomID = incomingRequest.Split('?')[1].Split(':')[0];
                    string itemName = incomingRequest.Split('?')[1].Split(':')[1].Replace("%20", " ");

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    foundRoom.ChangeSourceSelected(itemName);

                    response = "{ \"itemSelectedProcessed\": \"true\" }";
                }
                else if (incomingRequest.Contains("ChangeSouceSelected"))
                {
                    string roomID = incomingRequest.Split('?')[1].Split(':')[0];
                    string sourceName = incomingRequest.Split('?')[1].Split(':')[1].Replace("%20", " ");

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    foundRoom.ChangeSourceSelected(sourceName);

                    response = "{ \"sourceChanged\": \"true\" }";
                }
                else if (incomingRequest.Contains("/GetCurrentSource"))
                {
                    string roomID = incomingRequest.Split('?')[1];

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    response = foundRoom.GetCurrentSource();
                }

                #endregion

                #region Portable Equipment

                else if (incomingRequest.Contains("/AttachVideoInput"))
                {
                    try
                    {
                        string roomID = incomingRequest.Split('?')[1];
                        var rawSource = new StreamReader(context.Request.InputStream,
                        context.Request.ContentEncoding).ReadToEnd();

                        bool transmitterAlreadyAttached = false;
                        AVSources allSources = JsonConvert.DeserializeObject<AVSources>(FileOperations.loadRoomJson(int.Parse(roomID), "AVSources"));
                        AVSource portableTransmitter = JsonConvert.DeserializeObject<AVSource>(rawSource);
                        foreach (var source in allSources.sources)
                            if (source.sourceName == portableTransmitter.sourceName) transmitterAlreadyAttached = true;

                        if (!transmitterAlreadyAttached)
                        {
                            Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                            foundRoom.AttachPortableVideoInput(portableTransmitter);
                        }
                    }
                    catch (Exception ex)
                    {
                        ConsoleLogger.WriteLine("Problem Attaching Video Input: " + ex);
                    }

                    response = "{ \"requestProcessed\": \"true\" }";
                }

                else if (incomingRequest.Contains("/RemoveVideoInput"))
                {
                    string roomID = incomingRequest.Split('?')[1];
                    var rawSource = new StreamReader(context.Request.InputStream,
                    context.Request.ContentEncoding).ReadToEnd();

                    AVSources allSources = JsonConvert.DeserializeObject<AVSources>(FileOperations.loadRoomJson(int.Parse(roomID), "AVSources"));
                    AVSource portableTransmitter = JsonConvert.DeserializeObject<AVSource>(rawSource);
                    foreach (var source in allSources.sources)
                        if (source.sourceName == portableTransmitter.sourceName)
                        {
                            Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                            foundRoom.RemovePortableVideoInput(portableTransmitter);
                        }

                    response = "{ \"requestProcessed\": \"true\" }";
                }

                #endregion

                #region VolumeAndMicControls

                else if (incomingRequest.Contains("ChangeVolumeLevel"))
                {
                    int roomID = int.Parse(incomingRequest.Split('?')[1].Split(':')[0]);
                    string direction = incomingRequest.Split('?')[1].Split(':')[1];
                    bool state = bool.Parse(incomingRequest.Split('?')[1].Split(':')[2]);

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == roomID);
                    foundRoom.ChangeVolumeLevel(direction, state);

                    response = "{ \"CommandProcessed\": \"true\" }";
                }

                else if (incomingRequest.Contains("MuteVolume"))
                {
                    int roomID = int.Parse(incomingRequest.Split('?')[1]);

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == roomID);
                    foundRoom.MuteVolume();

                    response = "{ \"CommandProcessed\": \"true\" }";
                }

                else if (incomingRequest.Contains("ChangeMicLevel"))
                {
                    int roomID = int.Parse(incomingRequest.Split('?')[1].Split(':')[0]);
                    string direction = incomingRequest.Split('?')[1].Split(':')[1];
                    bool state = bool.Parse(incomingRequest.Split('?')[1].Split(':')[2]);

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == roomID);
                    foundRoom.ChangeMicLevel(direction, state);

                    response = "{ \"CommandProcessed\": \"true\" }";
                }

                else if (incomingRequest.Contains("MuteMic"))
                {
                    int roomID = int.Parse(incomingRequest.Split('?')[1]);

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == roomID);
                    foundRoom.MuteMic();

                    response = "{ \"CommandProcessed\": \"true\" }";
                }

                else if (incomingRequest.Contains("GetSliderLevel"))
                {
                    int roomID = int.Parse(incomingRequest.Split('?')[1].Split(':')[0]);
                    string sliderName = incomingRequest.Split('?')[1].Split(':')[1];

                    response = "{ \"CommandProcessed\": \"true\" }";

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == roomID);
                    if (sliderName == "mic") response = "{ \"MicLevel\": \""+ foundRoom.GetMicLevel() +"\" }";
                    if (sliderName == "vol") response = "{ \"VolLevel\": \"" + foundRoom.GetVolLevel() + "\" }";
                }

                else if (incomingRequest.Contains("GetMuteState"))
                {
                    int roomID = int.Parse(incomingRequest.Split('?')[1].Split(':')[0]);
                    string settingName = incomingRequest.Split('?')[1].Split(':')[1];

                    response = "{ \"CommandProcessed\": \"true\" }";

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == roomID);
                    if (settingName == "mic") response = "{ \"MicMuteState\": \"" + foundRoom.GetMicMuteState() + "\" }";
                    if (settingName == "vol") response = "{ \"VolMuteState\": \"" + foundRoom.GetVolMuteState() + "\" }";
                }

                #endregion

                #region Virtual Grouping
                else if (incomingRequest.Contains("/GetGroupMasterStatus"))
                {
                    string roomID = incomingRequest.Split('?')[1];

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    bool roomMasterStatus = foundRoom.GetRoomMasterStatus();

                    response = "{ \"roomMasterStatus\": \"" + roomMasterStatus + "\" }";
                }
                else if (incomingRequest.Contains("/GetGroupMasterDetails"))
                {
                    string roomID = incomingRequest.Split('?')[1];

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    response = foundRoom.GetRoomMasterDetails();
                }
                else if (incomingRequest.Contains("/GroupMasterDetails"))
                {
                    string roomID = incomingRequest.Split('?')[1];
                    var masterRoomDetailsRaw = new StreamReader(context.Request.InputStream,
                    context.Request.ContentEncoding).ReadToEnd();

                    ConsoleLogger.WriteLine(masterRoomDetailsRaw);
                    ConsoleLogger.WriteLine("here 1");
                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    ConsoleLogger.WriteLine("here 2");
                    GroupMasterRoom masterRoom = JsonConvert.DeserializeObject<GroupMasterRoom>(masterRoomDetailsRaw);
                    ConsoleLogger.WriteLine("here 3");
                    string groupingResult = foundRoom.AssignGroupMaster(masterRoom);
                    ConsoleLogger.WriteLine("here 4");

                    ConsoleLogger.WriteLine(masterRoomDetailsRaw);

                    response = "{ \"result\": \""+groupingResult+"\" }";
                }
                else if (incomingRequest.Contains("/MasterSourceChanged"))
                {
                    string roomID = incomingRequest.Split('?')[1];
                    var newSourceRaw = new StreamReader(context.Request.InputStream,
                    context.Request.ContentEncoding).ReadToEnd();

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    foundRoom.GroupMasterSourceChanged(JsonConvert.DeserializeObject<AVSource>(newSourceRaw));

                    response = "{ \"result\": \"Success\" }";
                }
                else if (incomingRequest.Contains("/GroupBreakUp"))
                {
                    string roomID = incomingRequest.Split('?')[1];

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    foundRoom.ClearGroupMaster();

                    response = "{ \"result\": \"Success\" }";
                }
                else if (incomingRequest.Contains("/RestoreFromVirtualGroup"))
                {
                    string roomID = incomingRequest.Split('?')[1];

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    foundRoom.RestoreRoom();

                    response = "{ \"result\": \"Success\" }";
                }
                #endregion

                #region VideoControlCalls

                else if (incomingRequest.Contains("/GetCameras"))
                {
                    string roomID = incomingRequest.Split('?')[1];

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    response = JsonConvert.SerializeObject(foundRoom.GetCameras());
                }
                else if (incomingRequest.Contains("/CameraControl"))
                {
                    string roomID = incomingRequest.Split('?')[1].Split(':')[0];
                    string camName = incomingRequest.Split('?')[1].Split(':')[1].Replace("%20", " ");
                    string command = incomingRequest.Split('?')[1].Split(':')[2];
                    string byValue = incomingRequest.Split('?')[1].Split(':')[3];

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    foundRoom.ProcessCameraControlCommand(camName, command, byValue);

                    response = "{ \"command\": \"processing\" }";
                }
                else if (incomingRequest.Contains("/SetDisplayControlOption"))
                {
                    string roomID = incomingRequest.Split('?')[1].Split(':')[0];
                    string newOption = incomingRequest.Split('?')[1].Split(':')[1];

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    DisplayControlOption result = foundRoom.setDisplayControlOption(newOption);
                    response = "{ \"DisplayControlOption\": \"None\" }";

                    if (result == DisplayControlOption.Both) response = "{ \"DisplayControlOption\": \"Both\" }";
                    if (result == DisplayControlOption.TVOnly) response = "{ \"DisplayControlOption\": \"TV\" }";
                    if (result == DisplayControlOption.ProjectorOnly) response = "{ \"DisplayControlOption\": \"Projector\" }";
                }
                else if (incomingRequest.Contains("/GetDisplayControlOption"))
                {
                    string roomID = incomingRequest.Split('?')[1];

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    DisplayControlOption result = foundRoom.getDisplayControlOption();
                    response = "{ \"DisplayControlOption\": \"None\" }";

                    if (result == DisplayControlOption.Both) response = "{ \"DisplayControlOption\": \"Both\" }";
                    if (result == DisplayControlOption.TVOnly) response = "{ \"DisplayControlOption\": \"TV\" }";
                    if (result == DisplayControlOption.ProjectorOnly) response = "{ \"DisplayControlOption\": \"Projector\" }";
                }
                #endregion

                else if (incomingRequest.Contains("/GetDivisionInfo"))
                {
                    response = JsonConvert.SerializeObject(_controlSystem.roomManager.GetCurrentDivisionScenario());
                }

                else if (incomingRequest.Contains("/GetLightingInfo"))
                {
                    string roomID = incomingRequest.Split('?')[1];
                    response = FileOperations.loadRoomJson(int.Parse(roomID), "Lighting");
                }

                else if (incomingRequest.Contains("/FireAlarmState"))
                {
                    response = "{ \"fireAlarmState\": \"" + _controlSystem.fireAlarmState + "\" }";
                }

                else if (incomingRequest.Contains("/TimeNow"))
                {
                    _eventServer.SendTimeToAllConnected(_controlSystem.dateAndTime);
                    response = "{\"response\": \"Ok\"}";
                }
                else if (incomingRequest.Contains("/Disconnect"))
                {
                    string IP = context.Request.RemoteEndPoint.Address.ToString();
                    _eventServer.DisconnectFromStream(IP);
                    response = $"\"response\": \"Ok\"";
                }

                else if (incomingRequest.Contains("/RoomShutdown"))
                {
                    string roomID = incomingRequest.Split('?')[1];

                    Room foundRoom = _controlSystem.roomManager.rooms.Find(x => x.GetRoomID() == int.Parse(roomID));
                    foundRoom.Shutdown();

                    response = "{\"roomShutDown\": \"true\"}";
                }

                context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(response);
                context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                context.Response.AddHeader("Access-Control-Allow-Methods", "*");
                context.Response.AddHeader("Access-Control-Allow-Headers", "*");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.OK;

                using (Stream s = context.Response.OutputStream)
                using (StreamWriter writer = new StreamWriter(s))
                    await writer.WriteAsync(response);
            }
            catch (Exception ex) { ConsoleLogger.WriteLine("Bad Request: " + ex.Message); }
        }

        int GetRoomAssigned(string TP_IPAddress)
        {
            IPtoRoom ipToRoomData = JsonConvert.DeserializeObject<IPtoRoom>(FileOperations.loadCoreInfo("IPtoRoom"));

            for (int i = 0; i < ipToRoomData.IPAddress.Length; i++)
                if (ipToRoomData.IPAddress[i] == TP_IPAddress)
                    return ipToRoomData.RoomID[i];
            
            return 1;
        }
    }
}