**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/lisyce/SDV_Allergies_Mod**

----

# BarleyZP's Allergies

**BarleyZP's Allergy Mod** is a [Stardew Valley](https://www.stardewvalley.net/) mod which allows players to configure food allergies for an extra challenge. It is compatible with version 1.6+.

![Stardew Valley player is prompted to choose whether or not to eat cheese, which they are allergic to.](docs/CheeseAllergenPopup.png)

# Installation

1. Install [SMAPI](https://smapi.io/)
1. Install [this mod](https://www.nexusmods.com/stardewvalley/mods/21238) from Nexus
1. Install [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915)
1. Optionally, install [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) to easily configure the mod within the game
1. Launch Stardew Valley through SMAPI
1. Configure some allergies and enjoy the challenge!

Ensure that both the "\[CP\] BzpAllergies" and the "BzpAllergies" mods are directly inside your mod folder. This mod contains both a content pack and a C# component, so SMAPI needs to be able to find both of these components.

# For Mod Authors

## Harmony Patches

This mod changes core gameplay logic and thus employs some use of Harmony. Currently, the following methods are patched with prefix or postfix methods:

- `GameLocation.createQuestionDialogue`
- `Farmer.doneEating`
- `NPC.checkAction`

All of these prefixes allow the original logic to run afterwards, so they should be fairly compatible with other mods that patch these methods.

## Integrating Your Mod

### Registering Objects Under an Allergen

Context tags are the primary method of determining whether an item has an allergen. Assign your object one or more context tags of the form `BarleyZP.BzpAllergies_{allergen}` to give it an allergen. Currently, there are six supported allergen tags: `egg`, `wheat`, `fish`, `shellfish`, `treenuts`, and `dairy`. There are a few other ways objects can be registered under an allergen:

- Any object with the tags `egg_item`, `mayo_item`, or `large_egg_item` contains the egg allergen
- Any object with `milk_item`, `large_milk_item`, `cow_milk_item`, or `goat_milk_item` contains the dairy allergen
- Any object with the fish category (`-4`) that was NOT registered with a shellfish allergen context tag contains the fish allergen

# Bug Reports and Feature Requests

You may leave a comment on the linked Nexus mod page or create an issue on this GitHub repository's issues page. There is no guarantee that feature requests will be implemented, but I will take a look at your suggestions!

## Planned Features

- Support for custom allergens
- Public API and potential content pack framework to allow other mods to integrate more easily
- Randomized allergens for an extra challenge
- Configurable allergies for NPCs that influence gift tastes, dialogue, etc.
- Compatibility with Stardew Valley Expanded and Ridgeside Village
- Translation support
