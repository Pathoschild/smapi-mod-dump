**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/SpousesIsland**

----

### Creating a Content Pack
The method to create content packs is the same as for any framework: you just need a manifest and a content file.
[A template of the content.json file can be found here](https://github.com/misty-spring/SpousesIsland/blob/main/content_template.json).

## Contents

* [Explanation](#explanation)

  * [Data model](#data-model)

  * [Dialogue](#dialogue)

  * [Schedule](#schedule)

* [Translating](#translating-your-mod)

## Explanation
Spouses' Island patches the game files in two different ways:
- Dialogues are always added, regardless of day.
- Schedules are only edited if it's an "island day".

### Data model
Spouses' Island's content pack uses a data model with the following information:

name | description
-----|------------
Name | The name of the spouse you're adding the schedule for.
ArrivalPosition | The position where the spouse will stand at when they reach the island. Uses three numbers (x, y, facing position). For more information, read the wiki.
ArrivalDialogue | If you interact with the spouse once they arrive to the island house, they'll say this dialogue.
Location Name | The name of the map the spouse will go to. **Allowed values: Any game map.**
Location Time | The time at which the spouse will begin moving. **Allowed values: Between 1030-2140**.
Location Position | The position the spouse stands at once they reach the location.
Location Dialogue | The dialogue the spouse will say in that location.

There are three `Location<number>` lists: from those, **only the third one is optional.**

You can add multiple characters in the same contentpack. For an example, [see here](https://github.com/misty-spring/SpousesIsland/blob/main/docs/example-contentpack.json).

### Dialogue
The dialogue field follows the same convention as game dialogue. For more information, see [Modding Dialogue](https://stardewvalleywiki.com/Modding:Dialogue#Format) in the stardew valley wiki.
Dialogue is added when the file is requested (e.g if the game wants to load `"Characters/Dialogue/Krobus"`, it will be edited then passed to the game).

An example of a dialogue string would be:
```json
"ArrivalDialogue" : "Do you think we can explore this volcano?$0#$b#Willy said we shouldn't get close..$2#$b#But I still brought my sword.$1",
```
You don't need to add any dialogue name/key; the mod will handle that internally.

### Schedule
The schedule follows the same convention as the game's schedules.
To handle that for you, it uses the following information: 
- Name (of the map your spouse will go to)
- Time (which your spouse will start moving at)
- Position (where they'll stand once they arrive)
For more information on how schedules work, [see here](https://stardewvalleywiki.com/Modding:Schedule_data#Schedule_points).

## Translating your mod
To translate your mod, simply add an entry to the "Translations" section:
```json
"Translations": [
    {
        "Key":"",
        "Arrival":"",
        "Location1":"",
        "Location2":"",
        "Location3":""
    }
]
```
Every "{}" counts as a different translation. So you can add multiple at once for a single content pack. You only need to add the translated dialogue. (Also supports modded languages)

For example:
```json
"Translations": [
    {
        "Key":"es",
        "Arrival":"Hola, @. El clima aquí es muy cálido.#$b#¿Harás algo más tarde?",
        "Location1":"¿Crees que podríamos quedarnos hasta mañana, @? $1",
        "Location2":"%Spouse está observando el atardecer.",
        "Location3":"Se está haciendo tarde...$0"
    }
]
```
The mod will recognize this as the translation for spanish, and add it to the respective language.
These dialogues also follow the same convention as the game's dialogue.
For more information on language codes, see [here](https://github.com/misty-spring/SpousesIsland/blob/main/languagecodes.md).
