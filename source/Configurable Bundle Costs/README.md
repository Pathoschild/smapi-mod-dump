**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/dtomlinson-ga/ConfigurableBundleCosts**

----

### Configurable Bundle Costs [CBC]
*by Vertigon*

[NexusMods](https://www.nexusmods.com/stardewvalley/mods/9444)

This mod allows the player to configure the cost of the Joja Development and Vault bundles, as well as the cost of building the Movie Theater.

Additionally, it supports content packs so that values can be changed dynamically based on in-game happenings.
### Usage
Upon downloading this mod, and running the game once with it in the Mods folder, a `config.json` file will be generated in this mod's folder. You can adjust the values in the config file to adjust the amount of money needed to purchase bundles in-game.

Alternatively, you may download a content pack for this mod and place it in your Mods folder to have the config values handled automatically, in a manner determined by the content pack you download.
### Dependencies
* [SMAPI](https://smapi.io/)  v3.12.0 or higher is a required dependency.
* [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915) v1.23.0 or higher is a required dependency.
* [GMCM](https://www.nexusmods.com/stardewvalley/mods/5098) v1.4.1 or higher is an optional dependency which provides the in-game configuration menu pictured below.

### Configuration
![Fancy menu provided by GMCM](https://i.imgur.com/tcTeCHd.png)

CBC uses a *hierarchical config system*. This means that values may be provided by one of several different sources. In order of priority,

**config.override.json** overrides all config values and patches if it exists. Only the values you wish to override should be defined in this file. It is not necessary to have this file.
This is a perfectly valid config.override.json:

    {
		"Joja": {
		  "busCost": 1000,
		  "greenhouseCost": 2000
		},
		"Vault": {
			"applyValues": false
		}
	}

Loaded content packs may provide a **Default** section which is lower priority than config.override.json but higher priority than the standard config.json. It is not necessary for a content pack's Default section to define all (or any) config values. For more information, see the *Content Pack Structure* section below.

Finally, the **config.json** which is created when the mod runs for the first time takes lowest priority. This includes the GMCM menu pictured above - values defined in GMCM only take effect if the value is not overridden by config.override.json or by a content pack's Default values. If a value is not defined in config.json, the game will use the vanilla value.

**In summary**, the game will search for a config value in `config.override.json`. If it does not find one, it will search all installed content packs' `Default` values. If it is not found there, it will search `config.json`. If it is not found there, the game will use the vanilla value for that option.

### Content Pack Structure

A content pack consists of the following files:
`manifest.json`: This is your bog-standard manifest file. See [the wiki](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest) for more information.
`content.json`: This is the file that contains any **Default** values, as well as any **Patches**. Note that it is not necessary to have a Default section, nor is it necessary to have any Patches.

If you choose to include any Default values, the section will take the same form as this mod's `config.json`. Patches take a form similar to a Content Patcher patch.

Here is an annotated `content.json`:

    {
      // Default, like config.json, is made up of two sections: "Joja" and "Vault"
      "Default": {
        "Joja": {
          // you only need to supply the values you want to set
          // bear in mind that config.override.json takes priority
          
          // applyValues determines whether or not config values and Patches are applied
          // if set to False, the vanilla values will be used instead
          "applyValues": true,
          
          // Possible config values are listed below
          "busCost": 500,
          "minecartsCost": 15000
	  
	  // Values which are omitted, or set to null, will be ignored
        },
        "Vault": {
          // if any fields are omitted, the game will use values from config.json
          // See the Configuration heading above for more details
          
          "bundle3": 10,
          "bundle4": 25
        }
      },
      // The Patches section is a list of as many Patches as you wish to include.
      "Patches": [
      
        // Each Patch contains a Name, Action, Value, Target, Frequency, and Conditions
        {
          // If you do not provide a Name, the mod will automatically generate one for you
          "Name": "Test Patch",
          
          // Action can be one of the following: Set, Add, Subtract, Multiply, Divide
          "Action": "Add",
          
          // Value can be any float (decimal) or integer (whole) number, positive or negative
          "Value": 1000,
          
          // Target can be any config value besides 'applyValue'
          "Target": "bundle1",
          
          // Frequency can be one of the following: Once, Daily, Weekly, Monthly, Yearly
          "Frequency": "once",
          
          // Conditions is formatted identically to a Content Patcher 'When' block
          "Conditions": {
    	    "Season": "Spring"
    	  }
        },
        {
          "Name": "Five Finger Discount",
          "Action": "Subtract",
          "Value": 2500,
          
          // Multiple targets can be listed in one patch
          "Target": "buscost, minecartscost, bridgecost, greenhousecost, panningcost",
          
          "Frequency": "yearly",
          "Conditions": {
          
            // Any Content Patcher token is fair game, as well as any mod-added tokens,
            // provided you add the mod as a dependency in your manifest.json.
            // See Content Patcher's README for more details.
            "Relationship:Shane": "Dating"
          }
        },
        {
          // This is a valid patch!
          // The Name field will be generated during parsing,
          // The Frequency field defaults to Once if omitted,
          // and Conditions can be omitted as well
          "Action": "Multiply",
          "Value": 0.80,
          "Target": "movieTheaterCost",
        }
      ]
    }

Target values are case-insensitive and may be fully qualified (Joja.busCost) or not (bundle3). The following are all valid Target values:
`JOJA.minecartscost`, `vault.bundle4`, `GREENHOUSECOST`, `jOjA.gReEnHoUsEcOsT`.

Possible config values:

|  | Option | Type | Can be set in Patch |
|---|---|---|---|
| **Joja** | *applyValues* | Boolean | No |
|  | *busCost* | Int | Yes |
|  | *minecartsCost* | Int | Yes |
|  | *bridgeCost* | Int | Yes |
|  | *greenhouseCost* | Int | Yes |
|  | *panningCost* | Int | Yes |
|  | *movieTheaterCost* | Int | Yes |
| **Vault** | *applyValues* | Boolean | No |
|  | *bundle1* | Int | Yes |
|  | *bundle2* | Int | Yes |
|  | *bundle3* | Int | Yes |
|  | *bundle4* | Int | Yes |

Detailed description of config values available on this mod's NexusMods page.

### Content Patcher Integration
As detailed above, the `Conditions` field of a Patch will support any CP-provided tokens, as well as any tokens which are provided by a mod listed as a dependency in your `manifest.json`. Additionally, this mod exposes all config values as tokens for use with other CP packs, provided they list this mod as a dependency. This means you can, for example, send a letter to inform the player of a discount on Joja bundles. You can reference a token as such: `Vertigon.ConfigurableBundleCosts/busCost`. It's probably also a good idea to make sure `applyValues` is true, by checking the appropriate token as well - `Vertigon.ConfigurableBundleCosts/JojaApplyValues` for Joja bundles and `Vertigon.ConfigurableBundleCosts/VaultApplyValues` for Vault bundles.

### Console Commands
Note that these are not recommended for casual players, but may provide some assistance to developers looking to debug their content packs and/or pinpoint unintended behaviors. Enter into the SMAPI console window to execute.

* `cbc_dump`: Display the current config values
* `cbc_reload_config`: Forces the current config values in a saved game to be reloaded. Will overwrite existing config changes such as already-triggered patches.
* `cbc_reload_packs`: Reloads the internal list of content packs
* `cbc_reload_patches`: Reloads the internal list of patches
* `cbc_list_packs`: Lists the currently loaded content packs
* `cbc_list_patches`: Lists the currently loaded patches

### Upcoming Features
 * Use mathematical expressions and queries in the `Target` field
 * More in-depth debug options (akin to Content Patcher)
