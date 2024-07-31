

var strInvince = "Invincible";
var strNormal = "Normal";
var iRealTimeRefresh = 7000;
var iGamePerfRefresh = 3000;
var iRealTimeErrorCount = 0;
var iGamePerfErrorCount = 0;
var arPageHistory = [];
var iPagePointer = -1;
var iPageViewing = -1;
var iMaxHistory = 10;

var bPopupVisible = false;
var bLookupVisible = false;
var bUseRT = #USERT#;
var divIdSeparator = "~!";
var divPartSepartor = "!~";

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

function WarpToMine(mineLevel) {
    RunDebugCommand('minelevel ' + mineLevel);

}

function AddGold(sElementName) {
    var sQuantity = document.getElementById(sElementName).value;

    RunDebugCommand("money " + sQuantity, "popup");
}

function ToggleRemote() {

    var remote = document.getElementById("remote");
    remote.classList.toggle("showlookup");
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
        //document.getElementById(selectedMenu).style.color = "lime";
        document.getElementById(selectedMenu).style.color = "black";
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


function WarpStudio(iMode, oParam) {

    switch (iMode) {
        case 0:
            var iNMode = GetRadioButtonValue("npcwarp");
            var e = document.getElementById("npcs");

            if (iNMode !== null) {
                switch (iNMode) {
                    case "wntm":  // warp npc to me
                        RunDebugCommand('wctm ' + e.value);
                        break;
                    case "wmtn":    // warp me to npc
                        RunDebugCommand('wtc ' + e.value);
                        break;
                    case "wnt":
                        var loc = document.getElementById("locs").value;
                        var locx = document.getElementById("xloc").value;
                        var locy = document.getElementById("yloc").value;
                        RunDebugCommand('wct ' + e.value + ' ' + loc + ' ' + locx + ' ' + locy + ' 1');
                        break;
                }
            }

            break;
        case 1:
            RunDebugCommand('ws ' + oParam);
            break;
        case 2:
            var ploc = document.getElementById("plocs").value;
            var plocx = document.getElementById("xploc").value;
            var plocy = document.getElementById("yploc").value;
            WarpTo(ploc, plocx, plocy);
            break;
        case 10:
            RunDebugCommand("custwarp " + oParam);
            break;
        case 3:
            var pMode = GetRadioButtonValue("playerwarp");
            switch (pMode) {
                case "mts":
                    //RunDebugCommand('mts');
                    break;
                case "stm":
                    RunDebugCommand('pstm');
                    break;
                case "mtnpc": //me to NPC;
                    var ePlayerNPC = document.getElementById("playernpcs");
                    RunDebugCommand('wtc ' + ePlayerNPC.value);
                    break;
                case "mtloc": //me to location
                    var ploc = document.getElementById("plocs").value;
                    var plocx = document.getElementById("xploc").value;
                    var plocy = document.getElementById("yploc").value;
                    WarpTo(ploc, plocx, plocy);
                    break;
                case "mwarp":
                    var minelevel = document.getElementById("mlevel").value;
                    RunDebugCommand('minelevel ' + minelevel);
                    break;
            }
            break;
        case 4:
            switch (oParam) {
                case "u":
                    RunDebugCommand('moveup');
                    break;
                case "r":
                    RunDebugCommand('moveright');
                    break;
                case "d":
                    RunDebugCommand('movedown');
                    break;
                case "l":
                    RunDebugCommand('moveleft');
                    break;

            }

    }


}

function GetRadioButtonValue(sGroupName) {
    var ele = document.getElementsByName(sGroupName);

    for (i = 0; i < ele.length; i++) {
        if (ele[i].checked)
            return ele[i].value;
    }

    return null;
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

function AddAutoGrabber(location, x, y) {

    RunDebugCommand("addgrabber " + location + " " + x + " " + y);
    document.getElementById("grabber").innerHTML = "Yes";
}

function AddHeater(location, x, y) {

    RunDebugCommand("addheater " + location + " " + x + " " + y);
    document.getElementById("heater").innerHTML = "Yes";
}

function SetFriendshipLevel(npc, level) {

    RunDebugCommand("friend " + npc + " " + level, "popup");
}

function SendJunimos(location, x, y) {
    RunDebugCommand("sendjs " + location + " " + x + " " + y);
}

function ExecuteQueryForItem(sQuery) {

    document.getElementById("waitgif").style.display = "block";
    var sURL = "/Search?query=" + sQuery;

    fetch(sURL).then(function (response) {
        return response.text();
    }).then(function (string) {
        AddPageHistory(string);
        document.getElementById("lookuptext").innerHTML = string;
        document.getElementById("lookuptext").scrollTop = 0;
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
function PopAndSearchWithCat(sId, sCat) {
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
        var myDiv = document.getElementById('lookuptext');
        myDiv.innerHTML = string;
        myDiv.scrollTop = 0;
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
        var myDiv = document.getElementById('lookuptext');
        myDiv.innerHTML = string;
        myDiv.scrollTop = 0;
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
    document.getElementById("showtrain").checked = bState;
    document.getElementById("fillhay").checked = bState;
    document.getElementById("showcook").checked = bState;
    document.getElementById("showconds").checked = bState;
    document.getElementById("showharvest").checked = bState;
    document.getElementById("showseth").checked = bState;
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

function GetBuilding() {
    var buidling = document.getElementById("buildings").value;
    var split1 = buidling.split('!!');
    var split2 = split1[1].split('.');


    var sUrl = "/Production/buildingdetails/" + split1[0] + "/" + split2[0] + "/" + split2[1];


    fetch(sUrl).then(function (response) {
        return response.text();
    }).then(function (string) {
        document.getElementById("building_details").innerHTML = string;
    }).catch(function (err) {
        document.getElementById("building_details").innerHTML = 'Error: ' + err;

    });
}

function FillHaySlots(location, posx, posy) {

    var sUrl = "/Utils/fillhay/" + location + "/" + posx + "/" + posy;


    fetch(sUrl).then(function (response) {
        return response.text();
    }).then(function (string) {
        document.getElementById("hay").innerHTML = string;
    }).catch(function (err) {
        //document.getElementById("building_details").innerHTML = 'Error: ' + err;
    });
}

function ToogleBuildingDoor(location, posx, posy) {

    var sUrl = "/Utils/doortoggle/" + location + "/" + posx + "/" + posy;


    fetch(sUrl).then(function (response) {
        return response.text();
    }).then(function (string) {
        var door = document.getElementById("animaldoor");
        if (door.innerHTML == "Open") {
            door.innerHTML = "Closed";
        } else {
            door.innerHTML = "Open";
        }
    }).catch(function (err) {
        //document.getElementById("building_details").innerHTML = 'Error: ' + err;
    });
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

var iDragCounter = 0;

function drag(ev, el) {
    console.debug("Drag started");
    ev.dataTransfer.setData("text", el.id + "^^^" + ev.target.id);
    console.debug("Drag ID:" + el.id);
}

function shippingbindrop(ev) {
    ev.preventDefault();

    var data = ev.dataTransfer.getData("text").split("^^^");
    console.debug("Source ID:" + data[1]);

    RunDebugCommand("moveitem " + data[0].replaceAll(" ", "+") + " sbin~!100", null, MoveComplete);

    document.getElementById(data[0]).innerText = "Empty";
}

function trashdrop(ev) {
    ev.preventDefault();

    var data = ev.dataTransfer.getData("text").split("^^^");
    console.debug("Source ID:" + data[1]);

    RunDebugCommand("removeItem " + data[0].replaceAll(" ", "+"), null, MoveComplete);

    document.getElementById(data[0]).innerText = "Empty";
}



function dragenter(e, el) {
    e.preventDefault();
    el.classList.add('dragging');
    //console.debug("Drag enter: parent div id='" + el.id + "', nodename='" + el.nodeName + "', e.nodeName='" + e.currentTarget.nodeName + "'");
}

function dragleave(e, el) {
    e.preventDefault();
    el.classList.remove('dragging');
    //console.debug("Drag exit: parent div id='" + el.id + "', nodename='" + el.nodeName + "'");
}

function drop(ev, el) {
    ev.preventDefault();

    var data = ev.dataTransfer.getData("text").split("^^^");
    console.debug("Source ID:" + data[1]);

    //var sTargetId = ev.originalTarget.id;
    var sTargetId = el.id;
    var sourceChest = data[0].split('#')[0];

    if (data[0] != sTargetId && ((data[0].split('#')[0] != sTargetId.split('#')[0]) || data[0].substring(0, 3) == "bp:")) {
        RunDebugCommand("moveitem " + data[0].replaceAll(" ", "+") + " " + sTargetId.replaceAll(" ", "+"), null, MoveComplete);

        var oMoveElment = document.getElementById(data[1]);

        //ev.target.innerText = "";
        //ev.target.appendChild(oMoveElment);
        //ev.target.classList.remove('dragging');
        console.debug("drop el id ='" + el.id + "', ev id='" + ev.id + "'");

        el.innerText = "";
        el.appendChild(oMoveElment);
        document.getElementById(data[0]).innerText = "Empty";
    }
    el.classList.remove('dragging');


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
function UpdateGamePerf() {
    var sUrl = "/Json/perfdata";
    fetch(sUrl)
        .then(response => {
            if (response.ok) {
                response.json().then(data => {
                    if (data.Loaded) {
                        document.getElementById('actjs').innerHTML = data.ActJunimos;
                    }
                    else {
                        document.getElementById('actjs').innerHTML = "-";
                    }
                    iGamePerfErrorCount = 0;
                });
            } else {
                PerfError();
            }
        })
        .catch(error =>
            PerfError()
        );
    if (iGamePerfErrorCount < 10) {
        setTimeout(UpdateGamePerf, iGamePerfRefresh * (iGamePerfErrorCount + 1));
    }
}
function PerfError() {
    iGamePerfErrorCount++;
    document.getElementById('actjs').innerHTML = "?";
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
                    document.getElementById('hud_warn').innerHTML = data.TtoT;
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
    document.getElementById('hud_warn').innerHTML = "";

}
function deleteForage(forageId) {
    //
    //  delete an forage entry from an expansion
    //
    if (confirm("This will delete the Forage item from the list.  Continue?")) {

        var xmlHttp = new XMLHttpRequest();
        xmlHttp.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200) {
                const element = document.getElementById(forageId);
                element.remove();
            }
        }
        var url = "/ModInfo/dforage/" + forageId.replaceAll("~", "/");
        xmlHttp.open("POST", url, true);
        xmlHttp.send(null);
    }
}
function deleteFish(fishId) {
    //
    //  delete an forage entry from an expansion
    //
    if (confirm("This will delete the Fish item from the list.  Continue?")) {

        var xmlHttp = new XMLHttpRequest();
        xmlHttp.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200) {
                const element = document.getElementById(fishId);
                element.remove();
            }
        }
        var url = "/ModInfo/dfish/" + fishId.replaceAll("~", "/");
        xmlHttp.open("POST", url, true);
        xmlHttp.send(null);
    }
}
function deleteArtifact(artId) {
    //
    //  delete an artifact entry from an expansion
    //
    if (confirm("This will delete the Artifact from the list.  Continue?")) {

        var xmlHttp = new XMLHttpRequest();
        xmlHttp.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200) {
                //document.getElementsByTagName('body')[0].innerHTML = xmlHttp.responseText;
                //SubPageLoaded('Mods', 'Realty');
                const element = document.getElementById(artId);
                element.remove();
            }
        }
        var url = "/ModInfo/dart/" + artId.replaceAll("~", "/");
        xmlHttp.open("POST", url, true);
        xmlHttp.send(null);
    }
}

function addArtifact(expName, artifactId, chance, season) {

    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            document.getElementById('artilist').innerHTML += xmlHttp.responseText;
            //SubPageLoaded('Mods', 'Realty');
            //
            //  add code to add new artifact box
            //
        }
    }
    var url = "/ModInfo/aart/" + expName + "/" + artifactId + "/" + chance + "/" + season;
    xmlHttp.open("POST", url, true);
    xmlHttp.send(null);
}

function addForage(expName, artifactId, chance, season) {

    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            switch (forageSeason) {
                case 0:
                    document.getElementById('foragesplist').innerHTML += xmlHttp.responseText;
                    break;
                case 1:
                    document.getElementById('foragesulist').innerHTML += xmlHttp.responseText;
                    break;
                case 2:
                    document.getElementById('foragefalist').innerHTML += xmlHttp.responseText;
                    break;
                case 3:
                    document.getElementById('foragewilist').innerHTML += xmlHttp.responseText;
                    break;
            }
        }
    }
    var url = "/ModInfo/afor/" + expName + "/" + artifactId + "/" + chance + "/" + season;
    xmlHttp.open("POST", url, true);
    xmlHttp.send(null);
}
function addFishToFishArea(expName,areaId, fishId, chance, season) {
    //
    //  add fish to fish area
    //
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            switch (fishSeason) {
                case 0:
                    document.getElementById('fishareasplist').innerHTML += xmlHttp.responseText;
                    break;
                case 1:
                    document.getElementById('fishareasulist').innerHTML += xmlHttp.responseText;
                    break;
                case 2:
                    document.getElementById('fishareafalist').innerHTML += xmlHttp.responseText;
                    break;
                case 3:
                    document.getElementById('fishareawilist').innerHTML += xmlHttp.responseText;
                    break;
            }
        }
    }
    var url = "/ModInfo/addtofisharea/" + expName + "/"+ areaId+ "/" + fishId + "/" + chance + "/" + season;
    xmlHttp.open("POST", url, true);
    xmlHttp.send(null);
}
function addFish(expName, fishId, chance, season) {
    //
    //  add fish data to expansion area
    //
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            switch (fishSeason) {
                case 0:
                    document.getElementById('fishsplist').innerHTML += xmlHttp.responseText;
                    break;
                case 1:
                    document.getElementById('fishsulist').innerHTML += xmlHttp.responseText;
                    break;
                case 2:
                    document.getElementById('fishfalist').innerHTML += xmlHttp.responseText;
                    break;
                case 3:
                    document.getElementById('fishwilist').innerHTML += xmlHttp.responseText;
                    break;
            }
        }
    }
    var url = "/ModInfo/afish/" + expName + "/" + fishId + "/" + chance + "/" + season;
    xmlHttp.open("POST", url, true);
    xmlHttp.send(null);
}
function dm_GameData_Seasons(dataType, dataId, itemName, htmlId, removedit) {
    //
    //  save/remove Seasons settings
    //

    //
    //  get values
    //
    var newValue = [];
    var index = 0;
    if (document.getElementById(htmlId + "_sea_spring").checked) {
        newValue[index] = "Spring";
        index++;
    }
    if (document.getElementById(htmlId + "_sea_summer").checked) {
        newValue[index] = "Summer";
        index++;
    }
    if (document.getElementById(htmlId + "_sea_fall").checked) {
        newValue[index] = "Fall";
        index++;
    }
    if (document.getElementById(htmlId + "_sea_winter").checked) {
        newValue[index] = "Winter";
    }
    //
    //  create Json payload
    //
    var jdata = JSON.stringify({
        "DataType": dataType,
        "DataItemId": dataId,
        "FieldName": htmlId,
        "Value": null,
        "ListValues": newValue,
        "DataItemName": itemName,
        "isList": true
    });
    //
    //  send update
    //
    dm_SendData(jdata, removedit);

}
function dm_GameData_TOD(dataType, dataId, itemName, htmlId, removedit) {
    //
    //  save/remove TOD settings
    //

    //
    //  get values
    //
    var newValue = [];
    var index = 0;
    if (document.getElementById(htmlId + "_time_day").checked) {
        newValue[index] = "Day";
        index++;
    }
    if (document.getElementById(htmlId + "_time_night").checked) {
        newValue[index] = "Night";
    }
    //
    //  create Json payload
    //
    var jdata = JSON.stringify({
        "DataType": dataType,
        "DataItemId": dataId,
        "FieldName": htmlId,
        "Value": null,
        "ListValues": newValue,
        "DataItemName": itemName,
        "isList": true
    });
    //
    //  send update
    //
    dm_SendData(jdata, removedit);

}

function dm_SendData(payload, removeEdit) {
    //
    //  create HTTP request
    //
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            dataItemSel();
        }
    }
    var url;
    if (removeEdit) {
        url = "/Data/editor/dmdelmod";
    }
    else {
        url = "/Data/editor/dmsetmod";
    }
    xmlHttp.open("POST", url, true);
    xmlHttp.setRequestHeader("Content-Type", "application/json");
    xmlHttp.send(payload);
}
function dm_GameData(dataType, dataId, itemName, htmlId, isCombo, removeEdit) {
    //
    //  save game data cusomization
    //
    var newValue;
    if (isCombo) {
        newValue = document.getElementById(htmlId).value
    }
    else {
        newValue = document.getElementById(htmlId).value
    }
    var jdata = JSON.stringify({
        "DataType": dataType,
        "DataItemId": dataId,
        "FieldName": htmlId,
        "Value": newValue,
        "DataItemName": itemName,
        "isList": false
    });

    dm_SendData(jdata, removeEdit);
}
function SaveCustomLocParameters() {
    //
    //  save custom location parameters
    //
    //  - seasonal override
    //  - weather override
    //  - allow giant crops
    //
    var selWeather = document.querySelector('input[name="weather"]:checked');
    var weather = "";
    if (selWeather != null) {
        weather = selWeather.value;
    }
    var jdata = JSON.stringify({
        "ExpansionName": document.getElementById("expname").innerText,
        "AllowGiantCrops": document.getElementById("allowgiant").checked,
        "AlwaysSnowing": weather == "snowing",
        "AlwaysRaining": weather == "raining",
        "AlwaysSunny": weather == "sunny",
        "CrowsEnabled": document.getElementById("allowcrows").checked,
        "SeasonOverride": document.getElementById("seasonlist").value
    });

    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            document.getElementsByTagName('body')[0].innerHTML = xmlHttp.responseText;
            SubPageLoaded('Mods', 'Realty');
            ShowPopUp('Settings saved.  The settings will not take effect until the save is re-loaded.', 6000);
        }
    }
    var url = "/ModInfo/saveV2";
    xmlHttp.open("POST", url, true);
    xmlHttp.setRequestHeader("Content-Type", "application/json");
    xmlHttp.send(jdata);
}
function deChanged() {
    //
    //  change status of the data editor
    //
    var sUrl = "/Data/enabled/" + (document.getElementById("enableDE").checked ? "true" : "false");
    fetch(sUrl).then(function (response) {
        return response.text();
    }).then(function (string) {

    }).catch(function (err) {
        alert(err);
    });
}

function addColor(htmlId, destId) {
    var newcolor = document.getElementById(htmlId).value;
    if (document.getElementById(destId).value == null || document.getElementById(destId).value == "") {
        document.getElementById(destId).value = "[" + hexToRgb(newcolor).toString().replaceAll(",", " ") + "]";
    } else {
        document.getElementById(destId).value = document.getElementById(destId).value.trim() + ", [" + hexToRgb(newcolor).toString() + "]";
    }
}
function hexToRgb(h) { return ['0x' + h[1] + h[2] | 0, '0x' + h[3] + h[4] | 0, '0x' + h[5] + h[6] | 0] }

function dm_rule_status(checkbox, ruleid, refresh) {
    //
    //  enabled or disable an edit ule
    //
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            if (refresh) dataItemSel();
        }
    }

    var url = "/Data/dmrule/" + ruleid + "/" + checkbox.checked;

    xmlHttp.open("POST", url, true);
    xmlHttp.setRequestHeader("Content-Type", "application/json");
    xmlHttp.send("");
}
function fa_area_sel(expansion) {
    //
    //  get selected FishAreaDetails
    //
    var fishArea = document.getElementById("fisharea").value;
    fetch("/ModInfo/fisharea/" + expansion + "/" + fishArea).then(function (response) {
        return response.text();
    }).then(function (string) {
        document.getElementById('fishAreaDetails').innerHTML = string;
    }).catch(function (err) {
        document.getElementById('fishAreaDetails').innerHTML = err;
    });
}

function dm_GetSubData(dataType, subDataType,primaryId, itemHTMLId, outputId)
{
    //
    //  get data manager sub data
    //
    var itemId = document.getElementById(itemHTMLId).value;
    fetch("/Data/subdata/" + dataType + "/" + subDataType + "/" + primaryId + "/" + itemId).then(function (response) {
        return response.text();
    }).then(function (responseText) {
        document.getElementById(outputId).innerHTML = responseText;
    }).catch(function (err) {
        document.getElementById(outputId).innerHTML = err;
    });
}