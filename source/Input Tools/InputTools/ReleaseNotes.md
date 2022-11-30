**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/sagittaeri/StardewValleyMods**

----


# Release Notes

## Version 0.2.0

Warning: API-breaking changes from previous version. Be sure to copy the new interface file. I don't plan to break API frequently,
but since this is the 2nd release after getting a bunch of great feedback from the initial release, I figured it's better I do it
now than layer when there are more users.

- API change: Renamed InputStack to InputLayer (and thus most references to Stack is now Layer) which is more closely reflected to what it is, which should eliminate some confusion
- API change: Is_X_Pressed etc now return the SButton instead of the InputDevice
- Fix: Default actions (Confirm, MoveX, etc) to use Stardew Valley keybinding options where possible
- New function: "PopLayer" and "PeekLayer" to make it clearer that the input layers are in a stack
- New function: "GetTextFromVirtualKeyboard" which works with keyboard, controller and mouse
- New event: "KeybindingConfigChanged" fires when SDV keybinding config is changed. Known issue: Reset keybindings button doesn't fire this event for now
- New config: "Instantly hide cursor when gamepad is used" is now a config that the user can disable

## Version 0.1.0

Initial release
