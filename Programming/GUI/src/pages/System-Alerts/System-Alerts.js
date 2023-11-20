let systemAlertsInitialized = false;

function InitializeSystemAlertsSp()
{
    GetSystemAlertsCall()
}

function ProcessSystemAlertsEntries(allEntries)
{
    if(currentSubpage !== "System-Alerts") return;

    if(!systemAlertsInitialized)
    {
        let backBtn = document.getElementById("systemAlertsPageBackBtn")
        let clearBtn = document.getElementById("systemAlertsPageClearBtn")
    
        backBtn.addEventListener('touchstart', function() {
            backBtn.classList.add("btn-generic-pressed")
        }, {passive: "true"})
        backBtn.addEventListener('touchend', function() {
            backBtn.classList.remove("btn-generic-pressed")
            systemAlertsInitialized = false
            InitializeHomeScreen()
        })
    
        clearBtn.addEventListener('touchstart', function() {
            clearBtn.classList.add("btn-generic-pressed")
        }, {passive: "true"})
        clearBtn.addEventListener('touchend', function() {
            clearBtn.classList.remove("btn-generic-pressed")
            ClearSystemAlertsCall()
        })
        
        systemAlertsInitialized = true
    }

    ClearSystemAlertsEntries()

    if(allEntries.cardList < 1) return;

    for(let i = 0; i < allEntries.cardList.length; i++)
        AddSystemAlertEntry(allEntries.cardList[i])
}

function AddSystemAlertEntry(entryData)
{
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './pages/System-Alerts/System-Alerts-Entry.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var allText = rawFile.responseText;

                let entryElement = document.createElement("div")
                entryElement.classList.add("system-alerts-entry")
                entryElement.innerHTML = allText

                document.getElementById("systemAlertsEntriesSection").appendChild(entryElement)
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;

    let dateText = document.getElementById("entryDate")
    let timeText = document.getElementById("entryTime")
    let roomName = document.getElementById("entryRoomName")
    let issue = document.getElementById("entryIssue")

    dateText.innerHTML = entryData.date
    timeText.innerHTML = entryData.time
    roomName.innerHTML = entryData.roomName
    issue.innerHTML = entryData.issue

    dateText.id = `entryDate${entryData.alertID}`
    timeText.id = `entryTime${entryData.alertID}`
    roomName.id = `entryRoomName${entryData.alertID}`
    issue.id = `entryIssue${entryData.alertID}`
}

function ClearSystemAlertsEntries()
{
    if(currentSubpage == "System-Alerts")
        document.getElementById("systemAlertsEntriesSection").innerHTML = "";
}