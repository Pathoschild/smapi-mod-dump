**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/LeFauxMatt/StardewMods**

----

# Portable Holes

Adds craftable holes to the game.

## Contents

* [Configurations](#configurations)
* [Customization](#customization)
* [Translations](#translations)
* [Credits](#credits)

## Configurations

For ease of use, it is recommended to set config options
from [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).

| Config              | Description                                        | Default Value | Other Value(s)    |
|:--------------------|:---------------------------------------------------|---------------|:------------------|
| SoftFall            | Stop taking fall damage when you drop down a hole. | `false`       | `true` or `false` |
| UnlockAutomatically | Automatically unlock the Ordinary Capsule recipe.  | `false`       | `true` or `false` |

## Customization

Integration with Portable Holes is possible
using [Content Patcher](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md).

### Texture

Replace the Portable Holes texture.

**Target** `furyx639.PortableHoles/Texture`

```jsonc
{
  "Format": "1.25.0",
  "Changes": [
    {
      "Action": "EditImage",
      "Target": "furyx639.PortableHoles/Texture",
      "FromFile": "assets/MyTexture.png",
    },
  ]
}
```

## Translations

See [here](i18n/default.json) for the base translation file.

| Language   | Status            | Credits |
|:-----------|:------------------|:--------|
| Chinese    | ❌️ Not Translated |         |
| French     | ❌️ Not Translated |         |
| German     | ❌️ Not Translated |         |
| Hungarian  | ❌️ Not Translated |         |
| Italian    | ❌️ Not Translated |         |
| Japanese   | ❌️ Not Translated |         |
| Korean     | ❌️ Not Translated |         |
| Portuguese | ❌️ Not Translated |         |
| Russian    | ❌️ Not Translated |         |
| Spanish    | ❌️ Not Translated |         |
| Turkish    | ❌️ Not Translated |         |

## Credits

The texture was created by [Tai](https://www.nexusmods.com/stardewvalley/users/92060238).