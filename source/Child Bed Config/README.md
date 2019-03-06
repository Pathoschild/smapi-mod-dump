# ChildBedConfig
<p>A very simple mod for Stardew Valley that can hide any combination of the crib and beds in the back room (i.e. show only the crib, hide one of the beds, hide everything).  Each save file can have a different set-up, so hiding the crib in File A will not hide it in File B.</p>

## Installation
* Get the file from <a href="https://www.nexusmods.com/stardewvalley/mods/3540">Nexus Mods</a>
* Unzip the contents of the file into Stardew Valley/Mods folder
* Run Stardew Valley
* Close the game and edit the newly-generated config file

## Configuring the Mod
After you've run the game once with SMAPI, a `config.json` file will appear in the mod folder.  You can edit this to hide the crib or beds for each save file.  The default config looks like this:

```
{
  "Farmers": [
    {
      "CharacterName": "NoName",
      "ShowCrib": true,
      "ShowBed1": true,
      "ShowBed2": true,
      "ShowCabinCrib": true,
      "ShowCabinBed1": true,
      "ShowCabinBed2": true,
    }
}
```

* `CharacterName`: The name of the farmer you want the following settings to affect.
* `ShowCrib`: If `true`, the crib will be shown; if `false` it will be hidden.
* `ShowBed1`: If `true`, the bed closest to the crib will be shown; if `false` it will be hidden.
* `ShowCrib`: If `true`, the bed furthest from the crib will be shown; if `false` it will be hidden.

`ShowCabinCrib`, `ShowCabinBed1`, and `ShowCabinBed2` do the same thing that their `ShowHome` counterparts do, except they affect the cabins. At this point in time, these settings will affect every cabin on the map.

For each farmer that you want to show/hide beds for, you need to add them to the config file.  It should look something like this:
```
{
  "Farmers": [
    {
      "CharacterName": "Character A",
      "ShowCrib": false,
      "ShowBed1": false,
      "ShowBed2": true,
      "ShowCabinCrib": true,
      "ShowCabinBed1": true,
      "ShowCabinBed2": true,
    },
    {
      "CharacterName": "Character B",
      "ShowCrib": true,
      "ShowBed1": false,
      "ShowBed2": false,
      "ShowCabinCrib": true,
      "ShowCabinBed1": true,
      "ShowCabinBed2": true,
    },
}
```

## Compatibility
* Stardew Valley 1.3
* SMAPI 2.10 - may work on older versions, but I haven't done any testing for them
* Will work other other mods that modify the farmhouse interior, provided that they don't change the location of the crib and beds

## Bugs & Other Issues
If you run into any problems, please don't hesitate to get in touch with me, either through the project's Issues tab on Github or on NexusMods.  This mod was tested on Windows 10, so I can't promise compatibility with Mac or Linux.

## Changelog
* <b>2/28/19:</b> Updated to ver 1.2, adding support for cabins and tidying up the code.  Now manipulates the tiles directly, allowing for compatibility for other farmhouse interior mods that change the map (provided they don't move the location of the crib!).
