function InitializeVideoProductionSp()
{
  PopulateRecordableSourcesList(
    RoomProcessorAjaxGETCall("GetProductionUnitSources", [currentRoomInfo.roomID])
  )
}

async function PopulateRecordableSourcesList(sourceList)
{
  $.each(sourceList.sources, function (i, source) {
    if(source.canBeRecorded)
    {
      $.get('pages/Video-Production/Source-Btn.html', async (newBtn) => {
        $('#productionSourceList').append(newBtn)
        $('#productionUnitSrcBtn').html(source.sourceName)
        $('#productionUnitSrcBtn').attr('id', `srcBtn${i}`)
      })
    }

    if(i == sourceList.sources.length-1)
    setTimeout(() => {
      ActivateProductionSrcButtons()
      var result = RoomProcessorAjaxGETCall("GetCurrentProductionStream", [currentRoomInfo.roomID])
      UpdateSelectedStreamFb(result.ProductionUnitSource)
    }, 200);
  });
}

function ActivateProductionSrcButtons()
{
  $.each($('.video-production-src-btn'), function (i, srcBtn) { 

     $(this).on('touchstart', () => {
      $(this).addClass('btn-generic-pressed')
     })

     $(this).on('touchend', () => {
      $(this).removeClass('btn-generic-pressed')
      var result = RoomProcessorAjaxGETCall("GetCurrentProductionStream", [currentRoomInfo.roomID])
      UpdateSelectedStreamFb(result.ProductionUnitSource)
     })

     $(this).on('click', () => {
      PlayBtnClickSound()
      var result = RoomProcessorAjaxGETCall("UpdateProductionUnitStream", [currentRoomInfo.roomID, $(this).text()])
      UpdateSelectedStreamFb(result.ProductionUnitSource)
     })

  });
}

function UpdateSelectedStreamFb(selectedStream)
{
  $.each($('.video-production-src-btn'), function (i, srcBtn) { 

    if($(this).text() == selectedStream) $(this).addClass('btn-generic-pressed')
    else $(this).removeClass('btn-generic-pressed')

  });
}