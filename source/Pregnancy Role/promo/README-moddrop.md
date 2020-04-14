![[icon]](https://kdau.gitlab.io/PregnancyRole/icon.png) People of all genders may or may not be able to become pregnant. This mod gives Stardew Valley farmers and NPCs the full range of possibilities.

Pregnancy Role does not change the chance or timing of baby proposals between spouses. However, if and when a proposal for a new baby is made, whether the spouse is an NPC or another player, this mod will affect what happens. There will be a pregnancy only if one of the spouses can become pregnant and the other spouse can make pregnant. In any other combination, an adoption will occur instead.

## ![[Compatibility]](https://kdau.gitlab.io/headers/compatibility.png)

This version of Pregnancy Role is compatible with **Stardew Valley 1.4+**.

The mod should work on **Linux, Mac, Windows and Android**.

There are no known problems with multiplayer use. The mod will only apply to players who install it and their spouses. If two players marry in-game, they must both have the mod installed.

Other mods that affect the pregnancy and adoption features may or may not be compatible with this mod.

## ![[Installation]](https://kdau.gitlab.io/headers/installation.png)

1. Install the latest version of [SMAPI](https://smapi.io/).
1. Download this mod from the link in the header above.
1. Unzip the mod and place the `PregnancyRole` folder inside your `Mods` folder.
1. Run the game using SMAPI.

## ![[Use]](https://kdau.gitlab.io/headers/use.png)

For **farmers and farmhands** (players), you can change the pregnancy role as follows:

1. Open the pause menu. (On Linux/Mac/Windows, press Escape. On Android, tap the menu icon.)
1. Go to the Skills tab (the tab whose icon is your farmer's face).
1. Choose a value in the "Pregnancy Role" dropdown.

For **vanilla NPC spouses** (characters from the base game), you can change the pregnancy role as follows:

1. Open the pause menu. (On Linux/Mac/Windows, press Escape. On Android, tap the menu icon.)
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

## ![[Translation]](https://kdau.gitlab.io/headers/translation.png)

This mod can be translated into any language supported by Stardew Valley. No translations are currently available.

Your contribution would be welcome. Please see the [details on the wiki](https://stardewvalleywiki.com/Modding:Translations) for help. You can send me your work in an issue [on GitLab](https://gitlab.com/kdau/pregnancyrole/-/issues), in the Bugs tab above or by DM on Discord.

## ![[Acknowledgments]](https://kdau.gitlab.io/headers/acknowledgments.png)

* This mod is based on ideas and advice from [EssGee](https://www.nexusmods.com/stardewvalley/users/83595503).
* Like all mods, this one is indebted to ConcernedApe.
* This mod would not function without [SMAPI](https://smapi.io/) by Pathoschild.

## ![[See also]](https://kdau.gitlab.io/headers/see-also.png)

* [Release notes](https://gitlab.com/kdau/pregnancyrole/-/blob/master/doc/RELEASE-NOTES.md) from existing versions
* [Issue tracker](https://gitlab.com/kdau/pregnancyrole/-/issues) for bug reports and feature plans
* [MIT license](https://gitlab.com/kdau/pregnancyrole/-/blob/master/LICENSE) (TLDR: do whatever, but credit me)
* [My other mods](https://kdau.gitlab.io)

Mirrors:

* [This mod on Nexus](https://www.nexusmods.com/stardewvalley/mods/5762)
* **This mod on ModDrop**
* [This mod on GitLab](https://gitlab.com/kdau/pregnancyrole)

Other mods to consider:

* [Diverse Stardew Valley with SVO](https://www.moddrop.com/stardew-valley/mods/580603-diverse-stardew-valley-with-seasonal-villager-outfits-dsvo)
* [Gender Neutrality Mod](https://www.nexusmods.com/stardewvalley/mods/722)
* [Mx. Qi](https://www.nexusmods.com/stardewvalley/mods/4310)
* [No More Dialogue Differences](https://www.nexusmods.com/stardewvalley/mods/4459)
