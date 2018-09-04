1. CREDITS

This is an adaption of the work of Maurício Gomes (OrSpeeder), who created the original Sprint and Dash Mod, and released it under the  GNU General Public License. Sprint and Dash Mod Redux will also use it.

Sprint and Dash Mod Redux is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

For information on this license, see: <http://www.gnu.org/licenses/>

Sprint and Dash Mod Redux mod is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

Source code is packaged with the mod as "SprintDashMain.cs.txt". Remove the ".txt" to use.

2. CONFIGURATION

Sprint and Dash Mod Redux (S&DR) has an expanded config file. In addition to the keys for its sprint and dash functions (but see "CONTROLS," below), you can set the stamina cost per tick of sprinting (stamCost) and the duration of the combat dash buff (dashDuration). Additionally, there is a new "winded" feature to use or not at your choice. You can also select whether to use the sprint (and normal "run") keys as a toggle, where pressing once turns it on until you press again.

stamCost: Measured in points of stamina. By default, this is 2. Minimum is 1, lower values will be treated as 1.

dashDuration: Measured in seconds. By default this is 4 seconds. Note that the cooldown will last for 2.5x as long as whatever you set its length to (i.e., by default 10 seconds). Minimum is 1 second, maximum is 10 seconds. Values beyond that will be set to the min or max as appropriate.

windedStep: Measured in seconds. By default, this is 5 seconds. Set to 0 to ignore/deactivate this system. Explained in more depth below. 

toggleMode: Whether to use the sprint (and normal "run") keys as a toggle, where pressing once turns it on until you press again.

Gamepad users: The mod doesn't have code to support a gamepad. However, you can use a third-party program called Joytokey to map keyboard buttons to gamepad buttons for now. Link: http://joytokey.net/en/

3. THE WINDED SYSTEM

This system puts a brake on sprinting for long periods of time beyond the stamina cost. The idea is that the longer you sprint for, the more it tires you out, making you "winded" after a certain amount of time set by the config value. 

For every (windedStep) seconds you run, the stamina cost per tick of running increases by 100% of the base. For example, at the default config values: for the first five seconds, it costs 2 stam/tick, then after that for the next five (5-10) seconds, 4 stam/tick, then after that for the next five (10-15) seconds, 6 stam/tick, etc...

You will also need to take a bit of a break from sprinting if you want this to go away. It takes 1/5 of the "step" time to lower the penalty by 100% of base. Continuing the example above: when you stop sprinting, if you immediately start again, the 6 stam/tick cost remains. If you stop sprinting for 1 second, it goes down to 4/tick, and if you stop sprinting for another second, it goes down to 2/tick (base).

You'll know how long you need to wait before sprinting at the base rate again because the cooldown effect has a light yellow glow. It will also keep track of your sprint time if you sprint for less than the step time, and pick back up where you left off. No cheesing with 4-second sprint bursts!

4. CHANGELOG

v1.3.0 (05/14/2017)
-Updated for SDV 1.2

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

6. FUTURE PLANS

None at this time.

7. CONTROLS

I've changed the default keys to the ones I use: spacebar for sprint, Q for dash. If you want to set these to something else, consult this list:

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