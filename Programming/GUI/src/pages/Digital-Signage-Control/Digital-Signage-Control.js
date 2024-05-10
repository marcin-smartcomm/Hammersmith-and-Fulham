function InitializeDigitalSignageControlSp()
{
    PopulateDigitalSignageSection(
        CoreProcessorAjaxGETCall("DigitalSignage", [])
    )
}

function PopulateDigitalSignageSection(data)
{
    $(".digital-signage-control-main-container")[0].innerHTML = "";
    data.zones.forEach(zone => {
        var rawFile = new XMLHttpRequest();
        rawFile.open("GET", './pages/Digital-Signage-Control/Digital-Signage-Zone-partial.html', false);
        rawFile.onreadystatechange = function ()
        {
            if(rawFile.readyState === 4)
            {
                if(rawFile.status === 200 || rawFile.status == 0)
                {
                    var allText = rawFile.responseText;
                    $(".digital-signage-control-main-container")[0].innerHTML += allText;

                    $(".digital-signage-control-section-title")[zone.zoneID-1].innerHTML = zone.zoneName;

                    $("#OnBtnX")[0].id = `OnBtn${zone.zoneID}`;
                    $("#OffBtnX")[0].id = `OffBtn${zone.zoneID}`;
                    
                    $("#ShutdownTimeX").text(zone.shutdownTime);
                    $("#ShutdownTimeX")[0].id = `ShutdownTime${zone.zoneID}`;

                    $("#RaiseBtnX")[0].id = `RaiseBtn${zone.zoneID}`;
                    $("#LowerBtnX")[0].id = `LowerBtn${zone.zoneID}`;
                }
            }
        }
        rawFile.send(null);
        rawFile.DONE;
    });

    InitialisePowerSections()
    InitialiseScheduleSections()
}

function InitialisePowerSections()
{
    var allOnBtns = $("[id^=OnBtn]")
    var allOffBtns = $("[id^=OffBtn]")

    $.each(allOnBtns, function (i, btn) { 
        $(btn).on("touchstart", function () {
            if(!$(this).hasClass("processing") && !$(`#OffBtn${i+1}`).hasClass("processing"))
            $(this).addClass("btn-generic-pressed")
        });
        $(btn).on("touchend", function () {
            if(!$(this).hasClass("processing") && !$(`#OffBtn${i+1}`).hasClass("processing"))
            $(this).removeClass("btn-generic-pressed")
        });
        $(btn).on("click", function () {
            if(!$(this).hasClass("processing") && !$(`#OffBtn${i+1}`).hasClass("processing"))
            CoreProcessorAjaxGETCall("DigitalSignage/Power", [btn.id.replace("OnBtn", ""), "On"])
            BlinkDigitalSignagePwrBtn(btn.id.replace("OnBtn", ""), "On")
        });
    });

    $.each(allOffBtns, function (i, btn) { 
        $(btn).on("touchstart", function () {
            if(!$(this).hasClass("processing") && !$(`#OnBtn${i+1}`).hasClass("processing"))
            $(this).addClass("btn-generic-pressed")
        });
        $(btn).on("touchend", function () {
            if(!$(this).hasClass("processing") && !$(`#OnBtn${i+1}`).hasClass("processing"))
            $(this).removeClass("btn-generic-pressed")
        });
        $(btn).on("click", function () {
            if(!$(this).hasClass("processing") && !$(`#OnBtn${i+1}`).hasClass("processing"))
            CoreProcessorAjaxGETCall("DigitalSignage/Power", [btn.id.replace("OffBtn", ""), "Off"])
            BlinkDigitalSignagePwrBtn(btn.id.replace("OffBtn", ""), "Off")
        });
    });
}

function InitialiseScheduleSections()
{
    var allRaiseBtns = $("[id^=RaiseBtn]")
    var allLowerBtns = $("[id^=LowerBtn]")

    $.each(allRaiseBtns, function (i, btn) { 
        $(btn).on("touchstart", function () {
            $(this).addClass("btn-generic-pressed")
        });
        $(btn).on("touchend", function () {
            $(this).removeClass("btn-generic-pressed")
        });
        $(btn).on("click", function () {
            CoreProcessorAjaxGETCall("DigitalSignage/ScheduleOffTime", [btn.id.replace("RaiseBtn", ""), "up"])
        });
    });

    $.each(allLowerBtns, function (i, btn) { 
        $(btn).on("touchstart", function () {
            $(this).addClass("btn-generic-pressed")
        });
        $(btn).on("touchend", function () {
            $(this).removeClass("btn-generic-pressed")
        });
        $(btn).on("click", function () {
            CoreProcessorAjaxGETCall("DigitalSignage/ScheduleOffTime", [btn.id.replace("LowerBtn", ""), "down"])
        });
    });
}

async function BlinkDigitalSignagePwrBtn(btnID, powerState)
{
    let blockForIterations = 20
    $(`#${powerState}Btn${btnID}`).addClass("processing")
    while(blockForIterations > 0)
    {
        await delay(500).then(() => {
            if(blockForIterations%2 == 0)
                $(`#${powerState}Btn${btnID}`).addClass('btn-generic-pressed')
            else
                $(`#${powerState}Btn${btnID}`).removeClass("btn-generic-pressed")
            blockForIterations--;
        });
    }
    $(`#${powerState}Btn${btnID}`).removeClass("processing")
}

function delay(time) {
    return new Promise(resolve => setTimeout(resolve, time));
}