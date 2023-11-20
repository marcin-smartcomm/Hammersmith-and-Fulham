function IniitalizeMenuSp()
{
    AddMenuBtns()
    if(panelType == "iPadM") GetAvailableFreeviewBoxesCall()
}

function AddMenuBtns()
{
    if(currentRoomInfo == null) return

    for(let i = 0; i < currentRoomInfo.menuItems.length; i++)
        AddMenuBtn(i, currentRoomInfo.menuItems[i])

    AddEventListeners()
}
function AddMenuBtn(id, menuItem)
{
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './pages/Menu/Menu-Item.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var allText = rawFile.responseText;
                document.querySelector('#mainMenuItemsContainer').innerHTML += allText;
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;

    let iconClassesSeperate = menuItem.menuItemIcon.split(" ")

    document.getElementById("mainMenuItemGenericID").id = `menuItem${id}`
    document.getElementById(`mainMenuItemIconGenericID`).id = `menuItemIcon${id}`
    document.getElementById(`mainMenuItemTextGenericID`).id = `menuItemText${id}`

    iconClassesSeperate.forEach(iconClass => {
        document.getElementById(`menuItemIcon${id}`).classList.add(iconClass)
    });

    document.getElementById(`menuItemText${id}`).innerHTML = menuItem.menuItemName.replace("-", "/")
}

function AddEventListeners()
{
    for(let i = 0; i < currentRoomInfo.menuItems.length; i++)
    {
        let menuItem = document.getElementById(`menuItem${i}`)
    
        menuItem.addEventListener('touchstart', function(){
            menuItem.classList.add('btn-generic-pressed')
            menuItem.style.color =`white`
        }, { passive: "true" })
        menuItem.addEventListener('touchend', function(){
            menuItem.style.color =`var(--generic-text-color-inactive)`
            menuItem.classList.add('btn-generic-pressed')
            menuItem.classList.remove('btn-generic-pressed')
        })
        menuItem.addEventListener('click', function(){
            if(!menuItemsBlocked)
            {
                openSubpage(currentRoomInfo.menuItems[i].menuItemPageAssigned, currentRoomInfo.menuItems[i].menuItemName, currentRoomInfo.menuItems[i].menuItemIcon)
                NewMenuItemSelectedCall(currentRoomInfo.menuItems[i].menuItemName)
                PlayBtnClickSound()
            }
        })
    }
}

function AddFreeviewBtns(freeviewBoxes)
{
    for(let i = 0; i < freeviewBoxes.boxes.length; i++)
    {
        if(freeviewBoxes.boxes[i].type == "freeview")
        {
            AddFreeviewBtn(freeviewBoxes.boxes[i].cp4IRPortNum, freeviewBoxes.boxes[i].boxName, freeviewBoxes.boxes[i].icons)
            AddFreeviewEventListener(document.getElementById(`freeviewBtn${freeviewBoxes.boxes[i].cp4IRPortNum}`), freeviewBoxes.boxes[i].boxName)
        }
    }
}

function AddFreeviewBtn(id, name, iconClasses)
{
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", './pages/Menu/Menu-Item-Freeview.html', false);
    rawFile.onreadystatechange = function ()
    {
        if(rawFile.readyState === 4)
        {
            if(rawFile.status === 200 || rawFile.status == 0)
            {
                var newChild = document.createElement("div")
                newChild.id = "mainMenuItemGenericID"
                newChild.classList.add('main-menu-item')
                newChild.classList.add('centered')

                var allText = rawFile.responseText;
                newChild.innerHTML = allText;
                document.querySelector('#mainMenuItemsContainer').appendChild(newChild);
            }
        }
    }
    rawFile.send(null);
    rawFile.DONE;

    let iconClassesSeperate = iconClasses.split(" ")

    document.getElementById("mainMenuItemGenericID").id = `freeviewBtn${id}`
    document.getElementById(`mainMenuItemIconGenericID`).id = `freeviewIcon${id}`
    document.getElementById(`mainMenuItemTextGenericID`).id = `freeviewText${id}`

    iconClassesSeperate.forEach(iconClass => {
        document.getElementById(`freeviewIcon${id}`).classList.add(iconClass)
    });

    document.getElementById(`freeviewText${id}`).innerHTML = name
}

function AddFreeviewEventListener(btn, freeviewName)
{
    btn.addEventListener("touchstart", function() {
        btn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    btn.addEventListener("touchend", function() {
        btn.classList.remove("btn-generic-pressed")
    }, {passive: "true"})
    btn.addEventListener("click", function() {
        openSubpage("Freeview-Main", `TV: ${freeviewName}`, 'fa-solid fa-tv', btn.id)
    }, {passive: "true"})
}