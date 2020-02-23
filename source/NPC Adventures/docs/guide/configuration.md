# Configuration

## Where can I find a configuration file?

`<game folder>/Mods/NpcAdventure/config.json`

Before you first run game with NPC Adventures mod, config.json is not present. Run game first and the you can found `config.json` here. It's a SMAPI feature.

## How to configure

Bellow you can see a configuration schema with default values

```js
{
  "ChangeBuffButton": "G", // {string} Key for change prostetics buff (like Maru's)
  "HeartThreshold": 5, // {int} Minimum of friendship hearts to companion agree with invitation to adventure (recruitment by player)
  "HeartSuggestThreshold": 7, // {int} Minimum of friendship to companion can suggest (invite) you to adventure (recruitment by NPC)
  "ShowHUD": true, // {boolean} Show companion HUD?
  "EnableDebug": true, // {boolean} Allows debug (cheat) commands in SMAPI console. Of course, only for development purposes :)
  "AdventureMode": true // {boolean} Enables an adventure mode. If this is false then mod has a same usage as version 0.9 and older.
}
```

## See also

- [Getting started](getting-started.md)
