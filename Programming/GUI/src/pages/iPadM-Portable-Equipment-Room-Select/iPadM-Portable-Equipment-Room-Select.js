function InitializePortableEquipmentRoomSelectSp(floor, portableEquipmentToChange)
{
    var request = new XMLHttpRequest();
    request.open("GET", `./availableRooms/floor${floor}.json`, false);
    request.onreadystatechange = function ()
    {
        if(request.readyState === 4)
            if(request.status === 200 || request.status == 0)
                var roomsAvailable = JSON.parse(request.responseText);
        
        FillRoomsAvailableSectionPE(roomsAvailable, portableEquipmentToChange)
    }
    request.send(null)
    request.DONE;

    PortableEquipmentAddReturnButton();
    PortableEquipmentInitializeReturnFromRoomSelectButton(portableEquipmentToChange)
}

function FillRoomsAvailableSectionPE(roomsAvailable, portableEquipmentToChange)
{
    roomsAvailable.room.forEach(room => {
        let newRoomBtn = document.createElement("div")
        newRoomBtn.id = `${room.ipAddress}:${room.roomID}:${room.name}`
        newRoomBtn.innerHTML = room.name
        newRoomBtn.classList.add('room-btn-roaming')
        newRoomBtn.classList.add('btn-generic')
        newRoomBtn.classList.add('centered')

        document.getElementById("roomSelectMainSection").appendChild(newRoomBtn)
    });

    //prevent room selection right after openning page
    setTimeout(() => {
        ActivateRoomSelectPortableBtns(portableEquipmentToChange)
    }, 200);
}

function ActivateRoomSelectPortableBtns(portableEquipmentToChange)
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
            CoreProcessorAjaxGETCall("ChangePortableEquipmentAssignment", [portableEquipmentToChange, roomBtn.id.split(':')[0], roomBtn.id.split(':')[1]]);
            openSubpage("iPadM-Portable-Equipment", "Portable Equipment", "fa-solid fa-tv")
        })
    });
}

function PortableEquipmentInitializeReturnFromRoomSelectButton(portableEquipmentToChange)
{
    $('#backBtn').on('touchstart', function () {
        $(this).addClass('btn-generic-pressed')
    });
    $('#backBtn').on('touchend', function () {
        $(this).removeClass('btn-generic-pressed')
    });
    $('#backBtn').on('click', function () {
        PlayBtnClickSound()
        if(portableEquipmentToChange == "All")
            openSubpage("iPadM-Portable-Equipment-Floor-Select", `Portable Equipment`, "fa-solid fa-tv", portableEquipmentToChange)
        else
            openSubpage("iPadM-Portable-Equipment-Floor-Select", `PE - ${portableEquipmentToChange}`, "fa-solid fa-tv", portableEquipmentToChange)
    });
}