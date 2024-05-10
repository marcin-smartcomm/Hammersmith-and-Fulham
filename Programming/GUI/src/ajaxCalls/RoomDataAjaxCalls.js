let eventStreamSource = "";

function RoomProcessorAjaxGETCall(endpoint, params)
{
    let dataToSend = ''

    if(params.length === 1) dataToSend = params[0];
    else if (params.length > 1)
    {
        $.each(params, function (i, value) { 
            if(i === params.length-1) dataToSend += value
            else dataToSend += value+':'
        });
    }

    var responseJSON = "";
    $.get(`http://${serverIP}:50000/api/${endpoint}?${dataToSend}`)
    .done(function(response) {responseJSON = response})
    .fail(function(xhr, status, error) 
    {
        console.log(xhr + "/" + status + "/" + error);
        responseJSON = "Error"
    })
    
    console.log(`Room Request: ${endpoint}`);
    console.log(responseJSON);

    return responseJSON;
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
        if(event.data.includes("SourceNumber")) 
        {
            var result = RoomProcessorAjaxGETCall("RoomData", [currentRoomInfo.roomID])
            currentRoomInfo = result;
            if(currentSubpage == "Menu") openSubpage("Menu")
        }
        if(event.data.includes("GroupMaster")) 
            if(panelType == "TSW") CheckRoomMasterState(currentRoomInfo.roomID)
        if(event.data.includes("DivisionChanged")) 
            if(panelType == "TSW") GetCurrentDivisionScenario()

        if(event.data.includes("Climate"))
            if(currentSubpage == "Temperature-Control")
                PopulateTemperatureControlSp(
                    RoomProcessorAjaxGETCall("RoomTemperatures", [currentRoomInfo.roomID])
                )

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
                {
                    var result = RoomProcessorAjaxGETCall("GetCurrentSource", [currentRoomInfo.roomID])
                    ChangeSubpageToSelectedSource(result)
                }
    }
    if(event.data.includes("TIME"))
    {
        ProcessNewTimeInfo(event)
        if(fireAlarm) CheckFireAlarm()
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