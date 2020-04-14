# Roadmap

Read this document to peer into the future of these very mods! Spooky.

See the [issue tracker](https://gitlab.com/kdau/predictivemods/-/issues) for bug fixes and minor enhancements.

---

## Shopping channel/experience

### `PredictiveCore.Shopping`

* `ListSalesForDate`
* `ListKrobusForDate`
* `ListPierreForDate`
* `ListSandyForDate`
* `ListTravelingCartForDate`

### `PublicAccessTV.ShoppingChannel`

* name: "Shop the Valley"
* host: Lewis
* condition: two hearts with Lewis
* introductory event: Lewis and Traveling Cart merchant discuss business climate in Cindersap Forest
* content:
	* today's wallpaper and flooring at Pierre's General Store
	* today's shirt at Sandy's Oasis (if bus repaired)
	* today's items at the Traveling Cart (on Fridays and Sundays)

### `ScryingOrb.ShoppingExperience`

* offering categories: Animal Product, Artisan Goods, Cooking or Flower (quantities?)
* offering exceptions:
	* Fried Egg, Salmonberry Wine, Tulip (too cheap; or else require higher quantities)
	* Magic Rock Candy, Void Egg or Void Mayonnaise (used by other experiences)
* input: date
* content: as for "Shop the Valley", plus Krobus's Fish/Cooking item (if Krobus met)

---

## Tailoring channel

*Ideally to release together with shopping.*

### `PredictiveCore.Tailoring`

* `ChooseRandomRecipe` (for the sewing machine)
* `ChooseRandomOutfit` (of a villager)

### `PublicAccessTV.TailoringChannel`

* name: "Fashion Showcase"
* hosts: Emily and Haley
* conditions:
	* sewing machine access
	* two hearts with Emily
	* two hearts with Haley
* introductory event: Emily and Haley discuss their common interests at home by the sewing machine
* content:
	* random tailoring recipe
	* fashion commentary from Haley (connect to mods like Seasonal Villager Outfits?)

---

## Item Finder experience

### `PredictiveCore.ItemFinder`

* `FindItem`
* `FindItemByCombat`
* `FindItemByCooking`
* `FindItemByCrafting`
* `FindItemByFarming`
* `FindItemByFishing`
* `FindItemByForaging`
* `FindItemBySpecial`

### `PredictiveCore` supporting methods

* `Garbage.FindItemInGarbage`
* `Geodes.FindItemInGeodes`
* `NightEvents.FindItemByNightEvents`
* `Mining.FindItemByMining`
* `Shopping.FindItemByShopping`
* `Tailoring.FindItemByTailoring`

### `ScryingOrb.ItemFinderExperience`

* offering categories: Fruit, Vegetable or Forage (quantities?)
* offering exceptions: Sap, Salmonberry, Blackberry or Spring Onion
* criterion: any item (needs UI; ref. CJB Item Spawner?)
* content:
	* days, stores and prices when item will be sold
	* numbers and types of upcoming geodes to contain item
	* days and garbage cans when item will be trashed
	* cooking, crafting and tailoring recipes for item
	* mining, combat, foraging and fishing drops of item
	* farming of item from seeds/trees/animals
	* special ways of getting item

---

## Future ideas

### Public Access TV channels (non-predictive)

* Arts (hosts: Elliott, Evelyn, Leah)
* Crafting (hosts: Maru, Robin)
* Desert (hosts: Pam, Sandy)
* History (hosts: Gunther, Penny)
* Music (hosts: Abigail, Sam, Sebastian)
* Ranching (hosts: Marnie, Shane)
* Wellness (hosts: Alex, Caroline, Harvey)

---

## Not planned

### Public Access TV channels

* ~~Geodes~~ (who would know this in advance?)
* ~~Item Finder~~ (wouldn't fit with TV interface)

### Scrying Orb experiences

* ~~Movies~~ (why would the spirits care?)
* ~~Tailoring~~ (no future component)
* ~~Trains~~ (Demetrius already knows the future schedule)
