/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/Think-n-Talk
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SDV_Speaker.Speaker;
using System.IO;
using SDV_BubbleGuy.SMAPIInt;


namespace SDV_Speaker.SMAPIInt
{
    internal class ModEntry : Mod
    {
        private BubbleGuyManager oManager;
        public override void Entry(IModHelper helper)
        {
            //
            //  check for Stardew Web, if installed
            //  do not load this mod
            //
            if (helper.ModRegistry.IsLoaded("prism99.stardewweb"))
            {
                Monitor.Log("Stardew Web is installed, this mod is not needed an will not be loaded.", LogLevel.Info);
            }
            else
            {
                SMAPIHelpers.Initialize(helper, Monitor);
                string sAssetPath = Path.Combine(helper.DirectoryPath, "assets","bubbleguy");
                oManager = new BubbleGuyManager(Path.Combine(helper.DirectoryPath, "saves"),sAssetPath, helper, Monitor, true);
                oManager.StartBubbleChat(ModManifest.UniqueID);
                helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            }
         }
        public override object GetApi()
        {
            return new BubbleGuyAPI(oManager);
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {

            BubbleGuyStatics.Initialize(Path.Combine(SMAPIHelpers.helper.DirectoryPath, "Sprites"));
             
#if DEBUG
            Monitor.Log($"BubbleGuy name: '{BubbleGuyStatics.BubbleGuyName}'", LogLevel.Info);
#endif
        }
    }

}