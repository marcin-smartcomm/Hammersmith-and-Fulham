function InitializeVideoProductionSp()
{
  GetProductionUnitSourcesCall(currentRoomInfo.roomID)
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
      GetCurrentlySelectedStreamCall(currentRoomInfo.roomID)
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
      GetCurrentlySelectedStreamCall(currentRoomInfo.roomID)
     })

     $(this).on('click', () => {
      PlayBtnClickSound()
      UpdateProductionUnitStreamCall(currentRoomInfo.roomID, $(this).text())
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