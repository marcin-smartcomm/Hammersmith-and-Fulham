let panelType;
let panelSettings;
let coreServerIP;
let serverIP;
let roomAssigned;
let lightingServerIP, lightingAreaNumber;
let currentRoomInfo;
let currentHour, currentMinute;
let assistanceRequired = "false";
let menuItemsBlocked = false;
let btnClickSound = new Audio("./sounds/click.mp3")
let scriptsLoaded = 0;

var db;

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

async function FillOutPanelSettings(setupInfo)
{
    panelType = setupInfo.panelType
    serverIP = setupInfo.serverIP;
    if(panelType == "iPadS") roomAssigned = setupInfo.roomID;

    await InitializePanelSettings()
    PanelBoot()
}

function PlayBtnClickSound()
{
    if(panelSettings.clickSound)
    {
        btnClickSound.pause();
        btnClickSound.currentTime = 0;
        btnClickSound.play();
    }
}

let originalThemeColors = ["black", "rgb(152, 252, 85)", "white", "rgb(152, 252, 85)", "white", "black", "black", "rgb(173, 173, 173)", "white", "rgb(26,26,26)"]
let lightModeColors = ["white", "black", "black", "black", "black", "white", "white", "black", "black", "rgb(215,215,215)"]

var r = document.querySelector(':root')
let currentlySelectedTheme = "dark";

function ChangeToLightMode()
{
    SaveSelectedTheme("light")
    UpdateThemeColors(lightModeColors)

    currentlySelectedTheme = "light"
}

function ChangeToOriginalTheme()
{
    SaveSelectedTheme("dark")
    UpdateThemeColors(originalThemeColors)

    currentlySelectedTheme = "dark"
}

async function SaveSelectedTheme(themeName)
{
    var storedPanelSettings = await db.settings.get(1);
    storedPanelSettings.theme = themeName;

    UpdatePanelSettingsInDB(storedPanelSettings)
}

function UpdateThemeColors(themeColors)
{
    r.style.setProperty('--bg-color', themeColors[0])
    r.style.setProperty('--seperation-line-color', themeColors[1])
    r.style.setProperty('--generic-border-color-inactive', themeColors[2])
    r.style.setProperty('--generic-border-color-active', themeColors[3])
    r.style.setProperty('--generic-text-color-inactive', themeColors[4])
    r.style.setProperty('--generic-text-color-active', themeColors[5])
    r.style.setProperty('--generic-text-color-menu-item-highlight', themeColors[6])
    r.style.setProperty('--generic-text-color-schedule-grey', themeColors[7])
    r.style.setProperty('--generic-text-color-schedule-white', themeColors[8])
    r.style.setProperty('--schedule-bg-color-grey', themeColors[9])
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

async function InitializePanelSettings()
{
    db = new Dexie("PanelSettings");
    
    db.version(1).stores({
    settings: `
        id,
        theme,
        brightness,
        volume,
        clickSound,
        screenTimeout`,
    });

    panelSettings = await db.settings.get(1);
    if(panelSettings == undefined)
    {
        console.log("Filling out default settings");
        db.settings.put(
            { id: 1, theme: "dark", brightness: 50, volume: 50, clickSound: true, screenTimeout: 5 }
        )
        panelSettings = await db.settings.get(1);
    }
    else
    {
        if(panelSettings.theme == "light") ChangeToLightMode();
        else ChangeToOriginalTheme();

        CrComLib.publishEvent('n', 17201, (panelSettings.brightness*655.35)); 
        CrComLib.publishEvent('n', 17307, (panelSettings.volume*655.35));
        CrComLib.publishEvent('n', 17203, panelSettings.screenTimeout);
    }
}

function UpdatePanelSettingsInDB(toBeStored)
{
    db.settings.put(
        { 
            id: 1,
            theme: toBeStored.theme,
            brightness: toBeStored.brightness,
            volume: toBeStored.volume,
            clickSound: toBeStored.clickSound,
            screenTimeout: toBeStored.screenTimeout 
        }
    )

    panelSettings = toBeStored
}