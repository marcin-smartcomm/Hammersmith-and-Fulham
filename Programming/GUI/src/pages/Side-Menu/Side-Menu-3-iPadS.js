function ActivateiPadSMainSideMenu()
{
  let floorSelectBtn = document.getElementById("floorSelectBtn")
  let volBtn = document.getElementById("sideMenuMainVolBtn");
  let micBtn = document.getElementById("sideMenuMainMicBtn");
  let settingsBtn = document.getElementById("sideMenuMainSettingsBtn");
  let shutdownBtn = document.getElementById("sideMenuMainShutdownBtn");

  if(currentRoomInfo !== null)
    document.getElementById('roomNameContainer').innerHTML = "<b>"+currentRoomInfo.roomName+"</b>";

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

  floorSelectBtn.addEventListener("touchstart", function(){
    floorSelectBtn.classList.add('btn-generic-pressed')
  }, { passive: "true" })
  floorSelectBtn.addEventListener('touchend', function(){
    PlayBtnClickSound()
    floorSelectBtn.classList.remove('btn-generic-pressed')
    openPopUp("Password-Input")
  })

  if(currentRoomInfo !== null)
    GetBookingsInfoCall(currentRoomInfo.roomID)
}