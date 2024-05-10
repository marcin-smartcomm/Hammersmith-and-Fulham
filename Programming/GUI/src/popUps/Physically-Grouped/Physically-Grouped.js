function InitalizePhysicallyGroupedPopUp()
{
  
}

function HideGroupedPopUp()
{
  if(document.getElementById("physicallyGroupedPopUp") != null)
  {
    var parentElement = $("#physicallyGroupedPopUp").parent()
    clearSpecificPopUp("physicallyGroupedPopUp")
    parentElement.remove()
  }
}

function GetCurrentDivisionScenario()
{
  var result = RoomProcessorAjaxGETCall("GetDivisionInfo", [])

  $.each(result.roomIDsNotInPlay, function (i, valueOfElement) { 
       if(currentRoomInfo.roomID === result.roomIDsNotInPlay[i])
       {
          HideGroupedPopUp()
          return;
       }
  });

  $.each(result.slaveRoomIDs, function (i, valueOfElement) { 
       if(currentRoomInfo.roomID === result.slaveRoomIDs[i])
       {
          if(document.getElementById("physicallyGroupedPopUp") == null) 
          openPopUp("Physically-Grouped")
          return;
       }
  });

  if(currentRoomInfo.roomID === result.masterRoomID) HideGroupedPopUp()
  if(result.masterRoomID === -1) HideGroupedPopUp()
}