function InitalizeVirtuallyGroupedPopUp()
{
  let restoreRoomBtn = document.getElementById("restoreRoomBtn")

  restoreRoomBtn.addEventListener('touchstart', function(){
    restoreRoomBtn.classList.add('btn-generic-pressed')
  }, { passive: "true" })
  restoreRoomBtn.addEventListener('touchend', function(){
      PlayBtnClickSound()
      restoreRoomBtn.classList.remove('btn-generic-pressed')
      RoomProcessorAjaxGETCall("RestoreFromVirtualGroup", [currentRoomInfo.roomID])
      CheckRoomMasterState(roomID);
  })
}

function FillOutGroupMasterInfo(groupMasterInfo)
{
  document.getElementById("virtualMasterRoom").innerHTML = groupMasterInfo.name;
}

function CheckRoomMasterState()
{
    var result = RoomProcessorAjaxGETCall("GetGroupMasterStatus", [currentRoomInfo.roomID])
    if(result.roomMasterStatus == "True")
    {
        if(document.getElementById("virtuallyGroupedPopUp") == null) openPopUp("Virtually-Grouped")
        FillOutGroupMasterInfo(
            RoomProcessorAjaxGETCall("GetGroupMasterDetails", [currentRoomInfo.roomID])
        )
    }
    if(result.roomMasterStatus == "False")
        if(document.getElementById("virtuallyGroupedPopUp") != null)
        {
            var parentElement = $("#virtuallyGroupedPopUp").parent()
            clearSpecificPopUp("virtuallyGroupedPopUp")
            parentElement.remove()
        }
}