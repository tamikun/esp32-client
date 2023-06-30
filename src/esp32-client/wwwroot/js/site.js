$(document).ready(function () {
    $(".alert").each(function () {
        var alertElement = $(this);
        setTimeout(function () {
            alertElement.fadeOut("slow");
        }, 3000);
    });
});
