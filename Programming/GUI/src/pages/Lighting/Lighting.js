function InitializeLightingSp()
{
    InitializeLightingPageBtns()
    UpdateLightingSceneFb(
        LightingProcessorAjaxGETCall("GetCurrentScene", [lightingAreaNumber])
    )
}

function InitializeLightingPageBtns()
{
    let lightingOnBtn = document.getElementById("lightingOnBtn")
    let lightingOffBtn = document.getElementById("lightingOffBtn")
    let lightingSceneBtns = document.querySelectorAll(".lighting-page-scene-controls-btn")

    lightingOnBtn.addEventListener('touchstart', function(){
        lightingOnBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    lightingOnBtn.addEventListener('touchend', function(){
        lightingOnBtn.classList.remove("btn-generic-pressed")
        PlayBtnClickSound()
        SetNewLightingScene(lightingOnBtn.getAttribute("name"))
    })

    lightingOffBtn.addEventListener('touchstart', function(){
        lightingOffBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    lightingOffBtn.addEventListener('touchend', function(){
        lightingOffBtn.classList.remove("btn-generic-pressed")
        PlayBtnClickSound()
        SetNewLightingScene(lightingOffBtn.getAttribute("name"))
    })

    lightingSceneBtns.forEach(sceneBtn => {
        sceneBtn.addEventListener('touchstart', function(){
            sceneBtn.classList.add("btn-generic-pressed")
        }, { passive: "true" })
        sceneBtn.addEventListener('touchend', function(){
            sceneBtn.classList.remove("btn-generic-pressed")
            PlayBtnClickSound()
            SetNewLightingScene(sceneBtn.getAttribute("name"))
        })
    });
}

function SetNewLightingScene(sceneName)
{
    LightingProcessorAjaxGETCall("SetNewScene", [lightingAreaNumber, sceneName])
    UpdateLightingSceneFb(newSceneName)
    setTimeout(() => { 
        UpdateLightingSceneFb(
            LightingProcessorAjaxGETCall("GetCurrentScene", [lightingAreaNumber])
        ) 
    }, 2000);
}

function UpdateLightingSceneFb(newScene)
{
    if(currentSubpage == "Lighting")
    {
        let lightingOnBtn = document.getElementById("lightingOnBtn")
        let lightingOffBtn = document.getElementById("lightingOffBtn")
        let lightingSceneBtns = document.querySelectorAll(".lighting-page-scene-controls-btn")

        lightingOnBtn.classList.remove("btn-generic-pressed")
        lightingOffBtn.classList.remove("btn-generic-pressed")
        lightingSceneBtns.forEach(btn => {
            btn.classList.remove("btn-generic-pressed")
        });

        if(lightingOnBtn.getAttribute("name") == newScene) lightingOnBtn.classList.add("btn-generic-pressed")
        if(lightingOffBtn.getAttribute("name") == newScene) lightingOffBtn.classList.add("btn-generic-pressed")
        lightingSceneBtns.forEach(btn => {
            if(btn.getAttribute("name") == newScene)
                btn.classList.add("btn-generic-pressed")
        });
    }
}

function GetLightingProcessorInfo()
{
    var result = RoomProcessorAjaxGETCall("GetLightingInfo", [currentRoomInfo.roomID])
    lightingServerIP = result.LightingProcessorIP,
    lightingAreaNumber = result.LightingAreaNumber
}