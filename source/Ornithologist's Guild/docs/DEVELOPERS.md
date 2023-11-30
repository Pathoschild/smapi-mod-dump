**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/greyivy/OrnithologistsGuild**

----

Table of Contents
=================

* [Creating a content pack](#creating-a-content-pack)
   * [Complete example](#complete-example)
   * [manifest.json](#manifestjson)
   * [content.json](#contentjson)
   * [Birdie object](#birdie-object)
   * [Weight system examples](#weight-system-examples)
      * [Winter forest birds](#winter-forest-birds)
      * [Vanilla Stardew Valley birds](#vanilla-stardew-valley-birds)
   * [Translations](#translations)
      * [i18n/default.json](#i18ndefaultjson)
* [Creating a static perch on a map](#creating-a-static-perch-on-a-map)
   * [Examples](#examples)
      * [Adding two roost perches](#adding-two-roost-perches)
* [Using the biomes system](#using-the-biomes-system)
   * [Specifying biomes on a custom map](#specifying-biomes-on-a-custom-map)
   * [List of biomes](#list-of-biomes)


# Creating a content pack

💡 **This documentation is a work in progress!**

## Complete example

See [`OrnithologistsGuild/assets/content-pack`](https://github.com/greyivy/OrnithologistsGuild/tree/main/OrnithologistsGuild/assets/content-pack) for a complete example.

Note that the mod has two build in content packs. All other content packs must be added to the game's `mods` folder with the prefix `[OG]` in the folder name.

## `manifest.json`

[Create your manifest](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest) as usual. Use prefix `[OG]` in your mod's name.

Add the following to your `manifest.json`:

```json
"ContentPackFor": {
    "UniqueID": "Ivy.OrnithologistsGuild"
}
```

## `content.json`

All content packs require a `content.json` file in their root.

| Key               | Description                                |
|-------------------|--------------------------------------------|
| `FormatVersion`   | Always `1`                                 |
| `Birdies`         | Array of [`Birdie object`](#birdie-object) |

## Birdie object

| Key                | Description                                                                                                                                                                                  | Required | Default                             |
|--------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------|-------------------------------------|
| `ID`               | Bird ID, unique to your mod                                                                                                                                                                  | `true`   |                                     |
| `AssetPath`        | Path to sprite                                                                                                                                                                               | `false`  | Stardew Valley `critters` tilesheet |
| `SoundAssetPath`   | Path to custom sound                                                                                                                                                                         | `false`  |                                     |
| `BaseFrame`        | Sprite base frame                                                                                                                                                                            | `false`  | `0`                                 |
| `BathingClipBottom`| Number of pixels to clip from the bottom of the sprite when the bird is bathing (see `CanBathe`)                                                                                             | `false`  | `8`                                 |
| `Attributes`       | Number of discoverable attributes (see [Translations](#translations))                                                                                                                        | `true`   |                                     |
| `CanUseBaths`      | Whether the bird should use bird baths (see `BathingClipBottom`)                                                                                                                             | `true`   |                                     |
| `MaxFlockSize`     | Maximum number of birds to spawn in a single flock                                                                                                                                           | `true`   |                                     |
| `Cautiousness`     | How close a player must in, in tiles, for a bird to frighten                                                                                                                                 | `true`   |                                     |
| `FlapDuration`     | Duration, in ms, between flaps                                                                                                                                                               | `true`   |                                     |
| `FlySpeed`         | Speed of bird flight (4 is relatively slow, 6 is relatively fast)                                                                                                                            | `true`   |                                     |
| `BaseWt`           | Number determining how likely bird is to spawn globally (1 being extremely likely, 0 being not spawned)                                                                                      | `true`   |                                     |
| `LandPreference`   | How likely the bird is to spawn on/relocate to land tiles. Set this to 0 for birds who should never be on the ground.                                                                        | `false`  | `1`                                 |
| `WaterPreference`  | How likely the bird is to spawn on/relocate to water tiles. Set this to 1 for birds who should swim in open water. This is separate from `CanUseBaths`.                                      | `false`  | `0`                                 |
| `PerchPreference`  | How likely the bird is to spawn on/relocate to perches. Set this to 0 for birds who should not perch.                                                                                        | `false`  | `0.5`                               |
| `FeederBaseWts`    | Object with feeder type keys and weight values determining how likely bird is to spawn nearby a feeder of this type (independant from `BaseWt` but added to `FoodBaseWts`)                   | `false`  | `{}`                                |
| `FoodBaseWts`      | Object with feeder type keys and weight values determining how likely bird is to spawn nearby a feeder containing food of this type (independant from `BaseWt` but added to `FeederBaseWts`) | `false`  | `{}`                                |
| `Conditions`       | Array of conditions affecting all of the above weights                                                                                                                                       | `true`   |                                     |
| `Conditions.When`  | [Content Patcher conditions](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/tokens.md#global-tokens)                                               | `true`   |                                     |
| `Conditions.AddWt` | Weight to add to `BaseWt`, `FeederBaseWts`, `FoodBaseWts` when condition is met                                                                                                              | `false`  |                                     |
| `Conditions.SubWt` | Weight to subtract to `BaseWt`, `FeederBaseWts`, `FoodBaseWts` when condition is met                                                                                                         | `false`  |                                     |
| `Conditions.NilWt` | Bird not spawned when condition is met                                                                                                                                                       | `false`  |                                     |

## Weight system examples

### Winter forest birds

- Spawn globally, but are somewhat rare
- Common at Hopper feeders with Fruit, somewhat rare at Platform feeders with Fruit
- Slightly more common in Winter in the `forest` [biome](#using-the-biomes-system)
- Significantly less common in Summer
- Do not spawn in `desert` and `void` [biomes](#using-the-biomes-system)

```json
{
    "BaseWt": 0.05,
    "FeederBaseWts": {
        "Hopper": 0.2,
        "Platform": 0.025
    },
    "FoodBaseWts": {
        "Fruit": 0.025,
    },
    "Conditions": [
        {
            "When": {
                "Season": "Winter",
                "Ivy.OrnithologistsGuild/LocationBiome": "forest"
            },
            "AddWt": 0.1
        },
        {
            "When": {
                "Ivy.OrnithologistsGuild/LocationBiome": "desert, void"
            },
            "NilWt": true
        },
        {
            "When": {
                "Season": "Summer"
            },
            "SubWt": 0.025
        }
    ]
}
```

### Vanilla Stardew Valley birds

Replicates the vanilla bird logic.

- Spawn globally and are very common
- Visit at all feeder types with all food types
- Do not spawn after 3 PM
- Do not spawn in Desert, Railroad, or Farm locations
- Do not spawn in Summer

```json
{
    "BaseWt": 0.5,
    "FeederBaseWts": {
        "Hopper": 0.25,
        "Tube": 0.25,
        "Platform": 0.25
    },
    "FoodBaseWts": {
        "Corn": 0.25,
        "Fruit": 0.25,
        "MixedSeeds": 0.25,
        "SunflowerSeeds": 0.25
    },
    "Conditions": [
        {
            "When": {
                "Time": "{{Range: 1500, 2600}}"
            },
            "NilWt": true
        },
        {
            "When": {
                "LocationName": "Desert, Railroad, Farm"
            },
            "NilWt": true
        },
        {
            "When": {
                "Season": "Summer"
            },
            "NilWt": true
        }
    ]
}
```

## Translations

### `i18n/default.json`

Replace `{ID}` with the `ID` of your birdie.

| Key                          | Description                                                             |
|------------------------------|-------------------------------------------------------------------------|
| `birdie.{ID}.commonName`     | Your bird's common name                                                 |
| `birdie.{ID}.scientificName` | Your bird's scientific name                                             |
| `birdie.{ID}.attribute.{N}`  | A short attribute like "plump body" where `{N}` is the attribute number |
| `birdie.{ID}.funFact`        | A fun fact about your bird!                                             |

# Creating a static perch on a map

Mod authors can add static perches to their maps. Birds will naturally land and roost on them!

Set **map** property `Perches` to a value in the following format:

`x y zOffset type` where `x` and `y` are tile coordinates, `zOffset` is a pixel offset on the Z axis to position a bird on the perch and `type` is currently always `0` (roost perch type). Multiple perches can be specified with a `/` separating them.

## Examples

### Adding two roost perches

Adds two roost perches at `(10,56)` and `(5,17)` with pixel a Z offset of `-18` pixels.

```json
{
    "Perches": "10 56 -18 0/5 17 -18 0"
}
```

# Using the biomes system

Biomes allow birds from all sources to spawn conditionally on vanilla maps *and* maps added by mods. For example, a mod author can add a desert themed map and have all desert birds spawn naturally.

Instead of spawning birds conditionally based on `LocationName`, we'll use `Ivy.OrnithologistsGuild/LocationBiome`. See [weight system examples](#weight-system-examples).

## Specifying biomes on a custom map

Mod authors can specify biomes on their own maps using the `Biomes` map property. The value should be a list of biomes seperated by a `/`.

For example, a map that is both a `forest` and a `wetland` biome would use `forest/wetland`.

If no biomes are specified, the `default` biome will be applied and a general set of birds will spawn.

## List of biomes

- `default` (unspecified biome)
- `void` (unhospitable biome)
- `farm`
- `forest`
- `city`
- `ocean`
- `wetland`
- `desert`
- `island` (not yet implemented)

Custom biome names can be used as well.
