let masterRoomName = "";

function InitializeGroupingResultsPopUp(masterRoom)
{
  masterRoomName = masterRoom;
  let closeBtn = document.getElementById("groupingResultsCloseBtn")

  closeBtn.addEventListener('touchstart', function(){
      closeBtn.classList.add('btn-generic-pressed')
  }, {passive: "true"})
  closeBtn.addEventListener('touchend', function(){
      PlayBtnClickSound()
      closeBtn.classList.remove('btn-generic-pressed')
      clearSpecificPopUp("popUpSection")
  })
}

function UpdateGroupingResults(results)
{
  let rawResults = JSON.parse(results)
  let textBox = document.getElementById("resultsTextBox")
  if(textBox == null) return;
  
  document.getElementById("resultsInfoText").innerHTML = "Grouping with " + masterRoomName + " Results"

  rawResults.forEach(result => {
    let newEntry = document.createElement("div")
    newEntry.classList.add("grouping-entry")

    let newEntryRoom = document.createElement("span")
    newEntryRoom.classList.add("grouping-entry-room")
    newEntryRoom.innerHTML = result.roomName

    let newEntryStatus = document.createElement("span")
    newEntryStatus.classList.add("centered")
    if(result.groupStatus.includes("Success"))
      newEntryStatus.classList.add("grouping-entry-status-green")
    else
      newEntryStatus.classList.add("grouping-entry-status-red")
    newEntryStatus.innerHTML = result.groupStatus
    
    newEntry.appendChild(newEntryRoom)
    newEntry.appendChild(newEntryStatus)

    textBox.appendChild(newEntry)
  });
}