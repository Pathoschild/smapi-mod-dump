# Stardew Valley IdlePause Mod

One of the big parts of Stardrew Valley is time management.  The days are reasonably short and figuring out exactly what you want to, or more accurately, will be able to finish in a day is part of the fun.  However, the game time continues to progress even when you're not doing anything unless you've opened up a game menu or are browsing through a chest.  

If you leave your computer or are distracted by something and you don't pause the game, you may come back to find that your entire day has been lost.  You can restart the day but at the cost of redoing all of the things you already completed.

In the worst case, you may have missed an entire day and passed out potentially losing items or a large chunk of cash.  The auto-save means that the only way to get your day back is to manually revert to the backed up save from the previous day that the game makes automatically.

Now you can save yourself the trouble by automatically pausing the game if you have been idle for a certain period of time.

## Installation

1. Make sure you have [installed SMAIP](http://canimod.com/for-players/install-smapi)
1. Download the [latest release on GitHub](./releases/latest) or get it [from NexusMods](http://www.nexusmods.com/stardewvalley/mods/1092).
1. Unzip the mod into the SMAPI Mods folder

## Configuration

```json
{
  "IdleDuration": 5000,
  "OpenMenuOnPause": false,
  "ShowIdleTooltip": true,
  "IdleText": "Zzzz"
}
```

* **IdleDuration** - *Default: 5 seconds (5000ms)* The default length of time you must be idle for the game pauses.
* **OpenMenuOnPause** - *Default: false* If true, the inventory menu will automatically be opened when you're idle.
* **ShowIdleTooltip** - *Default: true* If true, a tooltip will be shown when you are idle.
* **IdleText** - *Default: Zzzz* The text to show in the tooltip when you are idle.