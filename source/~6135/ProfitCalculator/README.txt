/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

# Profit Calculator

This is a simple profit calculator that calculates the profit of a product based on the cost of buying the seed, selling price and the number of units sold and their quality.

Provides the ability to select whether the user wants to buy seeds or fertilizer and the quality of said fertilizer. The user can select the day of the season and the season itself. Works for Modded crops but might have a slight seed price error. For more accurecy, disable the seed buying option or add the seed price manually.

## Installation

1. Install [SMAPI](https://smapi.io/).
2. Install [Generic Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098). Optional but recommended to allow for more customization of settings.
2. Install [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915). (Optional to add CP Crops)
3. Install [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720). (Optional to add JA Crops)
4. Install [DGA](https://www.nexusmods.com/stardewvalley/mods/9365). (Optional to add DGA Crops)		
5. Drop the contents of the provided folder into your `Stardew Valley/Mods` folder, or install from Nexus.
6. Run the game using SMAPI.
7. Press `F8` to open the calculator. This can be changed in the config file or in the Generic Config Menu.

## Configuration

The config file is located in `Stardew Valley/Mods/ProfitCalculator/config.json`. It allows you to change the keybind to open the calculator and the time for the tooltip to appear.

## Seed Price Override

The mod will automatically calculate the seed price based on the crop's base price and the season. However, if you want to override the seed price, you can do so by adding a `price` to the `SeedPrices.json` file in the `assets` folder. The `price` field should be a number. For example, if you want to override the seed price for the potato crop, you would add the following to the `SeedPrices.json` file:

```json
{
  //"SeedID": "price"
  "475": 50,
  //or for dga crops
  "ppja.fruitsandveggies.DGA/Adzuki Bean Seeds": 50

  ...
}
```

## Manual Crops 

If you want to add a crop that is not in the game, you can do so by adding a `crop` to the `ManualCrops.json` file in the `assets` folder. The `crop` field should be an object with the following fields (this example is for the tea bush crop):

```json
{
  "215": [
    "Tea Leaves",               // Name of the crop.
    "815",                      // Harvest ID (Id of item that drops).
    "20",                       // Growth time. Time it takes to grow.
    "1",                        // Regrowth time. Time it takes to regrow.
    "spring summer fall",       // Seasons the crop grows in.
    "50",                       // Sale price of the crop.
    "1500",                     // Seed price of the crop.
    "1",                        // Min harvest. Minimum number of items that drop.
    "1",                        // Max harvest. Maximum number of items that drop.
    "0",                        // Max harvest increase per farming level. The Number of items that the maximum limit increases by per farming level.
    "0.0",                      // Extra chance. Extra chance of getting an extra item.
    "false",                    // Affects quality. Whether the crop is affected by the quality (gold, Silver, Iridium).
    "false",                    // Affects Fertilizer. Whether the crop is affected by fertilizer.
    "false",                    // Raised crop. Whether the crop is raised (trelis).
    "true",                     // Wheter the crop is a bush.
    "false",                    // Paddy crop. Whether the crop is a paddy crop. (grows in water like rice)
    "false"                     // Giant crop. Whether the crop can grow into a giant crop.
  ]
}
```

## Know Issues

1. The mod does not take into account the farming level buffs. This is because I don't know how to get the farming level buffs. If anyone knows how to get them, please let me know.
2. The mod does not take into account the luck based chances of getting extra items.
3. Some modded crops might have a seed price error. This is because the mod uses the `SalePrice()` function to get the seed price but it seems that some modded crops don't return the correct seed price with this function, more specifically, DGA crops. If anyone Knows how to get these for DGA crops, please let me know. For now, you can add the seed price manually as described in section [Seed Price Override](#seed-price-override).
4. Some extra large text may be too small to read. This is because the mod lowers the size of huge text to make it fit in the crop box. If you notice this, please let me know which crop it is and from which mod it is from so I can figure out a proper fix.
5. When changing the scale of the game, the main options menu will be scaled but the options will be in the original positions, to fix this you need to close and reopen the options menu. 

### TODO:

- [X] Add support for Vanilla crops.
- [X] Take Fertilizer into account.
- [X] Take Quality into account.
- [X] Add support for JA crops.
- [X] Add support for CP crops.
- [X] Add support for DGA crops.
- [ ] Add proper scaling support for options menu.
- [ ] Obtain Seed prices from stores and from DGA to get more accurate seed prices.
- [ ] Add support to multi-drop crops.
- [ ] Add support for fruit trees.
- [ ] Add options to disable cross season crops.
- [ ] Automatically get accurate price for modded crop seeds. Currently it uses the base price it finds and not the actual shop price.
- [ ] Add Support for different types of output. (i.e. Jelly, Wine, Juice, etc.)
- [ ] Possibly add easer ways to add manual crops and seed prices maybe through a config menu or command.
- [ ] Take into account farming level buffs.
- [ ] Make API to allow other mods to add crops or Providers.