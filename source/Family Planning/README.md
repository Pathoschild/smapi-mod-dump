# FamilyPlanningMod
This is a Stardew Valley mod called Family Planning. Family Planning allows you to customize the maximum number of children you can have and their genders. Currently, you can have zero to four children.

## How to use:

After you've loaded the singleplayer save file you want the mod to change, go to the SMAPI console and use the command "set_number_of_children \<value\>". This will change the maximum number of children you can have to the value you entered, from zero to four. There's also a command "get_number_of_children" to check what the value is currently set to.

For example, if you wanted to have the max number of children possible, four, you would load your save file and type "set_number_of_children 4" into the console. Your spouse will then be able to prompt you about having children until you reach four.

Also, you will now be able to choose your child's gender when you name them. You could have one son, or three daughters, or two sons and two daughters, or no children at all, etc, the choice is yours.

## Child Appearance Mods:

If you're using other mods to change the appearance of your children, this mod will be affected. Any mod which directly replaces the xnb file will lead to children of the same gender being identical. Therefore, this mod is now compatible with Content Packs. If you'd like to make your own Content Pack for this mod, the process is relatively simple! The explanation is below.

### Creating a Content Pack
To create a Content Pack, first download the Example Content Pack from the Nexus page for Family Planning. There are three steps to finishing the Content Pack: you will need to edit the manifest.json, add your image files to the assets folder, and edit the data.json in the assets folder.

### Manifest.json
Inside the manifest.json, you should replace the text \<Your Name Here\> in the Author and UniqueID fields with your name.
  
For example, my name is Loe2run, so I would replace "\<Your Name Here\>" with "Loe2run". SMAPI will not run your Content Pack if "\<Your Name Here\>" is in the manifest.json file (because the \< character isn't allowed).
  
### Image files
Decide on which image files you'd like to use for your children and place those files in the assets folder.

For example, let's say that your Farmer is married to Leah and you'd like to use the sprite from Lakoria's BabiesTakeAfterSpouse. You have two daughters, Amber and Beverly, who need new sprites.
You look through the sprites available and decide that:
for Amber, you want "leahbaby.png" as a baby and "leahhairbuns.png" as a toddler,
for Beverly, you want "leahbaby.png" as a baby and "leahpigtails.png" as a toddler.

Copy the image files you want, "leahbaby.png", "leahhairbuns.png", and "leahpigtails.png" to the assets folder. (You can rename these files if it's convenient to you, but it's not necessary. They can have whatever names you want.)

### Data.json
Now that you've added the image files, you'll see a data.json file in the assets folder with your image files.
The default version of this file reads as:
```cs
{
  "ChildSpriteID": {
    "<Child Name>": {
      "Item1": "<name of baby sprite>",
      "Item2": "<name of toddler sprite>"
    }
  }
}
```
This default version only has a single entry, but you can copy and paste this as many times as you need for all of your children. Just be sure to put commas in between the sections. In our example with two children, the new data.json would read as:
```cs
{
  "ChildSpriteID": {
    "Amber": {
      "Item1": "leahbaby.png",
      "Item2": "leahhairbuns.png"
    },
    "Beverly": {
      "Item1": "leahbaby.png",
      "Item2": "leahpigtails.png"
    }
  }
}
```
And that's it! You should now be able to run the game with the custom sprites.

### Additional Notes
This method currently only works with .png files. I'm hoping to expand to allow .xnb files soon, but until then, your best bet is to try and convert the .xnb file you like to a .png image. More information on that at the [Stardew Valley Wiki.](https://stardewvalleywiki.com/Modding:Editing_XNB_files#Unpack_game_files)

If you don't know the name of your child because they haven't been born yet, that's fine! Just wait until the child is born, name them, wait for the game to save, and then exit to desktop and follow the steps above. The next time your open the game, your newly named child should have a custom sprite.

If you have two children with the same name between save files, the current method means they will end up with the same sprite. I'm hoping to fix this issue in the future.

If you're using this mod for multiple save files and want to have multiple save files worth of children to change, you have two options. You can either put all the child information in the same Content Pack or you can create a Content Pack for each save file. Both methods should work perfectly fine, though I would give the advice that it's easier to sort through multiple small Content Packs than one big one.

Using this method, the gender or skin color of the child has no effect on their sprite appearance. You could decide to use a male toddler sprite for a female child and the Content Pack will go ahead and apply the sprite regardless.

## Compatibility:

Works with Stardew Valley 1.3 beta on Linux/Mac/Windows.
Only works in single player for the current version.
This mod uses Harmony, so there may be interference with other mods using Harmony to patch the same methods. If you notice any issues, please let me know.

## More details:

The reason why the mod currently limits your to four children maximum is because it doesn't edit the number of beds in your house. Therefore, all of your children need to share the two existing beds. Two children can fit in a bed together, so four children is the limit (unless I update this mod to add more beds). Also, children will attempt to share a bed with a sibling of the same gender when possible.

For the 1.0.0 version, your spouse won't get dialogue acknowledging the additional children (besides the generic adoption dialogue for gay couples). This is something I'm planning to update.
