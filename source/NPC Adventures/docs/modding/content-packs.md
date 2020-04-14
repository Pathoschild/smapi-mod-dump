# Content packs

**NOTE:** This is an experimental feature. Can be changed or removed in future.  

NPC Adventures supports content packs. We can create SMAPI [content pack](https://stardewvalleywiki.com/Modding:Content_packs), target NPC Adventures mod in [manifest](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest) file.

NPC adventure content pack is a folder with these files:

- `manifest.json` for SMAPI to read (See [content pack manifest](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest) on SDV wiki
- `content.json` this file defines which content we add or edit
- Add files with your contents

## Define manifest

Target mod id `purrplingcat.npcadventure`. Optional we can define a minimum version of mod was required for our content pack.

**<your_cp_folder>/manifest.json**
```json
{
  "Name": "Your Project Name",
  "Author": "your name",
  "Version": "1.0.0",
  "Description": "One or two sentences about the mod.",
  "UniqueID": "<your name.<your project name>",
  "MinimumApiVersion": "3.1.0",
  "UpdateKeys": [],
  "ContentPackFor": {
    "UniqueID": "purrplingcat.npcadventure",
    "MinimumVersion": "0.11.0"
  }
}
```

## Define contents

In your content pack folder create file `content.json` and we can define custom contents for mod. A little example for define contents for custom NPC (custom NPC must be exists in game. This content packs not define game's NPC, we must add that NPC in other SMAPI mod or content pack, like via [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915), [CustomNPC](https://www.nexusmods.com/stardewvalley/mods/1607) or other mod that can add new contents into game and define NPCs):

**<your_cp_folder>/content.json**
```js
{
  "Format": "1.2",
  "Changes": [
    {
      "Action": "Edit",
      "Target": "Data/CompanionDispositions",
      "FromFile": "assets/data/companionDispositions.json"
    },
    {
      "Action": "Load",
      "Target": "Dialogue/Ashley",
      "FromFile": "assets/dialogues/ashley.json",
      "LogName": "Ashley's dialogue" // optional. Can be used for action edit too
    }
  ]
}
```

### Content definition file fields

| Field         | Means                                                                                                 |
| ------------- | ----------------------------------------------------------------------------------------------------- |
| `Format`      | The format version. You should always use the latest version (currently 1.2) to use the latest features and avoid obsolete behavior. Old formats could not be supported in current mod version. |
| `Changes`     | The changes you want to make. Each entry is called a patch, and describes a specific action to perform: Edit json file or load new  |

Under key `Changes` we must define content definitions. It's a list of dicts with these keys:

| Field                | Means                                                                                                 |
| -------------------- | ----------------------------------------------------------------------------------------------------- |
| `Action`             | The kind of change to make: `Load` for replace content or load new; `Edit` for patch existing content |
| `Target`             | The NPC Adventures mod asset you want to patch. This is the file path inside mod's assets folder, without the file extension or language (like `Dialogue/Abigail` to edit `assets/Dialogue/Abigail.json`)                 |
| `FromFile`           | The relative path to the content file in your content pack folder to patch into the target (like assets/dialogue/abigail.json). Supports only `.json` files.                                                             |
| `LogName` (optional) | This string replaces a default entry #no description in log with custom description                   |
| `Locale` (optional)  | **Can be used in `Edit` action only!** This key defines a lang code (like `pt-BR` and etc), for which this patch can be applied. Use this in pair with the existing content in mod or with existing patch (can be used in pair with action `Load` patch too). Locale patches are applied only for active game localization for which are defined. |

### Localized content pack patches example

```js
{
  "Format": "1.2",
  "Changes": [
    {
      "Action": "Edit",
      "Target": "Data/CompanionDispositions",
      "FromFile": "assets/data/companionDispositions.json"
    },
    {
      "Action": "Load",
      "Target": "Dialogue/Ashley",
      "FromFile": "assets/dialogues/ashley.json",
      "LogName": "Ashley's dialogue",
    },
    {
      "Action": "Edit",
      "Target": "Dialogue/Ashley",
      "FromFile": "assets/dialogues/ashley.pt-BR.json"
      "Locale": "pt-BR",
      "LogName": "Portuguese translation for Ashley's dialogue"
    },
    {
      "Action": "Edit",
      "Target": "Dialogue/Abigail",
      "FromFile": "assets/dialogues/abigail.pt-BR.json",
      "Locale": "fr-FR",
      "LogName": "Abigail's french dialogue patch"
    }
  ]
}
```

## Release a content pack
See [content packs](https://stardewvalleywiki.com/Modding:Content_packs) on the wiki for general info. Suggestions:

- Prefix your content pack folder with `[NA]`, like `[NA] Ashley companion`
- Add specific install steps in your mod description to help players:

```
[size=5]Install[/size]
[list=1]
[*][url=https://smapi.io]Install the latest version of SMAPI[/url].
[*][url=https://www.nexusmods.com/stardewvalley/mods/4582]Install NPC Aventures[/url].
[*]Download this mod and unzip it into [font=Courier New]Stardew Valley/Mods[/font].
[*]Run the game using SMAPI.
[/list]
```

When editing the Nexus page, add It's time to adventure (NPC adventures) under 'Requirements'. Besides reminding players to install it first, it will also add your content pack to the list on the NPC adventures page.

## Future

Remember, this feature is **experimental** and it's declared for testing purposes. In future this system of content packs can be changed, replaced or removed from mod. I promise I'll enhance and maintain this system if community creates and maintain their's content packs for this mod and gives me some feedbacks about this content pack support system.

## See also

- [Companion dispositions](dispositions.md)
