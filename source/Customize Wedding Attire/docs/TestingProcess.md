**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/elizabethcd/CustomizeWeddingAttire**

----

# Testing Process

There are a large number of possible scenarios that need testing to fully verify that this mod works correctly, both singleplayer and multiplayer. In any of these scenarios, the two things to check for are that the player appears correctly during the event, and that the player correctly returns to normal after the event (if applicable). 

## Setup Process

Mods to install while testing (see Mod Interaction Testing for further notes):
* Generic Mod Config Menu (GMCM)
* GMCM Options
* CJB Cheats
* CJB Item Spawner
* Skip Intro
* Instant Pets (optional, but cheating in gold/sleeping until a sunny day can trigger Marnie/Demetrius on your doorstep)

The easiest method to set up a save for testing is:
* Sleep until about a week has passed, and it's a sunny day
* Set time to 8am with CJB Cheats, and then walk into town from the bus stop
* Skip Community Center cutscene
* Warp to Community Center with CJB Cheats and view one of the scrolls
* Sleep, then check the mailbox until you read the wizard's letter
* Warp to the wizard with CJB Cheats
* Skip forest magic cutscene
* Use CJB Cheats to set 4 hearts or more with the wizard
* Use wizard basement shrine to set player gender
* Turn on "instant buildings" in CJB Cheats
* Warp to Robin's and upgrade your farmhouse once (use CJB Item Spawner for the materials, and CJB Cheats for the gold)
* Use ``debug wedding NPCNAME`` to marry one of the vanilla NPCs
* You can rerun the debug command anytime, you just have to change location to make the event start
* Use GMCM to change the config settings when needed

## Singleplayer Testing

Here are the different possible cases that need verifying in singleplayer:
* Male player, default config (should show as tux)
* Male player, no change config
* Male player, tuxedo config
* Male player, wedding dress config
* Female player, default config (should not change clothing)
* Female player, no change config
* Female player, tuxedo config
* Female player, wedding dress config

## Multiplayer Testing

When testing multiplayer, there is the additional complication of verifying what happens when players are in the audience. However, players can verify that they look correct to each other, which cuts down on the number of test cases. The test cases detailed here can be swapped around for equivalent effects.

### Player-Player Weddings

In these, the options can be combined any which way, as long as they all get tested. This means a total of 4 weddings. 
* Male player, default config (should show as tux)
* Male player, no change config
* Male player, tuxedo config
* Male player, wedding dress config
* Female player, default config (should not change clothing)
* Female player, no change config
* Female player, tuxedo config
* Female player, wedding dress config

### Player-NPC Weddings

Once the player-player weddings have been verified, the only remaining question is whether there's a difference between what the players in the audience see and what the players getting married see. Some of the exhaustive casework can likely be skipped.

Both of the following options should be tested as both the player marrying an NPC and a player in the audience of the wedding, for a total of 4 more weddings. 
* Male player, either no change config or wedding dress config
* Female player, either tuxedo config or wedding dress config

## Mod Interaction Testing

When testing, it's also necessary to check that the mod still works without GMCM installed, as well as without GMCM Options installed.

The singleplayer cases should be rerun with:
* GMCM but not GMCM Options installed
* Neither GMCM nor GMCM Options installed

For completeness, all singleplayer cases should be rerun, but since this is a bit tedious due to the need for repeated reloading, at the minimum one case that produces a visible change from vanilla should be tested for both possible player genders. In addition, this testing is skipped for multiplayer due to the extreme tedium of repeatedly reloading in multiplayer, but in theory would be tested for full completeness. 

## Technicalities

Technically speaking, some of these scenarios may not need testing, from a logical perspective relative to the code. However, agnostic to the code implementation, there may be even more scenarios that would need testing. This set of tests is written as a compromise between feasibility and completeness. 
