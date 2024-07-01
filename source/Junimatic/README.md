**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/NermNermNerm/Junimatic**

----

# Junimatic

This is a mod for people who can't play without [Pathoschild's Automate mod](https://github.com/Pathoschild/StardewMods/tree/develop/Automate),
but feel real dirty for it.  Its effect is so awesome and you just get it for free...

This mod requires a mildly demanding series of quests be done, and then you get an automation
system that is decidedly less efficient, but hopefully more engaging.  As part of that
"feel less like cheating/playing the game for you" idea, some things in the main Automate
mod will not be replicated.  For example, in Automate, you can rig up an automation of
traps and level yourself up to max-level fishing without ever casting a line after you
unlocked traps.  This mod's intention is to not do that - anything collected by the
Junimo's won't do anything for the player.

Think of this mod as *Early Access* - it is incomplete and may be janky at times.
If you want stability, Automate is the way to go.

## Installing the mod

Unpack the latest release into your Mods folder.  The folder structure should end up as `StardewValley\Mods\Junimatic`.

## Playing with the mod

After the first rainy day after the first week or so, you'll get a message in the morning about
the rain possibly having washed stuff away.  After that, you can find a odd-looking weed on the
farm that can be picked up.  That will start you on a quest line to see the wizard where you get
instructed on how to make the portals.

To actually get things to happen, you need to enlist some Junimos.  Each junimo will only handle
certain classes of machines:

* The Mining Junimo (furnaces and the like) is unlocked by finding a quest object inside a big slime.  In the mines,
  you'll only find them on slime-infested levels.
* The Crops Junimo is unlocked by growing a giant crop.
* The Animals Junimo is unlocked by having several different types of animals including at least 2 chickens.
* Tree Junimos (tappers, stumps, etc.) are not implemented yet.
* Fishing Junimos (traps, recycling machines, bait machines...) are not implemented yet.

## Placement

Unlike Automate, you have to create pathways for the Junimos to follow to do their thing.  Outdoors,
you must place flooring adjacent to the hut and create paths to each machine and chest you want them
to work with.  Right now, it considers all the floor tiles one tile north, south, east and west of the
portal to be legit walkable tiles.  It won't step on tiles other than these.  (Perhaps someday we should
make the tiles configurable.)  The safe play is to use a single style of flooring for where you want
your Junimos to travel outdoors.  Indoors, Junimos are happy to walk on bare floor.  You can use different
styles of walkways to cordon off areas and yet leave them easily accessible to you.

There are actually two modes of operation for the mod - one where the Junimos are actively automated
and the other is where they're operating invisibly to you.  You shouldn't really see that often in
real gameplay, but be aware that there can be bugs that happen only in one regime or the other.
Another quirk that's true now but may not stay this way is that animated Junimos will only deal with
chests and machines that they can walk to a cardinal direction to.  That is, if you have a walkway
that ends with machines on all sides, the animated Junimos will not touch the ones in the corners,
but off-screen Junimos will.

## Help Wanted

If you'd like to help with the mod, please get in touch either with a github issue or through Discord.
Someone with an interest in working on the art would be most welcome.  Here are some examples:

* The dream-sequence events don't have good backdrops and feel stilted and broken.
* It'd be fun to make the Junimos need some maintenance.  In particular I think they should
  work harder if you had permanent giant crops, permanent mystic and mahogany trees, and,
  of course, a steady source of raisins.  I'd like there to be a "Junimo Happiness Scorecard"
  accessible somewhere.
  
## Translating the mods
There are two ways you can do it.  One you're probably already familiar with, which is just create
a copy of `default.json` and translate the English, test it in-game and send it back to me either
on discord, Nexus, or here on GitHub via an Issue.  If you're into something fancier, you can
use the [new language template](https://github.com/NermNermNerm/Junimatic/blob/main/Junimatic/i18n/new-language-template.json).
You can either edit it as the comment describes or use it as a reference guide for the old
copy-default.json approach.

If you do send me a language json, be sure to note exactly which version of the mod you're
running!

Oh, and if you're a developer as well, you can clone the repo and build from the tip of `main`.

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
