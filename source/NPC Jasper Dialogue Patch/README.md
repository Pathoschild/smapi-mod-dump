# NPCJasperFix
A bug patch C# mod for Lemurkat's Jasper NPC.

## Overview
This is a patch mod for Stardew Valley that corrects a bug in the game code affecting NPCs (especially dialogue). 

**Problem:** when checking if *any* NPC should load marriage dialogue, the game currently checks if `Game1.player.spouse.Contains(this.Name)` instead of `Game1.player.spouse.Equals(this.Name)`.

**Result:** for a character like Jas—when the player is married to [Lemurkat's custom NPC Jasper](https://www.nexusmods.com/stardewvalley/mods/5599)—the game checks if `"Jasper".Contains("Jas")` and returns `true` because the "Jas" substring was found. The game will then try to load marriage dialogue for Jas. Because she doesn't have any, it loads the default marriage dialogue string `""` and **Jas will go completely silent, never having any dialogue.**

This same bug could potentially happen with other custom NPCs, especially for vanilla and custom NPCs with short names: Sam, Gus, Pam, Alex, Jas... and names like Jo, Eve, Ann, Max, Rob.

This patch mod fixes the problem.

## Permissions
This code is available under the MIT license. You are free to use, edit, modify, copy, rename, and re-distribute it with no restrictions. Credit is nice, but **not** required!

### NPC (content pack) modders:

If you are an NPC modder affected by this bug, feel free to copy and include this mod alongside your NPC's content packs! You don't need to re-compile the code yourself, either. All you need to do is this:
1. Download the latest version of the .zip package from the [bin/](https://github.com/Jonqora/NPCJasperFix/tree/master/NPCJasperFix/bin/x86/Debug) folder
2. Unzip the folder and open the `manifest.json` file in any text editor
3. Change the `UniqueID` field (**mandatory**).
4. Change the `Name` field (important for keeping SMAPI logs unambiguous)
5. Optional: edit the `Author` and `Description` fields for clarity
6. Do **not** change the `EntryDll` field. This has to stay the same.
7. Save the file, rename the containing folder (from `NPCJasperFix/` to whatever you like), and include it with your content pack folder(s) for distribution.

Example edited manifest:

    {
      "Name": "YourNPCName Bug Patch",
      "Author": "Jonqora (for YourName)",
      "Version": "1.1.0",
      "Description": "Patches a bug that can affect NPC dialogue.",
      "UniqueID": "YourName.YourNPCNameFix",
      "EntryDll": "NPCJasperFix.dll",
      "MinimumApiVersion": "3.0.0",
      "UpdateKeys": [ ]
    }

### C# modders:
If you want to include some or all of this code in another mod, especially one meant for NPC improvements or game code bugfixes, go right ahead!

## Patch Details
To fix the error, this mod uses a [harmony transpiler patch](https://harmony.pardeike.net/articles/patching-transpiler.html) to edit the game code and swap the String.Contains method call for a more appropriate String.Equals comparison instead. It applies the patch to two methods in the game:

In the `NPC.loadCurrentDialogue` method:

    ...
    dialogueStack.Clear();
    if (Game1.player.spouse != null && Game1.player.spouse.Contains(this.Name))
    { ...

In the `NPC.tryToReceiveActiveObject` method (jealousy response code):

    ...
    if (!(bool) (NetFieldBase<bool, NetBool>) this.datable || who.spouse == null || (who.spouse.Contains(this.Name) || 
        who.spouse.Contains("Krobus")) || (Utility.isMale(who.spouse) != Utility.isMale(this.Name) || 
        Game1.random.NextDouble() >= 0.3 - (double) who.LuckLevel / 100.0 - who.DailyLuck || 
        (this.isBirthday(Game1.currentSeason, Game1.dayOfMonth) || !who.friendshipData[this.Name].IsDating())))
      return;
    ...

Harmony patches are applied when SMAPI loads the game.
* When a patch is successful, this mod will log a Trace message to the console.
* When a patch is unsuccessful, it will log an Info message and have no effect. This might happen if another mod has fixed the same NPC bug already.
* If a patch finds multiple insertion points, it will patch them all. This logs a warning message to the console since only one correction by each patch is expected.

So long as each mod has a unique `manifest.json`, running multiple mods that use this patch code will not cause any errors.
