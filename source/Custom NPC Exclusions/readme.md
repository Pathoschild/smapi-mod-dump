**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Esca-MMC/CustomNPCExclusions**

----

# Custom NPC Exclusions
 A mod for the game Stardew Valley that allows mods to exclude NPCs from certain quests and events, especially those that randomly select NPCs.

## Contents
* [Installation](#installation)
* [Exclusion Rules](#exclusion-rules)
* [Mod Examples](#mod-examples)
     * [SMAPI Mods](#smapi-mods)
     * [Content Patcher Mods](#content-patcher-mods)
* [Credits](#credits)

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Download Custom NPC Exclusions** from [the Releases page on GitHub](https://github.com/Esca-MMC/CustomNPCExclusions/releases), Nexus Mods, or ModDrop.
3. **Unzip Custom NPC Exclusions** into the `Stardew Valley\Mods` folder.

Mods that use Custom NPC Exclusions should now work correctly. For information about creating mods, see the sections below.

Multiplayer note:
* It is recommended that **all players** install this mod for multiplayer sessions. There are no known issues directly related to this mod, but NPC mods can cause unexpected errors if mismatched.

## Exclusion Rules
This mod can apply different exclusion rules to each individual NPC, preventing them being involved in the related quests and events.

The available exclusion rules are:

Rule | Category | Description
-----|----------|------------
All | | Excludes the NPC from all content affected by this mod.
TownEvent | | Excludes the NPC from all content in the "TownEvent" category (see below).
TownQuest | | Excludes the NPC from all content in the "TownQuest" category (see below).
IslandEvent | | Excludes the NPC from all content in the "IslandEvent" category (see below).
OtherEvent | | Excludes the NPC from all content in the "OtherEvent" category (see below).
ShopDialog | TownEvent | Excludes the NPC from randomly discussing items that players have sold to certain shops.
WinterStar | TownEvent | Excludes the NPC from giving or receiving secret gifts at the Feast of the Winter Star festival.
ItemDelivery | TownQuest | Excludes the NPC from randomized "item delivery" quests on the Help Wanted board.
Socialize | TownQuest | Excludes the NPC from "socialize" quests, which currently includes the "Introductions" quest at the start of the game.
IslandVisit | IslandEvent | Excludes the NPC from random visits to the Ginger Island resort.
PerfectFriend | OtherEvent | Excludes the NPC from Stardew's "perfection score" tracker when it checks for maximum friendship with NPCs.

## Mod Examples
This mod loads a new data asset into Stardew: `Data/CustomNPCExclusions`.

The asset is similar to other "Data" types and is a `Dictionary<string, string>` internally. Each entry should have a NPC's name as the *key*, and a set of exclusion rules for that NPC as the *value*. Exclusion rules can be separated by spaces, commas, or forward slashes.

The asset can be edited in multiple ways; see the sections below for example of specific methods.

### SMAPI Mods
SMAPI mods can edit NPC exclusion data by using the `IAssetEditor` interface. See this wiki page for an overview: [https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Content#Edit_a_game_asset](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Content#Edit_a_game_asset)

Below is a more specific example, which adds the "WinterStar" and "ItemDelivery" rules to a custom NPC named "MyCustomNpcName":

```cs
public bool CanEdit<T>(IAssetInfo asset)
{
	if (asset.AssetNameEquals("Data/CustomNPCExclusions"))
		return true;
		
	return false;
}

public void Edit<T>(IAssetData asset)
{
	if (asset.AssetNameEquals("Data/CustomNPCExclusions"))
	{
		IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
		data.Add("MyCustomNpcName", "WinterStar ItemDelivery"); 
	}
}
```

### Content Patcher Mods
Content Patcher's content packs can edit NPC exclusion data by using `"Action": "EditData"` with `"Target": "Data/CustomNPCExclusions"`. See the Content Patcher author guide for an overview: [https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md#editdata](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md#editdata)

Below is a more specific example, which adds the "WinterStar" and "ItemDelivery" rules to a custom NPC named "MyCustomNpcName":

```js
{
   "Format": "1.18.0",
   "Changes": [
      {
         "Action": "EditData",
         "Target": "Data/CustomNPCExclusions",
         "Entries": {
            "MyCustomNpcName": "WinterStar ItemDelivery"
         }
      }
   ]
}
```

## Credits
This mod was commissioned by FlashShifter for use with [Stardew Valley Expanded](https://www.moddrop.com/stardew-valley/mods/833179-stardew-valley-expanded)’s custom NPCs.
