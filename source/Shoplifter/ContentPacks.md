**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TheMightyAmondee/Shoplifter**

----

# Creating Content Packs for Shoplifter

Content Packs can be created to allow for custom shops added using Content Patcher to be made shopliftable. Content Packs for Shoplifter require two files, a ``manifest.json`` and a ``shopliftables.json``. See [here](https://github.com/TheMightyAmondee/Shoplifter/blob/master/shopliftables.json) for a full example ``shopliftables.json``.

## manifest.json
---
The ``manifest.json`` tells SMAPI that your content pack is readable by Shoplifter. It is similar in format to other Content Pack manifest files. However, in the ``ContentPackFor`` section the ``UniqueID`` should have the value of ``TheMightyAmondee.Shoplifter``.

For example:
```json
{
    "Name": "Test",
    "Author": "TheMightyAmondee",
    "Version": "1.0.0",
    "Description": "...",
    "UniqueID": "TheMightyAmondee.Test",
    "UpdateKeys": [], 
    "ContentPackFor": {
        "UniqueID": "TheMightyAmondee.Shoplifter"
    }
}
```

## shopliftables.json
---
The ``shopliftables.json`` is the important one. Here, new shopliftable shops are defined. The ``shopliftables.json`` is made up of a list of ``MakeShopliftable`` where shopliftable shops are defined.

Each entry in ``MakeShopliftable`` has a few required and non-required fields, see below!

Field | Type | Required? | What it does | Notes
------|------|-----------|--------------|------
UniqueShopId | string | Yes | A unique identifier for the shop, anything unique will do! | -
ShopName | string | Yes | The id of the shop, as defined by the game in Data\Shops or by a custom shop, the mod uses this to get available stock | -
CounterLocation | ``ShopCounterLocation`` | Yes | Where the counter for the shop is located (the tile location to click on to open the store) | See ShopCounterLocation model below
ShopKeepers | List of strings | Yes | A list of all the shopkeepers' names (anyone who can catch, fine and/or ban the player) | -
CaughtDialogue |  Dictionary<string, string> | No | Unique dialogue for the shopkeeper to say when the player is caught. | See Unique Dialogue below
OpenConditions | ``ShopliftableConditions`` | No | Under what conditions the store is normally open and items can be purchased | See ShopliftableConditions model below
MaxStockQuantity | int | No | The maximum number of different stock items that can appear in each shoplift attempt | Default value is 1
MaxStackPerItem | int | No | The maximum stack size of each stock item | Default value is 1
Bannable | bool | No | Whether the player can be banned from the shop | Default value is false. I wouldn't recommend outdoor shops be bannable, since players will no longer be able to enter that outdoor area if banned.

## ShopCounterLocation model
---
This model describes where the store is located (what tile of what map must be clicked on to open the store)

Field | Type | Required? | What it does | Notes
------|------|-----------|--------------|------
LocationName | string | Yes | The location name where the shop is located | This is also the location the player won't be able to enter if banned.
NeedsShopProperty | bool | No | Whether the counter tile needs the OpenShop property for the shop to exist | Default value is true
TileX | int | Yes | The X tile coordinate of the shop counter | -
TileY | int | Yes | The Y tile coordinate of the shop counter | -

## ShopliftableConditions model
---
This model describes when the store is normally open. 
If any named condition is false the shop will be considered shopliftable.

Field | Type | Required? | What it does | Notes
------|------|-----------|--------------|------
Weather | List of strings | No | Under what weather conditions the store is open | By default store is considered open in all weather conditions
GameStateQueries | List of strings | No | A list of [Game State Queries](https://stardewvalleywiki.com/Modding:Game_state_queries) that state when the store is open | WEATHER queries won't work as intended for indoor locations. Use the Weather field to check weather conditions
ShopKeeperRange | List<``ShopKeeperConditions``> | No | The defined range each shopkeeper must be within the store for it to be considered open | See ShopKeeperConditions model below

Any conditions not defined will not count towards determining shop accessibility.
If no conditions are defined, shop is considered always open and not shopliftable (not very useful).

## ShopKeeperConditions model
---
This model describes when shopkeepers are present at the store (in other words, able to sell items).
Fields describe a rectangle area on the map that the named shopkeeper must be present in for the shop to be accessible. This would generally be the tile behind the shop counter, but can be anything you want.

Field | Type | Required? | What it does | Notes
------|------|-----------|--------------|------
Name | string | Yes | The name of the shopkeeper | -
TileX | int | Yes | The X tile coordinate of the upper left rectangle point | -
TileY | int | Yes | The Y tile coordinate of the upper left rectangle point | -
Width | int | No | The width of the rectangle the shopkeeper must be within | Default is 1
Height | int | No | The height of the rectangle the shopkeeper must be within | Default is 1

## Unique Dialogue
---
Shopkeepers can be given unique dialogue to say when the player is caught shoplifting using ``CaughtDialogue``. Each shopkeeper can have two different entries, one for when the player is fined, and one for when the player cannot afford the fine. Both, either or none may be specified. Shopkeepers will use generic dialogue if the appropriate dialogue is not defined. 

Each entry in ``UniqueDialogue`` is in the form of a key value pair (``"<key>" : "<value>"``) with entries separated by a comma.  The key should be the name of the shopkeeper (dialogue when fined) or in the form ``"<shopkeepername>_NoMoney"`` (dialogue when not fined) where ``<shopkeepername>`` is the name of the shopkeeper. Dialogue may be added in two ways, first by having dialogue directly in the ``shopliftables.json`` or by using translation tokens. I would recommend using translation tokens.

### Adding dialogue directly
When adding dialogue directly to the ``shopliftables.json`` The value for each entry should be the dialogue for the respective shopkeeper to say. In the case of dialogue for when the player is fined, the text {0} will be replaced with the fine amount. Dialogue replacers and commands will also work e.g @ being replaced with the farmer's name.

See below for an example which gives Harvey unique caught dialogue in both cases:
```
"CaughtDialogue": 
{ 
"Harvey" : "I'm fining you {0}g for this @.", 
"Harvey_NoMoney" : "You can't afford a fine right now." 
}
```

### Using translation tokens (recommended)

While a little more complicated, using translation tokens allows for the dialogue to be translated into other languages more easily. All translation files should be stored in a ``i18n`` folder within your content pack.

For the translation file format, see [i18n folder](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Translation#i18n_folder) on the wiki.

Translations are used in unique dialogue by using the ``{{i18n: <key>}}`` token in the value of each ``CaughtDialogue`` entry, replacing ``<key>`` with the key used in the translation files. For dialogue where the player is fined, the token ``{{fineamount}}`` can be included in the translation. This will be replaced by the fine amount. Dialogue replacers and commands will also work e.g @ being replaced with the farmer's name.

See below for an example which gives Harvey unique caught dialogue in both cases using translation tokens:

In ``shopliftables.json``:
```
"CaughtDialogue":{
                "Harvey":"{{i18n:TheMightyAmondee.TestShop.Harvey}}",
                "Harvey_NoMoney":"{{i18n:TheMightyAmondee.TestShop.Harvey_NoMoney}}"
                }
```

In ``i18n\default.json``:
```
{
    "TheMightyAmondee.TestShop.Harvey":"I'm fining you {{fineamount}}g for this @",
    "TheMightyAmondee.TestShop.Harvey_NoMoney":"You can't afford a fine right now."
}
```

See the [translation page](https://stardewvalleywiki.com/Modding:Translations) on the wiki for more information on translating.


