-~-ACTUALLY LUCKY RABBIT'S FOOT!-~-

With this mod, carrying around a rabbits foot will actually affect your luck bonus.

--DETAILS ON LUCK BONUS--
Out of the box, carrying around a rabbit's foot confers a <rabbit foot quality> * 1 + 1 luck bonus. This is a passive bonus, you won't see a luck bonus display but it's there. This table shows the bonuses by quality of rabbit's foot:
QUALITY	|	LUCK BONUS
0-star		+1
silver		+2
gold		+3
purple		+4

This mod affects your luck much like eating or drinking, so it shouldn't affect things like morning drop rates or certain fishing treasure rarity the way Daily Luck does. See the Stardew Valley wiki entry for "Luck" for more details on how luck works.
The effects of this mod should stack with food, drink, and wearables that give luck. You can turn on verbose mode for SMAPI and press the L key at any time to see what your current luck bonus is.
For this beta version, whenever your luck bonus is 20 or higher the luck report will display as a warning to show you that your luck may be in absurd territory. This could indicate an error with adjusting your luck bonus, or perhaps you have found a legitimate way to be extremely lucky! You may also have other mods installed that affect your luck bonus, which could push your bonus beyond regular levels.

--CONFIGURATION--
By default, the luck bonus is pretty potent! A gold star rabbit's foot is on par with constantly having the effect of eating a Lucky Lunch. If you don't want the bonus to have quite a punch, you can edit the quality muliplier by opening the luckyRabbitsFootConfig.json file. This is a double value, meaning you can use a decimal point. When calculated, the modifier will round down to the nearest integer.
For example, setting "QualityMultiplier" = 0 will result in any rabbit's foot of any quality only giving a luck bonus of +1.
Setting the "QualityMultiplier" = 0.5 would result in the following table:
QUALITY	|	LUCK BONUS
0-star		+1
silver		+1
gold		+2
purple		+2