function removeUnderline() {
    var elem = document.querySelectorAll('[role="tab"]')
    elem.forEach(function (item) {
        item.setAttribute("style", "text-decoration:none")
    });
}