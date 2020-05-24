# Debugging mod with commands (and some cheats)

For enable debugging commands you must enable the debug mode in [configuration](../guide/configuration.md).

## List of commands

- `npcadventure_eligible` - Make a player eligible to recruit companions  (Main player only)
- `npcadventure_recruit <NPC name>` - Recruit a companion without check if player is eligible (Mail player only)
- `npcadventure_patches [recheck]` - List all patched SDV methods with harmony patcher by this mod. Add word `recheck` as first argument if you want manually recheck possibly patches conflicts.

Main player only commands will be executed only in singleplayer or on the host in multiplayer game (for the multiplayer version of mod).
