**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/M3ales/RelationshipTooltips**

----

# Relationship Tooltips [![](http://cf.way2muchnoise.eu/298040.svg)](https://stardewvalley.curseforge.com/projects/298040)
A tooltip mod which displays NPC friendship/relationship information on mouse hover, as well as gifting information.

## Features
## API
As of RT 2.0.0-beta.2 you can now add your own text to the relationship tooltips mod. It currently will search for *anything* inheriting from `StardewValley.Character` and allow you to provide conditions for different text displays. It's currently limited to text, but hopefully in future this will expand to Images.

An example implementation as well as setup walkthrough is up [here](https://github.com/M3ales/RTExampleMod).
### Notes on API
* Don't use Priorities which are single increments of eachother unless you are **EXPLICITLY** intending them never to have anything run inbetween.
* Try stay away from Priorities which are the same, since their ordering is random dependant on the load order.
* You can see what other elements have been registered - and their priorities by iterating through `e.Relationships`.

### Gifting
Gifting is tracked while the mod is installed, and suggestions on responses for gifts that have been given will be made. This can optionally be turned off to provide a wiki like companion which tells you exactly what everyone wants. To disable you will need to set the property ```playerKnowsAllGifts=true``` in ```StardewValley>Mods>RelationshipTooltips>config.json```

### Farm Animals
Farm Animals now display tooltips which let you know your friendship (heart level) with them, as well as if you've petted them today - via tooltip.

### Pets
Pets display a simple nametag on hover, because we already know your pet loves you :3.

## Credits
* SMD Discord -- Thanks to @ashzification for the original idea for an NPC relationship tooltip.
* Built on [SMAPI 2.6](https://github.com/Pathoschild/SMAPI).
