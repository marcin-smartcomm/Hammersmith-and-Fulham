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