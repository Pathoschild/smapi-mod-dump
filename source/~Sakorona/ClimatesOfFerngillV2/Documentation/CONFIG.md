**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Sakorona/SDVMods**

----

# Configuration Options

- Verbose: This turns on debug logging. Note that the mod is *very chatty* if you turn on this feature. Verbose logs things to the console whenever possible
	Valid options: **false**, **true**. Default: **true** in alpha,beta and delta builds. **false** in release builds.

- WeatherMenuToggle: This sets what key toggles the weather menu. Default is the 'Z' key
	Valid options: Any SButton key. (more information TBA)

- Climate: The climate file the mod uses to generate weather and other elements.
	Valid options: **normal**, and any other climate in the assets/climates folder. Default: **normal**

- HazardousEvents: Toggles if events that can inflict harm are enabled.
	Valid options: **false**, **true**. Default: **true**

- FrostThreshold: This sets the threshold for where it gets cold enough to form frost. This temperature is set in Celsius.
	Valid option: Any double, with the default being **2.22**. Note that higher temperatures make frost more likely, and lower temperatures make frost less likely. 
		
- WiltThreshold: This sets the threshold for where it gets hot enough to wilt crops (requiring a second watering). This temperature is set in Celsius
   Valid option: Any double, with the default being **33**. Note that higher temperatures make wilting less likely, and lower temperatures make wilting more likely