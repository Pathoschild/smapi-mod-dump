**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods**

----

# Glow Buff Documentation

Glow buff provides a custom buff type which adds a glow ring like effect.

The following is some simple documentation on how to use it.

* [Get Started](#get-started)
	* [Basics](#basics)
	* [Fields](#fields)
* [Notes](#notes)
	* [Compatibility](#compatibility)
	* [Textures](#textures)
	* [Colors](#colors)

## Get Started

The mod is designed to be easy to integrate following the basic buff format as much as possible.

You don't need an extra content pack, everything can be added directly via C# or Content Patcher.

Glow buff is currently only applicable to food/drink items.

Before adding this buff, please read the [notes](#notes) section as it goes over some known incompatibilities and issues.

### Basics

The buff is added directly to the list of buffs for any food/drink item.

Here are some quick examples of the basic setup for adding the buff to an existing item with c# and content patcher:

**C#**
```cs
private void onAssetRequested(object? sender, AssetRequestedEventArgs e)
{
	if (e.NameWithoutLocale.IsEquivalentTo("Data\\Objects"))
	{
		e.Edit(asset => 
		{
			var data = asset.AsDictionary<string, ObjectData>().Data;
			data["{{Your_ItemId}}"].Buffs ??= []; //Make sure the buffs list exists
			data["{{Your_ItemId}}"].Buffs.Add(new()
			{
				Duration = 120, //The duration of the buff in game minutes
				BuffId = "MindMeltMax.GlowBuff.Glow", //Must be this exact Id to load the correct buff
				CustomFields = new()
				{
					//Add the properties for the buff, will be explained in Fields
				}
			});
		});
	}
}
```

**Content Patcher**
```json
{
  	"Format": "2.2.0",
  	"Changes": [
    	{
      		"Action": "EditData",
      		"Target": "Data/Objects",
      		"TargetField": [
        		"{{Your_ItemId}}"
      		],
      		"Entries": {
				"Buffs": [
					{
						"BuffId": "MindMeltMax.GlowBuff.Glow", //Must be this exact Id to load the correct buff
						"Duration": 120, //The duration of the buff in game minutes
						"CustomFields": {
							//Add the properties for the buff, will be explained in Fields
						}
					}
				]
      		}
    	}
  	]
}
```

Adding them to new objects works about the same, just add the Buffs field to your object data along with the fields specified above.

### Fields

All customization for the glow effect is added through the ``"CustomFields"`` property of an objects/buffs data.

Every field key must start with **"MindMeltMax.GlowBuff/"**, this is to ensure compatibility with other mods.

By default the following fields are supported:

| Key | Description |
| --- | ----------- |
| Glow | (Only for fields added to ObjectData) Tells the mod to load the glow buff even if it's not added to an objects list of buffs. |
| GlowTexture | The id of the texture to load. A list of recognized values can be found [here](#textures). |
| GlowRadius | The radius of the glow effect. |
| GlowColor | The rgba color value of the glow effect. (please see [notes](#colors) before using) |
| GlowDuration | The duration in game minutes of the buff. When added to a buff's custom fields, will be overriden when the buffs duration is not default (-2). When ommited from ObjectData, will default to lasting the rest of the day |
| DisplayName | The translated name of the buff as it will appear in the objects buff list when hovered (if the buff is added to the objects buffs), and the name of the buff in the buffs display. |
| Description | The translated description of the buff as it will appear in the buffs display. |

## Notes

The duration of the buff does not need to match that off the other applied buffs. If the duration of applied buffs do not match, the mod will show the duration of the glow buff after that of other buffs, separated by a comma.

### Compatibility

Due to a current compatibility issue with spacecore, if an item has multiple buffs and one doesn't have custom fields, it will crash when hovered.

While a fix for this has been submitted, there's already a few things you can do to avoid this issue:

* Make sure all custom fields for the objects buffs have a value, this can be empty, as long as it's not ``null``.
* Add the custom fields to the object directly instead of to the buffs custom fields.

### Textures

The game has a few different texture types for light sources, they are as follows:

| Id | Type |
| -- | ---- |
| 1 | (Default) the texture used by lanterns and glow rings |
| 2 | The texture used by televisions |
| 3 | Unused |
| 4 | The texture used by most light giving objects, fireflies, and junimo's in events |
| 5 | The texture used by regular ghosts |
| 6 | The texture used by the games default light points |
| 7 | The texture given of by the movie theater projector |
| 8 | The texture used by fish tanks |
| 9 | The texture used by the decorated trees during winter |
| 10 | The texture used for tiny light sources (like butterflies) |

### Colors

Due to how lighting colors are handled by the game, some colors show better than others. Keep this in mind when changing color values.

The format for colors can be either:

* **"{{red}},{{green}},{{blue}}"**
* **"{{red}},{{green}},{{blue}},{{alpha}}"**

~~But who's to say another value isn't possible~~