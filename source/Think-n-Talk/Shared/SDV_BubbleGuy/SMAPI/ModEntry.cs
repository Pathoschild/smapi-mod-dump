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

namespace SDV_Speaker.SMAPIInt
{
    internal class ModEntry : Mod
    {
        private IModHelper oHelper;
        private BubbleGuyManager oManager;
        public override void Entry(IModHelper helper)
        {
            oHelper = helper;
            oManager = new BubbleGuyManager(Path.Combine(helper.DirectoryPath, "saves"), Path.Combine(helper.DirectoryPath, "sprites"), helper, Monitor);
            BubbleChat.Initialize(oManager);
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;

            Harmony harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
          original: AccessTools.Method(typeof(ChatBox), "runCommand", new Type[] { typeof(string) }),
          prefix: new HarmonyMethod(typeof(BubbleChat), nameof(BubbleChat.RunCommand))
          );

            Monitor.Log($"Harmony patch applied", LogLevel.Info);

        }
        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {

            BubbleGuyStatics.Initialize(Path.Combine(oHelper.DirectoryPath, "Sprites"));
#if DEBUG
            Monitor.Log($"BubbleGuy name: '{BubbleGuyStatics.BubbleGuyName}'", LogLevel.Info);
#endif
        }
    }

}