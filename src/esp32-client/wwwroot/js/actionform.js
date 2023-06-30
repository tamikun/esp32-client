
function setPath(ipAddress) {

    var default_path = document.querySelector(`.tab-content [id="${ipAddress}"] [id="newFile"]`).files[0].name;

    document.querySelector(`.tab-content [id="${ipAddress}"] [id="filePath"]`).value = default_path;
}

function postDataJson() {
    var data = {
        key1: 'value1',
        key2: 'value2'
    };

    $.ajax({
        type: 'POST',
        url: '/api/TestApi/PostJson21',  // Replace with the actual API endpoint
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function (response) {
            console.log('POST request successful');
            // Handle the response here
            console.log(response);
        },
        error: function (xhr, status, error) {
            console.log('POST request failed');
            console.log(xhr);
            console.log(status);
            console.log(error);
            // Handle any errors here
        }
    });
}

function postFile(ipAddress) {
    var fileInput = document.getElementById('newFile');
    var filePath = document.getElementById('filePath');

    var formData = new FormData();
    formData.append('newFile', fileInput.files[0]);
    formData.append('filePath', filePath.value);
    formData.append('ipAddress', ipAddress);

    $.ajax({
        type: 'POST',
        url: '/api/OpenApi/PostAsyncFile', //client side cannot request http (just https)
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            console.log('POST request successful');
            console.log(response);

            if (response.statusCode === 200) {
                alert('Request successful!');
                // Reload the page
                location.reload();
            } else if (response.statusCode === 404) {
                alert('Not Found ' + response.statusCode);
            } else {
                alert('Unexpected status code: ' + response.statusCode);
            }

        },
        error: function (xhr, status, error) {
            console.log('POST request failed!');
            // Handle any errors here
            console.log(xhr);
            console.log(status);
            console.log(error.toString());
            alert('Request failed ' + xhr);
        }
    });
}