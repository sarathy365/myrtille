var disableUserClose = true;
var isSessionConnected = false;

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
        doDisconnect();
    }
}

function closeTab() {
    disableUserClose = false;
    window.close();
    hideLoadingDiv();
    hideRemoteOperationsDiv();
    setTimeout(function () { showErrorMessage("Unexpected Error.", true); }, 1000);
}

function dragElement(elmnt) {
    var clientWidth = window.innerWidth;
    var clientHeight = window.innerHeight;

    var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
    if (document.getElementById(elmnt.id + "header")) {
        /* if present, the header is where you move the DIV from:*/
        document.getElementById(elmnt.id + "header").onmousedown = dragMouseDown;
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
        var topPixel = (elmnt.offsetTop - pos2)
        if (topPixel > (clientHeight - 145)) {
            topPixel = clientHeight - 145;
        } else if (topPixel < 0) {
            topPixel = 0;
        }
        elmnt.style.top = topPixel + "px";
        var leftPixel = (elmnt.offsetLeft - pos1)
        if (leftPixel > (clientWidth-127)) {
            leftPixel = clientWidth - 127;
        } else if (leftPixel < 0) {
            leftPixel = 0;
        }
        elmnt.style.left = leftPixel + "px";
    }

    function closeDragElement() {
        /* stop moving when mouse button is released:*/
        document.onmouseup = null;
        document.onmousemove = null;
    }
}
