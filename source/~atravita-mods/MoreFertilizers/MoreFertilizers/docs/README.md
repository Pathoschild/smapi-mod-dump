**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Contributions README
====================================

Most users should download the mod from the [Nexus page](https://www.nexusmods.com/stardewvalley/mods/11837). That said, if you'd like to contribute:

### Additional Content:

Feel free to use these fertilizers as rewards for special orders or quests! Additionally, crops grown via the organic fertilizer carry the [context tag](https://stardewvalleywiki.com/Modding:Items#Categories) `atravita_morefertilizers_organic` and crops grown with the joja fertilizers carry the context tag `atravita_morefertilizers_joja`.

### Radioactive fertilizer exclusions:

If you'd like for the Radioactive fertilizer to exclude your crops, just edit the data asset `Mods/atravita/MoreFertilizers/RadioactiveDenylist`. This is a string-string dictionary where the key is an identifier (either the integer id or the name of the seed) and the value is ignored. For example:

```js
{
    "Format": "1.28.0",
    "Changes": [
        {
            "Action": "EditData",
            "Target": "Mods/atravita/MoreFertilizers/RadioactiveDenylist",
            "Entries": {
                "Pufferchick Seeds": "null",
            }
        }
    ]
}
```

will prevent the radioactive fertilizer from changing a seed into `Pufferchick Seeds`.

### API

Fertilizers that go into HoeDirt are handled by normal game methods (with the exception of the Prismatic Fertilizer, which is also handled internally), but fertilizers that don't are handled entirely within this mod. For integration, there's an API for applying the non-hoedirt fertilizers, located at [../IMoreFertilizersAPI.cs](../IMoreFertilizersAPI.cs). 

### Translations:

This mod uses SMAPI's i18n feature for translations. I'd love to get translations! Please see the wiki's guide [here](https://stardewvalleywiki.com/Modding:Translations), and feel free to message me, contact me on Discord (@atravita#9505) or send me a pull request!

### Compiling from source:

3. Fork [this repository](https://github.com/atravita-mods/StardewMods).
4. Make sure you have [dotnet-5.0-sdk](https://dotnet.microsoft.com/en-us/download/dotnet/5.0) installed.
5. If your copy of the game is not in the standard STEAM or Gog install locations, you may need to edit the csproj to point at it. [Instructions here](https://github.com/Pathoschild/SMAPI/blob/develop/docs/technical/mod-package.md#available-properties).

This project uses Pathos' multiplatform nuget, so it **should** build on any platform, although admittedly I've only tried it with Windows.