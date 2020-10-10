**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Sakorona/SDVMods**

----

# Climates Advanced Documentation

## 1. General Notes

### 1a. Frost
Frost, defined as when a freeze is left on plants, is a difficult thing to pin down. Some say it's below 2.2C (~36 F), others below 0C (32 F). We define it here as 2.2C(~36 F) by default, and if I ever need to, a *hard* frost is below -3.33C.

This matters because we want to say what the default max chance of frost is in the mod. Without the hazardous events toggle, this does nothing, however.

### 1b. Wilting

Formally called heatwaves, this controls the threshold where plants wilt and require watering again. If hazardous events are toggled, this can kill crops if they are not watered.

By default, we pin this at 33C. 

### 1c. Wind

We define winds by the Beaufort wind scale, with some simplications. There is an important note here. We use the JMA categories, since Harvest Moon is based in Japan. (Yes, this matters.)

While it's very unlikely we'll be able to be this precise in game, this is the basis for descriptions.

* Calm is 0-1 mph (0-1.61 kph)
* Light breeze is 1-4 mph (1.61-6 kph)
* Gentle breeze is 4-12 mph (6-19 kph).  
* Moderate breeze is 12-18 mph (19-29 kph).
* Fresh breeze is 18-24 mph (29-39 kph)
* Strong breeze is 24-31 mph (39-49 kph)
* Near gale is 31-38 mph (49-61 kph)
* Gale is 38-46 mph (61-74 kph)
* Severe gale is 46-54 mph (74-87 kph)
* Tropical Storm is 54-63 mph (87-102 kph)
* Severe Tropical Storm is 63-72 mph (102-118 kph)
* Category 1 Typhoon is 73-96 mph (118-156 kph)(Equiv to Cat1/2 Hurricanes.)
* Category 2 (Very strong) Typhoon is 97-123mph (156-200 kph) (Equiv to Cat 3 and 1/3 Cat 4 Hurricane)
* Category 3 (Violent) Typhoon is 123mph+ (200kph+) (Equiv to 2/3 Cat 4 and Cat 5 Hurricane)

## 2. Temperature Mechanics


##
