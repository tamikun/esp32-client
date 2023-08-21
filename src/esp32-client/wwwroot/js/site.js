$(document).ready(function () {

    $(".alert").each(function () {
        var alertElement = $(this);
        setTimeout(function () {
            alertElement.fadeOut("slow");
        }, 15000);
    });

    $(".close-alert").each(function () {
        var alertElement = $(this).closest(".alert");
        alertElement.on("click", function () {
            alertElement.fadeOut("slow");
        });
    });

    document.querySelectorAll(`input[type="submit"]`).forEach(
        item => {
            item.addEventListener('click', showSubmitLoadingPopup);
        }
    );
    document.querySelectorAll(`button[type="submit"]`).forEach(
        item => {
            item.addEventListener('click', showSubmitLoadingPopup);
        }
    );

    $("a:not(.download-file):not(.no-loading)").each(function () {
        $(this).on("click", showNormalLoadingPopup);
    });

});

function showSubmitLoadingPopup(event) {
    event.preventDefault();
    var loadingPopup = document.querySelector('.loading-popup');
    loadingPopup.style.display = 'flex';
    event.target.form.submit();
}

function showNormalLoadingPopup() {
    var loadingPopup = document.querySelector('.loading-popup');
    loadingPopup.style.display = 'flex';
}

function hideLoadingPopup() {
    var loadingPopup = document.querySelector('.loading-popup');
    loadingPopup.style.display = 'none';
}

function checkAll() {
    if (this.checked) {
        document.querySelectorAll(`input[type="checkbox"]`).forEach(item => {
            item.checked = false;
        });
        this.checked = false;
    }
    else {
        document.querySelectorAll(`input[type="checkbox"]`).forEach(item => {
            item.checked = true;
        });
        this.checked = true;
    }
}

// JavaScript functions to open and close the popup form
function openPopup(id) {
    document.getElementById(id).style.display = "block";
}

function closePopup(id) {
    document.getElementById(id).style.display = "none";
}

function submitById(id) {
    document.getElementById(id).submit();
}

function openInNewTab(url) {
    window.open(url, '_blank').focus();
}

function openTab(url) {
    window.open(url, '_self').focus();
}
