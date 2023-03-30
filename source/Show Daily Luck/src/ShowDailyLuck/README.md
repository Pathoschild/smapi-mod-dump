**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/arphox/StardewValleyMods**

----

# See the [mod's Nexus page](https://www.nexusmods.com/stardewvalley/mods/15910).

**This file is only for development notes.**

---

# Show Daily Luck

## Implementation notes
#### Cached (static) TV instance
The implementation of the `getFortuneForecast` could be static as it doesn't access instance data.
This means there is an opportunity of a performance gain by caching a TV object in a static field and
using only that for data retrieval.

However, that method is `virtual`, which means anyone could inherit the TV class and override the
implementation of the method. If they would do that, this mod would not be compatible with that change,
and I don't want that.

It's true that the mod uses the _first_ TV it finds in the current location's furniture collection,
which is not a perfect solution as a player could have different types of TVs, and it would be
questionable which should be used. (If there are 3 kinds of TVs in the collection, how to decide 
which should be used for data retrieval? It's kind of an undecidable question without knowing the
actual types.)

So I have _decided_ to:
- ✅ **support the possibility of different TV implementations**: there could be a mod which replaces _all_ TVs 
and in that case this mod would _not_ have compatibility issues with that mod.
- ❌ **NOT support having multiple types of TVs**: this mod's goal is to give some help for the player and I
don't want to extend its feature set to support this scenario as it would probably either:
  - result in multiple chat messages sent to the player and I don't like this idea (because this mod then loses its simplicity/conciseness) OR
  - result in configuration for TV type priorization/selection and honestly I think this would be too much for such a tiny feature the mod implements.
