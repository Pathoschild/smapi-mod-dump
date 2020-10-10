**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Drynwynn/StardewValleyMods**

----

# Stardew Valley Mods

A collection of my mods for [Stardew Valley](https://stardewvalley.net/).  See the mod directories for more information.

[Fishing Automaton](./FishingAutomaton) - Play the fishing game (kind of) like an actual human.\
[No Added Flying Mine Monsters](./NoAddedFlyingMineMonsters) - Remove the random monsters that appear off screen inside the mines.\
[D's Stardew Library](./DsStardewLib) - A shared library of common code used in the mods.

## Using the Source

If you're looking to use the mod, you should go to the mod directory you're interested in and follow the link to the Nexus Mods page; this is probably
what you want.

See below if you want to contribute with a pull request.

### PR Prerequisites

All mods use SMAPI and some of them use Harmony to patch the IL.
* SMAPI is pulled in via NuGet.  I'm pretty sure this comes in via the project settings, you can just go to your NuGet package configuration and install.
* There is a NuGet entry for Harmony, but (as of this writing) there is no public NuGet Harmony repository.  You must needs download the current Harmony release,
unpack it, and add the resulting repository to your NuGet configuration.

## Built With

* [SMAPI](https://smapi.io/) - Stardew Valley Modding API
* [Harmony](https://github.com/pardeike/Harmony) - Non-destructive IL injection

## Contributing

Install the prereqs and submit a pull request.

## Versioning

[SemVer](http://semver.org/)

## Authors

* **Jason Burns** - *Initial work* - [Drynwynn](https://github.com/Drynwynn)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* PathosChild for SMAPI
