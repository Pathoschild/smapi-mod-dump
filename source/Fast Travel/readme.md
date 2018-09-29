# Fast Travel
Fast Travel is a mod which allows the player to fast travel to any location on the map, by simply left clicking the icon.
This allows the player to quickly go from say, the mines, to the blacksmith.

# Download
The latest version of the mod can be found on [Nexus](https://www.nexusmods.com/stardewvalley/mods/1529/?).

# Mod Support
As this is my first mod, I decided not to add full support for others' mods. I knew that would simply be biting off more than I could chew, so instead I opted to use a configurable system for each warp. These can be edited / added in the config.json file.
As an example, here is the warp for the town square:
```json
    {
      "MapName": "Town Square",
      "GameLocationIndex": 3,
      "SpawnPosition": "29, 67",
      "RerouteName": "Town"
    }
```
If you had installed a mod which added a giant fountain in the middle of the town square for instance, you could move the spawn position up a few tiles on the Y axis(second number) to stop yourself from spawning inside of it.

# Known Bugs
- Can't fast travel to Quarry. (Currently disabled due to a bug with indoor backgrounds)
- The bus animation doesn't play when fast travelling with a mount

# Bug Reporting
- Contact me on Discord(can be found on the Stardew Valley discord server)
- Post an issue here, on the github.
