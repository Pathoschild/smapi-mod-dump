# Sprint Sprint Configuration
([← README](../README.md))

There are 2 ways to edit the config file
1. Download optional dependency Generic Mod Config Menu and you can configure the mod to your liking while in the title screen on a much friendlier user interface (A button is added to the title screen in the bottom left to access the config menus.)
2. Run the game with this mod installed once to generate a config.json in your Stardew Valley/Mods/Sprint Sprint/ folder and directly edit the config.json file with a text editor (i.e. Notepad, Notepad++, Sublime Text, etc.)

### Configuration Options
This list is mostly for people who are directly editing the `config.json` file found in this mod's folder after running the game once. For a more friendlier way to edit the config, do option 1 above.
| Option | Value Type | Default Value | Description |
| ------ | ---------- | ------------- | ----------- |
| SprintKey | Button/Keybind | `Left Shift` | The key to hold in order to sprint. See [accepted keybind values here](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Available_bindings) |
| HoldToSprint | Boolean (true/false) | `true` | Hold down or toggle `SprintKey` to sprint |
| SprintSpeed | Integer | `8` | The player sprinting speed |
| HorseSpeed | Integer | `5` | The default horse speed, since sprinting is disabled when riding a horse |
| StaminaDrain.Enabled | Boolean (true/false) | `true` | If player loses stamina when sprinting |
| StaminaDrain.StaminaCost | Float (number w/decimals) | `1.0` | The amount of stamina that is loss every second when sprinting |
| NoSprintIfTooTired.Enabled | Boolean (true/false) | `true` | If sprinting should be disabled when you are low on stamina |
| NoSprintIfTooTired.TiredStamina | Float (number w/decimals) | `20.0` | If your stamina goes below this value, you are unable to sprint |
