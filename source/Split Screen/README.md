**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Ilyaki/SplitScreen**

----

# SplitScreen
Stardew Valley mod that enables split screen multiplayer with multiple controllers, keyboards or mice.

Mod works by running multiple instances of StardewModdingAPI.exe, SplitScreen allows input to be received and used by inactive windows. Also creates "virtual/fake" mouse for each instance

Use with WindowResize mod to resize windows smaller: https://www.nexusmods.com/stardewvalley/mods/2266


(All these overwrites are made with https://github.com/pardeike/Harmony)

BREAKDOWN OF MOD:
 * Game1.game1.InactiveSleepTime = 0 : no fps throttling when window inactive
 * XNA Keyboard/GamePad/Mouse.GetState is overwritten to pass only one device in
 * XNA SetMouse is also overwritten to set a fake mouse (Stardew Valley sets the mouse when using a gamepad to mimick the mouse cursor moving)
 * Raw input for keyboard is determined using library: https://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard
 * Mouse is also overwritten by mouse obtained from a slightly modified (I removed a Console.WriteLine) RawInputSharp: http://jstookey.com/arcade/rawmouse/ 
 * The OS mouse is locked in place by System.Windows.Forms.Cursor.Clip and an embedded autohotkey script (see MouseDisabler)
