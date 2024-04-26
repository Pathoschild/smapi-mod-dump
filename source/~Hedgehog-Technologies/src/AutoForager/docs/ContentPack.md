**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Hedgehog-Technologies/StardewMods**

----

# Content Pack Documentation

[Content packs](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Content_Packs) are a special type of mod that are owned and read by a given mod instead of SMAPI directly. 

## Auto Forager Content Pack Format
### Example
#### content.json
```json
{
	"Category": "Lumisteria - Visit Mount Vapius Forage",
	"Forageables": [
		"Lumisteria.MtVapius_BirchSap",
		//... limited for brevity
	],
	"FruitTrees": [
		"Lumisteria.MtVapius_CasalotierSapling",
		//... limited for brevity
	],
	"WildTrees": [
		"Lumisteria.MtVapius.AmberTree",
		//... limited for brevity
	],
	"IgnoredItems": [
		"Lumisteria.MtVapius.FakeItem"
	]
}
```

Key          | Required  | Description
:----------: | :-------: | :-------------------:
Category     | YES       | This is the Category that the options will be displayed under in Generic Mod Config Menu
Forageables  | NO        | An array of Unique Object Ids. Any items that can be spawned and picked up as a forageable in the world. Also can include buried forageables.
FruitTrees   | NO        | An array of Unique Fruit Tree Ids. Any fruit trees that you'd like categorized under your designated "Category". They should be detected and added as config options regardless.
WildTrees    | NO        | An array of Unique Wild Tree Ids. Any wild trees that you'd like categorized under your designated "Category". They should be detected and added as config options regardless.
IgnoredItems | NO        | An array of Unique Object Ids. Any items that may get recognized by the Auto Forager that may not actually be spawned forage.

#### manifest.json
```json
{
	"Name": "[AF] Lumisteria - Visit Mount Vapius Forage",
	"Description": "Forage added by Lumisteria - Visit Mount Vapius",
	"Author": "Jag3Dagster",
	"UniqueID": "HedgehogTechnologies.LVMForageAF",
	"Version": "1.0.0",
	"MinimumApiVersion": "4.0",
	"ContentPackFor": {
		"UniqueID": "HedgehogTechnologies.AutoForager",
		"MinimumVersion": "3.0.0"
	}
}
```

Key               | Description
:---------------: | :----------:
Name              | The content pack's human-readable name
Description       | A short explanation of the content pack's purpose
Author            | Who wrote / maintains the content pack
UniqueID          | A unique identifier for the content pack. Recommended format \<your name\>.\<mod name\>
Version           | The content pack's [semantic version](http://semver.org/). Should be updated with along any update release.
MinimumApiVersion | Minimum version of SMAPI that the content pack is compatible with.
ContentPackFor    | UniqueID: The Unique of the mod that will own / read this content pack. MinimumVersion: Minimum required version of the owning mod.
