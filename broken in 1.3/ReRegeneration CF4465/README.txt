1. CREDITS

This was inspired by "Regeneration" by Hammurabi. The permissions involved here are ambiguous. Hammurabi was elaborating on another mod, which in turn elaborated on another, and at each step on the way the authors involved released the source code. This implies to me that the project is fairly "open source" in its operation even if this was not explicitly clear. Hammurabi was last online in July and isn't responding to questions about this. I will comply with any regulation that indicates that I can't publish my own elaboration of the idea. The source code will be available in the install package.

2. INTRODUCTION

ReRegeneration is one more iteration of the idea of regenerating health and stamina in Stardew Valley. It updates the code to the newest version of the SMAPI, fixes a moderately annoying glitch, and adds new options.

What it fixes: "Regeneration" did not check to make sure that the player was not in a cutscene before activating. To me, this seemed to break the "time economy" of the mod, which trades time for stamina/health, because time does not pass in a cutscene. 

What it adds: Previously, you could set whether you wanted regeneration to work while "running" (i.e., the default movement speed that probably 99% of SDV players use 99% of the time). Now, you can set the degree to which regeneration works while running. Meaning, you can have it not work at all, or work half as fast, or similar. Additionally, you can set up the basic regen rate to work as a percentage of the max value.

3. CONFIGURATION

The config file clusters the options for health and stamina, but they are basically the same in function.

"RegenPerSecond" is the amount to be regenerated per second.

"IdleSeconds" is how long to wait since the last exertion/injury before regeneration begins.

"WhileRunningRate" is a fractional value (between 0 and 1) that is something of a "penalty" to the regen rate while running. If set to 0.4, for example, the value will regenerate 40% as fast while running. Values higher than 1 will be set to 1 (meaning they regenerate as quickly while running as not).

The default values are:

  "healthRegenPerSecond": 0.1,
  "healthIdleSeconds": 15,
  "regenHealthWhileRunningRate": 0.0,

Health will recover by 1 point every 10 seconds after waiting 15 seconds, and won't heal at all while running.

  "staminaRegenPerSecond": 1.0,
  "staminaIdleSeconds": 10,
  "regenStaminaWhileRunningRate": 0.25

Stamina will recover by 1 point every second after waiting 10 seconds, and recovers 75% slower while running.

One new option in v1.1.0 appears at the bottom: "percentageMode" which is false by default. When set to "true," the "RegenPerSecond" values will be interpreted as percentages of the maximum value. This allows the regeneration rate to scale with higher levels of health and stamina. So, at the default values, at the start of the game 2.7 points of stamina will regenerate per second (1% of the starting value of 270 stamina). Health starts at 100 points, so there's no discrepancy there.

Note that if you do enable percentage mode, I'd advise you to change the default value of "staminaRegenPerSecond" to 0.4 because this will result in a starting regen rate of 1.08, which is close to the default in normal mode. 

The "verboseMode" option (which does not even appear in the config file packaged with the mod) should probably be ignored by most players. By default it is "false" and controls whether to output regular calibration data.

4. CHANGELOG

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

5. KNOWN ISSUES

-During cutscenes, there may be brief windows of time when the conditions of a cutscene enable regeneration to briefly work. This seems to be because cutscenes sometimes briefly allow the game clock to advance time.
-It might be possible to "cheese" the running rate regen for health due to its typically long check time. You're on your honor!

6. FUTURE PLANS

-Look into a more robust way of detecting whether a cutscene is playing. This relates to a fairly minor issue, though.
