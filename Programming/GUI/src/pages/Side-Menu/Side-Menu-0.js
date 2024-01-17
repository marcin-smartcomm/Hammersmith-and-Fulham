//Call from SchedulingAjaxCalls.js
function FillSideMenuBookingsInfo(currentAndNextBookingInfo)
{
    if(!sideMenuVis || sideMenuCurrentlyDisplayed != "Main") return;

    if(panelType == "TSW" || panelType == "iPadS") SideMenuBookingsInfoFill(currentAndNextBookingInfo)
    if(panelType == "iPadM") iPadMSideMenuBookingsInfoFill(currentAndNextBookingInfo)
}
function SideMenuBookingsInfoFill(currentAndNextBookingInfo)
{
    if(currentAndNextBookingInfo.currentMeetingSubject.length > 20)
    currentAndNextBookingInfo.currentMeetingSubject = currentAndNextBookingInfo.currentMeetingSubject.slice(0, 20) + "..."

    if(currentAndNextBookingInfo.currentMeetingOrganiser.length > 15)
    currentAndNextBookingInfo.currentMeetingOrganiser = currentAndNextBookingInfo.currentMeetingOrganiser.slice(0, 15) + "..."

    if(currentAndNextBookingInfo.nextMeetingSubject.length > 20)
    currentAndNextBookingInfo.nextMeetingSubject = currentAndNextBookingInfo.nextMeetingSubject.slice(0, 20) + "..."

    if(currentAndNextBookingInfo.nextMeetingOrganiser.length > 15)
    currentAndNextBookingInfo.nextMeetingOrganiser = currentAndNextBookingInfo.nextMeetingOrganiser.slice(0, 15) + "..."

    if(currentAndNextBookingInfo.currentMeetingStartEndTime != "")
    {
        document.getElementById('currentMeetingDuration').innerHTML = "Now: " + currentAndNextBookingInfo.currentMeetingStartEndTime;
        document.getElementById('currentMeetingNameAndSubject').innerHTML =  currentAndNextBookingInfo.currentMeetingOrganiser + " - " + currentAndNextBookingInfo.currentMeetingSubject;
    }
    else
    {
        document.getElementById('currentMeetingDuration').innerHTML = "<b>Now:</b>";
        document.getElementById('currentMeetingNameAndSubject').innerHTML = "Room Is Free";
    }

    if(currentAndNextBookingInfo.nextMeetingStartEndTime != "")
    {
        document.getElementById('nextMeetingDuration').innerHTML = "Next booking: " + currentAndNextBookingInfo.nextMeetingStartEndTime;
        document.getElementById('nextMeetingNameAndSubject').innerHTML =  currentAndNextBookingInfo.nextMeetingOrganiser + " - " + currentAndNextBookingInfo.nextMeetingSubject;
    }
    else
    {
        document.getElementById('nextMeetingDuration').innerHTML = "Next booking:";
        document.getElementById('nextMeetingNameAndSubject').innerHTML = "Free All Day";
    }
}

function ActivateSideMenuBtns()
{
    if(panelType == "TSW" || panelType == "iPadM" || panelType == "iPadS")
    {
        if(sideMenuCurrentlyDisplayed == "Volume") ActivateVolSideMenu()
        if(sideMenuCurrentlyDisplayed == "Mic") ActivateMicSideMenu()
    }

    if(panelType == "TSW" || panelType == "iPadS")
    {
      if(sideMenuCurrentlyDisplayed == "Help") ActivateHelpSideMenu()
      if(sideMenuCurrentlyDisplayed == "Settings") ActivateSettingsSideMenu()
    }

    if(panelType == "iPadM" || panelType == "iPadS")
      if(sideMenuCurrentlyDisplayed == "FloorList") ActivateFloorListSideMenu()

    if(panelType == "TSW")
        if(sideMenuCurrentlyDisplayed == "Main") ActivateTSWMainSideMenu()
    if(panelType == "iPadS")
        if(sideMenuCurrentlyDisplayed == "Main") ActivateiPadSMainSideMenu()
    if(panelType == "iPadM")
    {
        if(sideMenuCurrentlyDisplayed == "Main") ActivateiPadMMainSideMenu()
        if(sideMenuCurrentlyDisplayed == "Settings") ActivateiPadMSettingsSideMenu()
    }
}

function ActivateVolSideMenu()
{
    let closeVolPageBtn = document.getElementById("sideMenuVolCloseBtn")
    let volUpBtn = document.getElementById("sideMenuVolUpBtn")
    let volDownBtn = document.getElementById("sideMenuVolDownBtn")
    let volMuteBtn = document.getElementById("sideMenuVolMuteBtn")
    let volSlider = document.getElementById("sideMenuVolSlider")

    closeVolPageBtn.addEventListener("touchstart", function(){
        closeVolPageBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    closeVolPageBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        closeVolPageBtn.classList.remove('btn-generic-pressed')
        LoadSideMenu("Main")
        if(panelType == "TSW") ActivateTSWMainSideMenu()
        if(panelType == "iPadM") ActivateiPadMMainSideMenu()
        if(panelType == "iPadS") ActivateiPadSMainSideMenu()
    })

    volUpBtn.addEventListener("touchstart", function(){
        PlayBtnClickSound()
        volUpBtn.classList.add('btn-generic-pressed')
        VolChangeCall(currentRoomInfo.roomID, "up", true)
    }, { passive: "true" })
    volUpBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        volUpBtn.classList.remove('btn-generic-pressed')
        VolChangeCall(currentRoomInfo.roomID, "up", false)
    })

    volDownBtn.addEventListener("touchstart", function(){
        PlayBtnClickSound()
        volDownBtn.classList.add('btn-generic-pressed')
        VolChangeCall(currentRoomInfo.roomID, "down", true)
    }, { passive: "true" })
    volDownBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        volDownBtn.classList.remove('btn-generic-pressed')
        VolChangeCall(currentRoomInfo.roomID, "down", false)
    })

    volMuteBtn.addEventListener("touchstart", function(){
    }, { passive: "true" })
    volMuteBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        MuteVolCall(currentRoomInfo.roomID)
    })

    volSlider.disabled = true;
}

function UpdateVolLevel(volLevel)
{
    if(sideMenuCurrentlyDisplayed != "Volume") return;

    let volSlider = document.getElementById("sideMenuVolSlider")
    let volSliderLevelText = document.getElementById("volLevelText")
    
    volSliderLevelText.innerHTML = volLevel + "%"
    volSlider.value = volLevel
    volSlider.style.background = `
    linear-gradient(to right, var(--generic-text-color-inactive) 0%, var(--generic-text-color-inactive) ${volLevel}%, var(--generic-border-color-grey) ${volLevel}%, var(--generic-border-color-grey) 100%)`
}

function UpdateVolMuteState(newState)
{
    if(sideMenuCurrentlyDisplayed != "Volume") return;

    let volMuteBtn = document.getElementById("sideMenuVolMuteBtn")

    if(newState) volMuteBtn.classList.add('btn-generic-pressed')
    if(!newState) volMuteBtn.classList.remove('btn-generic-pressed')
}

function ActivateMicSideMenu()
{
    let closeVolPageBtn = document.getElementById("sideMenuMicCloseBtn")
    let volUpBtn = document.getElementById("sideMenuMicUpBtn")
    let volDownBtn = document.getElementById("sideMenuMicDownBtn")
    let volMuteBtn = document.getElementById("sideMenuMicMuteBtn")
    let volSlider = document.getElementById("sideMenuMicSlider")

    closeVolPageBtn.addEventListener("touchstart", function(){
        closeVolPageBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    closeVolPageBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        closeVolPageBtn.classList.remove('btn-generic-pressed')
        LoadSideMenu("Main")
        if(panelType == "TSW") ActivateTSWMainSideMenu()
        if(panelType == "iPadM") ActivateiPadMMainSideMenu()
        if(panelType == "iPadS") ActivateiPadSMainSideMenu()
    })

    volUpBtn.addEventListener("touchstart", function(){
        PlayBtnClickSound()
        volUpBtn.classList.add('btn-generic-pressed')
        MicChangeCall(currentRoomInfo.roomID, "up", true)
    }, { passive: "true" })
    volUpBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        volUpBtn.classList.remove('btn-generic-pressed')
        MicChangeCall(currentRoomInfo.roomID, "up", false)
    })

    volDownBtn.addEventListener("touchstart", function(){
        PlayBtnClickSound()
        volDownBtn.classList.add('btn-generic-pressed')
        MicChangeCall(currentRoomInfo.roomID, "down", true)
    }, { passive: "true" })
    volDownBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        volDownBtn.classList.remove('btn-generic-pressed')
        MicChangeCall(currentRoomInfo.roomID, "down", false)
    })

    volMuteBtn.addEventListener("touchstart", function(){
    }, { passive: "true" })
    volMuteBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        MuteMicCall(currentRoomInfo.roomID)
    })

    volSlider.disabled = true;
}

function UpdateMicLevel(volLevel)
{
    if(sideMenuCurrentlyDisplayed != "Mic") return;

    let volSlider = document.getElementById("sideMenuMicSlider")
    let volSliderLevelText = document.getElementById("micLevelText")
    
    volSliderLevelText.innerHTML = volLevel + "%"
    volSlider.value = volLevel
    volSlider.style.background = `
    linear-gradient(to right, var(--generic-text-color-inactive) 0%, var(--generic-text-color-inactive) ${volLevel}%, var(--generic-border-color-grey) ${volLevel}%, var(--generic-border-color-grey) 100%)`
}

function UpdateMicMuteState(newState)
{
    if(sideMenuCurrentlyDisplayed != "Mic") return;

    let volMuteBtn = document.getElementById("sideMenuMicMuteBtn")

    if(newState) volMuteBtn.classList.add('btn-generic-pressed')
    if(!newState) volMuteBtn.classList.remove('btn-generic-pressed')
}

function ActivateHelpSideMenu()
{
    InitializeTSWHelpSideMenu()
    
    let sideMenuCloseBtn = document.getElementById("sideMenuHelpCloseBtn")

    sideMenuCloseBtn.addEventListener("touchstart", function(){
        sideMenuCloseBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    sideMenuCloseBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        sideMenuCloseBtn.classList.remove('btn-generic-pressed')
        LoadSideMenu("Main")
        if(panelType == "TSW") ActivateTSWMainSideMenu()
        if(panelType == "iPadS") ActivateiPadSMainSideMenu()
    })
}

function ActivateSideMenuHelpBtns(btns)
{
    for(let i = 0; i < btns.length; i++)
    {
        btns[i].addEventListener("touchstart", function() {
            btns[i].classList.add("btn-generic-pressed")
        }, { passive: "true" })
        btns[i].addEventListener("touchend", function() {
            btns[i].classList.remove("btn-generic-pressed")
        })
        btns[i].addEventListener("click", function() {

            PlayBtnClickSound()
            UpdateHelpFb(btns[i].id)
            if(btns[i].innerHTML == "Video Input")
                openSubpage(`Help-Page-Video-Input`, "Video Input", currentRoomInfo.menuItems[currentRoomInfo.menuItems.length-1].menuItemIcon)
            else if(btns[i].innerHTML == "Video Production")
                openSubpage(`Help-Page-Video-Production`, "Video Production", currentRoomInfo.menuItems[currentRoomInfo.menuItems.length-1].menuItemIcon)
            else
                openSubpage(`Help-Page-${currentRoomInfo.menuItems[i].menuItemName.replace(' ', '-')}`, currentRoomInfo.menuItems[i].menuItemName, currentRoomInfo.menuItems[i].menuItemIcon)
        })

        //If current subpage is one of help buttons, navigate to that help page
        if(currentSubpage.includes(btns[i].innerHTML.replace(' ', '-').replace('/', '-'))) 
        {
            if(btns[i].classList.contains("btn-generic-pressed")) continue

            btns[i].classList.add("btn-generic-pressed")
            if(btns[i].innerHTML == "Video Input")
                openSubpage(`Help-Page-Video-Input`, "Video Input", currentRoomInfo.menuItems[currentRoomInfo.menuItems.length-1].menuItemIcon)
            else if(btns[i].innerHTML == "Video Production")
                openSubpage(`Help-Page-Video-Production`, "Video Production", currentRoomInfo.menuItems[currentRoomInfo.menuItems.length-1].menuItemIcon)
            else
                openSubpage(`Help-Page-${currentRoomInfo.menuItems[i].menuItemName.replace(' ', '-')}`, currentRoomInfo.menuItems[i].menuItemName, currentRoomInfo.menuItems[i].menuItemIcon)
        } 
    }

    for(let i = 0; i < btns.length; i++){
        if(btns[i].classList.contains("btn-generic-pressed"))
            btns[i].scrollIntoView()
    }
}

function UpdateHelpFb(idPressed)
{
    let optionBtns = document.querySelectorAll(".side-menu-help-option-btn")

    optionBtns.forEach(btn => {
        if(btn.id == idPressed)
            btn.classList.add("btn-generic-pressed")
        else
            btn.classList.remove("btn-generic-pressed")
    });
}

function UpdateSettingsFb(idPressed)
{
    let optionBtns = document.querySelectorAll(".side-menu-settings-option-btn")

    optionBtns.forEach(btn => {
        if(btn.id == idPressed)
            btn.classList.add("btn-generic-pressed")
        else
            btn.classList.remove("btn-generic-pressed")
    });
}

function ProcessAdminAssistanceData(assistanceRequests)
{
    let assistanceBtn = document.getElementById("sideMenuMainAdminAssistanceBtn")

    if(assistanceBtn !== null)
    {
        if(assistanceRequests.cards.length > 0)
        {
            assistanceBtn.style.borderColor = "var(--generic-border-color-active)"
            assistanceBtn.style.color = "var(--generic-text-color-schedule-red)"
        }
    }
}

function ProcessSystemAlertsData(systemAlerts)
{
    let systemAlertsBtn = document.getElementById("sideMenuMainSystemAlertsBtn")

    if(systemAlertsBtn !== null)
    {
        if(systemAlerts.cardList.length > 0)
        {
            systemAlertsBtn.style.borderColor = "var(--generic-text-color-schedule-red)"
            systemAlertsBtn.style.color = "var(--generic-text-color-schedule-red)"
        }
        else
        {
            systemAlertsBtn.style.borderColor = "var(--generic-text-color-inactive)"
            systemAlertsBtn.style.color = "var(--generic-text-color-inactive)"
        }
    }
}

function ActivateSettingsSideMenu()
{
    let panelSettingsBtn = document.getElementById("settingsOptionBtn0")
    let lightModeBtn = document.getElementById("settingsOptionBtn1")
    let lightModeStateText = document.getElementById("lightModeStateText")
    let sideMenuCloseBtn = document.getElementById("sideMenuSettingsCloseBtn")

    panelSettingsBtn.addEventListener("touchstart", function(){
        panelSettingsBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    panelSettingsBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        panelSettingsBtn.classList.remove('btn-generic-pressed')
        openSubpage("Panel-Settings-"+panelType, "Panel Settings", 'fa-solid fa-gear')
        UpdateSettingsFb(panelSettingsBtn.id);
    })

    lightModeBtn.addEventListener("touchstart", function(){
        lightModeBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    lightModeBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        lightModeBtn.classList.remove('btn-generic-pressed')

        if(inLightMode)
        {
            //SystemVariables.js
            ChangeToOriginalTheme()
            inLightMode = false;
            lightModeStateText.innerHTML = "Off"
        }
        else
        {
            //SystemVariables.js
            ChangeToLightMode()
            inLightMode = true;
            lightModeStateText.innerHTML = "On"
        }
    })

    if(inLightMode) lightModeStateText.innerHTML = "On"
    else lightModeStateText.innerHTML = "Off"

    sideMenuCloseBtn.addEventListener("touchstart", function(){
        sideMenuCloseBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    sideMenuCloseBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        sideMenuCloseBtn.classList.remove('btn-generic-pressed')
        LoadSideMenu("Main")
        if(panelType == "TSW") ActivateTSWMainSideMenu()
        if(panelType == "iPadS") ActivateiPadSMainSideMenu()
    })

    if(currentSubpage.includes("Panel-Settings")) 
        panelSettingsBtn.classList.add("btn-generic-pressed")
}

function ActivateFloorListSideMenu()
{
    let roomName = document.getElementById("sideMenuRoomName")
    let backBtn = document.getElementById("backBtn")
    let floorBtns = document.querySelectorAll(".floor-btn")
    let colabScreenBtn = document.getElementById("colabBtn")

    try
    {
        roomName.innerHTML = currentRoomInfo.roomName;
    }
    catch(e)
    {
        roomName.innerHTML = "No Room Selected"
    }
    
    backBtn.addEventListener('touchstart', function() {
        backBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    backBtn.addEventListener('touchend', function() {
        backBtn.classList.remove("btn-generic-pressed")
        LoadSideMenu("Main")
        ActivateSideMenuBtns()
    })

    floorBtns.forEach(btn => {
        btn.addEventListener("touchstart", function() {
            btn.classList.add("btn-generic-pressed")
        }, {passive: "true"})
        btn.addEventListener("touchend", function() {
            btn.classList.remove("btn-generic-pressed")
            openSubpage("Room-Select", btn.id.split('-')[1])
        })
    });

    colabScreenBtn.addEventListener('touchstart', function() {
        colabScreenBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    colabScreenBtn.addEventListener('touchend', function() {
        colabScreenBtn.classList.remove("btn-generic-pressed")
        openSubpage("Collaboration-Screens-Main", "Collaboration Screens", "fa-solid fa-tv")
        LoadSideMenu("Main")
        ActivateSideMenuBtns()
    })

    if(panelType == "iPadS") colabScreenBtn.style.display = "none"
}