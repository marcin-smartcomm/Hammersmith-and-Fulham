function InitializeRoomSelectRoamingiPadsSp(floor, slaveiPadID)
{
    var request = new XMLHttpRequest();
    request.open("GET", `./availableRooms/floor${floor}.json`, false);
    request.onreadystatechange = function ()
    {
        if(request.readyState === 4)
            if(request.status === 200 || request.status == 0)
                var roomsAvailable = JSON.parse(request.responseText);
        
        FillRoomsAvailableSectioniPadM(roomsAvailable, slaveiPadID)
    }
    request.send(null)
    request.DONE;
}

function FillRoomsAvailableSectioniPadM(roomsAvailable, slaveiPadID)
{
    roomsAvailable.room.forEach(room => {
        let newRoomBtn = document.createElement("div")
        newRoomBtn.id = `${room.ipAddress}:${room.roomID}:${slaveiPadID}:${room.name}`
        newRoomBtn.innerHTML = room.name
        newRoomBtn.classList.add('room-btn-roaming')
        newRoomBtn.classList.add('btn-generic')
        newRoomBtn.classList.add('centered')

        document.getElementById("roomSelectMainSection").appendChild(newRoomBtn)
    });

    //prevent room selection right after openning page
    setTimeout(() => {
        ActivateRoomSelectRoamingBtns()
    }, 200);
}

function ActivateRoomSelectRoamingBtns()
{
    let roomBtns = document.querySelectorAll(".room-btn-roaming")

    roomBtns.forEach(roomBtn => {
        roomBtn.addEventListener("touchstart", function(){
            roomBtn.classList.add("btn-generic-pressed")
        }, {passive: "true"})
        roomBtn.addEventListener("touchend", function(){
            roomBtn.classList.remove("btn-generic-pressed")
        })
        roomBtn.addEventListener("click", function(){
            roomBtn.classList.remove("btn-generic-pressed")

            var result = CoreProcessorAjaxGETCall("RoamingiPadRoomChange", [roomBtn.id])
            if(result.request == "conplete")
                openSubpage("iPadM-Roaming-iPads", "Roaming iPads", 'fa-solid fa-tablet-screen-button')
        })
    });
}