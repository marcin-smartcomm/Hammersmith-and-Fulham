let eventStreamSource = "";

function getRoomDataCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/RoomData",
        dataType: "json",
        data: roomID + '',
        success: function (result) {
            if(panelType == "TSW" || panelType == "iPadS")
            {
                GetRoomAssistanceStateCall(result.floor, result.roomName)
                currentRoomInfo = result;
                if(eventStreamSource.readyState != 1)
                    SubscribeToRoomEvents(currentRoomInfo.roomID);
            }
            if(panelType == "iPadM")
            {
                if(currentRoomInfo != null) 
                    DisconnectFromEvents()

                currentRoomInfo = result;

                SubscribeToRoomEvents(currentRoomInfo.roomID);
            }

            if(panelType == "iPadM" || panelType == "iPadS") ClearConnectingPopUp()
            if(panelType.includes("iPad")) 
            {
                InitializeHomeScreen()
                GetCurrentSourceCall(result.roomID, true)
            }

            
        },
        error: function (err) {
            console.log(err)
            console.error("Error in communication")
            if(panelType == "iPadM" || panelType == "iPadS")
            {
                ClearConnectingPopUp()

                if(err.readyState === 4)
                    document.getElementById("projectBody").innerHTML = `<span class=\"unable-to-connect\"> Unable to connect to selected zone <br> Reason: Request reached ${serverIP}, but could not be fulfilled </span>`
                if(err.readyState === 0)
                    document.getElementById("projectBody").innerHTML = `<span class=\"unable-to-connect\"> Unable to connect to selected zone <br> Reason: ${serverIP} did not respond within expected time </span>`

                LoadSideMenu("FloorList")
                ActivateSideMenuBtns()
                document.getElementById("backBtn").remove()
            }
        },
        timeout: 3000
    });
}

function UpdateRoomDataCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/RoomData",
        dataType: "json",
        data: roomID + '',
        success: function (result) {
            currentRoomInfo = result;

            if(currentSubpage == "Menu")
                openSubpage("Menu")
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function GetRoomTemperatureDataCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/RoomTemperatures",
        dataType: "json",
        data: roomID + '',
        success: function (result) {
            //Temperature-Control.js
            populateTemperatureControlSp(result)
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function ChangeTempCall(roomID, direction)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/ChangeTemp",
        dataType: "json",
        data: roomID + ':' + direction,
        success: function (result) {
            //Temperature-Control.js
            updateCurrentSetpoint(result)
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function SetNewSourceCall(roomID, sourceName)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/ChangeSouceSelected",
        dataType: "json",
        data: roomID + ':' + sourceName,
        success: function (result) {
            //TV.js
            ProcessFreeviewSourceSelected(result);
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function GetCurrentSourceCall(roomID, changePage)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/GetCurrentSource",
        dataType: "json",
        data: roomID + '',
        success: function (result) {
            //SubpageManager.js
            if(changePage) ChangeSubpageToSelectedSource(result)
            else {
                //Menu.js
                if(currentSubpage == "Menu") 
                    HighlightCurrentlySelectedSource(result)
            }
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function GetFireAlarmStateCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/FireAlarmState",
        dataType: "json",
        data: '',
        success: function (result) {
            if(result.fireAlarmState == "True") ProcessFireAlarmState("1")
            if(result.fireAlarmState == "False") ProcessFireAlarmState("0")
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function NewMenuItemSelectedCall(newItemName)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/NewMenuItemSelected",
        dataType: "json",
        data: currentRoomInfo.roomID+':'+newItemName,
        success: function (result) {
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function ShutdownRoomCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/RoomShutdown",
        dataType: "json",
        data: roomID+'',
        success: function (result) {
            if(result.roomShutDown) 
                openSubpage("Menu")
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function CheckRoomMasterCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/GetGroupMasterStatus",
        dataType: "json",
        data: roomID+'',
        success: function (result) {
            if(result.roomMasterStatus == "True")
            {
                if(document.getElementById("virtuallyGroupedPopUp") == null)
                    openPopUp("Virtually-Grouped")
                GetGroupMasterDetailsCall(currentRoomInfo.roomID)
            }
            if(result.roomMasterStatus == "False")
                if(document.getElementById("virtuallyGroupedPopUp") != null)
                {
                var parentElement = $("#virtuallyGroupedPopUp").parent()
                clearSpecificPopUp("virtuallyGroupedPopUp")
                parentElement.remove()
                }
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function GetGroupMasterDetailsCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/GetGroupMasterDetails",
        dataType: "json",
        data: roomID+'',
        success: function (result) {
            //Virtually-Grouped.js
            FillOutGroupMasterInfo(result);
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function RestoreFromVirtualGroupCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/RestoreFromVirtualGroup",
        dataType: "json",
        data: roomID+'',
        success: function () {
            //Virtually-Grouped.js
            CheckRoomMasterCall(roomID);
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function GetCurrentDivisionScenarioCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/GetDivisionInfo",
        dataType: "json",
        data: '',
        success: function (result) {

            $.each(result.roomIDsNotInPlay, function (i, valueOfElement) { 
                 if(currentRoomInfo.roomID === result.roomIDsNotInPlay[i])
                 {
                    HideGroupedPopUp()
                    return;
                 }
            });

            $.each(result.slaveRoomIDs, function (i, valueOfElement) { 
                 if(currentRoomInfo.roomID === result.slaveRoomIDs[i])
                 {
                    if(document.getElementById("physicallyGroupedPopUp") == null)
                        openPopUp("Physically-Grouped")
                    return;
                 }
            });

            if(currentRoomInfo.roomID === result.masterRoomID) {
                HideGroupedPopUp()
            }

            if(result.masterRoomID === -1) HideGroupedPopUp()

        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function GetLightingProcessorInfoCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/GetLightingInfo",
        dataType: "json",
        data: roomID+'',
        success: function (result) {
            lightingServerIP = result.LightingProcessorIP,
            lightingAreaNumber = result.LightingAreaNumber
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function SetNewLightingSceneCall(newSceneName)
{
    console.log(newSceneName)
    $.ajax({
        type: "GET",
        url: "http://"+lightingServerIP+":50000/api/SetNewScene",
        dataType: "json",
        data: lightingAreaNumber+':'+newSceneName,
        success: function () {
            UpdateLightingSceneFb(newSceneName)
            setTimeout(() => {
                GetCurrentLightingSceneCall()
            }, 2000);
        },
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function GetCurrentLightingSceneCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+lightingServerIP+":50000/api/GetCurrentScene",
        dataType: "json",
        data: lightingAreaNumber+'',
        success: function (result) {
            //Lighting.js
            UpdateLightingSceneFb(result.currentScene)
        },
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function GetCamerasCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/GetCameras",
        dataType: "json",
        data: roomID+'',
        success: function (result) {
            //PTZ-Control.js
            if(currentSubpage == "PTZ-Control") AddCamerasToList(result)

            //PC-Laptop.js
            if(currentSubpage == "PC-Laptop") DeterminePTZControlBtnFunctionality(result)
        },
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function CameraControlCall(roomID, camName, command, byValue)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/CameraControl",
        dataType: "json",
        data: roomID+':'+camName+':'+command+':'+byValue,
        success: function (result) {},
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function VolChangeCall(roomID, direction, state)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/ChangeVolumeLevel",
        dataType: "json",
        data: roomID+':'+direction+':'+state,
        success: function (result) {console.log(result)},
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function MuteVolCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/MuteVolume",
        dataType: "json",
        data: roomID+'',
        success: function (result) {console.log(result)},
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function MicChangeCall(roomID, direction, state)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/ChangeMicLevel",
        dataType: "json",
        data: roomID+':'+direction+':'+state,
        success: function (result) {console.log(result)},
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function MuteMicCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/MuteMic",
        dataType: "json",
        data: roomID+'',
        success: function (result) {console.log(result)},
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function GetSliderLevelCall(roomID, sliderName)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/GetSliderLevel",
        dataType: "json",
        data: roomID+':'+sliderName,
        success: function (result) {
            if(sliderName == 'vol') UpdateVolLevel(result.VolLevel)
            if(sliderName == 'mic') UpdateMicLevel(result.MicLevel)
        },
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function GetMuteStateCall(roomID, settingName)
{
    console.log("http://"+serverIP+":50000/api/GetMuteState")
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/GetMuteState",
        dataType: "json",
        data: roomID+':'+settingName,
        success: function (result) {
            if(settingName == 'vol') UpdateVolMuteState((result.VolMuteState === 'True'))
            if(settingName == 'mic') UpdateMicMuteState((result.MicMuteState === 'True'))
        },
        error: function (err) {
            console.log(err)
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function GetSmallHallDisplayControlOpitonCall()
{
    $.ajax({
        type: "GET",
        url: "http://192.168.1.241:50000/api/GetDisplayControlOption",
        dataType: "json",
        data: '1',
        success: function (result) {
            //Screen-Control.js
            PopulateSmallHallSection(result)
        },
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function SetSmallHallDisplayControlOpitonCall(option)
{
    $.ajax({
        type: "GET",
        url: "http://192.168.1.241:50000/api/SetDisplayControlOption",
        dataType: "json",
        data: `1` + `:` + option,
        success: function (result) {
            //Screen-Control.js
            PopulateSmallHallSection(result)
        },
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function SubscribeToRoomEvents(roomID)
{
    eventStreamSource = new EventSource("http://"+serverIP+":50001/api/events?"+roomID)

    eventStreamSource.onmessage = function(event) {
        ProcessIncomingEvent(event);
    }
    eventStreamSource.onopen = function(event) {
        console.log("Connected to Room Event Stream");
        GetCurrentTimeInfoCall();
    }
    eventStreamSource.onerror = function(event) {
        console.log("connection error")
    }
}

function DisconnectFromEvents()
{
    $.ajax({
        type: "POST",
        url: "http://"+serverIP+":50000/api/Disconnect",
        dataType: "json",
        success: function () 
        {
        },
        error: function(xhr){
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
        }
    });
    eventStreamSource.close();
}

function ProcessIncomingEvent(event)
{
    console.log(event.data)
    if(event.data.includes("NEWINFO"))
    {
        if(event.data.includes("Bookings"))
        {
            if(document.getElementById('sideMenu') != null)
            {
                GetBookingsInfoCall(currentRoomInfo.roomID);
                GetMeetingDurationsCall(currentRoomInfo.roomID);
            }
            if(currentSubpage == "Scheduling-Page")
            {
                //Scheduling-Page.js
                if(processingSchedulingRequest)
                {
                    InitializeSchedulingPageVariables()
                    processingSchedulingRequest = false;
                }
                else
                    UpdateShedulingData()
            }
        }
        if(event.data.includes("FireAlarm")) ProcessFireAlarmState(event.data.split(':')[2])
        if(event.data.includes("SourceNumber")) UpdateRoomDataCall(currentRoomInfo.roomID)
        if(event.data.includes("GroupMaster")) 
            if(panelType == "TSW") CheckRoomMasterCall(currentRoomInfo.roomID)
        if(event.data.includes("DivisionChanged")) 
            if(panelType == "TSW") GetCurrentDivisionScenarioCall()

        if(event.data.includes("Climate"))
            if(currentSubpage == "Temperature-Control")
                GetRoomTemperatureDataCall(currentRoomInfo.roomID)

        if(event.data.includes("VolLevel"))
            UpdateVolLevel(event.data.split(':')[2])

        if(event.data.includes("VolMute"))
            UpdateVolMuteState((event.data.split(':')[2] === 'True'))

        if(event.data.includes("MicLevel"))
            UpdateMicLevel(event.data.split(':')[2])

        if(event.data.includes("MicMute"))
            UpdateMicMuteState((event.data.split(':')[2] === 'True'))

        if(event.data.includes("Source"))
            if(sideMenuCurrentlyDisplayed == "Main")
                GetCurrentSourceCall(currentRoomInfo.roomID, true)

        if(event.data.includes("RoomOff"))
            InitializeHomeScreen()
    }
    if(event.data.includes("TIME"))
    {
        ProcessNewTimeInfo(event)
        if(fireAlarm) GetFireAlarmStateCall()
    }
}

function ProcessFireAlarmState(newState)
{
    if(newState == "1")
    {
        fireAlarm = true;
        openPopUp("Fire-Alarm")
    }
    if(newState == "0")
    {
        fireAlarm = false;

        if(document.getElementById("popUpSection") !== null)
            clearSpecificPopUp("popUpSection")
    }
}

function ProcessNewTimeInfo(timeData)
{
    splitData = timeData.data.split(':');
    currentHour = parseInt(splitData[2]);
    currentMinute = parseInt(splitData[1]);

    if(document.getElementById('hoursAndMinutes') !== null)
        document.getElementById('hoursAndMinutes').innerHTML = splitData[2] + ":"+splitData[1]

    if(document.getElementById('dayAndMonth') !== null)
        document.getElementById('dayAndMonth').innerHTML = splitData[3] + " " + splitData[4] + " " + splitData[5];

    if(currentSubpage == "Scheduling-Page")
    {
        document.getElementById('schedulingSpDateAndTime').innerHTML = splitData[3] + ", " + splitData[5] + " " + splitData[4] + ", " + splitData[6] + " " + splitData[2] + ":" + splitData[1]
    }
}