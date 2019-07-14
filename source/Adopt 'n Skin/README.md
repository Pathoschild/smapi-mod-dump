# Adopt & Skin (Animal Skinner)

What A&S lets you do:

- Use a suite of your own custom sprites for any animal, pet, or horse in the game

- Uses a randomized skin by default when a new animal, pet, or horse appears

- Allows adopting multiple pets

- Allows adopting multiple horses

- Call a horse to you with a hotkey

- One-tile-wide horses, for galloping anywhere you damn well please

- Compatible with [BFAV](https://www.nexusmods.com/stardewvalley/mods/3296) (Both 2.2.6 and the 3.0.0 beta)

- Compatible with save files with multipets or multihorses from another mod



# To Use Adopt & Skin:

- Uninstall other mods that affect the skins of creatures. A&S will take over handling these.

- To use skins with A&S (including skins from those other mods you uninstalled), rename the spritesheet `.png` or `.xnb` files as described below. A few example skins are included with the download from [Nexus](https://www.nexusmods.com/stardewvalley/mods/4011/).

- Place these properly named skins in `AdoptSkin/assets/skins`

- To use the vanilla skin for an animal instead of a custom one, just remove any skins for that creature type from the `/skins` folder

That's it! You're all set.
(Customized options can be set in the `Config` file, as described below)


# Sprite names:

All farm animals, pets, and horses must be one of the following types (Note on BFAV animals below):

| **Animal Type** | **Baby subtype** | **Sheared subtype**|
|---|---|---|
| BlueChicken | *BabyBlueChicken* | - |
| BrownChicken | *BabyBrownChicken* | - |
| WhiteChicken | *BabyWhiteChicken* | - |
| VoidChicken | *BabyVoidChicken* | - |
| Duck | *BabyDuck* | - |
| Rabbit | *BabyRabbit* | - |
| Dinosaur | - | - |
| BrownCow | *BabyBrownCow* | - |
| WhiteCow | *BabyWhiteCow* | - |
| Goat | *BabyGoat* | - |
| Sheep | *BabySheep* | *ShearedSheep* |
| Pig | *BabyPig* | - |
| | | |
| | | |
| **Pet Type**| - | - |
| Cat | - | - |
| Dog | - | - |
| | | |
| | | |
| **Horse Type** | - | - |
| Horse | - | - |



Sprite sheets should be named in the manner listed below and placed in the `/asses/skins` folder. If you're familiar with the way [More Animals](https://www.nexusmods.com/stardewvalley/mods/2274) names these files, it is exactly the same.

* The file must be named in the format of `[Baby/Sheared][Type]_[Skin ID]`

**EX: `Cat_1`, `BabyVoidChicken_7`, `Dinosaur_3`, `Horse _27`, etc.**
* The `[Type]` needs to be exactly as shown in the above type chart (BFAV note below). Spaces and capitalization optional.
* All animals with a baby or sheared sprite MUST have these sprites in the folder, alongside the normal adult sprite.
* Each baby or sheared skin is matched to the normal adult version of the skin with that ID (i.e. `babysheep_1` will be the baby sprite of a sheep with the skin `sheep_1`)
* Skin IDs can be any number greater than 1, but must be unique from others of that type (i.e. you can have `dog_1` and `whitechicken_1`, but not two `whitechicken_1`s)
* **BFAV animal skins are to be named in the same way**, with their name looking the same as it appears in the BFAV `Config` file. **Double check the `Config`,** as animals may have a variety of "color variants" (like whitechicken and brownchicken have), which all need their own skin sets.
* All sprite files must be of type `.PNG` or `.XNB`
* All sprite files must be placed in `AdoptSkin/assets/skins`


# BFAV Compatibility

BFAV-added animals must have their skin files named in the same format as other farm animals/pets/horses, with the type from the chart above instead being the name as it is written in the BFAV `Config` file.

**ADDITIONAL BFAV NOTE:**
If you're using a BFAV animal with color variants and would like the functionality of additional custom sprites that A&S adds, note that you will have to do the whole skin set for *each* color variant.
I.E. Say BFAV adds a `RedSeagull` and a `BlueSeagull`. If you want them both to rotate through the same three skins, you will have to make your three skin files say `blueseagull_1`, `blueseagull_2`, and `blueseagull_3`, alongside any baby/sheared sprites, and then copy these skins and rename them to have an identical set that is `redseagull_1`, `redseagull_2`, `redseagull_3`, etc.

If you know what you're doing (a.k.a I am not user support for BFAV, and am also not responsible for broken code) then you can make a new animal class of just "seagull".

---

# FAQ

**Does the horse whistle work with the tractor mod?**
> Yep.

**Can I turn off/change X feature?**
> Check the `Config` file. There's a ton of things you can turn on and off or set to a different value. Explanations of the options in the `Config` are described below.

**Where's the `Config` file?**
> The `Config` file will generate after you've run Stardew Valley at least once after installing A&S.

**The SMAPI commands aren't working!**
> The commands went through an overhaul with the release of 2.4.0, and again with 2.6.0. If you're used to the old commands, or are confused on how to use them in general, refer to the explanations and example of the commands below.

**Why aren't the skins showing up?**
> First, make sure that you've placed skins in `AdoptSkin/assets/skins`, as A&S does not come with many skins of its own- only a few example skins.
> Second, check the file naming guide below.
> Third, if you're still having problems or are confused, feel free to post a comment or ask on the SDV discord's modding channel!

**Why aren't my BFAV animal skins working?**
> Make sure the skins are named **exactly** as they appear in the BFAV Config file. Some BFAV animals have "color variants", that work just like how brown chickens and white chickens do, in that they're each treated as a completely different animal. You'll have to name skins for each variant.

**Can I use A&S in multiplayer?**
> Conditionally. Multipet/multihorse isn't yet tested in multiplayer, but the farm animal skins work. Other things are a little unpredictable, as they are not yet supported. Multiplayer multipet/multihorse is currently being worked on.

**Can I reskin the petbed at Marnie's?**
> Sure can. Just make sure your image is named petbed.png and is the same size as the one in `AdoptSkin/assets`. Then just replace that image with yours!

**I don't see any stray pets at Marnie's!**
> Strays only start appearing the day after you've received your first pet from Marnie in-game, and don't appear every day. The chance of them appearing can be altered in the `Config` file.

**I can't find any wild horses!**
> Wild horses will only appear after you've built a stable, and are somewhat uncommon, having about a 25% chance to appear somewhere on the map each day, give or take some luck. This choice was made in order for wild horses to be an exciting find, rather than something you get bored of running across. That said, you can increase the chance of one appearing or set the chat and console to notify you when and where a wild horse appears via the `Config` file.

**I see a wild horse but can't reach it??**
> Tricky little devils, aren't they

---

# Config Options

* **OneTileHorse** : Squishes the horse you're riding to fit anywhere your can normally walk
* **HorseWhistleKey** : The hotkey for calling one of your horses to you. By default, this is set to R
* **CorralKey** : The hotkey for calling all horses back to the stable. By default, this is set to Y
* **StrayAdoptionPrice** : The price for adopting a stray pet at Marnie's, once the player receives their first pet
* **WalkThroughPets** : Whether or not pets can be walked through by the player. Useful for when you own a lot of pets
* **DisperseCuddlePuddle** : Whether or not to spread pets out from the water dish, rather than have them all spawn in a single tile
* **CuddleExplosionRadius** : How far out, in all directions from the water dish, pets can be spread when `DisperseCuddlePuddle = true`
* **WildHorseSpawn** : Whether or not wild horses can be found after the player obtains a stable
* **StraySpawn** : Whether or not stray pets can show up at Marnie's after the player receives their first pet
* **WildHorseChancePercentage** : The chance of a wild horse spawning each day after the player obtains a stable. By default, this is set to 25%
* **StrayChancePercentage** : The chance of a stray pet appearing each day at Marnie's, after the player obtains their first pet. By default, this is set to 60%
* **ChanceAffectedByLuck** : Whether or not the chance for a wild horse or stray pet to spawn each day is affected by daily luck (up to 10%)
* **NotifyHorseSpawn** : Whether the player will be alerted, via SMAPI console and chat notification, if a wild horse spawns that day, and where it is located. By default, this is set to false.
* **NotifyStraySpawn** : Whether the player will be alerted, via SMAPI console and chat notification, if a stray spawns that day at Marnie's. By default, this is set to false.
* **DebuggingMode** : A mode for debugging. This adds a suite of debugging commands to the SMAPI console for your use, including commands to spawn in a wild horse or stray.

---

# Console Commands

**`list_creatures [creature category]`**

Gives the Name, ID, and skin ID of every creature in the given category

*Accepted categories: all, animal, pet, horse, coop, barn, chicken, cow, or any specific type (i.e. whitechicken)*

**`randomize_skin [creature category or creature ID]`**

Randomizes the skin of the creatures with the given category or the creature with the given ID

*Accepted categories: all, animal, pet, horse, coop, barn, chicken, cow, or any specific type (i.e. whitechicken)*

**`set_skin [skin ID] [creature ID]`**

Select a specific skin for the given creature

**`sell [creature ID]`**

Gives away the specific pet or horse with the given creature ID

**`horse_whistle [optional: horse ID]`**

Calls a horse to your location. When used with a horse's ID, that specific horse is called. This also has a configurable hotkey, which is "R" by default.

**`corral_horses`**

Sends all the horses you own to your stable, giving you the honor of being a professional clown car chauffeur. This also has a configurable hotkey, which is "Y" by default.


## *Command Example:*

Let's set our chicken, who we'll call Chirpchirp, to have the skin `whitechicken_4`, which we've placed in the `/assets/skins` folder of A&S.

First, we'll need to figure out Chirpchirp's ID, which the the short number A&S uses to reference individual farm animals, pets, and horses.
To do that, we'll call `list_creatures chicken` in the SMAPI console. The output should show all the chickens you own. Chirpchirp's information should look something like this:

**`# Chirpchirp: Type - whitechicken`**

**`Short ID: 3`**

**`Skin ID:  9`**

From this we can tell that Chirpchirp's farm animal ID is 3, and it's currently set to have the `whitechicken` type skin 9, which would be the skin named `whitechicken_9` in the `/assets/skins` folder.

To set Chirpchirp's skin to `whitechicken_4`, we then need to call `set_skin 4 3`, which calls the `set_skin` function on the creature with the ID of 3 (which happens to be Chirpchirp) and gives it the fourth skin of that creature's type (Chirpchirp is a `whitechicken`, so that's the skin named `whitechicken_4`).

Chirpchirp will now appear with the skin `whitechicken_4`!
