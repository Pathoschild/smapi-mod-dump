**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Player conditions
Extra conditions related to the player. Used in notifications and (framework) NPC dialogue.

## Parameters

| name           | type   | description                            |
|----------------|--------|----------------------------------------|
| Hat            | string | Hat ID.                                |
| Shirt          | string | Shirt ID.                              |
| Pants          | string | Pants ID.                              |
| Boots          | string | Boots' ID.                             |
| Rings          | string | Ring ID. Can be multiple.\*            |
| Inventory      | string | Items in inventory. Can be multiple.\* |
| GameStateQuery | string | A [game state query](https://stardewvalleywiki.com/Modding:Game_state_queries).                |


------------

## Using player conditions

You can add extra conditions for the dialogue/notification.

For example, with these conditions:
```
"Conditions": {
          "Shirt": "1009",
          "Rings": "520 OR 521"
}
```
The dialogue will *only* happen if player is wearing shirt ID 1009 and rings 520 or 521.

All conditions support custom items.

## Multiple items (rings/inventory)
Some fields -specifically, rings and items- accept multiple item conditions. They can be either "AND" "OR".

For example:
```
"Conditions": {
          "Inventory": "(O)680 OR (O)413 OR (O)437"
}
```

Here, the dialogue will happen if you have any of these objects: 680, 413, or 437.

Unlike rings, <u>**Inventory** requires **qualified ID.**</u>

<br>

You can also specify "either AND" by using quotes in the condition:
```
"Conditions": {
          "Inventory": "\"(O)182 AND (W)9\" OR (F)3"
}
```
The mod will interpret this as "player must either have `(O)182 AND (W)9` or `(F)3`".

There's no limit to how many items you can require.
