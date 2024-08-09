function InitializePanelSettingsTSWSp()
{
    InitializeScreenBrightnessMenu()
    InitializeVolumeMenu()
    InitializeButtonNoiseMenu()
    InitializeScreenTimeoutMenu()
}

function InitializeScreenBrightnessMenu()
{
    let screenBrtSlider = document.getElementById("screenBrightnessSlider");
    
    screenBrtSlider.oninput = async function() {
        CrComLib.publishEvent('n', 17201, (screenBrtSlider.value*655.35)); 

        var storedPanelSettings = await db.settings.get(1);
        storedPanelSettings.brightness = screenBrtSlider.value;
        UpdatePanelSettingsInDB(storedPanelSettings)
    }
    CrComLib.subscribeState('n', 17201, (value) => {
        screenBrtSlider.value = value/655.35
        screenBrtSlider.style.background = `
        linear-gradient(to right, var(--generic-text-color-inactive) 0%, var(--generic-text-color-inactive) ${screenBrtSlider.value}%, var(--generic-border-color-grey) ${screenBrtSlider.value}%, var(--generic-border-color-grey) 100%)`
    }); 

    CrComLib.publishEvent('n', 17201, (panelSettings.brightness*655.35)); 
    screenBrtSlider.value = panelSettings.brightness

    screenBrtSlider.style.background = `
    linear-gradient(to right, var(--generic-text-color-inactive) 0%, var(--generic-text-color-inactive) ${screenBrtSlider.value}%, var(--generic-border-color-grey) ${screenBrtSlider.value}%, var(--generic-border-color-grey) 100%)`
}

function InitializeVolumeMenu()
{
    let sampleAudio = new Audio('./sounds/ding.mp3')
    let screenVolSlider = document.getElementById("screenVolumeSlider"); 

    screenVolSlider.addEventListener("change",  async function() {
        CrComLib.publishEvent('n', 17307, (screenVolSlider.value*655.35));

        var storedPanelSettings = await db.settings.get(1);
        storedPanelSettings.volume = screenVolSlider.value;
        UpdatePanelSettingsInDB(storedPanelSettings)
    })
    screenVolSlider.addEventListener('touchend', function()
    {
        sampleAudio.play();
    })
    CrComLib.subscribeState('n', 17307, (value) => { 
        screenVolSlider.value = value/655.35
        screenVolSlider.style.background = `
        linear-gradient(to right, var(--generic-text-color-inactive) 0%, var(--generic-text-color-inactive) ${screenVolSlider.value}%, var(--generic-border-color-grey) ${screenVolSlider.value}%, var(--generic-border-color-grey) 100%)`
    }); 

    CrComLib.publishEvent('n', 17307, (panelSettings.volume*655.35));
    screenVolSlider.value = panelSettings.volume

    screenVolSlider.style.background = `
    linear-gradient(to right, var(--generic-text-color-inactive) 0%, var(--generic-text-color-inactive) ${screenVolSlider.value}%, var(--generic-border-color-grey) ${screenVolSlider.value}%, var(--generic-border-color-grey) 100%)`
}

function InitializeButtonNoiseMenu()
{
    let noiseOnBtn = document.getElementById("noiseOnBtn")
    let noiseOffBtn = document.getElementById("noiseOffBtn")

    if(panelSettings.clickSound)
        noiseOnBtn.classList.add("btn-generic-pressed")
    else
        noiseOffBtn.classList.add("btn-generic-pressed")

    noiseOnBtn.addEventListener("touchstart",  function() {
        noiseOnBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    noiseOnBtn.addEventListener("touchend",  async function() {
        var storedPanelSettings = await db.settings.get(1);
        storedPanelSettings.clickSound = true;
        UpdatePanelSettingsInDB(storedPanelSettings)

        UpdateNoiseBtnsFb(noiseOnBtn, noiseOffBtn)
        PlayBtnClickSound()
    })

    noiseOffBtn.addEventListener("touchstart",  function() {
        noiseOffBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    noiseOffBtn.addEventListener("touchend",  async function() {
        var storedPanelSettings = await db.settings.get(1);
        storedPanelSettings.clickSound = false;
        UpdatePanelSettingsInDB(storedPanelSettings)

        UpdateNoiseBtnsFb(noiseOnBtn, noiseOffBtn)
    })
}

function UpdateNoiseBtnsFb(noiseOnBtn, noiseOffBtn)
{
    if(panelSettings.clickSound)
    {
        noiseOnBtn.classList.add("btn-generic-pressed")
        noiseOffBtn.classList.remove("btn-generic-pressed")
    }
    else
    {
        noiseOnBtn.classList.remove("btn-generic-pressed")
        noiseOffBtn.classList.add("btn-generic-pressed")
    }
}

function InitializeScreenTimeoutMenu()
{
    let screenTimeoutValue = document.getElementById("screenTimeoutValue")
    let screenTimeoutUpBtn = document.getElementById("screenTimeoutUp")
    let screenTimeoutDownBtn = document.getElementById("screenTimeoutDown")

    screenTimeoutUpBtn.addEventListener("touchstart",  function() {
        screenTimeoutUpBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    screenTimeoutUpBtn.addEventListener("touchend",  async function() {
        screenTimeoutUpBtn.classList.remove("btn-generic-pressed")

        var storedPanelSettings = await db.settings.get(1);
        storedPanelSettings.screenTimeout += 1;

        if(storedPanelSettings.screenTimeout > 60) storedPanelSettings.screenTimeout = 60;
        CrComLib.publishEvent('n', 17203, storedPanelSettings.screenTimeout);
        screenTimeoutValue.innerHTML = storedPanelSettings.screenTimeout + " min";

        UpdatePanelSettingsInDB(storedPanelSettings)

        PlayBtnClickSound()
        
        resetTimer()
    })

    screenTimeoutDownBtn.addEventListener("touchstart",  function() {
        screenTimeoutDownBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    screenTimeoutDownBtn.addEventListener("touchend",  async function() {
        screenTimeoutDownBtn.classList.remove("btn-generic-pressed")

        var storedPanelSettings = await db.settings.get(1);
        storedPanelSettings.screenTimeout -= 1;

        if(storedPanelSettings.screenTimeout < 0) storedPanelSettings.screenTimeout = 0;
        CrComLib.publishEvent('n', 17203, storedPanelSettings.screenTimeout);
        screenTimeoutValue.innerHTML = storedPanelSettings.screenTimeout + " min";

        UpdatePanelSettingsInDB(storedPanelSettings)

        PlayBtnClickSound()
        
        if(storedPanelSettings.screenTimeout > 0)
            resetTimer()
    })

    screenTimeoutValue.innerHTML = panelSettings.screenTimeout + " min";
}