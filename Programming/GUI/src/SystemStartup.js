let panelType;
let coreServerIP;
let serverIP;
let roomAssigned;
let lightingServerIP, lightingAreaNumber;
let currentRoomInfo;
let currentHour, currentMinute;
let assistanceRequired = "false";
let systemClickSoundsEnabled = false;
let menuItemsBlocked = false;
let btnClickSound = new Audio("./sounds/click.mp3")
let scriptsLoaded = 0;

document.onload = WaitForLastScriptToLoad();

function WaitForLastScriptToLoad()
{
    var body = document.getElementsByTagName("body")[0];    
    
    body.addEventListener("load", function(event) {
        if (event.target.nodeName === "SCRIPT")
        {
            //When last script from index.html loads, start system
            if(event.target.getAttribute("src") == "./popUps/Physically-Grouped/Physically-Grouped.js")
                InitializeSystemVariables()
        }
    }, true);
}

function InitializeSystemVariables()
{
    $.ajaxSetup({async: false, timeout: 2000});

    var request = new XMLHttpRequest();
    request.open("GET", './panelSettings.json', false);
    request.onreadystatechange = function ()
    {
        if(request.readyState === 4)
        {
            if(request.status === 200 || request.status == 0)
            {
                var allText = JSON.parse(request.responseText);
                coreServerIP = allText.coreServerIP;
            }
        }
    }
    request.send(null)
    request.DONE;

    FillOutPanelSettings(CoreProcessorAjaxGETCall("PanelInfo", []))
    $("#logo").on('touchend', function () {
        location.reload();
    });
}

function FillOutPanelSettings(setupInfo)
{
    panelType = setupInfo.panelType
    serverIP = setupInfo.serverIP;
    if(panelType == "iPadS") roomAssigned = setupInfo.roomID;

    PanelBoot()
}

function PlayBtnClickSound()
{
    if(systemClickSoundsEnabled)
    {
        btnClickSound.pause();
        btnClickSound.currentTime = 0;
        btnClickSound.play();
    }
}

let inLightMode = false;
let originalThemeColors = ["black", "rgb(152, 252, 85)", "white", "rgb(152, 252, 85)", "white", "black", "black"]
let lightModeColors = ["white", "black", "black", "black", "black", "white", "white"]

var r = document.querySelector(':root')

function ChangeToLightMode()
{
    r.style.setProperty('--bg-color', lightModeColors[0])
    r.style.setProperty('--seperation-line-color', lightModeColors[1])
    r.style.setProperty('--generic-border-color-inactive', lightModeColors[2])
    r.style.setProperty('--generic-border-color-active', lightModeColors[3])
    r.style.setProperty('--generic-text-color-inactive', lightModeColors[4])
    r.style.setProperty('--generic-text-color-active', lightModeColors[5])
    r.style.setProperty('--generic-text-color-menu-item-highlight', lightModeColors[6])
}

function ChangeToOriginalTheme()
{
    r.style.setProperty('--bg-color', originalThemeColors[0])
    r.style.setProperty('--seperation-line-color', originalThemeColors[1])
    r.style.setProperty('--generic-border-color-inactive', originalThemeColors[2])
    r.style.setProperty('--generic-border-color-active', originalThemeColors[3])
    r.style.setProperty('--generic-text-color-inactive', originalThemeColors[4])
    r.style.setProperty('--generic-text-color-active', originalThemeColors[5])
    r.style.setProperty('--generic-text-color-menu-item-highlight', originalThemeColors[6])
}

let assistancePollTimer = false
let assistanceInterval;
function StartAssistancePoll()
{
    if(!assistancePollTimer)
        assistanceInterval = setInterval(() => {
            var result = CoreProcessorAjaxGETCall("RoomAssistanceState", [currentRoomInfo.floor, currentRoomInfo.roomName])
            ProcessAssistanceRequestState(result)
        }, 3000);

    assistancePollTimer = true;
}
function StopAssistancePoll()
{
    if(assistancePollTimer)
        clearInterval(assistanceInterval)

    assistancePollTimer = false;
}
function ProcessAssistanceRequestState(result)
{
    assistanceRequired = result.roomAssistanceState;

    //SystemVariables.js
    if(assistanceRequired == "true" && result.assistanceAcknowledged == "False") StartAssistancePoll()
    if(assistanceRequired == "true" && result.assistanceAcknowledged == "True")
    {
        StopAssistancePoll()
        assistanceRequired = "false"
        openPopUp("Assistance-Coming")

        if(document.getElementById("assistanceBtn") !== null)
        {
            document.getElementById("assistanceBtn").classList.remove("btn-generic-pressed")
        }
    } 
    if(assistanceRequired == "false") StopAssistancePoll()
}