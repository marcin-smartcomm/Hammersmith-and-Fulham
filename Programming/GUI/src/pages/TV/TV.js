function InitializeTVSp()
{
    GetAvailableFreeviewBoxesCall()
}

function AddFreeviewBtnsTVPage(freeviewBoxes)
{
    freeviewBoxes.boxes.forEach(box => {
        if(currentRoomInfo.signageAccess == true && box.type == "signage") AddTVBoxToList(box)
        if(currentRoomInfo.signageAccess == false && box.type == "freeview") AddTVBoxToList(box)
        if(currentRoomInfo.signageAccess == true && box.type == "freeview") AddTVBoxToList(box)
    });

    InitializeTVPageBtns()
}

function AddTVBoxToList(box)
{
    let btnContainer = document.createElement("div")
    let btn = document.createElement("div")

    btnContainer.classList.add("tv-page-source-btn-container")
    btnContainer.classList.add("centered")

    btn.classList.add("tv-page-source-select-btn")
    btn.classList.add("btn-generic")
    btn.classList.add("centered")
    btn.id = `freeviewBoxBtn${box.cp4IRPortNum}`
    btn.innerHTML = `<div class="centered-bottom">${box.boxName}</div>`

    btnContainer.appendChild(btn)
    document.getElementById("srcBtnsContainer").appendChild(btnContainer)
}

function InitializeTVPageBtns()
{
    let sourceBtns = document.querySelectorAll(".tv-page-source-select-btn")

    sourceBtns.forEach(btn => {
        btn.addEventListener('touchstart', function(){
            btn.classList.add("btn-generic-pressed")
        }, { passive: "true" })
        btn.addEventListener('touchend', function(){
            PlayBtnClickSound()
            btn.classList.remove("btn-generic-pressed")
            NewMenuItemSelectedCall(("Freeview "+btn.id.replace("freeviewBoxBtn", "")))
        })
    });
}

function ProcessFreeviewSourceSelected(result)
{
    console.log(result)
    if(result.freeviewSource == 0)
    {
        openSubpage("TV-Now-Playing", "Now Playing", "btn-generic tv-page-back-btn fa-solid fa-chevron-left centered")
    }
}