// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.getElementById('CustomTimeVisible').addEventListener('change', function() {
    var CustomTimeInfo = document.getElementById('CustomTimeInfo');
    CustomTimeInfo.style.display = this.checked ? 'block' : 'none';
});

// Optional: Set initial state
window.addEventListener('load', function() {
    var checkbox = document.getElementById('CustomTimeVisible');
    var CustomTimeInfo = document.getElementById('CustomTimeInfo');
    CustomTimeInfo.style.display = checkbox.checked ? 'block' : 'none';
});

function goBack() {
    window.history.back();
}