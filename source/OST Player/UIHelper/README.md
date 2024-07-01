**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ProfeJavix/StardewValleyMods**

----

**UI Helper**: A dependency to help you save time and effort making your SMAPI UI

To use it, you'll need to ***copy the IUIHelperApi.cs to your SMAPI project folder***. Then, in ModEntry you have to ***create an instance and initialize it*** like any other API:
>IUIHelperApi api = Helper.ModRegistry.GetApi\<IUIHelperApi\>("ProfeJavix.UIHelper");
When starting StardewValley.exe, make sure you have the Nexus's file in Mods folder.
And that's it! You'll have access to a set of functionalities that will make easier the UI building :grin:. Every method and param its explained in the file.
