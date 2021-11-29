

var strInvince = "Invincible";
var strNormal = "Normal";
var iRealTimeRefresh = 7000;
var iRealTimeErrorCount = 0;
var arPageHistory = [];
var iPagePointer = -1;
var iPageViewing = -1;
var iMaxHistory = 10;

var bPopupVisible = false;
var bLookupVisible = false;
var bUseRT = #USERT#;

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

    if (bUseRT) setTimeout(UpdateRealTimeStats, iRealTimeRefresh);

}

function SubPageLoaded(selectedMenu, sSelectedSubMenu) {

    SetSubTab(selectedMenu, sSelectedSubMenu);
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

    if (bUseRT) setTimeout(UpdateRealTimeStats, iRealTimeRefresh);
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

function AddGold(sElementName) {
    var sQuantity = document.getElementById(sElementName).value;

    RunDebugCommand("money " + sQuantity, "popup");
}

function ShowLookup() {
    var popup = document.getElementById("myLookup");
    var pText = document.getElementById("lookuptext");

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
        document.getElementById(selectedMenu).classList.add("active");
    }
}

function SetSubTab(selectedMenu, selectedSubTab) {

    SetSelectedMenu(selectedMenu);

    if (selectedMenu != "") {
        document.getElementById('grp_' + selectedMenu).style.display = "inline-flex";
    }

    if (selectedSubTab != "") {
        document.getElementById('grp_' + selectedSubTab).style.color = "#03c2fc";
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

function RunDebugCommand(sCommand, sElement, callback) {
    var sURL = '/dcom?comm=' + encodeURIComponent(sCommand);

    console.debug('URL: ' + sURL);

    fetch(sURL).then(function (response) {
        return response.text();
    }).then(function (string) {
        if (sElement !== undefined && sElement !== null) {
            if (sElement == "popup") {
                ShowPopUp(string, 6000);
            }
            else if (sElement == "recres") {
                //
                //  deal with recording result
                //
                AddRecordingResult(string);
            } else if (sElement == "moveres") {
                UpdateRecording(string);
            }
            else if (sElement == "loadres") {
                PopulateSave(string);
            }
            else {
                document.getElementById(sElement).innerHTML = string;
            }
        }
        if (callback !== undefined && callback !== null) callback();
    }).catch(function (err) {
        if (sElement !== undefined) {
            document.getElementById(sElement).innerHTML = 'Error: ' + err;
        }
    });
}
function UpdateRecording(sResult) {
    Index = document.getElementById("recordings").selectedIndex;
    if (Index >= 0) {
        var opt = document.createElement("option");
        opt.text = sResult;
        opt.value = sResult;
        document.getElementById("recordings").options[Index] = opt;
    }

}
function AddRecordingResult(sResult) {
    if (sResult != "") {
        var opt = document.createElement("option");
        opt.text = sResult;
        opt.value = sResult;
        document.getElementById("recordings").options.add(opt);
    }
}
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

    var sQuery = document.getElementById("lookupquery").value
    ExecuteQueryForItem(sQuery);
}

function ExecuteQueryForItem(sQuery) {

    document.getElementById("waitgif").style.display = "block";
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
function PopAndSearch(sId) {
    ShowLookup();
    ExecuteQueryForItem(sId);
}
function PopAndSearch(sId, sCat) {
    ShowLookup();
    LookupId(sId, sCat);
}

function PopAndMetaSearch(sMetaId) {
    ShowLookup();
    LookupMetaId(sMetaId);
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

function LookupMetaId(sMetaId) {

    var sURL = "/Search?metaid=" + sMetaId;
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
    document.getElementById("npcevents").checked = bState;
    document.getElementById("packad").checked = bState;
    document.getElementById("farmcheat").checked = bState;
    document.getElementById("invc").checked = bState;
    document.getElementById("eventtoggle").checked = bState;
    document.getElementById("farmercheat").checked = bState;
    document.getElementById("showcrops").checked = bState;
    document.getElementById("towncheat").checked = bState;
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
        document.getElementById("timebutton").setAttribute("class", "hud_label_inline hud_warp_button flasher");
    } else {
        document.getElementById("timebutton").innerHTML = strStopTime;
        document.getElementById("timebutton").setAttribute("class", "hud_label_inline hud_warp_button");
    }
}

function AddToInventory(iObjectId) {

    AddToInventoryFromId(iObjectId, "ADDTOPACK");
}

function AddToInventoryFromId(iObjectId, sQtyId) {

    var iQty = document.getElementById(sQtyId).value;
    RunDebugCommand("addtopack " + iObjectId + " " + iQty, "popup");
}

function AddTypeToInventory(sObjectType, iObjectId) {

    var iQty = document.getElementById("ADDTOPACK").value;
    RunDebugCommand("addtopack " + iObjectId + " " + iQty + " -1 " + sObjectType, "popup");

}
function AddToInventoryFromMetaId(sMetaId, sQtyId) {

    var iQty = document.getElementById(sQtyId).value;
    RunDebugCommand("addtopack m:" + sMetaId + " " + iQty, "popup");
}

function ToggleInv() {
    RunDebugCommand("inv", "popup");
    if (document.getElementById("invbutton").innerHTML == strInvince) {
        document.getElementById("invbutton").innerHTML = strNormal;
    } else {
        document.getElementById("invbutton").innerHTML = strInvince;
    }
}

function SetBackpack() {
    var iSize = document.getElementById("BPACK").value;
    RunDebugCommand("backpack " + iSize, "popup");

}

function SetSubMenu(sSubMenuId) {

}

function allowDrop(ev) {
    ev.preventDefault();
}

function drag(ev) {
    console.debug("Drag started");
    ev.dataTransfer.setData("text", ev.target.parentNode.id + "^^^" + ev.target.id);
    console.debug("Drag ID:" + ev.target.parentNode.id);
}

function shippingbindrop(ev) {
    ev.preventDefault();

    var data = ev.dataTransfer.getData("text").split("^^^");
    console.debug("Source ID:" + data[1]);

    RunDebugCommand("moveitem " + data[0].replaceAll(" ", "+") + " sbin.100", null, MoveComplete);

    document.getElementById(data[0]).innerText = "Empty";
}

function trashdrop(ev) {
    ev.preventDefault();

    var data = ev.dataTransfer.getData("text").split("^^^");
    console.debug("Source ID:" + data[1]);

    RunDebugCommand("removeItem " + data[0].replaceAll(" ", "+"), null, MoveComplete);

    document.getElementById(data[0]).innerText = "Empty";
}

function drop(ev) {
    ev.preventDefault();

    var data = ev.dataTransfer.getData("text").split("^^^");
    console.debug("Source ID:" + data[1]);

    var sTargetId = ev.originalTarget.id;
    RunDebugCommand("moveitem " + data[0].replaceAll(" ", "+") + " " + sTargetId.replaceAll(" ", "+"), null, MoveComplete);

    var oMoveElment = document.getElementById(data[1]);

    ev.target.innerText = "";
    ev.target.appendChild(oMoveElment);
    document.getElementById(data[0]).innerText = "Empty";

}

function MoveComplete() {
    TopNameSelected();
    BNameSelected();
}

function removeOptions(selectElement) {
    var i, L = selectElement.options.length - 1;
    for (i = L; i >= 0; i--) {
        selectElement.remove(i);
    }
}

function ToggleSeen() {

    RunDebugCommand("toggleevent " + iEventId, null, function () { location.reload(); });
}

function RunEvent() {
    RunDebugCommand("ebi " + iEventId, null);
}

function RefreshTopChest() {

    if (document.getElementById("topName").length == 0) {
        TopCategorySelected();
    }
    else {
        TopNameSelected();
    }
}

function RefreshBottomChest() {

    if (document.getElementById("bName").length == 0) {
        BNameSelected();
    }
    else {
        BCategorySelected();
    }
}

function ToggleKey(oCheckbox) {
    //
    //  toggle the value of a wallet item
    //
    RunDebugCommand("togglekey " + oCheckbox.id, "popup");
}

function SetLevel(sQtyId, sLevelType) {
    //
    //  set Farmer skill level
    //
    var iQty = document.getElementById(sQtyId).value;
    RunDebugCommand("setskill " + sLevelType + " " + iQty, "popup");
}

function IncreaseSkillLevel(sQtyId, sLevelType) {
    RunDebugCommand("incskill " + sLevelType, "popup");
    if (parseInt(document.getElementById(sQtyId).value) < 10) {
        document.getElementById(sQtyId).value = parseInt(document.getElementById(sQtyId).value) + 1;
    }
}
function DecreaseSkillLevel(sQtyId, sLevelType) {
    RunDebugCommand("decskill " + sLevelType, "popup");
    if (parseInt(document.getElementById(sQtyId).value) > 0) {
        document.getElementById(sQtyId).value = parseInt(document.getElementById(sQtyId).value) - 1;
    }
}

function ClearFarm() {
    if (confirm('Doing this will clear all crops, debries and tress from your Farm')) {
        RunDebugCommand('clearfarm', 'popup');
    }
}

function ClearBuildings() {
    if (confirm('Doing this will remove all Buildings and the animals in them.  If theere are Animals outside of the Buildings they will become homeless.')) {
        RunDebugCommand('removebuildings', 'popup');
    }

}

function Build() {
    var sBuilding = document.getElementById("BUILDING").value;
    RunDebugCommand("build " + sBuilding, "popup");
}
function SetupFarm() {
    var sFarmType = document.getElementById("FARMTYPE").value;
    switch (sFarmType) {
        case "Small":
            RunDebugCommand("setupfarm", "popup");
            break;
        case "Fish":
            RunDebugCommand("setupfishpondfarm", "popup");
            break;
        case "BigFarm":
            RunDebugCommand("setupbigfarm", "popup");
            break;
    }
}

function AddAnimal() {
    var sAnimal = document.getElementById("ANIMALTYPE").value;
    RunDebugCommand("animal " + sAnimal, "popup");

}

function ClickAndOpen(sURL) {
    window.location = sURL;
}

function DownloadDailyScreenShot() {
    if (confirm("This will create an animated Gif of your screenshots.  This may take a few minutes, depending upon the number of screenshots.  You will be prompted to save the file when it is complete.")) {
        var iWidth = document.getElementById("gifWidth").value;
        var iHeight = document.getElementById("gifHeight").value;
        var iDelay = document.getElementById("gifDelay").value;
        var bDMark = document.getElementById("dmark").checked;

        ClickAndOpen('/Utils/dailygif?width=' + iWidth + '&height=' + iHeight + '&delay=' + iDelay + '&dmark=' + bDMark);
    }
}

function SetSeason() {
    var sSeason = document.getElementById("SEASON").value;
    RunDebugCommand("season " + sSeason, "popup");
}

function SetDate() {
    var sDate = document.getElementById("DATE").value;
    RunDebugCommand("day " + sDate, "popup");
}

function SetTime() {
    var sTime = document.getElementById("TIME").value;
    RunDebugCommand("time " + sTime, "popup");
}

function SetYear() {
    var sYear = document.getElementById("YEAR").value;
    RunDebugCommand("year " + sYear, "popup");
}


function AddGold() {
    var sGold = document.getElementById("GOLD").value;
    RunDebugCommand("money " + sGold, "popup");

}

function MarkAsRead(sLetterId) {
    RunDebugCommand("readletter " + sLetterId, null, RefreshPage);
    //location.reload(); 
}

function MarkAsNew(sLetterId) {
    RunDebugCommand("unreadletter " + sLetterId, null, RefreshPage);
    // location.reload(); 
}

function RemoveLetter(sLetterId) {
    RunDebugCommand("deleteletter " + sLetterId, null, RefreshPage);
}

function RefreshPage() {

    location.reload();
}

function UpdateRealTimeStats() {
    var sUrl = "/Json/realtime";
    fetch(sUrl)
        .then(response => {
            if (response.ok) {
                response.json().then(data => {
                    document.getElementById('hud_balance').innerHTML = data.Balance;
                    document.getElementById('hud_year').innerHTML = data.Year;
                    document.getElementById('hud_season').innerHTML = data.Season;
                    document.getElementById('hud_daytext').innerHTML = data.DayDate;
                    document.getElementById('hud_tod').innerHTML = data.TOD;
                    document.getElementById('hud_warn').innerHTML = data.Warning;
                    iRealTimeErrorCount = 0;
                });
            } else {
                RealTimeError();
            }
        })
        .catch(error =>
            RealTimeError()
        );
    if (iRealTimeErrorCount < 10) {
        setTimeout(UpdateRealTimeStats, iRealTimeRefresh * (iRealTimeErrorCount + 1));
    }
}

function RealTimeError() {
    iRealTimeErrorCount++;
    document.getElementById('hud_balance').innerHTML = "????";
    document.getElementById('hud_year').innerHTML = "????";
    document.getElementById('hud_season').innerHTML = "????";
    document.getElementById('hud_daytext').innerHTML = "????";
    document.getElementById('hud_tod').innerHTML = "????";

}

function SaveCustomLocParameters() {

    //
    //  add forage data
    //
    var sURL = document.getElementById("SPFORAGERAW").value + "/";
    sURL += document.getElementById("SUFORAGERAW").value + "/";
    sURL += document.getElementById("FAFORAGERAW").value + "/";
    sURL += document.getElementById("WIFORAGERAW").value + "/";
    //
    //  add fish data
    //
    sURL += document.getElementById("SPFISHINGRAW").value + "/";
    sURL += document.getElementById("SUFISHINGRAW").value + "/";
    sURL += document.getElementById("FAFISHINGRAW").value + "/";
    sURL += document.getElementById("WIFISHINGRAW").value + "/";
    //
    //  add artifact data
    //
    sURL += document.getElementById("ARTIFACTRAW").value;


    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            document.getElementsByTagName('body')[0].innerHTML = xmlHttp.responseText;
            SubPageLoaded('Mods', 'Realty');
        }
    }

    var expname = document.getElementById('expname').innerText;
    xmlHttp.open("GET", "/ModInfo/save/" + expname + "!" + sURL, true);
    xmlHttp.send(null);
}