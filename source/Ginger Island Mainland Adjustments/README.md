**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments**

----

# Ginger Island Mainland Adjustments

One of the things I've noticed is that Ginger Island is not particularly well integrated with the rest of NPC scheduling and dialogue. Pierre can tell you that he'd rather be outside than running his shop one minute, and the next you'll find him enjoying the Resort. Or Jodi would tell you to wipe your feet off when she was outside on the beach. For that, this mod implements *specific dialogue keys* that are only used while an NPC is (a) on the mainland and (b) headed to and away from Ginger Island. I was going to write dialogue lines for everyone but honestly I couldn't come up with lines for...a lot of characters, actually. So a few have lines, but for the rest, I hope the included content pack and documentation are clear enough.

Additionally, NPCs would just head directly to bed after going to Ginger Island (with the exception of Gus), skipping their usual evening schedule points, leaving, say, Demetrius dancing alone if Robin went to Ginger Island. `GIRemainder` schedules now take over for the evening instead, so characters might make a stop or two before going home. These **are** included in the optional CP pack. Instructions to adding more can be found [here](./Ginger%20Island%20Mainland%20Adjustments/docs/GIRemainderSchedules.md).

The original point of this mod was to change those two things. A little scope creep may have happened. This mod has an option to completely replace the usual Ginger Island scheduling to handle the following:

* **Custom NPC groups**. One of the more charming things I found about Ginger Island is that the NPCs actually attend in groups. For this mod, the vanilla groups have been moved to a data file, and mod authors can easily add more visiting groups. (Details in the documentation - just know that no additional groups for custom NPCs are included in this mod.)

* **More NPCs visit Ginger Island**. If you, like me, enjoy having large numbers of NPCs, it's sometimes hard to catch a specific NPC going to the resort. This mod will let you increase the cap to up to 12 per day by setting the configuration option `Capacity`. Great for running around gifting people!
* **NPCs** can wander onto `IslandSouthEast` and `IslandNorth`. **Most won't get too far**, but some might be a little more...adventurous.
* **Gus has a dedicated day**. Set the config options `GusDay` and `GusChance`, and he'll have specific days where he's more likely to go to Ginger Island...
* **Willy, Sandy, George, and Evelyn can go to Ginger Island**. (Configurable). Sandy and Willy will even leave a box on their counters so you can still shop! My friend [Elizabeth](https://www.nexusmods.com/stardewvalley/users/120958053?tab=user+files) has made her these excellent beach sprites, [available here](https://www.nexusmods.com/stardewvalley/mods/10960)!

### Install

1. Install the latest version of [SMAPI](https://smapi.io).
2. Download this mod and unzip it into `Stardew Valley/Mods`.
3. Run the game using SMAPI.
4. Optionally: download the small example content pack, which has `GIRemainder` schedules and a *few* dialogue keys, mostly just replacing lines I felt were most egregious. 

### Uninstall
1. Simply delete this mod from your mods directory.

### How to use

**Configuration settings:**

Note: you will need to run the mod at least once to generate the `config.json` file. [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) integration is supported if you want to edit the values from inside the game.

1. `EnforceGITiming` will output warnings if schedule points are placed too closely to each other and skip that schedule point, but only if `UseThisScheduler` is **false**. Recommended while setting up/debugging schedules.
2. `UseThisScheduler` will cause the mod to generate Ginger Island schedules. **The remaining settings will only have affect if this is enabled**.
3. `Capacity` sets the maximum number of visitors to the Ginger Island Resort. Will allow values up to twelve.
4. `GroupChance` sets the probability a group of NPCs will be picked to visit the Resort. 
5. `ExplorerChance` sets the probability a smaller group of NPCs will explore Island North.
6. `GusDay` and `GusChance` set a day that Gus is more likely to go to the Resort, and the chance that he'll be there. (set `GusDay` to `None` to disable this feature.)
6. `AllowWilly` allows Willy access to the Resort.
7. `AllowSandy` allows Sandy access to the Resort. (Psst! If you want her to have beach sprites, check out [violetlizabet](https://www.nexusmods.com/stardewvalley/users/120958053?tab=user+files)'s mod [here](https://www.nexusmods.com/stardewvalley/mods/10960)!)
8. `AllowGeorgeAndEvelyn` allows George and Evelyn access to the resort.

**Additionally:**

Three console commands: `av.gima.get_schedule <NPCname>` shows you their schedule of the day.`av.gima.get_islanders` tells you who is going to the island. Do note that if `UseThisScheduler` is set to false, GIRemainder schedules are added one per ten minutes starting from 610, and thus they may not be in until 7AM or so.

`av.gima.get_locations_list` dumps out the current value for `NPC.routesFromLocationToLocation`.

For more documentation, see the [author guide](./Ginger%20Island%20Mainland%20Adjustments/docs/README.MD).

### Compatibility

* Works with Stardew Valley 1.5.6 on Linux/macOS/Windows.
* Works in single player, multiplayer, and split-screen mode. 

Compatibility with other mods: mostly unknown. It should be relatively independent and thus compatible, and Island schedules are actually perfectly normal schedules in the code, they're just generated randomly by the code. Let me know if there's specific incompatibilities, but do keep in mind that I'll only build in compat for mods that are **open source** and only when I have the time and energy to do it.

If you have a mod that changes the Ginger Island maps, this mod may be incompatible, since both it (and the vanilla game) hardcode locations for NPCs to stand on. I might do something about that in the future, but it's not on the top of my list.

#### Current compatibility list:

* **Child2NPC**: For NPCs created with Child2NPC, my schedules won't parse *unless* they have a defined `GIRemainder` schedule. (This has to do with scheduling code changing between 1.4 and 1.5). For GIMA versions 1.1.2 and above, child NPCs can only go the Resort as part of a group, so for now I'd recommend either not putting them into a group or defining a `GIRemainder` schedule for them. I'm definitely working on compatibility for this!

### Additionally...

* Translations would be very welcome! DM me or send me a pull request!
* [Change log](./Ginger%20Island%20Mainland%20Adjustments/docs/changelog.md)
* [Source code](https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments)
* Technical details: requires SMAPI, uses Harmony.
