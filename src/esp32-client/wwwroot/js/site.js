$(document).ready(function () {
    // Execute code after the page has finished loading
    $(".alert").each(function () {
        var alertElement = $(this);
        setTimeout(function () {
            alertElement.fadeOut("slow");
        }, 2000);
    });

});
