Speeder's Configurable Improved Sprinklers
==========================================
 
This is a mod for the game [Stardew Valley](http://stardewvalley.net/), it improves the existing sprinklers and let you configure them to your liking, it also has highlight area for scarecrows and sprinklers.

![Game Screenshot](screenshot.png)

The config hotkey is "K", you can edit it in your config file. Sadly, there is no way to edit the hotkey in-game (yet).

As for how the mod works: It adds extra watering area to the existing sprinklers, it can't remove watering area from the existing sprinklers though.

The idea for this mod originally was only to improve the normal sprinkler, that is pretty terrible, and for optimal use requires a non-standard farm organization that cause collision issues with the main character.

The 1.x version was rewritten entirely for this 2.0 release, to support future sprinkler mods, UI-based configuration, and other nice features, including autobalance.

Features
--------

* Default sprinkler shapes allow more flexibility in farm layouts.
* You can use any shape you want, literally.
* Infinite sprinkler range (requires editing the config file manually though, not recommended).
* Automatic balancing, sprinkler monetary value, and crafting cost increase as you increase the sprinkler area, ensuring the game stay balanced.
* You CAN cheat though, in two different ways, but I won't explain how, this is for the more adventurous people to mess with.
* When placing sprinklers or scarecrows you can a highlight of the area they cover.
* After pressing a hotkey (default, F3) you can also see highlight of the areas when hovering the cursor over existing sprinklers and scarecrows.

Usage
-----

Press K, edit what you want, and click OK.

Examples of what can be done:

Circle

![Game Screenshot](circle.png)

Butterfly Shape (like real world sprinklers)

![Game Screenshot](butterfly.png)

The sprinklers activate at night, like the normal game rules.

There is a feature that highlight scarecrow effect area, and sprinkler area, it is always on when you are going to place them, and can be turned on and off for ones that you are only hovering your cursor over them, the default button in the configuration is F3.

![Game Screenshot](scarecrowarea.png)

Visit the [Forum thread](http://community.playstarbound.com/threads/configurable-improved-sprinklers-scarecrow-and-sprinklers-area-highlights.112443/)

Changelog
---------

### 2.3

Corrected forum thread link, and default config minor error.

### 2.2

Updated for Stardew Valley 1.1 and SMAPI 0.40.0 1.1.

### 2.1

Added highlighting to the area of sprinklers and scarecrows.
Added grid rendering.
Added html readme.
Fixed a bug with the configuration key (it was always K even if you edited the config.json)

### 2.0.1

Fixed a mistake that made it incompatible with SMAPI 0.39.2

### 2.0

Updated to SMAPI 0.39.2
Added a GUI to configure the sprinklers.
Sprinklers now work on all farmable areas, including greenhouses and anything added by mods.

Installing
----------

Just unpack the .zip file in the mods folder.

Uninstalling
------------

Just delete the mod files again.

Credits
-------

Author: Maur&#237;cio Gomes (Speeder) [![Patreon](ipatreon.png)](https://patreon.com/user?u=3066937)

Special Thanks: Jesse and Pathoschild.

License
-------

The license of the project is [GPL3](https://gnu.org/licenses/gpl.html).

Source
------

Mod [source hosted on ![GitLab](igitlab.png)](https://gitlab.com/speeder1/SMAPISprinklerMod), special thanks to them! (that even provided free support when needed!)

[Installing a stable release from Nexus Mods](http://www.nexusmods.com/stardewvalley/mods/41/) is
recommended. If you really want to compile the mod yourself, just edit `SMAPISprinklerMod.csproj` and
set the `<GamePath>` setting to your Stardew Valley directory path. Launching the project in Visual
Studio will compile the code, package it into the mod directory, and start the game.

Binary Download Link
--------------------

You can download the compiled mod from [Nexus Mods](http://www.nexusmods.com/stardewvalley/mods/41/?).