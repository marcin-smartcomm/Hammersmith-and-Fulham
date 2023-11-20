function InitializeRoomSelectSp(floorNum)
{
    var request = new XMLHttpRequest();
    request.open("GET", `./availableRooms/floor${floorNum}.json`, false);
    request.onreadystatechange = function ()
    {
        if(request.readyState === 4)
            if(request.status === 200 || request.status == 0)
                var roomsAvailable = JSON.parse(request.responseText);
        
        FillRoomsAvailableSection(roomsAvailable)
    }
    request.send(null)
    request.DONE;
}

function FillRoomsAvailableSection(roomsAvailable)
{
    roomsAvailable.room.forEach(room => {
        let newRoomBtn = document.createElement("div")
        newRoomBtn.id = `${room.ipAddress}:${room.roomID}`
        newRoomBtn.innerHTML = room.name
        newRoomBtn.classList.add('room-btn')
        newRoomBtn.classList.add('btn-generic')
        newRoomBtn.classList.add('centered')

        document.getElementById("roomSelectMainSection").appendChild(newRoomBtn)
    });

    ActivateRoomSelectSpBtns()
}

function ActivateRoomSelectSpBtns()
{
    let backBtn = document.getElementById("roomSelectPageBackBtn")
    let roomBtns = document.querySelectorAll(".room-btn")

    backBtn.addEventListener("touchstart", function()
    {
        backBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    backBtn.addEventListener("touchend", function()
    {
        backBtn.classList.remove("btn-generic-pressed")
        try
        {
            if(currentRoomInfo.roomID !== null)
            {
                document.getElementById("projectBody").innerHTML = "";
                LoadSideMenu("Main")
                openSubpage("Menu")
            }
        }
        catch(e)
        {
            document.getElementById("projectBody").innerHTML = ""
            LoadSideMenu("FloorList")
            ActivateSideMenuBtns()
        }
    })

    roomBtns.forEach(roomBtn => {
        roomBtn.addEventListener("touchstart", function(){
            roomBtn.classList.add("btn-generic-pressed")
        }, {passive: "true"})
        roomBtn.addEventListener("touchend", function(){
            roomBtn.classList.remove("btn-generic-pressed")

            if(panelType == "iPadM")
            {
                DisplayConnectingSubpage()
                currentRoomInfo = null;
                RoomChangeCall(roomBtn.id)
                serverIP = roomBtn.id.split(':')[0]
            }

            if(panelType == "iPadS")
            {
                DisplayConnectingSubpage()
                currentRoomInfo = null;
                SlaveRoomChangeCall(roomBtn.id)
                serverIP = roomBtn.id.split(':')[0]
                getRoomDataCall(roomBtn.id.split(':')[1])
            }
        })
    });
}

function DisplayConnectingSubpage()
{
    var mainProjectBody = document.getElementById("mainProjectBody")

    var connectingPopUp = document.createElement("div")
    connectingPopUp.style.position = "absolute";
    connectingPopUp.style.width = "100%";
    connectingPopUp.style.height = "100%";
    connectingPopUp.style.top = "0"
    connectingPopUp.classList.add("centered")
    connectingPopUp.style.backgroundColor = "#00000080"
    connectingPopUp.id = "connectingPopUp"

    var connectingElement = document.createElement("div")
    connectingElement.innerHTML = "Connecting ...";
    connectingElement.style.color = "white";
    connectingElement.style.fontSize = "250%";
    connectingElement.style.margin = "auto"

    connectingPopUp.appendChild(connectingElement)
    mainProjectBody.appendChild(connectingPopUp)
}

function ClearConnectingPopUp()
{
    if(document.getElementById("connectingPopUp") != null)
        $("#connectingPopUp").remove();
}