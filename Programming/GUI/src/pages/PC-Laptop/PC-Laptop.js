function InitializePCLaptopSp(pageName, pageIcon)
{
    InitializePCLaptopBtns(pageName, pageIcon)
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