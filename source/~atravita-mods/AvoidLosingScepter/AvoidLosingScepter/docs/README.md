**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Contributions README
====================================

Most users should download the mod from the [Nexus page](https://www.nexusmods.com/stardewvalley/mods/11856). That said, if you'd like to contribute:

### Deprecation:
This mod will be eaten by the game in 1.6; if you use the context tag `atravita_no_loss_on_death` switch to `prevent_loss_on_death`.

### Extending the mod.

Have an item you don't want to lose on death? Simply add the context tag ~~`atravita_no_loss_on_death`~~ `prevent_loss_on_death` to it! (Use `prevent_loss_on_death`, that'll be what the game itself uses in 1.6).

### Translations:

This mod uses SMAPI's i18n feature for translations. I'd love to get translations! Please see the wiki's guide [here](https://stardewvalleywiki.com/Modding:Translations), and feel free to message me, contact me on Discord (@atravita#9505) or send me a pull request!

### Compiling from source:

3. Fork [this repository](https://github.com/atravita-mods/StardewMods).
4. Make sure you have [dotnet-5.0-sdk](https://dotnet.microsoft.com/en-us/download/dotnet/5.0) installed.
5. If your copy of the game is not in the standard STEAM or Gog install locations, you may need to edit the csproj to point at it. [Instructions here](https://github.com/Pathoschild/SMAPI/blob/develop/docs/technical/mod-package.md#available-properties).

This project uses Pathos' multiplatform nuget, so it **should** build on any platform, although admittedly I've only tried it with Windows.