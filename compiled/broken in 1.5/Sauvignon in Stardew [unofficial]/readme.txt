Link to the original mod page on Nexus:
https://www.nexusmods.com/stardewvalley/mods/2597
All credits and assets go to the original mod author; I'm just a humble, wine loving farmer that wanted to see his favorite mod back online.

With Stardew 1.4, a lot of existing mods broke. In addition to that, Sauvignon in Stardew hasn't been updated in a year or so and was already not working in 1.3. I've fixed both the outstanding issues in 1.3 and the compatibility issues for 1.4.

FEATURES:

-If the Tiller profession is chosen at Farming level 5, you gain a third profession choice at Farming level 10: the Distiller. If the Distiller Profession is enabled in the mod's config file, all alcoholic beverages will be considered a new category 
of goods: Distilled Craft. The Distiller gains a 40% increase in the sale of Distilled Craft. Of course, this means that the Artisan profession no longer grants bonuses to alcohol, and instead only grants bonuses to cheese, oil, etc. This can all be 
disabled in favor of the more powerful, general-purpose Artisan profession by setting DistillerProfessionBool : false in the mod's config file.

-Robin Sells a new building called the Winery. The Winery is a large building based off the slime hutch with a mostly-functional walkthrough archway (see further below). Kegs and casks work 30% faster (on any goods) while inside the Winery. However, 
space is fairly limited so you will most likely have to build multiple. Think of them as upgraded sheds with a bigger investment cost. Plays pretty music inside.


FIXED ISSUES:

-Previously, the code to prompt you with the option for the Distiller profession was broken and checked if your farming level was 0 instead of 10. This has since been fixed, so you will be prompted at Level 10 Farming to choose Distiller if you chose 
Tiller at Level 5.

-Cleaned up some code that became obsolete with Stardew 1.4.

KNOWN REMAINING QUIRKS:

-If you dig around the source code or even just the mod folder, you'll notice a lot of unused assets (big keg, wineroom, etc). These were plans the original developer had to make a big wine room with custom, enormous kegs that would require an upgrade 
from Robin. I'm not sure how to fiddle with a lot of these assets and I don't think I plan on working on them unless the OG mod creator wanted to work together.

-The archway on the exterior of the Winery is slightly broken. If you walk inside it, you will be bounded on the left, but can walk right through it to the right. You can even walk straight through the right side of the arch. However, this is easily 
fixed with some fencing, kegs, <put your favorite one-tile object here> immediately inside and to the right of the arch, going in a line backwards for several tiles.

-Alcohol that appears in shops, chests, item spawner menus, or fresh from produce machines will still bear the "Artisan Goods" label. However, once they are in your inventory, their category will be correctly changed to "Distilled Craft" when 10 
minutes passes. Also, all Distilled Craft in your inventory are converted back to Artisan Goods when the day ends, and converted back to Distilled Craft in the morning. This is so that you don't have bricked items in your inventory if you 
uninstall the mod. This is just how the mod works and I don't plan on changing it. 

-I have zero clue if this mod works for multiplayer or not, but I don't have the expertise to fix it if it doesn't. Sorry :(

To the original mod author: thank you for making your work open source so that other community members can continue to keep it up to date, even if you're no longer pursuing it personally. That said, if you should ever come back, please let me know 
if you want to work on those future features for this mod together! It looks really awesome and I'd love to help, even if I'm a noob at modding. 