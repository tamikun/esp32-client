// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// JavaScript to handle tab switching and search functionality
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
