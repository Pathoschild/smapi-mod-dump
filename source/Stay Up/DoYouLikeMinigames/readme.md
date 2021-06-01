**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/su226/StardewValleyMods**

----

# Do You Like Minigames?

Now you can play minigames anywhere and anytime.

## Download

Sorry, currently you can only download source code and compile by yourself.

## Usage

Press G to show minigames menu.

## Config

```jsonc
{
  "key": "G", // The key to show minigames menu.
  "checkCinema": false, // Crane cannot be played when there's no cinema.
  "checkClubCard": false, // Casino games cannot be played when you have no club card.
  "checkFair": false, // Fair games cannot be played before first fair.
  "checkFriendship": false, // JOTPK cannot be played with Abigail when friendship level is lower than 2.
  "checkIsland": false, // Darts cannot be played when island is unavailable.
  "checkMoney": false, // Crant cannot be played when player doesn't have enough money. (This also enable the cost of crane.)
  "checkQiCoin": false, // Casino games cannot be played when player doesn't have enough Qi coin.
  "checkSkullKey": false // Minecart cannot be played when skull cave is locked.
}
```

