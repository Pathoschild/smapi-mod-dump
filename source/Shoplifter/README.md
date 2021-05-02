**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TheMightyAmondee/Shoplifter**

----

# Shoplifter

Shoplifter is a mod for [Stardew Valley](https://www.stardewvalley.net/) that allows the player to shoplift when the shopkeeper is not at the counter. 
Stock is randomly generated based on the save file and the number of days played for each shop. This is to ensure stock is the same throughout the day.
Only basic items that are currently available to your character can be shoplifted, it's unlikely you could get away with stealing a TV from Robin's (as she does point out...)

Ready for a spree of petty crime? Just don't get caught...

Any villager within 7 tiles of you when you shoplift will lose 2 hearts of friendship. If the villager also happens to be the shopkeeper or their family/employee they will also fine you the value of your current funds up to 1000g.

If you get caught by the shopkeeper or their family/employee three times within a 28 day period, you'll receive a three day ban from entering the shop. You're given a clean slate 28 days after the first time shoplifting, a second shoplift won't carry over. Lucky you!

You can only successfully shoplift once per day, you're not that bad are you?

The shops you can shoplift from are: Willy's Shop, Pierre's General Store, Carpenter, Marnie's Ranch, Harvey's Clinic, Saloon, Blacksmith, Sandy's Oasis. 
If you're wondering why all shops aren't included it's because the shopkeeper never leaves said store, you'll never get away with it. Custom shops cannot be shoplifted from.

Boring specifics are listed below:

Shop | Who can ban you | Stock exclusions | Max number of different items | Max quantity of each item
-----|-----------------|------------------|-------------------------------|--------------------------
Willy's Shop | Willy | Any furniture, fish tanks, fishing rods | 3 | 3
Pierre's General Store | Pierre, Caroline, Abigail | Recipes, wallpaper, flooring | 5 | 5
Carpenter | Robin, Demetrius, Maru, Sebastian | Recipes, any furniture, workbench | 2 | 20
Marnie's Ranch | Marnie, Shane | Decorations, tools, heater | 1 | 15
Harvey's Clinic | Harvey, Maru | None | 1 | 3
Saloon | Gus, Emily | Recipes | 2 | 1
Blacksmith | Clint | None | 3 | 10
Sandy's Oasis | Sandy | Furniture, clothing | 3 | 3

## Compatibilty ##

Fully compatible with multiplayer, shoplifting data is seperate for each player. Should work fine with a controller, just make sure you're under the counter.

Compatibile with shop tile framework in that altered stock will be considered when stock is generated for each shop (except for Sandy's Oasis, this causes an exception due to reflection and Harmony Patches). However, new shops added by the mod can't be shoplifted since even added vanilla shops are regarded as custom shops.

## Versions ##
1.0.0 - Initial release



