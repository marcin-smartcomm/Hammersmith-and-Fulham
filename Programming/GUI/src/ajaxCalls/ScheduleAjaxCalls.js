function GetBookingsInfoCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/CalendarBookings",
        dataType: "json",
        data: roomID+'',
        success: function (result) {
            if(panelType == "TSW" || panelType == "iPadS")
            {
                //Side-Menu.js
                FillSideMenuBookingsInfo(result);
                //Scheduling-Page.js
                FillSchedulingPageInfo(result);
            }
            if(panelType == "iPadM")
            {
                //Side-Menu.js
                FillSideMenuBookingsInfo(result);
            }
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function GetMeetingDurationsCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/MeetingDurations",
        dataType: "json",
        data: roomID+'',
        success: function (result) {
            //Scheduling-Page.js
            if(currentSubpage.includes("Scheduling-Page"))
                DrawMeetingsOnTimeline(result);
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function GetSpecificMeetingInfoCall(roomID, bookingIndex)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/SpecificMeeting",
        dataType: "json",
        data: roomID+":"+bookingIndex,
        success: function (result) {
            //Scheduling-Page.js
            ShowMeetingInfoPopUp(result)
        },
        error: function () {
            console.error("Error in communication")
        }
    });
}

function GetCurrentTimeInfoCall()
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/TimeNow",
        dataType: "json",
        data: '1',
        success: function (result) {},
        error: function () {
            console.log("Error in communication")
        }
    });
}

function NewMeetingRequestCall(roomID, startTime)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/NewMeeting",
        dataType: "json",
        data: roomID + ":" + startTime + '',
        success: function (result) {
            //Scheduling-Page.js
            ShowNewMeetingPopUp(result)
        },
        error: function(xhr){
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
        }
    });
}

function CreateNewMeetingCall(roomID, meetingSubject, startTime, endTime)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/CreateNewMeeting",
        dataType: "json",
        data: roomID + "|" + meetingSubject + "|" + startTime + "|" + endTime,
        success: function () 
        {
            DisplayWaitingSubpage();
        },
        error: function(xhr){
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
        }
    });
}

function EndMeetingRequestCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/EndMeetingRequest",
        dataType: "json",
        data: roomID+'',
        success: function (result) {
            //Scheduling-Page.js
            ShowEndMeetingPopUp(result)
        },
        error: function(xhr){
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
        }
    });
}

function EndMeetingNowCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/EndMeetingNow",
        dataType: "json",
        data: roomID+'',
        success: function (result) {
            //Scheduling-Page.js
            DisplayWaitingSubpage();
        },
        error: function(xhr){
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
        }
    });
}

function ExtendMeetingRequestCall(roomID)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/ExtendMeetingRequest",
        dataType: "json",
        data: roomID+'',
        success: function (result) {
            //Scheduling-Page.js
            ShowExtendMeetingPopUp(result)
        },
        error: function(xhr){
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
        }
    });
}

function ExtendMeetingCall(roomID, extendTime)
{
    $.ajax({
        type: "GET",
        url: "http://"+serverIP+":50000/api/ExtendMeeting",
        dataType: "json",
        data: roomID+'|'+extendTime,
        success: function (result) {
            //Scheduling-Page.js
            DisplayWaitingSubpage()
        },
        error: function(xhr){
            console.log('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
        }
    });
}