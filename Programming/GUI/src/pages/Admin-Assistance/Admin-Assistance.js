let adminAssistanceInitialized = false;

function InitializeAdminAssistanceSp()
{
    ProcessAssistanceRequests(CoreProcessorAjaxGETCall("RoomAssistanceRequests", []))
}

function ProcessAssistanceRequests(result)
{
    ProcessAdminAssistanceEntries(result)
 
    //Side-Menu.js
    ProcessAdminAssistanceData(result)
}

function ProcessAdminAssistanceEntries(allEntries)
{
    if(currentSubpage !== "Admin-Assistance") return;

    if(!adminAssistanceInitialized)
    {
        let backBtn = document.getElementById("adminAssistancePageBackBtn")
        let clearBtn = document.getElementById("adminAssistancePageClearBtn")

        backBtn.addEventListener('touchstart', function() {
            backBtn.classList.add("btn-generic-pressed")
        }, {passive: "true"})
        backBtn.addEventListener('touchend', function() {
            backBtn.classList.remove("btn-generic-pressed")
            adminAssistanceInitialized = false
            InitializeHomeScreen()
        })

        clearBtn.addEventListener('touchstart', function() {
            clearBtn.classList.add("btn-generic-pressed")
        }, {passive: "true"})
        clearBtn.addEventListener('touchend', function() {
            clearBtn.classList.remove("btn-generic-pressed")
            ClearAssistanceRequestsCall()
        })
        
        adminAssistanceInitialized = true
    }

    if(allEntries.cards < 1) return;

    for(let i = 0; i < allEntries.cards.length; i++)
        AddAssistanceEntry(allEntries.cards[i], i)
}

function ClearAssistanceRequestsCall()
{
    var result = CoreProcessorAjaxGETCall("ClearAssistanceRequests", [])

    if(result.allRequestsAck == "true")
    {
        if(document.getElementById("adminAssistanceEntriesSection") !== null)
            document.getElementById("adminAssistanceEntriesSection").innerHTML = ""
        ProcessAssistanceRequests(CoreProcessorAjaxGETCall("RoomAssistanceRequests", []))
    }

    if(result.allRequestsAck == "False") openPopUp("Acknowledge-requests")
}

function AddAssistanceEntry(entryData, id)
{
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './pages/Admin-Assistance/Admin-Assistance-Entry.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var allText = rawFile.responseText;

                let entryElement = document.createElement("div")
                entryElement.classList.add("admin-assistance-entry")
                entryElement.innerHTML = allText

                document.getElementById("adminAssistanceEntriesSection").appendChild(entryElement)
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;

    let dateText = document.getElementById("entryDate")
    let timeText = document.getElementById("entryTime")
    let roomName = document.getElementById("entryRoomName")
    let checkbox = document.getElementById("entryAck")

    dateText.innerHTML = entryData.date
    timeText.innerHTML = entryData.time
    roomName.innerHTML = entryData.roomName

    dateText.id = `entryDate${id+1}`
    timeText.id = `entryTime${id+1}`
    if(!entryData.requestAcknowledged)
        roomName.style.color = `red`
    roomName.id = `entryRoomName${id+1}`

    if(entryData.requestAcknowledged)
    {
        checkbox.checked = true
        checkbox.disabled = true
    }
    else
        checkbox.checked = false

    checkbox.addEventListener("change", function() {
        if(this.checked) {
            checkbox.checked = true
            checkbox.disabled = true
            roomName.style.color = "var(--generic-text-color-inactive)"
            CoreProcessorAjaxGETCall("AcknowledgeAssistanceRequest", [entryData.requestID])
        }
    })

    checkbox.id = `entryAck${id+1}`
}