function InitializeAssistanceFailedPopUp()
{
    let closeBtn = document.getElementById("assistanceFailedCloseBtn")

    closeBtn.addEventListener('touchstart', function(){
        closeBtn.classList.add('btn-generic-pressed')
    }, {passive: "true"})
    closeBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        closeBtn.classList.remove('btn-generic-pressed')
        clearSpecificPopUp("popUpSection")
    })
}