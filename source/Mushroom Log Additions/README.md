**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/justastranger/MushroomLogAdditions**

----

# Mushroom Log Additions

Framework for adding new tree type -> mushroom log output mappings.

## Format

The content pack format for this framework is ultimately: `Dictionary<string, List<Tuple<string, float>>>`.

```json
{
	"treeType#": [
		{ "outputQualifiedItemId": 0.1 }
	]
}
```

Example:
```json
{
	// mushroom tree -> mushroom tree seed
	"7": [ { "(O)891": 1 } ],
	// birch -> 10% chance of purple mushroom, otherwise red mushroom
	"2": [
        { "(O)422": 0.1 },
        { "(O)420": 1 }
    ]
}
```