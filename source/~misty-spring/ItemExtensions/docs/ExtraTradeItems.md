**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Extra trade items

In vanilla, you can only add 1 trade item. This mod allows for multiple.

## Contents

* [How to add](#how-to-add)
  * [Example](#example)
  * [Edit another shop](#edit-someone-elses-shop)
* [Tips](#tips)

---

## How to add

You can add extra trades by editing the salable's custom fields.

Just add this to your shop Item's `CustomFields`:  `mistyspring.ItemExtensions/ExtraTrades` . This must state ID and count.

**Note:** The item must also have a regular "trade item id". This is for _extra_ requirements.

### Example

In this example, we add two extra trade requirements. **ShopData's required fields were ommitted for readability**.

Here, Pineapple seeds will require: 5 sap, 1 banana, and 2 weeds.

```json
{
  "Action": "EditData",
  "Target": "Data/Shops",
  "Entries": {
    "MyCustomShop": {
      "Items":[
        {
          "Id": "Pineapple seeds",
          "ItemId": "(O)833",
          "TradeItemId": "(O)92", 
          "TradeItemAmount": 5, 
          "CustomFields": {
            "mistyspring.ItemExtensions/ExtraTrades": "(O)91 1 (O)0 2"
          }
        }
      ]
    }
  }
}
```

As a result, whenever you try to buy the seeds (in MyCustomShop), it'll require the extra items. (They'll also be shown on hover, and the trade will fail if you don't have all.)


## Edit someone else's shop

To edit someone else's shop, you must use contentpatcher's text operations.

You also need to know the salable's Id.
(For example: Clint's ore shop. During Y1, he sells copper cheaper: Y1 copper's id is `CopperOre_YearOne`.)

Here's an example:

Let's say SomeoneElsesShop sells golden coconuts, and you want to add more requirements. Like before, we need to know its sale Id.

```jsonc
{
  "Action": "EditData",
  "Target": "Data/Shops",
  "TargetFields": [
    "SomeoneElsesShop",     // we're targetting the shop
    "Items",                // 's items
    "The_sale_id",          // and this item's
    "CustomFields"          // custom fields
  ],
  "Entries": {
    "mistyspring.ItemExtensions/ExtraTrades": "(O)88 2 (O)831 1"
  }
}
```

Here, any time `SomeoneElsesShop` sells that salable, it will also require 2 coconuts and a taro tuber.

## Tips

You must state the item count, even if it's 1.


For better readability, you can separate the items by comma, or by a double space.
Like this:

```jsonc
"mistyspring.ItemExtensions/ExtraTrades": "(O)91 1, (O)0 2"
```

Or this:
```jsonc
"mistyspring.ItemExtensions/ExtraTrades": "(O)91 1  (O)0 2"
```
