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

By default:
Any villager within 5 tiles of you when you shoplift will lose 2 hearts of friendship. If the villager also happens to be the shopkeeper or their family/employee they will also fine you the value of your current funds up to 1000g.

If you get caught by the shopkeeper or their family/employee three times within a 28 day period, you'll receive a three day ban from entering the shop, this excludes the Icecream Stand. You're given a clean slate 28 days after the first time shoplifting, a second shoplift won't carry over. Lucky you!

You can only successfully shoplift once per day, you're not that bad are you?

Version 1.1.0 added a config so shoplifting preferences can be changed, including: 
- The maximum shoplifts per day, setting this to 0 is equivalent to setting it to 1
- How many times the same shop can be shoplifted each day, setting this to 0 is equivalent to setting it to 1
- How many times you must be caught before being banned, setting this to 0 is equivalent to setting it to 1
- How many days you are banned for, set to 0 if you don't want to be banned
- Maximum fine amount
- Maximum friendship penalty
- Maximum distance a villager must be to catch the player, default is 5.

When changing the config, ensure only positive whole numbers or zero are entered so the config can be parsed. The mod will use the default values if it can't parse the config. 
Version 1.1.4 added Generic Mod Config Menu support so the config values can be changed whilst in game (thanks to luelista!).

The shops you can shoplift from are: Willy's Shop, Pierre's General Store, Carpenter, Marnie's Ranch, Harvey's Clinic, Saloon, Blacksmith, Sandy's Oasis, Icecream Stand.
If you're wondering why all shops aren't included it's because the shopkeeper never leaves said store, you'll never get away with it. Custom shops cannot be shoplifted from.

Boring specifics are listed below:

Shop | Who can catch you | Stock exclusions | Max number of different items | Max quantity of each item | Bannable
-----|-----------------|------------------|-------------------------------|-------------------------- | --------
Willy's Shop | Willy | Any furniture, fish tanks, fishing rods | 3 | 3 | Yes
Pierre's General Store | Pierre, Caroline, Abigail | Recipes, wallpaper, flooring | 5 | 5 | Yes
Carpenter | Robin, Demetrius, Maru, Sebastian | Recipes, any furniture, workbench | 2 | 20 | Yes
Marnie's Ranch | Marnie, Shane | Decorations, tools, heater | 1 | 15 | Yes
Harvey's Clinic | Harvey, Maru | None | 1 | 3 | Yes
Saloon | Gus, Emily | Recipes | 2 | 1 | Yes
Blacksmith | Clint | None | 3 | 10 | Yes
Sandy's Oasis | Sandy | Furniture, clothing | 3 | 3 | Yes
Icecream Stand | Alex | None | 1 | 5 | No

## Compatibilty ##

Fully compatible with multiplayer, shoplifting data is separate for each player. Should work fine with a controller, just make sure you're under the counter.

Compatibile with shop tile framework in that altered stock will be considered when stock is generated for each shop (except for Sandy's Oasis, this causes an exception due to reflection and Harmony Patches). However, new shops added by the mod can't be shoplifted since even added vanilla shops are regarded as custom shops.

## Translation ##

Shoplifter fully supports translation! In game translations currently available in:
- Chinese, thanks to 20080618 and XinJiDA!
- Korean, thanks to Aromay7!
- Italian, thanks to peanutbatteries and AlixNauts!
- French, thanks to Breeis!
- Portuguese, thanks to Kadomine!
- German, thanks to NotErikWasTaken!
- Spanish, thanks to viridian-fog!
- Turkish, thanks to zenura!
- Russian, thanks to Bellden!

Generic Mod Config Menu config menu translations currently avaliable in:
- Chinese, thanks to XinJiDA!
- Italian, thanks to AlixNauts!
- German, thanks to Nordmole!

Other Generic Mod Config Menu translations done using DeepL and Google Translate. Improved translations are welcome.

New translations always welcome! The best place to reach me is on the Shoplifter [mod page](https://www.nexusmods.com/stardewvalley/mods/8569).
Translations are released separately to the main files until a sufficient number of translations are received for a full release. 1.1.4 was the last translation release including all currently available languages and some basic translation for the new GMCM menu. 

Note: Translated GMCM menus may not have the proper layout.

## Versions ##
1.0.0 - Initial release

1.1.0 - Made shoplifting preferences configurable, added Icecream Stand as shopliftable

1.1.2 - Added translation support

1.1.3 - Added Korean, Chinese, French, Portuguese, German and Italian translations

1.1.4 - Added GMCM support thanks to luelista! Added Turkish, Spanish and Russian translations.

1.15 to 1.19 - Minor bug fixes

