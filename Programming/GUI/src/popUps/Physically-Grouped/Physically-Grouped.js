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