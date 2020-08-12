window.copyFormatted = function () {

    var formattedCode = document.getElementById("formattedCode");

    var range = document.createRange();
    range.selectNode(formattedCode);
    window.getSelection().addRange(range);
    var success = document.execCommand("copy");
    window.getSelection().empty();

    return success;

}

window.prettify = function () {

    var formattedCode = document.getElementById("formattedCode");
    var value = formattedCode.textContent;
    if (value) {

        pretty = prettyPrintOne(value)
        this.formattedCode.innerHTML = pretty;
        return true;
    }

    return false;
}