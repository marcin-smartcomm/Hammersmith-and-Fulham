let currentHourToDisplay;
let timelineInitialized = false;

function InitializeSchedulingPageVariables()
{
    document.getElementById('schedulingPageBackBtn').addEventListener('touchstart', function(){
        document.getElementById('schedulingPageBackBtn').classList.add('btn-generic-pressed')
    }, { passive: "true" })
    document.getElementById('schedulingPageBackBtn').addEventListener('touchend', function(){
        timelineInitialized = false;
        InitializeHomeScreen();
        PlayBtnClickSound()
    })
    GetBookingsInfoCall(currentRoomInfo.roomID);
    GetMeetingDurationsCall(currentRoomInfo.roomID);
    GetCurrentTimeInfoCall()

    currentHourToDisplay = currentHour + 3;
    ScrollTimelineToView(currentHourToDisplay)
    DetermineTimeIndicatorPosiiton();
}  

function DetermineTimeIndicatorPosiiton()
{
    document.getElementById("timelineTimeIndicator").style.left = (((currentHour*60)+currentMinute) * 5.33) + "px"
}

function FillSchedulingPageInfo(result)
{
    if(document.getElementById("meetingPopUpSp") != null)
        if(document.getElementById("meetingInfoSectionContainer") == null)
        document.getElementById("meetingPopUpSp").remove();

    CheckIfMeetingEndingSoon(result)

    if(sideMenuVis) return;

    document.getElementById('schedulingSpMeetingRoomName').innerHTML = currentRoomInfo.roomName;

    if(result.inMeeting) 
    {
        LoadMiddleSection("Reserved")
        InitializeReservedSection(result)
        document.getElementById("timelineTimeIndicator").style.borderColor = "blue"
    }
    if(!result.inMeeting && !result.freeAllDay) 
    {
        LoadMiddleSection("Available-Until")
        InitializeAvailableUntilSection(result);
        document.getElementById("timelineTimeIndicator").style.borderColor = "green"
    }
    if(!result.inMeeting && result.freeAllDay)
    {
        LoadMiddleSection("Available-All-Day")
        InitializeQuickAddBtn()
        document.getElementById("timelineTimeIndicator").style.borderColor = "green"
    }

    ShowHideExtraBtns(result);
}

function LoadMiddleSection(type)
{
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './pages/Scheduling-Page/Scheduling-Info-'+type+'.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var allText = rawFile.responseText;
                document.getElementById('schedulingSpMainSectionAvailabilityInfo').innerHTML = allText;
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;
}

function InitializeReservedSection(result)
{
    if(result.currentMeetingSubject.length > 20)
        result.currentMeetingSubject = result.currentMeetingSubject.slice(0,20) + "..."

    if(result.currentHoursRemaining <= 0)
        document.getElementById('hourTimeBlock').style.display = "none"
    else
    {
        document.getElementById('hourTimeBlock').style.display = "block"
        document.getElementById('hourTimeValue').innerHTML = result.currentHoursRemaining
    }
    
    document.getElementById('minuteTimeValue').innerHTML = result.currentMinutesRemaining

    document.getElementById('meetingSubjectValue').innerHTML = result.currentMeetingSubject

    document.getElementById('meetingOrganiserValue').innerHTML = result.currentMeetingOrganiser

    document.getElementById('meetingDurationValue').innerHTML = result.currentMeetingStartEndTime
}

function InitializeAvailableUntilSection(result)
{
    if(result.hoursUntilNextMeeting <= 0)
        document.getElementById('hourTimeBlock').style.display = "none"
    else
    {
        document.getElementById('hourTimeBlock').style.display = "block"
        document.getElementById('hourTimeValue').innerHTML = result.hoursUntilNextMeeting
    }
    
    document.getElementById('minuteTimeValue').innerHTML = result.minutesUntilNextMeeting

    InitializeQuickAddBtn()
}

function InitializeQuickAddBtn()
{
    let quickAddBtn = document.getElementById("newMeetingQuickAddBtn")
    quickAddBtn.addEventListener('touchstart', function() {
        quickAddBtn.classList.add('selected-grey-block')
    }, {passive: "true"})
    quickAddBtn.addEventListener('touchend', function() {
        quickAddBtn.classList.remove('selected-grey-block')

        let startTime = currentHour*60 + currentMinute;

        NewMeetingRequestCall(currentRoomInfo.roomID, startTime);
        timelineInitialized = false;
        PlayBtnClickSound()
    })
}

function ShowHideExtraBtns(meetingInfo)
{
    if(!document.getElementById("schedulingEndMeetingBtn").classList.contains('non-vis')) return;

    if(meetingInfo.inMeeting && parseInt(meetingInfo.currentHoursRemaining) == 0 && parseInt(meetingInfo.currentMinutesRemaining) <= 15)
    {
        document.getElementById("schedulingEndMeetingBtn").classList.remove("non-vis")
        document.getElementById("schedulingExtendMeetingBtn").classList.remove("non-vis")

        ActivateExtraBtns();
    }
    else
    {
        HideExtraBtns()
    }
}

function HideExtraBtns()
{
    document.getElementById("schedulingEndMeetingBtn").classList.add("non-vis")
    document.getElementById("schedulingExtendMeetingBtn").classList.add("non-vis")
}

function ActivateExtraBtns()
{
    let endBtn = document.getElementById("schedulingEndMeetingBtn")
    endBtn.addEventListener('touchstart', function(){
        endBtn.classList.add('selected-grey-block')
    }, { passive: "true" })
    endBtn.addEventListener('touchend', function(){
        endBtn.classList.remove('selected-grey-block')
        EndMeetingRequestCall(currentRoomInfo.roomID)
        timelineInitialized = false;
        PlayBtnClickSound()
    })
    
    let extendBtn = document.getElementById("schedulingExtendMeetingBtn")
    extendBtn.addEventListener('touchstart', function(){
        extendBtn.classList.add('selected-grey-block')
    }, { passive: "true" })
    extendBtn.addEventListener('touchend', function(){
        extendBtn.classList.remove('selected-grey-block')
        ExtendMeetingRequestCall(currentRoomInfo.roomID)
        timelineInitialized = false;
        PlayBtnClickSound()
    })
}

function DrawMeetingsOnTimeline(startAndEndTimes)
{
    let currentTimeInMinutes = currentHour*60 + currentMinute

    document.querySelectorAll(`.green-booking-slot`).forEach(e => e.remove());
    document.querySelectorAll(`.red-booking-slot`).forEach(e => e.remove());
    document.querySelectorAll(`.grey-booking-slot`).forEach(e => e.remove());

    for(let i = 0; i < startAndEndTimes.startHours.length; i++)
    {
        if(startAndEndTimes.startTimesInMinutes[i] > currentTimeInMinutes)
        {
            DrawBookingSlot(startAndEndTimes.startHours[i], startAndEndTimes.startMinutes[i], startAndEndTimes.endHours[i], startAndEndTimes.endMinutes[i], i, 'green-booking-slot')
        }
        if(startAndEndTimes.startTimesInMinutes[i] <= currentTimeInMinutes && startAndEndTimes.endTimesInMinutes[i] > currentTimeInMinutes)
        {
            DrawBookingSlot(startAndEndTimes.startHours[i], startAndEndTimes.startMinutes[i], startAndEndTimes.endHours[i], startAndEndTimes.endMinutes[i], i, 'red-booking-slot')
        }
        if(startAndEndTimes.endTimesInMinutes[i] <= currentTimeInMinutes)
        {
            DrawBookingSlot(startAndEndTimes.startHours[i], startAndEndTimes.startMinutes[i], startAndEndTimes.endHours[i], startAndEndTimes.endMinutes[i], i, 'grey-booking-slot')
        }
    }

    DetermineActiveTimeBlocks(currentTimeInMinutes)
}

function DrawBookingSlot(startHour, startMinute, endHour, endMinute, bookingIndex, bookingSlotColor)
{
    setTimeout(() => {
        let bookingSlotLength = ((endHour*60 + endMinute) - (startHour*60 + startMinute))*5.33;
        
        let bookingSlotBox = document.createElement("div")
        bookingSlotBox.classList.add(""+bookingSlotColor)
        bookingSlotBox.id = "bookingID-"+bookingIndex
    
        bookingSlotBox.style.width = bookingSlotLength + "px"
    
        let pixeOffset = ((startHour*320) + (startMinute *5.3));

        document.getElementById("schedulingTimlineGrid").appendChild(bookingSlotBox)
    
        document.getElementById(bookingSlotBox.id).style.left = pixeOffset + "px";

        bookingSlotBox.addEventListener('touchstart', function(){
            if(bookingSlotColor.includes("grey"))   bookingSlotBox.classList.add('selected-grey-block')
            if(bookingSlotColor.includes("red"))   bookingSlotBox.classList.add('selected-red-block')
            if(bookingSlotColor.includes("green"))   bookingSlotBox.classList.add('selected-green-block')
        }, { passive: "true" })
        bookingSlotBox.addEventListener('touchend', function(){
            if(bookingSlotColor.includes("grey"))   bookingSlotBox.classList.remove('selected-grey-block')
            if(bookingSlotColor.includes("red"))   bookingSlotBox.classList.remove('selected-red-block')
            if(bookingSlotColor.includes("green"))   bookingSlotBox.classList.remove('selected-green-block')
            PlayBtnClickSound()
        })
        bookingSlotBox.addEventListener('click', function(){
            GetSpecificMeetingInfoCall(currentRoomInfo.roomID, bookingIndex)
        })
    }, 100);
}

function DetermineActiveTimeBlocks(currentTimeInMinutes)
{
    if(timelineInitialized) return;

    var nodes = document.getElementById('schedulingTimlineGrid').childNodes;
    
    nodes.forEach(node => {
        if(parseInt(node.id) > currentTimeInMinutes || (parseInt(node.id) < currentTimeInMinutes) && (parseInt(node.id)+30) > currentTimeInMinutes)
        {
            var selectable = document.getElementById(node.id)

            selectable.addEventListener('touchstart', function(){
                    selectable.classList.add('selected-free-block')
            }, { passive: "true" })

            selectable.addEventListener('touchend', function(){
                    selectable.classList.remove('selected-free-block')
                    PlayBtnClickSound()
            })

            selectable.addEventListener('click', function(){
                    NewMeetingRequestCall(currentRoomInfo.roomID, node.id)
            })
        }
    });

    timelineInitialized = true;
}

function ShowMeetingInfoPopUp(meetingInfo)
{
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './pages/Scheduling-Page/Scheduling-Info-Meeting-Info.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var allText = rawFile.responseText;
                document.getElementById('schedulingContainer').innerHTML += allText;
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;

    currentHourToDisplay = parseInt(meetingInfo.startHour) + 3;
    ScrollTimelineToView(currentHourToDisplay)

    FillOutMeetingInfoPopUp(meetingInfo)
}

function FillOutMeetingInfoPopUp(meetingInfo)
{
    document.getElementById("meetingDetailsPopupSubject").innerHTML = meetingInfo.meetingSubject
    document.getElementById("meetingDetailsPopupOrganiser").innerHTML = meetingInfo.meetingOrganiser
    document.getElementById("meetingDetailsPopupOrganiser2").innerHTML = meetingInfo.meetingOrganiser
    document.getElementById("meetingDetailsPopupDuration").innerHTML = meetingInfo.meetingDurationText

    var closeBtn = document.getElementById("meetingDetailsPopupCloseBtn");
    closeBtn.addEventListener('touchstart', function() {
        closeBtn.classList.add('selected-grey-block')
    }, {passive: "true"})
    closeBtn.addEventListener('touchend', function() {
        closeBtn.classList.remove('selected-grey-block')
        timelineInitialized = false;
        PlayBtnClickSound()
        openSubpage("Scheduling-Page")
    })
}

function ShowNewMeetingPopUp(newMeetingInfo)
{
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './pages/Scheduling-Page/Scheduling-Info-New-Meeting.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var allText = rawFile.responseText;
                document.getElementById('schedulingContainer').innerHTML += allText;
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;

    currentHourToDisplay = parseInt(newMeetingInfo.startTime.slice(0, 2)) + 3;
    ScrollTimelineToView(currentHourToDisplay)

    FillOutNewMeetingPopUp(newMeetingInfo)
}

function FillOutNewMeetingPopUp(meetingInfo)
{
    document.getElementById("newMeetingOrganiser").innerHTML = meetingInfo.organiser;
    document.getElementById("newMeetingStartTime").innerHTML = meetingInfo.startTime;
    for(let i = 0; i < meetingInfo.endTimes.length; i++)
    {
        let endTimeBlock = document.getElementById("endTime"+i);
        endTimeBlock.innerHTML = meetingInfo.endTimes[i];
        if(endTimeBlock.innerHTML !== "")
            endTimeBlock.addEventListener('touchstart', function() {
                endTimeBlock.classList.add('selected-grey-block')
            }, {passive: "true"})
            endTimeBlock.addEventListener('touchend', function() {
                endTimeBlock.classList.remove('selected-grey-block')
                PlayBtnClickSound()
                for(let j = 0; j < meetingInfo.endTimes.length; j++)
                    document.getElementById("endTime"+j).classList.remove("new-meeting-selected-end-time")
                endTimeBlock.classList.add("new-meeting-selected-end-time")
            })
    }

    var closeBtn = document.getElementById("newMeetingCancelBtn");
    closeBtn.addEventListener('touchstart', function() {
        closeBtn.classList.add('selected-grey-block')
    }, {passive: "true"})
    closeBtn.addEventListener('touchend', function() {
        closeBtn.classList.remove('selected-grey-block')
        timelineInitialized = false;
        PlayBtnClickSound()
        openSubpage("Scheduling-Page")
    })

    var reserveBtn = document.getElementById("newMeetingReserveBtn");
    reserveBtn.addEventListener('touchstart', function() {
        reserveBtn.classList.add('selected-grey-block')
    }, {passive: "true"})
    reserveBtn.addEventListener('touchend', function() {
        PlayBtnClickSound()
        reserveBtn.classList.remove('selected-grey-block')
    
        let meetingSubject = "Walk up meeting";
        if(document.getElementById("newMeetingSubject").value !== "")
        meetingSubject = document.getElementById("newMeetingSubject").value;
        let startTime = document.getElementById("newMeetingStartTime").innerHTML
        let endTime = document.querySelector(".new-meeting-selected-end-time").innerHTML

        CreateNewMeetingCall(currentRoomInfo.roomID, meetingSubject, startTime, endTime);
        timelineInitialized = false;
    })
}

function DisplayWaitingSubpage()
{
    var meetingPopUpSp = document.getElementById("meetingPopUpSp")
    meetingPopUpSp.innerHTML = "Processing ...";
    meetingPopUpSp.style.color = "white";
    meetingPopUpSp.style.fontSize = "250%";
}

function ShowEndMeetingPopUp(endTime)
{
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './pages/Scheduling-Page/Scheduling-Info-End-Meeting.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var allText = rawFile.responseText;
                document.getElementById('schedulingContainer').innerHTML += allText;
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;

    currentHourToDisplay = currentHour + 3;
    ScrollTimelineToView(currentHourToDisplay)

    FillOutEndMeetingPopUp(endTime)
}

function FillOutEndMeetingPopUp(endTime)
{
    document.getElementById("endMeetingEndsInTime").innerHTML = endTime.minutesLeft

    var closeBtn = document.getElementById("endMeetingCancelBtn");
    closeBtn.addEventListener('touchstart', function() {
        closeBtn.classList.add('selected-grey-block')
    }, {passive: "true"})
    closeBtn.addEventListener('touchend', function() {
        PlayBtnClickSound()
        closeBtn.classList.remove('selected-grey-block')
        timelineInitialized = false;
        openSubpage("Scheduling-Page")
    })

    var endBtn = document.getElementById("endMeetingEndBtn")
    endBtn.addEventListener('touchstart', function() {
        endBtn.classList.add('selected-grey-block')
    }, {passive: "true"})
    endBtn.addEventListener('touchend', function() {
        PlayBtnClickSound()
        endBtn.classList.remove('selected-grey-block')
        EndMeetingNowCall(currentRoomInfo.roomID)
        timelineInitialized = false;
    })
}

function ShowExtendMeetingPopUp(endAndExtendTimes)
{
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './pages/Scheduling-Page/Scheduling-Info-Extend-Meeting.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var allText = rawFile.responseText;
                document.getElementById('schedulingContainer').innerHTML += allText;
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;

    ScrollTimelineToView(currentHour+3)
    FillOutExtendMeetingPopUp(endAndExtendTimes)
}

function FillOutExtendMeetingPopUp(endAndExtendTimes)
{
    document.getElementById("extendMeetingEndsInTime").innerHTML = endAndExtendTimes.minutesRemaining

    var closeBtn = document.getElementById("extendMeetingCancelBtn");
    closeBtn.addEventListener('touchstart', function() {
        closeBtn.classList.add('selected-grey-block')
    }, {passive: "true"})
    closeBtn.addEventListener('touchend', function() {
        closeBtn.classList.remove('selected-grey-block')
        timelineInitialized = false;
        openSubpage("Scheduling-Page")
    })

    var extendBtn = document.getElementById("extendMeetingExtendBtn")
    extendBtn.addEventListener('touchstart', function() {
        extendBtn.classList.add('selected-grey-block')
    }, {passive: "true"})
    extendBtn.addEventListener('touchend', function() {
        PlayBtnClickSound()
        extendBtn.classList.remove('selected-grey-block')
        ExtendBtnPressed(endAndExtendTimes)
        timelineInitialized = false;
    })

    ActivateExtendTimeBtns(endAndExtendTimes)
}

function ActivateExtendTimeBtns(endAndExtendTimes)
{
    for(let i = 0; i < endAndExtendTimes.extendTimes.length; i++)
    {
        let timeSlot = document.getElementById(`extendTimeSlot${i}`)

        timeSlot.classList.remove('non-vis')
        timeSlot.addEventListener('touchstart', function() {
            timeSlot.classList.add('selected-grey-block')
        }, {passive: "true"})
        timeSlot.addEventListener('touchend', function() {
            PlayBtnClickSound()
            timeSlot.classList.remove('selected-grey-block')
            UpdateSelectedTimeFb(endAndExtendTimes.extendTimes.length, timeSlot.id)
        })
    }
}

function UpdateSelectedTimeFb(numberOfTimeSlots, selectedTimeSlot)
{
    for(let i = 0; i < numberOfTimeSlots; i++)
    {
        let timeSlot = document.getElementById(`extendTimeSlot${i}`)

        if(timeSlot.id == selectedTimeSlot)
            timeSlot.classList.add('extend-time-slot-selected')
        else
            timeSlot.classList.remove('extend-time-slot-selected')
    }
}

function ExtendBtnPressed(endAndExtendTimes)
{
    let extendTimeSelcted = parseInt(document.querySelector('.extend-time-slot-selected').id.replace('extendTimeSlot', ''))

    console.log(endAndExtendTimes.extendTimes[extendTimeSelcted])

    ExtendMeetingCall(currentRoomInfo.roomID, endAndExtendTimes.extendTimes[extendTimeSelcted])

    HideExtraBtns()
}

function ScrollTimelineToView(hour)
{
    if(hour > 23) hour = 23;
    document.getElementById(`hour${hour}`).scrollIntoView();
}

function CheckIfMeetingEndingSoon(meetingInfo)
{
    if(meetingInfo.currentHoursRemaining == 0 && meetingInfo.currentMinutesRemaining <= 5 && meetingInfo.currentMinutesRemaining > 0)
    {
        if(currentSubpage !== "Scheduling-Page")
        {
            openPopUp("Meeting-Ending")

            if(document.getElementById("meetingEndTime") !== null)
                document.getElementById("meetingEndTime").innerHTML = meetingInfo.currentMinutesRemaining
        }
    }
}