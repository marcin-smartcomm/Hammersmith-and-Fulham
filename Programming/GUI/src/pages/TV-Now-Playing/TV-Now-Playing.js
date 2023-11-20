function InitializeTVNowPlayingSp()
{
    InitializeTVNowPlayingBtns()
}

function InitializeTVNowPlayingBtns()
{
    let backBtn = document.querySelector(".tv-page-back-btn")

    backBtn.addEventListener('touchstart', function(){
        backBtn.classList.add("btn-generic-pressed")
    }, { passive: "true" })
    backBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        backBtn.classList.remove("btn-generic-pressed")
        openSubpage("TV", "TV", "fa-solid fa-tv")
    })
}