function iPadMSideMenuBookingsInfoFill(currentAndNextBookingInfo)
{
    let minsLeft = document.getElementById("sideMenuMinsLeft")
    let minsLeftInfo = document.getElementById("sideMenuMinsLeftInfo")

    if(currentAndNextBookingInfo.freeAllDay)
    {
        minsLeft.style.color ="var(--generic-border-color-active)"
        minsLeft.innerHTML = "FREE"
        minsLeftInfo.innerHTML = "All Day"
    }
    else if(currentAndNextBookingInfo.inMeeting)
    {
        minsLeft.style.color = "var(--generic-text-color-schedule-red)"
        minsLeft.innerHTML = parseInt(currentAndNextBookingInfo.currentHoursRemaining)*60 + parseInt(currentAndNextBookingInfo.currentMinutesRemaining)
        minsLeftInfo.innerHTML = "MINS REMAINING"
    }
    else if(!currentAndNextBookingInfo.inMeeting)
    {
        minsLeft.style.color ="var(--generic-border-color-active)"
        minsLeft.innerHTML = parseInt(currentAndNextBookingInfo.hoursUntilNextMeeting)*60 + parseInt(currentAndNextBookingInfo.minutesUntilNextMeeting)
        minsLeftInfo.innerHTML = "MINS ROOM AVAILBLE"
    }
}

function ActivateiPadMMainSideMenu()
{
    let roomName = document.getElementById("sideMenuRoomName")
    let roomSelectBtn = document.getElementById("floorSelectBtn")
    let assistanceBtn = document.getElementById("sideMenuMainAdminAssistanceBtn")
    let systemAlertsBtn = document.getElementById("sideMenuMainSystemAlertsBtn")
    let settingsBtn = document.getElementById("sideMenuMainAdminSettingsBtn")
    let volBtn = document.getElementById("sideMenuMainVolBtn");
    let micBtn = document.getElementById("sideMenuMainMicBtn");
    let shutdownBtn = document.getElementById("sideMenuMainShutdownBtn");
    
    roomSelectBtn.addEventListener('touchstart', function() {
        roomSelectBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    roomSelectBtn.addEventListener('touchend', function() {
        PlayBtnClickSound()
        roomSelectBtn.classList.remove("btn-generic-pressed")
        LoadSideMenu("FloorList")
        ActivateSideMenuBtns()
    })
    
    assistanceBtn.addEventListener('touchstart', function() {
        assistanceBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    assistanceBtn.addEventListener('touchend', function() {
        PlayBtnClickSound()
        assistanceBtn.classList.remove("btn-generic-pressed")
        openSubpage("Admin-Assistance")
    })
    
    systemAlertsBtn.addEventListener('touchstart', function() {
        systemAlertsBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    systemAlertsBtn.addEventListener('touchend', function() {
        PlayBtnClickSound()
        systemAlertsBtn.classList.remove("btn-generic-pressed")
        openSubpage("System-Alerts")
    })
    
    settingsBtn.addEventListener('touchstart', function() {
        settingsBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    settingsBtn.addEventListener('touchend', function() {
        PlayBtnClickSound()
        settingsBtn.classList.remove('btn-generic-pressed')
        LoadSideMenu("Settings")
        ActivateSideMenuBtns()
    })

    volBtn.addEventListener("touchstart", function(){
        volBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    volBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        volBtn.classList.remove('btn-generic-pressed')
        LoadSideMenu("Volume")
        ActivateSideMenuBtns()
    })

    micBtn.addEventListener("touchstart", function(){
        micBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    micBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        micBtn.classList.remove('btn-generic-pressed')
        LoadSideMenu("Mic")
        ActivateSideMenuBtns()
    })

    shutdownBtn.addEventListener("touchstart", function(){
        shutdownBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    shutdownBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        shutdownBtn.classList.remove('btn-generic-pressed')
        openPopUp("Shutdown")
    })

    roomName.innerHTML = currentRoomInfo.roomName

    GetBookingsInfoCall(currentRoomInfo.roomID)
    GetAssistanceRequestsCall()
    GetSystemAlertsCall()
}
function ActivateiPadMSettingsSideMenu()
{
    let panelSettingsBtn = document.getElementById("settingsOptionBtn0")
    let globalTempBtn = document.getElementById("settingsOptionBtn1")
    let roamingIpadsBtn = document.getElementById("settingsOptionBtn2")
    let portableEquipmentBtn = document.getElementById("settingsOptionBtn3")
    let digitalSignageBtn = document.getElementById("settingsOptionBtn4")
    let combineRoomsBtn = document.getElementById("settingsOptionBtn5")
    let lightModeBtn = document.getElementById("settingsOptionBtn6")
    let lightModeStateText = document.getElementById("lightModeStateText")
    let screenControlBtn = document.getElementById("settingsOptionBtn7")

    let sideMenuCloseBtn = document.getElementById("sideMenuSettingsCloseBtn")

    panelSettingsBtn.addEventListener("click", function() {
        panelSettingsBtn.classList.add('btn-generic-pressed')
        PlayBtnClickSound()
        openSubpage("iPadM-Panel-Settings", "Panel Settings", 'fa-solid fa-gear')
        UpdateSettingsFb(panelSettingsBtn.id);
        scrollIntoViewElement = panelSettingsBtn
    })

    globalTempBtn.addEventListener("click", function() {
        globalTempBtn.classList.add('btn-generic-pressed')
        PlayBtnClickSound()
        openSubpage("iPadM-Global-Temp", "Global Temperature", 'fa-solid fa-temperature-half')
        UpdateSettingsFb(globalTempBtn.id);
        scrollIntoViewElement = globalTempBtn
    })

    roamingIpadsBtn.addEventListener("click", function() {
        roamingIpadsBtn.classList.add('btn-generic-pressed')
        PlayBtnClickSound()
        openSubpage("iPadM-Roaming-iPads", "Roaming iPads", 'fa-solid fa-tablet-screen-button')
        UpdateSettingsFb(roamingIpadsBtn.id);
        scrollIntoViewElement = roamingIpadsBtn
    })

    portableEquipmentBtn.addEventListener("click", function() {
        portableEquipmentBtn.classList.add('btn-generic-pressed')
        PlayBtnClickSound()
        openSubpage("iPadM-Portable-Equipment", "Portable Equipment", 'fa-solid fa-tv')
        UpdateSettingsFb(portableEquipmentBtn.id);
        scrollIntoViewElement = portableEquipmentBtn
    })

    digitalSignageBtn.addEventListener("click", function() {
        digitalSignageBtn.classList.add('btn-generic-pressed')
        PlayBtnClickSound()
        openSubpage("Digital-Signage-Control", "Digital Signage", 'fa-solid fa-tv')
        UpdateSettingsFb(digitalSignageBtn.id);
        scrollIntoViewElement = digitalSignageBtn
    })

    combineRoomsBtn.addEventListener("click", function() {
        combineRoomsBtn.classList.add('btn-generic-pressed')
        PlayBtnClickSound()
        openSubpage("Combine-Rooms", "Combine Rooms", 'fa-solid fa-gear')
        UpdateSettingsFb(combineRoomsBtn.id);
        scrollIntoViewElement = combineRoomsBtn
    })

    screenControlBtn.addEventListener("click", function() {
        screenControlBtn.classList.add('btn-generic-pressed')
        PlayBtnClickSound()
        openSubpage("Screen-Control", "Screen Control", 'fa-solid fa-tv')
        UpdateSettingsFb(screenControlBtn.id);
        scrollIntoViewElement = screenControlBtn
    })

    lightModeBtn.addEventListener("click", function() {
        lightModeBtn.classList.add('btn-generic-pressed')
        PlayBtnClickSound()
        if(inLightMode)
        {
            ChangeToOriginalTheme()
            inLightMode = false;
            lightModeStateText.innerHTML = "Off"
        }
        else
        {
            ChangeToLightMode()
            inLightMode = true;
            lightModeStateText.innerHTML = "On"
        }
        lightModeBtn.classList.remove('btn-generic-pressed')
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
        ActivateiPadMMainSideMenu()
    })

    if(currentSubpage.includes("Panel-Settings")) 
    {
        panelSettingsBtn.classList.add("btn-generic-pressed")
        panelSettingsBtn.scrollIntoView({ block: "end" })
    }
    if(currentSubpage.includes("Global-Temp")) 
    {
        globalTempBtn.classList.add("btn-generic-pressed")
        globalTempBtn.scrollIntoView({ block: "end" })
    }
    if(currentSubpage.includes("Roaming-iPads")) 
    {
        roamingIpadsBtn.classList.add("btn-generic-pressed")
        roamingIpadsBtn.scrollIntoView({ block: "end" })
    }
    if(currentSubpage.includes("Portable-Equipment")) 
    {
        portableEquipmentBtn.classList.add("btn-generic-pressed")
        portableEquipmentBtn.scrollIntoView({ block: "end" })
    }
    if(currentSubpage.includes("Digital-Signage")) 
    {
        digitalSignageBtn.classList.add("btn-generic-pressed")
        digitalSignageBtn.scrollIntoView()
    }
    if(currentSubpage.includes("Combine-Rooms")) 
    {
        combineRoomsBtn.classList.add("btn-generic-pressed")
        combineRoomsBtn.scrollIntoView()
    }
    if(currentSubpage.includes("Screen-Control")) 
    {
        screenControlBtn.classList.add("btn-generic-pressed")
        screenControlBtn.scrollIntoView()
    }

}