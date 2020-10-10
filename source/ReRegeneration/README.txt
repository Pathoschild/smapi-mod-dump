/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/littleraskol/ReRegeneration
**
*************************************************/

ReRegeneration is one more iteration of the idea of regenerating health and stamina in Stardew Valley. It is based on the code of "Regeneration" by Hammurabi, and while it does not replicate all its features, I've tried to add a good mix of options to make it a worthwhile option while that mod is not being maintained anymore.

Source code is included in the install package and available here: https://github.com/littleraskol/ReRegeneration

1. CONFIGURATION

The config file (config.json) clusters the options for health and stamina, but they are basically the same in function.

RegenPerSecond: The amount of stamina/health to be passively regenerated per second.

IdleSeconds: How long to wait since the last exertion/injury before regeneration begins. This is referred to as the "regen delay" or "cooldown."

maxRatioToRegen: Max percentage value of total stamina/health that passive regen can reach. Set to 1 to be able to passively regenerate all stamina or health; any value higher than 1 will be treated as 1. Note that the lowest possible value for this is 0.01 (or 1%), anything lower will be treated as that. IMPORTANT: Every mod setting based on the player's maximum stamina or health will scale to the value derived from this; in other words, "max stamina/health" in these definitions really means the actual in-game player value multiplied by this value.

scaleRegenRateTo: Scale the rate of stamina/health regen to be slower when lower. Specifically, as the value approaches 0, the regen rate gets closer being multiplied by this setting's value (e.g. at 0.5, cut in half). Another way of looking at this is that you will get (1 minus this setting's value) times less stamina/health regen (e.g. at 0.25, 1 - 0.25 = 0.75, or 75% less). Setting to 0 will turn this feature off.

scaleRegenDelayTo: Scale the time it takes for stamina/health regen to start to be longer when lower. Specifically, as the value approaches 0, the delay time gets closer to being multiplied by 1 plus this setting's value (e.g. at 0.5, 1.5x or 50% longer). Setting to 0 will turn this feature off. 

The default values (for stamina and then health) are:

  "staminaRegenPerSecond": 1.0,
  "staminaIdleSeconds": 5,
  "maxStaminaRatioToRegen": 0.8,
  "scaleStaminaRegenRateTo": 0.8,
  "scaleStaminaRegenDelayTo": 0.2,

Stamina will passively recover by 1 point every second after waiting 5 seconds. Passive regen will reach a maximum of 80% of max stamina, and regen rate and delay scale with current stamina value to approach 20% less/longer at 0 stamina.

  "healthRegenPerSecond": 0.5,
  "healthIdleSeconds": 10,
  "maxHealthRatioToRegen": 0.6,
  "scaleHealthRegenRateTo": 0.6,
  "scaleHealthRegenDelayTo": 0.4,

Health will passively recover by 1 point every 2 seconds after waiting 10 seconds. Passive regen will reach a maximum of 60% of max health, and regen rate and delay scale with current health value to approach 40% less/longer at 0 health.

There are other options that modify how the regen system works:

percentageMode: Whether the "RegenPerSecond" values will be interpreted as percentages of the maximum value, so that e.g. a value of "1" will not regenerate 1 point of stamina per second but rather 1% of the player's maximum stamina. This allows the regeneration rate to scale with higher levels of health and stamina. So, at the default stamina regen value of staminaRegenPerSecond = 1, at the start of the game 2.7 points of stamina will regenerate per second (1% of the starting value of 270 stamina and assuming maxStaminaRegen is set to 1). Note that if you do enable percentage mode, I'd advise you to change the default value of "staminaRegenPerSecond" to 0.4 because this will result in a starting regen rate of 1.08 points per second, which is close to the default in normal mode. Health starts at 100 points, so there's no discrepancy there. (Default = false)

regenWhileActiveRate: A 0 to 1 fractional value to multiply regen rates by while fishing or riding a horse. Default = 0.8, meaning 20% less regeneration. Set to 1 to ignore this feature. Set to 0 to turn regeneration off during these activities.

regenWhileRunningRate: A 0 to 1 fractional value to multiply regen rates by while running. Default = 0.5, meaning 50% less regeneration. Set to 1 to ignore this feature. Set to 0 to turn regeneration off while running.

exhuastionPenalty: A 0 to 1 fractional value by which the amount of regeneration is decreased and the duration of the idle delay is increased while the player is exhausted. At the default value of 0.25, regen amount is reduced by 25% and the delay time is increased by 25% while the player is exhausted. Set to 0 to turn this feature off. Note that when set to 1 or more, this value will be treated as 0.99 for the purposes of reducing regen rate.

endExhaustionAt: A 0 to 1 fractional value specifying when to end the "exhaustion" effect, such that it will end when player reaches the defined portion of max stamina. The default value of 0.75 means this will happen at 75% of max stamina. Set to 0 to turn this feature off.

shortenDelayWhenStillBy: Shortens the regen delay while standing still by the specified fractional multiplier. At the default setting of 0.5, each second spent standing still counts for 1.5 seconds for ending the cooldown (i.e., 50% more). Set to 0 to turn this feature off.

lengthenDelayWhenRunningBy: Lengthens the regen delay while running by the specified 0 to 1 fractional value. At the default setting of 0.5, each second spent running counts as half of a second for ending the cooldown (i.e., 50% less). Set to 0 to turn this feature off. Note that if set to 1, each second running counts for 100% of a second less, i.e., 0 seconds, meaning that the regen cooldown lasts indefinitely while running.

Finally, there are two "hidden" config items that are advanced and not needed by most players. These do not appear in the config file by default but can be added.

timeInterval: How often, in seconds, to actually perform regeneration calculations. By default this is 0.25, meaning every quarter second (or about 15 game ticks). Note that this has no impact on any "per second" values, as they are multiplied accordingly. Minimum value is 0.01, anything less than that will be treated as 0.01. This setting is not really useful unless you have major performance issues. The higher this number is, the less frequently the mod will do any real calculations and therefore the less work it makes your computer do. The lower this number is, though, the "smoother" regeneration will be.

verboseMode: By default it is "false" and controls whether to output debug data every update interval.

2. CHANGELOG

v2.1.0 (09/16/20)
-Settings can now be changed in game using Generic Mod Config Menu.
-Some under the hood changes.
-Slightly different default config values.

v2.0.0 (09/13/20)
-Updated to most recent SMAPI and SDV versions.
-Bunch of new options.

v1.1.2 (02/25/17)
-Added a control for calibration output. In theory using the player version of SMAPI should prevent excessive log output but I've had reports that it happens even with the player version.

v1.1.1 (02/09/17)
-Code tweak changed how cutscenes are detected (by checking whether time can pass).

v1.1.0 (01/24/17)
-Added "percentage mode," cleaned up some code.

v1.0.0 (01/21/17)
-Released.

v0.9.0b (01/21/17)
-Fixed cutscene glitch, added fractional while-running regen option.

3. KNOWN ISSUES

-In the mod config menu, there might be situations where numbers show as being one less than expected (e.g., 89 instead of 90). This is due to some kind of floating point conversion issue and is merely cosmetic, probably.
-During cutscenes, there may be brief windows of time when the conditions of a cutscene enable regeneration to briefly work. This seems to be because cutscenes sometimes briefly allow the game clock to advance time.
-It might be possible to "cheese" the running rate regen for health due to its typically long check time. You're on your honor!

4. FUTURE PLANS

In-game menus. Otherwise I think it's done and just needs to be maintained.

5. CREDITS

This was inspired by (originally, was a direct update to) "Regeneration" by Hammurabi. The permissions involved here are ambiguous. Hammurabi was elaborating on another mod, which in turn elaborated on another, and at each step on the way the authors involved released the source code. This implies to me that the project is fairly "open source" in its operation even if this was not explicitly clear. Hammurabi last updated his version of the mod in 2017. I will comply with any regulation that indicates that I can't publish my own elaboration of the idea. The source code will be available in the install package.