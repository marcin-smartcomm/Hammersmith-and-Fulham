function InitializeRoamingiPadFloorSelectSp(slaveiPadID)
{
    let floorBtns = document.querySelectorAll(".floor-btn")

    floorBtns.forEach(btn => {
        btn.addEventListener("touchstart", function() {
            btn.classList.add("btn-generic-pressed")
        }, {passive: "true"})
        btn.addEventListener("touchend", function() {
            btn.classList.remove("btn-generic-pressed")
            openSubpage("iPadM-Roaming-iPads-Room-Select", `Roaming iPads (${slaveiPadID})`, "fa-solid fa-tablet-screen-button", btn.id.split('-')[1], slaveiPadID)
        })
    });
}