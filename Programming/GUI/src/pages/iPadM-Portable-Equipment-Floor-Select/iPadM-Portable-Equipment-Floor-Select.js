function InitializePortableEquipmentFlorSelectSp(portableEquipmentToChange)
{
    let floorBtns = document.querySelectorAll(".floor-btn")

    floorBtns.forEach(btn => {
        btn.addEventListener("touchstart", function() {
            btn.classList.add("btn-generic-pressed")
        }, {passive: "true"})
        btn.addEventListener("touchend", function() {
            btn.classList.remove("btn-generic-pressed")

            if(portableEquipmentToChange == "All")
                openSubpage("iPadM-Portable-Equipment-Room-Select", `Portable Equipment`, "fa-solid fa-tv", btn.id.split('-')[1], portableEquipmentToChange)
            else
                openSubpage("iPadM-Portable-Equipment-Room-Select", `PE - ${portableEquipmentToChange}`, "fa-solid fa-tv", btn.id.split('-')[1], portableEquipmentToChange)
        })
    });
}