**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/DecidedlyHuman/StardewValleyMods**

----

# Template

# Checklist (remove as completed)

# x.x.x — Changes

### UI

* Test opening/closing UI.
* Test moving UI.
* Test resizing window with UI close to out of bounds.
* Test UI interaction with game menus.
* Test UI interaction with festivals/events.
* Test UI button states.

### Interaction blocking

* Placement
* Chest opening
* Fish pods
* Storage furniture

### Misc

* Ensure every setting is registered in GMCM
* Interactions with menus
* Ensure debug controls are disabled by default
* Leftover debug gubbins
* Version bump

### I18n Translations

* Ensure no untranslated strings are present in the source
* Ensure other languages (zh right now) function correctly

### Mod Integration

* More Fertilizers — "morefertilisers"
    * Ensure all non-HoeDirt fertilisers place correctly
    * Ensure no unforeseen consequences without More Fertilizers installed
    * Ensure this doesn't interfere with vanilla/other fertilisers

### Known prior bugs. Ensure no regression — "regression"

* Scaling issues
    * Build HUD
    * Items drawn in-world
* Auto-grabbers giving broken chest item when picking them up using Smart Building
* Trees that have their tappers removed with Smart Building being indestructible to an axe

### Object and producer identification — "identification"

* Vanilla
    * Fish tank furniture
    * Storage furniture
    * Bed furniture
    * TV furniture
    * Generic furniture
    * Floors/paths
    * Chests
    * Fences
    * Grass starters
    * Crab pots
    * Seeds
    * Tree fertilisers
* Modded (where applicable)
    * Fish tank furniture
    * Storage furniture
    * Bed furniture
    * TV furniture
    * Generic furniture
    * Floors/paths
    * Chests
    * Fences
    * Grass starters
    * Crab pots
    * Seeds
    * Tree fertilisers
    *

### Flooring — "flooring"

* More lax setting **off**
    * Placement
    * Under fences placement
    * Crazy placement
    * Removal
* More lax setting **on**
    * Placement
    * Under fence placement
    * Crazy placement
    * Removal

### Furniture — "furniture"

* More lax setting **off**
    * Placement
    * Removal
    * Crazy placement
    * Functionality (lighting, etc.)
* More lax setting **on**
    * Placement
    * Removal
    * Crazy placement
    * Functionality (lighting, etc.)

### TV Furniture — "tv"

* More lax setting **off**
    * Placement
    * Removal
    * Crazy placement
    * Functionality (lighting, etc.)
* More lax setting **on**
    * Placement
    * Removal
    * Crazy placement
    * Functionality (lighting, etc.)

### Storage furniture — "storage"

* More lax setting **off** and storage furniture placement **off** (Expected: un-placeable on all accounts)
    * Placement
    * Removal
    * Crazy placement
    * Functionality (lighting, etc.)
* More lax setting **on** and storage furniture placement **off** (Expected: un-placeable on all accounts)
    * Placement
    * Removal
    * Crazy placement
    * Functionality (lighting, etc.)
* More lax setting **off** and storage furniture placement **on** (Expected: placeable)
    * Placement
    * Removal
    * Crazy placement
    * Functionality (lighting, etc.)
* More lax setting **on** and storage furniture placement **on** (Expected: placeable)
    * Placement
    * Removal
    * Crazy placement
    * Functionality (lighting, etc.)

### Bed furniture — "bed"

* More lax setting **off**
    * Placement
    * Removal
    * Crazy placement
    * Functionality (lighting, etc.)
* More lax setting **on**
    * Placement
    * Removal
    * Crazy placement
    * Functionality (lighting, etc.)

### Furniture catalogue?

* Does... do bad things? Expect more info to be added later!
    * Actually seems fine. Leave this here for now just in case, though.

### Chests — "chests"

* Chest removal setting **off**
    * Placement
    * Removal
* Chest removal setting **on**
    * Placement
    * Removal

### Trees — "trees"

* Regular
    * Tree fertiliser setting **off**
        * Planting trees
        * Fertilising trees
    * Tree fertiliser setting **on**
        * Planting trees
        * Fertilising trees
    * Tree tapper setting **off**
        * Tapping
        * Tapper removal
    * Tree tapper setting **on**
        * Tapping
        * Tapper removal
* Fruit
    * Crop planting setting **off**
        * Planting
    * Crop planting setting **on**
        * Planting

### Crops — "crops"

* Crop planting setting **off**, and fertiliser placement setting **off**
    * Planting seeds
    * Planting fertilisers
* Crop planting setting **on**, and fertiliser placement setting **off**
    * Planting seeds
    * Planting fertilisers
* Crop planting setting **off**, and fertiliser placement setting **on**
    * Planting seeds
    * Planting fertilisers
* Crop planting setting **on**, and fertiliser placement setting **on**
    * Planting seeds
    * Planting fertilisers
* DGA crops

### Fences — "fences"

* Replacement setting **off**
    * Placement
    * Removal
    * Replacement
* Replacement setting **on**
    * Placement
    * Removal
    * Replacement

### Modded objects — "modded"

* Placement
* Removal

### Item insertion — "insertion"

* Item insertion setting **off**
    * Insertion
    * Deduction of inserted items
* Item insertion setting **on**
    * Insertion
    * Deduction of inserted items

### Item insertion (modded; PFM)

* Item insertion setting **off**
    * Insertion
    * Deduction of inserted items
* Item insertion setting **on**
    * Insertion
    * Deduction of inserted items

### Torches — "torches"

* Placing normally
* Placing on/in fences

### Rectangle drawing — "rectangle"

* Refunding correctly when cancelled
* Deducting correct amount of items for drawing
* Queue clearing correctly
    * On screen transition
    * On build mode being cancelled