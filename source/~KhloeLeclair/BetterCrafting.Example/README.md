**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

# Better Crafting: Content Pack Example

This mod demonstrates how to use a content pack to modify
Better Crafting's default data sets.

* [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/11115/)
* [Changelog](https://github.com/KhloeLeclair/StardewMods/blob/main/BetterCrafting.Example/CHANGELOG.md)


## Available Changes

### categories.json

By including a `categories.json` file, you can modify the default item
categories. This includes both adding new recipes to a category, removing
existing recipes, changing the icon, and changing the default name.

You can find the [complete default `categories.json` file here](https://github.com/KhloeLeclair/StardewMods/blob/main/BetterCrafting/assets/categories.json).
Modifications use the same syntax. If you're just adding or removing recipes,
all you need to include is the category's Id and a list of Recipes to add or
remove. For example:

```json
{
	"Crafting": [
		{
			"Id": "machinery",
			"Recipes": [
				"Fabricator",
				"--Mini-Jukebox"
			]
		}
	]
}
```

That snippet would add a recipe named `Fabricator` to the Machinery category,
and remove a recipe named `Mini-Jukebox`. Recipes to be removed should be
prefixed with two minus signs (`--`). If your need to add a recipe that's
name starts with two minus signs, please prefix it with a space (` --MyRecipe`).

When renaming a category, you should provide your own localization key by
setting `I18nKey` to a value. If you wish to disable localization for the
category name, `I18nKey` should be set to an empty string (`""`).


### connector_examples.json

The `connector_examples.json` file is a simple list of object names that should
be displayed in Better Crafting's configuration menu, providing a default list
of choices for users to select for adding objects to the valid connectors list.

> Please note that not all objects will be detected. You should test whether or
> not your object actually works as a connector before adding it to this list.

Like with recipes in the categories file, you can prefix a connector name with
two minus signs (`--`) to instead remove from the list, and if needed prefix the
name with a space (` --`) if you need to add an item with a name starting with
two minus signs.


### floors.json

The `floors.json` file is a map of floor indexes (`floor.whichFloor`) to names.
This list is used when determining if a bit of flooring is a valid connector
by converting the numeric id to a string for checking against the user's valid
connectors list.

Unlike the other data files, this file does not support negating entries. You
may, however, rename existing entries simply by including them in your own data
file and setting a new name.
