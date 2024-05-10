function InitializePCLaptopSp(pageName, pageIcon)
{
    $('#sourceName').text(pageName)
    InitializePCLaptopBtns(pageName, pageIcon)
    DeterminePTZControlBtnFunctionality(
        RoomProcessorAjaxGETCall("GetCameras", [currentRoomInfo.roomID])
    )
}

function InitializePCLaptopBtns(pageName, pageIcon)
{
    let ptzBtn = document.getElementById("pcLaptopPTZBtn")

    ptzBtn.addEventListener('touchstart', function() {
        ptzBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    ptzBtn.addEventListener('touchend', function() {
        ptzBtn.classList.remove("btn-generic-pressed")
        openSubpage("PTZ-Control", pageName, pageIcon)
        PlayBtnClickSound()
    })
}

function DeterminePTZControlBtnFunctionality(cameras)
{
    if(cameras.length <= 0) document.getElementById("pcLaptopPTZBtn").style.visibility = 'hidden'
}