**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/AndyCrocker/StardewMods**

----

In the 'Example' folder, you will see a variety of examples I've made. You can just copy them out and replace the Cursor.png IMPORTANT: YOU NEED TO RENAME THEM THE Cursor.png OTHERWISE IT WILL NOT BE PICKED UP BY THE CODE 
(They have incrementing numbers due to Windows not allowing multiple files with the same name in the same folder) that is in the root of Assets if you want to use one of those (They are only colour changes).

You will also see a 'Base.png' this is the default cursor and you can use this as a base for a custom cursor if you wish (Useful so you know the layout of cursors etc. Also good as you can see the rough boundries for each cursor
so you can see the limit if you make a new cursor that changes the shape).

If you have no Cursor asset in the Assset folder you will get a warning in console and the default cursor will be used. 

Make sure to keep the default dimensions (126x27 WxH).

If you want multiple cursors saved, simply make a folder in the Assets folder called anything you like and store them in there, the mod will not detect assets that are not in the root of the Assets folder.
This makes it easier as they are stored in the same folder if you want to switch out cursors.

Make sure the asset is called 'Cursor' (Must be caps specific)(The mod only supports png assets, no XNB - png is also the only picture format that SMAPI supports)

Tip: When creating your own assets, make sure the unused space is transparent and not white. This will show as the cursor surrounded in white otherwise
It is also important that when you don't change part of the asset (the console buttons for example) that you keep them as default - This is because the mod replaces that part of the texture (instead of overlaying it
the reason is so the default cursor doesn't show through if you make the cursor smaller)