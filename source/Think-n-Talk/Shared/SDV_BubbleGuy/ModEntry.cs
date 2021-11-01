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
using StardewValley.Menus;
using SDV_Speaker.Speaker;
using System.IO;
using HarmonyLib;
using SDV_BubbleGuy.SMAPIInt;
using StardewModHelpers;


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
                SDV_Logger.Init(Monitor,helper.DirectoryPath, true);
                StardewThreadSafeLoader.Initialize(helper);
                SMAPIHelpers.Initialize(helper);
                BubbleGuyStatics.Initialize(helper.DirectoryPath);
                Monitor.Log($"Asset directory: {BubbleGuyStatics.AssetsPath}", LogLevel.Info);
                oManager = new BubbleGuyManager(BubbleGuyStatics.SavesPath, BubbleGuyStatics.AssetsPath, helper, Monitor, true);
                oManager.StartBubbleChat(ModManifest.UniqueID);
            }
        }
        public override object GetApi()
        {
            return new BubbleGuyAPI(oManager);
        }
    }

}