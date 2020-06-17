# ![[icon]](promo/icon.png) East Scarpe SMAPI component

This is the SMAPI (C#) component of LemurKat's East Scarpe mod. This component is written by kdau.

## Specification

### Water

Tidepool water should not shimmer like other water does.

The water should turn green on Summer 12, Summer 13 and Summer 14.

### Grass

The grass should survive through the winter with appropriate seasonal sprites.

### Fishing

Divide the map into three fishing subareas: freshwater pond, tidepools and ocean. Freshwater pond and ocean are separated by a Y coordinate threshold. Tidepools are distinguished by using specific tile indexes.

### Crab Pots

Crab Pots placed in the tidepool and ocean subareas should yield saltwater rather than freshwater catches.

### Critters

Seagulls should spawn in a cluster of zero to three, with the same logic as on the vanilla beach map, but only on the beach portion of the map (same as the ocean fishing portion). Reskinning to pelicans will be accomplished through CP instead.

### Sea Monster

The Sea Monster should make occasional appearances in the ocean, with the same probability and logic as on the vanilla beach map.

### Orchard

On the `ESOrchard` map, there should be a one-time spawn of fruit trees based on the sapling indexes given as values of the `FruitTree` property on the `Back` layer. The trees should spawn as fully mature, with in-season trees already bearing one fruit. Fruit trees on this map should not be allowed to bear more than on fruit at a time.

## Release notes

### Version 1.4.0

* Add Crab Pot fishing area support.
* Add Orchard fruit tree spawns.

### Version 1.1.3

* Avoid additional errors when map fails to load.

### Version 1.1.0

* All constants are now in a `data.json` file for easier editing.
* Any grass on the map is now preserved through the winter with appropraite sprites.

### Version 1.0.0

* Initial release.
