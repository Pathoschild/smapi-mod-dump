**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/thespbgamer/ZoomLevel**

----

## ZoomLevel
You can increase zoom level with ``, (comma)`` and decrease it with ``. (period)``.

By holding ``Left Shift`` or ``Right Shift`` and using the controls above, you can change the UI Scale.

If you use a controller, you can also adjust it by pressing the ``left stick`` to decrease the zoom and ``right stick`` to increases the zoom.
By holding ``Left Trigger & Right Trigger`` and using the controls above, you can change the UI Scale.


By pressing ``Home`` to reset both values to 100%.

## Install:

1. [Install the latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
2. Unzip the mod folder into your Stardew Valley/Mods.
3. Run the game using SMAPI.

## Configuration:

In the mod folder open the file ``config.json``:
```
  - "IncreaseZoomOrUI": the keys that increases the Zoom or UI
  - "DecreaseZoomOrUI": the key that decreases the zoom or UI
  - "HoldToChangeUIKeys": the key that you need to hold to change the UI instead of the zoom
  - "SuppressControllerButton": when set to "true" it only changes the zoom level with the controller button and "false" to let the game also handle the button press
  - "ResetZoom": resets the zoom back to 100%
  - "ResetUI": resets the UI back to 100%
  - "ZoomAndUIControlEverywhere": if true then you can change your zoom and UI levels on menus, cutscenes, etc...
  - "ZoomLevelIncreaseValue": How much to increase the zoom level (needs to be a positive number)
  - "ZoomLevelDecreaseValue": How much to decrease the zoom level (needs to be a negative number)
  - "MaxZoomOutLevelAndUIValue": The zoom out value cap (default at 35%)
  - "MaxZoomInLevelAndUIValue": The zoom in value cap (default at 200%)
  - "ResetZoomValue": The value of the zoom level reset (default at 1)
  - "ResetUIValue": The value of the UI level reset (default at 1)
```

## Based on:
[This Mod](https://github.com/GuiNoya/SVMods/).

## Nexus Page: 
[Click here](https://www.nexusmods.com/stardewvalley/mods/7363).


