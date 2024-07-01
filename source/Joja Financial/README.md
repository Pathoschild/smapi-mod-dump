**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/NermNermNerm/JojaFinancial**

----

# Joja Financial

This mod is for people who are frustrated with not being able to get their house furnished
without cheating.

## Installing the mod

Unpack the latest release into your Mods folder.  The folder structure should end up as `StardewValley\Mods\Junimatic`.

## Playing with the mod

Early in your playthrough, you'll get a visit from Morris, who offers you a loan to buy the
furniture and wallpaper catalogs.  The payment terms are such that you don't pay much at all
in the first year and the bulk is paid for in Fall and Winter of year 2.  You pay the debt
by phone calls to JojaFinancial.

## Translating the mods

Translations are welcome!  To make them, start with the `defaults.json` file -- it's important
that you keep the order of the keys in there, else when you check it in, the automated system
for generating the i18n files will reorder it, and it'll be difficult to track your changes.

## Compiling the mods
Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mod yourself, read on.

These mods use the [crossplatform build config](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
so they can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
for troubleshooting.

Build the project in [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](https://www.monodevelop.com/) to
build it and deploy it to your 'mod' directory in your Stardew Valley installation.

Launching it under the debugger will start Stardew Valley and your mod will be picked up as in the game.

### Compiling a mod for release
To package a mod for release:

1. Switch to `Release` build configuration.
2. Recompile the mod per the previous section.
3. Upload the generated `bin/Release/<mod name>-<version>.zip` file from the project folder.
