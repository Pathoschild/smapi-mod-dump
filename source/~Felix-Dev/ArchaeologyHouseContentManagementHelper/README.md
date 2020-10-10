**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Felix-Dev/StardewMods**

----

**Archaeology House Content Management Helper** is a [Stardew Valley](http://stardewvalley.net/) mod that is aimed at improving 
the management of the Library/Museum. It provides status information about found books and contributed items and improves 
management of museum pieces. It largely fixes the in-game bug preventing item placement in certain cases and allows 
item rearrangement even if you have nothing to donate. 

Furthermore, to help the user bringing structure into his museum pieces arrangement, each selected item will display a tooltip 
showing detailed item information. To make rearrangement less tedious, items can be directly swapped with each other and 
gamepad cursors can now also be used to select an item (instead of only being usable when an item is selected). 

Lost Books can now be "grabbed & sent" to the library even if the user's inventory is full. No longer will the user have to make space for a book which it doesn't occupy anyway and leaves the user with an empty slot in the inventory. Once the player found all books, a congratulations message will be displayed.

Lost Books can now be read without having to go the library. Simply open the "Collections" page in the game menu and scroll to its last sidetab ("Lost Books"). The books found by the player so far will be listed there and they can be read by clicking on them. A book content preview is also shown on mouse hover.

Also fixes some small bugs in the original game (such as pressing the 'Ok' button next to the inventory will select the item 
beneath it instead of exiting the menu).

## Contents
* [Install](#install)
* [Use](#use)
* [Configure](#configure)
* [Showcase](#showcase)
* [Compatibility](#compatibility)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. [Install this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/2804/).
3. Run the game using SMAPI.

## Use
Just talk to Gunther and you will be presented with the updated menu. 
To close the museum menu (shown when you donate/replace items), simply press the [Exit] key you use for the game 
(default: key 'E') or press the [Cancel] button on your gamepad.

To read your found Lost Books, open the game menu -> select the Collections page -> select the "Lost Books" tab.

## Configure
The mod creates a `config.json` file in its mod folder the first time you run it. You can open that
file in a text editor to configure the mod.

These are the available settings:

| setting           | what it affects
| ----------------- | -------------------
| `MuseumItemDisplayTime` | Default `3000`. The display duration of a museum item description when a museum item is selected in the museum to be rearranged. In **milliseconds**. Available options:<ul><li>`-1`: infinite display duration.</li><li>`0`: No description will be shown.</li><li>`Greater than 0`: The specified display duration.</li></ul>
| `ShowVisualSwapIndicator` | Default `false`. If enabled, the visual "can-place" museum space indicator is also shown for spaces which are already filled.

## Showcase
* Talk to Gunther to see the extended interaction menu.
  ![](screenshots/libraryMuseum-interaction-menu.png)

* Select an item to rearrange it or see information about it.
  ![](screenshots/selecting-a-museum-item.png)

* See how you many books and items you have contributed so far.
  ![](screenshots/contributed-items-status.png)
  
* The [Lost Books] tab of the "Collections" page in action:
  ![](screenshots/collectionsPage-lostBooks-tab2.png)

## Compatibility
* Works with Stardew Valley 1.3 on Windows/Linux (Mac likely, but not tested).
* Works in single-player. Multiplayer works too, excluding lost books which do not seem to synchronize across players.
* No known mod conflicts.

## See also
* [Release notes](release-notes.md)
* [Nexus mod](http://www.nexusmods.com/stardewvalley/mods/2804)
