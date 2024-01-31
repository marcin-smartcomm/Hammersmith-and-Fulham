using System.Net;
using System.Text;
using System;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;

namespace H_and_F_Core
{
    class WebServer
    {
        ControlSystem _controlSystem;
        SSE_Server _eventServer;
        Task changingGlobalTemp;

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
            bool standardResponseCode = true;

            try
            {
                //Respond to Request
                string response = "";
                string incomingRequest = context.Request.RawUrl;
                ConsoleLogger.WriteLine("Request Coming on " + context.Request.RawUrl + " || from: " + context.Request.RemoteEndPoint.Address.ToString());

                if(incomingRequest.Contains("/PanelInfo"))
                {
                    PanelInfoList panelInfoList = JsonConvert.DeserializeObject<PanelInfoList>(FileOperations.loadJson("panelSettings"));

                    string reqIPAddress = context.Request.RemoteEndPoint.Address.ToString();

                    bool ipFound = false;

                    foreach (var panelInfo in panelInfoList.panels)
                        if(panelInfo.panelIP == reqIPAddress)
                        {
                            response = JsonConvert.SerializeObject(panelInfo);
                            ipFound = true;
                        }

                    if(!ipFound)
                        foreach (var panelInfo in panelInfoList.panels)
                            if (panelInfo.panelIP == "0.0.0.0")
                                response = JsonConvert.SerializeObject(panelInfo);
                }
                if(incomingRequest.Contains("/ChangeDefaulPanelType"))
                {
                    string newType = incomingRequest.Split('?')[1];

                    PanelInfoList panelInfoList = JsonConvert.DeserializeObject<PanelInfoList>(FileOperations.loadJson("panelSettings"));
                    PanelInfo panelToChnge = null;

                    foreach (var panelInfo in panelInfoList.panels)
                        if (panelInfo.panelIP == "0.0.0.0")
                        {
                            panelInfo.panelType = newType;
                            panelToChnge = panelInfo;
                        }

                    FileOperations.savePanelInfo(panelInfoList);

                    response = JsonConvert.SerializeObject(panelToChnge);
                }

                else if (incomingRequest.Contains("/RoomData"))
                {
                    string processorIP = _controlSystem.ipadMaster.connectedRoomProcessorIP, roomID = _controlSystem.ipadMaster.roomID;
                    response = "{ \"roomID\": \"" + roomID + "\", \"connectedRoomProcessorIP\": \"" + processorIP + "\"}";
                }
                else if (incomingRequest.Contains("/RoomChange"))
                {
                    string controllerIP = incomingRequest.Split('?')[1].Split(':')[0];
                    string assignedRoomID = incomingRequest.Split('?')[1].Split(':')[1];

                    _controlSystem.ipadMaster.connectedRoomProcessorIP = controllerIP;
                    _controlSystem.ipadMaster.roomID = assignedRoomID;

                    response = "{\"request\": \"conplete\"}";
                }
                else if (incomingRequest.Contains("/SlaveRoomChange"))
                {
                    string slaveIP = context.Request.RemoteEndPoint.Address.ToString();
                    string newServerIP = incomingRequest.Split('?')[1].Split(':')[0];
                    string newRoomID = incomingRequest.Split('?')[1].Split(':')[1];

                    PanelInfoList panelInfoList = JsonConvert.DeserializeObject<PanelInfoList>(FileOperations.loadJson("panelSettings"));

                    foreach(var panel in panelInfoList.panels)
                        if(panel.panelIP == slaveIP)
                        {
                            panel.serverIP = newServerIP;
                            panel.roomID = int.Parse(newRoomID);
                            FileOperations.savePanelInfo(panelInfoList);
                        }

                    response = "{\"request\": \"conplete\"}";
                }
                else if (incomingRequest.Contains("/RoamingiPadRoomChange"))
                {
                    string serverIP = incomingRequest.Split('?')[1].Split(':')[0];
                    string roomID = incomingRequest.Split('?')[1].Split(':')[1];
                    string slaveID = incomingRequest.Split('?')[1].Split(':')[2];
                    string roomName = incomingRequest.Split('?')[1].Split(':')[3];

                    PanelInfoList panelInfoList = JsonConvert.DeserializeObject<PanelInfoList>(FileOperations.loadJson("panelSettings"));

                    foreach(var panel in panelInfoList.panels)
                        if(panel.slaveID == int.Parse(slaveID))
                        {
                            panel.serverIP = serverIP;
                            panel.roomID = int.Parse(roomID);
                            panel.roomName = roomName.Replace("%20", " ");
                        }

                    FileOperations.savePanelInfo(panelInfoList);

                    response = "{\"request\": \"conplete\"}";
                }
                else if (incomingRequest.Contains("/SlaveRooms"))
                {
                    PanelInfoList panelInfoList = JsonConvert.DeserializeObject<PanelInfoList>(FileOperations.loadJson("panelSettings"));
                    PanelInfoList listToSend = new PanelInfoList();
                    listToSend.panels = new List<PanelInfo>();

                    foreach(var panel in panelInfoList.panels)
                        if(panel.panelType == "iPadS") listToSend.panels.Add(panel);

                    response = JsonConvert.SerializeObject(listToSend);
                }

                else if (incomingRequest.Contains("/FreeviewBoxes"))
                {
                    response = FileOperations.loadJson("FreeviewBoxes");
                }
                else if (incomingRequest.Contains("/NewFreeviewName"))
                {
                    string freeviewID = incomingRequest.Split('?')[1].Split(':')[0];
                    string newName = incomingRequest.Split('?')[1].Split(':')[1].Replace("%20", " ");

                    FreeviewBoxes freeviewBoxes = JsonConvert.DeserializeObject<FreeviewBoxes>(FileOperations.loadJson("FreeviewBoxes"));

                    foreach (FreeviewBox freeviewBox in freeviewBoxes.boxes)
                        if(freeviewBox.cp4IRPortNum == int.Parse(freeviewID))
                            freeviewBox.boxName = newName;

                    if (FileOperations.saveFreeviewBoxes(freeviewBoxes)) response = "{ \"newFreeviewName\": \"success-"+newName+"\" }";
                    else response = "{ \"newFreeviewName\": \"failed-" + freeviewBoxes.boxes[int.Parse(freeviewID)].boxName + "\" }";
                }
                else if (incomingRequest.Contains("/FreeviewBtnPress"))
                {
                    string freeviewID = incomingRequest.Split('?')[1].Split(':')[0];
                    string btnPress = incomingRequest.Split('?')[1].Split(':')[1];

                    _controlSystem.FreeviewButtonPress(int.Parse(freeviewID), int.Parse(btnPress));

                    response = "{\"request\": \"conplete\"}";
                }

                else if (incomingRequest.Contains("/AssistanceRequest"))
                {
                    string floor = incomingRequest.Split('?')[1].Split('|')[0];
                    string roomName = incomingRequest.Split('?')[1].Split('|')[1].Replace("%20", " ");

                    AssistanceCards allAssistanceCards = JsonConvert.DeserializeObject<AssistanceCards>(FileOperations.loadJson("AssistanceRequests"));

                    if (CheckIfEntryExists(allAssistanceCards, floor, roomName) == null)
                    {
                        AssistanceNeeddCard newAssistanceCard = GetAssistanceCard(floor, roomName, context.Request.RemoteEndPoint.Address.ToString());
                        newAssistanceCard.requestID = allAssistanceCards.cards.Count + 1;

                        allAssistanceCards.cards.Add(newAssistanceCard);

                        FileOperations.saveAssistanceCards(allAssistanceCards);

                        response = "{\"roomAssistanceCall\": \"true\"}";

                        SSE_Server.UpdateAllConnected("AssistanceRequest");
                    }
                    else
                    {
                        response = "{ \"roomAssistanceState\": \"true\", \"assistanceAcknowledged\": \"" + CheckIfEntryExists(allAssistanceCards, floor, roomName).requestAcknowledged + "\"}";
                    }
                }
                else if (incomingRequest.Contains("/RoomAssistanceState"))
                {
                    string floor = incomingRequest.Split('?')[1].Split('|')[0];
                    string roomName = incomingRequest.Split('?')[1].Split('|')[1].Replace("%20", " ");

                    AssistanceCards allAssistanceCards = JsonConvert.DeserializeObject<AssistanceCards>(FileOperations.loadJson("AssistanceRequests"));

                    foreach(var card in allAssistanceCards.cards) 
                        if(card.floor == floor && card.roomName == roomName)
                        {
                            response = "{ \"roomAssistanceState\": \"true\", \"assistanceAcknowledged\": \""+ card.requestAcknowledged +"\"}";
                            break;
                        }else
                            response = "{ \"roomAssistanceState\": \"false\"}";

                    if(allAssistanceCards.cards.Count < 1)
                        response = "{ \"roomAssistanceState\": \"false\"}";
                }
                else if (incomingRequest.Contains("/RoomAssistanceRequests"))
                {
                    response = FileOperations.loadJson("AssistanceRequests");
                }
                else if (incomingRequest.Contains("/AcknowledgeAssistanceRequest"))
                {
                    string entryID = incomingRequest.Split('?')[1];

                    AssistanceCards allAssistanceCards = JsonConvert.DeserializeObject<AssistanceCards>(FileOperations.loadJson("AssistanceRequests"));

                    foreach (var card in allAssistanceCards.cards)
                        if (card.requestID == int.Parse(entryID))
                            card.requestAcknowledged = true;

                    FileOperations.saveAssistanceCards(allAssistanceCards);

                    response = "{ \"requestAcknowledged\": \"true\" }";
                }
                else if (incomingRequest.Contains("/ClearAssistanceRequests"))
                {
                    AssistanceCards allAssistanceCards = JsonConvert.DeserializeObject<AssistanceCards>(FileOperations.loadJson("AssistanceRequests"));

                    bool allRequestsAck = true;
                    foreach (var card in allAssistanceCards.cards)
                        if (!card.requestAcknowledged)
                        {
                            allRequestsAck = false;
                            response = "{ \"allRequestsAck\": \""+ allRequestsAck + "\" }";
                            break;
                        }
                        else
                            response = "{ \"allRequestsAck\": \"true\" }";

                    if(allRequestsAck)
                    {
                        allAssistanceCards.cards.Clear();
                        FileOperations.saveAssistanceCards(allAssistanceCards);
                    }
                }

                else if (incomingRequest.Contains("/ClearSystemAlerts"))
                {
                    SystemAlerts allAlerts = JsonConvert.DeserializeObject<SystemAlerts>(FileOperations.loadJson("SystemAlerts"));

                    allAlerts.cardList.Clear();
                    FileOperations.saveAlertCards(allAlerts);

                    response = "{ \"alertsCleared\": \"true\" }";
                }
                else if (incomingRequest.Contains("/SystemAlerts"))
                {
                    response = FileOperations.loadJson("SystemAlerts");
                }
                else if (incomingRequest.Contains("/SystemAlert"))
                {
                    var data_text = new StreamReader(context.Request.InputStream,
                    context.Request.ContentEncoding).ReadToEnd();

                    try
                    {
                        SystemAlertRequest newAlertRequest = JsonConvert.DeserializeObject<SystemAlertRequest>(data_text);

                        if (newAlertRequest.createAlert == "True")
                        {
                            CreateSystemAlert(newAlertRequest, context.Request.RemoteEndPoint.Address.ToString());
                            response = "{\"systemAlertCreated\": \"true\"}";
                        }
                        else
                        { 
                            ClearSystemAlert(newAlertRequest, context.Request.RemoteEndPoint.Address.ToString());
                            response = "{\"systemAlertCleared\": \"true\"}";
                        }

                        Task.Run(() =>
                        {
                            Thread.Sleep(500);
                            SSE_Server.UpdateAllConnected("AssistanceRequest");
                        });

                    }
                    catch(Exception ex)
                    {
                        ConsoleLogger.WriteLine("Exception in /SystemAlert Call: " + ex);
                    }
                }

                else if (incomingRequest.Contains("/ChngePassRequest"))
                {
                    string oldPass = incomingRequest.Split('?')[1].Split(':')[0];
                    string newPass = incomingRequest.Split('?')[1].Split(':')[1];
                    string newPassConfirm = incomingRequest.Split('?')[1].Split(':')[2];

                    ChangePassResponse rawResponse = new ChangePassResponse();
                    SlaveiPadPass currentPass = JsonConvert.DeserializeObject<SlaveiPadPass>(FileOperations.loadJson("SlaveiPadsPass"));

                    if (oldPass != currentPass.password) rawResponse.oldPassMatch = false;
                    else rawResponse.oldPassMatch = true;

                    if (newPass != newPassConfirm) rawResponse.newPassMatch = false;
                    else rawResponse.newPassMatch = true;

                    if(rawResponse.oldPassMatch && rawResponse.newPassMatch)
                    {
                        currentPass.password = newPass;
                        FileOperations.saveNewPassword(currentPass);
                    }

                    response = JsonConvert.SerializeObject(rawResponse);
                }
                
                else if (incomingRequest.Contains("/ChangePortableEquipmentAssignment"))
                {
                    string allOrSpecificPiece = incomingRequest.Split('?')[1].Split(':')[0].Replace("%20", " ");
                    string serverIPAddress = incomingRequest.Split('?')[1].Split(':')[1];
                    string roomID = incomingRequest.Split('?')[1].Split(':')[2];

                    PortableTVs tvs = JsonConvert.DeserializeObject<PortableTVs>(FileOperations.loadJson("PortableEquipment/TVs"));
                    PortableTransmitter transmitter = JsonConvert.DeserializeObject<PortableTransmitter>(FileOperations.loadJson("PortableEquipment/Transmitter"));

                    Task.Run(() =>
                    {
                        if (allOrSpecificPiece == "All" || allOrSpecificPiece == tvs.tvs[0].TVName || allOrSpecificPiece == tvs.tvs[1].TVName)
                            _controlSystem.protableAVStation.AssignVideoEquipmentToRoom(allOrSpecificPiece, serverIPAddress, roomID);
                        if (allOrSpecificPiece == transmitter.transmitterName)
                            _controlSystem.protableAVStation.AssignVideoInputToRoom(serverIPAddress, roomID);
                    });

                    response = "{ \"requestAcknowledged\": \"true\" }";
                }
                else if (incomingRequest.Contains("/NewSourceSelected"))
                {
                    string clientIP = context.Request.RemoteEndPoint.Address.ToString();
                    string roomID = incomingRequest.Split('?')[1];
                    var newSourceRaw = new StreamReader(context.Request.InputStream,
                    context.Request.ContentEncoding).ReadToEnd();

                    AVSource newSource = JsonConvert.DeserializeObject<AVSource>(newSourceRaw);

                    ConsoleLogger.WriteLine(clientIP + " Room" + roomID + " says: New Source - " + newSource.sourceName);

                    _controlSystem.protableAVStation.SourceChanged(clientIP, roomID, newSource);
                    _controlSystem.groupManager.SourceChanged(clientIP, roomID, newSource);

                    response = "{ \"requestAcknowledged\": \"true\" }";
                }

                else if (incomingRequest.Contains("/GroupRooms"))
                {
                    var data_text = new StreamReader(context.Request.InputStream,
                    context.Request.ContentEncoding).ReadToEnd();

                    ConsoleLogger.WriteLine(data_text);

                    try
                    {
                        GroupedRooms rooms = JsonConvert.DeserializeObject<GroupedRooms>(data_text);
                        _controlSystem.groupManager.CreateNewRoomGroup(rooms);
                    }
                    catch(Exception ex)
                    {
                        ConsoleLogger.WriteLine("Problem parsing grouped rooms: " + ex);
                    }

                    standardResponseCode = false;
                    context.Response.StatusCode = (int)HttpStatusCode.Created;
                    response = "{\"response\": \"success\"}";
                }
                else if (incomingRequest.Contains("/BreakRoomFromGroup"))
                {
                    string clientIP = context.Request.RemoteEndPoint.Address.ToString();
                    string roomID = incomingRequest.Split('?')[1];

                    _controlSystem.groupManager.BreakRoomFromGroup(roomID, clientIP);

                    response = "{\"response\": \"success\"}";
                }

                else if (incomingRequest.Contains("/GetColabScreens"))
                {
                    response = _controlSystem.colabScreenManager.GetColabScreens();
                }
                else if (incomingRequest.Contains("/GetColabSources"))
                {
                    response = _controlSystem.colabScreenManager.GetColabSources();
                }
                else if (incomingRequest.Contains("/SetColabScreenSource"))
                {
                    string receiverIPID = incomingRequest.Split('?')[1].Split(':')[0];
                    string transmitterIPID = incomingRequest.Split('?')[1].Split(':')[1];

                    _controlSystem.colabScreenManager.SetColabScreenSource(int.Parse(receiverIPID), int.Parse(transmitterIPID));

                    response = "{\"response\": \"success\"}";
                }
                else if (incomingRequest.Contains("/SlaveiPadPassCheck"))
                {
                    string passToCheck = incomingRequest.Split('?')[1];

                    SlaveiPadPass currentPass = JsonConvert.DeserializeObject<SlaveiPadPass>(FileOperations.loadJson("SlaveiPadsPass"));

                    if(passToCheck == currentPass.password)
                        response = "{\"password\": \"correct\"}";
                    else
                        response = "{\"password\": \"incorrect\"}";
                }

                else if (incomingRequest.Contains("/SetMasterTemp"))
                {
                    string globalTemp = "";

                    changingGlobalTemp = Task.Run(() =>
                    {
                        string direction = incomingRequest.Split('?')[1];

                        if (direction == "Up") globalTemp = GlobalTempControl.TempUp().ToString();
                        if (direction == "Down") globalTemp = GlobalTempControl.TempDown().ToString();
                    });

                    if (changingGlobalTemp != null)
                        await changingGlobalTemp;

                    response = "{ \"globalTemp\": \"" + globalTemp + "\" }";
                }
                else if (incomingRequest.Contains("/GetMasterTemp"))
                {
                    if(changingGlobalTemp!= null)
                        await changingGlobalTemp;

                    string globalTemp = JsonConvert.DeserializeObject<GlobalTemp>(FileOperations.loadJson("GlobalTemp")).globalTemp.ToString();

                    response = "{ \"globalTemp\": \"" + globalTemp + "\" }";
                }

                else if (incomingRequest.Contains("/DigitalSignage/Power"))
                {
                    string zoneID = incomingRequest.Split('?')[1].Split(':')[0];
                    string newPowerState = incomingRequest.Split('?')[1].Split(':')[1];

                    DigitalSignageManager.Power(int.Parse(zoneID), newPowerState);

                    response = "{ \"Request\": \"Accepted\" }";
                }
                else if (incomingRequest.Contains("/DigitalSignage/ScheduleOffTime"))
                {
                    string boxID = incomingRequest.Split('?')[1].Split(':')[0];
                    string raiseOrLower = incomingRequest.Split('?')[1].Split(':')[1];

                    try
                    {
                        if (raiseOrLower == "up") DigitalSignageManager.ScheduleTimeUp(int.Parse(boxID));
                        if (raiseOrLower == "down") DigitalSignageManager.ScheduleTimeDown(int.Parse(boxID));
                    }
                    catch (Exception e) {
                        ConsoleLogger.WriteLine("Exception Changing Digital Signage Shutdown time: " + e);
                    }

                    response = "{ \"Request\": \"Accepted\" }";
                }
                else if (incomingRequest.Contains("/DigitalSignage"))
                {
                    response = FileOperations.loadJson("/DigitalSignage/zoneAssignment");
                }

                context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(response);
                context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                context.Response.AddHeader("Access-Control-Allow-Methods", "*");
                context.Response.AddHeader("Access-Control-Allow-Headers", "*");
                context.Response.ContentType = "application/json";

                if(standardResponseCode)
                    context.Response.StatusCode = (int)HttpStatusCode.OK;

                using (Stream s = context.Response.OutputStream)
                using (StreamWriter writer = new StreamWriter(s))
                    await writer.WriteAsync(response);
            }
            catch (Exception ex) { ConsoleLogger.WriteLine("Bad Request: " + ex.Message); }
        }

        void CreateSystemAlert(SystemAlertRequest newAlertRequest, string ipAddress)
        {
            SystemAlerts allAlerts = JsonConvert.DeserializeObject<SystemAlerts>(FileOperations.loadJson("SystemAlerts"));

            if(allAlerts.cardList.Count > 0)
                foreach (var card in allAlerts.cardList)
                    if (card.floor == newAlertRequest.floor && card.roomName == newAlertRequest.roomName && card.issue == newAlertRequest.issue)
                        return;

            SystemAlertCard newAlertCard = new SystemAlertCard();

            newAlertCard.alertID = allAlerts.cardList.Count + 1;
            newAlertCard.floor = newAlertRequest.floor;
            newAlertCard.roomName = newAlertRequest.roomName;
            newAlertCard.issue = newAlertRequest.issue;
            newAlertCard.date = GetDateAssistanceFormat();
            newAlertCard.time = GetTimeAssistanceFormat();
            newAlertCard.ipAddress = ipAddress;

            allAlerts.cardList.Add(newAlertCard);

            FileOperations.saveAlertCards(allAlerts);
            SSE_Server.UpdateAllConnected("SystemAlerts");
        }
        void ClearSystemAlert(SystemAlertRequest newAlertRequest, string ipAddress)
        {
            SystemAlerts allAlerts = JsonConvert.DeserializeObject<SystemAlerts>(FileOperations.loadJson("SystemAlerts"));
            SystemAlertCard cardToRemove;

            foreach (var card in allAlerts.cardList)
                if (card.floor == newAlertRequest.floor && card.roomName == newAlertRequest.roomName && card.issue == newAlertRequest.issue)
                {
                    cardToRemove = card;
                    allAlerts.cardList.Remove(cardToRemove);
                    break;
                }

            FileOperations.saveAlertCards(allAlerts);
            SSE_Server.UpdateAllConnected("SystemAlerts");
        }

        AssistanceNeeddCard CheckIfEntryExists(AssistanceCards allAssistanceCards, string floor, string roomName)
        {
            foreach (var card in allAssistanceCards.cards)
                if (card.floor == floor && card.roomName == roomName)
                    return card;

            return null;
        }

        AssistanceNeeddCard GetAssistanceCard(string floor, string roomName, string ipAddress)
        {

            AssistanceNeeddCard newCard = new AssistanceNeeddCard();
            newCard.floor = floor;
            newCard.roomName = roomName;
            newCard.ipAddress = ipAddress;
            newCard.date = GetDateAssistanceFormat();
            newCard.time = GetTimeAssistanceFormat();

            return newCard;
        }

        string GetDateAssistanceFormat()
        {
            DateTime date = DateTime.Now;

            string day, month, year;

            if(date.Day < 10) day = "0" + date.Day;
            else day = "" + date.Day;

            if (date.Month < 10) month = "0" + date.Month;
            else month = "" + date.Month;

            year = date.Year.ToString().Substring(date.Year.ToString().Length - 2);

            return $"{day}/{month}/{year}";
        }

        string GetTimeAssistanceFormat()
        {
            DateTime date = DateTime.Now;

            string hour, minute;

            if (date.Hour < 10) hour = "0" + date.Hour;
            else hour = "" + date.Hour;

            if (date.Minute < 10) minute = "0" + date.Minute;
            else minute = "" + date.Minute;

            return $"{hour}:{minute}";
        }
    }
}