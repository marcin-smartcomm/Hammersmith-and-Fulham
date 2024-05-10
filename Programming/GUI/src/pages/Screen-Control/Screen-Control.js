function InitializeScreenControlSp()
{
  ActivateSmallHallSection()
  GetSmallHallDisplayControlOpitonCall()
}

function ActivateSmallHallSection()
{
  $('#smallHallTV').on('click', () => {
    SetSmallHallDisplayControlOpitonCall('1')
    PlayBtnClickSound()
  })
  $('#smallHallProjector').on('click', () => {
    SetSmallHallDisplayControlOpitonCall('2')
    PlayBtnClickSound()
  })
  $('#smallHallBoth').on('click', () => {
    SetSmallHallDisplayControlOpitonCall('0')
    PlayBtnClickSound()
  })
}

function PopulateSmallHallSection(result)
{
  ClearBtnFb()
  $(`#smallHall${result.DisplayControlOption}`).addClass('btn-generic-pressed')
}

function ClearBtnFb()
{
  $('#smallHallTV').removeClass('btn-generic-pressed')
  $('#smallHallProjector').removeClass('btn-generic-pressed')
  $('#smallHallBoth').removeClass('btn-generic-pressed')
}

function GetSmallHallDisplayControlOpitonCall()
{
    $.ajax({
        type: "GET",
        url: "http://192.168.1.241:50000/api/GetDisplayControlOption",
        dataType: "json",
        data: '1',
        success: function (result) {
            //Screen-Control.js
            PopulateSmallHallSection(result)
        },
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}

function SetSmallHallDisplayControlOpitonCall(option)
{
    $.ajax({
        type: "GET",
        url: "http://192.168.1.241:50000/api/SetDisplayControlOption",
        dataType: "json",
        data: `1` + `:` + option,
        success: function (result) {
            //Screen-Control.js
            PopulateSmallHallSection(result)
        },
        error: function () {
            console.error("Error in communication")
        },
        timeout: 2000
    });
}