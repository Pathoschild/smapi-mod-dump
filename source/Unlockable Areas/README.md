**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas**

----

[Nexusmods page](https://www.nexusmods.com/stardewvalley/mods/15132)

<b>Unlockable Areas</b> is a Framework that makes map overlays purchaseable, which can be used for bridges, quests, farm upgrades, shortcuts and much more.

How does it work?<br>
Just add "DeLiXx.Unlockable_Areas" in your manifest dependencies and append the UnlockableAreas/Unlockables asset.<br>
You can do that either using Content Patcher or the OnAssetRequested event in C#.<br>
* [Manifest Dependency](#manifest-dependency)
* [Unlockables](#unlockables)
    * [Full Example in ContentPatcher](#full-example-in-contentpatcher)
* [ContentPatcher  Token](#contentpatcher-token)
* [C# API](#c-api)
* [Custom Tile Properties](#custom-tile-properties)
<br>

# Manifest Dependency

Don't forget to specify Unlockable Areas as a dependency in your manifest.json!<br>
If you're using content patcher you will have to specify your mod as a ContentPatcher content pack.

```js
"ContentPackFor": {
    "UniqueID": "Pathoschild.ContentPatcher"
},
```
```js
"Dependencies":[
    {
        "UniqueID": "DeLiXx.Unlockable_Areas",
        "IsRequired": true
    }
]
```

# Unlockables

UnlockableAreas/Unlockables expects a Dictionary of strings and Unlockables, where the string is a unique identifier of the Unlockable<br>
Unlockables are defined as such:

| Property        |                                                                                                                                                                                                                                                                                                         |
|-----------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Location        | _String_<br><br>The Name of the GameLocation                                                                                                                                                                                                                                                                |
| ShopDescription | _String_<br><br>The text that is shown when opening a shop.<br>Use [i18n](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Translation) to provide translations if needed.                                                                                                                           |
| ShopPosition    | _String in the Format "x, y"_<br><br>The coordinates of the shop that is used to purchase the unlockable                                                                                                                                                                                                    |
| ShopTexture     | _(Optional) String_<br><br>The name of a 32px * 64px image asset.<br>It will draw at the tile at ShopPosition and the tile above.<br>If no ShopTexture is provided then the default UA Sign is used.                                                                                                        |
| ShopAnimation   | _(Optional) String in the Format "&lt;Frames&gt;@&lt;Milliseconds&gt;" (eg. "6@100")_<br><br>If you want to animate your Shop provide a `ShopTexture` that is 32px times frames wide.<br>Use `ShopAnimation` to specify how many frames your animation has and what delay in milliseconds between each frame you'd like. |
| ShopEvent | (Optional) String<br><br>A custom [Event Script](https://stardewvalleywiki.com/Modding:Event_data#Event_scripts) that will be played upon purchase.<br>This event will only be played for whoever bought the Unlockable.<br>Leave it empty for the default fade+hammer sounds event.<br>**`None`** will cause no event to be played at |
| Price           | _Dictionary<itemID as string, amount as int>_<br><br>The items required to purchase the Overlay.<br>The keyword **`Money`** can be used as an itemID.<br>You can find a list of item IDs [here](#https://docs.google.com/spreadsheets/d/1CpDrw23peQiq-C7F2FjYOMePaYe0Rc9BwQsj3h6sjyo/edit#gid=1082266757)<br>In case an item is spread accross multiple itemIDs, like with eggs, you can seperate the itemIDs by commas.                                                                                                                                                        |
| UpdateMap       | _String_<br><br>The name of a map asset that will be placed on top of `Location` at `UpdatePosition`<br><br>**`None`**  will ignore the map overlay. |
| UpdateType      | _(Optional) Default =_ **`Overlay`**<br><br>**`Overlay`** will place replace only where tiles overlap<br>**`Replace`** replaces everything from all layers that overlap between both maps, even when the overlay tile is empty                                                                                    |
| UpdatePosition  | _String in the Format "x, y"_<br><br>The top left coordinates of `Location` where `UpdateMap` will be overlayed towards the bottom right.                                                                                                                                                                             |

<br>

## Full Example in ContentPatcher

Is the table above too technical for you?<br>
No worries! This example contains a bit of everything.<br>
Feel free to just copy and paste!

```js
{
  "Format": "1.28.0",
  "Changes": [
	{
      "Action": "EditData",
      "Target": "UnlockableAreas/Unlockables",
      "Entries": {
        //This is where you'd put all your Unlockables
    "DeLiXx.ExampleUnlockable": { //Choose unique Unlockable identifiers!
      "Location": "Farm",
      "ShopDescription": "This is something I can buy!",
      "ShopTexture": "UnlockableAreas/ShopTextures/DeLiXx_ExampleUnlockable",
      "ShopAnimation": "6@200", // That's 6 frames at 200milliseconds per frame.
      "ShopPosition": "38, 9",
      "Price": {
        "Money": 25000,
        "388": 200, //388 is the ID of Wood
        "390": 35, //390 is the ID of Stone
      },
      "UpdateMap": "Maps/DeLiXx_ExampleUnlockable_Overlay",
      "UpdatePosition": "41, 52",
        },
      }
	},
	{ //This loads the overlay map file into the Maps/DeLiXx_ExampleUnlockable_Overlay asset for UpdateMap
		"Action": "Load",
		"Target": "Maps/DeLiXx_ExampleUnlockable_Overlay", //Choose unique asset names! 
		"FromFile": "assets/Overlay.tmx"
	},
	{ //This loads the newAnimatedShop image into the UnlockableAreas/ShopTextures/DeLiXx_ExampleUnlockable asset for ShopTexture
		"Action": "Load",
		"Target": "UnlockableAreas/ShopTextures/DeLiXx_ExampleUnlockable", //Choose unique asset names! 
		"FromFile": "assets/newAnimatedShop.png",
	},
  ]
}
```

# ContentPatcher Token

You can use the **DeLiXx.Unlockable_Areas/UnlockablePurchased** Token in your CP When conditions to check what Unlockables have already been purchased - or not.<br>
Like this for example:
```js
{
  "Format": "1.28.0",
  "Changes": [
	{
		"Action": "EditImage",
		"Target": "Maps/{{season}}_outdoorsTileSheet",
		"FromFile": "assets/YellowBush.png",
		"FromArea": { "X": 0, "Y": 0, "Width": 32, "Height": 32 },
		"ToArea": { "X": 208, "Y": 48, "Width": 32, "Height": 32 },
		"When": { "DeLiXx.Unlockable_Areas/UnlockablePurchased": "DeLiXx.ExampleUnlockable" }
	}
  ]
}
```

Please note, that your changes aren't taken into effect immediately, but instead the next time content patcher evaluates its conditions, which is on daystart by default.<br>
Due to this UA currently only fully refreshes the Unlockable shops on daystart

# C\# API

Unlockable Areas offers a simple API which you can use in your .NET based mods.<br>
Being able to use the API requires embedding two classes in your project and calling `Helper.ModRegistry.GetApi` after the game has launched.<br>
<br>
[IUnlockableAreasAPI](https://gitlab.com/delixx/stardew-valley-unlockable-areas/-/blob/main/Unlockable%20Areas/API/IUnlockableAreasAPI.cs)<br>
<br>
Example:<br>
```cs
public override void Entry(IModHelper helper)
{
  helper.Events.GameLoop.GameLaunched += gameLaunched;
}

private static void gameLaunched(object sender, GameLaunchedEventArgs e)
{
  if (Helper.ModRegistry.IsLoaded("DeLiXx.Unlockable_Areas")) {
    var unlockableAreasAPI = Helper.ModRegistry.GetApi<IUnlockableAreasAPI>("DeLiXx.Unlockable_Areas");
    unlockableAreasAPI.shopPurchasedEvent += onShopPurchased;
  }
}

private void onShopPurchased(object source, ShopPurchasedEventArgs e)
{
  if(e.unlockableKey != "myUnlockableKey")
    return;

  //My Code goes here
}
```

# Custom Tile Properties

| Layer | Property     | Explanation                                                                                                     |
|-------|--------------|-----------------------------------------------------------------------------------------------------------------|
| Back  | UA_NoGrass T | Prevents the placement of Grass Starters, Paths/Floors and Torches that are ignored by the `Placeable` property |
