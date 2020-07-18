# Forages

## Where are forages defined

Look into `assets/Data/Forages.json`, target name is `Data/Forages` for patching by content packs.

## The definition

Forages definition is simple key-value (string-string) dictionary. In the valuze is a string definition contains indexes (numbers) of items in Stardew Valley game. Which item has which id you can found in the original game content file `Content/Data/ObjectInformation.xnb` (for read it you must unpack this file. See the SDV modding wiki how to do it.)

**NOTE:** You can't use items added by JSON Assets, because this mod uses item names and generates dynamic ids. NPC Adventures currently supports only static id numbers for items. JSON assets support will be implemented soon.

### Example

```js
{
  "summer": "396 398 402",
  "fall": "404 406 408 410",
  "winter": "283 412 414 416 418",
  "Desert": "88 90",
  "Beach": "372 392 393 394 397 718 719 723",
  "Woods_fall": "281 420",
  "Forest_spring": "16 18 20 22 399"
}
```

## Regular forages

You can define regular forages for these categories: 

- Seasson based forages
- Location based forages
- Combination of both

#### Lookup

Mod try to find forage for random select in reverse sort of these categories.

For example:

Is *spring* and we are in the *Forest* location.

- First lookup if exists definition for key `Forest_spring`
- If the previous key not exists, then mod lookup if there are defined key `Forest`.
- If no key defined for the `Forest` location, then try to find key `spring`
- If no key for `spring` is defined, then fallback and returns default `-1` (means no item found)

### Location based forages

Is a forages which can be found by your companion only in that specified locations. Location name must start with capital.

For example: Coconut (88) and Cactus (90) can be found only in Desert.

```js
{
  "Desert": "88 90"
}
```

### Seasson based forages

You can define in which seasson which forages can be found by your companion. There are available these seassons: *spring*, *summer*, *fall* and *winter*. All seasson names are in lowercase.

For example: Forages Wild Horseradish, Daffodil, Leek and Dandelion can be found only in spring.

```js
{
  "spring": "16 18 20 22"
}
```

### Combined seasson and location forages.

You can specify which item can be found in concrete location in concrete seasson.

For example: Spring onion (399) can be found only in *Forest* location only in *spring*. All regular spring forages can be found here too.

```js
{
  "Forest_spring": "16 18 20 22 399"
}
```

## Special forages

There are special keys for special forages.

- Cave forages (key `cave`)
- Rare forages (key `rare`)
- Farm type forages (key `farmType_*`)

### Cave forages

Cave forages is a special location based forages. These forages can be found only in all caves (Mines and/or skull cavern). This magic category cover all mineshafts and modder don't must specify copy-pasted forage items for every mineshaft location. Modder can't specify special forage for concrete mineshaft level.

Example of cave forages:

```js
{
  "cave": "78 420 422"
}
```

### Rare forages

This kind of forages can be found only by companion shake the bush. There are 0.005% chance to found a rare forage. Farmer must have **foraging skill level 5** or higher.

Example of rare forages definition:

```js
{
  "rare": "347 114 815"
}
```

### Farm type forages

These forages can be found only on farm based on which farm type player selected while creating a new game. These forages can be conditioned on these factors:

- Which farm type
- Current seasson
- Combination of both

Uses the same logic for lookup and select which forage like in location based forages, but all values under found keys will be concatenated and then selected random item.

Available farm types: `Farm` (the standard farm), `Farm_Fishing`, `Farm_Foraging`, `Farm_Mining`, `Farm_Combat`, `Farm_FourCorners`.
Available seassons: `spring`, `summer`, `fall` and `winter`.

Example of farm type based forages definition:

```js
{
  "farmType_Farm_spring": "273 597 24 591",
  "farmType_Farm_summer": "431 433 376",
  "farmType_Farm_fall": "280 595",
  "farmType_Farm_Fishing": "685",
  "farmType_Farm_Foraging_spring": "16 18 20 22 257 404",
  "farmType_Farm_Foraging_summer": "396 398 402 259 420",
  "farmType_Farm_Foraging_fall": "404 406 408 410 281 420",
  "farmType_Farm_Mining": "378 380 380 2 382",
  "farmType_Farm_Combat": "766 767"
}
```

## Quality categories

This special key in the forages definition dictionary says which items from which item categories can be found as higher quality than normal. Items from thesecategories can be found as normal, silver, gold or iridium quality. Other items from categories which is not enlisted under this key can be found only as normal quality on the world.

```js
{
  "qualityCategories": "-4 -5 -6 -14 -26 -27 -74 -75 -79 -80 -81"
}
```

## Define forages in custom content packs

In your file for define forages (like `assets/myForages.json`) define your custom forages. In most cases you can probably want to define custom forages for your custom specific location. You can only use static id numbers (from original game or from mods which define custom items under the static id number).

```js
// File `assets/myForages.json`
{
  "MyCustomLocation": "18 20 88",
  "MyCustomLocation_summer": "402 394 397"
}
```

And in `content.json` in your NPC Adventures content pack to the `Changes` section add:

```js
{
  "Target": "Data/Forages",
  "FromFile": "assets/myForages.json"
}
```
