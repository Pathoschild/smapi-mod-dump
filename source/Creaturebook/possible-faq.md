**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KediDili/Creaturebook**

----

# Welcome to possible mod faq.
This is a user guide for Creaturebook. If you're looking to make content packs or do translations, please go back to this page then find your way:
https://github.com/KediDili/Creaturebook#readme

Let's get started, users.
### Is it compatible with SVE/RSV/Any other known mods? 
- Dont you dare not using your logic for such an annoying question.

### Any known conflicted mods?
- Any mod that adds an item with a hardcoded ID of `31`. It shouldn't cause errors, but either this mod will overwrite it, or the other mod will.

### Any known bugs?
- There's a bug with attempting to discover with giftable NPCs, they might take the Creaturebook as a gift and they will hate it, unless the NPC has a rejecting dialogue for it. I *think* the discovering logic should still work, though this is untested.

### What is Creaturebook?
- Creaturebook is a framework mod that allows content packs to add their creatures' pages for information, and allows said creatures to be discovered by the player. So yeah, it's sort of a bestiary mod. Since the mod is a framework, don't expect it to do anything without content packs.

### How to install it?
- Just like any other mod. Drop it to anywhere in Mods folder. 

### What are its dependencies? 
- The only necessary requirement is SMAPI. Content Patcher and Generic Mod Config Menu  are optional dependencies.

### Does this mod use Harmony?
- No. And it never will unless needed.

### How do I see information about creatures?
- First, you need to have discovered a creature. If you already have, use the mod's menu opening keybind while no other menu is open. You can find it in your config.json, or install Generic Mod Config Menu to easily change it. The default keybind is `LeftControl + LeftShift + B`.

### How do I discover a creature? 
- There are two ways. One is via clicking a tile (with the Creaturebook item in hand) that 's set to  get you to discover a creature instantly (Note that this feature is completely untested). The second and primary way is clicking to anything that has got a page in Creaturebook and that's coded into the game as an NPC (So yes, using Custom Companions will work, fellow authors.) with Creaturebook item in hand.

### How do I obtain Creaturebook item?
- That depends on what your `WayToGetNotebookItem` config is set to. It's valid values are `Letter`, `Inventory` and `Events`. The value names should be self explaining. Use Generic Mod Config Menu for easier configuration.

### There's this button on the mod's menu, what is it?
- It's a button to open the menu's search box. Type one of a creature's ID, translated name or scientific name, press enter and the correct page shall show up.

### Is this mod multiplayer compatible?
- There's some kind of support implemented into it, but it's untested.

### Is this mod compatible with Linux/Mac/Android?
- I believe it is compatible with Linux and Mac, but this is untested. As for Android, unfortunately no, and won't be unless someone else volunteers to port it.

### Is this mod split-screen compatible?
- I don't know, but I don't think so.
