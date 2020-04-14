# Time of the Prairie King

This is a mod for Stardew Valley that displays the time of day in _Journey of the Prairie King_ during multiplayer. The goal is to bring the same feature that is part of the _Junimo Cart_ game to _Journey of the Prairie King_. Now you don't have to leave the game suddenly when you realize it's 1:20am and your friends are yelling at you to get back to the farm!

The time will only appear in a multiplayer game with other players joined. This is because, when playing _Journey of the Prairie King_, time does not progress unless you are playing in a multiplayer game with at least one other player present.

You can customize where the time appears, either with the HUD (by default) or in any of the four corners of the screen. You can also customize the color of the time.

The mod is currently compatible with Stardew Valley 1.4

### Installation
1. Install the latest version of [SMAPI](https://smapi.io/) _(requires 3.0.0 or above)_
2. Download this mod and unzip the `TimeOfThePrairieKing` folder into `Stardew Valley/Mods`. The latter can be found in your game's installation directory
3. Run the game using SMAPI


### Configuration options

| Name        | Datatype | Purpose                                                     | Default Value |
|-------------|----------|-------------------------------------------------------------|---------------|
| `Enabled`     | boolean  | Whether or not the mod is enabled                           | `true`          |
| `Indentation` | decimal  | The number of pixels to indent the time (_only applies to the corners_) | `25.0`            |
| `MinigameHud` |          |                                                             |               |
|  ➥ `Show`     | boolean  | Whether or not to draw the time on the minigame's HUD       | `true`          |
|  ➥ `HexColor` | string   | The color to draw the time on the minigame's HUD            | `#CD853F`      |
| `TopLeft`     |          |                                                             |               |
|  ➥ `Show`     | boolean  | Whether or not to draw the time in the top left corner      | `false`         |
|  ➥ `HexColor` | string   | The color to draw the time in the top left corner           | `#808080`      |
| `TopRight`    |          |                                                             |               |
|  ➥ `Show`     | boolean  | Whether or not to draw the time in the top right corner     | `false`         |
|  ➥ `HexColor` | string   | The color to draw the time in the top right corner          | `#808080`      |
| `BottomLeft`  |          |                                                             |               |
|  ➥ `Show`     | boolean  | Whether or not to draw the time in the bottom left corner   | `false`         |
|  ➥ `HexColor` | string   | The color to draw the time in the bottom left corner        | `#808080`      |
| `BottomRight` |          |                                                             |               |
|  ➥ `Show`     | boolean  | Whether or not to draw the time in the bottom right corner  | `false`         |
|  ➥ `HexColor` | string   | The color to draw the time in the bottom right corner       | `#808080`      |


### Development

Want to contribute? Great! Open up an issue and if we both agree it's a good addition, fork the mod and make a pull request! Or just fork it to put your own spin on it!


### To-do:

 - Fix the positioning when the user changes their game zoom level to be 100% consistent instead of 85% consistent
 - Maybe one day add an easier way to customize the color

#

###### _Uses the GPL-3.0 License_
