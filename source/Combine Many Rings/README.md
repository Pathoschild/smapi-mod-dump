**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/SlivaStari/CombineManyRings**

----

# CombineManyRings

A simple mod to allow the [Forge](https://stardewvalleywiki.com/Forge#Combined_Rings) to combine multiple rings together instead of just two.

Supports several configuration options, including **AllowSameRing**, and **BalancedMode**. BalancedMode is based on the code in arruda's [BalancedCombineManyRings](https://github.com/arruda/BalancedCombineManyRings/tree/master)

## AllowSameRing

**AllowSameRing** allows the forge to combine the same ring into itself, or into a Combined Ring that already contains a version of that ring. Obviously, this is very strong.

## BalancedMode

**BalancedMode** creates a balanced and challenging system to scale cost to combine based on previous combination. Defaults to `false`.

With the default other configurations, this is the expected behaviour of combining more than two rings in the forge:

* Combining 2 Rings - Default cost and behavior
* Combining 3 Rings - cost of 100 Cinder Shards and 20% of chance of failing to forge
* Combining 4 Rings - cost of 200 Cinder Shards and 40% of change of failing to forge
* Combining 6 Rings - cost of 400 Cinder Shards and 80% of change of failing to forge
* Combining 7 Rings - cost of 500 Cinder Shards and 90% of change of failing to forge
* Combining 12 Rings - cost of 999 Cinder Shards and 90% of change of failing to forge

Therefore to be able to get a lvl 4 Combined Ring (4 rings combined into one) the player will need to expend a total of 300 Cinder Shards (100 for 3 rings, and then another 200 for the 4 rings forge), with an overall probability of success of 48% (80% for the 3 ring forge, and then 60% for the 4 ring forge).

There are multiple configurations that can be changed to adapt the mod to best suit your own level of challange and fun:
### CostPerExtraRing
How much is the increase in the Cinder Shards cost for each extra ring other than the two initial ones being combined.
The default value is set to `100`.

### FailureChancePerExtraRing:
Changes the how much will the chance of failured increase for each extra ring being combined, other than the two original rings, to a maximun of 90%.
Default value is set to `20`.

### DestroyRingOnFailure
For the players that are looking to a more extreme penalty. On the event of a failure in the forge, one of the rings (mostlikelly the one with a high-number of combined rings) will be destroyed, in addition to consuming the Cinder Shards for the process.
Default value is set to `false`.

Should work with [WearMoreRings](https://github.com/bcmpinc/StardewHack/tree/master/WearMoreRings).
