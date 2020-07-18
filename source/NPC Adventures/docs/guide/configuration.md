# Configuration

## Where can I find a configuration file?

`<game folder>/Mods/NpcAdventure/config.json`

Before you first run game with NPC Adventures mod, config.json is not present. Run game first and the you can found `config.json` here. It's a SMAPI feature.

## How to configure

Bellow you can see a configuration schema with default values

```js
{
  "ChangeBuffButton": "G", // {string} Key to change prosthetics buff (like Maru's)
  "HeartThreshold": 5, // {int} Minimum amount of friendship hearts required to successfully recruit a companion (by player)
  "HeartSuggestThreshold": 7, // {int} Minimum amount of friendship hearts required to be offered a recruitment (by an NPC)
  "ShowHUD": true, // {boolean} Show companion HUD?
  "EnableDebug": false, // {boolean} Allows debug (cheat) commands in SMAPI console. Of course, only for development purposes :)
  "AdventureMode": true, // {boolean} Enables an adventure mode. If this is false then mod has the same usage as version 0.9 and older.
  "AvoidSayHiToMonsters": true, // {boolean} Disables/Enables the bubble speech above companions saying "Hi" to the monsters while fighting
  "RequestsWithShift": false, // {boolean} Enables/Disables "ask to follow" dialogs with shift key. If it's enabled, player must hold a shift key before right-click for ask to follow or show companions's choices dialog. Shift key can be user-defined. (disabled by default)
  "RequestsShiftButton": "LeftShift", // {string} Which key is a shift key for hold to recruit
  "AllowGainFriendship": true, // {boolean} Enable or disable friendship points gain every whole hour while adventuring with companion.
  "FightThruCompanion": true, // {boolean} Disable showing companion dialogue while fighting (on left-click). If the player wants to show it, they must do a right-click on a companion.
  "UseCheckForEventsPatch": true, // {boolean} Use patched SDV method `GameLocation.checkForEvents()` for check NPC Adventures events instead of SMAPI's player warped event
  "Experimental": { // WARNING! This section enables experimental features which can affect gameplay and cause errors or unstability.
    "UseSwimsuits": false // {boolean} Allow companions to change to swimsuit in bathroom (disabled by default). Not all companions has own swimsuit!
  }
}
```

## See also

- [Getting started](getting-started.md)
- [Experimental features](experimental.md)
- [Requests with shift key](requests.with-shift.md)
