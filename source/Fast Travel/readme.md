**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/DeathGameDev/SDV-FastTravel**

----

# Fast Travel

Fast Travel is a mod which allows the player to fast travel to any location on the map, by simply left clicking the icon.
This allows the player to quickly go from say, the mines, to the blacksmith.

# IMPORTANT!

This mod is no longer regularly maintained, as I now lack the time to continue development / updates for each SDV version. If you would like to see this mod continue, feel free to make a pull request and message me on discord at Mckenon#1692 :)

# Download

The latest version of the mod can be found on **Nexus**:  
[![Download from Nexus](https://i.imgur.com/dJobTR2.png)](https://www.nexusmods.com/stardewvalley/mods/1529/?)

# Mod Support

As this is my first mod, I decided not to add full support for others' mods. I knew that would simply be biting off more than I could chew, so instead I opted to use a configurable system for each warp. These can be edited / added in the config.json file.
As an example, here is the warp for the town square:

```json
{
  "GameLocationIndex": 3,
  "SpawnPosition": "29, 67",
  "RerouteName": "Town",
  "pointId": 1010
},
```

## Note: 
The `pointId`, is identify of point click on map. You can see on console, with debug mode ON typing `ft_helper debugmode 1` on console.

And the `SpawnPosition` is a X, Y position of player inside the map. You can get this typing `ft_helper playerlocation` on console, to get your current location X, Y.

If you had installed a mod which added a giant fountain in the middle of the town square for instance, you could move the spawn position up a few tiles on the Y axis(second number) to stop yourself from spawning inside of it.

# Known Bugs

- Can't fast travel to Quarry. (Currently disabled due to a bug with indoor backgrounds)
- The bus animation doesn't play when fast travelling with a mount

# Bug Reporting

- Contact me on Discord(can be found on the Stardew Valley discord server)
- Post an issue here, on the github.
