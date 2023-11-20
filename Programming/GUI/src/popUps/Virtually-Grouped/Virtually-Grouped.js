function InitalizeVirtuallyGroupedPopUp()
{
  let restoreRoomBtn = document.getElementById("restoreRoomBtn")

  restoreRoomBtn.addEventListener('touchstart', function(){
    restoreRoomBtn.classList.add('btn-generic-pressed')
  }, { passive: "true" })
  restoreRoomBtn.addEventListener('touchend', function(){
      PlayBtnClickSound()
      restoreRoomBtn.classList.remove('btn-generic-pressed')
      RestoreFromVirtualGroupCall(currentRoomInfo.roomID)
  })
}

function FillOutGroupMasterInfo(groupMasterInfo)
{
  document.getElementById("virtualMasterRoom").innerHTML = groupMasterInfo.name;
}