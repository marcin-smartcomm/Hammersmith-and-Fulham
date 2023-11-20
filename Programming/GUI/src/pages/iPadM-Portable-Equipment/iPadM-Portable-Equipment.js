function InitializePortableEquipmentSp()
{
    let breakUpBtn = document.getElementById("breakUpEquipment")

    CombineEquipment()

    breakUpBtn.addEventListener('touchstart', function() {
        breakUpBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    breakUpBtn.addEventListener('touchend', function() {
        breakUpBtn.classList.remove("btn-generic-pressed")

        if(breakUpBtn.innerHTML == "BREAK UP EQUIPMENT")
        {
            BreakUpEquipmnt()
            breakUpBtn.innerHTML = "COMBINE EQUIPMENT"
            return
        }
        
        if(breakUpBtn.innerHTML == "COMBINE EQUIPMENT")
        {
            CombineEquipment()
            breakUpBtn.innerHTML = "BREAK UP EQUIPMENT"
            return
        }
    })
}

function CombineEquipment()
{
    let container = document.getElementById("portableEquipmentContainer")

    container.innerHTML = "";

    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './pages/iPadM-Portable-Equipment/Portable-Equipment-box.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var allText = rawFile.responseText;

                let entryElement = document.createElement("div")
                entryElement.classList.add("portable-equipment-box")
                entryElement.classList.add("centered")
                entryElement.innerHTML = allText

                container.appendChild(entryElement)
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;

    document.getElementById("boxText").innerHTML = "Portable Equipment Kit"

    let assignRoomBtn = document.getElementById("assignRoomBtn")

    assignRoomBtn.addEventListener('touchstart', function() {
        assignRoomBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    assignRoomBtn.addEventListener('touchend', function() {
        assignRoomBtn.classList.remove("btn-generic-pressed")
        openSubpage("iPadM-Portable-Equipment-Floor-Select", `Portable Equipment`, "fa-solid fa-tv", "All")
    })
}

function BreakUpEquipmnt()
{
    let container = document.getElementById("portableEquipmentContainer")

    container.innerHTML = "";

    for(let i = 0;i < 4; i++)
    {
        var rawFile = new XMLHttpRequest();
        rawFile.open("GET", './pages/iPadM-Portable-Equipment/Portable-Equipment-box.html', false);
        rawFile.onreadystatechange = function ()
        {
            if(rawFile.readyState === 4)
            {
                if(rawFile.status === 200 || rawFile.status == 0)
                {
                    var allText = rawFile.responseText;
    
                    let entryElement = document.createElement("div")
                    entryElement.classList.add("portable-equipment-box")
                    entryElement.classList.add("centered")
                    entryElement.innerHTML = allText
    
                    container.appendChild(entryElement)
                }
            }
        }
        rawFile.send(null);
        rawFile.DONE;

        if(i == 0) document.getElementById("boxText").innerHTML = "TV Display 1"
        if(i == 1) document.getElementById("boxText").innerHTML = "TV Display 2"
        if(i == 2) document.getElementById("boxText").innerHTML = "Audio"
        if(i == 3) document.getElementById("boxText").innerHTML = "Video Input"

        document.getElementById("boxText").id = `boxText${i}`
        document.getElementById("assignRoomBtn").id = `assignRoomBtn${i}`
        
        let assignRoomBtn = document.getElementById(`assignRoomBtn${i}`)
        let equipmentText = document.getElementById(`boxText${i}`).innerHTML;

        assignRoomBtn.addEventListener("touchstart", function(){
            assignRoomBtn.classList.add("btn-generic-pressed")
        }, {passive: "true"})
        assignRoomBtn.addEventListener('touchend', function() {
            assignRoomBtn.classList.remove("btn-generic-pressed")
            openSubpage("iPadM-Portable-Equipment-Floor-Select", `PE - ${equipmentText}`, "fa-solid fa-tv", equipmentText)
        })
    }
}