function InitalizeTemperatureControlSp()
{
    InitializeTemperatureControlBtns()
    PopulateTemperatureControlSp(
        RoomProcessorAjaxGETCall("RoomTemperatures", [currentRoomInfo.roomID])
    )
}

function InitializeTemperatureControlBtns()
{
    let raiseBtn = document.getElementById("temperatureRaiseBtn")
    let lowerBtn = document.getElementById("temperatureLowerBtn")

    raiseBtn.addEventListener('touchstart', function(){
        raiseBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    raiseBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        raiseBtn.classList.remove('btn-generic-pressed')
        UpdateCurrentSetpoint(
            RoomProcessorAjaxGETCall("ChangeTemp", [currentRoomInfo.roomID, "Up"])
        )
    })

    lowerBtn.addEventListener('touchstart', function(){
        lowerBtn.classList.add('btn-generic-pressed')
    }, { passive: "true" })
    lowerBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        lowerBtn.classList.remove('btn-generic-pressed')
        UpdateCurrentSetpoint(
            RoomProcessorAjaxGETCall("ChangeTemp", [currentRoomInfo.roomID, "Down"])
        )
    })
}

function PopulateTemperatureControlSp(climateData)
{
    if(currentSubpage == "Temperature-Control")
    {
        if(!climateData.setpoint.toString().includes("."))
            climateData.setpoint = `${climateData.setpoint}.0`
        if(!climateData.currentTemp.toString().includes("."))
            climateData.currentTemp = `${climateData.currentTemp}.0`
        $("#tempepraturePageC02Value").text(climateData.spaceCO2)
        $("#temperaturePageSetpointValue").text(`${climateData.setpoint}°`)
        $("#temperaturePageCurrentTempValue").text(`${climateData.currentTemp}°`)
    }
}

function UpdateCurrentSetpoint(climateData)
{
    if(!climateData.setpoint.toString().includes("."))
        climateData.setpoint = `${climateData.setpoint}.0`
    if(currentSubpage == "Temperature-Control")
        if(climateData.setpoint != -100.0)
            $("#temperaturePageSetpointValue").text(`${climateData.setpoint}°`)
}