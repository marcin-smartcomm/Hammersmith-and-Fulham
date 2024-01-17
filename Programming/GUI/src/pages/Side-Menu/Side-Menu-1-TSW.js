function ActivateTSWMainSideMenu()
{
    let bookingBtn = document.getElementById("bookingBtn");
    let volBtn = document.getElementById("sideMenuMainVolBtn");
    let micBtn = document.getElementById("sideMenuMainMicBtn");
    let settingsBtn = document.getElementById("sideMenuMainSettingsBtn");
    let shutdownBtn = document.getElementById("sideMenuMainShutdownBtn");

    document.getElementById('roomNameContainer').innerHTML = "<b>"+currentRoomInfo.roomName+"</b>";

    bookingBtn.addEventListener("touchstart", function(){
        bookingBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    bookingBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        bookingBtn.classList.remove('btn-generic-pressed')
        openSubpage("Scheduling-Page");
    })

    volBtn.addEventListener("touchstart", function(){
        volBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    volBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        volBtn.classList.remove('btn-generic-pressed')
        LoadSideMenu("Volume")
        ActivateSideMenuBtns()
        GetSliderLevelCall(currentRoomInfo.roomID, 'vol')
        GetMuteStateCall(currentRoomInfo.roomID, 'vol')
    })

    micBtn.addEventListener("touchstart", function(){
        micBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    micBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        micBtn.classList.remove('btn-generic-pressed')
        LoadSideMenu("Mic")
        ActivateSideMenuBtns()
        GetSliderLevelCall(currentRoomInfo.roomID, 'mic')
        GetMuteStateCall(currentRoomInfo.roomID, 'mic')
    })

    settingsBtn.addEventListener("touchstart", function(){
        settingsBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    settingsBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        settingsBtn.classList.remove('btn-generic-pressed')
        LoadSideMenu("Settings")
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

    GetBookingsInfoCall(currentRoomInfo.roomID)
}

function InitializeTSWHelpSideMenu()
{
    if(document.querySelectorAll(".side-menu-help-option-btn").length > 0)
    {
        ActivateSideMenuHelpBtns(document.querySelectorAll(".side-menu-help-option-btn"))
        return;
    }

    for(let i = 0; i < currentRoomInfo.menuItems.length; i++)
    {
        if(currentRoomInfo.menuItems[i].menuItemName.includes("HDMI Input")) continue

        let newBtn = document.createElement("div")
        newBtn.classList.add("side-menu-help-option-btn")
        newBtn.classList.add("btn-generic")
        newBtn.classList.add("centered")
        newBtn.id = `helpOptionBtn${i}`
        newBtn.innerHTML = currentRoomInfo.menuItems[i].menuItemName.replace('-', '/')
        document.getElementById("optionsContainer").appendChild(newBtn)
    }

    ActivateSideMenuHelpBtns(document.querySelectorAll(".side-menu-help-option-btn"))
}