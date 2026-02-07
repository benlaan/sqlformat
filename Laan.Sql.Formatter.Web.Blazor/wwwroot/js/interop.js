window.copyFormatted = function () {

    var formattedCode = document.getElementById("formattedCode");

    var range = document.createRange();
    range.selectNode(formattedCode);
    window.getSelection().addRange(range);
    var success = document.execCommand("copy");
    window.getSelection().empty();

    return success;

}

window.getPrettifiedHtml = function (sqlCode) {
    if (sqlCode) {
        return prettyPrintOne(sqlCode, 'lang-sql', true);
    }
    return "";
}

window.scrollToOutput = function () {
    var outputContainer = document.getElementById("outputContainer");
    if (outputContainer) {
        outputContainer.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
}