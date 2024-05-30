**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/stardew-access/stardew-access**

----

## Changelog

### New Features


### Feature Updates

- Detect Pierre's Booth tile in Luau festival.
- Farm animal's age is spoken in days if it is less than a month old.
- Animal's info include the baby and hungry statuses.
- Animal purchase menu:
    - The menu will now speak the "Select Building" prompt along with the animal's colour.
    - It will also speak the animal names as they are shown. Previously, it spoke white cow as opposed to what's shown, dairy cow.

### Bug Fixes

- Removed duplicate entries of entrances in bath house women's locker room
- Updated location of soup pot in Luau festival
- Fixed glitchiness in Pierre's shop in the Luau festival; #318
- Fixed other player's mail boxes showing up in buildings category
- Fix duplicate animal category and out of bounds animals being tracked in farms.

### Translation Changes

- (en.ftl) new: `event_tile-luau-pierre_booth` with English value `Pierre's Booth`

- (en.ftl) Modified: `npc-farm_animal_info` added `is_hungry`, `is_baby` and `is_age_in_days` attributes, look [here for updated english translation](https://github.com/khanshoaib3/stardew-access/blob/a33d90157baa532e09f45d72bed91ff53a601649/stardew-access/i18n/en.ftl#L333-L348)
- (en.ftl) Added: `tile-town-bookseller` with english value = `Bookseller`
- (static_tiles.en.ftl) Removed: `static_tile-town-bookseller`
- (menu.en.ftl) Modified: `menu-animal_query-animal_info` added `is_age_in_days` attribute, look [here for updated english translation](https://github.com/khanshoaib3/stardew-access/blob/a33d90157baa532e09f45d72bed91ff53a601649/stardew-access/i18n/menu.en.ftl#L345-L367)

### Tile Tracker Changes

- Bookseller's tile is now dynamically tracked.

### Guides And Docs


### Misc

- Added pull request template
- ci: As opposed to `/fast-forward`, we can also now use `/fast-forward-force` to merge the PR without checking for `mergeable_state`.

