**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/SwizzyStudios/SV-SwizzyMeads**

----

# SV-SwizzyMeads
Stardew Valley mod to add more mead flavors based on existing honey types. This was accomplished by using [Harmony](https://github.com/pardeike/Harmony) to alter the Stardew Valley base code.

New Meads
=
Following is the new Meads and their base values.

| Honey Type | Base Value | Mead Type | Mead Base Value | Artisan Profession (40%) |
|----------|---------:|---------|--------------:|-----------------------:|
|Honey|100g|Mead|200g|280g|
|Wild Honey|100g|Mead|200g|280g|
|Tulip Honey|160g|Tulpi Mead|320g|448g|
|Blue Jazz Honey|200g|Blue Jazz Mead|400g|560g|
|Summer Spangle Honey|280g|Summer Spangle Mead|560g|784g|
|Poppy Honey|380g|Poppy Mead|760g|1,064g|
|Fairy Rose Honey|680g|Fairy Rose Mead|1,360g|1,904g|

Installation steps
=
1. Download the mod from [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/3241)
2. Place the SwizzyMeads folder into the Stardew Valley Mods (Steam/steamApps/common/Stardew Valley/Mods)
3. Start up Stardew Valley through SMAPI.

Altered Stardew Valley Code
=
Stardewvalley.Object.cs method "performObjectDropInAction"
