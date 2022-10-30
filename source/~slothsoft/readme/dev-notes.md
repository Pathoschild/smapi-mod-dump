**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/slothsoft/stardew-challenger**

----

# Challenger - Dev Notes

_(This page contains notes to myself.)_

- [Starting Development](#starting-development)
- [Release](#release)
- [Test Plan](#test-plan)
- [Used Tutorials](#used-tutorials)

## Starting Development

To start developing this mod, you need to

1. Create [stardewvalley.targets](https://github.com/Pathoschild/SMAPI/blob/develop/docs/technical/mod-package.md#custom-game-path) file with the game folder


## Release

1. Check that all the versions are correct (see point 5)
2. Run _build.bat_, which only really works on my PC, but so what: 
```bat
.\build x.x.x
```
3. Put the contents of _bin/Challenger*.zip_ in a fresh Stardew Valley and test if everything works (see [Test Plan](#test-plan))
4. Create a new tag and release on GitHub, append the ZIPs
5. Increment the version in _manifest.json_ and _build/common.targets_


## Test Plan

- [ ] Extract the _Challenger*.zip_ files into one folder - they should each create their own folder with at least _*.dll_, _*.pdb_ and _manifest.json_ 
- [ ] Check that _Challenger/Readme.html_ is present and looks okay
- [ ] Start Stardew Valley with the mods
- [ ] Test that you get a mail with the **Magical Challenge Object**™
- [ ] Check that the  **Magical Challenge Object**™ changes once you change challenges
- [ ] Wait till 9:00 and check at least one restriction on each challenge


## Used Tutorials

- **General Information:**
  - SMAPI API: [Modding:Modder Guide/APIs](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs)
  - Stardew Valley API: [StawdewValley on GitHub](https://github.com/veywrn/StardewValley)
- **Other Mods:**
  - [Sonozuki's Mods](https://github.com/Sonozuki/StardewMods)
  - [Pathoschild's Mods](https://github.com/Pathoschild/StardewMods) 
  - [CJBok's SDV-Mods](https://github.com/CJBok/SDV-Mods) (CJBCheatsMenu)
  - [spacechase0's Mods](https://github.com/spacechase0/StardewValleyMods) (JsonAssets)
  - [Digus's Mods](https://github.com/Digus/StardewValleyMods)
  - [Dan Volchek's Mods](https://github.com/danvolchek/StardewMods)
  - [ImJustMatt's Mods](https://github.com/ImJustMatt/StardewMods) (Ordinary Capsule)
  - [Platonymous's Mods](https://github.com/Platonymous/Stardew-Valley-Mods) (Arcade Machines)
