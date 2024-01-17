function InitializeShutdownPopUp()
{
    let closeBtn = document.getElementById("shutdownCloseBtn")
    let confirmBtn = document.getElementById("shutdownConfirmBtn")
    let cancelBtn = document.getElementById("shutdownCancelBtn")

    closeBtn.addEventListener('touchstart', function(){
        closeBtn.classList.add('btn-generic-pressed')
    }, {passive: "true"})
    closeBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        closeBtn.classList.remove('btn-generic-pressed')
        clearSpecificPopUp("shutDownPopUpSection")
    })

    confirmBtn.addEventListener('touchstart', function(){
        confirmBtn.classList.add('btn-generic-pressed')
    }, {passive: "true"})
    confirmBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        confirmBtn.classList.remove('btn-generic-pressed')
        clearSpecificPopUp("shutDownPopUpSection")

        NewMenuItemSelectedCall("Off")
    })

    cancelBtn.addEventListener('touchstart', function(){
        cancelBtn.classList.add('btn-generic-pressed')
    }, {passive: "true"})
    cancelBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        cancelBtn.classList.remove('btn-generic-pressed')
        clearSpecificPopUp("shutDownPopUpSection")
    })
}