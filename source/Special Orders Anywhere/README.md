**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/AcidicNic/StardewValleyMods**

----

# Stardew Valley Mods

## [Menu Cycle](https://www.nexusmods.com/stardewvalley/mods/7588) (Name changed. Previously: Special Orders Anywhere)

Quickly cycle through varies menus, anywhere, using 1 to 3 keybinds.

... <—> Calendar <—> Daily Quests <—> Special Orders <—> Qi's Special Orders <—> Journal (Disabled by default) <—> ...

* Open/close the first menu in the cycle: **P**
* Next Menu: **]**
* Previous Menu: **[**

While there are options for a cycle right & left keybind, you can disabled one if you'd like.

If you enable the journal, you can open your journal like normal and cycle left/right from there without using the ActivateKey binding.

There are options to access both the Special Orders Board and Qi's Special Orders Board before they are unlocked. Both disabled by default.

### 🚨 *If you have installed Special Orders Anywhere*🚨

Delete the `SpecialOrdersAnywhere` from your `Mods` folder or both mods will be disabled.

### Install

* Install the latest version of SMAPI.
* Download the zip from [NexusMods](https://www.nexusmods.com/stardewvalley/mods/7588).
* Extract the zip file into your Mods folder.
  * You can find the Mods folder using steam by right clicking Stardew Valley, then clicking Properties > Local Files > Browse

### Generic Mod Config Menu (Recommended)

1. Install the [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) mod from NexusMods.
2. You can edit settings from the title screen ([via the cog button](https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/docs/screenshot-title.png)) or in-game ([at the bottom of the in-game options menu](https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/docs/screenshot-in-game-options.png)).

### Manual Config

Here's a [list of valid keyboard and controller inputs](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings) for the hotkey options.

You can [combine keys](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Multi-key_bindings) to create multi-key bindings.

| Config Variables | Valid Inputs | Description |
|---|:---:|---|
|`ActivateKey`|[Any key/button listed here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings)|(Default: `P`) Pressing this key in-game opens the first enabled menu in the cycle.|
|`CycleRightKey`|[Any key/button listed here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings)|(Default: `]`) After pressing `ActivateKey`, use this key to cycle to the next menu.|
|`CycleLeftKey`|[Any key/button listed here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings)|(Default: `[`) After pressing `ActivateKey`, use this key to cycle to the previous menu. **If you only want one cycle key: Set this to the same value as CycleRightKey**|
|`SpecialOrdersBeforeUnlocked`, `QiBeforeUnlocked`|`true`, `false`|(Default: `false`) If `true`, you can access corresponding menu before unlocking it.|
|`enableCalendar`, `enableDailyQuests`, `enableSpecialOrders`, `enableQiSpecialOrders`, `enableJournal`|`true`, `false`| If `false`, the corresponding menu will not be in the cycle.|

---

## [Monster Slayer Anywhere](https://www.nexusmods.com/stardewvalley/mods/21162)

View your Monster Eradication Goal List anywhere. 

Open/close Monster Eradication Goal List: **F7**

### Install

* Install the latest version of SMAPI.
* Download the latest version from [NexusMods](https://www.nexusmods.com/stardewvalley/mods/21162).
* Extract the zip file into your Mods folder.
  * You can find the Mods folder using steam by right clicking Stardew Valley, then clicking Properties > Local Files > Browse

### Generic Mod Config Menu (Recommended)

1. Install the [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) mod from NexusMods.
2. You can edit settings from the title screen ([via the cog button](https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/docs/screenshot-title.png)) or in-game ([at the bottom of the in-game options menu](https://github.com/spacechase0/StardewValleyMods/blob/develop/GenericModConfigMenu/docs/screenshot-in-game-options.png)).

### Manual Config

A config file called `config.json` will generate inside of `StardewValley/Mods/MonsterSlayerAnywhere/` after installing the mod and running the game once. Open `config.json` and change `ToggleMonsterList` to your desired key.

Here's a [list of valid keyboard and controller inputs](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings) for the hotkey option.

You can [combine keys](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Multi-key_bindings) to create multi-key bindings.

| Config Variables | Valid Inputs | Description |
|---|:---:|---|
|`ToggleMonsterList`|[Any key(s)/button(s) listed here](https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings)|(Default: `F7`) Opens the Monster Eradication Goal List.|

### ChangeLog

### 1.1.0

    + Keybind now toggles the monster eradication goal list, instead of only opening it.
    + Multi-key binding added.

### 1.0.0

    + Added a hotkey that opens the monster eradication goal list.
    + Generic Mod Config Menu is supported.
