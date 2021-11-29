**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/XxHarvzBackxX/Custom-Winter-Star-Gifts**

----

← [README](README.md)
This document helps mod authors create a content pack for Custom Winter Star Gifts.

**See the [main README](README.md) for other info**.

# Content Packs

## How-to
In your `manifest.json` file, specify that this is a content pack for CWSG by placing CWSG's unique ID inside the `ContentPackFor` field (`harvz.CWSG`).
Next, create a `content.json` file. It should look similar to this:
```js
{
  "NPCGifts": [
    {
      "ItemNames": [
        {
          "Name": "...",
          "Quantity": 1,
          "Type": "Vanilla"
        }
      ],
      "NameOfNPC": "...",
      "Mode": "...",
      "Priority": 100
    }
  ]
}
```

## Fields

Field                | Valid Entries                        
-------------------- | ------------------------------- 
NameOfNPC            | `(string)` The internal name of the NPC you are targeting. <small>(Can also put 'All' if you want to apply to every NPC)</small>
ItemNames            | `(string, int, string)` The information about the items. `Type` being whether the item is 'Vanilla', 'JA' or 'DGA'.
Mode                 | `(string)` Which mode to use. Accepts "AddToExisting" <small>(adds to existing list for this target)</small>, "AddToVanilla" <small>(adds to the vanilla list for this target)</small>, or "Overwrite" <small>(overwrites existing list for this target).</small>
Priority             | `(int)` The order in the stack of which this should be patched (carries over to multiple content packs). This should be any number above 0, but you usually won't need to go much higher than 200.

An example content pack can be found on the [Nexus page under files tab](https://www.nexusmods.com/stardewvalley/mods/10024?tab=files).

### Multiple Targets
To add multiple targets, simply add multiple 'NPCGifts' entries. Copy and paste the first one, adding a comma to the end, and fill in the appropriate info. Example:
```js
{
  "NPCGifts": [
    {
      "ItemNames": [
        {
          "Name": "Apple",
          "Quantity": 5,
          "Type": "Vanilla"
        }
      ],
      "NameOfNPC": "Robin",
      "Mode": "Overwrite",
      "Priority": 100
    },
    {
      "ItemNames": [
        {
          "Name": "Apple",
          "Quantity": 5,
          "Type": "Vanilla"
        }
      ],
      "NameOfNPC": "Clint",
      "Mode": "AddToExisting",
      "Priority": 100
    }
  ]
}
```

## Variety
Have a combination of many modes, items, quantities, NPCs, and custom / non-custom items to make a fun collection of items, ideal for your vision. Example:
```js
{
  "NPCGifts": [
    {
      "ItemNames": [
        {
          "Name": "Apple",
          "Quantity": 5,
          "Type": "Vanilla"
        },
        {
          "Name": "Parsnip",
          "Quantity": 4,
          "Type": "Vanilla"
        }
      ],
      "NameOfNPC": "All",
      "Mode": "Overwrite",
      "Priority": 100
    },
    {
      "ItemNames": [
        {
          "Name": "Iridium Bar",
          "Quantity": 1,
          "Type": "Vanilla"
        },
        {
          "Name": "Pathos Cookies",
          "Quantity": 1,
          "Type": "JA"
        }
      ],
      "NameOfNPC": "Robin",
      "Mode": "AddToExisting",
      "Priority": 100
    }
  ]
}
```
This setup would make everyone give either 5 apples, or 4 parsnips, except Robin, who could give 5 apples, 4 parsnips, 1 iridium bar or 1 'Pathos Cookies' which is a custom JA item.
Make sure to combine many factors for some real variety!


# Questions
If you have any questions, feel free to open an issue on the Github, leave a comment on Nexus, or DM me on Discord at `XxHarvzBackxX#3665`!
-Harvz
