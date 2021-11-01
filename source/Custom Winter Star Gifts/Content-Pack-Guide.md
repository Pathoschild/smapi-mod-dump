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
      "NameOfNPC": "...",
      "ItemNames": [
        "..."
      ],
      "Mode": "..."
    }
  ]
}
```

## Fields

Field                | Valid Entries                        
-------------------- | ------------------------------- 
NameOfNPC            | (string) The internal name of the NPC you are targeting.<small>(Can also put 'All' if you want to apply to every NPC)</small>
ItemNames            | (string array) Names of the items you wish to add for this target.    
Mode                 | (string) Which mode to use. Accepts "Add" <small>(adds to existing list for this target)</small> or "Overwrite" <small>(overwrites existing list for this target).</small>   

An example content pack can be found on the [Nexus page under files tab](https://www.nexusmods.com/stardewvalley/mods/10024?tab=files).

### Multiple Targets
To add multiple targets, simply add multiple 'NPCGifts' entries. Copy and paste the first one, adding a comma to the end, and fill in the appropriate info. Example:
```js
{
  "NPCGifts": [
    {
      "NameOfNPC": "Robin",
      "ItemNames": [
        "Apple", "Parsnip"
      ],
      "Mode": "Add"
    },
    {
      "NameOfNPC": "Clint",
      "ItemNames": [
        "Prismatic Shard"
      ],
      "Mode": "Overwrite"
    }
  ]
}
```

## Variety
Have a combination of many modes, items and NPCs to make a fun collection of items, ideal for your vision. Example:
```js
{
  "NPCGifts": [
    {
      "NameOfNPC": "All",
      "ItemNames": [
        "Apple", "Parsnip"
      ],
      "Mode": "Overwrite"
    },
    {
      "NameOfNPC": "Robin",
      "ItemNames": [
        "Iridium Bar"
      ],
      "Mode": "Add"
    }
  ]
}
```
This setup would make everyone give either an apple or a parsnip, except Robin, who could give an apple, a parsnip, or an iridium bar.
Make sure to combine many factors for some real variety!


# Questions
If you have any questions, feel free to open an issue on the Github, leave a comment on Nexus, or DM me on Discord at `XxHarvzBackxX#3665`!
-Harvz
