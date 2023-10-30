**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/nihilistzsche/FashionSenseOutfits**

----

# FashionSenseOutfits
FashionSenseOutfits adds a Content Patcher endpoint at nihilistzsche.FashionSenseOutfits/Outfits that you can target to change the current Fashion Sense outfit based on Content Patcher conditions and tokens.

It will do nothing on its own, you must have an additional Content Patcher content pack targeting the "nihilistzsche.FashionSenseOutfits/Outfits" endpoint.

You can change your outfit at the start of a new day, when traveling to a new location, or at a certain time.

In addition, a Content Patcher token at ```{{nihilistzsche.FashionSenseOutfits/CurrentOutfit}}``` is provided that returns the currently equipped outfit, which can be used to synergize with other Content Patcher mods.

You can see an example mod showing how to set a seasonal outfit [here](https://github.com/nihilistzsche/-CP-Seasonal-Fashion-Sense-Outfits), which you can download and modify as necessary.

Visit the [Content Patcher website](https://github.com/Pathoschild/StardewMods/tree/develop/ContentPatcher/docs/author-guide) for more information on tokens, EditData actions, and conditions to see what you can do.

The OutfitID is matched using case insensitivity, and will unpredictably select one if you have outfits with the same name but different cases.
I think that is a rare enough situation that I can live with the caveat.