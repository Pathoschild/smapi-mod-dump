**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/AcidicNic/StardewValleyMods**

----

# Stardew Valley Mods

## [Special Orders Anywhere](https://www.nexusmods.com/stardewvalley/mods/7588)
Quickly cycle through varies menus. Access your Calendar, Daily Quests, Special Orders, Qi's Special Orders, and Journal anywhere.

### Install
- Install the latest version of SMAPI.
- Download the zip from the latest [release](https://github.com/AcidicNic/StardewValleyMods/releases/tag/latest) page or [NexusMods](https://www.nexusmods.com/stardewvalley/mods/7588).
- Extract the zip file into your Mods folder.
    - You can find the Mods folder using steam by right clicking Stardew Valley, then clicking Properties > Local Files > Browse

### Generic Mod Config Menu (Recommended)
1. Install the [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) mod from NexusMods.
2. You can edit settings from the title screen ([via the cog button](https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/docs/screenshot-title.png)) or in-game ([at the bottom of the in-game options menu](https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/docs/screenshot-in-game-options.png)).

### Manual Config
Here's a [list of valid options](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings) for the hotkey options, for your keyboard or controller.

| Config Variables | Valid Inputs | Description |
|---|:---:|---|
|`ActivateKey`|[Any key/button listed here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings)|(Default: `P`) Pressing this key in-game opens the Calendar.|
|`CycleRightKey`|[Any key/button listed here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings)|(Default: `]`) After pressing `ActivateKey`, use this key to cycle to the next menu.|
|`CycleLeftKey`|[Any key/button listed here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings)|(Default: `[`) After pressing `ActivateKey`, use this key to cycle to the previous menu. **If you only want one cycle key: Set this to the same value as CycleRightKey**|
|`SpecialOrdersBeforeUnlocked`, `QiBeforeUnlocked`|`true`, `false`|(Default: `false`) If `true`, you can access corresponding menu before unlocking it.|
|`enableCalendar`, `enableDailyQuests`, `enableSpecialOrders`, `enableQiSpecialOrders`, `enableJournal`|`true`, `false`| If `false`, the corresponding menu will not be in the cycle.|

---

## Pelican Fiber
This is just an updated version of the original Pelican Fiber mod created by PathosChild. I loved this mod a few years back when it was being maintained, so I decided to try updating it! I didn't have to change much, I just made it work on 1.5. I might try adding the new Ginger Island shops to the menu, not sure yet.
 - [Original Github Repo](https://github.com/jdusbabek/stardewvalley)
 - [Original NexusMods Listing](https://www.nexusmods.com/stardewvalley/mods/631)

### Install
- Install the latest version of SMAPI.
- Download the zip from the latest [release](https://github.com/AcidicNic/StardewValleyMods/releases/tag/latest) page.
- Extract the zip file into your Mods folder.
    - You can find the Mods folder using steam by right clicking Stardew Valley, then clicking Properties > Local Files > Browse

### Config
Here's a [list of valid options](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings) for the `KeyBind` config var.

I haven't changed any config settings, so check out [the configuration section on the original NexusMods page](https://www.nexusmods.com/stardewvalley/mods/631?tab=description).
