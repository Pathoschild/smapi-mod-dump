**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/agilbert1412/StardewArchipelago**

----

# Stardew Valley Archipelago Mod Changes

## Introduction

For the sake of aligning several mods with the randomizer, certain modifications are made.  Many of these are obvious (ex: Friendsanity shuffles hearts, including modded villagers); this document is to help offer notices for changes that may not be immediately obvious.

## Stardew Valley Expanded

- Galmoran Gem can be shipped, but is given to the player once and is easily lost.  Thus there is no Shipsanity check for this item.
- Normally Gunther requires donating enough materials to the museum, and replaces the reward for the Rusty Key as Marlon now gives it.  Instead, Gunther is simply unlocked from the start.
- Most events are modified due in part to the [Content Patcher mod](https://github.com/Witchybun/SDV-Randomizer-Content-Patcher/releases), such as:
    - Enchanted Grove warps are all items, and so all cutscenes are dummied out.
    - Aurora Vineyard introduction event is now tied to an item, so cannot be triggered the normal way.
    - To minimize using similar text as known events in SVE, most Claire/Martin events post-Jojamart are in their Jojamart state, due to them being tied to community center completion.
    - Scarlett becoming friendable is no longer tied to Sophia's 8 heart event, to avoid making Sophia's hearts too valuable.  This is now an item: Scarlett's Job Offer.
    - Morgan becoming the Wizard's student is no longer a Year Three event, as we want to avoid time only dependent events.  Them coming to Pelican Town is now an item: Morgan's Schooling.
- Most quest are split from their rewards, such as:
    - Void Soul Retrieval reward is now an item, so the cutscene given for completion does not trigger without it.
    - Grandpa's Shed dropbox is moved outside the building, and now repairing the building is an item.
    - Marlon's Boat Paddle is the item reward for Marlon's Boat, which lets you go to the Highlands.
    - Iridium Bomb is the item reward for Railroad Boulder, which lets you go to the Summit.
- Normally, Junimo Woods shops are a bit of a reward for the halfway point of playing the game plus the mod itself, giving some hard to obtain end game items; however, many of these are simpler to acquire.  Instead, these shops sell the following to keep a similar spirit:
    - Grey Junimo offers any mineral you've found before.
    - Red Junimo offers any artifact you've found before.
    - Blue Junimo offers any fish you've found before.
    - Yellow junimo offers seeds, similar to Pierre and Sandy.
    - Purple junimo offers money, friendship bonuses, or dewdrop berries.
- In addition, several shops barter, instead of trade in money.  This includes the Bear Shop, and the Junimos.  The Junimo barter using random items of the same color as them, and the bear trades in berries.
- Normally, Morris leaves Pelican Town upon completing the community center.  While this is still true, it is not tied to the item Progressive Movie Theater.  If these are acquired, instead of Morris move into Jojamart, he will sit at a nearby bench to be accessible and avoid errors.

## Boarding House and Bus Stop Extension
- The mod introduces the ability to 'restore' certain artifacts that you can find in the abandoned mine.  These craftables are not included in craftsanity, and the resulting pieces are not considered for shipsanity.

## Magic

- Learning a spell is a location, and a spell is an item, save the original starting spells one would get from this mod.
- When any spell is learned, the altar at Pierre's becomes usable.  Resting afterwards is enough to allow for learning the first starting spells such as Analyze.

## DeepWoods

- Normally, pulling Excalibur requires a bit of daily luck, 8 luck (as in, the stat), 10 in all skills, and several goals completed which are in of themselves goals for the base randomizer.  To avoid pushing this location behind most standard goals, thus trivializing it, the requirements have changed to:
    - Daily luck is no longer counted.
    - 7 Luck
    - 40 total in all non-mod stats.
    - Three pendant items: Pendant of Depths, Pendant of Community, Pendant of Elders, as reference to the original requirements.
- Petting the unicorn no longer has a check for how fast you are moving, as its possible to move too fast due to speed buffs.  Instead it checks if you are walking.
- With mines elevators shuffled, the Woods Obelisk levels are also shuffled, up to level 100.  Upon receiving the last level as an item, you may gain depth levels normally.

## Skull Cavern Elevator

- With mines elevators shuffled, the elevator floors are also shuffled, up to level 200.  Upon receiving the last level as an item, you may gain elevators normally.
