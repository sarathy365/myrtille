var disableUserClose = true;
var isSessionConnected = false;
var isCloseTabCalled = false;

this.isIEBrowserExceptEdge = function () {
    var ua = navigator.userAgent;

    // IE 10 or older
    var msie = ua.indexOf('MSIE ');
    if (msie > 0) {
        return true;
    }

    // IE 11
    var trident = ua.indexOf('Trident/');
    if (trident > 0) {
        return true;
    }

    return false;
};

window.onbeforeunload = function (evt) {
    if (disableUserClose) {
        if (!isIEBrowserExceptEdge() || isSessionConnected) {
            return "Do you want to close tab?";
        }
    }
}

window.onunload = function (evt) {
    if (isSessionConnected) {
        doDisconnect();
    }
}

function hideLoadingDiv() {
    if (document.getElementById('loadingDiv')) {
        document.getElementById('loadingDiv').style.display = 'none';
    }
}

function showRemoteOperationsDiv() {
    if (document.getElementById('remoteOperationsDiv')) {
        document.getElementById('remoteOperationsDiv').style.display = 'block';
    }
}

function hideRemoteOperationsDiv() {
    if (document.getElementById('remoteOperationsDiv')) {
        document.getElementById('remoteOperationsDiv').style.display = 'none';
    }
}

function showErrorMessage(message, hideCloseButton) {
    hideLoadingDiv();
    hideRemoteOperationsDiv();
    if (message) {
        document.getElementById("webrdp-error-text").innerText = message;
    }
    if (hideCloseButton) {
        document.getElementById("webrdp-back-home-btn").style.display = "none";
    }
    document.getElementById("errorMessageDialogEle").style.display = "block";
}

function onSessionConnection() {
    isSessionConnected = true;
    hideLoadingDiv();
    setTimeout(showRemoteOperationsDiv, 2000);
}

function onSessionDisconnection() {
    disableUserClose = false;
    if (!isSessionConnected && document.getElementById('loadingDiv')) {
        showErrorMessage();
    } else {
        closeTab();
    }
}

function disconnectSession() {
    if (confirm("Do you want to disconnect? The remote session will be simply disconnected and not logged out.")) {
        disableUserClose = false;
        doDisconnect();
    }
}

function closeTab() {
    if (isCloseTabCalled) {
        return;
    }
    isCloseTabCalled = true;
    disableUserClose = false;
    window.close();
    hideLoadingDiv();
    hideRemoteOperationsDiv();
    setTimeout(function () { showErrorMessage("Unexpected Error.", true); }, 1000);
}

function dragElement(elmnt) {
    var clientWidth = window.innerWidth;
    var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
    if (document.getElementById(elmnt.id + "Header")) {
        /* if present, the header is where you move the DIV from:*/
        document.getElementById(elmnt.id + "Header").onmousedown = dragMouseDown;
    } else {
        /* otherwise, move the DIV from anywhere inside the DIV:*/
        elmnt.onmousedown = dragMouseDown;
    }

    function dragMouseDown(e) {
        e = e || window.event;
        e.preventDefault();
        // get the mouse cursor position at startup:
        pos3 = e.clientX;
        pos4 = e.clientY;
        document.onmouseup = closeDragElement;
        // call a function whenever the cursor moves:
        document.onmousemove = elementDrag;
    }

    function elementDrag(e) {
        e = e || window.event;
        e.preventDefault();
        // calculate the new cursor position:
        pos1 = pos3 - e.clientX;
        pos2 = pos4 - e.clientY;
        pos3 = e.clientX;
        pos4 = e.clientY;
        // set the element's new position:
        var leftPos = elmnt.offsetLeft - pos1;
        if (leftPos > (clientWidth - elmnt.offsetWidth)) {
            leftPos = clientWidth - elmnt.offsetWidth;
        }
        if (leftPos < 0) {
            leftPos = 0;
        }
        elmnt.style.left = leftPos + "px";
    }

    function closeDragElement() {
        /* stop moving when mouse button is released:*/
        document.onmouseup = null;
        document.onmousemove = null;
    }
}
