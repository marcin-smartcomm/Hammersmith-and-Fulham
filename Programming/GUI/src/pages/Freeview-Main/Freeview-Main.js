let numOfFreeviewBtns = 13;

function InitializeFreeviewMainSp(freeviewID)
{
    let keypadBtn = document.getElementById("freeviewMainKepadBtn")
    let changeNameBtn = document.getElementById("freeviewMainChangeNameBtn")

    keypadBtn.addEventListener("touchstart", function() {
        keypadBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    keypadBtn.addEventListener("touchend", function() {
        keypadBtn.classList.remove("btn-generic-pressed")
        openSubpage("Freeview-Keypad", document.getElementById("pageTopName").innerHTML, "fa-solid fa-tv", freeviewID)
    })

    changeNameBtn.addEventListener("touchstart", function() {
        changeNameBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    changeNameBtn.addEventListener("touchend", function() {
        changeNameBtn.classList.remove("btn-generic-pressed")
        InitializeBoxNameChange(freeviewID)
    })

    for(let i = 0; i < numOfFreeviewBtns; i++)
    {
        let btn = document.getElementById(`freeviewCtrlBtn${i}`)

        //if keypad btns
        if(i >= 2 && i <=6)
        {
            btn.addEventListener("touchstart", function() {
                btn.classList.add("freeview-main-btn-keypad-pressed")
            }, {passive: "true"})
            btn.addEventListener("touchend", function() {
                btn.classList.remove("freeview-main-btn-keypad-pressed")
                FreeviewBtnPressCall(freeviewID.replace("freeviewBtn", ""), btn.id.replace("freeviewCtrlBtn", ""));
            })
        }
        else
        {
            btn.addEventListener("touchstart", function() {
                btn.classList.add("btn-generic-pressed")
            }, {passive: "true"})
            btn.addEventListener("touchend", function() {
                btn.classList.remove("btn-generic-pressed")
                FreeviewBtnPressCall(freeviewID.replace("freeviewBtn", ""), btn.id.replace("freeviewCtrlBtn", ""));
            })
        }
    }
}

function InitializeBoxNameChange(freeviewID)
{
    let pageName = document.getElementById("pageTopName");
    pageName.innerHTML = "TV: ";
    pageName.style.width = "10%";

    let inputField = document.createElement("input")
    inputField.id = "freeviewMainPageInputField"
    inputField.classList.add("freeview-main-page-input-field")

    pageName.after(inputField)

    inputField.focus()
    inputField.click()

    document.getElementById("freeviewMainPageInputField").addEventListener("keypress", function(evt)
    {
        //if enter pressed
        if(evt.code == "Enter")
        {
            let newName = inputField.value;
            
            inputField.remove()
            pageName.style.width = "70%"
            pageName.innerHTML = `TV: Saving ...`

            ChangeFreeviewNameCall(freeviewID.replace("freeviewBtn", ""), newName)
        }
    })
}