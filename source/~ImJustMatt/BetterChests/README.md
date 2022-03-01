**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ImJustMatt/StardewMods**

----

# Better Chests

Adds enhanced chest features to the game.

## Contents

* [Features](#features)
    * [Auto Organize](#auto-organize)
    * [Carry Chest](#carry-chest)
    * [Categorize Chest](#categorize-chest)
    * [Chest Menu Tabs](#chest-menu-tabs)
    * [Collect Items](#collect-items)
    * [Craft From Chest](#craft-from-chest)
    * [Custom Color Picker](#custom-color-picker)
    * [Filter Items](#filter-items)
    * [Open Held Chest](#open-held-chest)
    * [Organize Chest](#organize-chest)
    * [Resize Chest](#resize-chest)
    * [Resize Chest Menu](#resize-chest-menu)
    * [Search Items](#search-items)
    * [Slot Lock](#slot-lock)
    * [Stash To Chest](#stash-to-chest)
    * [Unload Chest](#unload-chest)
* [Usage](#usage)
    * [Item Tags](#item-tags)
* [Configurations](#configurations)
    * [Config Inheritance](#config-inheritance)
    * [Group By Values](#group-by-values)
    * [Option Values](#option-values)
    * [Range Values](#range-values)
    * [Sort By Values](#sort-by-values)
* [Mod Integrations](#mod-integrations)
    * [Automate](#automate)
    * [Horse Overhaul](#horse-overhaul)
* [Customization](#customization)
    * [Custom Chests](#custom-chests)
    * [Customized Tabs](#customized-tabs)
    * [Other Assets](#other-assets)
* [Translations](#translations)

## Features

### Auto Organize

Every night, after going to bed, your chests will automatically organize items based on the same rules
as [Stash to Chest](#stash-to-chest). Items will only move from/to chests with the feature enabled, and only if the
other chest has a higher priority.

It also applies your [Organize Chest](#organize-chest) rules at the end.

| Config          | Description                         | Default Value | Other Value(s)                                                       |
|:----------------|:------------------------------------|:--------------|:---------------------------------------------------------------------|
| AutoOrganize    | Enables the  Auto Organize feature. | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>1</sup>                                |

### Carry Chest

With Carry Chest enabled, you can hit the Use Tool button to pick up chests into your inventory even if it has items.

| Config          | Description                             | Default Value | Other Value(s)                                                       |
|:----------------|:----------------------------------------|:--------------|:---------------------------------------------------------------------|
| CarryChest      | Enables the Carry Chest feature.        | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>1</sup>                                |
| CarryChestLimit | Limits how many chests can be carried.  | `0`           | Any positive integer.<sup>2</sup>                                    |
| CarryChestSlow  | Slowness debuff while carrying a chest. | `0`           | Positive integer from `0` (normal speed) to `4` (slime speed debuff) |

1. See [Option Values](#option-values).
2. Use `0` for unlimited chests.

### Categorize Chest

Categorize Chest allows you to assign item categories to chests so that only those items can be stashed into that chest.

| Config Option   | Description                           | Default Value | Other Value(s)                        |
|:----------------|:--------------------------------------|:--------------|:--------------------------------------|
| CategorizeChest | Enables the Categorize Chest feature. | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>1</sup> |

1. See [Option Values](#option-values).

### Chest Menu Tabs

Tabs are added to the Chest Menu to allow you to quickly toggle between categories of items.<sup>1</sup>

| Config          | Description                                    | Default Value    | Other Value(s)                        |
|:----------------|:-----------------------------------------------|------------------|:--------------------------------------|
| ChestMenuTabs   | Enables the Chest Menu Tabs feature.           | `"Enabled"`      | `"Disabled"`, `"Default"`<sup>2</sup> |
| ChestMenuTabSet | Assigns what tabs to show/hide.                | `[]`<sup>3</sup> | The names of tabs to show.            |
| NextTab         | Assigns a keybind to move to the next tab.     | `"DPadRight"`    | Any valid button code.<sup>4</sup>    |
| PreviousTab     | Assigns a keybind to move to the previous tab. | `"DPadLeft"`     | Any valid button code.<sup>4</sup>    |

1. See [Customized Tabs](#customized-tabs).
2. See [Option Values](#option-values).
3. An empty string array shows all available tabs.
4. See [Button Codes](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Button_codes).

### Collect Items

While you are carrying a chest in your inventory, any items you pick up may be collected directly into the chest,
bypassing your inventory.<sup>1</sup>

| Config       | Description                        | Default Value | Other Value(s)                        |
|:-------------|:-----------------------------------|:--------------|:--------------------------------------|
| CollectItems | Enables the Collect Items feature. | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>2</sup> |

1. Only items assigned from [Categorize Chest](#categorize-chest) will be collected.
2. See [Option Values](#option-values).

### Craft From Chest

Hit a configurable key to bring up a crafting menu that uses items stored in nearby chests.

| Config                         | Description                                               | Default Value | Other Value(s)                                                                 |
|:-------------------------------|:----------------------------------------------------------|:--------------|:-------------------------------------------------------------------------------|
| CraftFromChest                 | Enables the Craft From Chest feature.                     | `"Location"`  | `"Disabled"`, `"Default"`, `"Inventory"`, `"Location"`, `"World"` <sup>1</sup> |
| OpenCrafting                   | Assigns the keybind for opening the crafting menu.        | `"K"`         | Any valid button code.<sup>2</sup>                                             |
| CraftFromChestDisableLocations | A list of locations that crafting will not be allowed in. | `[]`          | The locations to block.<sup>3</sup>                                            |
| CraftFromChestDistance         | Limits the distance that a chest can be crafted from.     | -1            | Any positive integer or `-1`.<sup>4</sup>                                      |

1. See [Range Values](#range-values).
2. See [Button Codes](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Button_codes).
3. Add `"UndergroundMine"` to the list to disable in Mine and Skull Cavern.
4. Measured in tiles away from the player. Use `-1` for "unlimited" distance.

### Custom Color Picker

Replaces the Chest Color Picker with a more precise version that lets you pick a color with sliders for hue, saturation,
and lightness.

| Config                | Description                                   | Default Value | Other Value(s)                        |
|:----------------------|:----------------------------------------------|:--------------|:--------------------------------------|
| CustomColorPicker     | Enables the Custom Color Picker feature.      | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>1</sup> |
| CustomColorPickerArea | Which side will the Color Picker be added to. | `"Right"`     | `"Left"`                              |

1. See [Option Values](#option-values).

### Filter Items

Impose restrictions on what types of items are allowed to go into a chest. With this enabled, items that are not part of
the allowed list will be blocked.

| Config          | Description                                          | Default Value | Other Value(s)                        |
|:----------------|:-----------------------------------------------------|:--------------|:--------------------------------------|
| FilterItems     | Enables the Filter Items feature.                    | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>1</sup> |
| FilterItemsList | A list of context tags used to select allowed items. | `[]`          | The tags to allow.<sup>2</sup>        |

1. See [Option Values](#option-values).
2. See [Item Tags](#item-tags).

### Open Held Chest

With a chest as your active item, you can hit the action button to bring up the chest menu and access the chests
contents.

| Config        | Description                          | Default Value | Other Value(s)                        |
|:--------------|:-------------------------------------|:--------------|:--------------------------------------|
| OpenHeldChest | Enables the Open Held Chest feature. | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>1</sup> |

1. See [Option Values](#option-values).

### Organize Chest

Organize Chest allows you to group and sort items by a configurable property of those items.

| Config Option        | Description                         | Default Value | Other Value(s)                           |
|:---------------------|:------------------------------------|:--------------|:-----------------------------------------|
| OrganizeChest        | Enables the Organize Chest feature. | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>1</sup>    |
| OrganizeChestGroupBy | What will items be grouped by.      | `"Default"`   | See [Group By Values](#group-by-values). |
| OrganizeChestSortBy  | What will items be sorted by.       | `"Default"`   | See [Sort By Values](#sort-by-values).   |

1. See [Option Values](#option-values).

### Resize Chest

The default storage for a chest is 36 items. With Resize Chest enabled you can increase storage space up to virtually
unlimited storage.<sup>1</sup>

| Config              | Description                              | Default Value | Other Value(s)                            |
|:--------------------|:-----------------------------------------|:--------------|:------------------------------------------|
| ResizeChest         | Enables the Resize Chest feature.        | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>2</sup>     |
| ResizeChestCapacity | The number of items the chest can store. | `60`          | Any positive integer or `-1`.<sup>3</sup> |
| ScrollDown          | Assigns a keybind to scroll down.        | `"DPadDown"`  | Any valid button code.<sup>4</sup>        |
| ScrollUp            | Assigns a keybind to scroll up.          | `"DPadUp"`    | Any valid button code.<sup>4</sup>        |

1. If the number of items exceeds the menu space, you can scroll to access the overflow.
2. See [Option Values](#option-values).
3. Use `-1` for "unlimited" items or a positive multiple of 12 for limited storage.
4. See [Button Codes](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Button_codes).

### Resize Chest Menu

The default chest menu can show 3 rows of items at once. With Resize Chest Menu enabled you can display anywhere between
3 and 6 rows of items.

| Config              | Description                            | Default Value | Other Value(s)                        |
|:--------------------|:---------------------------------------|:--------------|:--------------------------------------|
| ResizeChestMenu     | Enables the Resize Chest Menu feature. | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>1</sup> |
| ResizeChestMenuRows | The number of rows to show.            | `5`           | `3`, `4`, `5`, `6`                    |

1. See [Option Values](#option-values).

### Search Items

Adds a search bar to the top of the Chest Menu to only display items that meet a search criteria.

| Config          | Description                                              | Default Value | Other Value(s)                        |
|:----------------|:---------------------------------------------------------|:--------------|:--------------------------------------|
| SearchItems     | Enables the Search Items feature.                        | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>1</sup> |
| SearchTagSymbol | Denotes a prefix for searching by item tags.<sup>2</sup> | `"#"`         | Any single character symbol.          |

1. See [Option Values](#option-values).
2. See [Item Tags](#item-tags).

### Slot Lock

Hover over an item slot in your backpack, and hit a configurable key to lock the item in its slot which prevents it from
being stashed into a chest.

| Config   | Description                             | Default Value | Other Value(s)                        |
|:---------|:----------------------------------------|:--------------|:--------------------------------------|
| SlotLock | Enables the Slot Lock feature.          | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>1</sup> |
| LockSlot | Assigns the keybind for locking a slot. | `"A"`         | Any valid button code.<sup>2</sup>    |

1. See [Option Values](#option-values).
2. See [Button Codes](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Button_codes).

### Stash To Chest

Hit a configurable key to instantly stash items from your inventory into nearby chests.<sup>1</sup>

| Config Option                | Description                                               | Default Value | Other Value(s)                                                                 |
|:-----------------------------|:----------------------------------------------------------|:--------------|:-------------------------------------------------------------------------------|
| StashToChest                 | Enables the Stash To Chest feature.                       | `"Location"`  | `"Disabled"`, `"Default"`, `"Inventory"`, `"Location"`, `"World"` <sup>2</sup> |
| StashItems                   | Assigns the keybind for stashing items.                   | `"Z"`         | Any valid button code.<sup>3</sup>                                             |
| StashToChestDisableLocations | A list of locations that stashing will not be allowed in. | `[]`          | The locations to block.<sup>4</sup>                                            |
| StashToChestDistance         | Limits the distance that a chest can be stashed into.     | -1            | Any positive integer or `-1`.<sup>5</sup>                                      |
| StashToChestPriority         | Prioritize certain chests over others.                    | 0             | Any integer value.                                                             |

1. Included chests are determined by config options.
2. See [Range Values](#range-values).
3. See [Button Codes](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Button_codes).
4. Add `"UndergroundMine"` to the list to disable in Mine and Skull Cavern.
5. Measured in tiles away from the player. Use `-1` for "unlimited" distance.

### Unload Chest

While carrying a chest and facing another chest, hit the Use Tool button to unload the carried chests contents into the
placed chest.

| Config Option | Description                       | Default Value | Other Value(s)                        |
|:--------------|:----------------------------------|:--------------|:--------------------------------------|
| UnloadChest   | Enables the Unload Chest feature. | `"Enabled"`   | `"Disabled"`, `"Default"`<sup>1</sup> |

1. See [Option Values](#option-values).

## Usage

### Item Tags

The game adds various [Context Tags](https://stardewcommunitywiki.com/Modding:Context_tags)
to each item which are used throughout this mod.

There are a few ways to see what context tags each item contains:

* Enter the console command `debug listtags` to show all tags for the currently held item.
* Refer to the [Modding Docs](https://stardewcommunitywiki.com/Modding:Context_tags) for some tags (note may be
  outdated).
* Install [Lookup Anything](https://www.nexusmods.com/stardewvalley/mods/541),
  enable [ShowDataMiningField](https://github.com/Pathoschild/StardewMods/tree/stable/LookupAnything#configure) in its
  config and hit F1 while hovering over any item.

Here are examples of some useful tags:

| Description | Tags                                                        |
|-------------|-------------------------------------------------------------|
| Category    | `category_clothing`, `category_boots`, `category_hats`, ... |
| Color       | `color_red`, `color_blue`, ...                              |
| Name        | `item_sap`, `item_wood`, ...                                |
| Type        | `wood_item`, `trash_item`, ...                              |
| Quality     | `quality_none`, `quality_gold`, ...                         |
| Season      | `season_spring`, `season_fall`, ...                         |
| Index       | `id_o_709`, `id_r_529`, ...                                 |

In addition to the vanilla game tags, this also adds some custom tags to items:

| Tag                  | Description                                                                                  |
|----------------------|----------------------------------------------------------------------------------------------|
| `category_artifact`  | Selects any item that is an Artifact.                                                        |
| `category_furniture` | Includes regular furniture items with this tag even though the game doesn't normally add it. |
| `donate_bundle`      | Selects any items that are missing from the Community Center Bundle.                         |
| `donate_museum`      | Selects any items that can be donated to the Museum.                                         |

## Configurations

For ease of use, it is recommended to set config options
from [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).

### Config Inheritance

For some config options, the same options can be applied to multiple levels. The lowest level config will always take
precedence.

* **Default Chest** - Every chest inherits from the Default Chest options.
* **Chest Type** - Applies to a particular type of chest.
    * Chest
    * Stone Chest
    * Junimo Chest
    * Auto-Grabber
    * Mini-Fridge
    * Mini-Shipping Bin
    * Fridge
    * Custom chests added by other mods.<sup>1</sup>
* **Individual Chest** - A single instance of a chest can be configured individually.

1. External mod must "opt-in" by [registering custom chest](#custom-chests) with Better Chests.

### Group By Values

Group by is a text property of the item that organize will order by first.

* **Default** - Group by the original organize method.
* **Category** - Group by the item category.
* **Color** - Group by the item color (only if context tag supports it).
* **Name** - Group by the item name.

### Option Values

The option value determines whether a feature will be enabled or disabled for a chest.

* **Default** - The value will be inherited from a parent config.<sup>1</sup>
* **Disabled** - The feature will be disabled.
* **Enabled** - The feature will be enabled.

1. If parent value is unspecified, Enabled will be the default value.

### Range Values

The Range value limits which chests will be selected for a feature relative to the player.

* **Default** - The value will be inherited from a parent config.<sup>1</sup>
* **Disabled** - The feature will be disabled.
* **Inventory** - Only chests in the player inventory.
* **Location** - Only chests in the players current location.
* **World** - Any chest accessible to the player in the world.

1. If parent value is unspecified, Location will be the default value.

### Sort By Values

Sort by is a numerical property of the item that organize will order by second.

* **Default** - Sort by the original organize method.
* **Type** - Sort by the numerical category type of the item.
* **Quality** - Sort by the quality of the item (if applicable).
* **Quantity** - Sort by the stack size of the item.

## Mod Integrations

### Automate

When [Filter Items](#filter-items) is enabled, then any categorizations that a chest has will be applied to
[Automate](https://www.nexusmods.com/stardewvalley/mods/1063). This means that Automate will be blocked from adding
items into the chest if the Filter list does not allow it.

### Better Crafting

Craft from Chest will launch a Better Crafting Page and it will correctly include all chests with the feature enabled.

### Horse Overhaul

Better Chests automatically integrates with [Horse Overhaul](https://www.nexusmods.com/stardewvalley/mods/7911)
saddlebags. The distance to the player's Horse will be considered for features such
as [Craft from Chest](#craft-from-chest) and
[Stash to Chest](#stash-to-chest).

The SaddleBag can have its own Better Chest config by adding an entry for a chest named `"SaddleBag"` to the
`BetterChests/assets/chests.json` file.

## Customization

### Custom Chests

The config for chest types are stored in the `chests.json` in the `assets`
folder.

If `chests.json` is not found, a default one for vanilla chests is automatically generated with all default settings.

For mods adding custom chests, there are two ways to register them with Better Chests:

#### API

Register your chest using the [Better Chests API](../Common/Integrations/BetterChests/IBetterChestsApi.cs).

#### Data Path

`furyx639.BetterChests\\Chests`  
Add/replace chest settings by editing entries<sup>1</sup>.

Sample `content.json`:

```jsonc
{
  "Format": "1.24.0",
  "Changes": [
    {
      "Action": "EditData",
      "Target": "furyx639.BetterChests\\Chests",
      "Entries": {
        "example.ModId_Chest": {
          "CarryChest": "Enabled",
          "FilterItems": "Enabled",
          "ResizeChest": "Enabled",
          "ResizeChestCapacity": "48"
        }
      }
    }
  ]
}
```

1. See
   the [Edit Data](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/action-editdata.md)
   docs for Content Patcher.

### Customized Tabs

The default tabs are defined by the `tabs.json` and `tabs.png` files which are both found under the `assets` folder.

If `tabs.json` is not found, a default one is automatically generated:

| Key<sup>1</sup> | Display Name | Texture                                | Icon Index | Item Tags<sup>2</sup>                                                                                        |
|:----------------|:-------------|:---------------------------------------|:-----------|:-------------------------------------------------------------------------------------------------------------|
| Clothing        | Clothing     | `furyx639.BetterChests\\Tabs\\Texture` | 0          | Boots, Clothing, Hat                                                                                         |
| Cooking         | Cooking      | `furyx639.BetterChests\\Tabs\\Texture` | 1          | Artisan Goods, Cooking, Egg, Ingredients, Meat, Milk, Sell at Pierre's and Marnie's, Sell at Pierre's, Syrup |
| Crops           | Crops        | `furyx639.BetterChests\\Tabs\\Texture` | 2          | Flowers, Fruits, Greens, Vegetables                                                                          |
| Equipment       | Equipment    | `furyx639.BetterChests\\Tabs\\Texture` | 3          | Equipment, Ring, Tool, Weapon                                                                                |
| Fishing         | Fishing      | `furyx639.BetterChests\\Tabs\\Texture` | 4          | Bait, Fishing, Sell at Fish Shop, Tackle                                                                     | 
| Materials       | Materials    | `furyx639.BetterChests\\Tabs\\Texture` | 5          | Building Resources, Crafting, Gem, Metal Resources, Minerals, Monster Loot                                   |
| Misc            | Misc         | `furyx639.BetterChests\\Tabs\\Texture` | 6          | Big Craftable, Furniture, Junk                                                                               | 
| Seeds           | Seeds        | `furyx639.BetterChests\\Tabs\\Texture` | 7          | Fertilizer, Seeds                                                                                            |

You can edit these files directly for personal usage, or they may be targeted externally for edits from other mods at
the following paths<sup>3</sup>:

`furyx639.BetterChests\\Tabs`

Sample `content.json`:

```jsonc
{
  "Format": "1.24.0",
  "Changes": [
    // Customize Tabs
    {
      "Action": "EditData",
      "Target": "furyx639.BetterChests\\Tabs",
      "Entries": {
        // Edit an existing tab
        "Misc": "/furyx639.BetterChests\\Tabs\\Texture/6/category_big_craftable category_furniture category_junk forage_item"
        
        // Add a new tab
        "Community Center": "{{i18n: tab.community-center.name}}/example.ModId\\TabTexture/0/category_donate"
      }
    },
    
    // Load texture for new tab
    {
      "Action": "Load",
      "Target": "example.ModId/TabTexture",
      "FromFile": "assets/{{TargetWithoutPath}}j.png",
    },
    

  ]
}
```

1. The key value is used in the [ChestMenuTabSet](#chest-menu-tabs) config option.
2. See [Item Tags](#item-tags)
3. See
   the [Edit Data](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/action-editdata.md)
   docs for Content Patcher.

### Other Assets

#### Icons

Replace any or all of the icons for the Configure, Craft from Chest, and Stash to Chest buttons by editing the
image<sup>1</sup>:

`furyx639.BetterChests\\Icons`.

Sample `content.json`:

```jsonc
{
  "Format": "1.24.0",
  "Changes": [
    {
      "Action": "EditImage",
      "Target": "furyx639.BetterChests\\Icons",
      "FromFile": "assets/MyConfigureButton.png",
      "FromArea": {"X": 0, "Y": 0, "Width": 16, "Height": 16},
      "ToArea": {"X": 0, "Y": 0, "Width": 16, "Height": 16}
    },
  ]
}
```

1. See
   the [Edit Image](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/action-editimage.md)
   docs for Content Patcher.

#### Tab Texture

Replace any or all of the default tab textures by editing the image<sup>1</sup>:

`furyx639.BetterChests\\Tabs\\Textures`

Sample `content.json`:

```jsonc
{
  "Format": "1.24.0",
  "Changes": [
    // Replace texture for mining icon
    {
      "Action": "EditImage",
      "Target": "furyx639.BetterChests\\Tabs\\Texture",
      "FromFile": "assets/mining-icon.png",
      "FromArea": {"X": 0, "Y": 0, "Width": 16, "Height": 16},
      "ToArea" {"X": 48, "Y": 0, "Width": 16, "Height": 16}
    }
  ]
}
```

1. See
   the [Edit Image](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/action-editimage.md)
   docs for Content Patcher.

## Translations

See [here](i18n/default.json) for the base translation file.

| Language                   | Status            | Credits     |
|:---------------------------|:------------------|:------------|
| Chinese                    | ❌️ Not Translated |             |
| [French](i18n/fr.json)     | ❔ Incomplete      | Ayatus      |
| [German](i18n/de.json)     | ❔ Incomplete      | Loni4ever   |
| [Hungarian](i18n/hu.json)  | ❔ Incomplete      | martin66789 |
| [Italian](i18n/it.json)    | ✔️ Complete       | zomboide    |
| Japanese                   | ❌️ Not Translated |             |
| [Korean](i18n/ko.json)     | ✔️ Complete       | wally232    |
| [Portuguese](i18n/pt.json) | ✔️ Complete       | Aulberon    |
| [Russian](i18n/ru.json)    | ❔ Incomplete      | Newrotd     |
| [Spanish](i18n/es.json)    | ❔ Incomplete      | Soraien     |
| Turkish                    | ❌️ Not Translated |             |
