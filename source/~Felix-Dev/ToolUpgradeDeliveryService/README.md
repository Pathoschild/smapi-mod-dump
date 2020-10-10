**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Felix-Dev/StardewMods**

----

**Tool-Upgrade Delivery Service** is a [Stardew Valley](http://stardewvalley.net/) mod which makes retrieving upgraded farm tools 
less tedious. With this mod, Clint (the blacksmith) will simply send you a mail with the upgraded tool included as soon as 
the upgrade is finished. No more visiting the blacksmith simply to get your improved farm tool!

## Contents
* [Install](#install)
* [Use](#use)
* [Configure](#configure)
* [Showcase](#showcase)
* [Compatibility](#compatibility)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. [Install this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/2938).
3. Run the game using SMAPI.

## Use
Order a tool upgrade as usual. Check your mailbox the following days for a mail sent by Clint with the tool included. 
If you want, you can still receive the tool by visiting the blacksmith. In this case, the tool won't be included in the mail.

## Configure
The mod creates a `config.json` file in its mod folder the first time you run it. You can open that
file in a text editor to configure the mod.

These are the available settings:

| setting           | what it affects
| ----------------- | -------------------
| `RemoveToolDuplicates` | Default `false`. If enabled, all tool duplicates in the player's **inventory** will be removed when the player retrieves an upgraded tool by mail. I.e. if the player upgraded an Axe, all 'Axe' tools (copper/silver/...) will be removed.

## Showcase
* An example mail sent by Clint:
  ![](screenshots/tool-email.png)

## Compatibility
* Compatible with Stardew Valley 1.3 on Windows/Linux (Mac likely, but not tested).
* Works in both single-player and multiplayer.
* This mod is compatible with the mod [Rented Tools](https://www.nexusmods.com/stardewvalley/mods/1307). Enable the option `RemoveToolDuplicates` so that rented tools will be removed from the inventory. See [Configure](#configure) for more details.
* This mod is compatible with the mod [Prismatic Tools](https://www.nexusmods.com/stardewvalley/mods/2428).
* This mod is compatible with the mod [Rush Orders](https://www.nexusmods.com/stardewvalley/mods/605) (Rush Orders 1.1.4 and above).

## See also
* [Release notes](release-notes.md)
* [Nexus mod](https://www.nexusmods.com/stardewvalley/mods/2938)
