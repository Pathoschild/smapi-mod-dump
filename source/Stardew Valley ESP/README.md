# stardewvalley-esp
Onscreen indicators for various entities and objects in Stardew Valley.

### Example pictures
foragables | npcs
:-----------:|:----------------------:
![example1-npcs](https://i.imgur.com/U9TZGnw.png)|![example2-foraging](https://i.imgur.com/SvqttR7.png)

### Planned features
- [x] Filter objects
- [x] Pick your own colors
- [ ] Entity checklist
  - [x] NPCs
  - [x] Other players (untested, should work with NPCs)
  - [x] Farm Animals
  - [x] Foragables
  - [x] Stone, twigs, weed
  - [ ] Dropped stuff
  - [ ] Fishing hotspots
- [ ] General QOL
  - [x] Ingame menu to change settings

### Settings
When you first start the mod, a file called `settings.json` will be placed in the same folder as the mod location e.g. "Stardew Valley/Mods/sdv-helper". As entities are encountered in the game, the file will be populated, however you can populate it manually
if you want. Each entry is formatted like so:
```js
{
  "Name": [enabled, r, g, b, a],
  // ...
}
```
- Name is the name of the object e.g. Weeds, Stone, and for NPCs and animals, their actual name.
- enabled is a number that determines whether or not something is enabled. 0 or less means it is disabled and anything greater than that means it is enabled.
- The final three values are the color you want the background of the label to be: red, green, blue, and alpha. Everything automatically gets halved for a cool transparent effect (which I might change in the future).

You can change the file while ingame and reload it by pressing the `L` key, you should see changes take place pretty quickly. You can save settings with the `K` key, although there's no point right now because you can't change settings in game and it would be saved anyways if you could.

### Is this cheating?
I don't know, but who cares it's Stardew Valley.
