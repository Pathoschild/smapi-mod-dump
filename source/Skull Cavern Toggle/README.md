**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TheMightyAmondee/SkullCavernToggle**

----

## Skull Cavern Toggle

This mod allows the Skull cavern difficulty to be toggled between the harder version and the normal version. By default this can only be done after Skull Cavern Invasion is complete, similar to the Shrine of Challenge.

After Skull Cavern Invasion is completed a shrine will appear to the left of the door to the Skull Cavern to allow the difficulty of the Skull Cavern to be toggled. The shrine will have different eye colours depending on the difficulty for quick reference, yellow for normal and red for dangerous.

If you prefer the original approach (using a hotkey to toggle the difficulty), this can be re-enabled by changing ShrineToggle to false in the config. In that case, the key listed in ToggleDifficulty (default is Z) will be used for toggling the Skull Cavern.

**The config:**

- The config will generate after the game is run once

- ShrineToggle determines whether the shrine will be used to toggle the difficulty, default is true. If set to false, the hotkey is used instead

- ToggleDifficulty determines which key, or key combination, is pressed to toggle difficulty if ShrineToggle is false, default is Z

- MustCompleteQuest determies whether Skull Cavern Invasion must be completed to allow difficulty to be toggled, default is true

**Some things to note:**

- This mod requires SMAPI 3.9 or above to be installed

- The change is immediate (usually, see note below), and applies to everyone in multiplayer

- Because of multiplayer syncing, in multiplayer the difficulty of the caverns won't update until the in-game clock changes time. It also won't update if time is frozen

- When using the shrine, the cursor must be used to select it, so if you're using a gamepad controller use the right thumbstick to move the cursor over the shrine

- Difficulty can't be toggled when Skull Cavern Invasion is an active special order to prevent issues


**Version 1.1.0**
- Introduced the Shrine of Greater Challenge to toggle the difficulty of the caverns if ShrineToggle is true in the config
- Added new config options
- Players in multiplayer now informed when the difficulty changes
