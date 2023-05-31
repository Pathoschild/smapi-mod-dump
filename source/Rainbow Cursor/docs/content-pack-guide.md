**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/jltaylor-us/StardewRainbowCursor**

----


# Rainbow Cursor Content Pack Guide

A content pack is a type of mod that provides _content_ to some other mod
(generally called the "framework" mod).  Content packs for **Rainbow
Cursor** allow you to add new color palettes without writing any code.
This page will walk you through creating a content pack mod.

## Getting started

Start with an empty directory.  By convention, content pack directories
usually start with some indication of the framework mod for which they are
a content pack.  So, in this example we're going to create an empty
directory named `[RC] Example`.

## Create a manifest

Create a plain text file named `manifest.json`.  This file tells SMAPI
about your mod and lets it know what framework mod should load it.

You can read about what goes in the manifest (and get a basic example that
you can copy) on the [Stardew Valley
Wiki](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest).
The important bit that's specific to making a **Rainbow Cursor** content
pack is to set the `UniqueID` property that's inside the `ContentPackFor`
section to **Rainbow Cursor**'s unique ID: `"jltaylor-us.RainbowCursor"`.
(Do not confuse this property with the `UniqueID` property at the "top
level" of the manifest; that is for your own mod's unique ID.)

## Define some color palettes

The color palettes are defined in a file named `palettes.json`, so create
an empty plain text file with that name.  Start with some initial contents
that look like this:

```json
{
  "FormatVersion": "1.0.0",
  "Palettes": [

  ]
}
```

Your color palette or palettes go in the blank space in the middle there.
Each color palette definition looks like this:

```json
    {
      "Id": "some internal name you make up - whatever you want",
      "Name": "Name for UI",
      "Colors": [
       
      ]
    },
```

The comma on the last line is optional for the last (or only) palette, but
it's ok to leave it in.  `Name` is optional; the value displayed in the
configuration UI will fall back to the value of `Id` if `Name` is missing (and
there's no internationalization file - see below).  `Id` and `Colors` are
required.

The colors in your color palette go in the blank space in the middle.
Each one is specified as `R`ed, `G`reen, and `B`lue values from 0 to 255.  For
example, red would look like
```json
         {"R": 255, "G": 0, "B": 0},
```

As with the entire palette object, the trailing comma is optional on the
last (or only) color.

So, putting all of that together, if you wanted to make a Christmas
themed palette with Red, White, and Green, then your `palettes.json` would
look something like this:

```json
{
  "FormatVersion": "1.0.0",
  "Palettes": [
    {
      "Id": "xmas",
      "Name": "Christmas",
      "Colors": [
         {"R": 255, "G": 0, "B": 0},
         {"R": 255, "G": 255, "B": 255},
         {"R": 0, "G": 255, "B": 0},
      ]
    },
  ]
}

```

## Advanced Topics:  Tooltips and Internationalization

**Rainbow Cursor** also lets you use SMAPI's
[internationalization](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Translation#i18n_folder)
facilities to provide translations for your content pack.  So far the only
thing we've talked about that _could_ be translated is the `Name`
property, but if you use the i18n feature then you can also provide text
to display as a tooltip over your palette in the GMCM interface.

You can read more about the file layout and format on the Stardew Valley
Wiki (linked above), but to continue on with this example, create an empty
translation file at `[RC] Example/i18n/default.json`.

Each palette you defined in `palettes.json` has two different translatable
properties.  The names of these properties are built using the value of
the `Id` field for that palette.  Here's a complete example for our
Christmas themed palette.

```json
{
  "palette.xmas.name": "Christmas",
  "palette.xmas.tooltip": "Merry Christmas!"
}
```
