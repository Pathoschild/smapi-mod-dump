**Custom Chores** is a [Stardew Valley](https://www.stardewvalley.net/) mod which lets you
add/configure custom chores to the game. It is intended to make Spouses more helpful around
the farm in a helpful and lore-friendly way.

## How to Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. Download/Install [Custom Chores from Nexus](https://www.nexusmods.com/stardewvalley/mods/5175).

## Configuration Options
After you run the game for the first time, the default config.json file will be generated in the CustomChores folder.
Edit whatever setting you want in that file.

option           | description
---------------- | --------------
`DailyLimit`     | Default `3`. The maximum number of chores that a spouse will perform in one day. Set to 0 to disable.
`HeartsNeeded`   | Default `10`. The minimum number of hearts required before a spouse will do chores.
`GlobalChance`   | Default `1.0`. The chance that a spouse will do any chores on a given day.
`EnableDialogue` | Default `false`. Enable a dialogue from your spouse in the morning related to one of the chore(s) they helped out with that day.
`Spouses`        | For each spouse, configure what chores they can do, and what chance they have to do it.

Spouses are configured as a key with the spouse name, and a list of chores.
How it works is that the mod will start on the first chore in the list, and based on the chance given your spouse will either perform the chore or skip to the next one.
It will loop through all the chores in the given order until it has gone through all of them, or the total number of chores performed has reached the limit.

For each chore, you must specify the name of the chore (ChoreName) and the chance that it will be performed (Chance).

The default chores are as follows:

* `FeedTheAnimals`
* `LoveThePets`
* `GiveAGift`
* `PetTheAnimals`
* `RepairTheFences`
* `WaterTheCrops`
* `WaterTheSlimes`

## Modders
This mod exposes a simple API that allows other mods to add chores to the game.

After you get a copy of the API, just call `AddCustomChore` and pass it an instance of your object that implements an `ICustomChore`.