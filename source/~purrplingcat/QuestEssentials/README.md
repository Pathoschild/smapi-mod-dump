**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/StardewMods**

----

# Quest Essentials

Adds new quest types and some extra stuff for quests for Quest Framework

## Installation

1. Install [SMAPI](https://smapi.io) (version 3.7 or newer)
2. Install [Quest Framework](https://www.nexusmods.com/stardewvalley/mods/6414) (version 1.4 or newer)
3. Download Quest Essentials from Nexus mods
4. Unpack ZIP file into `Mods` directory
5. Run Stardew Valley via SMAPI

Now you can install mods or content packs which requires **Quest Essentials** or you can start create your own. See docs bellow.

## New quest types

Quest type | UID | Trigger format | Description
---- | ---- | ---- | ----
EarnMoney | `PurrplingCat.QuestEssentials/EarnMoney` | `<int:MoneyEarningsGoal>` | Earn a specified goal of money.
SellItem | `PurrplingCat.QuestEssentials/SellItem` | `<int:WhichItem> [<int:Amount>]` | Sell a specified item of specified amount. If the amount is not defined in trigger, then amount is 1.
Talk | `PurrplingCat.QuestEssentials/Talk` | `<string:NPC_name>` | Talk with an NPC
Collect | `PurrplingCat.QuestEssentials/Collect` | `<int:itemId> [<int:count>]` | Collect items (alternative to harvest quest type)
SpecialQuest | `PurrplingCat.QuestEssentials/Special` | *none* | Multi-staged quest. See [Special Quests](docs/special-quests.md) docs for more details.

## Use Quest Essentials in your content pack

Create your `manifest.json` file

```js
{
  "Name": "<Your pack name>",
  "Author": "<Your name>",
  "Version": "1.0.0",
  "Description": "<Your pack description>",
  "UniqueID": "<your name>.<your pack name>",
  "MinimumApiVersion": "3.7.0",
  "UpdateKeys": [],
  "ContentPackFor": {
    "UniqueID": "PurrplingCat.QuestFramework",
    "MinimumVersion": "1.4.0"
  },
  "Dependencies": [
    {
      "UniqueID": "PurrplingCat.QuestEssentials",
    }
  ]
}
```

Create file `quests.json` (see [Quest Framework docs](https://questframework.purrplingcat.com/docs/content-pack-guide.html))

And now you can define your quests using with custom quest types exposed by **Quest Essentials**. Here is an example:

```js
{
  "Format": "1.0",
  "Quests": [
    {
      "Name": "TestSellQuest",
      "Type": "PurrplingCat.QuestEssentials/SellItem",
      "Title": "Sell some parsnips",
      "Description": "Sell 10 pieces of parsnip in shipping bin or in any shop in the town.",
      "Objective": "Sell 10 pieces of parsnip",
      "Reward": 1500,
      "Cancelable": true,
      "Trigger": "24 10"
    },
    {
      "Name": "TestEarnQuest",
      "Type": "PurrplingCat.QuestEssentials/EarnMoney",
      "Title": "Grow up!",
      "Description": "Earn lot of money! Earn a package of 10000g.",
      "Reward": 1500,
      "Cancelable": true,
      "Trigger": "10000"
    },
    {
      "Name": "TestTalkQuest",
      "Type": "PurrplingCat.QuestEssentials/Talk",
      "Title": "Talk with Abby",
      "Description": "Abigail wants talk with you.",
      "Objective": "Talk to Abigail",
      "Cancelable": true,
      "Trigger": "Abigail"
    },
  ],
  "Offers": [
    // Define your quest offers here
  ]
}
```

## Authors

- [PurrplingCat](https://www.nexusmods.com/users/68185132) (Main author of Quest Essentials and Quest Framework)

## Support

- [Discord](https://discord.gg/wnEDqKF)
- [Nexusmods page](https://www.nexusmods.com/stardewvalley/mods/????)
- [Stardew Modding API (SMAPI)](https://smapi.io)
- [PurrplingCat's Patreon](https://www.patreon.com/purrplingcat)

---

This mod was made with love :heart: by PurrplingCat
