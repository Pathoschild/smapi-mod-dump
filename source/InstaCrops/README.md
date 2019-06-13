# Instacrops
A Stardew Valley mod to instantly grow crops on the farm or green house.

# Configuration Options
1. **chanceForGrowth** - This is the random chance for an individual crop to become fully grown overnight.
2. **useRandomChance** - Determines if the random chance value is used. When this is set to false all crops planted are fully grown overnight.


# Daily Luck Modifiers
The player's daily luck can modify the chance for growth value.
This only occurs when the "useRandomChance" configuration value is set to "true"

1. When the day is unlucky, the chance for growth percentage is ***reduced by 10***.
2. When the day isn't lucky but not unlucky, the chance for growth ***isn't changed***.
3. When the day is a little bit lucky, the chance for growth is ***increased by 5***.
4. When the day is very lucky, the chance for growth is ***increased by 10***.

# Other Notes
The crop has to be watered for this mod to attempt to instantly grow it. It also has to be not-dead.

The crops are instantly grown overnight. So when you go to your bed to sleep the crops will instantly grow before the game is saved. This means that the luck modifier used is on the day you go to bed, not the day you wake up.
