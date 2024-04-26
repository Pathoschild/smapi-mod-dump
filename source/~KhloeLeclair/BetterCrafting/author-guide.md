**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

← [README](README.md)

This document is intended to help mod authors create content packs for Better Crafting.

## Contents

* [Getting Started](#getting-started)
  * [Create a Content Pack](#create-a-content-pack)
* [Big Craftable Actions](#big-craftable-actions)
* [Map Tile Actions](#map-tile-actions)
  * [Open Crafting Menu](#open-crafting-menu)
* [Trigger Actions](#trigger-actions)
  * [Open Crafting Menu](#open-crafting-menu)
* [Categories](#categories)
  * [Using Content Patcher](#categories-using-content-patcher)
* [Crafting Stations](#crafting-stations)
  * [Using Content Patcher](#crafting-stations-using-content-patcher)
* [Custom Recipes](#custom-recipes)
  * [Ingredients](#ingredients)
* [Dynamic Rules](#dynamic-rules)
* [Themes](#themes)

## Getting Started

### Create a Content Pack

1. Install [SMAPI](https://smapi.io/) and [Better Crafting](https://www.nexusmods.com/stardewvalley/mods/11115/)
   if you haven't yet. (If you haven't, how did you even find this?)
2. Create an empty folder in your `Stardew Valley\Mods` folder and name it
   `[BCraft] Your Mod's Name`. Replace `Your Mod's Name` with your mod's
   unique name, of course.
3. Create a `manifest.json` file inside the folder with this content:
   ```json
   {
	   "Name": "Your Mod's Name",
	   "Author": "Your Name",
	   "Version": "1.0.0",
	   "Description": "Something short about your mod.",
	   "UniqueId": "YourName.YourModName",
	   "ContentPackFor": {
		   // Do not change this when changing UniqueID
		   "UniqueID": "leclair.bettercrafting"
	   },
	   "UpdateKeys": [
		   // When you get ready to release your mod, you will populate
		   // this section as according to the instructions at:
		   // https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Update_checks
	   ]
   }
   ```
4. Change the `Name`, `Author`, `Description`, and `UniqueID` values to
   describe your mod. Later, don't forget to set your UpdateKeys before
   uploading your mod.
5. Create other files as described below, based on what you want your content
   pack to do with Better Crafting.


## Big Craftable Actions

By using Content Patcher, you can add a custom action to any big craftable that
does not already have one. These actions are [map tile actions](https://stardewvalleywiki.com/Modding:Maps#Action)
so you can do anything that you can do with one of those. For example, this
snippet would open the Prairie King arcade game when clicking a Tub o' Flowers:
```json
{
	"Format": "2.0.0",

	"Changes": [
		{
			"Action": "EditData",
			"Target": "Data/BigCraftables",
			"TargetField": [
				"108", // Tub o' Flowers
				"CustomFields"
			],
			"Entries": {
				"leclair.bettercrafting_PerformAction": "Arcade_Prairie"
			}
		}
	]
}
```

You can use this with any map tile action, but it is intended for use with
Better Crafting's custom action to open the crafting menu.

## Map Tile Actions

### Open Crafting Menu

`leclair.bettercrafting_OpenMenu [station/isCooking] [flags] [area]`

This is a custom map / big craftable action that can be used to open a
Better Crafting menu. It has three optional arguments.

1. `[station/isCooking]`: This can be either the Id of a custom crafting
   station, in which case the opened menu will be for that crafting station,
   or a `true` or `false` value to denote whether the menu is for cooking.
   If this is set to `false` or left out, the menu will be for crafting.
   If this is set to `true`, the menu will be for cooking.
   If this is set to a station's Id, the menu will be for that station.
2. `[flags]`: A comma separated list of flags to apply to the menu. Flags
   control how the menu discovers inventories that the player should be
   allowed to craft from, on top of any existing connected inventories from
   the Extended Workbench feature. The following values are valid flags:

   * `Area`: If this is present, the third argument is required and must be
     the radius in tiles of nearby inventories that should be available.
   * `Map`: If this is present, all inventories in the current map will
     be made available.
   * `World`: If this is present, all inventories in all active locations
     will be made available.
   * `Buildings`: If this is present, inventories inside buildings will
     be made available. This can be combined with `Area` or `Map`, or just
	 work in conjunction with Extended Workbench discovery.
3. `[area]`: If the `Area` flag is present, then this should be the
   discovery area, in tiles.

Here is an example command that opens the crafting menu, using all inventories
in a 16 tile radius, and that searches into buildings it encounters:

```
leclair.bettercrafting_OpenMenu false Area,Buildings 16
```


## Trigger Actions

### Open Crafting Menu

`leclair.bettercrafting_OpenMenu [station/isCooking] [flags] [area]`

This is identical to the map tile action, just exposed to triggers as well.


## Categories

Better Crafting organizes recipes into categories to make them easier for users
to find, especially in large packs with many recipes. Many of these categories
will categorize items automatically using rules, but some categories require a
manual touch.

To make changes to categories, you'll want to make a file in your content pack
called `categories.json`. This file is split into two main groups: `Crafting`
and `Cooking`, as all crafting in Better Crafting is divided into those two
separate groups.

A `categories.json` file might look like this:
```json
{
	"Crafting": [
		{
			"Id": "decoration",
			"Recipes": [
				"--Scarecrow",
				"--Deluxe Scarecrow",
				"Mini-Obelisk"
			]
		}
	]
}
```

The top level `Crafting` and `Cooking` objects are lists, containing category
objects. Each category must have an `Id` to identify it and a list of `Recipes`
at a minimum. The following fields are available:

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr>
<td><code>Id</code></td>
<td>

**Required.** The category's Id. This need not be unique to your content pack.
Categories with the same Id will be merged automatically.

</td>
</tr>
<tr>
<td><code>Name</code></td>
<td>

The category's display name. This does not currently support localization or
tokenized strings.

</td>
</tr>
<tr>
<td><code>Icon</code></td>
<td>

An optional CategoryIcon. A category icon must have a `Type` of either
`Item` or `Texture`. You should use `Item` in most cases. When using
`Item`, you may set a `RecipeName` or `ItemId` and the relevant item
will be used as the category's icon.

</td>
</tr>
<tr>
<td><code>Recipes</code></td>
<td>

A list of recipes for this category. Each entry should be a
recipe's name. You can start an entry with `--` to instead remove the recipe
from the category's list of recipes.

</td>
</tr>
<tr>
<td><code>UseRules</code></td>
<td>

If this is set to true, the Recipes list will be ignored and the provided
DynamicRules will be used to determine which recipes should appear within
the category.

</td>
</tr>
<tr>
<td><code>DynamicRules</code></td>
<td>

A list of DynamicRuleData. Each entry in the list should have an `Id`,
as well as any special data the specific rule requires. Most do not
require any.

</td>
</tr>
<tr>
<td><code>IncludeInMisc</code></td>
<td>

If this is set to true, recipes that appear in the category will also appear
in the Miscellaneous category.

</td>
</tr>
</table>


### Categories Using Content Patcher

In addition to using a `categories.json` file, you can edit the default
categories using Content Patcher. However, there are a few caveats.

1. Removing recipes via `--` does not work when using Content Patcher.
2. It is difficult to edit lists using Content Patcher.

However, Content Patcher does allow you to perform localization. In
some cases, you might want to use a hybrid approach where you use
Content Patcher to assign a display name, and a `categories.json` file
to set up your categories.

> When using Content Patcher, you'll need to make a separate content pack
> that targets Content Patcher rather than Better Crafting. For more on that, you'll
> want to see [Content Patcher's own documentation](https://github.com/Pathoschild/StardewMods/blob/stable/ContentPatcher/docs/author-guide.md).

To edit category data with Content Patcher, you'll want to use its `EditData`
action on the target `Mods/leclair.bettercrafting/Categories`.


## Crafting Stations

Similar to the mod Custom Crafting Stations, you can use Better Crafting to
set up customized crafting interfaces that only show certain recipes to users.

I implemented this feature because the Custom Crafting Stations mod is
currently on life support. Pathoschild is maintaining the mod as it's in, as
Pathos puts it, 'keeping the lights on' priority.

There are several benefits to using Better Crafting rather than Custom Crafting
Stations, as well as one drawback.

#### Benefits:
* The menu looks nicer. When using a custom crafting station with Better Crafting,
  you can display a custom icon and name to label the menu.
* You can categorize the recipes that are displayed.
* You can include any recipe in Better Crafting, and not just vanilla crafting
  recipes, which will be more of a benefit once custom recipe support is added
  for content packs via JSON.
* You can make a crafting menu that lets users craft recipes they don't
  have unlocked.
* Better Crafting supports almost all features of Custom Crafting Stations.

#### Drawbacks:
* You cannot make a crafting station with mixed crafting and cooking recipes.
* It is slightly more complicated to configure.

If this sounds good to you, then let's get started!

First though, there's a converter to help get you started if you're coming from
CCS. Check it out here: https://khloeleclair.github.io/CCSConverter/

You can define crafting stations with either Content Patcher (recommended), or
by creating a `stations.json` file in your Better Crafting content pack.

The `stations.json` file must contain a list of station objects, each object
having at minimum an `Id` and a list of `Recipes`. The following fields are
available:

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr>
<td><code>Id</code></td>
<td>

**Required.** The station's Id. This should be unique. It should not include
spaces. You will need this Id in order to open your crafting station.

</td>
</tr>
<tr>
<td><code>DisplayName</code></td>
<td>

The crafting station's display name. This does not currently support
localization or tokenized strings. It is recommended to implement localization
by using Content Patcher.

</td>
</tr>
<tr>
<td><code>Icon</code></td>
<td>

An optional CategoryIcon. A category icon must have a `Type` of either
`Item` or `Texture`. You should use `Item` in most cases. When using
`Item`, you may set a `RecipeName` or `ItemId` and the relevant item
will be used as the crafting station's icon.

If this is not set, and your station is opened via a big craftable, it
will automatically use your big craftable's sprite as the icon.

</td>
</tr>
<tr>
<td><code>AreRecipesExclusive</code></td>
<td>

If this is set to true, the recipes in this crafting station's list will
not be displayed in standard crafting menus, forcing users to use
this crafting station (or another custom crafting station) to craft them.

</td>
</tr>
<tr>
<td><code>DisplayUnknownRecipes</code></td>
<td>

If this is set to true, recipes that a user does not have unlocked
will be made available as if the user knew them when using this
crafting station. 

</td>
</tr>
<tr>
<td><code>IsCooking</code></td>
<td>

Whether or not this station is a cooking station. You can only display
crafting recipes OR cooking recipes.

</td>
</tr>
<tr>
<td><code>Recipes</code></td>
<td>

A list of recipes for this crafting station. Each entry should be a
recipe's name.

</td>
</tr>
<tr>
<td><code>Categories</code></td>
<td>

An optional list of [categories](#categories). These are not currently
editable by end users.

If you need help with categories, you ma want to create a category in
the normal menu using Better Crafting, and examine the JSON file it
saves to see how to structure your categories.

</td>
</tr>
</table>

### Dev Commands

To see if your station has been loaded, you can use the `bc_station`
console command to list all crafting stations. You can also open your
station by using `bc_station [id]` with the station's Id.

If you're using `stations.json` files, you can use `bc_station reload`
to reload all crafting station data.

If you're using Content Patcher, reloading your patch will automatically
reload all crafting station data.


### Crafting Stations Using Content Patcher

The best way to add a custom crafting station is by using Content Patcher,
as you can localize your strings and include dynamic tokens like your
mod's Id.

> When using Content Patcher, you'll need to make a separate content pack
> that targets Content Patcher rather than Better Crafting. For more on that, you'll
> want to see [Content Patcher's own documentation](https://github.com/Pathoschild/StardewMods/blob/stable/ContentPatcher/docs/author-guide.md).

To add or edit custom crafting stations, you'll want to use Content Patcher's
`EditData` action with the target `Mods/leclair.bettercrafting/CraftingStations`.
Here's a quick example that adds a station for crafting torches:
```json
{
	"Format": "2.0.0",

	"Changes": [
		{
			"Action": "EditData",
			"Target": "Mods/leclair.bettercrafting/CraftingStations",
			"Entries": {
				"{{ModId}}_TorchBench": {
					"Id": "{{ModId}}_TorchBench",
					"DisplayName": "Torches Only",
					
					"AreRecipesExclusive": true,

					"Recipes": [
						"Torch",
						"Wooden Brazier"
					],

					"Categories": [
						{
							"Id": "torches",
							"Name": "Torches",
							"Icon": {
								"Type": "Item"
							},
							"Recipes": [
								"Torch"
							]
						},
						{
							"Id": "not-torches",
							"Name": "Kind of Torch-Like",
							"Icon": {
								"Type": "Item"
							},
							"Recipes": [
								"Wooden Brazier"
							]
						}
					]
				}
			}
		},

		{
			"Action": "EditData",
			"Target": "Data/BigCraftables",
			"TargetField": [
				"108", // Tub o' Flowers
				"CustomFields"
			],
			"Entries": {
				"leclair.bettercrafting_PerformAction": "leclair.bettercrafting_OpenMenu FALSE TRUE {{ModId}}_TorchBench"
			}
		}
	]
}
```

This file does two things.

First, it defines a custom crafting station that lets you craft a Torch and
a Wooden Brazier. It's marked as exclusive, so you won't be able to craft
those using other crafting menus. It also sets up a pair of simple categories
for the menu.

Second, it edits the data of the Tub o' Flowers big craftable to add a custom
field. That custom field tells Better Crafting to perform a map tile action
when you attempt to use the big craftable, and the action is to open a
Better Crafting menu with our custom crafting station.


## Custom Recipes

Better Crafting has the ability to add custom recipes that are more advanced
than is possible in the base game. These recipes can be added either through
Content Patcher, or by using the C# API for maximum flexibility. It is not
yet possible to add custom recipes with a content pack targeting Better
Crafting directly, but that is planned.

> **Note:** These custom recipes are only usable through Better Crafting. They
> do not appear in the base crafting menu. They can be used with
> [Crafting Stations](#crafting-stations).

> When using Content Patcher, you'll need to make a separate content pack
> that targets Content Patcher rather than Better Crafting. For more on that, you'll
> want to see [Content Patcher's own documentation](https://github.com/Pathoschild/StardewMods/blob/stable/ContentPatcher/docs/author-guide.md).

To edit recipes using Content Patcher, you need to use the `EditData` action
with the target `Mods/leclair.bettercrafting/Recipes`. Here's a quick example
of a recipe that lets you craft a Prismatic Shard with 10 Iron Bars and 1,000g:

```json
{
	"Format": "2.0.0",

	"Changes": [
		{
			"Action": "EditData",
			"Target": "Mods/leclair.bettercrafting/Recipes",
			"Entries": {
				"{{ModId}}_Shard": {
					"AllowRecycling": false,

					"Ingredients": [
						{
							"Id": "this-id-doesn't-matter",
							"ItemID": "(O)335",
							"Quantity": 10
						},
						{
							"Id": "but-the-id-must-be-unique",
							"Type": "Currency",
							"Quantity": 1000
						}
					],

					"Output": [
						{
							"Id": "the-id-doesn't-matter-here-either",
							"ItemId": "(O)74"
						}
					]
				}
			}
		}
	]
}
```

These custom recipes have several top level properties for customizing their
behavior and how they appear. They can have as many ingredients as you want,
and there is a flexible system for specifying what output the recipe should
have building on the [item spawn fields](https://stardewvalleywiki.com/Modding:Item_queries#Item_spawn_fields)
introduced into the base game in version 1.6.

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr>
<td><code>Id</code></td>
<td>

**Required.** The recipe's Id. This must be unique. It should not contain
spaces. The recipe Id uses the same namespace as the game's built-in
recipes, so you should not pick names that conflict with those recipes.

</td>
</tr>
<tr>
<td><code>Default</code></td>
<td>

*Optional.* Whether or not players should know this recipe automatically.
If this is set to true, players will have this recipe by default.

If this is set to false, then the player must learn this recipe before
they will be able to craft it. You can use any standard method of teaching
the player a recipe for this to work.

Additionally, you can use a Crafting Station set to not require the
player to know recipes in order to access the recipe. This can be
combined with non-default recipes to make recipes that can only be
used via special crafting stations.

Default: `false`

</td>
</tr>
<tr>
<td><code>SortValue</code></td>
<td>

*Optional.* A string that should be used when sorting this recipe
in the menu.

</td>
</tr>
<tr>
<td><code>Condition</code></td>
<td>

*Optional.* A [game state query](https://stardewvalleywiki.com/Modding:Game_state_queries)
that determines whether or not this recipe should be available. This will
be evaluated every time a crafting menu is opened, and happens regardless
of if the player knows the recipe or if the recipe is listed in a
crafting station.

</td>
</tr>
<tr>
<td><code>IsCooking</code></td>
<td>

*Optional.* Whether or not the recipe is a cooking recipe. This changes
which built-in crafting stations (kitchen, workbench, cookout kit, etc.)
are able to craft this recipe.

Default: `false`

</td>
</tr>
<tr>
<td><code>AllowBulk</code></td>
<td>

*Optional.* Whether or not the recipe can be crafted in bulk. Setting this
to false will stop players from being able to open the bulk crafting window
for this recipe, as well as stopping them from holding Control or Shift to
make more of the recipe at a time.

Default: `true`

</td>
</tr>
<tr>
<td><code>AllowRecycling</code></td>
<td>

*Optional.* Whether or not this recipe can be reversed using Better Crafting's
recycling feature. By default, all recipes can be reversed. You may, however,
wish to prevent people from reversing particular recipes.

Default: `true`

</td>
</tr>
<tr>
<td><code>DisplayName</code></td>
<td>

**Required.** The recipe's name, as it should be displayed to the player in the
crafting menu. This supports [tokenizable strings](https://stardewvalleywiki.com/Modding:Tokenizable_strings).

The display name shouldn't be too long, since it is all displayed on one
line in a large font.

</td>
</tr>
<tr>
<td><code>Description</code></td>
<td>

The recipe's description, which is displayed to the player in the crafting
menu beneath the ingredients list. This also supports
[tokenizable strings](https://stardewvalleywiki.com/Modding:Tokenizable_strings).

This string can be longer than the display name, as it wraps and uses a
smaller font.

</td>
</tr>
<tr>
<td><code>Icon</code></td>
<td>

*Optional.* A CategoryIcon, similar to how icons are specified for categories
and crafting stations. It may have a `Type` of `Item` or `Texture`. If the type
is `Item`, then it can have an `ItemId` set, otherwise it will use the first
output item.

Default: `{"Type": "Item"}`

</td>
</tr>
<tr>
<td><code>GridSize</code></td>
<td>

*Optional.* An `X` and `Y` value for how large this recipe should appear on
the crafting page. Normal recipes are 1x1 and big craftables, such as chests,
are 1x2 by default.

Example, to make a wide item: `{"X": 2, "Y": 1}`

</td>
</tr>
<tr>
<td><code>Ingredients</code></td>
<td>

**Required.** A list of [ingredients](#ingredients). You can have as many
as you'd like, though for obvious reasons you should keep the number small.

</td>
</tr>
<tr>
<td><code>ActionsOnCraft</code></td>
<td>

*Optional.* A list of [trigger actions](https://stardewvalleywiki.com/Modding%3ATrigger_actions)
that should be run whenever this recipe has been crafted by a player.

Example: `
{
	"ActionsOnCraft": [
		"If !PLAYER_HAS_MAIL Current SomeFlag ## AddMail Current SomeFlag"
	]
}`

</td>
</tr>
<tr>
<td><code>Output</code></td>
<td>

**Required.** A list of item spawning rules. You can have as many as you'd
like, but at least one is required. Each output entry will be evaluated,
in order, until one returns an item. That item will be the recipe's output.

Each entry in the list uses the [item spawn fields](https://stardewvalleywiki.com/Modding:Item_queries#Item_spawn_fields)
feature built into the base game, giving you a lot of control over what
exactly a recipe will produce.

</td>
</tr>
</table>


### Ingredients

Each ingredient can represent a currency or item the player needs to spend
in order to craft the recipe. Ingredients have the following fields:

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr>
<th colspan="2">Shared Fields</th>
</tr>
<tr>
<td><code>Id</code></td>
<td>

**Required.** The ingredient's Id. This only needs to be unique within
the recipe's ingredients section.

</td>
</tr>
<tr>
<td><code>Type</code></td>
<td>

*Optional.* The ingredient type. This can be either `Currency` or `Item`.

Default: `Item`

</td>
</tr>
<tr>
<td><code>Condition</code></td>
<td>

*Optional.* A [game state query](https://stardewvalleywiki.com/Modding:Game_state_queries)
to control whether or not the ingredient is visible and required. If the
query does not evaluate to true, it will not be visible nor required.

</td>
</tr>
<tr>
<td><code>RecycleRate</code></td>
<td>

*Optional.* The rate at which this ingredient's resources are returned
when the recipe is used for recycling. Setting this to zero or below
will prevent the ingredient from being recycled at all.

For example, setting this to `0.5` will return exactly half of the
ingredient. If, for example, the ingredient is a currency ingredient
that takes 1,000 gold, it would return 500 to the player.

Default: `1.0`

</td>
</tr>
<tr>
<td><code>Quantity</code></td>
<td>

*Optional.* The amount of the ingredient that is required. Defaults to 1.

</td>
</tr>
<tr>
<th colspan="2">Currency Fields</th>
</tr>
<tr>
<td><code>Currency</code></td>
<td>

*Optional.* The currency that should be consumed by this ingredient.
This can be one of the following values:

<table>
<tr>
<th>Value</th><th>Description</th>
</tr>
<tr>
<td><code>Money</code></td>
<td>The player's gold.</td>
</tr>
<tr>
<td><code>FestivalPoints</code></td>
<td>The player's earned points at the current festival.</td>
</tr>
<tr>
<td><code>ClubCoins</code></td>
<td>The player's coins at the casino.</td>
</tr>
<tr>
<td><code>QiGems</code></td>
<td>The player's Qi Gems.</td>
</tr>
</table>

Default: `Money`

</td>
</tr>
<tr>
<th colspan="2">Item Fields</th>
</tr>
<tr>
<td><code>ItemId</code></td>
<td>

> You **must** specify one of `ItemId`, `ContextTags`, or `Query`.

*Optional.* The qualified or unqualified Item Id of the item this
ingredient should consume. This may also be used to consume any item
of a specific category by using a negative number for the category,
as you would do with traditional recipes as documented
[on the wiki](https://stardewvalleywiki.com/Modding:Recipe_data).

</td>
</tr>
<tr>
<td><code>ContextTags</code></td>
<td>

*Optional.* A list of one or more context tags. If these are
provided, then any item this ingredient should consume must have
all context tags in the list.

When using this, you should include a `DisplayName` and possible
an `Icon` as the crafting menu won't understand how to best
display the ingredient's requirements to the user.

</td>
</tr>
<tr>
<td><code>Query</code></td>
<td>

*Optional.* An [item query](https://stardewvalleywiki.com/Modding:Item_queries)
for filtering items that can match this ingredient.

Please be careful when using this. While we do perform caching
when we can, this can get expensive very quickly if the query
matches a lot of items.

</td>
</tr>
<tr>
<td><code>RecycleItem</code></td>
<td>

*Optional.* An item spawning rule to customize the resulting item
when this ingredient is recycled. This may be useful for overriding
the behavior when a crafting recipe is recycled, particularly when
using context tags or category matching.

This uses the [item spawn fields](https://stardewvalleywiki.com/Modding:Item_queries#Item_spawn_fields)
feature built into the base game.

</td>
</tr>
<tr>
<td><code>DisplayName</code></td>
<td>

*Optional.* A name to display to the user for this ingredient. If this
is not provided, it will be taken from the required item or currency.

This automatically supports all categories built into the game, but
will not display a sensible value when context tags are used and should
be manually set in that case.

</td>
</tr>
<tr>
<td><code>Icon</code></td>
<td>

*Optional.* A CategoryIcon, similar to how icons are specified for categories
and crafting stations. It may have a `Type` of `Item` or `Texture`. If the type
is `Item`, then it can have an `ItemId` set, otherwise it will use the first
item to match this ingredient.

Default: `{"Type": "Item"}`

</td>
</tr>
</table>


## Dynamic Rules

Better Crafting has a feature to set up recipe categories using dynamic rules
that match against one or more recipes, allowing users to benefit from
categorization without anyone needing to go through and include recipes in
them manually. These dynamic rules are traditionally handled via the C# API,
but as of version 2.6 they can also be introduced using Content Patcher by
taking advantage of the game's native
[item queries](https://stardewvalleywiki.com/Modding:Item_queries) feature.

> When using Content Patcher, you'll need to make a separate content pack
> that targets Content Patcher rather than Better Crafting. For more on that, you'll
> want to see [Content Patcher's own documentation](https://github.com/Pathoschild/StardewMods/blob/stable/ContentPatcher/docs/author-guide.md).

To edit dynamic rules using Content Patcher, you need to use the `EditData` action
with the target `Mods/leclair.bettercrafting/Rules`. Here's a quick example that
adds a dynamic rule that matches fertilizer items, based on the item's category:

```json
{
	"Format": "2.0.0",

	"Changes": [
		{
			"Action": "EditData",
			"Target": "Mods/leclair.bettercrafting/Rules",
			"Entries": {
				"{{ModId}}_Fertilizers": {
					"DisplayName": "FERTILIZER",
					"Description": "IT'S GOT WHAT PLANTS CRAVE.",
					"Icon": {
						"ItemId": "(O)167"
					},
					"Rules": [
						{
							"ItemId": "RANDOM_ITEMS (O)",
							"PerItemCondition": "ITEM_CATEGORY Target -19"
						}
					]
				}
			}
		}
	]
}
```

These dynamic rules can be added to categories you set up using their IDs,
just like any other dynamic rules. You can set a display name, description,
an icon, and a list of rules for matching items.

<table>
<tr>
<th>Field</th>
<th>Description</th>
</tr>
<tr>
<td><code>Id</code></td>
<td>

**Required.** The dynamic rule's unique identifier. This must be unique.

</td>
</tr>
<tr>
<td><code>DisplayName</code></td>
<td>

**Required.** The dynamic rule's name, as it should be displayed to the player in the
crafting menu. This supports [tokenizable strings](https://stardewvalleywiki.com/Modding:Tokenizable_strings).

</td>
</tr>
<tr>
<td><code>Description</code></td>
<td>

The dynamic rule's description, which is displayed to the player in the crafting
menu when picking rules and hovering over the rule in the list. This also supports
[tokenizable strings](https://stardewvalleywiki.com/Modding:Tokenizable_strings).

This string can be longer than the display name, as it wraps and uses a
smaller font.

</td>
</tr>
<tr>
<td><code>Icon</code></td>
<td>

*Optional.* A CategoryIcon, similar to how icons are specified for categories
and crafting stations. It may have a `Type` of `Item` or `Texture`. If the type
is `Item`, then it can have an `ItemId` set, otherwise it will use the first
matching item.

Default: `{"Type": "Item"}`

</td>
</tr>
<tr>
<td><code>Rules</code></td>
<td>

**Required.** A list of rules. You should keep this limited to avoid performance
issues when performing many item matching operations. This is a list of
[item spawn fields](https://stardewvalleywiki.com/Modding:Item_queries#Item_spawn_fields)
though certain fields are ignored.

You should stick to ItemId, RandomItemId, and PerItemCondition.

</td>
</tr>
</table>


### Themes

Better Crafting supports custom themes, which can include both images and textures.
I'm just going to be brief now, but please check out the Example theme included in
the mod to see the images and `theme.json` format the mod supports.

More interestingly, you should know that you can include a mod for Better Crafting
within other mods in two ways.

First, if a content pack targeting Better Crafting has a `theme.json` file in its
root folder, that will be detected as a theme.

Second, if a mod has a `leclair.bettercrafting:theme` key in its manifest, with
the name of a JSON file containing a theme, that will be detected as a theme
and be made available. As an example, if you have a manifest like this:

```json
{
	"UniqueID": "leclair.example",
	"Name": "Some Example Mode",
	"Author": "Khloe Leclair",
	"Version": "1.0.0",
	"Description": "Maybe this is a UI recolor mod?",
	"ContentPackFor": {
		"UniqueID": "Pathoschild.ContentPatcher"
	},
	"leclair.bettercrafting:theme": "Supported/Better Crafting/my-theme.json"
}
```

That would prompt Better Crafting to try loading a theme file from your mod's
`Supported/Better Crafting` folder with the name `my-theme.json`. All assets
would be expected to be relative to that path.
