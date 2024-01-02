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

document.onload = InitializeSystemVariables();

function InitializeSystemVariables()
{
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

    GetPanelSettingsCall()
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
            GetRoomAssistanceStateCall(currentRoomInfo.floor, currentRoomInfo.roomName)
        }, 3000);

    assistancePollTimer = true;
}
function StopAssistancePoll()
{
    if(assistancePollTimer)
        clearInterval(assistanceInterval)

    assistancePollTimer = false;
}