let selectedReceiverIPID;

function InitialzieColabScreensSourcesSp(param2, param3)
{
  GetColabSourcesCall()
  selectedReceiverIPID = param3
  document.getElementById("pageTopName").innerHTML = param2
}

function PopulateColabSourcesOnScreen(sources)
{
  sources.transmitters.forEach(source => {
    let sourceBtn = document.createElement("div")
    sourceBtn.classList.add('colab-source-btn', 'btn-generic', 'centered')
    sourceBtn.innerHTML = source.transmitterName;
    sourceBtn.id = source.IPID;
  
    document.getElementById("colabSourcesContainer").appendChild(sourceBtn)

    sourceBtn.addEventListener('touchstart', function() {
      sourceBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    sourceBtn.addEventListener('touchend', function() {
      sourceBtn.classList.remove("btn-generic-pressed")
      SetColabSourceCall(selectedReceiverIPID, source.IPID)
      openSubpage("Collaboration-Screens-Main", "Collaboration Screens", "fa-solid fa-tv")
    })
  });
}