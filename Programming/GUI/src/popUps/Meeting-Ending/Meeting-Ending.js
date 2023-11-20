//openned from Scheduling-Page.js when new wbooking info is parsed
function InitializeMeetingEndingPopUp()
{
    let closeBtn = document.getElementById("meetingEndingCloseBtn")
    let bookBtn = document.getElementById("meetingEndingBookBtn")

    closeBtn.addEventListener('touchstart', function(){
        closeBtn.classList.add('btn-generic-pressed')
    }, {passive: "true"})
    closeBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        closeBtn.classList.remove('btn-generic-pressed')
        clearSpecificPopUp("meetingEndingPopUpSection")
    })

    bookBtn.addEventListener('touchstart', function(){
        bookBtn.classList.add('btn-generic-pressed')
    }, {passive: "true"})
    bookBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        bookBtn.classList.remove('btn-generic-pressed')
        clearSpecificPopUp("meetingEndingPopUpSection")
        openSubpage("Scheduling-Page")
    })

    if(panelType == "iPadS") bookBtn.style.display = "none"
}