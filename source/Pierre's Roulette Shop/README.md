**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/xynerorias/pierre-roulette-shop-SV**

----

# Pierre's Roulette Shop

This is a small Stardew Valley mod that adds a daily rotation to the seed shop in an effort to declutter it on heavier modlists.<br/> 
It is almost entirely configurable.

## Contents

* [Description](#description)

* [Installation](#installation)

* [Configurations](#configurations)

* [Compatibility](#compatibility)

## Description

The premise of this mod is pretty simple. When you have the entire PPJA suite of mods, your shops become oh so crowded and it's a pain to scroll through a billion crop seeds at Pierre's or the JojaMart.<br/>

I was heavily inspired by [Khadija's Recipe Shop](https://www.nexusmods.com/stardewvalley/mods/5245) but I didn't want to make an STF mod as I wanted maximum compatibility (wether it's with other mods or the game itself), with minimum dependencies, which apart from SMAPI itself of course, this mod has none.<br/>

The mod uses a pseudo-random to refresh the shop's stock everyday so that it is different every in-game day, and different on every save too. But if the player wants to replicate exactly the situation, they can if they use the exact same savegame name and have the exact same mod config (which outside of a testing environment, is pretty unlikely).

Visit the [Nexus page] for more informations.<br/>

## Installation

* Install [SMAPI](https://smapi.io/) (chances are if you're here, you already have it installed)
* Install the mod by downloading it either from [the Nexus], from the releases or by building it through source.
<br/>

If you do not know how to install mods, [refer to this guide](https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started)<br/>

## Configurations

|Setting|What it does|Available values|
|---|---|---|
|<code>JojaEnabled</code>|Wether or not the mod is enabled for the JojaMart.|*boolean* default: false|
|<code>Owners[]</code>|The list of shop owners the mod applies itself to. **In normal use case, this should stay as is.** <br/>But if the players wants to add a shop to the list, they should add the exact name of the owner (**for modders, that is their exact ShopMenu.portaitPerson.Name.** I didn't add a lot of checks as evn with crop mods, vendors without a portraitPerson -e.g Harvey, the Travelling Merchant...- aren't really crowded).<br/> So if the player wanted to add Sandy's shop for example, they would write ", "Sandy"" without the first quotation marks.|*string array* default: [ "Pierre" ]|
|<code>Mode</code>|The mode the mod should use. The user can choose between three modes: **"OnlyCrops"**, **"OnlySaplings"** or **"Both"**. The first one makes the mod apply to only the crop seeds in the shop, the second only to the saplings and lastly the third to both of them. The mod only uses the relevant value from the last two configurations below: for example, if the mode is set to "OnlyCrops", the SaplingStock configuration below is of course not taken into account.|*string* default: "Both"|
|<code>SeedStock</code>|The number of seeds the shops should have daily in stock. Set to 0 to disable.|*integer* default: 2|
|<code>SaplingStock</code>|The number of saplings the shops should have daily in stock. Set to 0 to disable.|*integer* default: 2|

## Compatibility

Unless a big major game update that completely change how the shop works (which is unlikely), the mod should always be compatible with the latest version of the game, as long as SMAPI is updated and working.<br/>

As for mod compatibility, the mod here should once again be compatible with all other mods. If you find an incompatibility, please report the issue here and/or on [the Nexus page] <br/>

