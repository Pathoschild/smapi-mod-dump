**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/hawkfalcon/Stardew-Mods**

----

# Better Junimos

## TODO

* Nothing

## Changelog
* 3.0 RELEASE
  * Update for SDV 1.6 / SMAPI 4.0
* 3.1.2
  * Missing from this repo
* 2.0.8 RELEASE
  * Redo asset loaders for new SMAPI
  * How bout we actually bundle the CP mod into the bundle this time
* 2.0.7 RELEASE
  * Add a bit of logging to the Junimo spawn key
  * Work Ridiculously Fast mode is now ridiculous
* 2.0.6 RELEASE
  * Incorporate changes since 2.0.4 
* 2.0.6-beta8
  * Remove Event Repeater fields from configuration event. Otherwise event happens each day when Event Repeater installed.
* 2.0.6-beta7
  * Add event when first hut is built to ask configuration questions. Sets up wages and progression, explains hut menu and greenhouse
* 2.0.6-beta6
    * Add config option to disable harvesting of all remaining crops at end of season. For use when player has mod that allows crop survival across seasons esp. flowers.
    * Add config option to disable out-of-season check when planting seeds. For use when player has mod that allows all-season growing but doesn't update the seasons check properly.
    * Use warp point from greenhouse map when kicking Junimos out of the greenhouse. For realistic leaving behaviour when using custom greenhouse.
    * PathfindToRandomSpot now considers the entire greenhouse fair game including for custom greenhouses. Stops disturbing behaviour where Junimos appear to hang around the work area in a scary swarm.
    * Paddy crops sown near water get correct 25% growth boost.
* 2.0.6-beta5
    * Fix the pathfinding override for Junimo spawn in greenhouse. Makes custom greenhouses work.
    * Suppress Junimos-need-payment message when closing a non-hut chest. OnDayStarted now tags all Junimo hut chests via modData, so when an ItemGrabMenu gets closed we can see if it's for one of our hut chests.
* 2.0.6-beta4
   * Payment-needed message shows what items are still outstanding. Helps users understand what's going on when items vanish from the chest during partial-payment situations.
    * Add console commands `bj_list_abilities`, `bj_list_cooldowns`, `bj_reset_cooldowns`. Mostly used to narrow down what's going on when Junimos seem to be work-shy.
* 2.0.6-beta2
  * Add ability for Junimos to warp into greenhouse within range of hut. Enables a Junimo hut to do work in nearby greenhouse.
  * Add API signatures to pass location to plugin abilities, not just Farm. Allows BJ plugins to support non-farm locations in particular the greenhouse.
  * Rewrite all abilities to work in the greenhouse.
  * Rewrite Junimo pathfinding to work in the greenhouse.
  * Rewrite action availability search to work in the greenhouse.
* 2.0.6-beta1
    * Change how Abilities dictionary is loaded/saved. Allows centralized control of all BJ and plugin-provided abilities. In future this might be possible via GMCM.
    * Keep farmhands out of the hut progression menu. Progression information and a lot of the hut settings are not synced so the menu is wildly inaccurate.
* 2.0.5-beta8
  * Detect when player has Alternative Textures installed and tool active. Allows players to repaint the hut without the progression menu ruining everything.
  * Allow custom keybind for opening hut progression menu, and for menu to be disabled entirely. Options of last resort for players who need the hut menu click available for some other mod to use.
* 2.0.5-beta7
  * Add API call `ShowConfigurationMenu` to open BJ's GMCM menu. Allows BJF to have a configuration icon on the HUD menu.
  * Add configuration option `WorkRidiculouslyFast` to shorten harvesting delays as much as possible. Junimos are still not working at a truly ridiculous rate because they run out of work to do within the ten-minute, further work needed.
* 2.0.5-beta5
  * Move progression data out of the save into Farm.modData. Allows all players in MP game to contribute to progression quests.
  * Remove checks that prevent various updates running for MP farmhands, now that the mod is more MP-safe 
  * Option to grant experience for harvesting, farming and foraging. Provided against my better judgement for players who need lots of XP for Walk of Life but won't adjust WoL's options.
  * Build list of all available fertilizers in FertilizeAbility. Removes a hardcode so next time CA or soeme mod adds GlobalHyperMegaNetSpeedGro, Junimos will start using it.
  * Add a default unlock cost to all abilities when registered. Allows plugin-provided abilities to at least kinda participate in the progression system.
* 2.0.5-beta4
  * Add Korean translation from wally232
* 2.0.5-beta3
  * Move more strings out into i18n files. Improves ability to translate mod content.
* 2.0.5-beta2
  * Streamline how hut radius is calculated
* 2.0.5-beta1
  * Ensure latest configuration settings are always used.  In particular hut radius is always recalculated.Allows GMCM config changes in-game.  
  * Display whether Junimos have been paid in hut info. Helps users understand when Junimos are on strike rather than just lazy.
* 2.0.3
  * Add debug commands to list huts/abilities/actions, trigger and unlock progression quests in console.
  * Avoid accessing save data when running as MP farmhand 