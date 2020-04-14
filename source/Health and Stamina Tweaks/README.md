Health and Stamina Tweaks - By Svard (aka Kiddles)
=
With code from Zoryn's RegenMod
-

Health and Stamina Tweaks is yet another regeneration mod that does a little bit more. On top of providing the basic functions of the standard regen mod, it also allows the modification of one's max health and stamina values with some additional customization for each area.

I initially was going based on Hammurabi's old Regeneration mod as it already had a lot of the options I wanted -- and more. However, there were a lot of functions that were either very niche or not necessary at all. I'm also still a novice at coding, and couldn't get his code to work with the current version of SMAPI. So I used code from Zoryn's mod instead and added my own bits to get more functionality.

This code is open source under GPL 3.0, so you are free to use, modify and release modifications as you please.
Special thanks also to PathosChild for SMAPI and for updating Zoryn's RegenMod.


Fields are listed and described below:
-
StartingHealth (int)
 - The player's default max health that they start the game with
 
StarDropHealth (int)
 - The amount of max health gained from Stardrops
 
SnakeMilkHealth (int)
 - The amount of max health gained from Iridium Snake Milk
 
CombatLevelHealth (int)
 - The amount of max health gained for each Combat skill level
 
StartingStamina (int)
 - The player's default max stamina that they start the game with
 
StarDropStamina (int)
 - The amount of max stamina gained from Stardrops
 
SnakeMilkStamina (int)
 - The amount of max stamina gained from Iridium Snake Milk
 
SkillLevelStamina (int)
 - The amount of max stamina gained for each Farming, Fishing, Foraging and Mining skill level
 
RegenStaminaConstant (bool)
 - Whether to regenerate stamina by a constant amount

RegenStaminaConstantAmountPerSecond (float)
 - The amount of stamina to constantly regenerate per second

RegenStaminaPercent (bool)
 - Whether to regenerate stamina by percentage of the player's current max stamina

RegenStaminaPercentPerSecond (float)
 - The percent of stamina to constantly regenerate per second - stacks with Constant value if both are enabled

RegenStaminaMoving (bool)
 - Whether to allow stamina regeneration while moving

RegenStaminaMovingMult (float)
 - Multiplier for health regen while moving

RegenStaminaStillTimeRequiredMS (int)
 - Required amount required of still time (milliseconds) before the player regenerates stamina at full ratio

RegenStaminaFishing (bool)
 - Whether to allow stamina regeneration while fishing

RegenStaminaEvent (bool)
 - Whether to allow stamina regeneration during an event

RegenHealthConstant (bool)
 - Whether to regenerate health by a constant amount

RegenHealthConstantAmountPerSecond (float)
 - The amount of health to constantly regenerate per second

RegenHealthPercent (bool)
 - Whether to regenerate health by percentage of the player's current max stamina

RegenHealthPercentPerSecond (float)
 - The percent of health to constantly regenerate per second - stacks with Constant value if both are enabled

RegenHealthMoving (bool)
 - Whether to allow health regeneration while moving

RegenHealthMovingMult (float)
 - Multiplier for health regen while moving

RegenHealthStillTimeRequiredMS (int)
 - Required amount required of still time (milliseconds) before the player regenerates health at full ratio

RegenHealthFishing (bool)
 - Whether to allow health regeneration while fishing

RegenHealthEvent (bool)
 - Whether to allow health regeneration during an event
