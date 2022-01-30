**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/dmarcoux/DisplayEnergy**

----

# <a href="https://github.com/dmarcoux/DisplayEnergy">dmarcoux/DisplayEnergy</a>

_DisplayEnergy_ is a [_Stardew Valley_](https://www.stardewvalley.net/) mod
built with [_SMAPI_](https://smapi.io/) mod to permanently display the player's
current and max energy. This mod supports split-screen.

## How to Install This Mod

Refer to the [_SMAPI_ player
guide](https://stardewvalleywiki.com/Modding:Player_Guide) to learn how to
install _SMAPI_ mods.

This mod is available on
[ModDrop](https://www.moddrop.com/stardew-valley/mods/1087175-displayenergy) and
[NexusMods](https://www.nexusmods.com/stardewvalley/mods/10662).

## How This Mod Was Created

Refer to the [_SMAPI_ modder
guide](https://stardewvalleywiki.com/Modding:Modder_Guide) to learn how to
create _SMAPI_ mods. This is a simplified version of the guide and without using
any GUI from an IDE like _Visual Studio_.

1. Create the project's directory and go into this directory:

```bash
mkdir DisplayEnergy && cd DisplayEnergy
```

2. Spin up the development environment and wait for the container to be built:

```bash
podman-compose up
```

3. Create a solution:

```bash
dotnet new sln
```

4. Create a _Class Library_ project and add it to the solution:

```bash
dotnet new classlib -o DisplayEnergy && dotnet sln add DisplayEnergy/DisplayEnergy.csproj
```

5. Delete the _Class1.cs_ or _MyClass.cs_ file (it was generated in the previous step, but it's not needed):

```bash
rm -f DisplayEnergy/Class1.cs DisplayEnergy/MyClass.cs
```

6. Add [_SMAPI_ package](https://smapi.io/package/readme) to allow modding with _SMAPI_:

```bash
dotnet add DisplayEnergy package Pathoschild.Stardew.ModBuildConfig --version 4.0.0
```

7. Create the _ModEntry.cs_ and _manifest.json_ files (see examples in the [_SMAPI_ modder guide](https://stardewvalleywiki.com/Modding:Modder_Guide)):

```bash
touch DisplayEnergy/ModEntry.cs DisplayEnergy/manifest.json
```

## How to Develop This Mod

1. Clone Git repository
2. With [podman-compose](https://github.com/containers/podman-compose), spin up development environment: `podman-compose up`.
3. Once inside the container, change whatever needs to change, then build the project with `dotnet build`.
4. The mod should now be installed in _Stardew Valley_. Start the game to test the mod.
5. When satisfied with the changes, update the [semantic version](https://semver.org/) in the [mod manifest](./DisplayEnergy/manifest.json) and push all changes.
6. Create a release with [_GitHub CLI_](https://cli.github.com/): `gh release create VERSION_NUMBER ./DisplayEnergy/bin/Debug/**/*.zip`
7. Download the ZIP archive _DisplayEnergy.VERSION_NUMBER.zip_ from the new release and upload it to [ModDrop](https://www.moddrop.com).

## Acknowledgements

_DisplayEnergy_ does exactly the same as the
[EnergyCount](https://www.nexusmods.com/stardewvalley/mods/4272) mod, except
that it supports split-screen. Thank you _Crystallyne_ for creating that mod.
