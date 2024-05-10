function InitializeRoamingiPadSp()
{
    let assignBtns = document.querySelectorAll(".roaming-ipads-assgn-room-btn");
    let changePassBtn = document.getElementById("changePass");
    
    for(let i = 0; i < assignBtns.length; i++)
    {
        assignBtns[i].addEventListener("touchstart", function() {
            assignBtns[i].classList.add("btn-generic-pressed")
        }, {passive: "true"})
        assignBtns[i].addEventListener("touchend", function() {
            PlayBtnClickSound()
            assignBtns[i].classList.remove("btn-generic-pressed")
            openSubpage("iPadM-Roaming-iPads-Floor-Select", `Roaming iPads (${i+1})`, "fa-solid fa-tablet-screen-button", (i+1))
        })
    }

    changePassBtn.addEventListener("touchstart", function() {
        changePassBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    changePassBtn.addEventListener("touchend", function() {
        PlayBtnClickSound()
        changePassBtn.classList.remove("btn-generic-pressed")
        openPopUp("Change-Password")
    })

    AddSlaveRoomsToName(CoreProcessorAjaxGETCall("SlaveRooms", []))
}

function AddSlaveRoomsToName(roomNames)
{
    let iPadNames = document.querySelectorAll(".roaming-ipads-ipad-text");
    
    for(let i = 0; i < iPadNames.length; i++)
    {
        iPadNames[i].innerHTML += ` - ${roomNames.panels[i].roomName}`
    }
}