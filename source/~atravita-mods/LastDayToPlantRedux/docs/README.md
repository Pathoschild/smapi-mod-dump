**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Contributions README
====================================

Most users should download the mod from the [Nexus page](https://www.nexusmods.com/stardewvalley/mods/15004). That said, if you'd like to contribute:

### Exclusions:

If you'd like your seeds/fertilizers to show up or if you'd like to ban them for good, you can edit the data asset `Mods/atravita.LastDayToPlantRedux/AccessControl`, which is a string->string dictionary where the key is an identifier (internal seed name name or ID), and the value is either `allow` or `deny` (ignores case). For example:


```js
{
    "Format": "1.28.0",
    "Changes": [
        {
            "Action": "EditData",
            "Target": "Mods/atravita.LastDayToPlantRedux/AccessControl",
            "Entries": {
                "Pufferchick Seeds": "deny",
            }
        }
    ]
}
```

will prevent this mod from reporting information on `Pufferchick Seeds` UNLESS the user has explicitly added `Pufferchick Seeds` to their config file.

### Translations:

This mod uses SMAPI's i18n feature for translations. I'd love to get translations! Please see the wiki's guide [here](https://stardewvalleywiki.com/Modding:Translations), and feel free to message me, contact me on Discord (@atravita#9505) or send me a pull request!

### Compiling from source:

3. Fork [this repository](https://github.com/atravita-mods/StardewMods).
4. Make sure you have [dotnet-5.0-sdk](https://dotnet.microsoft.com/en-us/download/dotnet/5.0) installed.
5. If your copy of the game is not in the standard STEAM or Gog install locations, you may need to edit the csproj to point at it. [Instructions here](https://github.com/Pathoschild/SMAPI/blob/develop/docs/technical/mod-package.md#available-properties).

This project uses Pathos' multiplatform nuget, so it **should** build on any platform, although admittedly I've only tried it with Windows.