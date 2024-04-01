var lastLeftDir = "";
var lastRightDir = "";
var Joy1;
var joy1InputPosX = 0;
var joy1InputPosY = 0;
var joy2InputPosX = 0;
var joy2InputPosY = 0;
var joy1Direzione;
var joy1X;
var joy1Y;

var Joy2;
function JoyStickLoaded() {

     Joy1 = new JoyStick('joy1Div');
    Joy2 = new JoyStick('joy2Div');

    //joy1IinputPosX = document.getElementById("joy1PosizioneX");
    //joy1InputPosY = document.getElementById("joy1PosizioneY");
    //joy1Direzione = document.getElementById("joy1Direzione");
    //joy1X = document.getElementById("joy1X");
    //joy1Y = document.getElementById("joy1Y");

    //setInterval(function () { joy1IinputPosX.value = Joy1.GetPosX(); }, 50);
    //setInterval(function () { joy1InputPosY.value = Joy1.GetPosY(); }, 50);
    setInterval(CheckLeftThumb, 50);
    setInterval(CheckLeftThumb1, 50);
    //setInterval(function () { joy1X.value = Joy1.GetX(); }, 50);
    //setInterval(function () { joy1Y.value = Joy1.GetY(); }, 50);
    //document.body.requestFullscreen();
}

function SendToken(token) {

    fetch("/phonepad/" + token);

}

function CheckLeftThumb() {
    //joy1Direzione.value = Joy1.GetDir();

    //if (joy1InputPosX != Joy1.GetPosX() || joy1InputPosY != Joy1.GetPosY())

    if (lastLeftDir != Joy1.GetDir()) {
        lastLeftDir = Joy1.GetDir();
        joy1InputPosX = Joy1.GetPosX();
        joy1InputPosY = Joy1.GetPosY();
        //SendToken('thumb/left/' + joy1InputPosX + "/" + joy1InputPosY);
        SendToken('thumb/left/' + lastLeftDir);
    }
}

function CheckLeftThumb1() {
    //joy1Direzione.value = Joy1.GetDir();

    //if (joy2InputPosX != Joy2.GetPosX() || joy2InputPosY != Joy2.GetPosY()) 
    if (lastRightDir != Joy2.GetDir()) {
        lastRightDir = Joy2.GetDir();
        joy2InputPosX = Joy2.GetPosX();
        joy2InputPosY = Joy2.GetPosY();
        //SendToken('thumb/right/' + joy2InputPosX + "/" + joy2InputPosY);
        SendToken('thumb/right/' + lastRightDir);
    }
}