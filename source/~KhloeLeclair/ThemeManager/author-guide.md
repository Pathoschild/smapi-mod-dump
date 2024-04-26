**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KhloeLeclair/StardewMods**

----

← [README](README.md)

This document is intended to help mod authors create content packs using Theme Manager.

# Contents

* [TL;DR](#tldr)

* [Getting Started](#getting-started)
  * [What is a Theme?](#what-is-a-theme)
  * [What About Game Themes?](#what-about-game-themes)
* [Creating a Theme](#creating-a-theme)
  * [`theme.json` Basics](#theme.json-basics)
  * [Theme Discovery](#theme-discovery)
  * [Create a Content Pack](#create-a-content-pack)
* [Game Themes](#game-themes)
  * [Basic Data](#basic-data)
  * [What are Color Variables?](#what-are-color-variables)
  * [What are Sprite Text Colors?](#what-are-sprite-text-colors)
  * [What is a Patch?](#what-is-a-patch)
  * [Built-in Patches](#built-in-patches)
  * [Writing a Patch](#writing-a-patch)
* [Other Mod Themes](#other-mod-themes)
  * [Asset Loading](#asset-loading)
  * [Code Example](#code-example)
* [Content Patcher Integration](#content-patcher-integration)
  * [Current Theme Token](#current-theme-token)
  * [Modifying Theme Data](#modifying-theme-data)
  * [Modifying Assets](#modifying-assets)
* [Miscellaneous](#miscellaneous)
  * [Color Parsing](#color-parsing)
  * [Helpful Commands](#helpful-commands)


# TLDR

Just want to mod the game's hard-coded colors? Sure. Do this:

1. If you have an existing mod, add a line like this to its `manifest.json`:
   ```json
   "stardew:theme": "theme.json",
   ```
2. Create a new file called `theme.json` in the same folder as your
   mod's manifest, and put this in it:
   ```json
   {
       "Name": "YOUR THEME'S NAME",
	   "SupportedMods": [
    	   "YourMods.UniqueId.GoesHere"
	   ],
	   "ColorVariables": {
    	   "Text": "hotpink", // you want hot pink text, right?
	   }
   }
   ```
3. Start putting colors in the ColorVariables section. You can
   get their names by checking [the built-in patches list](builtin-patches.md).
4. In game, make sure your theme is selected by Theme Manager. You can
   use Generic Mod Config Menu for that.
5. Use the `retheme` command whenever you change your theme to reload it.
6. Use the `tm_repatch` command if you ever make changes to patches.
7. ???
8. Profit!

> P.S. Please bug Khloe on the Stardew discord if there are colors you
> can't find, or try using the `tm_method_view` command and/or
> `tm_method_genpatch` to make your own patches!


# Getting Started

## What is a Theme?

A theme has two parts. First, there is a custom data model loaded from
the theme's `theme.json` file. This data model will change depending on
the mod, but should commonly contain color values though they are by no
means limited to only colors.

Second, themes can contain assets for mods to load. This relies upon the
mod implementing Theme Manager's API. For general asset replacement,
Content Patcher is still recommended.


## What About Game Themes?

In addition to providing a theme system for other mods to use, Theme Manager
also adds themes to the base game. Themes for the base game don't allow you
to replace assets. You should use Content Patcher for that. Instead, themes
allow you to replace colors. Almost every hard-coded color in the game can be
replaced with any other colors you want.


# Creating a Theme

## `theme.json` Basics

Every theme has a `theme.json` file (though it can, in some situations, have
a different filename). This file acts as a combination of a manifest for the
theme and a container for whatever data is supported. The specific data will
vary depending on if you're making a theme for a mod or for the base game.

The manifest properties will remain the same, however. Here's a simple example
`theme.json` that doesn't contain any extra data and just serves as a
manifest for the theme:

```json
{
    // The theme's name. A human-readable string shown to users in the
    // Theme Manager's theme selectors.
    "Name": "White Flowers",

    // Optional. The theme's name in other locales. This provides a simple
    // way to translate the theme's name, and it works no matter how the
    // theme is loaded.
    "LocalizedNames": {
        "es": "Flores Blancas"
    },

    // Optional. A list of unique IDs of mods that this theme is intended
    // to compliment. When a user's theme is set to "Automatic", the list
    // of supported mods is used to determine which theme should be used.
    "SupportedMods": [
        "SomeOtherMods.UniqueId"
    ]
}
```

There's a full list of supported manifest keys in the [Theme Manifest](#theme-manifest)
section below. You can create a valid theme with just a `Name`. But you'll
probably want to include more than that. Here's an example of a very basic
theme for the base game that would make all your text pink:

```json
{
    "Name": "Unnecessarily Pink",

    "ColorVariables": {
        "Text": "hotpink",
        "TextShadow": "maroon",
        "TextShadowAlt": "black"
    },

	"SpriteTextColorSets": {
		"*": {
			"default": "hotpink"
		}
	},

    "IndexedSpriteTextColors": {
        "-1": "hotpink"
    },

    "Patches": [
        "ItemTooltips"
    ]
}
```

In this example, `ColorVariables`, `SpriteTextColorSets`, and `IndexedSpriteTextColors`
are theme data that will be used by Theme Manager to overwrite colors used by the game.
`Patches` is another type of data used by game themes specifically to tell Theme Manager
what patches should be applied. Check out the section on [Game Themes](#game-themes)
for more details about how it all works.


## Theme Discovery

There are three separate ways to include a theme:

1. First, themes can be included directly in a mod. When Theme Manager is
   used for a mod, it will check that mod's `assets/themes/` folder for
   valid themes. The resulting file structure will look like:
   ```
   📁 Mods/
      📁 MyCoolMod/
         🗎 MyCoolMod.dll
         🗎 manifest.json
         📁 assets/
            📁 themes/
               📁 SomeTheme/
                  🗎 theme.json
                  📁 assets/
                     🗎 example.png
   ```

2. Next, themes can be included within a content pack for a mod. When Theme
   Manager is used for a mod, it will also check the mod's content packs to
   see if any of them have valid `theme.json` files. The resulting file
   structure for such a mod would look like:
   ```
   📁 Mods/
      📁 [MCM] My Cool Theme/
         🗎 manifest.json
         🗎 theme.json
         📁 assets/
            🗎 example.png
   ```

3. Finally, themes can be included within *any* mod or content pack simply
   by including a specific key within the mod's `manifest.json`. Theme Manager
   looks for a key starting with your mod's unique ID and that then ends with
   `:theme`. For example, if your mod's ID is `YourName.YourModName`, then
   Theme Manager would look for the key `YourName.YourModName:theme` in the
   manifests of other mods, checking for themes.

   > Note: If you want to use this method for adding a theme for the base game,
   > the manifest key to use is simply: `stardew:theme`

   This key should contain a relative filepath from the root of the mod to
   the theme's `theme.json` file. It uses the folder that `theme.json` file is
   in as the root folder of the theme. For example, you might end up with a
   `manifest.json` that looks like this:
   ```json
   {
       // The Usual Suspects
       "Name": "Some Other Cool Mod",
       "Author": "A Super Cool Person",
       "Version": "5.4.3",
       "Description": "Totally rad stuff.",
       "UniqueID": "SuperCoolPerson.OtherCoolMod",
       "MinimumApiVersion": "3.7.3",
       "ContentPackFor": {
           "UniqueID": "Pathoschild.ContentPatcher"
       },
   
       // Our Theme!
       "MyName.MyCoolMod:theme": "compat/MyCoolMod/theme.json"
   }
   ```

   To go with that, you might have a file structure that would look like:

   ```
   📁 Mods/
      📁 SomeOtherCoolMod/
         🗎 manifest.json
         📁 compat/
            📁 MyCoolMod/
               🗎 theme.json
               📁 assets/
                  🗎 example.png
   ```

## Create a Content Pack

The easiest way to create a new theme is to create a content pack for it.

1. Install [SMAPI](https://smapi.io/) and [Theme Manager](https://www.nexusmods.com/stardewvalley/mods/14525)
   if you haven't yet. (If you haven't, how did you even find this?)
2. Create an empty folder in your `Stardew Valley\Mods` folder and name it
   `[TM] Your Mod's Name`. Replace `Your Mod's Name` with your mod's
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
           // This should be the UniqueID of the mod your theme is for.
           // If you're making a theme for the base game, you should
           // leave this as "leclair.thememanager"
           "UniqueID": "leclair.thememanager"
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
5. Create a `theme.json` file inside the folder with this content:
   ```json
   {
       "Name": "Your Theme's Name"
   }
   ```
6. Change the `Name` value to describe your theme. It doesn't *need* to match
   the `Name` from your `manifest.json` file but it should to
   minimize confusion.
7. Add any other [Theme Manifest](#theme-manifest) values that you want.
8. Add all your theme's `assets/` and theme data, depending on what the mod
   you're making a theme for needs.


# Game Themes

## Basic Data

When creating a theme for the game, you're dealing with three main concepts:

1. Color Variables.
2. Sprite Text Colors.
3. Patches.

> Note: Theme Manager's game theming system does not allow you to replace
> assets. You should still use Content Patcher for that.

A theme file for the game could be as simple as:
```json
{
    "Name": "Blue Drop",

    "ColorVariables": {
        "DropDownText": "navy"
    },

    "Patches": [
        "OptionsDropDown"
    ]
}
```


## What are Color Variables?

ColorVariables are an easy way to specify certain colors to fulfill certain roles.
ColorVariables have names that start with a `$`, and they have a value that's
either a color or the name of another variable. Because of this, patches can
define specific variables that fall back to more generic variables if the
specific color hasn't been overwritten.

As an example, consider the `OptionsDropDown` patch. We'll get into the basics
of patches in a bit, but for now you should know that `OptionsDropDown` uses
the variable `$DropDownText`. But the variable `$DropDownText` has the default
value `$Text`.

In effect, this means that if your theme defines a value for `$DropDownText`,
then it will use that color. If your theme *doesn't* have a value for that
variable, it'll use `$Text` instead.

Variables can inherit from each other like this as much as you want, as long
as they don't loop. (And don't worry, there's loop detection.)

For a list of built-in color variables, please read the section on
[Built-in Patches](#built-in-patches). Every patch is described with a list of
supported variables, and I want to eventually add example screenshots.

> Note: If someone wants to help with the screenshots and improved
> documentation, I would appreciate you forever.


## What are Sprite Text Colors?

Sprite Text is one of the game's primary ways of rendering text. It's a larger
font, and by default it is a dark reddish brown. Here's an example of the text
`Journal`:

![](docs/SpriteText.png)

In previous versions of Stardew Valley, Sprite Text was limited to a few
predefined colors. This is largely a thing of the past (you're welcome),
but still applies to a few minor things in the game. Notably, this applies
to mail.

These are the default colors:
```json
{
    "IndexedSpriteTextColors": {
        "-1": "86, 22, 12", // #56160C
        "1": "SkyBlue",
        "2": "Red",
        "3": "110, 43, 255", // #6E2BFF
        "4": "White",
        "5": "OrangeRed",
        "6": "LimeGreen",
        "7": "Cyan",
        "8": "60, 60, 60" // #3C3C3C,
		"9": "JoJaBlue" // ##34327A
    }
}
```

All other indexes are black by default.

The color `-1` has special handling from the game. If the color is `-1` and you
are using a language that uses latin characters (English, Spanish, French,
German, etc.), then the game will use the texture `LooseSprites\font_colored`
for rendering. This texture has the default reddish brown color baked in. 

If you reassign the color `-1` to a color in your theme, Theme Manager will
automatically force the game to treat it as colored text and it will use the
texture `LooseSprites\font_bold` instead.

Here's the same `Journal` text but with the color `-1` set to `hotpink`:

![](docs/SpriteText-Pink.png)

However, as of Stardew Valley 1.6, I was able to request that SpriteText would
accept any arbitrary color. In order to deal with this increased flexibility,
we have a secondary data type: `SpriteTextColorSets`

There is a default color set named `*` (asterisk), but patches can add
their own named color sets. For example, there's also a color set named
`Billboard:Colors` that applies specifically to the `BillboardMenu` class
that renders the calendar and help wanted board outside Pierre's shop.

That same list of indexed colors above becomes this:
```json
"SpriteTextColorSets": {
	"*": {
		"default": null,
		"skyblue": "skyblue",
		"red": "red",
		"#6E2BFF": "#6E2BFF",
		"white": "white",
		"orangered": "orangered",
		"limegreen": "limegreen",
		"cyan": "cyan",
		"#3c3c3c": "#3c3c3c",
		"jojablue": "jojablue"
	}
}
```

You can use this to override *any* SpriteText rendering in the entire
game, be it from the base game or from mods, but due to the nature it can
be imprecise, changing all text with a specific color, everywhere. That's
why we have color sets, so you can focus your changes onto specific menus.


## What is a Patch?

Patches are the heart of how game themes work. In order to replace the
hard-coded colors the game's user interfaces use, Theme Manager needs to use
Harmony to modify the game's code.

Patches tell Theme Manager what specifically it should be modifying. Take
the following example:

```json
{
    "ID": "OptionsDropDown",

    "ColorVariables": {
        "$DropDownHover": "$Hover",
        "$DropDownText": "$Text"
    },

    "Patches": {
        "#OptionsDropDown:draw(SpriteBatch,*)": {
            "Colors": {
                "Wheat": { "*": "$DropDownHover" }
            },

            "ColorFields": {
                "Game1:textColor": { "*": "$DropDownText" }
            }
        }
    }
}
```

This patch is changing the draw method of the `StardewValley.Menus.OptionsDropDown`
class, and it's doing so by replacing every reference to the color `Wheat`
with the variable `$DropDownHover` and by replacing every reference to the
field `Game1.textColor` with the variable `$DropDownText`.

As you can see above, there's also a `ColorVariables` section in this patch.
This lets a patch set up sensible defaults. In this case, the default value
for `$DropDownText` is `$Text` and the default value for `$DropDownHover` is
`$Hover`. These are only used if the current theme doesn't have them defined.


## Built-in Patches

Please see [this document](builtin-patches.md) for a list of all built-in
patches, along with details on what variables they use with screenshots
demonstrating their effects.


## Writing a Patch

Sorry, the guide isn't very fleshed out yet. There are a few helpful
commands for writing your own patches.

### `tm_method_tree [method]`

This command will display a rendering tree starting from the given method,
showing you every single method it calls that (potentially) does any drawing.
If you don't supply a method, this will try grabbing the currently visible
menu's draw method.

### `tm_method_view [method]`

This command will display a list of every color, texture, SpriteText, Lerp,
etc. that you can modify within a given method. If you don't supply a method,
this will try grabbing the currently visible menu's draw method.

### `tm_method_genpatch [method]`

This command is similar to the `tm_method_view` command, but it will generate
a basic patch JSON and print it to the console to give you a starting point.


# Other Mod Themes

## Asset Loading

## Code Example

# Content Patcher Integration

## Current Theme Token

## Modifying Theme Data

## Modifying Assets

# Miscellaneous

## Color Parsing

## Helpful Commands

# Oops, I haven't written this yet!

Sorry. I'm still working on this. It should be finished within a few days.
Until then, please check out:

* The [ThemeManager Example C# Mod](https://github.com/KhloeLeclair/StardewMods/tree/main/ThemeManagerExample)
  * Its [assets/themes/](https://github.com/KhloeLeclair/StardewMods/tree/main/ThemeManagerExample/assets/themes) folder
* The [Theme Template](https://github.com/KhloeLeclair/StardewMods/blob/main/ThemeManager/assets/themes/template/theme.json)
  for the base game.
* The [assets/patches/](https://github.com/KhloeLeclair/StardewMods/tree/main/ThemeManager/assets/patches)
  folder with all the currently available patches to use in themes for the base game.

You can also bug me on the Stardew modding Discord server. I'm the one
and only Khloe Leclair, and I hang out in #making-mods.
