**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Custom game state queries
## Contents
* [Items worn](#items-worn)
* [Tool upgrade level](#tool-upgrades)

-------------

## Items worn
`mistyspring.dynamicdialogues_PlayerWearing <player> <type> <item> [only worn]`

  Checks if the player is wearing the given item. This can be a hat, shirt, pant, or shoe.

### Parameters
| name      | required | alias                                                                                            |
|-----------|----------|--------------------------------------------------------------------------------------------------|
| player    | yes      | The [target player](https://stardewvalleywiki.com/Modding:Game_state_queries#Target_player).     |
| type      | yes      | The [item type](https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Item_types). |
| item      | yes      | The item ID.                                                                                     |
| only worn | no       | If it should ignore display type\*. Default false.                                               |

`*` = Sometimes, the pants/shirt display is different from the one worn. In these cases, the query will prioritize the displayed one (unless this value is true).

### Valid item types

| name | alias |
|------|-------|
| (H)  | Hat   |
| (S)  | Shirt |
| (R)  | Ring  |
| (P)  | Pants |
| (B)  | Boots |

### Examples
 
- `mistyspring.dynamicdialogues_PlayerWearing Current Hat CustomHat` 

This will check if current player's hat has ID CustomHat.


- `mistyspring.dynamicdialogues_PlayerWearing Current (S) MyCustomShirt false`

This will check if current player is wearing a shirt with ID MyCustomShirt. If the displayed shirt is a different one, it'll be ignored.
