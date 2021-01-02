**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/PurrplingMod**

----

# Events

NPC Adventures events are loaded from data asset `Data/Events` (file `assets/Data/Events.json` in NA mod folder). Event content structure is ordinary string-string dictionary.

## Adventuring story events

Currently there are only one adventuring story event. It's Marlon's invitation event stored under key `adventureBegins`. **Please don't change or remove this event in your content packs**

## Companion cutscenes

Companion cutscenes are events which are played when you are entered to a location with recruited companion (during adventuring with currently recruited companion). The key of these kind of events ere more complex, the template is: `companion/<int:eventId>/<string:npcName>/<string:location>[/<epu_conditions> ...]`

The key always starts with `companion/` and follow these parameters:

| # | param name | type | description |
| --- | ---------- | ---- | -------------------------------- |
| 1 | eventId | int | SDV event id. Avoid conflict with another event ids. It's recommended prefix id with yout nexusmods mod id |
| 2 | npcName | string | An NPC name of recruited companion. Player must have recruited this companion to play this event |
| 3 | location | string | A location name for this event. Player must enter this location with recruited companion (defined in previous parameter) to play this event |
| 4 | epu_conditions | array of string splited by `/` | (optional) [Extended precondition utility](https://github.com/ChroniclerCherry/stardew-valley-mods/blob/master/ExpandedPreconditionsUtility/README.md) condition like `f Linus 1000/w rainy/z spring`

Internal event id standard (in NA development team) is:

<details>
  <summary>Companion event id standard</summary>
```
     Nexus mod id (4582 is NA's nexus id. For content packs use yours)
    /
   /    Number of companion in companion disposition file (entry index)
  /    /
4582 1n xx
     \   \ 
      \   Number of event for this companion
       \ 
        Event type: companion event (1)
```
</details>

### List of companion events

See [Events](../guide/events.md) in the mod guide.

### Example

An example of content pack which adds/edits an companion cutscene.

```js
// File assets/data/my-events.json
{            
  "companion/45821700/Emily/SandyHouse": "none/6 18/farmer 4 9 0 Emily 4 8 0 Sandy 2 5 2 Bouncer 17 3 2/pause 1000/skippable/pause 200" // prefix 4582 is NPC Adventures nexusmods mod id. Dont' use this prefix, use another one in your content pack
  "companion/45821701/Emily/SandyHouse/t 900 1000/f Emily 1500": "none/6 18/farmer 4 9 0 Emily 4 8 0 Sandy 2 5 2 Bouncer 17 3 2/pause 1000/skippable/pause 200"
}
```

```js
{
  "Format": "1.3",
  "Changes": [
    {
      "Target": "Data/Events",
      "FromFile": "assets/data/my-events.json"
    }
  ]
}
```

If you are NPC Adventures developer, you can edit file `assets/Data/Events.json` in NPC Adventures mod folder directly.
