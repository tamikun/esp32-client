$(document).ready(function () {
    // Execute code after the page has finished loading
    // $(".alert").each(function () {
    //     var alertElement = $(this);
    //     setTimeout(function () {
    //         alertElement.fadeOut("slow");
    //     }, 15000);
    // });

    $(".close-alert").each(function () {
        var alertElement = $(this).closest(".alert");
        alertElement.on("click", function () {
            alertElement.fadeOut("slow");
        });
    });
    
});


function handleEvent(type, message) {
    const content = document.querySelector(`.tab-content`);
    let html = "";
    switch (type) {
        case 'success':
            html = "<div class=\"alert alert-success alert-dismissible\"><strong>Success: </strong>" + message + "</div>";
            content.insertAdjacentHTML("beforebegin", html);

            setTimeout(function () {
                location.reload();
            }, 1000);

            break;
        case 'error':
            html = "<div class=\"alert alert-danger alert-dismissible\"><strong>Error: </strong>" + message + "</div>";
            content.insertAdjacentHTML("beforebegin", html);
            $(".alert").each(function () {
                var alertElement = $(this);
                setTimeout(function () {
                    alertElement.fadeOut("slow");
                }, 2000);
            });
            break;
        default:
    }

}


// Search for list Server
const tabLinks = document.querySelectorAll('.tab-menu .list-server a');
const searchInput = document.getElementById('searchInput');

searchInput.addEventListener('input', function () {
    const searchQuery = this.value.toLowerCase();

    tabLinks.forEach(link => {
        const tabId = link.innerHTML;

        if (tabId.toLowerCase().includes(searchQuery)) {
            link.style.display = 'block';
        } else {
            link.style.display = 'none';
        }
    });
});
