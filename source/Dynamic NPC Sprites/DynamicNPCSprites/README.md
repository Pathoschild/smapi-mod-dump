**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/miketweaver/BashNinja_SDV_Mods**

----

**DynamicNPCSprites** is a [Stardew Valley](http://stardewvalley.net/) mod which lets you to add custom character portraits and sprites that can dynamically change based on the weather and season, and lets NPC wear random clothing each day. You can configure the mods to use by editing `.json` files.

This mod can be used directly by players, or as a framework for sprite and portrait designers.

**There are no portraits or sprites included by default** (<s>though there's a [demo pack]() you can use to try it out</s>).

Compatible with Stardew Valley 1.2+ on Linux, Mac, and Windows. 

Written by bashNinja — pull requests are welcome!

## Contents
* [Install](#install)
* [Use](#use)
* [Examples](#examples)
* [Compatibility](#compatibility)
* [Versions](#versions)
* [Help](#help)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
2. Install <s>this mod from Nexus mods</s>.
3. Run the game using SMAPI.

## Use
The mod looks for files inside the `config/` folder with a `.json` extension, and reads them when the game starts. You can create a `.json` file in a text editor to configure the mod.

### Available settings
setting | effect | required
:------ | :----- | :-----
`Name` | Put the NPC name here, or `Cat` or `Dog`. | Yes
`Weather` | Put the weather types here. Possible values: `clear`, `leaves`, `pollen`, `rainy`, `snowy`, `stormy`, `default`. If `default` or `null` (or nonexistent) it will match as long as there are no other matches for that weather. Additionally, if you have Climates of Ferngill installed, you can also put `flurries`.  | No
`Season` | Put the season names here. Possible values: `spring`, `summer`, `fall`, `winter`. | Yes
`Sprite` | Put the path to the sprite's `.png` or `.xnb` file (relative to the DynamicNPCSprites mod folder). | No
`Portrait` | Put the path to the portrait's `.png` or `.xnb` file (relative to the DynamicNPCSprites mod folder). | No 

## Examples
### Example 1:
```json
[
	{
	"Name": "Haley",
	"Weather": ["Clear","Rainy","Stormy","Pollen","Leaves"],
	"Season": ["Spring","Summer","Fall"],
	"Sprite": "sprites/haley.png",
	"Portrait": "portraits/haley.png"
	},
	{
	"Name": "Haley",
	"Weather": ["Clear","Rainy","Stormy","Pollen","Leaves"],
	"Season": ["Spring","Summer","Fall"],
	"Sprite": "sprites/haley2.png",
	},
	{
	"Name": "Haley",
	"Weather": ["Clear","Rainy","Stormy","Pollen","Leaves"],
	"Season": ["Spring","Summer","Fall"],
	"Portrait": "portraits/haley3.png"
	},
	{
	"Name": "Haley",
	"Weather": ["Clear","Rainy","Stormy","Pollen","Leaves"],
	"Season": ["Winter"],
	"Sprite": "sprites/haley-cold.xnb",
	"Portrait": "portraits/haley-cold.xnb"
	}
]
```
The above example does the following:
1. During all weathers in Spring/Summer/Fall, randomly choose a portrait/sprite each day for the `Haley` NPC. Notice that some of them set both the sprite and portrait, and some only set one of them; if a sprite/portrait isn't specified, the default one will be used.
2. During all weathers in Winter, the `Haley` NPC will have the `haley-cold.xnb` sprite and portrait.

### Example 2:
```json

	{
	"Name": "Cat",
	"Weather": ["Clear"],
	"Season": ["Spring","Summer","Fall","Winter"],
	"Sprite": "sprites/Eevee_Sheet.xnb",
	"Portrait": "portraits/Eevee_Portrait.png"
	},
	{
	"Name": "Cat",
	"Weather": ["Rainy"],
	"Season": ["Spring","Summer","Fall"],
	"Sprite": "sprites/Vaporeon_Sheet.xnb",
	"Portrait": "portraits/Vaporeon_Portrait.png"
	},
	{
	"Name": "Cat",
	"Weather": ["Snowy"],
	"Season": ["Winter"],
	"Sprite": "sprites/Glaceon_Sheet.xnb",
	"Portrait": "portraits/Glaceon_Portrait.png"
	},
	{
	"Name": "Cat",
	"Weather": ["Stormy"],
	"Season": ["Spring","Summer","Fall"],
	"Sprite": "sprites/Jolteon_Sheet.xnb",
	"Portrait": "portraits/Jolteon_Portrait.png"
	},
	{
	"Name": "Cat",
	"Weather": ["Pollen"],
	"Season": ["Spring"],
	"Sprite": "sprites/Leafeon_Sheet.xnb",
	"Portrait": "portraits/Leafeon_Portrait.png"
	},
	{
	"Name": "Cat",
	"Weather": ["Leaves"],
	"Season": ["Fall"],
	"Sprite": "sprites/Umbreon_Sheet.xnb",
	"Portrait": "portraits/Umbreon_Portrait.png"
	}
]
```
This example will set the `Cat` NPC to be Eevee every day except those days with weather. The Eevee evolves each day to the corresponding weather (e.g. Vaporeon in the rain and Glaceon in the snow).

## Compatibility
This mod is compatible with:
- Climates of Ferngill
  - Climates of Ferngill adds an extra weather, `Flurries`. When that mod is installed, you can use that weather type too.
- Portraiture
  - This mod allows higher-resolution character portraits. When that mod is installed, just add your high-res portraits to the DynamicNPCSprites `.json` files for seasonal changes. Don't put them in the Portraiture folders if you want seasonal/weather changes.

## Versions
### 1.0.0
* Initial release.

## Help
If you need help, contact me in on of the following (preferred first):
* [Stardew Valley Discord](https://stardewvalley.community/), #modding channel. Ask for `@bashNinja`.
* <s>Discussion thread</s>
* [Github Issues](https://github.com/miketweaver/BashNinja_SDV_Mods/issues)

## See also
* <s>Nexus mod</s>
* <s>Discussion thread</s>

## License
This is licensed under the [MIT License](LICENSE) which gives a lot of freedom. I would appreciate pull requests and keeping this as the main mod location. If you'd like to fork for a feature, consider bringing it back to the main mod or working with me to make the mod do what you would like. That said, the mod is as open as the license says. Enjoy.
