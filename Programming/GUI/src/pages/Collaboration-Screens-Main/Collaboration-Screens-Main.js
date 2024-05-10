function InitialzieColabScreensMainSp()
{
  PopulateColabDataOnScreen(
    CoreProcessorAjaxGETCall("GetColabScreens", [])
  )
}

function PopulateColabDataOnScreen(receivers)
{
  console.log(receivers)
  let receiversContainer = document.getElementById("colabReceiversContainer")

  receivers.receivers.forEach(receiver => {
    let receiverContainer = document.createElement("div")
    receiverContainer.classList.add('colab-screens-receiver-container', 'centered')
    receiversContainer.appendChild(receiverContainer)
  
    let receiverName = document.createElement("div")
    receiverName.classList.add('colab-screens-receiver-text')
    receiverName.innerHTML = receiver.receiverName + " - " + receiver.transmitterAssigned

    let changeSourceBtn = document.createElement("div")
    changeSourceBtn.classList.add('colab-screens-change-source-btn', 'btn-generic', 'centered')
    changeSourceBtn.id = receiver.IPID;
    changeSourceBtn.innerHTML = "Change Source"

    receiverContainer.appendChild(receiverName)
    receiverContainer.appendChild(changeSourceBtn)

    changeSourceBtn.addEventListener('touchstart', function() {
      changeSourceBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    changeSourceBtn.addEventListener('touchend', function() {
      changeSourceBtn.classList.remove("btn-generic-pressed")
        openSubpage("Collaboration-Screens-Sources", "Collaboration Sources", "fa-solid fa-tv", receiver.receiverName, receiver.IPID)
    })
    });
}