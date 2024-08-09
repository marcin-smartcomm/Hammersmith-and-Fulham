function LightingProcessorAjaxGETCall(endpoint, params)
{
    let dataToSend = ''

    if(params.length === 1) dataToSend = params[0];
    else if (params.length > 1)
    {
        $.each(params, function (i, value) { 
            if(i === params.length-1) dataToSend += value
            else dataToSend += value+':'
        });
    }

    var responseJSON = "";
    $.get(`http://${lightingServerIP}:50000/api/${endpoint}?${dataToSend}`)
    .done(function(response) {responseJSON = response})
    .fail(function(xhr, status, error) 
    {
        alert(`Unable to communicate with Lighting Processor (${lightingServerIP})`)
        responseJSON = "Error"
    })
    
    console.log(`Lighting Request: ${endpoint}`);
    console.log(responseJSON);

    return responseJSON;
}