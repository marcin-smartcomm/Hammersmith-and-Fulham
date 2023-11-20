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
    
    screenBrtSlider.oninput = function() {
        CrComLib.publishEvent('n', 17201, (screenBrtSlider.value*655.35)); 
    }
    CrComLib.subscribeState('n', 17201, (value) => { 
        screenBrtSlider.value = value/655.35
        screenBrtSlider.style.background = `
        linear-gradient(to right, var(--generic-text-color-inactive) 0%, var(--generic-text-color-inactive) ${screenBrtSlider.value}%, var(--generic-border-color-grey) ${screenBrtSlider.value}%, var(--generic-border-color-grey) 100%)`
    }); 

    screenBrtSlider.style.background = `
    linear-gradient(to right, var(--generic-text-color-inactive) 0%, var(--generic-text-color-inactive) ${screenBrtSlider.value}%, var(--generic-border-color-grey) ${screenBrtSlider.value}%, var(--generic-border-color-grey) 100%)`
}

function InitializeVolumeMenu()
{
    let sampleAudio = new Audio('./sounds/ding.mp3')
    let screenVolSlider = document.getElementById("screenVolumeSlider"); 

    screenVolSlider.addEventListener("change",  function() {
        CrComLib.publishEvent('n', 17307, (screenVolSlider.value*655.35));
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

    screenVolSlider.style.background = `
    linear-gradient(to right, var(--generic-text-color-inactive) 0%, var(--generic-text-color-inactive) ${screenVolSlider.value}%, var(--generic-border-color-grey) ${screenVolSlider.value}%, var(--generic-border-color-grey) 100%)`
}

function InitializeButtonNoiseMenu()
{
    let noiseOnBtn = document.getElementById("noiseOnBtn")
    let noiseOffBtn = document.getElementById("noiseOffBtn")

    if(systemClickSoundsEnabled)
        noiseOnBtn.classList.add("btn-generic-pressed")
    else
        noiseOffBtn.classList.add("btn-generic-pressed")

    noiseOnBtn.addEventListener("touchstart",  function() {
        noiseOnBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    noiseOnBtn.addEventListener("touchend",  function() {
        systemClickSoundsEnabled = true;
        UpdateNoiseBtnsFb(noiseOnBtn, noiseOffBtn)
        PlayBtnClickSound()
    })

    noiseOffBtn.addEventListener("touchstart",  function() {
        noiseOffBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    noiseOffBtn.addEventListener("touchend",  function() {
        systemClickSoundsEnabled = false;
        UpdateNoiseBtnsFb(noiseOnBtn, noiseOffBtn)
    })
}

function UpdateNoiseBtnsFb(noiseOnBtn, noiseOffBtn)
{
    if(systemClickSoundsEnabled)
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
    screenTimeoutUpBtn.addEventListener("touchend",  function() {
        screenTimeoutUpBtn.classList.remove("btn-generic-pressed")

        currentTimeoutValue += 1;
        if(currentTimeoutValue > 60) currentTimeoutValue = 60;
        CrComLib.publishEvent('n', 17203, currentTimeoutValue);
        screenTimeoutValue.innerHTML = currentTimeoutValue + " min";
        PlayBtnClickSound()
        
        resetTimer()
    })

    screenTimeoutDownBtn.addEventListener("touchstart",  function() {
        screenTimeoutDownBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    screenTimeoutDownBtn.addEventListener("touchend",  function() {
        screenTimeoutDownBtn.classList.remove("btn-generic-pressed")

        currentTimeoutValue -= 1;
        if(currentTimeoutValue < 0) currentTimeoutValue = 0;
        CrComLib.publishEvent('n', 17203, currentTimeoutValue);
        screenTimeoutValue.innerHTML = currentTimeoutValue + " min";
        PlayBtnClickSound()
        
        if(currentTimeoutValue > 0)
            resetTimer()
    })

    screenTimeoutValue.innerHTML = currentTimeoutValue + " min";
}