**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/NermNermNerm/StardewMods**

----

This repository contains the source for the Junimatic Stardew Valley mod.

The intent of this mod is to create a version of [Pathoschild's Automate mod](https://github.com/Pathoschild/StardewMods/tree/develop/Automate)
that's more fun and balanced.  If you either don't use Automate because it feels too overpowered or you do
use Automate and it makes you feel kinda dirty, this mod might be for you.  It makes you do a modicum
of work to enable the junimos and it comes with some limitations:

* Junimos are scared of people and don't work outside the farm.
* All of the loading operations are done one-at-a-time, so it's not as efficient as Automate.
* You need pathways for the Junimos to follow.  (So you can't just pack a shed full of
  kegs and one chest.)
* Unlike Automate, you get no XP or achievements or collection progress for work the Junimos do.
  That's by-design and won't change.
* Right now, the Junimos don't use shipping bins.  That's something that's going to come
  with a future feature.

Note that the speed limitation isn't seriously impactful - you can run hundreds of kegs
and the Junimos will still keep up with it.

## Unlocking the Mod

When you first start, there's no functionality at all.  After the first rain after the first week, you'll
get a message when you wake up that suggests that the rain might have washed the mud off of something.
That's your clue to poke around your farm looking for an old and small Junimo portal.  If you have
trouble finding it, wait a few days and your pet will find it.  Look for a HUD message on entering the
farm to the tune of "I wonder what <pet> is up to".  Find the pet and you'll be close to the hut.
Once you have the hut there's an easy quest chain to gain the ability to craft portals.

There are several quests to convince Junimo "friends" to help you out.  Each quest grants Junimos
that will work with certain kinds of machines.  Avert your eyes if you don't want spoilers,
but the quests are not especially tricky to do, have villager chats that give you powerful
hints, and are things that are likely to just happen in the course of play without even especially
looking for them.

* Having several kinds of animals and at least 2 chickens among them will start the "Animal"
  Junimo quest that unlocks a junimo that will deal with mayonaise machines, cheese presses,
  and so forth.  Note that this Junimo does not pick up eggs, milk cows, etc.  You still
  want an autopicker.
* Growing a giant crop enables the Junimo for kegs, preserves jars, etc.
* Gaining highish friendship with Linus (6 hearts) and highish forestry skill (6)
  starts a quest chain that unlocks tappers, stumps and other tree-related things.
* Killing a giant slime in the mines (on a slime level) will start a quest that unlocks the mining
  Junimo that works with furnaces, crystalariums and other mining-related machines.
* Gaining fishing level 8, having been to level 60 in the mines, and Linus at 6 hearts
  starts a (painful) quest to unlock the fishing Junimo, who works your traps

## Future Plans

As of the 1.0 release, everything basically works.  You can use this mod in place of Automate
and, barring perhaps a few edge cases, everything will work for you.  There are two things
that are on the future-plans list:

### Junimo's Need Maintenance

I'd like to create a "Junimo Happiness Index" -- basically you're going to have to maintain
and continue to grow giant crops, a variety of trees, keep your animals happy, and supply
raisins to your Junimo helpers.  (Maybe other stuff too, if we can think such stuff up.)
Not keeping the Junimos happy will cause them to be less prompt to deal with
machines that need filling/emptying.

### Shiny Stuff

I'd like to make it so that there's a way to tell the Junimos that some things are
not to be put into machines.  The rough sketch is that the Junimo Portal will have an
inventory that designates what stuff is too shiny.  If you have, say, an autopicker,
a portal, a chest and a mayonaise machine in your coop, and you put a gold-quality
void egg in the portal's inventory, then the Junimos will leave gold and iridium
quality void eggs in the autopicker and will only make void mayonaise out of plain
or silver quality void eggs.  You can use those void eggs (still in the autopicker)
to show Krobus and Sebastian just how much you care.

This would plug into the shipping-bin implementation, where Junimos would not ship
anything that either could be used as an input for one of the machines or is in
the shiny stuff list.

## Help Wanted!

This is open source for a reason!  Contributions are welcome, particularly on the art front!
Here are a couple areas where I know help is needed:

1. The dream-sequences events really seem to want some dreamy-effects.  If you know how to make them prettier,
   that'd be great.
2. The "Junimo Happiness Index" scorecard needs to be made.  That'll take some pixel art.

Please discuss this either through issues or Discord before doing any serious work -- wouldn't want to duplicate
effort or create something that isn't accepted because it strays from the mod's vision.

## Translating

Until the mod's content fully stabilizes, it's going to be English-only.

## Compiling

Installing stable releases from Nexus Mods is recommended for most users. If you really want to
compile the mod yourself, read on.

These mods use the [crossplatform build config](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
so they can be built on Linux, Mac, and Windows without changes. See [the build config documentation](https://www.nuget.org/packages/Pathoschild.Stardew.ModBuildConfig)
for troubleshooting.

Simply building the mods will deploy them to your Stardew Valley installation.

Opening the project with [Visual Studio](https://www.visualstudio.com/vs/community/) or [MonoDevelop](https://www.monodevelop.com/)
and then starting the debugger will start Stardew Valley under SMAPI and the debugger.
