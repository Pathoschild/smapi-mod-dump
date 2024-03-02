# Sprint-And-Dash-Redux
 Stardew Valley mod adding new sprinting and dashing functions.

1. INTRO

This mod adds the following functions to the game:
-Sprinting: Moving faster than usual in exchange for stamina while moving. By default, this uses the Space Key to toggle this behavior on or off, but it can be configured to only activate when the key is held down.
-Dashing: A temporary skill-scaling powerup that buffs your speed, attack power, and defense for a time, with an associated cooldown/vulnerability phase. Think of it as a combat panic button. By default activated with the Q Key.
-Getting winded: Progressive stamina drain over time such that the longer you sprint for, the more stamina you will lose every second.

Gamepad/controller users: Although I use the word "key" throughout this document, under the hood the code can recognize controller buttons too. The only limitation (for now) is some logic used to detect whether you are running still expects a key. Since I think 99% of users probably autorun, this will likely not matter.

Source code is included in the install package and available here: https://github.com/littleraskol/Sprint-And-Dash-Redux

2. CREDITS

This is an adaption of the work of OrSpeeder, who created the original Sprint and Dash Mod, and released it under the GNU General Public License. Sprint and Dash Mod Redux will also use it. I recommend checking his version of the mod out if this isn't your thing, or to check out his other mods.

Sprint and Dash Mod Redux is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

For information on this license, see:

Sprint and Dash Mod Redux mod is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

Source code is packaged with the mod.

3. CONFIGURATION

Note that while this section refers to the config file, you can change settings in game using the Generic Mod Config Menu (GMCM) interface.

Sprint and Dash Mod Redux can be configured to change the keys for its sprint and dash functions, the stamina cost per tick of sprinting (stamCost), and the duration of the combat dash buff (dashDuration). Additionally, there is a new "winded" feature to use or not at your choice. You can also select whether to use the sprint (and normal "run") keys as a toggle, where pressing once turns it on until you press again.

StamCost: Measured in points of stamina, how much sprinting costs per second. By default, this is 2. Minimum is 1, lower values will be treated as 1.

DashDuration: Measured in seconds. By default this is 4 seconds. Note that the cooldown will last for 2.5x as long as whatever you set its length to (i.e., by default 10 seconds). Minimum is 1 second, maximum is 10 seconds. Values beyond the maximum will be treated as the maximum. Values below the minimum (e.g., 0) will turn the dash feature off.

WindedStep: Measured in seconds. By default, this is 5 seconds. Set to 0 to ignore/deactivate this system. Explained in more depth below.

QuitSprintingAt: Stop player from sprinting when stamina drops below a percentage of max stamina defined by this value (such that 0.XX = XX%). Default is 0.1 (10%). Minimum is 0, lower values will be treated as 0 Maximum is 0.99, higher values will be treated as 0.99.

ToggleMode: Whether to use the sprint (and normal "run") keys as a toggle, where pressing once turns it on until you press again to turn it off. By default, this is true. Note that this will turn the run key into a toggle, due to the need to avoid applying the sprint buff while walking. (I kind of assume everyone is using autorun tbh.) When set to false, you only sprint while the button is pressed.

Finally, there are two "hidden" config items that are advanced and not needed by most players. These do not appear in the config file by default but can be added.

TimeInterval: How often, in seconds, to actually perform regeneration calculations. By default this is 0.1, meaning every tenth of a second (or about 6 game ticks). This value is validated such that we always end up with an interval between 1 and 60 ticks (inclusive), therefore it makes no sense to enter more than 1, or 0 or less. This setting is not really useful unless you have major performance issues. The higher this number is, the less frequently the mod will do any real calculations and therefore the less work it makes your computer do. The lower this number is, though, the more responsive the mod will be to changes in state. Setting this value too low may cause some issues if you rapidly press the sprint key for some reason.

VerboseMode: By default it is "false" and controls whether to output regular calibration data.

4. THE WINDED SYSTEM

This system puts a brake on sprinting for long periods of time beyond the basic per-second stamina cost. The idea is that the longer you sprint for, the more it tires you out, making you "winded" after a certain amount of time set by the config value.

For every (windedStep) seconds you run, the stamina cost per second of running increases by 100% of the base. For example, at the default config values: for the first five seconds, it costs 2 stam/sec, then after that for the next five (6-10) seconds, 4 stam/sec, then after that for the next five (11-15) seconds, 6 stam/sec, etc... It will also keep track of your sprint time if you sprint for less than the step time, and pick back up where you left off. No cheesing with 4-second sprint bursts!

You will also need to take a bit of a break from sprinting if you want this to go away. It takes one second to lower the penalty by 100% of base. Continuing the example above: when you stop sprinting, if you immediately start again, the 6 stam/sec cost remains. If you stop sprinting for 1 second, it goes down to 4/sec, and if you stop sprinting for another second, it goes down to 2/sec (base).

5. CHANGELOG

v2.1.X (??/??/??)
-Minor GMCM option description change.

v2.1.0 (09/16/20)
-Settings can now be changed in game using Generic Mod Config Menu.

v2.0.1 (9/15/20)
-Fixed bug with interval system that prevented proper continuity of sprint buff and activation of winded system.
-Also fixed issue that allowed briefly being able to sprint even when stamina was under the min req.

v2.0.0 (09/13/20)
-Updated to most recent SMAPI and Stardew Valley.
-Toggle is default behavior.
-Winded after increments of 5 seconds of sprinting is default behavior.
-Fix: Sprint no long drains stamina or accumulates winded-ness if not moving.
-Dash buff minimal buff of +1.
-Dash buff can no longer kill the player (I think??).
-Support for controller buttons (can't test this though, don't have a controller).

v1.2.0 (02/09/2017)
-More robust check for appropriate conditions (uses existing SDV function to determine if "time should pass.")
-Toggle mode transition logic improved so that beginning a sprint will end "walking" and vice-versa.

v1.1.0 (01/25/2017)
-Added option to use the sprint (and run) keys in a toggle mode.
-Coding cleanup/reorganization.

v1.0.1 beta (01/16/2017):
-Minor patch to remove to-be-deprecated stuff in code.

v1.0.0 beta (09/24/2016):
-Fully implemented winded feature.
-Partially fixed issue persistent effect after mounting horse (from 0.0.1a).
-Fixed existing issue where stamina would drain just due to holding the sprint key even if not moving.
-Some code reworking. Possibly more efficient? (I hope!)

v0.1.0 alpha (09/23/2016):
-Experimental test of using comparisons of Buff references rather than Buff.which to identify Buffs. (This worked and seems more usable/extensible.)

v0.0.2 alpha (09/22/2016):
*Reworked min stamina to sprint calcs; should not continue sprinting once stam < 30.
*Started adding winded-ness logic. Incomplete.

v0.0.1 alpha (09/21/2016):
*Added the ability to set the stamina cost per tick of sprinting and the duration of the combat dash buff in a config file.
*Prevented sprinting and dashing while on horseback.

5. KNOWN ISSUES

*Dashing and riding the horse will probably always have wonky interactions, but this is kind of an edge case so whatevs.
*The "winded" condition may persist throughout a cutscene if one begins while winded.

7. FUTURE PLANS

None, unless I can get Stardew Cofig Menu working again.

8. CONTROLS

I've set the default keys to the ones I use: spacebar for sprint, Q for dash. In the past, changing this required using a code number system. That's no longer the case but I will leave that index here (below) in case it is needed for any reason. Otherwise, you should use this reference for other keys/buttons/etc: https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings

None = 0,
Back = 8,
Tab = 9,
Enter = 13,
Pause = 19,
CapsLock = 20,
Kana = 21,
Kanji = 25,
Escape = 27,
ImeConvert = 28,
ImeNoConvert = 29,
Space = 32,
PageUp = 33,
PageDown = 34,
End = 35,
Home = 36,
Left = 37,
Up = 38,
Right = 39,
Down = 40,
Select = 41,
Print = 42,
Execute = 43,
PrintScreen = 44,
Insert = 45,
Delete = 46,
Help = 47,
D0 = 48,
D1 = 49,
D2 = 50,
D3 = 51,
D4 = 52,
D5 = 53,
D6 = 54,
D7 = 55,
D8 = 56,
D9 = 57,
A = 65,
B = 66,
C = 67,
D = 68,
E = 69,
F = 70,
G = 71,
H = 72,
I = 73,
J = 74,
K = 75,
L = 76,
M = 77,
N = 78,
O = 79,
P = 80,
Q = 81,
R = 82,
S = 83,
T = 84,
U = 85,
V = 86,
W = 87,
X = 88,
Y = 89,
Z = 90,
LeftWindows = 91,
RightWindows = 92,
Apps = 93,
Sleep = 95,
NumPad0 = 96,
NumPad1 = 97,
NumPad2 = 98,
NumPad3 = 99,
NumPad4 = 100,
NumPad5 = 101,
NumPad6 = 102,
NumPad7 = 103,
NumPad8 = 104,
NumPad9 = 105,
Multiply = 106,
Add = 107,
Separator = 108,
Subtract = 109,
Decimal = 110,
Divide = 111,
F1 = 112,
F2 = 113,
F3 = 114,
F4 = 115,
F5 = 116,
F6 = 117,
F7 = 118,
F8 = 119,
F9 = 120,
F10 = 121,
F11 = 122,
F12 = 123,
F13 = 124,
F14 = 125,
F15 = 126,
F16 = 127,
F17 = 128,
F18 = 129,
F19 = 130,
F20 = 131,
F21 = 132,
F22 = 133,
F23 = 134,
F24 = 135,
NumLock = 144,
Scroll = 145,
LeftShift = 160,
RightShift = 161,
LeftControl = 162,
RightControl = 163,
LeftAlt = 164,
RightAlt = 165,
BrowserBack = 166,
BrowserForward = 167,
BrowserRefresh = 168,
BrowserStop = 169,
BrowserSearch = 170,
BrowserFavorites = 171,
BrowserHome = 172,
VolumeMute = 173,
VolumeDown = 174,
VolumeUp = 175,
MediaNextTrack = 176,
MediaPreviousTrack = 177,
MediaStop = 178,
MediaPlayPause = 179,
LaunchMail = 180,
SelectMedia = 181,
LaunchApplication1 = 182,
LaunchApplication2 = 183,
OemSemicolon = 186,
OemPlus = 187,
OemComma = 188,
OemMinus = 189,
OemPeriod = 190,
OemQuestion = 191,
OemTilde = 192,
ChatPadGreen = 202,
ChatPadOrange = 203,
OemOpenBrackets = 219,
OemPipe = 220,
OemCloseBrackets = 221,
OemQuotes = 222,
Oem8 = 223,
OemBackslash = 226,
ProcessKey = 229,
OemCopy = 242,
OemAuto = 243,
OemEnlW = 244,
Attn = 246,
Crsel = 247,
Exsel = 248,
EraseEof = 249,
Play = 250,
Zoom = 251,
Pa1 = 253,
OemClear = 254