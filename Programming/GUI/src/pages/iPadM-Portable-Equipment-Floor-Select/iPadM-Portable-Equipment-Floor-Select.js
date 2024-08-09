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

    PortableEquipmentAddReturnButton()
    PortableEquipmentInitializeReturnButton()
}

function PortableEquipmentAddReturnButton()
{
    let topName = $("#pageTopName");
    topName.css("width", "50%")
    topName.css("font-size", "200%")

    let topSection = $("#pageTopSection");
    topSection.prepend(`
            <div class="page-top-section-menu-btn-container">
                <div class="page-top-section-menu-btn btn-generic centered" id="backBtn"><div class="centered-bottom">BACK</div></div>    
            </div>
        `)
}

function PortableEquipmentInitializeReturnButton()
{
    let portableEquipmentBtn = document.getElementById("settingsOptionBtn3")

    $('#backBtn').on('touchstart', function () {
        $(this).addClass('btn-generic-pressed')
    });
    $('#backBtn').on('touchend', function () {
        $(this).removeClass('btn-generic-pressed')
    });
    $('#backBtn').on('click', function () {
        PlayBtnClickSound()
        openSubpage("iPadM-Portable-Equipment", "Portable Equipment", 'fa-solid fa-tv')
        UpdateSettingsFb(portableEquipmentBtn.id);
    });
}