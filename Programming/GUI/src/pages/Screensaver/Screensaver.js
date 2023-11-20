function InitializeScreenSaverVariables()
{
    let fireAlarmPoll = setInterval(() => {
        GetFireAlarmStateCall()
    }, 5000);

    document.getElementById('screenSaver').addEventListener('touchend', function() {
        PlayBtnClickSound()
        //SubpageManager.js
        clearTimeout(fireAlarmPoll)
        DisplayLoadingScreen()
        ConnectToSystem();
        document.getElementById("hoursAndMinutes").style.display = "block"
        document.getElementById("dayAndMonth").style.display = "block"
    })

    document.getElementById("hoursAndMinutes").style.display = "none"
    document.getElementById("dayAndMonth").style.display = "none"
    
    if(document.getElementById("popUpSection") !== null)
        document.getElementById("popUpSection").remove()
}

function DisplayLoadingScreen()
{
    let loadingElement = document.createElement("div")
    let loadingElementText = document.createElement("div")
    
    loadingElementText.innerHTML = "Loading"
    loadingElementText.style.width = "100%"

    loadingElement.appendChild(loadingElementText)
    loadingElement.style.fontSize = "250%"
    loadingElement.style.textAlign = "center"
    loadingElement.style.color = "var(--generic-text-color-inactive)"
    loadingElement.style.width = "100%"
    loadingElement.style.height = "30%"
    loadingElement.style.display = "flex"
    loadingElement.style.alignItems ="center"
    loadingElement.style.justifyContent = "center"
    loadingElement.style.flexWrap = "wrap"

    document.getElementById("projectBody").innerHTML = "";
    document.getElementById("projectBody").appendChild(loadingElement)

    let loadingBar = document.createElement("div")
    loadingElement.appendChild(loadingBar)
    loadingBar.style.width = "1%"
    loadingBar.style.height = "20%"
    loadingBar.style.backgroundColor = "var(--seperation-line-color)"

    let loadingBarTime = setInterval(() => {
        console.log()
        loadingBar.style.width = (parseInt(loadingBar.style.width)+1)+"%"
    }, 10);

    setTimeout(() => {
        clearInterval(loadingBarTime)
    }, 1000);
}