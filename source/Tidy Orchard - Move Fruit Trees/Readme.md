**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/huancz/SDV-TidyOrchard**

----

# Overview
The base game has several points when you will probably curse your lack of foresight while placing fruit trees. I always do. Even on n-th playthrough when I should know better. With this mod, you don't have to.

# How does this work?
1. hit tree with a hoe, to dig it up. You will get a sapling back
2. plant it where you want it to grow (you still have to obey game restrictions on surrounding objects. Don't dig up mature tree surrounded by tea bushes, thinking you can plant it back)
3. there is no 3
4. profit (as much profit as you can get from fruit, which... is not very much, even on iridium. But your OCD can rest easy)

It only works with nearly mature trees (7 days before they give fruit, stage 3). Younger trees can be dug up with hoe in base game (giving only common sapling), and this mods preserves that behavior.

# Installation / uninstallation

Install via the usual methods - download a zip, or with Vortex (Vortex is safe for THIS mod. It might break your other more complex mods though). Only dependency is SMAPI 4.0.0 or greater.

Uninstallation is safe, just delete the mod folder. It doesn't permanently change anything. You can install, move your trees and uninstall without consequences.

# Compatibility

The mod uses Harmony which makes it inherently suspicious (SMAPI log will mention that). That said, it should be pretty safe, I tried to keep changes to a minimum. It only attaches little piece of data to the special saplings (and makes them not stack).

- [tested] you can keep the sapling in your inventory overnight. It won't mature, but you will still get correct tree back when you plant it, as old as it was when you dug it up
- [untested] you should be able to pass the sapling to a player who doesn't have this mod installed, and back. Unless they plant it or mix it with stack of other saplings, it should still work. If they DO plant it, you'll get normal tree back that has full season before it will give fruit.

# Further ideas
- limit digging up trees to better quality hoes. That alone is easy, but it would probably require a configuration option, and integration with GenericModConfigMenu. Too much work.

# Alternatives

[Tree transplant](https://www.nexusmods.com/stardewvalley/mods/1342). AFAICT it only allows you to move trees around in one location, through Robin's build menu. If you want to migrate your trees to greenhouse, or Ginger island - you'd still have to chop them down and wait until they mature again in new location.
