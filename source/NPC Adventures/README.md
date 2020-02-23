[![following][library]](https://www.youtube.com/watch?v=gFX9OVSS3fg)

# NPC Adventures (It's time to adventure)

**THIS MOD IS IN BETA VERSION!** It's relatively stable, but may cause some bugs and issues. Found any defect? Report it and help improvement this mod. Any idea? Create a proposal on Github. Thanks.

Go to an adventure with Pelican Town's villagers! Recruit bachelorete or bachlor and go to an adventure togehter.

**Watch [trailer video](https://www.youtube.com/watch?v=gFX9OVSS3fg) for this mod.**
**Looking for user guide? See [Documentation](docs/index.md)**

## Get it!

Download this mod on **[Nexusmods](https://nexusmods.com/stardewvalley/mods/4582)**

### Install
- [Install the latest version of SMAPI](https://smapi.io).
- Download this mod and unzip it into *Stardew Valley/Mods*.
- Run the game using SMAPI.

## Features

- Ask NPC to a follow farmer (5 hearts and more required)
- Recruited NPC can fight with monsters (with swords and for NPC with personal skill fighter)
- Various dialogues for different locations (incomplete yet)
- Can save items from our repository to a npc's backpack
- If you want to break adventure, then you can release a companion
- Next morning you can find a package with your items you saved in companion's backpack
- Idle animations
- Speech bubbles while companion fighting
- Different personal skills: warrior, fighter and doctor (next comming soon)
- Doctor can heal you if your health is under 30% and can try to save your life before death
- Warrior can use critical defense fists
- Fighter can level up (syncing level with player) and can upgrade swords
- Display personal skills in HUD (with mouseover tooltip)
- Better critical defense fist fight animation and sound
- Support for content packs and localizations. You can add custom NPC as companion (see [how to create content pack](https://github.com/purrplingcat/PurrplingMod/wiki/Content-packs) on wiki)
- User configuration (via `config.json`, see [SMAPI docs](https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started#Configure_mods))
- Every companion NPC grants a buffs
- Two gameplay modes: Adventure (default) and classic (like alpha versions)
- Location dialogues can be assigned by time update in game
- Prostetic (changeable) buffs for Maru (hold `G` to change buff)
- Recruitable villagers can suggest you an adventure (requires 7 hearts and more. Can be changed in config)
- Companion introduction quests (visit Marlon, recruit 1 companion, recruit 5 companions and recruit 10 companions) - ADVENTURE MODE ONLY!
- Debug mode for developers and content pack creators (by default disabled, you can enable it in config)
- Added debug command `npcadventure_eligible` for eligible player to recruit, DEBUG AND ADVENTURE MODE ONLY!-
- New documentation for players and modders/content creators
- [NEW] Support for localizations in content packs
- [NEW] Android support (experimental) Thx @MaxPresi

Next features comming soon...

### Supported companion NPCs

| Companion | Personal skills | Buffs                        |
| --------- | --------------- | ---------------------------- |
| Abigail   | Warrior         | +1 Speed, +1 Luck, +1 Attack |
| Alex      | Warrior         | +1 Speed, +2 Attack          |
| Haley     |                 | +2 Luck                      |
| Maru      | Doctor          | +1 Mining, +1 Luck, +1 Speed |
| Shane     |                 | +3 Farming                   |
| Leah      |                 | +2 Foraging                  |
| Emily     |                 | +2 Mining                    |
| Penny     |                 | +3 Farming                   |
| Sam       |                 | +2 Speed                     |
| Sebastian | Warrior         | +1 Speed, +1 Luck, +1 Attack |
| Elliott   |                 | +3 Fishing                   |
| Harvey    | Doctor          | +3 Defense                   |

All listed NPCs can figth with sword! **Married spouse** grants additional buffs **+1 Luck** and **+1 Magnetic** radius

For some NPCs listed above we're missing companion dialogues. You can help us and you can create it. How? Fork this repo and see [Dialogue wiki](https://github.com/purrplingcat/PurrplingMod/wiki/Dialogues). You can get inspiration [from code](https://github.com/purrplingcat/PurrplingMod/tree/master/PurrplingMod/assets/Dialogue). Are you created dialogues? Just send us a PR.

### Adventure mode

By default this mod is played in adventure mode. This mode brings quests and events to interaction with your companions. For enable companion asking you must reach **10 level of mines**, have granted access to **Adventurer's guild** and received **letter from Marlon with invitation** and seen **Adventure begins** event with Marlon. Then you can ask villagers to companion. Marlon's letter you will receive morning when you reached **level 20, you have a 66% (3 hearts for default level 5 required hearts) of required heart level with any recruitable villager and you have an access to Adventurer's guild.** You already reached level and you have access and you installed/updated this mod? You will receive letter morning immediately (must to go out from FarmHouse to receive it tomorrow).

### Classic mode

Worried about adventure mode? Do you want old companion functionality like was in alpha versions? Don't be sad, just **disable adventure mode** in [configuration](docs/guide/configuration.md). By disabling adventure mode you disable also all quest lines and companion events!

### Custom NPCs

You can add your custom NPC as companion to this mod via content pack. See [documentation](https://github.com/purrplingcat/PurrplingMod/wiki/Content-packs)

## Compatibility

- Works with Stardew Valley 1.4 on Linux/Mac/Windows.
- Works on Android 6 and newer (experimental support)
- Works in **single player** ONLY.

### Note for Android users

Beginning with version *0.11.0* there's an experimental support for Android SMAPI and Stardew Valley. There is list of known android issues:

- **HUD may be drawn at an incorrect position on some devices.** - If you have this problem, you can disable the HUD in configuration file. (Set `ShowHUD` to `false`)
- **The game may crash on Android 5.1 and older** - Can't fix it, because it's caused by SMAPI. Android SMAPI has an experimental harmony patching, you can report these crashes to @MartyrPher (developer of SMAPI for Android). Remember Android 5.1 and older is not officialy supported by this mod.

### Compatibility with other mods

- ✅ **Custom Kissing Mod** by *Digus* - 100% compatible (from version 0.9.0 with version 1.2.0 and newer of Custom Kissing Mod.
- ⚠️ **Automatic gates** - NOT COMPATIBLE! Companion can stuck in gate when gate is automatic closed after farmer.
- ⚠️ **Json Assets** - CAN'T USE CUSTOM ITEMS FOR COMPANIONS. Can't use custom weapons in disposition file from JA in your content packs, because this mod not exported stacit item ids.

## Translations

- English (Corashirou, [RememberEmber](https://www.nexusmods.com/users/11961608), [PurrplingCat](https://www.nexusmods.com/users/68185132))
- Portuguese Brazilian ([andril11](https://www.nexusmods.com/users/68848663)) (55% covered)
- French ([Reikounet](https://www.nexusmods.com/users/70092158)) (49% covered)
- Chinese ([wu574932346](https://www.nexusmods.com/users/67777356)) (53% covered)

## Feature preview

**Asking NPC to a follow farmer**

![Ask to follow farmer][ask2follow]

**Companion follows you**

![following][library]

**Various dialogues for different locations**

![Various dialogues][dialogues]

**We can save items from our repository to a companion's backpack**

![Using companion's backpack][usebag]

**If we want to break adventure, then we can release a companion**

![Release companion][release]

**Next morning we can find a package with our items we saved in companion's backpack**

![Delivered items][delivery]

**Companion with `doctor` skill can heal you if you are injured**

![Companion heal a player][heal]

More features comming soon...

## Contributors

- [purrplingcat](https://www.nexusmods.com/users/68185132) - main developer and producer
- Corashirou - author of dialogues and texts
- [RememberEmber](https://www.nexusmods.com/users/11961608) - author of dialogues and texts
- [andril11](https://www.nexusmods.com/users/68848663) - Portuguese translation
- [Reikounet](https://www.nexusmods.com/users/70092158) - French translation
- [wu574932346](https://www.nexusmods.com/users/67777356) - Chinese translation

[library]: docs/images/library.gif
[ask2follow]: docs/images/asktofollow.gif
[usebag]: docs/images/usebag.gif
[dialogues]: /docs/images/dialogues.gif
[release]: docs/images/release.gif
[delivery]: docs/images/delivery.gif
[heal]: docs/images/harveyheal.gif
