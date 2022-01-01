**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TehPers/StardewValleyMods**

----

# Teh's Fishing Overhaul

For **SDV** 1.5.5+ and **SMAPI** 3.13.0+

Completely reworks fishing:

- Control every aspect of fishing! Configure the chances of hitting fish instead of trash, the difficulty of catching fish, the chances of finding treasure, what fish/treasure/trash you'll find, where you'll find it all, and how the fish behave.
  - Add new fish, adjust fish behaviors, and control where they can be caught. If you want to be able to catch diamonds, wearable boots, or even fishing rods while fishing, go ahead!
- Perfect catches are more rewarding. As you get more perfect catches in a row, your streak increases, and fish become more valuable.
- Legendary fish are no longer caught by standing in a specific spot and casting your line. Now, they are caught like any other fish, but there is a very low chance you'll find one!
- Optional built-in fishing HUD. See what fish you can catch while you're fishing!
- Support for [Generic Mod Config Menu][gmcm], including in-game config editing.
  - Some complex settings may not be modifiable through GMCM though.
- Create your own content packs, or install a content pack created by someone else!

## Configs

Most config settings can be modified in-game with [Generic Mod Config Menu][gmcm]. For more fine-tuned control over the settings, the configs can also be modified through their respective `.json` files in the mod's `config` folder.

| File            | Purpose                           |
| --------------- | --------------------------------- |
| `fish.json`     | Fishing and the fishing minigame. |
| `treasure.json` | Treasure found while fishing.     |
| `hud.json`      | The fishing HUD.                  |

If you want to configure which fish, trash, or treasure are available to catch and when you can catch them, [create][create a content pack] or install a content pack.

## Content packs

This mod supports content packs. For more information on how to make a content pack, check the [content pack docs].

## API

Two APIs are available for this mod:

- The simplified API can be retrieved through SMAPI's mod registry.
- The full API can be accessed by using TehCore.

To use the API, check out the [API docs][api docs].

## Source code & License

This mod is licensed under MIT License. The source code and full license text can be found on the [GitHub repository][github repo].

[gmcm]: https://www.nexusmods.com/stardewvalley/mods/5098
[create a content pack]: https://stardewvalleywiki.com/Modding:Content_packs#Create_a_content_pack
[content pack docs]: https://github.com/TehPers/StardewValleyMods/tree/full-rewrite/docs/TehPers.FishingOverhaul/Content%20Packs.md
[api docs]: https://github.com/TehPers/StardewValleyMods/tree/full-rewrite/docs/TehPers.FishingOverhaul/API.md
[github repo]: https://github.com/TehPers/StardewValleyMods/tree/full-rewrite
