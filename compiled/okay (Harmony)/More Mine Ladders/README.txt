Thanks for downloading my mod!

To configure the ladder chance and/or disable the mod, you need to change the values in config.json(which should be in the same folder you found this file)

:: Config options ::
{
  "Enabled": true, // I wonder what this does! Spooky.
  "affectedByLuck": false, // If enabled, your dropLadderChance will also be affected by daily/player luck levels.
  "dropLadderChance": 0.1 // The chance of a ladder spawning when a rock is broken. Default: 10%. Goes from 0-1
}

:: Commands ::
makeladder - Creates a ladder at your farmer's current position.
mml_reloadconfig - Reloads MML's configuration.