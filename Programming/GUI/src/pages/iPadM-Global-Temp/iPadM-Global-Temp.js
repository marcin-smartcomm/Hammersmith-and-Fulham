function InitializeGlobalTempSp()
{
  GetGlobalTempCall()

    $("#temperatureRaiseBtn").on("touchstart", function () {
      $(this).addClass("btn-generic-pressed")
    });
    $("#temperatureRaiseBtn").on("touchend", function () {
      $(this).removeClass("btn-generic-pressed")
      PlayBtnClickSound()
      ChangeGlobalTempCall("Up")
    });

    $("#temperatureLowerBtn").on("touchstart", function () {
      $(this).addClass("btn-generic-pressed")
    });
    $("#temperatureLowerBtn").on("touchend", function () {
      $(this).removeClass("btn-generic-pressed")
      PlayBtnClickSound()
      ChangeGlobalTempCall("Down")
    });
}

function UpdateMasterTemp(newTemp)
{
  if(currentSubpage == "iPadM-Global-Temp")
    $("#masterTargetTempValue").text(newTemp);
}