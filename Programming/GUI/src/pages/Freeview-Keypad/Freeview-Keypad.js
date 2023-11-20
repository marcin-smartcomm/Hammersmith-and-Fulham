let numOfFreeviewKpBtns = 10;

function InitializeFreeviewKeypadSp(freeviewID)
{
    let backBtn = document.getElementById("freeviewKPPageBackBtn")

    backBtn.addEventListener("touchstart", function() {
        backBtn.classList.add("btn-generic-pressed")
    }, {passive: "true"})
    backBtn.addEventListener("touchend", function() {
        backBtn.classList.remove("btn-generic-pressed")
        openSubpage("Freeview-Main", document.getElementById("pageTopName").innerHTML, "fa-solid fa-tv", freeviewID)
    })

    for(let i = numOfFreeviewBtns; i < numOfFreeviewBtns+numOfFreeviewKpBtns; i++)
    {
        let btn = document.getElementById(`freeviewCtrlBtn${i}`)

        btn.addEventListener("touchstart", function() {
            btn.classList.add("btn-generic-pressed")
        }, {passive: "true"})
        btn.addEventListener("touchend", function() {
            btn.classList.remove("btn-generic-pressed")
            FreeviewBtnPressCall(freeviewID.replace("freeviewBtn", ""), btn.id.replace("freeviewCtrlBtn", ""));
        })
    }
}