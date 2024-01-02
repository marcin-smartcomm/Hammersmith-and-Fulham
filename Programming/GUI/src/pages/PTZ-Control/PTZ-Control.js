function InitializePTZControlSp(pageName, pageIcon)
{
    GetCamerasCall(currentRoomInfo.roomID)
    InitializePTZControlBtns(pageName, pageIcon)
}

function InitializePTZControlBtns(pageName, pageIcon)
{
    let backBtn = document.getElementById("ptzPageBackBtn")

    let zoomUpBtn = document.getElementById("ptzCameraZoomUp")
    let zoomDownBtn = document.getElementById("ptzCameraZoomDown")

    let keypadBtnUp = document.getElementById("ptzCameraUp")
    let keypadBtnLeft = document.getElementById("ptzCameraLeft")
    let keypadBtnRight = document.getElementById("ptzCameraRight")
    let keypadBtnDown = document.getElementById("ptzCameraDown")

    let keypadBtnUpLeft = document.getElementById("ptzCameraUpLeft")
    let keypadBtnUpRight = document.getElementById("ptzCameraUpRight")
    let keypadBtnDownLeft = document.getElementById("ptzCameraDownLeft")
    let keypadBtnDownRight = document.getElementById("ptzCameraDownRight")

    let presetBtns = document.querySelectorAll(".ptz-camera-page-preset-btn")

    backBtn.addEventListener('touchstart', function(){
        backBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    backBtn.addEventListener('touchend', function(){
        backBtn.classList.remove("btn-generic-pressed")
        openSubpage("PC-Laptop", pageName, pageIcon)
        PlayBtnClickSound()
    })

    zoomUpBtn.addEventListener('touchstart', function(){
        zoomUpBtn.classList.add("btn-generic-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("zoom+|start", 0)
    }, { passive: "true" })
    zoomUpBtn.addEventListener('touchend', function(){
        zoomUpBtn.classList.remove("btn-generic-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("zoom+|stop", 0)
    })

    zoomDownBtn.addEventListener('touchstart', function(){
        zoomDownBtn.classList.add("btn-generic-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("zoom-|start", 0)
    }, { passive: "true" })
    zoomDownBtn.addEventListener('touchend', function(){
        zoomDownBtn.classList.remove("btn-generic-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("zoom-|stop", 0)
    })
    
    keypadBtnUp.addEventListener('touchstart', function(){
        keypadBtnUp.classList.add("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("up|start", 50)
    }, { passive: "true" })
    keypadBtnUp.addEventListener('touchend', function(){
        keypadBtnUp.classList.remove("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("up|stop", 50)
    })
    keypadBtnDown.addEventListener('touchstart', function(){
        keypadBtnDown.classList.add("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("down|start", 50)
    }, { passive: "true" })
    keypadBtnDown.addEventListener('touchend', function(){
        keypadBtnDown.classList.remove("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("down|stop", 50)
    })
    keypadBtnLeft.addEventListener('touchstart', function(){
        keypadBtnLeft.classList.add("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("left|start", 50)
    }, { passive: "true" })
    keypadBtnLeft.addEventListener('touchend', function(){
        keypadBtnLeft.classList.remove("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("left|stop", 50)
    })
    keypadBtnRight.addEventListener('touchstart', function(){
        keypadBtnRight.classList.add("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("right|start", 50)
    }, { passive: "true" })
    keypadBtnRight.addEventListener('touchend', function(){
        keypadBtnRight.classList.remove("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("right|stop", 50)
    })
    
    keypadBtnUpLeft.addEventListener('touchstart', function(){
        keypadBtnUpLeft.classList.add("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("leftup|start", 50)
    }, { passive: "true" })
    keypadBtnUpLeft.addEventListener('touchend', function(){
        keypadBtnUpLeft.classList.remove("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("leftup|stop", 50)
    })
    keypadBtnUpRight.addEventListener('touchstart', function(){
        keypadBtnUpRight.classList.add("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("rightup|start", 50)
    }, { passive: "true" })
    keypadBtnUpRight.addEventListener('touchend', function(){
        keypadBtnUpRight.classList.remove("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("rightup|stop", 50)
    })
    keypadBtnDownLeft.addEventListener('touchstart', function(){
        keypadBtnDownLeft.classList.add("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("leftdown|start", 50)
    }, { passive: "true" })
    keypadBtnDownLeft.addEventListener('touchend', function(){
        keypadBtnDownLeft.classList.remove("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("leftdown|stop", 50)
    })
    keypadBtnDownRight.addEventListener('touchstart', function(){
        keypadBtnDownRight.classList.add("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("rightdown|start", 50)
    }, { passive: "true" })
    keypadBtnDownRight.addEventListener('touchend', function(){
        keypadBtnDownRight.classList.remove("btn-keypad-pressed")
        PlayBtnClickSound()
        ProcessCameraCommand("rightdown|stop", 50)
    })

    presetBtns.forEach(btn => {
        btn.addEventListener('touchstart', function(){
            btn.classList.add("btn-generic-pressed")
        }, { passive: "true" })
        btn.addEventListener('touchend', function(){
            btn.classList.remove("btn-generic-pressed")
            PlayBtnClickSound()
            ProcessCameraCommand("preset|recall", btn.id.replace("preset", ""))
        })
    });
}

function AddCamerasToList(cameras)
{
    cameras.forEach(camera => {
        $('#ptzcamSelectBtn').html($('#ptzcamSelectBtn').html()+`<option class="cam-select-option" value="${camera.name}" selected>${camera.name}</option>`)
    });
}

function ProcessCameraCommand(command, byValue)
{
    let camName = $('#ptzcamSelectBtn').find(":selected").text()
    CameraControlCall(currentRoomInfo.roomID, camName, command, byValue)
}