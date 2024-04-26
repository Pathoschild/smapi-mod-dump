**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Leroymilo/Catalogue-Framework**

----

# Why use Catalogue Framework?

When making a Content Pack, you may want to make a Catalogue to allow players to access all the cool stuff you made.
Unfortunately, the game will not let you do so : all the catalogues in the main game are hard-coded!
Thanks to this mod, you can link a Furniture to a Shop.

# How to use Catalogue Framework?

To make your custom catalogue work you'll need to create 2 sparate mods :
- a Content Pack for Content Patcher
- a Content Pack for Catalogue Framework

Once you made both of them, you can compress them both in the same .zip file and upload them on Nexus as a single file so the players only have one file to download.

**Be carefull!** Even if those 2 mods are part of a single Nexus mod, they need to have distincts UniqueIds! However, they can have the same update key.

If you like to learn with examples, check the [Custom Catalogue Example](https://github.com/Leroymilo/Catalogue-Framework/tree/main/CatalogueFramework/example)

## Making the Content Pack for Content Patcher

You're probably already used to this, if you want a custom catalogue to access stuff from an already existin Content Pack of yours, it's only a matter of adding a few changes to your Content Pack.

In short, you need to have a Furniture to host your Catalogue, and a Shop entry to describe what's in your catalogue.
Here's the [example](https://github.com/Leroymilo/Catalogue-Framework/tree/main/CatalogueFramework/example/[CP]%20Custom%20Catalogue%20Example/content.json), it should have all the information you need.

For more information on how to use Content Patcher, see [its amazing documentation](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/README.md).

It's also possible to create a custom catalogue from an already existing Shop and/or an already existing Furniture, see the next part for more info on how to do this.

## Making the Content Pack for Catalogue Framework

This step is the easy part, all you need is a file named `catalogue.json` and a `manifest.json`.
The `catalogue.json` should contain a dictionary with the structure :
```json
{
	"Furniture Id": "Shop Id"
}
```
With `Furniture Id` and `Shop Id` matching those set in the Content Patcher Content Pack.

In a single Catalogue Content Pack, you can define as many catalogues as you want, just make sure to have a separate furniture for each shop.

**Be carefull!** This does not support Content Patcher tokens like `{{ModId}}` (and even if it did, the mod's UniqueId would be different), so make sure to write the exact same identfiers!

The `manifest.json` is very similar to what you need for a Content Patcher Content Pack, but you need to change the `ContentPackFor` to this :
```json
{
	"UniqueID": "leroymilo.CatalogueFramework"
}
```

Again, make sure to use a different UniqueId for the 2 packs.