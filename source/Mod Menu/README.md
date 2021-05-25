**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Enaium-StardewValleyMods/ModMenu**

----

# ModMenu

ModMenu

## Install

1. [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod](https://github.com/Enaium-StardewValleyMods/ModMenu/releases).
3. Install [EnaiumToolKit](https://github.com/Enaium-StardewValleyMods/EnaiumToolKit/releases).
4. Run the game using SMAPI.

## Custom

`manifest.json`

### Setting

namespace.className

```json
{
  "Custom": {
    "ModMenu": {
      "Setting": "ModMenu.Framework.Screen.SettingScreen"
    }
  }
}
```

```c#
namespace ModMenu.Framework.Screen
{
    public class SettingScreen : IClickableMenu
    {
        
    }
}
```

### Contact

```json
{
  "Custom": {
    "ModMenu": {
      "Contact": {
        "HomePage": "https://github.com/Enaium-StardewValleyMods/ModMenu",
        "Issues": "https://github.com/Enaium-StardewValleyMods/ModMenu/issues"
      }
    }
  }
}
```