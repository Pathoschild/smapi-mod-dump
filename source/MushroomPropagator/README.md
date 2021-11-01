**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine**

----

![](https://i.imgur.com/SDY3EWe.png)

## How do I install it?

![](https://i.imgur.com/r5bh8IB.png) Install the latest versions of [SMAPI](https://smapi.io/) and [SpaceCore](https://www.nexusmods.com/stardewvalley/mods/1348),  
![](https://i.imgur.com/r5bh8IB.png) Install this mod via [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/4637),  
![](https://i.imgur.com/r5bh8IB.png) Run the game using SMAPI

## How do I get it?

![](https://i.imgur.com/3JjzINP.png) The Propagator isn't immediately available – by default you'll need a cellar in the farmhouse, and mushrooms in the cave.  
Demetrius will come visit you the morning after Robin finishes work on your house to deliver the recipe.  
This mod is officially sponsored by the 'didnt-pick-fruit-cave' gang.

## How does it work?

### ![](https://i.imgur.com/LuahOQY.png) Crafting
The propagation log is craftable for the same ingredients as a cask – 20 wood, 1 hardwood. Drop a mushroom onto the log to start growing!

### ![](https://i.imgur.com/LuahOQY.png) Growing
At the end of the day, each of the logs will grow a little more.  
A common mushroom will typically grow every night, a red mushroom every two nights, and a purple mushroom every four nights.  
Custom mushrooms added with Json Assets, like Shiitake Mushrooms and Black Fungus, will grow relative to their value.  
You can leave the logs for a few days and they'll keep growing more mushrooms for their stacks for when you check on them.  
The logs can only hold so many of each type of mushroom, though: the bigger and better mushrooms will grow smaller stacks.  
Keep an eye out for logs with huge, fully-grown mushrooms: they've reached their capacity, and you'll need to pop them off to keep growing.

### ![](https://i.imgur.com/LuahOQY.png) Interacting
Click while holding a mushroom to set it as the root-mushroom for an empty log.  
If the log's already got one, it'll pop off and swap to yours.  
Click the log later on to pop any extra mushrooms off it, and click it again to pop the root-mushroom off.  
Like the Casks in your cellar, you can whack them with an axe or a hoe to pop off the root-mushroom, too.

### ![](https://i.imgur.com/LuahOQY.png) Perks
Your Foraging perks improve the mushrooms you grow!  
With the Gatherer perk, you'll get a chance for a bonus mushroom on each harvest.  
Once you've reached the Botanist perk, you'll pluck only the highest-quality mushrooms.

## How can I configure it?
##### Launching SMAPI creates a `config.json`, just edit it and re-launch!

If you'd like to use the logs in places other than your cellar and the cave, you can enable them almost anywhere else, too.

![](https://i.imgur.com/bLL1O6n.png) **DisabledForFruitCave** – Picked the fruit cave and you still want to grow more mushrooms? I'll allow it!

![](https://i.imgur.com/bLL1O6n.png) **RecipeAlwaysAvailable** – No time for Demetrius? Don't want a big house? You can get the recipe from day 1!

![](https://i.imgur.com/bLL1O6n.png) **MaximumDaysToMature** – Want mushrooms to grow faster or slower to match your other mods? Change the timescale!

![](https://i.imgur.com/bLL1O6n.png) **MaximumQuantityLimitsDoubled** – Don't check the cellar often? The total mushrooms each log holds can be raised!

![](https://i.imgur.com/bLL1O6n.png) **OnlyToolsCanRemoveRootMushrooms** – Want better control over your logs? Clicking won't pop mushrooms off using this!

![](https://i.imgur.com/bLL1O6n.png) **PulseWhenGrowing** – Don't want the default machine working effect? You don't need it! Set this to false!

![](https://i.imgur.com/bLL1O6n.png) **OtherObjectsThatCanBeGrown** – The mushroom machine accepts all custom items called "mushroom" or "fungus".  
If you have any custom objects that use a different name, like "Red Cap" instead of "Red Cap Mushroom", add them here!

You can't spawn in a working Propagator with CJB Item Spawner.  
If you're a cheater, set **DebugMode** to `true` to allow hotkey spawning:  
You can change the key to add one to your inventory, the default's comma `,`

![](https://i.imgur.com/FvVFR7K.png)
