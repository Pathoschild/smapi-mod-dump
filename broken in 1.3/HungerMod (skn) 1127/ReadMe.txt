About
   Creates a simple hunger mechanic that's very customizable. With everything on the default values
   the player will have to eat once every 2 days, after 2 days stamina will be reduced by a % of your maximum stamina compared to your hungerValue
   After 7 days player will not have any stamina when they wake up(forcing you to eat to be able to do anything).
   
   For example(assuming all default values) your maximum stamina is 300.  You go 4 days of no eating(so 104 hungerValue) 
   you'd start the day with 124.8(125) stamina((hungerValue / maxHungerValue) * maxStamina).

   to go from 0 to 250 hungerValue you'd need 16 leek(16 hungerValue), 5 bread(60 hungerValue) or 2 pizza(180 hungerValue)'s. 
   To maintain yourself daily, you'd have to eat 2 leeks a day(with exception of 1 day each season where you'd have to eat 3 leeks)

   First game day player will not have their hunger values reduced on default(can configure in the json file).

Configure the mod using the config.json file.

HungerBar
   > A bar that shows how hungry\non hungry you are, changes color between green and red if affected by hunger.

HungerFirstDay
   > If true, player will start the game with hunger reduced by HugerValueReduction(15 default, so 85 hunger value, 
                               meaning you have to eat that day or suffer stamina penalty).

ShowHungerValue
   > If true, creates a message displaying the current hunger value every morning.
       Default is false

ConstantStuffedMessage 
   > If true you will recieve a message saying you're stuffed everytime you eat(when not hungry), if false will only recieve the message once per day.
       Default is false

ShowNotFillingMessage
   > Display a message if the food eaten is in the notFoodItems list
       Default is false, already displays message depending on how full you are if the item is valid food.

FoodMultiplier
   > Multiply by how much better veryGoodFoodItems is. Stamina of items are edibility * 2.5.  multiplyer is edibility * multiplier
       default is 3

FoodDivident { get; set; } = 1.8;
   > How much worse notGoodFoodItems are.   same as with multiplier, except it's edibility / divident.
       default is 1.8

HungerValueMax
   > The maximum hunger value you can have(100% full).
       default value is 250

HungerValueReduction
   > How much to reduce the hunger value by each night
       default is 36, giving you 7 days until 0 stamina and allowing you to not eat for 2 days

SpringDays\SummerDays\FallDays\WinterDays
   > a list of Days in the respective season where hunger will not be affected



NotFoodItems
   > A list of item ID's that's not considered food(will not increase hunger value).
       default items are:
              348 - Wine | 459 - Mead | 346 - Bear | 303 - Pale ale | 395 - Coffee | 432 - Truffle Oil
              247 - oil | 419 - Vinegar | 302, 304 - Hops | 167 - Joja cola | 349 - energy tonic | 351 - muscle remedy

VeryGoodFoodItems
   > A list of item ID's that is considered "very good" and gives double the hunger value(*2 edibility(which is / 2.5 of energy shown)).
     default items are all items found under "food"\cooking in the official wiki.

StartHunger
   > at what value hunger effect will be taken into action
     default is 150.

Hunger25
   > also 50\75, is at what hunger values the different messages will be shown.

CurrentHungerValue
   > Message to display before showing the current hunger value

NotFillingMessage
   > Message to be displayed if eating food that is in the NotFoodItems list.

HungerDown#
   > Message to be displayed when hunger values reduce to that value(after sleeping).

HunderUp#
   > Message to be displayed when hunger value increase to that value(after eating).


Installation instructions:
    Download then install SMAPI 
         > Src - https://github.com/Pathoschild/SMAPI/releases
    Unzip\Create folder named HungerMod in Stadrew Vallley\Mods   folder. Structure map:
    Stardew valley\Mods\HungerMod\|> HungerMod.dll
                                  |> Hungermod.pdb
                                  |> manifest.json