# ![[icon]](promo/icon.png) Pregnancy Role

*a [Stardew Valley](http://stardewvalley.net/) mod by [kdau](https://kdau.gitlab.io)*

People of all genders may or may not be able to become pregnant. This mod gives Stardew Valley farmers and NPCs the full range of possibilities.

Pregnancy Role does not change the chance or timing of baby proposals between spouses. However, if and when a proposal for a new baby is made, whether the spouse is an NPC or another player, this mod will affect what happens. There will be a pregnancy only if one of the spouses can become pregnant and the other spouse can make pregnant. In any other combination, there will be an adoption instead.

## ![Compatibility](https://kdau.gitlab.io/headers/compatibility.png)

**Game:** Stardew Valley 1.4+

**Platform:** Linux, macOS, Windows or Android

**Multiplayer:** Only applies to players with the mod installed and their spouses. If two players marry in-game, they must both have the mod installed.

**Other mods:**

* Mods affecting the pregnancy and adoption features may or may not be compatible.
* If custom skill mods are installed, the dropdown on the Skills tab will be adjusted to fit.

## ![Installation](https://kdau.gitlab.io/headers/installation.png)

1. Install [SMAPI](https://smapi.io/)
1. Download this mod from [Nexus](https://www.nexusmods.com/stardewvalley/mods/5762?tab=files) or [ModDrop](https://www.moddrop.com/stardew-valley/mods/768494-pregnancy-role)
1. Unzip and put the `PregnancyRole` folder inside your `Mods` folder
1. Run the game using SMAPI

## ![Use](https://kdau.gitlab.io/headers/use.png)

For **farmers and farmhands** (players), you can change the pregnancy role as follows:

1. Open the pause menu. (On Linux/macOS/Windows, press Escape. On Android, tap the menu icon.)
1. Go to the Skills tab (the tab whose icon is your farmer's face).
1. Choose a value in the "Pregnancy Role" dropdown.

For **vanilla NPC spouses** (characters from the base game), you can change the pregnancy role as follows:

1. Open the pause menu. (On Linux/macOS/Windows, press Escape. On Android, tap the menu icon.)
1. Go to the Social tab (the tab whose icon is a heart).
1. Choose your spouse in the list.
1. Choose a value in the "Pregnancy Role" dropdown.

For **custom NPCs** and modded vanilla NPCs, modders can use Pregnancy Role as follows:

1. Add this mod as a dependency of your mod. Its unique ID is `kdau.PregnancyRole`.
1. Locate field 9 (relationships) of the NPC's entry in `NPCDispositions`:
	* For a custom NPC, use the `EditData` patch that manifests your NPC.
	* For a vanilla NPC, add an `EditData` patch that edits this field for the NPC.
1. Append one of the following (no quotes) to the list of relationships:
	* `PregnancyRole become` (can become pregnant)
	* `PregnancyRole make` (can make pregnant)
	* `PregnancyRole adopt` (always adopt)

## ![Configuration](https://kdau.gitlab.io/headers/configuration.png)

If you need to hide or reposition the "Pregnancy Role" dropdown on the Skills tab and/or the Social tab detail page, you can edit this mod's `config.json` file. It will be created in the mod's main folder (`Mods/PregnancyRole`) the first time you run the game with the mod installed. These options are available:

* `ShowPlayerDropdown`: Set to `false` to hide the "Pregnancy Role" dropdown for the player on the Skills tab.
* `PlayerDropdownOrigin`: Set the `X` and `Y` values to pixel coordinates, relative to the top left of the game menu, where the dropdown should appear. Coordinates (0,0) mean that the position will be calculated automatically.
* `ShowSpouseDropdown`: Set to `false` to hide the "Pregnancy Role" dropdown for the spouse on the Social tab detail page.
* `SpouseDropdownOrigin`: Set the `X` and `Y` values to pixel coordinates, relative to the top left of the detail page, where the dropdown should appear. Coordinates (0,0) mean that the position will be calculated automatically.

## ![Translation](https://kdau.gitlab.io/headers/translation.png)

No translations are available yet.

This mod can be translated into any language supported by the game. Your contribution would be welcome. Please see the [instructions on the wiki](https://stardewvalleywiki.com/Modding:Translations). You can send me your work in [a GitLab issue](https://gitlab.com/kdau/pregnancyrole/-/issues) or [the Discord channel](https://discord.gg/Bs8fQYW).

## ![Acknowledgments](https://kdau.gitlab.io/headers/acknowledgments.png)

* This mod is based on ideas and advice from [EssGee](https://www.nexusmods.com/stardewvalley/users/83595503).
* Like all mods, this one is indebted to ConcernedApe, Pathoschild and the various framework modders.

## ![See also](https://kdau.gitlab.io/headers/see-also.png)

* [Release notes](doc/RELEASE-NOTES.md)
* [Source code](https://gitlab.com/kdau/pregnancyrole)
* [Discuss on Discord](https://discord.gg/Bs8fQYW)
* [Report bugs](https://gitlab.com/kdau/pregnancyrole/-/issues)
* [My other mods](https://kdau.gitlab.io)
* Mirrors:
	[Nexus](https://www.nexusmods.com/stardewvalley/mods/5762),
	[ModDrop](https://www.moddrop.com/stardew-valley/mods/768494-pregnancy-role),
	[forums](https://forums.stardewvalley.net/index.php?resources/pregnancy-role.53/)

Other mods to consider:

* [Diverse Stardew Valley with SVO](https://www.nexusmods.com/stardewvalley/mods/4079)
* [Gender Neutrality Mod](https://www.nexusmods.com/stardewvalley/mods/722)
* [Mx. Qi](https://www.nexusmods.com/stardewvalley/mods/4310)
* [No More Dialogue Differences](https://www.nexusmods.com/stardewvalley/mods/4459)
