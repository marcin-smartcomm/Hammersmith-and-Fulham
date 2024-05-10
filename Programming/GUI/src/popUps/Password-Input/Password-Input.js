function InitalizePasswordInputPopUp()
{
    let inputField = document.getElementById("passInputField")
    let closeBtn = document.getElementById("passwordInputCloseBtn")

    closeBtn.addEventListener('touchstart', function(){
        closeBtn.classList.add('btn-generic-pressed')
    }, {passive: "true"})
    closeBtn.addEventListener('touchend', function(){
        PlayBtnClickSound()
        closeBtn.classList.remove('btn-generic-pressed')
        clearSpecificPopUp("passwordPopUpSection")
    })

    inputField.focus()
    inputField.click()

    inputField.addEventListener("keypress", function(evt)
    {
        //if enter pressed
        if(evt.code == "Enter")
        {
            let pass = inputField.value;
            console.log("pass: " + pass)
            ProcessPasswordCheckResult(
                CoreProcessorAjaxGETCall("SlaveiPadPassCheck", [pass])
            )
        }
    })
}

function ProcessPasswordCheckResult(result)
{
    if(result.password == "incorrect")
    {
        let topText = document.getElementById("passInputText")
        topText.style.color = "red"
        setTimeout(() => {
            topText.style.color = "var(--generic-text-color-inactive)"
        }, 1000);
    }
    if(result.password == "correct")
    {
        let topText = document.getElementById("passInputText")
        topText.style.color = "green"
        setTimeout(() => {
            clearSpecificPopUp("passwordPopUpSection")
            LoadSideMenu("FloorList")
            ActivateSideMenuBtns()
        }, 500);
    }
}