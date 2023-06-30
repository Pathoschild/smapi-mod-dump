**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Shockah/Stardew-Valley-Mods**

----

[← back to readme](README.md)

# Release notes

## 1.1.3
Released 25 June 2023.

* Additional Inflation affix fixes.

## 1.1.2
Released 25 June 2023.

* Potentially fixed the Inflation affix infinitely stacking the price change on the same items over and over.

## 1.1.1
Released 16 June 2023.

* Fixed the "Season" affix set choice period option presenting choice every day.

## 1.1.0
Released 16 June 2023.

* Added an option to change the affix set choice period, as in, how often you get to choose the affixes and how long they stay active for.
* Added an option to have an additional affix set choice on Year 1 Spring 2.
* Modified the config menu and config handling to allow no-affix choices to be presented. This will be presented as a fake "Monotony" affix.
* Fixed the Bursting affix asking for confirmation if [Stop Rug Removal](https://www.nexusmods.com/stardewvalley/mods/10192) was installed.
* Fixed the Resilience affix throwing errors.
* Fixed the (vanilla) Skill affixes giving their extra levels multiple times.
* Fixed some multiplayer issues - the mod should now proceed if a player leaves while the choice menu is up.
* Fixed reversed end of day menus (for example, if you leveled up twice in the same skill).
* Fixed an error when adding affix set entries in the config menu and then reverting to default settings.
* Added an `AffixActivationContext context` parameter to some of the API methods. This is a breaking change.

## 1.0.1
Released 9 June 2023.

* Improved Harmony errors (for easier debugging).
* Fixed the Hurricane affix always being active.
* Fixed the Love affix throwing errors upon entering split-screen.
* Fixed the Regrowth affix causing crops to be invisible.

## 1.0.0
Released 3 June 2023.

* Initial release.