**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments**

----

Ginger-Island Scheduling
=======================

[← back to readme](../../README.md) <br/>
[← back to main guide](./README.MD)

If the setting `UseThisScheduler` is enabled, this mod will try to schedule NPCs at Ginger Island using the below data:

### Custom Groups

Vanilla NPCs may go to Ginger Island in groups. They're typically family groups, although all the bachelors and all the bachelorettes are a possible group. A full list of groups in this mod can be found here [here](https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments/blob/master/Ginger%20Island%20Mainland%20Adjustments/assets/defaultGroupings.json), but more can be added by adding them to `Mods/atravita_Ginger_Island_Adjustments_groups`. For example:

```js
{
	"Format": "1.24.0",
	"Changes": [
		{
			"Action": "EditData",
			"Target": "Mods/atravita_Ginger_Island_Adjustments_groups",
			"Entries": {
				"GoldenTrio": "Harry, Hermione, Ron"
			}
		}
	]
}
```

Groups will sometimes be picked to attend the Resort together.

Note: the keys are global, so prepending them with your mod's uniqueID may help reduce conflicts. You can also use [Content Patcher's `TextOperations`](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md#text-operations) to append here.

#### Explorer Groups

Some NPCs may want to explore the rest of the island. 

```js
    {
        "LogName": "Explorers",
        "Action": "EditData",
        "Target": "Mods/atravita_Ginger_Island_Adjustments_explorers", // Explorers go to IslandNorth and either the tent or the volcano
        "Entries": {
            "ScienceGroup": "Maru, Demetrius, Robin",
            "BandKids": "Abigail, Sebastian, Sam",
            "ArtsyGroup": "Leah, Elliott"
        }
    }
```

would add three explorer groups. Explorer groups will make either a stop at the volcano or a stop by Professor Snail's. They'll use the following dialogue keys:

* `Resort_IslandNorth`: Said whenever they're on IslandNorth and moving from point to point.
* `Resort_Adventure`: Said while they're standing just south of the Island Trader, preparing to head north.
* `Resort_Tent`: Said while they're standing next to Professor Snail's tent.
* `Resort_Volcano`: Said while they're standing next to the volcano.
* `Resort_AdventureReturn`: Said while they're standing south of the Island Trader, ready to head home.

You can append numbers to the end of these keys as well, in which case one key will be picked randomly - ie if an NPC has `Resort_Tent_2` and `Resort_Tent`, they'll say one or the other with equal probability. (This is the same way `Resort` keys work in vanilla). (The marriage variant, however, probably won't work.)

### Specific roles

#### Bartenders

Gus, whenever he visits, will tend bar. When he does so, other characters (if they are adults) will crowd around him and drink. You can assign another NPC the bartender role by patching `Mods/atravita_Ginger_Island_Adjustments_bartenders` and using the internal name of the NPC as the key. (The value is ignored and can be anything).

```js
{
	"Format": "1.24.0",
	"Changes": [
		{
			"Action": "EditData",
			"Target": "Mods/atravita_Ginger_Island_Adjustments_bartenders",
			"Entries": {
				"NPCname": null,
			}
		}
	]
}
```

(Note: the Resort shop still requires Gus's presence. You can use Shop Tile Framework if you'd like your bartender to have a shop of their own, I suspect.)

#### Musicians

Sam, whenever he visits, has a chance of standing on a towel and playing his guitar, and Emily will dance around him. Using the `Mods/atravita_Ginger_Island_Adjustments_musicians` data asset, you can allow someone else music-playing. 

```js
{
	"Format": "1.24.0",
	"Changes": [
		{
			"Action": "EditData",
			"Target": "Mods/atravita_Ginger_Island_Adjustments_musicians",
			"Entries": {
				"NPCname": null,
			}
		},
		{
			"Action": "EditData",
			"Target": "Data/animationDescriptions",
			"Entries": {
				"npcname_beach_towel": //some sort of music-playing animation.
			}
		}
	]
}
```

(Any NPC with an animation description of `npcname_beach_dance` can be chosen as a dancer.) 

#### Fishers

Fishers do not need to be specified; they just need a `<name>_beach_fish` animation (ie `pam_beach_fish` for Pam.)

See also:

* [Wiki on schedules and animations](https://stardewvalleywiki.com/Modding:Schedule_data#Schedule_points)

### Exclusions

You can also exclude characters from the resort based on various conditions by patching into `Mods/atravita_Ginger_Island_Adjustments_exclusions`.

```js
{
	"Format": "1.24.0",
	"Changes": [
		{
			"Action": "EditData",
			"Target": "Mods/atravita_Ginger_Island_Adjustments_exclusions",
			"Entries": {
				"NPCname": "spring,17,mon,spring_mon,spring_17,neveralone",
			}
		}
	]
}
```

Possible values include: `<season>`, `<day>`, `<dayofweek>`, `<season>_<dayofweek>`, `<season>_<day>`, and `neveralone`. The first five should be self-explanatory, but `neveralone` prevents NPCs from attending the resort EXCEPT as [part of a group](#custom-groups). 

As an aside: NPCs already cannot go to Ginger Island on their hospital visit days, and no one goes to the Resort during the Winter Market. 

**Note:** I haven't actually tested this with conditional patching (using Content Patcher's `When` field.) [CustomNPCExclusions](https://www.nexusmods.com/stardewvalley/mods/7089), which does a very similar thing, forces `IslandSouth.SetUpIslandSchedules` to [run after Content Patcher is done updating patches](https://github.com/Esca-MMC/CustomNPCExclusions/blob/master/CustomNPCExclusions/HarmonyPatch_IslandVisit.cs).