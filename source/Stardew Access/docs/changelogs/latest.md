**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/stardew-access/stardew-access**

----

## Changelog

### New Features


### Feature Updates

- Farm animal's age is spoken in days if it is less than a month old.
- Animal's info include the baby and hungry statuses.
- Animal purchase menu:
    - The menu will now speak the "Select Building" prompt along with the animal's colour.
    - It will also speak the animal names as they are shown. Previously, it spoke white cow as opposed to what's shown, dairy cow.
- Added custom names support for items. This can be used to have separate names for items with same names (like for jungle decals purchasable in Luau). At the moment the scope is only at the inventory level or wherever InventoryUtils is used to get the plural form of a name.
- Inventory slots that haven't been unlocked yet now appropriately read as "Locked Slot" instead of "Empty Slot"; #361

### Bug Fixes

- Removed duplicate entries of entrances in bath house women's locker room
- Updated location of soup pot in Luau festival
- Fixed glitchiness in Pierre's shop in the Luau festival; #318
- Fixed other player's mail boxes showing up in buildings category
- Fix duplicate animal category and out of bounds animals being tracked in farms.
- Fixed support for medowlands and custom farm types in the custom tile entry menu.
- Fixed descriptive flooring names not being read. Introduced a new config, `DisableDescriptiveFlooring`; #362
- Fixed green rain weeds being categorized as other instead of debris; #365

### Translation Changes

- New(en.ftl): `event_tile-luau-pierre_booth` with English value `Pierre's Booth`
- Modified(en.ftl): `npc-farm_animal_info` added `is_hungry`, `is_baby` and `is_age_in_days` attributes, look [here for updated english translation](https://github.com/khanshoaib3/stardew-access/blob/a33d90157baa532e09f45d72bed91ff53a601649/stardew-access/i18n/en.ftl#L333-L348)
- New(en.ftl): `tile-town-bookseller` with english value `Bookseller`
- Removed(static_tiles.en.ftl): `static_tile-town-bookseller`
- Modified(menu.en.ftl): `menu-animal_query-animal_info` added `is_age_in_days` attribute, look [here for updated english translation](https://github.com/khanshoaib3/stardew-access/blob/a33d90157baa532e09f45d72bed91ff53a601649/stardew-access/i18n/menu.en.ftl#L345-L367)
- New(en.ftl): `dynamic_tile-farm-lumber_pile` with english value `Lumber Pile`.
- Removed(static_tiles.en.ftl): `static_tile-farm-lumber_pile`.
- New(en.ftl): `inventory_util-special_items-name` with placeholder values at the moment.
- New(en.ftl): `inventory_util-locked_slot` = `Locked Slot`
- New(en.ftl): `inventory_util-enchantments-galaxy_soul` = `Galaxy Soul ({$progress_in_percentage}% transformed)`
- Modified(menu.en.ftl): `menu-forge-start_forging_button` = [English value](https://github.com/khanshoaib3/stardew-access/blob/499637832b0801a75c4435517e0420c08a06bbeb/stardew-access/i18n/menu.en.ftl#L260-L263)

### Tile Tracker Changes

- Bookseller's tile is now dynamically tracked.
- Detect Pierre's Booth tile in Luau festival.
- Detect lumber pile dynamically.

### Guides And Docs


### Misc

- Added pull request template
- ci: As opposed to `/fast-forward`, we can also now use `/fast-forward-force` to merge the PR without checking for `mergeable_state`.
- ci: Fix if condition failure in fast-forward.yml
- Added progress information for infinity conversion (galaxy soul enchantment); #239
- "Start Forging" button in the forge menu now also speaks the forge cost.
- The special orders board menu now correctly indicates when a quest is completed; #228

