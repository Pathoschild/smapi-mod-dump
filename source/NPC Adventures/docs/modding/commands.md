**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/PurrplingMod**

----

# Debugging mod with commands (and some cheats)

For enable debugging commands you must enable the debug mode in [configuration](../guide/configuration.md).

## List of commands

- `npcadventure_eligible` - Make a player eligible to recruit companions  (Main player only)
- `npcadventure_recruit <NPC name>` - Recruit a companion without check if player is eligible (Mail player only)
- `npcadventure_patches [recheck]` - List all patched SDV methods with harmony patcher by this mod. Add word `recheck` as first argument if you want manually recheck possibly patches conflicts.
- `npcadventure_debug set|unset|list <debug flag name>` - Set, unset or list debug flags.

Main player only commands will be executed only in singleplayer or on the host in multiplayer game (for the multiplayer version of mod).

### Available debug flags

- `forager.pickAlways` Forager always find a forage.
- `forager.shareAlways` Forager always share found forages with farmer.
