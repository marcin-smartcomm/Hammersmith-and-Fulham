let adminEventStreamSource = "";

function GetPanelSettingsCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/PanelInfo",
        dataType: "json",
        data: '',
        success: function (result) {
            FillOutPanelSettings(result)
        },
        error: function () {
            console.error("Error in communication")
        },
        timeout: 5000
    });
}

function RoomChangeCall(ipAndRoomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/RoomChange",
        dataType: "json",
        data: ipAndRoomID+'',
        success: function () {
            GetPanelSettingsCall()
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function SlaveRoomChangeCall(ipAndRoomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/SlaveRoomChange",
        dataType: "json",
        data: ipAndRoomID+'',
        success: function () {
            InitializeHomeScreen()
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function getRoomAssignedCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/RoomData",
        dataType: "json",
        data: '',
        success: function (result) {
            if(result.roomID == "NotDefined")
            {
                LoadSideMenu("FloorList")
                ActivateSideMenuBtns()
            }
            else
            {
                serverIP = result.connectedRoomProcessorIP;
                getRoomDataCall(result.roomID)
                GetLightingProcessorInfoCall(result.roomID)
            }
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function GetAvailableFreeviewBoxesCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/FreeviewBoxes",
        dataType: "json",
        data: '',
        success: function (result) {
            if(currentSubpage == "Menu" && panelType == "iPadM") AddFreeviewBtns(result)
            if(currentSubpage == "TV") AddFreeviewBtnsTVPage(result)
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function ChangeFreeviewNameCall(freeviewID, newName)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/NewFreeviewName",
        dataType: "json",
        data: freeviewID+':'+newName,
        success: function (result) {
            if(currentSubpage.includes("Freeview"))
                document.getElementById("pageTopName").innerHTML = `TV: ${result.newFreeviewName.split('-')[1]}`
        },
        error: function(){
            console.error("Error in communication");
        }
    });
}

function FreeviewBtnPressCall(freeviewID, btnNum)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/FreeviewBtnPress",
        dataType: "json",
        data: freeviewID+':'+btnNum,
        success: function () {
        },
        error: function(){
        },
        timeout: 100
    });
}

function SendAssistanceCall(floor, roomName)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/AssistanceRequest",
        dataType: "json",
        data: floor + "|" + roomName+'',
        success: function (result) 
        {
            console.log(assistanceRequired)
            if(result.roomAssistanceCall == "true")
            {
                assistanceRequired = "true"
            
                //SubpageManager.js
                ProcessAssitanceState()
                StartAssistancePoll()
            }
            else openPopUp("Assistance-Coming")
        },
        error: function () {
            openPopUp("Assistance-Request-Failed")
        },
        timeout: 10000
    });
}

function GetRoomAssistanceStateCall(floor, roomName)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/RoomAssistanceState",
        dataType: "json",
        data: floor + "|" + roomName+'',
        success: function (result) {
            assistanceRequired = result.roomAssistanceState;

            //SystemVariables.js
            if(assistanceRequired == "true" && result.assistanceAcknowledged == "False") StartAssistancePoll()
            if(assistanceRequired == "true" && result.assistanceAcknowledged == "True")
            {
                StopAssistancePoll()
                assistanceRequired = "false"
                openPopUp("Assistance-Coming")

                if(document.getElementById("assistanceBtn") !== null)
                {
                    document.getElementById("assistanceBtn").classList.remove("btn-generic-pressed")
                }
            } 
            if(assistanceRequired == "false") StopAssistancePoll()
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function GetAssistanceRequestsCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/RoomAssistanceRequests",
        dataType: "json",
        data: '',
        success: function (result) {

            //Side-Menu.js
            ProcessAdminAssistanceData(result)
            
            //Admin-Assistance.js
            ProcessAdminAssistanceEntries(result)
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function AcknowledgeAssistanceRequestCall(entryID)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/AcknowledgeAssistanceRequest",
        dataType: "json",
        data: entryID+'',
        success: function () {},
        error: function () {
            console.error("Error in communication")
        }
    });
}

function ClearAssistanceRequestsCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/ClearAssistanceRequests",
        dataType: "json",
        data: '',
        success: function (result) {
            console.log(result)

            if(result.allRequestsAck == "true")
            {
                if(document.getElementById("adminAssistanceEntriesSection") !== null)
                    document.getElementById("adminAssistanceEntriesSection").innerHTML = ""

                GetAssistanceRequestsCall()
            }

            if(result.allRequestsAck == "False")
            {
                openPopUp("Acknowledge-requests")
            }
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function GetSystemAlertsCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/SystemAlerts",
        dataType: "json",
        data: '',
        success: function (result) {

            //Side-Menu.js
            ProcessSystemAlertsData(result)
            
            //Admin-Assistance.js
            ProcessSystemAlertsEntries(result)
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function ClearSystemAlertsCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/ClearSystemAlerts",
        dataType: "json",
        data: '',
        success: function (result) {
            //System-Alerts.js
            if(result.alertsCleared == "true")
                ClearSystemAlertsEntries()
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function GetSlaveAssignedRoomsCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/SlaveRooms",
        dataType: "json",
        data: '',
        success: function (result) {
            //iPadM-Roaming-iPads
            AddSlaveRoomsToName(result)
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function RoamingiPadRoomChangeCall(ip_room_slaveID)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/RoamingiPadRoomChange",
        dataType: "json",
        data: ip_room_slaveID+'',
        success: function (result) {
            if(result.request == "conplete")
                openSubpage("iPadM-Roaming-iPads", "Roaming iPads", 'fa-solid fa-tablet-screen-button')
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function SendNewPassRequestCall(oldPass, newPass, newPassConfirm)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/ChngePassRequest",
        dataType: "json",
        data: oldPass+':'+newPass+':'+newPassConfirm,
        success: function (result) {
            ProcessNewPassResponse(result)
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function ChangePortableEquipmentAssignmentCall(portableEquipmentToChange, serverIP, roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/ChangePortableEquipmentAssignment",
        dataType: "json",
        data: portableEquipmentToChange + ':' + serverIP + ':' + roomID,
        success: function (result) {
            console.log(result)
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function SendRoomsToBeGroupedCall(rooms)
{
    $.ajax({
        type: "POST",
        url: "http://"+coreServerIP+":50000/api/GroupRooms",
        dataType: "json",
        data: JSON.stringify({ rooms }),
        success: function () {},
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function GetColabScreensCal()
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/GetColabScreens",
        dataType: "json",
        data: '',
        success: function (result) {
            //Collaboration-Screens-Main.js
            PopulateColabDataOnScreen(result)
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function GetColabSourcesCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/GetColabSources",
        dataType: "json",
        data: '',
        success: function (result) {
            //Collaboration-Screens-Main.js
            PopulateColabSourcesOnScreen(result)
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function SetColabSourceCall(receiverIPID, transmitterIPID)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/SetColabScreenSource",
        dataType: "json",
        data: '' + receiverIPID + ":" + transmitterIPID,
        success: function () {},
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function ChangeGlobalTempCall(direction)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/SetMasterTemp",
        dataType: "json",
        data: "" + direction,
        success: function (result) 
        {
            //iPadM-Global-Temp.js
            UpdateMasterTemp(result.globalTemp)
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function GetGlobalTempCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/GetMasterTemp",
        dataType: "json",
        data: "",
        success: function (result) 
        {
            //iPadM-Global-Temp.js
            UpdateMasterTemp(result.globalTemp)
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function CheckIfPassCorrectCall(pass)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/SlaveiPadPassCheck",
        dataType: "json",
        data: '' + pass,
        success: function (result) {
            //Password-Input.js
            ProcessPasswordCheckResult(result)
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function GetDigitalSignageDataCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/DigitalSignage",
        dataType: "json",
        data: '',
        success: function (result) {
            //Digital-Signage-Control.js
            PopulateDigitalSignageSection(result)
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function DigitalSignagePowerCall(btnID, powerState)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/DigitalSignage/Power",
        dataType: "json",
        data: btnID + ':' + powerState,
        success: function (result) {
            //Digital-Signage-Control
            BlinkDigitalSignagePwrBtn(btnID, powerState)
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function DigitalSignageScheduleTimeCall(btnID, changeDirection)
{
    $.ajax({
        type: "GET",
        url: "http://"+coreServerIP+":50000/api/DigitalSignage/ScheduleOffTime",
        dataType: "json",
        data: btnID + ':' + changeDirection,
        success: function (result) {
            console.log(result)
        },
        error: function (xhr) {
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText)
        }
    });
}

function SubscribeToCoreEvents()
{
    adminEventStreamSource = new EventSource("http://"+coreServerIP+":50001/api/events")

    adminEventStreamSource.onmessage = function(event) {
        console.log(event.data)
        CoreProcessorProcessIncomingEvent(event);
    }
    adminEventStreamSource.onopen = function(event) {
        console.log("Connected to Core Event Stream");
    }
    adminEventStreamSource.onerror = function(event) {
        console.log("connection error")
    }
}

function CoreProcessorProcessIncomingEvent(event)
{
    if(event.data.includes("NEWINFO"))
    {
        if(event.data.includes("AssistanceRequest")) GetAssistanceRequestsCall()
        if(event.data.includes("SystemAlerts")) GetSystemAlertsCall()
        if(event.data.includes("GroupingResults")) UpdateGroupingResults(event.data.replace("NEWINFO:GroupingResults", ""))
        if(event.data.includes("ScheduleTimes")) GetDigitalSignageDataCall();
    }
}