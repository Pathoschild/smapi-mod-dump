**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Contributions README
====================================

Most users should download the mod from the [Nexus page](https://www.nexusmods.com/stardewvalley/mods/10186). That said, if you'd like to contribute:

### Translations:

This mod uses SMAPI's i18n feature for translations. I'd love to get translations! Please see the wiki's guide [here](https://stardewvalleywiki.com/Modding:Translations), and feel free to message me, contact me on Discord (@atravita#9505) or send me a pull request!

### Integration with other mods.

#### Denylist

Don't want this mod to spawn *your* fruit? If you use Content Patcher to add an entry to `Mods/atravita_FarmCaveSpawn_denylist` that looks like

```
{
    "Action": "EditData",
    "Target": "Mods/atravita_FarmCaveSpawn_denylist",
    "Entries": {
        "your.uniqueID": "Comma,Seperated,List,Of,Fruit,Names"
    }
}
```

this mod won't spawn your fruit.

#### AdditionalLocations

Want this mod to spawn fruit in a mod-added location? Just use Content patcher to add an entry to `Mods/atravita_FarmCaveSpawn_additionalLocations` that looks like

```
{
    "Action": "EditData",
    "Target": "Mods/atravita_FarmCaveSpawn_additionalLocations",
    "Entries": {
        "your.uniqueID": "Comma,Seperated,List,Of,Location,Names"
    }
}
```

to add your location as a location fruit will spawn. (To limit the spawn location, add `:[(x1;y1);(x2;y2)]` after the location name - ie `Custom_NewLocation:[(3;4);(57;54)]`). Note - semicolons, not commas. 

### Compiling from source:

3. Fork [this repository](https://github.com/atravita-mods/StardewMods).
4. Make sure you have [dotnet-5.0-sdk](https://dotnet.microsoft.com/en-us/download/dotnet/5.0) installed.
5. If your copy of the game is not in the standard STEAM or Gog install locations, you may need to edit the csproj to point at it. [Instructions here](https://github.com/Pathoschild/SMAPI/blob/develop/docs/technical/mod-package.md#available-properties).

This project uses Pathos' multiplatform nuget, so it **should** build on any platform, although admittedly I've only tried it with Windows.