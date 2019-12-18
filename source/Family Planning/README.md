# FamilyPlanningMod
This is a Stardew Valley mod called Family Planning. Family Planning allows you to customize the maximum number of children you can have and their genders. Currently, you can have zero to four children. Family Planning also allows you to adopt children with a roommate by config.

## How to use:

After you've loaded the singleplayer save file you want the mod to change, go to the SMAPI console and use the command "set_max_children \<value\>". This will change the maximum number of children you can have to the value you entered, from zero to four. There's also a command "get_max_children" to check what the value is currently set to.

For example, if you wanted to have the max number of children possible, four, you would load your save file and type "set_max_children 4" into the console. Your spouse will then be able to prompt you about having children until you reach four.

From the 1.2.0 update on, this information is now saved to a file in the data folder to allow multiplayer compatibility. This file shares the same name as your normal save file. (So for example, Farmer_123456789.) If you wanted, you could edit this file to the value you want. If you edit it to an illegal value, like -1, it will automatically change back when you next run the game.

You will be able to choose your child's gender when you name them. You could have one son, or three daughters, or two sons and two daughters, or no children at all, etc, the choice is yours.

To adopt children with a roommate, go to the config.json file in the Family Planning folder, and change the value "AdoptChildrenWithRoommate" to true. From there, all other settings will apply, and your roommate will be able to prompt you to adopt a child.

## Mod Customization
### Child Appearance Mods

By default, children of the same gender are identical. So if you'd like them to look different, you'll need to use another mod to change their appearance. There are two options: Content Packs for Family Planning, or using Content Patcher mods.

If you'd like to make a Content Pack for this mod, the process is relatively simple! The explanation is below the Content Patcher section.

If you'd like to make a Content Patcher mod, then you'll want to use the custom CP tokens that this mod uses. The explanation is below.

### Content Patcher tokens
To use the CP tokens that Family Planning provides, make sure to set Family Planning ("Loe2run.FamilyPlanning") as a dependency.

Family Planning will attempt to load the child sprite from the file "Characters\\\\Child_\<Child Name\>". In order for your CP mod to target that file, you'll need to use the ChildName token. There's a version of this token for each child (by birth order).
  
So for example, when you want to patch over the appearance of the oldest child, your Target field would look like this.

```cs
"Target": "Characters\\Child_{{Loe2run.FamilyPlanning/FirstChildName}}"
```
The second token that you can use is the IsToddler token, which returns "true" or "false". This helps you distinguish whether to use a toddler sprite or a baby sprite.

So to continue the example from above, this is what your full entry would look like when trying to load a new toddler sprite for the oldest child.
```cs
{
  "LogName": "First Child Sprites (Toddler)",
  "Action": "Load",
  "Target": "Characters/Child_{{Loe2run.FamilyPlanning/FirstChildName}}",
  "FromFile": "assets/first_child_toddler.png",
  "When":
  {
    "Loe2run.FamilyPlanning/FirstChildIsToddler": "true"
  }
},
```
Note: If you run into an issue where Family Planning should have more tokens for your CP pack to work, let me know! I'd be happy to add more as necessary.

### Creating a Content Pack
To create a Content Pack, first download the Example Content Pack from the Nexus page for Family Planning. There are three steps to finishing the Content Pack: you will need to edit the manifest.json, add your image files to the assets folder, and edit the data.json in the assets folder.

#### Manifest.json
Inside the manifest.json, you should replace the text \<Your Name Here\> in the Author and UniqueID fields with your name.
  
For example, my name is Loe2run, so I would replace "\<Your Name Here\>" with "Loe2run". SMAPI will not run your Content Pack if "\<Your Name Here\>" is in the manifest.json file (because the \< character isn't allowed).
  
#### Image files
Decide on which image files you'd like to use for your children and place those files in the assets folder.

For example, let's say that your Farmer is married to Leah and you'd like to use the sprite from Lakoria's BabiesTakeAfterSpouse. You have two daughters, Amber and Beverly, who need new sprites.
You look through the sprites available and decide that:
for Amber, you want "leahbaby.png" as a baby and "leahhairbuns.png" as a toddler,
for Beverly, you want "leahbaby.png" as a baby and "leahpigtails.png" as a toddler.

Copy the image files you want, "leahbaby.png", "leahhairbuns.png", and "leahpigtails.png" to the assets folder. (You can rename these files if it's convenient to you, but it's not necessary. They can have whatever names you want.)

#### Data.json
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

#### Additional Notes
This method currently only works with .png files. I'm hoping to expand to allow .xnb files soon, but until then, your best bet is to try and convert the .xnb file you like to a .png image. More information on that at the [Stardew Valley Wiki.](https://stardewvalleywiki.com/Modding:Editing_XNB_files#Unpack_game_files)

If you don't know the name of your child because they haven't been born yet, that's fine! Just wait until the child is born, name them, wait for the game to save, and then exit to desktop and follow the steps above. The next time your open the game, your newly named child should have a custom sprite.

If you have two children with the same name between save files, the current method means they will end up with the same sprite. I'm hoping to fix this issue in the future.

If you're using this mod for multiple save files and want to have multiple save files worth of children to change, you have two options. You can either put all the child information in the same Content Pack or you can create a Content Pack for each save file. Both methods should work perfectly fine, though I would give the advice that it's easier to sort through multiple small Content Packs than one big one.

Using this method, the gender or skin color of the child has no effect on their sprite appearance. You could decide to use a male toddler sprite for a female child and the Content Pack will go ahead and apply the sprite regardless.

## Spouse Dialogue

In addition to Content Packs allowing for customization of child sprites, they can also allow for customization of spouse dialogue. Your spouse can have a custom dialogue line on the day of a child's birth. This dialogue is also added to the data.json file in the assets folder.

### Data.json

The default version of the data.json in the Example Content Pack will read:

```cs
{
  "SpouseDialogue": {
      "<Spouse Name>": {
        "Item1": <birth number>,
        "Item2": "<dialogue>"
      },
  }
}
```

For an example of how to fill this out, let's say you want your spouse, Maru, to say "(child's name)'s birth reminded me of MARILDA... I wonder what she's doing right now." when your first child is born. You would enter that into your data.json file like this:

```cs
{
  "SpouseDialogue": {
      "Maru": {
        "Item1": 1,
        "Item2": "{0}'s birth reminded me of MARILDA... I wonder what she's doing right now."
      },
  }
}
```
Item1 is a number representing which birth should trigger the dialogue, with 1 as the first child, 2 as the second, etc. Item2 is the dialogue line itself. As you see in this example, the characters "{0}" can be used to represent your child's name. In addition, the characters "{1}" can be used to represent the player's name.

### Using both at once

Here's an example of formatting your data.json to contain both child sprite information and spouse dialogue information:

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
  },
  "SpouseDialogue": {
      "Maru": {
        "Item1": 1,
        "Item2": "{0}'s birth reminded me of MARILDA... I wonder what she's doing right now."
      },
  }
}

```
## Multiplayer

I haven't checked compatibility with multiplayer yet for the 1.4 update. If you're using this mod with multiplayer, proceed with caution.

## Compatibility:

Works with Stardew Valley 1.4 beta on Linux/Mac/Windows.
This mod uses Harmony, so there may be interference with other mods using Harmony to patch the same methods. If you notice any issues, please let me know.

## More details:

-> To uninstall Family Planning, remove the Family Planning mod folder. If you have any content packs for Family Planning, be sure to remove them as well. The children present at the time the mod is removed will not be removed. The only way to remove children is to remove all of them by turning them into doves at the witch's hut.

As described in the multiplayer section above, there are issues when one player has a Family Planning content pack and the other players don't. When uninstalling Family Planning from a multiplayer farm, be certain that no players have a content pack installed to avoid glitches.

-> Family size preferences are, from 1.2.0 on, saved to a file in the data folder. This file has the same name as your save folder. (For example, Farmer_123456789.) This file is what keeps track of the maximum children you can have. If you delete it, the game will generate a new json file and reset your default value to 2 (unless you already have more than 2 children).

-> I've removed the four child maximum from this mod, but I encourage you to stay with four or less. The reason why is because this mod doesn't edit the number of beds in your house. Therefore, all of your children need to share the two existing beds. Two children can fit in a bed together, so with more than four children, they'll start to pile up in bed. Also, children will attempt to share a bed with a sibling of the same gender when possible.
