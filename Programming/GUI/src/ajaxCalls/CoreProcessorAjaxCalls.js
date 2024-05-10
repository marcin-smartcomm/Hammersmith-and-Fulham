let adminEventStreamSource = "";

function CoreProcessorAjaxGETCall(endpoint, params)
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
    $.get(`http://${coreServerIP}:50000/api/${endpoint}?${dataToSend}`)
    .done(function(response) {responseJSON = response})
    .fail(function(xhr, status, error) 
    {
        alert(`Can not communicate with ${coreServerIP}`);
        responseJSON = "Error"
    })
    
    console.log(`Core Request: ${endpoint}`);
    console.log(responseJSON);

    return responseJSON;
}

function CoreProcessorAjaxPOSTCall(endpoint, rooms)
{
    $.post(`http://${coreServerIP}:50000/api/${endpoint}`, JSON.stringify({rooms}))
    .fail(function(xhr, status, error) 
    {
        alert(`Can not communicate with ${coreServerIP}`);
    })
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
        if(event.data.includes("AssistanceRequest")) ProcessAssistanceRequests(CoreProcessorAjaxGETCall("RoomAssistanceRequests", []))
        if(event.data.includes("SystemAlerts")) ProcessSystemAlertsResponse(CoreProcessorAjaxGETCall("SystemAlerts", []))
        if(event.data.includes("GroupingResults")) UpdateGroupingResults(event.data.replace("NEWINFO:GroupingResults", ""))
        if(event.data.includes("ScheduleTimes")) PopulateDigitalSignageSection(CoreProcessorAjaxGETCall("DigitalSignage", []))
    }
}