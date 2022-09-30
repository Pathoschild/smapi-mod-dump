**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/KediDili/Creaturebook**

----

# Creating content packs for Creaturebook
Creaturebook is a framework that allows other modders to add their animals or plants to the mod's own book. 

**Warning! After this point, this documentation will assume that you have experience with creating content packs. If not, please see this wiki page**
**https://stardewvalleywiki.com/Modding:Content_packs#For_modders**

## File structure
All content packs require a `chapter.json` to have along with the `manifest.json`. Near these, it is also required a subfolder that's named after the `chapter.json`'s `ChapterFolder` string field. Under that subfolder, there should be finally subfolders that contain a `creature.json` and a `book-image.png` for each creature.
So it would look like this:
```
ModName > ChapterTitle > AnySubfolderName > creature.json
          manifest.json                     book-image.png
          chapter.json
```

If you would like this approach, let's make an example out of Project Starlight below:
```
[CB] Project Starlight > Moths > Anomalous Bluetail > creature.json
                         manifest.json                book-image.png
                         chapter.json
```

### Formatting for chapter.json
Each `chapter.json` is possible to have these fields below:

Field Name | Value Type | Is it Required? | Default Value    |  Notes
---------- | ---------- | --------------- | ---------------- | ---------------
`CreatureAmount` | `int` | Yes | - | This has to match with how many creature subfolders you have in your chapter. If it's not, Creaturebook will show a warning and your chapter will be ignored.
`ChapterTitle` | `string` | Yes | - | This will be displayed on your chapter's header page in the book's own menu.
`ChapterFolder` | `string` | Yes | - | Your chapter subfolder near the `chapter.json` and `manifest.json` need to be named after this field and it has to be unique among all of your chapters in a pack.
`CreatureNamePrefix` | `string` | Yes | - | This has to be unique among all of your chapters in your pack, since the mod does its data storing using this, `ID`(see creature.json formatting below) and the packs's unique ID.
`Author` | `string` | No | `"Example Author name for Header Page"` | This will be displayed on your chapter's header page in the mod's book menu.
`EnableSets` | `bool` | No | `false` | This chooses whether if your chapter should be opted in to the mod's Sets feature.

### Formatting for creature.json
Each `creature.json` is possible to have these fields below:

Field Name | Value Type | Is it Required? | Default Value    |  Notes
---------- | ---------- | --------------- | ---------------- | ---------------
`ID` | `int` | Absolutely. | - | This is where your creature is in the chapter (This means the smaller ID a creature has, the earlier its page will be). It has to be unique for each creature that are in the same chapter. It always should begin with the first one having `0` as its ID, next one `1`, then `2`, and so goes on.
`HasExtraImages` | `bool` | No | `false` | This sets if your creature owns one or two extra images to display on the Creaturebook menu. If enabled, it'll require the said creature to have a ``book-image_2.png`` under its subfolder. You can also have a ``book-image_3.png``, but it's optional even with this option enabled.
`HasFunFact` | `bool` | No | `true` | This sets if your creature has a fact to be shared when its discovered on Creaturebook menu's right side. If enabled, would require you to add an ``i18n`` folder (See translations), inside the translation files, your fact should be stored like `"<CreatureNameprefix>_<CreatureID>_name" : "My creature fact"`
`HasScientificName` | `bool` | No | `true` | This sets if your creature should have a scientific name. Enabling this will require `ScientificName` to be filled.
`ScientificName` | `string` | Only if `HasScientificName` is enabled. | - | This is your creature's Latin scientific name. It will be displayed on the mod menu once your creature is discovered.
`OffsetX` | `int` | No | 0 | This offsets your creature's `book-image.png`'s placement on X axis.
`OffsetY` | `int` | No | 0 | Does the same thing with `OffSetX`, but for Y axis.
`Scale_1` | `float` | No | 1f | This changes the scale of your creature's `book-image.png`. The bigger the value is, the bigger it'll look!
`OffsetX_2` | `int` | No | 0 | Does the same thing with `OffSetX`, but for `book-image_2.png`
`OffsetY_2` | `int` | No | 0 | Does the same thing with `OffSetY`, but for `book-image_2.png`
`Scale_2` | `float` | No | 1f | Does the same thing with `Scale_1`, but for `book-image_2.png`
`OffsetX_3` | `int` | No | 0 | Does the same thing with `OffSetX`, but for `book-image_3.png`
`OffsetY_3` | `int` | No | 0 | Does the same thing with `OffSetY`, but for `book-image_3.png`
`Scale_3` | `float` | No | 1f | Does the same thing with `Scale_1`, but for `book-image_3.png`
`OverrideDefaultNaming` | `string` | No | - | This is used if you want your creature to be found by clicking an NPC-in-game-code that has a different internal name than how does Creaturebook name creatures' pages internally.
`BelongsToSet` | `string` | No, and will be used only if your chapter's ``EnableSets`` is enabled. | `"Other"` | This is the name of the said creature's set. Any creatures with the same ``BelongsToSet`` will be assigned to same set. Creatures that are in same set aren't required to have sequencing pages, but rather would be preferred to have for better organization.

## Nice and all but I don't see anything about creatures' translatable display names or descriptions?

Yes you don't. You'll find your answer here!: https://github.com/KediDili/Creaturebook/blob/main/translation.md

## For any questions about something else than the chapter.json or creature.json
### You mentioned a second way to discover creatures and stuff about spesific tiles in user guide, where is it's documentation?
- Here. You can have TileDatas to instantly discover a certain creature. You can do a patch like this to add one, but **note that this feature is untested.**:
```
{ 
  "Changes": [
      {
          "LogName": "Tiledata for instant discover",
          "Action": "EditMap",
	  "MapTiles":
	  [
	     {
	         "Position": {
		     "X": "5",
		     "Y": "10"
		 },
		 "Layer": "Back", //This has to be the Back layer!
		 "SetProperties":{
		     "KediDili.Creaturebook": "Discover <ContentPackID>.<CreatureNamePrefix>_<ID>" // Like KediDili.ProjectStarlight.CB.Flutter_32
		 }
	     }
	  ]
      }
]
```

### Are there any image size restrictions with any of `book-image.png`, `book-image_2.png` or `book-image_3.png` files?
- No restrictions put by Creaturebook itself! However, it should be smaller than 4096x4096, because that this is a game limitation.

### Can I access any of the data Creaturebook keeps track of?
- Sure, you can use it's Content Patcher intermod tokens! Here's what each one of them does:

Token Name | What input does it want? | Returns This Value Type | Nice but what's value is this?
----------- | ------------------- | ----------------------- | -----------------------------
`AllCreatures` | None | `int` | How many creatures has been successfully registered to Creaturebook.
`AllDiscoveredCreatures` | None | `int` | How many creatures has been discovered in the current save.
`DiscoveredCreaturesFromAChapter` | A chapter's `CreatureNamePrefix` and it's mod's UniqueID (Like `<exampleAuthor.ExampleModname>.<CreatureNamePrefix>`) | `int` | How many creatures has been discovered from a spesific chapter.
`IsCreatureDiscovered` | A chapter's `CreatureNamePrefix` and a creature's `ID` (Like `<CreatureNamePrefix>.<ID>`) | `bool` | If this certain creature is discovered.

**Don't forget to prefix this mod's UniqueID and a slash (`KediDili.Creaturebook/`) so that CP knows you want this mod's tokens.**

### I want to change X content pack's Y chapter's Z creature's one of `book-image.png` files, is this possible?
- Absolutely! All you should do is do a CP patch like:
```
{
	"LogName": "Changing Z creature's image",
	"Action": "Load", //Can do EditImage too then completely replace the image
	"FromFile": "assets/creature.png",
          "Target": "KediDili.Creaturebook/<X content pack's UniqueID>.<Y chapter's `CreatureNamePrefix`>_<Z creature's ID>_Image1", //Do Image_2 or Image_3 if you want to change one of book-image_2.png or book-image_3.png files
}
```
#### But my image is smaller/bigger than the original. Is this any problem?
- If this was an image from another mod/one of the game's own stuff you're patching, it obviously would be. I don't know if it'll be actually a trouble for Creaturebook, but in theory I don't think so since it doesn't care about dimensions. Though if it's too far from the original, original `Scale` and `Offset` values can make your image messed up.

### I want to retexture Creaturebook's own assets. Can I do that?
- Of course! All you should do is do any of these CP patches like:
```
{ 
  "Changes": [
      {
         "LogName": "Changing Creaturebook's notebook menu background",
         "Action": "Load", //Can do EditImage too then completely replace the imag
         "FromFile": "assets/notebook.png",
         "Target": "KediDili.Creaturebook/NotebookTexture" //Patches notebook background
      },
      {
         "LogName": "Changing Creaturebook's item",
         "Action": "Load", //Can do EditImage too then completely replace the imag
         "FromFile": "assets/item.png",
         "Target": "KediDili.Creaturebook/NoteItem" //Patches Creaturebook's item image
      },
      {
         "LogName": "Changing Creaturebook's searching button",
         "Action": "Load", //Can do EditImage too then completely replace the imag
         "FromFile": "assets/button.png",
         "Target": "KediDili.Creaturebook/SearchButton" //Patches Creaturebook's searching button on menu
      },
   ]
}
```
