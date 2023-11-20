function InitializeAcknowledgeRequestPopUp()
{
    let closeBtn = document.getElementById("acknowledgeRequestCloseBtn")

    closeBtn.addEventListener('touchstart', function(){
        closeBtn.classList.add('btn-generic-pressed')
    }, {passive: "true"})
    closeBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        closeBtn.classList.remove('btn-generic-pressed')
        clearSpecificPopUp("popUpSection")
    })
}