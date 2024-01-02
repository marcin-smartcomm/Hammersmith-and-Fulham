let masterRoomSelected;

function InitializeCombineRoomsSp()
{ 
  masterRoomSelected = null;

  let availableFloors = ["0", "1", "2", "3", "5"]

  for(let i = 0; i < availableFloors.length; i++)
  {
    let floorRooms = GetAllRoomsForFloor(availableFloors[i])
    ProcessFloorRooms(floorRooms, availableFloors[i])
  }
}

function GetAllRoomsForFloor(floorNum)
{
  var roomsAvailable;
  var request = new XMLHttpRequest();
  request.open("GET", `./availableRooms/floor${floorNum}.json`, false);
  request.onreadystatechange = function ()
  {
      if(request.readyState === 4)
          if(request.status === 200 || request.status == 0)
          {
            roomsAvailable = JSON.parse(request.responseText);
          }
  }
  request.send(null)
  request.DONE;

  return roomsAvailable;
}

function ProcessFloorRooms(roomsAvailable, floor)
{
  let roomList = document.getElementById("combineRoomsRoomList")

  roomsAvailable.room.forEach(room => {
    if(room.isGroupable)
    {
      let roomBtn = document.createElement("div")
      roomBtn.classList.add("btn-generic")
      roomBtn.classList.add("centered")
      roomBtn.classList.add("combine-room-room-btn")
      roomBtn.innerHTML = `Floor ${floor}<br>${room.name}`

      roomBtn.id = `${room.ipAddress}:${room.roomID}`

      roomList.appendChild(roomBtn)

      ActivateRoomBtn(roomBtn, roomsAvailable)
    }
  });
}

let slaveRoomList;
function ActivateRoomBtn(roomBtn, roomsAvailable)
{
  slaveRoomList = [];

  roomBtn.addEventListener('touchstart', function(){
    if(masterRoomSelected == null)
      roomBtn.classList.add('btn-generic-pressed')
  }, { passive: "true" })
  roomBtn.addEventListener('touchend', function(){
      PlayBtnClickSound()

      if(masterRoomSelected == null)
        roomBtn.classList.remove('btn-generic-pressed')
  })
  roomBtn.addEventListener("click", function(){
    if(masterRoomSelected == null)
    {
      document.getElementById("masterRoomSelected").innerHTML = roomBtn.innerHTML.replace("Floor 0", "").replace("Floor 1", "").replace("Floor 2", "").replace("Floor 3", "").replace("Floor 4", "").replace("Floor 5", "").replace("<br>", "");

      document.getElementById("combineRoomsTopSectionText").innerHTML = "Select Secondary Rooms";

      masterRoomSelected = roomBtn;

      document.getElementById(roomBtn.id).remove()
    }
    else
    {
      if(!slaveRoomList.includes(roomBtn))
      {
        roomBtn.classList.add("btn-generic-pressed")
        slaveRoomList.push(roomBtn)
      }
      else
      {
        for(let i = 0; i < slaveRoomList.length; i++)
        {
          if(slaveRoomList[i] == roomBtn)
          {
            slaveRoomList.splice(i, 1)
            roomBtn.classList.remove("btn-generic-pressed")
          }
        }
      }
      
      if(slaveRoomList.length > 0) ShowCombineBtn(roomsAvailable)
      else  HideCombineBtn()
    }
  })
}

function ShowCombineBtn(roomsAvailable)
{
  if(document.getElementById("combineRoomsBtn") !== null) return;

  let topSection = document.getElementById("combineRoomsTopSection")

  let combineBtn = document.createElement('div')
  combineBtn.classList.add("btn-generic")
  combineBtn.classList.add("combine-btn")
  combineBtn.style.position = "absolute"
  combineBtn.innerHTML = "Combine"

  combineBtn.id = "combineRoomsBtn"

  combineBtn.style.top = (topSection.getBoundingClientRect().top+5)+"px";
  combineBtn.style.left = (topSection.getBoundingClientRect().left+5)+"px";

  topSection.prepend(combineBtn)

  ActivateCombineBtn(combineBtn, roomsAvailable)
}

function HideCombineBtn()
{
  document.getElementById("combineRoomsBtn").remove()
}

function ActivateCombineBtn(combineBtn)
{
  combineBtn.addEventListener('touchstart', function(){
    combineBtn.classList.add('btn-generic-pressed')
  }, { passive: "true" })
  combineBtn.addEventListener('touchend', function(){
      PlayBtnClickSound()
      combineBtn.classList.remove('btn-generic-pressed')
  })
  combineBtn.addEventListener("click", function() {

    let roomsToSend = [];
    let availableFloors = ["0", "1", "2", "3", "5"]
    
    for(let i = 0; i < availableFloors.length; i++)
    {
      let floorRooms = GetAllRoomsForFloor(availableFloors[i])
      floorRooms.room.forEach(room => {
          if(masterRoomSelected.id == `${room.ipAddress}:${room.roomID}`)
            roomsToSend.push(room)
      });
    }
    
    for(let i = 0; i < availableFloors.length; i++)
    {
      floorRooms = GetAllRoomsForFloor(availableFloors[i])
      floorRooms.room.forEach(room => {
        slaveRoomList.forEach(slaveRoom => {
          if(slaveRoom.id == `${room.ipAddress}:${room.roomID}`)
            roomsToSend.push(room)
        });
      });
    }
    
    SendRoomsToBeGroupedCall(roomsToSend)

    openSubpage("Combine-Rooms", "Combine Rooms", 'fa-solid fa-gear')
    openPopUp("Grouping-Results", roomsToSend[0].name)

  })
}