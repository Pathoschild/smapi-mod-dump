**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TheMightyAmondee/CustomTokens**

----


Custom Tokens is a mod that provides some additional tokens for Content Patcher, extending what can be done. I recommend using this mod with [Stats as Tokens](https://www.nexusmods.com/stardewvalley/mods/9659) for even more Content Patcher extensibility with creating content packs. This README is intended for Content Patcher authors.

### Custom Tokens registers the following tokens:
Basic Tokens:
- ``MineLevel`` the player is currently on
- ``VolcanoFloor`` the player is currently on
- ``DeepestMineLevel``, the deepest minelevel the player has reached, inclusive of both the normal and Skull Cavern mines
- ``DeepestVolcanoFloor``, the deepest volcano floor reached by the player
- ``DeepestNormalMineLevel``, the deepest normal minelevel reached by the player
- ``DeepestSkullCavernMineLevel``, the deepest skull cavern minelevel reached
- Anniversary of the player, split into two tokens, ``AnniversaryDay`` and ``AnniversarySeason``
- ``YearsMarried``, total number of years married
- ``QuestIDsCompleted``, a list of quest ids that the player has completed
- ``QuestsCompleted``, the total number of quests completed
- ``SOIDsCompleted``, a list of the special orders a player has completed. See Special Order data for how to interpret return values
- ``SOCompleted``, total number of different special orders completed
- ``DeathCount``, total number of times the player has died
- ``DeathCountMarried``, an extension of DeathCount that tracks how many times a player has died after being married.
- ``DeathCountPK``, (provides a more accurate value than the DeathCount token for use in the PlayerKilled event)
- ``DeathCountMarriedPK``, an extension of DeathCountPK that tracks how many times a player has died after being married.
- ``PassOutCount``, how many times a player has passed out.

Advanced Tokens:
- ``Child``, holds information about each child that can be accessed with additional input arguments.

### Using Custom Tokens:
- SMAPI must be installed, minimum version is 3.12
- Ensure Custom Tokens is listed as a dependency in your content pack
- Tokens used must be prefixed with the mod's unique ID e.g TheMightyAmondee.CustomTokens/MineLevel
- Many tokens require an update rate faster than CP's default as they can change throughout the day

### Limitations:
Previous quest determination was removed in version 1.4.0 due to issues (unreliable, buggy and incorrectly added cancelled quests as complete). Therefore, only newly completed quests are recorded in the token. Version 1.4.0 uses Harmony for more reliable quest recording, just FYI.

Necessary token values are now stored in the save file, data files can be deleted as of version 1.4.0

The mod works best with new save files due to these limitations, while it will still work fine with old save files, some tokens will be missing values.

### Using advanced tokens:

These tokens are more unstable as they use the advanced api so that input arguments are supported. They may not work fully for farmhands in split screen.

#### Child:
This token takes exactly 3 input arguments. They are not case sensitive. This token is not fully compatible with ChildToNPC.

The first argument, ``player`` ,gives the player type 
- Can be either ``host`` or ``local``. 
- Use ``host`` to get values related to the main player and use ``local`` to get values for the connected farmhand player. 
- If the player is the host, ``local`` and ``host`` will give the same value. 
- E.g ``player=host``

The second argument, ``childindex``, gives the child index, starting from 0. 
- 0 is the oldest child, 1 is the second oldest and so on. 
- Accepts any numerical value, however the largest to return a value is likely to be 1 as the game limits each player to two children unless mods are used.
- E.g ``childindex=1``

The third argument gives the name of the value you want 
- Can be either ``birthdayday``, ``birthdayseason``, ``daysold``, ``darkskinned`` or ``hat``.
- E.g ``daysold``

Index 2 accepted arguments in more detail:

Accepted arguments | what it gives | Notes
-------------------|---------------|------
birthdayday | The day of the month the child was born on
birthdayseason | The season the child was born in. | Value is in all lower case.
daysold | The current age of the child in days.
darkskinned | Whether the child uses the dark skinned sprite. | Gives true or false.
hat | The name of the hat the child is wearing, else ``"null"``. | The token will have a value of ``"Party Hat"`` for all three party hat colours.

Example of a full token with input arguments: ``TheMightyAmondee.CustomTokens/Child:player=host|childindex=0|birthdayday``. This token returns the day of the month the first born child of the host player was born on (very wordy, I know).

The token will return ``"null"`` if no suitable values are found.
  
### Config:
- AllowDebugging adds a single debug command so the values of the tokens can be viewed in the SMAPI console. When enabled typing "tracker" in the console will display a list of token values, this does not indicate whether CP has updated the token to this value. Use patch summary for that.
- ResetDeathCountMarriedWhenDivorced will cause the DeathCountMarried token to reset to 0 when divorced, default is true.

### Tokens in more detail:
Token | Default value | What it tracks | Notes 
----- | ------------- | -------------- | ------
Minelevel | 0 | Players current minelevel | Add 120 to Skull Cavern floors for token value. The quarry mine has a minelevel of 77377
VolcanoFloor | 0 | Players current floor in the Volcano Dungeon
DeepestMineLevel | 0 | The deepest minelevel the player has reached | Skull Cavern floors included
DeepestNormalMinelevel | 0 | Players deepest minelevel in the normal mines
DeepestSkullCavernMinelevel | 0 | Players deepest minelevel in the Skull Cavern
DeepestVolcanoFloor | 0 | Players deepest floor in the Volcano Dungeon | The deepest floor reached since the mod has been added.
AnniversaryDay | 0 | The day the player was married on
AnniversarySeason | No season | The season the player was married in | Value is in all lower-case
YearsMarried | 0 | The number of years the player has been married for
QuestIDsCompleted | None | A list of quest ids that the player has completed | Only records quests with ids as specified in the Quests.xnb
QuestsCompleted | 0 | The total number of quests completed | Includes quests with no ids e.g Bulletin board quests
SOIDsCompleted | None | A list of the special orders a player has completed | Repeating the same order will not add the key to the token again 
SOCompleted | 0 | Total number of different special orders completed | Only tracks different orders, won't include repeated orders
DeathCount | 0 | The number of deaths
DeathCountPK | 0 | Value is DeathCount + 1 when save is loaded |Because there are limits on the update rate of tokens in CP, this token can be used as a more accurate snapshot of DeathCount in some cases, mainly the PlayerKilled event
DeathCountMarried | 0 | The number of deaths that occur when the player is married
DeathCountMarriedPK | 0 | Value is DeathCountMarried + 1 when married | Because there are limits on the update rate of tokens in CP, this token can be used as a more accurate snapshot of DeathCountMarried in some cases, mainly the PlayerKilled event
PassOutCount | 0 | The number of times the player has passed out, either from exhaustion or it reaching 2AM
Child | "null" | Child state data, the token just by itself has no useable value and input arguments must be used.

### Special Order data
Since Special Orders don't have a number id like quests they are recorded in the save file using a unique string as shown in the table below. The mod will return this unique string as an ID for each Special Order completed in the SOKeysCompleted token

ID | Special Order Name
----|------------------
Willy | Juicy Bugs Wanted!
Willy2 | Tropical Fish
Pam | The Strong Stuff
Pierre | Pierre's Prime Produce
Robin | Robin's Project
Robin2 | Robin's Resource Rush
Emily | Rock Rejuvenation
Demetrius | Aquatic Overpopulation
Demetrius2 | Biome Balance
Gus | Gus' Famous Omelet
Lewis | Crop Order
Wizard | A Curious Substance
Wizard2 | Prismatic Jelly
Clint | Cave Patrol
Linus | Community Cleanup
Evelyn | Gifts for George
Gunther | Fragments of the past
Caroline | Island Ingredients
QiChallenge | FivePlagues
QiChallenge2 | Qi's Crop
QiChallenge3 | Let's Play A Game
QiChallenge4 | Four Precious Stones
QiChallenge5 | Qi's Hungry Challenge
QiChallenge6 | Qi's Cuisine
QiChallenge7 | Qi's Kindness
QiChallenge8 | Extended Family
QiChallenge9 | Danger In The Deep
QiChallenge10 | Skull Cavern Invasion
QiChallenge11 | Find Qi's Double
QiChallenge12 | Qi's Prismatic Grange

### Getting old save files up to speed (Version 1.3.1 and below):
Old or current save files require some additional set-up. This isn't strictly necessary but may result in incorrect token values.

A per-save JSON file will be generated after the day is started for each save so the mod can track values not tracked by the game. This is found in the data folder located in the mod folder. These values can be adjusted as needed as they will have an initial value of 0, which may not be accurate for older saves. Ensure the old tracker value is also updated to the same value as the current tracker value when changing values
 
While the mod can determine whether most quests have been completed, some quest ids from previously completed quests in old save files need to be added manually to the save's JSON file in AdditionalQuestsCompleted. Custom quest ids can also be added here. The field should look something like this "AdditionalQuestsCompleted": [6,16,128,130] depending on what quests have previously been completed.
  - If you have completed "Getting Started", add 6
  - If you have entered the Adventurer's Guild, add 16
  - If you have done secret note 25 (Abigail), add 128
  - If you have done secret note 25 (Caroline), add 129
  - If you have finished "The Pirate's Wife", add 130
