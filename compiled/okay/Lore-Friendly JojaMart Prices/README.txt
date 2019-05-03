Configuration instructions:

* PriceDiscountNoMembership - This is the discount applied for non-members relative to Pierre's prices (default: 3g).
* PriceDiscountMembership - This is the discount applied for members relative to Pierre's prices (default: 20%).
* ModifySunflowerSeeds - (true/false) If true, sunflower seeds are marked up to be priced with Pierre's prices, and the price discount values apply; if false (default), sunflower seed prices are unchanged from vanilla.
* WheatDiscountNoMembership - This is the discount applied for non-members to wheat seeds only; they get their own config because 20% of 10 is less than 3 (default: 3g).
* WheatDiscountMembership - This is the discount applied for members relative to wheat seeds only; they get their own config because 20% of 10 is less than 3 (default: 4g).
* MatchRandomWallpaperFlooring - (true/false) If true (default), the starting price for the random wallpaper/flooring is the same as Pierre's; if false, it remains at 250.
* RandomWallpaperFlooringDiscountNoMembership - This is the discount applied for non-members to the random wallpaper and flooring (default: 3g).
* RandomWallpaperFlooringDiscountMembership - This is the discount applied for members to the random wallpaper and flooring (default: 20%).
* JojaBrandedItemDiscountMembership - This is the discount applied for members to Joja-branded items (default: 20%).

All discount values must be formatted as a non-negative integer less than 1000 and be followed by either a "%" or a "g". Any discount that does not match this format will revert to its respective default value. The config.json configuration file will only appear after the first time the game is run with this mod installed.

For example, if I want non-Joja-members to get a 1g discount on all goods compared to the prices at Pierre's, I would configure "PriceDiscountNoMembership" to be "1g"; if I wanted members to get a 10% discount, I would configure "PriceDiscountMembership" to be "10%".