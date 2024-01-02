let previousSubpage;
let currentSubpage;
let sideMenuVis;
let sideMenuCurrentlyDisplayed;
let fireAlarm = false;
let currentTimeoutValue;

function ConnectToSystem(){
    if(panelType == "TSW")
    {
        getRoomDataCall("999")
        setTimeout(() => {
            InitializeHomeScreen()
            CheckRoomMasterCall(currentRoomInfo.roomID)
            GetCurrentDivisionScenarioCall()
            GetLightingProcessorInfoCall(currentRoomInfo.roomID)
            GetCurrentSourceCall(currentRoomInfo.roomID, true)
        }, 1000);
    }
    if(panelType == "iPadM")
    {
        getRoomAssignedCall()
        SubscribeToCoreEvents()
    }
    if(panelType == "iPadS")
    {
        getRoomDataCall(roomAssigned)
        setTimeout(() => {
            InitializeHomeScreen()
            CheckRoomMasterCall(currentRoomInfo.roomID)
            GetLightingProcessorInfoCall(currentRoomInfo.roomID)
            GetCurrentSourceCall(currentRoomInfo.roomID, true)
        }, 1000);
    }
}

let time;

document.addEventListener('touchstart', function(e)
{
    if(currentTimeoutValue > 0)
        resetTimer();
});
function logout() {
    if(!fireAlarm)
    {
        clearPopUps()
        openSubpage("Screensaver");
        DisconnectFromEvents()
    }
}
function resetTimer() {
    clearTimeout(time);
    time = setTimeout(logout, (currentTimeoutValue*60*1000)-1000)
}

function PanelBoot()
{
    if(panelType == "TSW")
    {
        currentTimeoutValue = 5
        CrComLib.publishEvent('n', 17203, currentTimeoutValue);
        OldTimeoutValue = currentTimeoutValue;
        
        resetTimer();
        GetFireAlarmStateCall()
        openSubpage("Screensaver");
    }
    else
    {
        ConnectToSystem();
    }
}

function InitializeHomeScreen()
{
    document.querySelector('#projectBody').innerHTML = "";
    LoadSideMenu("Main");
    openSubpage("Menu")
}

function LoadScreenSaver()
{
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './pages/Screensaver/Screensaver.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var allText = rawFile.responseText;
                document.querySelector('#projectBody').innerHTML = allText;
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;

    InitializeScreenSaverVariables();
}

function LoadSideMenu(menuType)
{
    try
    {
        if(sideMenuVis) document.getElementById("sideMenu").remove()
    }
    catch(e)
    {
        console.log(e)
    }

    sideMenuVis = true;
    sideMenuCurrentlyDisplayed = menuType;

    var rawFile = new XMLHttpRequest();
    if(menuType == "Mic" || menuType == "Volume" || menuType == "Help" || menuType == "FloorList")
        rawFile.open("GET", './pages/Side-Menu/Side-Menu-'+menuType+'.html', false);
    else
        rawFile.open("GET", './pages/Side-Menu/Side-Menu-'+menuType+'-'+panelType+'.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var allText = rawFile.responseText;
                let sideMenuElement = document.createElement("div")
                sideMenuElement.id = "sideMenu"
                sideMenuElement.classList.add("side-menu")
                sideMenuElement.innerHTML = allText
                document.getElementById("projectBody").insertBefore(sideMenuElement, document.getElementById("projectBody").firstChild)
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;
}

function openSubpage(file, param1, pageIcon, param2, param3)
{
  currentSubpage = file;

  var rawFile = new XMLHttpRequest();

  if(file.includes("Help-Page"))
    rawFile.open("GET", './pages/Help-Page/'+file+'.html', false);
  else
    rawFile.open("GET", './pages/'+file+'/'+file+'.html', false);

  rawFile.onreadystatechange = function ()
  {
      if(rawFile.readyState === 4)
      {
          if(rawFile.status === 200 || rawFile.status == 0)
          {

            var allText = rawFile.responseText;

            if(file == "Screensaver" || file == "Scheduling-Page" || file == "Room-Select" || file == "Admin-Assistance" || file == "System-Alerts")
            {
                document.querySelector('#projectBody').innerHTML = allText;
                sideMenuVis = false;
                sideMenuCurrentlyDisplayed = "";
            }
            else
            {
                if(sideMenuVis)
                {
                    if(document.querySelector('#pageContainer') != null)
                    {
                        document.querySelector('#pageContainer').remove();
                        AddPageContainer()
                    }
                    else
                        AddPageContainer()

                    if(file != "Menu")
                    {
                        document.querySelector('#pageCustomSection').innerHTML = allText;
                        InitializeTemplatePageBts()

                        //if(file != "TV-Now-Playing")
                            FillTopOfTemplatePage(param1, pageIcon)
                    }
                    else
                        document.querySelector('#pageContainer').innerHTML = allText;

                    //Side-Menu.js
                    ActivateSideMenuBtns()
                }
                else
                {
                    document.querySelector('#projectBody').innerHTML += allText;
                }

            }
          }
      }
  }
  rawFile.send(null);
  rawFile.DONE;
  
  if(!file.includes("Help-Page"))
    InitializeSubpageVariables(file, param1, pageIcon, param2, param3);
}

function ChangeSubpageToSelectedSource(source)
{
    if(source.sourceName.includes("Freeview")) source.sourceName = "TV"
    currentRoomInfo.menuItems.forEach(item => {
        if(source.sourceName === item.menuItemName) 
            openSubpage(item.menuItemPageAssigned, item.menuItemName, item.menuItemIcon)
    });
}

function openPopUp(file, param1)
{
    if(fireAlarm && file !== "Fire-Alarm") return;

    menuItemsBlocked = true;
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './popUps/'+file+'/'+file+'.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
              var allText = rawFile.responseText;
              var newChild = document.createElement("div")
              newChild.innerHTML = allText

              clearPopUps()

              document.getElementById("mainProjectBody").appendChild(newChild)
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;

    InitializeSubpageVariables(file, param1)
}

function clearPopUps()
{
    var popUps = $(".main-popup-section")
    $.each(popUps, function (i, valueOfElement) { 
      popUps[i].parentElement.remove()
    });
}

function clearSpecificPopUp(popupName)
{
    $(`#${popupName}`).parent().remove()
    setTimeout(() => {
        menuItemsBlocked = false
    }, 250);
}

function AddPageContainer()
{
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './pages/PageTemplate.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var allText = rawFile.responseText;
                document.querySelector('#projectBody').innerHTML += allText;
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;
}

function FillTopOfTemplatePage(pageName, pageIcon)
{
    var iconClasses = pageIcon.split(" ")
    iconClasses.forEach(iconClass => {
        document.getElementById("pageTopIcon").classList.add(iconClass)
    });
    document.getElementById("pageTopName").innerHTML = pageName
}

function InitializeTemplatePageBts()
{
    let menuBtn = document.getElementById("pageMenuBtn")
    let instructionBtn = document.getElementById("instructionBtn")
    let assistanceBtn = document.getElementById("assistanceBtn")

    menuBtn.addEventListener('touchstart', function(){
        menuBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    menuBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        menuBtn.classList.remove('btn-generic-pressed')

        if(GoBackToMainSideMenuCheck())
        {
            LoadSideMenu("Main")
            ActivateSideMenuBtns()
        }
        
        openSubpage("Menu")
    })

    if(panelType == "iPadM")
    {
        document.getElementById("templatePageBottomSection").style.display = "none"
        document.getElementById("pageCustomSection").style.height = "90%"
        return
    }

    instructionBtn.addEventListener('touchstart', function(){
        instructionBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    instructionBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        instructionBtn.classList.remove('btn-generic-pressed')
        LoadSideMenu("Help")
        ActivateSideMenuBtns()
    })

    assistanceBtn.addEventListener('touchstart', function(){
        assistanceBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    assistanceBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        if(assistanceRequired == "false") assistanceBtn.classList.remove('btn-generic-pressed')
        if(assistanceRequired == "false") 
        {
            SendAssistanceCall(currentRoomInfo.floor, currentRoomInfo.roomName);
        }
    })

    ProcessAssitanceState()
}

function GoBackToMainSideMenuCheck()
{
    let foundMatch = false;
    let listOfPages = 
    ["Panel-Settings", "Help-Page", "Global-Temp",
     "Roaming-iPads", "Portable-Equipment", "Combine-Rooms", "Screen-Control"];

    listOfPages.forEach(pageName => {
        if(currentSubpage.includes(pageName))
            foundMatch = true;
    });

    return foundMatch;
}

function ProcessAssitanceState()
{
    let assistanceBtn = document.getElementById("assistanceBtn")

    if(assistanceBtn !== null)
    {
        if(assistanceRequired == "true")
            assistanceBtn.classList.add("btn-generic-pressed")
        if(assistanceRequired == "false")
            assistanceBtn.classList.remove("btn-generic-pressed")
    }
}

function InitializeSubpageVariables(file, param1, pageIcon, param2, param3)
{
    //Pages TSW and iPad
    if(file.includes("Scheduling-Page"))
        InitializeSchedulingPageVariables();
    else if (file.includes("Menu"))
        IniitalizeMenuSp()
    else if (file.includes('Temperature-Control'))
        InitalizeTemperatureControlSp()
    else if (file.includes('Lighting'))
        InitializeLightingSp()
    else if (file.includes('TV-Now-Playing'))
        InitializeTVNowPlayingSp()
    else if (file.includes('TV'))
        InitializeTVSp(param1, pageIcon)
    else if (file.includes('PC-Laptop'))
        InitializePCLaptopSp(param1, pageIcon)
    else if (file.includes('PTZ-Control'))
        InitializePTZControlSp(param1, pageIcon)
    else if (file.includes('Panel-Settings-TSW'))
        InitializePanelSettingsTSWSp()
    else if (file.includes('Video-Production'))
        InitializeVideoProductionSp()
    else if (file.includes('Screensaver'))
        InitializeScreenSaverVariables()

    //Pages iPad
    else if (file.includes('iPadM-Roaming-iPads-Room-Select'))
        InitializeRoomSelectRoamingiPadsSp(param2, param3)
    else if (file.includes('Roaming-iPads-Floor-Select'))
        InitializeRoamingiPadFloorSelectSp(param2)
    else if (file.includes('Roaming-iPads'))
        InitializeRoamingiPadSp()
    
    else if (file.includes('Portable-Equipment-Floor-Select'))
        InitializePortableEquipmentFlorSelectSp(param2)
    else if (file.includes('Portable-Equipment-Room-Select'))
        InitializePortableEquipmentRoomSelectSp(param2, param3)
    else if (file.includes('Portable-Equipment'))
        InitializePortableEquipmentSp()

    else if (file.includes("Combine-Rooms"))
        InitializeCombineRoomsSp()
    else if (file.includes("Screen-Control"))
        InitializeScreenControlSp()
    
    else if (file.includes('Room-Select'))
        InitializeRoomSelectSp(param1)
    else if (file.includes('Freeview-Main'))
        InitializeFreeviewMainSp(param2)
    else if (file.includes('Freeview-Keypad'))
        InitializeFreeviewKeypadSp(param2)
    else if (file.includes('Admin-Assistance'))
        InitializeAdminAssistanceSp()
    else if (file.includes('System-Alerts'))
        InitializeSystemAlertsSp()
    else if (file.includes('Global-Temp'))
        InitializeGlobalTempSp()
    else if (file.includes('Collaboration-Screens-Main'))
        InitialzieColabScreensMainSp()
    else if (file.includes('Collaboration-Screens-Sources'))
        InitialzieColabScreensSourcesSp(param2, param3)
    else if (file.includes("Digital-Signage-Control"))
        InitializeDigitalSignageControlSp()

    //PopUps
    else if (file.includes('Assistance-Coming'))
        InitializeAssistanceComingPopUp()
    else if (file.includes('Assistance-Request-Failed'))
        InitializeAssistanceFailedPopUp()
    else if (file.includes('Meeting-Ending'))
        InitializeMeetingEndingPopUp()
    else if (file.includes('Shutdown'))
        InitializeShutdownPopUp()
    else if (file.includes('Acknowledge-requests'))
        InitializeAcknowledgeRequestPopUp()
    else if (file.includes('Change-Password'))
        InitializeChangePassPopUp()
    else if (file.includes('Grouping-Results'))
        InitializeGroupingResultsPopUp(param1)
    else if (file.includes('Virtually-Grouped'))
        InitalizeVirtuallyGroupedPopUp()
    else if (file.includes('Physically-Grouped'))
        InitalizePhysicallyGroupedPopUp()
    else if (file.includes('Password-Input'))
        InitalizePasswordInputPopUp()
}