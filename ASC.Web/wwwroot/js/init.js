$(document).ready(function () {
    $('.parallax').parallax();
    $('.sidenav').sidenav();
    $('.collapsible').collapsible();

    // Block right-click context menu
    $(document).on("contextmenu", function (e) {
        e.preventDefault();
    });

    // Block back and forward browser buttons
    history.pushState(null, null, location.href);
    window.onpopstate = function () {
        history.go(1);
    };
});
