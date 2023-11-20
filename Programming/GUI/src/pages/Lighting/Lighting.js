function InitializeLightingSp()
{
    InitializeLightingPageBtns()
    GetCurrentLightingSceneCall()
}

function InitializeLightingPageBtns()
{
    let lightingOnBtn = document.getElementById("lightingOnBtn")
    let lightingOffBtn = document.getElementById("lightingOffBtn")
    // let lightingRaiseBtn = document.getElementById("lightingRaiseBtn")
    // let lightingLowerBtn = document.getElementById("lightingLowerBtn")
    let lightingSceneBtns = document.querySelectorAll(".lighting-page-scene-controls-btn")

    lightingOnBtn.addEventListener('touchstart', function(){
        lightingOnBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    lightingOnBtn.addEventListener('touchend', function(){
        lightingOnBtn.classList.remove("btn-generic-pressed")
        PlayBtnClickSound()
        SetNewLightingSceneCall(lightingOnBtn.getAttribute("name"))
    })

    lightingOffBtn.addEventListener('touchstart', function(){
        lightingOffBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    lightingOffBtn.addEventListener('touchend', function(){
        lightingOffBtn.classList.remove("btn-generic-pressed")
        PlayBtnClickSound()
        SetNewLightingSceneCall(lightingOffBtn.getAttribute("name"))
    })

    // lightingRaiseBtn.addEventListener('touchstart', function(){
    //     lightingRaiseBtn.classList.add("btn-generic-pressed")
    // }, { passive: "true" })
    // lightingRaiseBtn.addEventListener('touchend', function(){
    //     lightingRaiseBtn.classList.remove("btn-generic-pressed")
    //     PlayBtnClickSound()
    // })

    // lightingLowerBtn.addEventListener('touchstart', function(){
    //     lightingLowerBtn.classList.add("btn-generic-pressed")
    // }, { passive: "true" })
    // lightingLowerBtn.addEventListener('touchend', function(){
    //     lightingLowerBtn.classList.remove("btn-generic-pressed")
    //     PlayBtnClickSound()
    // })

    lightingSceneBtns.forEach(sceneBtn => {
        sceneBtn.addEventListener('touchstart', function(){
            sceneBtn.classList.add("btn-generic-pressed")
        }, { passive: "true" })
        sceneBtn.addEventListener('touchend', function(){
            sceneBtn.classList.remove("btn-generic-pressed")
            PlayBtnClickSound()
            SetNewLightingSceneCall(sceneBtn.getAttribute("name"))
        })
    });
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