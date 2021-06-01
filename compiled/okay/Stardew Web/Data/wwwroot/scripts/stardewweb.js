function PageLoaded(selectedMenu) {

    SetSelectedMenu(selectedMenu);
    document.addEventListener('click', movePopup, false);
    //
    //  add lookup hook
    //
    var input = document.getElementById("lookupquery");

    input.addEventListener("keyup", function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            QueryForItem();
        }
    });
}

function movePopup(event) {
    x = event.pageX;
    y = event.pageY;

    document.getElementById("myPopup").style.left = (x + 14) + "px";
    document.getElementById("myPopup").style.top = (y > 100 ? y - 100 : y) + "px";
}

function WarpTo(LocationName, XPoint, YPoint) {
    console.debug("warping");
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            if (xmlHttp.responseText != "Warped") {
                ShowPopUp(xmlHttp.responseText);
            }
        }
    };
    xmlHttp.open("GET", "/warp?l=" + LocationName + "&X=" + XPoint + "&Y=" + YPoint, true);
    xmlHttp.send(null);

}

var bPopupVisible = false;
var bLookupVisible = false;

function ShowLookup() {
    var popup = document.getElementById("myLookup");
    var pText = document.getElementById("lookuptext");
    //pText.innerHTML = sLookupText;
    if (!bLookupVisible) {
        popup.classList.toggle("showlookup");
        bLookupVisible = true;
    }
}

function CloseLookup() {
    var popup = document.getElementById("myLookup");
    if (bLookupVisible) {
        popup.classList.toggle("showlookup");
        bLookupVisible = false;
    }
}

function ShowPopUp(textString, iTimeout) {
    var popup = document.getElementById("myPopup");
    var pText = document.getElementById("popuptext");
    pText.innerHTML = textString;
    if (!bPopupVisible) {
        popup.classList.toggle("showpop");
        bPopupVisible = true;
    }

    if (iTimeout === undefined) {
        setTimeout(ClosePopup, 15000);
    }
    else {
        setTimeout(ClosePopup, iTimeout);
    }
}

function ClosePopup() {
    var popup = document.getElementById("myPopup");
    if (bPopupVisible) {
        popup.classList.toggle("showpop");
        bPopupVisible = false;
    }
}
function SetSelectedMenu(selectedMenu) {

    if (selectedMenu != '' && document.getElementById(selectedMenu) != null) {
        document.getElementById(selectedMenu).style.color = "lime";
    }
}

function openTab(tabName) {
    var i;
    var x = document.getElementsByClassName("subtab");
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
    document.getElementById(tabName).style.display = "block";
}

function RunDebugCommand(sCommand, sElement) {
    var sURL = '/dcom?comm=' + sCommand;

    console.debug('URL: ' + sURL);

    fetch(sURL).then(function (response) {
        return response.text();
    }).then(function (string) {
        if (sElement !== undefined) {
            if (sElement == "popup") {
                ShowPopUp(string, 6000);
            }
            else {
                document.getElementById(sElement).innerHTML = string;
            }
        }
    }).catch(function (err) {
        if (sElement !== undefined) {
            document.getElementById(sElement).innerHTML = 'Error: ' + err;
        }
    });
}

var arPageHistory = [];
var iPagePointer = -1;
var iPageViewing = -1;
var iMaxHistory = 10;

function GetPreviousPage() {
    if (iPageViewing > 0) {
        iPageViewing--;
        document.getElementById("lookuptext").innerHTML = arPageHistory[iPageViewing];
    }
}
function GetNextPage() {

    if (iPageViewing < iPagePointer) {
        iPageViewing++;
        document.getElementById("lookuptext").innerHTML = arPageHistory[iPageViewing];
    }

}

function AddPageHistory(pagecontent) {

    if (pagecontent != arPageHistory[iPagePointer]) {
        iPagePointer++;

        if (iPagePointer > iMaxHistory) {
            for (var iPtr = 0; iPtr < iMaxHistory - 1; iPtr++) {
                arPageHistory[iPtr] = arPageHistory[iPtr + 1];
            }
            iPagePointer--;
        }
        iPageViewing = iPagePointer;
        arPageHistory[iPagePointer] = pagecontent;
    }
}
function QueryForItem() {

    document.getElementById("waitgif").style.display = "block";
    var sQuery = document.getElementById("lookupquery").value
    var sURL = "/Search?query=" + sQuery;

    fetch(sURL).then(function (response) {
        return response.text();
    }).then(function (string) {
        AddPageHistory(string);
        document.getElementById("lookuptext").innerHTML = string;
        document.getElementById("waitgif").style.display = "none";
    }).catch(function (err) {
        document.getElementById("waitgif").style.display = "none";
        document.getElementById("lookuptext").innerHTML = 'Error: ' + err;
    });

}

function PopAndSearch(sId, sCat) {
    ShowLookup();
    LookupId(sId, sCat);
}
function LookupId(sId, sCat) {

    var sURL = "/Search?id=" + sId + "&cat=" + sCat;
    document.getElementById("waitgif").style.display = "block";

    fetch(sURL).then(function (response) {
        return response.text();
    }).then(function (string) {
        AddPageHistory(string);
        document.getElementById("waitgif").style.display = "none";
        document.getElementById("lookuptext").innerHTML = string;
    }).catch(function (err) {
        document.getElementById("lookuptext").innerHTML = 'Error: ' + err;
        document.getElementById("waitgif").style.display = "none";
    });
}

function SetCheats(bState) {
    //
    //  disable all cheat options
    //

    document.getElementById("warping").checked = bState;
    document.getElementById("events").checked = bState;
    document.getElementById("tastes").checked = bState;
    document.getElementById("recipes").checked = bState;
    document.getElementById("npcmet").checked = bState;
    document.getElementById("arti").checked = bState;
    document.getElementById("fish").checked = bState;
    document.getElementById("stardrop").checked = bState;
    document.getElementById("potential").checked = bState;
    document.getElementById("unlocs").checked = bState;
    document.getElementById("schedule").checked = bState;
    document.getElementById("warptab").checked = bState;
    document.getElementById("modtab").checked = bState;
    document.getElementById("locationtab").checked = bState;
    document.getElementById("eventtab").checked = bState;
    document.getElementById("sleep").checked = bState;
    document.getElementById("search").checked = bState;
    document.getElementById("pause").checked = bState;

}

function ClearCache() {
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            if (xmlHttp.responseText != "Warped") {
                ShowPopUp(xmlHttp.responseText);
            }
        }
    };
    xmlHttp.open("GET", "/Config?cc=true", true);
    xmlHttp.send(null);


}

function ToggleTime() {

    RunDebugCommand("pausetime");
    console.debug("pt: " + document.getElementById("timebutton").innerHTML)
    if (document.getElementById("timebutton").innerHTML == strStopTime) {
        document.getElementById("timebutton").innerHTML = strStartTime;
    } else {
        document.getElementById("timebutton").innerHTML = strStopTime;
    }
}

function AddToInventory(iObjectId) {

    AddToInventoryFromId(iObjectId, "ADDTOPACK");
}

function AddToInventoryFromId(iObjectId, sQtyId) {

    var iQty = document.getElementById(sQtyId).value;
    RunDebugCommand("addtopack " + iObjectId + " " + iQty, "popup");
}