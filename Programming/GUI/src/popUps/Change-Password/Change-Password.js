function InitializeChangePassPopUp()
{
  let oldPassInputField = document.getElementById("oldPass")
  let oldPassText = document.getElementById("oldPassText")
  let newPassInputField = document.getElementById("newPass")
  let newPassText = document.getElementById("newPassText")
  let newPassConfirmInputField = document.getElementById("newPassConfirm")
  let newPassConfirmText = document.getElementById("newPassConfirmText")

  let saveBtn = document.getElementById("saveBtn")
  let cancelBtn = document.getElementById("cancelBtn")

  oldPassInputField.focus()

  oldPassInputField.addEventListener("keypress", function(evt)
  {
      //if enter pressed
      if(evt.code == "Enter")
      {
        if(oldPassInputField.value != "") {
          oldPassText.style.color = "var(--generic-text-color-inactive)"
          newPassInputField.focus()
        }
        else oldPassText.style.color = "red"
      }
  })

  newPassInputField.addEventListener("keypress", function(evt)
  {
      //if enter pressed
      if(evt.code == "Enter")
      {
        if(newPassInputField.value != "") {
          newPassText.style.color = "var(--generic-text-color-inactive)"
          newPassConfirmInputField.focus()
        }
        else newPassText.style.color = "red"
      }
  })

  newPassConfirmInputField.addEventListener("keypress", function(evt)
  {
      //if enter pressed
      if(evt.code == "Enter")
      {
        if(newPassConfirmInputField.value != "") {
          newPassConfirmText.style.color = "var(--generic-text-color-inactive)"
          newPassConfirmInputField.blur()
          ProcessNewPassRequest()
        }
        else newPassConfirmText.style.color = "red"
      }
  })

  cancelBtn.addEventListener("touchstart", function() {
    cancelBtn.classList.add("btn-generic-pressed")
  }, {passive: "true"})
  cancelBtn.addEventListener("touchend", function() {
    PlayBtnClickSound()
    clearSpecificPopUp("changePassPopUpSection")
  })

  saveBtn.addEventListener("touchstart", function() {
    saveBtn.classList.add("btn-generic-pressed")
  }, {passive: "true"})
  saveBtn.addEventListener("touchend", function() {
    PlayBtnClickSound()
    saveBtn.classList.remove("btn-generic-pressed")
    ProcessNewPassRequest()
  })

  function ProcessNewPassRequest()
  {
    let formFilled = true;

    if(oldPassInputField.value == "") 
    {
      oldPassText.style.color = "red"
      formFilled = false;
    } else oldPassText.style.color = "var(--generic-text-color-inactive)"
    if(newPassInputField.value == "")
    {
      newPassText.style.color = "red"
      formFilled = false;
    } else newPassText.style.color = "var(--generic-text-color-inactive)"
    if(newPassConfirmInputField.value == "")
    {
      newPassConfirmText.style.color = "red"
      formFilled = false;
    } else newPassConfirmText.style.color = "var(--generic-text-color-inactive)"

    if(!formFilled) return

    SendNewPassRequestCall(oldPassInputField.value, newPassInputField.value, newPassConfirmInputField.value)
  }
}

function ProcessNewPassResponse(newPassResult)
{
  if(newPassResult.oldPassMatch && newPassResult.newPassMatch) 
  {
    ChangePassColors("green", "green", "green")
    setTimeout(() => {
      document.getElementById("changePassText").style.color = "green"
      document.getElementById("changePassText").innerHTML = "Password Changed Successfully"
      setTimeout(() => {
        clearSpecificPopUp("changePassPopUpSection")
      }, 1000);
    }, 1500);
  }
  else if (newPassResult.oldPassMatch && !newPassResult.newPassMatch)
    ChangePassColors("green", "red", "red")
  else if (!newPassResult.oldPassMatch && newPassResult.newPassMatch)
    ChangePassColors("red", "green", "green")
  else if (!newPassResult.oldPassMatch && !newPassResult.newPassMatch)
    ChangePassColors("red", "red", "red")
}

function ChangePassColors(oldPassColor, newPassColor, newPassConfirmColor)
{
  setTimeout(() => {
    document.getElementById("oldPassText").style.color = oldPassColor
    setTimeout(() => {
      document.getElementById("newPassText").style.color = newPassColor
      setTimeout(() => {
        document.getElementById("newPassConfirmText").style.color = newPassConfirmColor
      }, 500);
    }, 500);
  }, 500);
}