**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/kdau/cropbeasts**

----

# Customizing Cropbeasts

Additional crops, including custom crops added by Json Assets or otherwise, can be made eligible to become cropbeasts by patching this mod's data.

The relevant asset is stored in the mod as `assets/Cropbeasts.json`, but should not be edited there. Instead, patch it as if it were a base game asset at `Content/Data/Cropbeasts.xnb`. Your mod will need to depend on this one, but should not be designated as a content pack for it.

The cropbeast data file is a JSON dictionary. The keys and values are both strings with fields separated by slashes (`/`), like in many base game files. These fields are supported:

|String|Index|Field             |Required|Default|Example              |
|------|-----|------------------|--------|-------|---------------------|
|key   |0    |harvest object ID |yes     |—      |`190` *(Cauliflower)*|
|key   |1    |whether giant crop|no      |`false`|`true`               |
|key   |2    |ignored           |no      |—      |`Giant Cauliflower`  |
|value |0    |cropbeast name    |yes     |—      |`Giant Cropbeast`    |
|value |1    |choice weight     |no      |`0.0`  |`0.3`                |
|value |2    |primary color     |no      |*      |`#f3f0c0` *(flesh)*  |
|value |3    |secondary color   |no      |*      |`#4e8a1e` *(leaves)* |

The last field in the key is ignored, but can be used to annotate entries with the corresponding crop names for easy reference.

The cropbeast name must be one of the following:

* `Berrybeast`, `Leafbeast`, `Trellisbeast`: These work with non-giant crops and use the texture of the harvested crop object.
* `Grainbeast`, `Rootbeast`: These work with non-giant crops and use the texture of the fully grown crop.
* `Giant Cropbeast`: This works with giant crops and uses the giant crop texture.
* `Ancient Beast`, `Cactusbeast`, `Coffeebeast`, `Qi Beast`, `Starbeast`: These are intended for specific stock crops and may have unexpected results with other crops.

The choice weight biases the pseudorandom algorithm that spawns cropbeasts towards (if positive) or away from (if negative) choosing a crop of this type. Values of `5.0` and higher should effectively guarantee that a crop of this type will be chosen if one is present.

If no primary color is specified, one of the game's 36 standard colors will be chosen based on the crop object's `color_` tag. If no secondary color is specified, it will be the same as the primary color.
