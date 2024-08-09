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

    RoamingiPadAddReturnButton();
    RoamingiPadInitializeReturnButton();
}

function RoamingiPadAddReturnButton()
{
    let topName = $("#pageTopName");
    topName.css("width", "50%")

    let topSection = $("#pageTopSection");
    topSection.prepend(`
            <div class="page-top-section-menu-btn-container">
                <div class="page-top-section-menu-btn btn-generic centered" id="backBtn"><div class="centered-bottom">BACK</div></div>    
            </div>
        `)
}

function RoamingiPadInitializeReturnButton()
{
    let roamingIpadsBtn = document.getElementById("settingsOptionBtn2")

    $('#backBtn').on('touchstart', function () {
        $(this).addClass('btn-generic-pressed')
    });
    $('#backBtn').on('touchend', function () {
        $(this).removeClass('btn-generic-pressed')
    });
    $('#backBtn').on('click', function () {
        PlayBtnClickSound()
        openSubpage("iPadM-Roaming-iPads", "Roaming iPads", 'fa-solid fa-tablet-screen-button')
        UpdateSettingsFb(roamingIpadsBtn.id);
    });
}