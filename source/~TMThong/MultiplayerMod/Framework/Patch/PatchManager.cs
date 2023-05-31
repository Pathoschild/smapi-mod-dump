/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using StardewModdingAPI;
using MultiplayerMod;
using HarmonyLib;
using MultiplayerMod.Framework.Patch.Mobile;

namespace MultiplayerMod.Framework.Patch
{
    internal class PatchManager
    {
        public IModHelper Helper { get; set; }
        public IManifest Manifest { get; set; }
        public Config Config { get; set; }
        public List<IPatch> Patches { get; set; } = new List<IPatch>();
        public Harmony Harmony { get; }
        public PatchManager(IModHelper helper, IManifest manifest, Config config)
        {
            Helper = helper;
            Manifest = manifest;
            Config = config;
            Harmony = new Harmony(Manifest.UniqueID);
            Patches.Add(new GameServerPatch());
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                Patches.Add(new TitleMenuPatch());
                Patches.Add(new DebrisPatch());
                Patches.Add(new Game1Patch());
                Patches.Add(new IClickableMenuPatch());
                Patches.Add(new MobileCustomizerPatch());
                Patches.Add(new MobileFarmChooserPatch());
                Patches.Add(new SaveGamePatch());
            }
            else
            {
                Patches.Add(new CoopMenuPatch());
            }
            // Patches.Add(new SGamePatch());
            
        }
        public void Apply()
        {
            foreach (var patch in Patches)
            {
                var p = patch;
                p.Apply(Harmony);
                ModUtilities.ModMonitor.Log($"Patch {p.GetType().Name} done...", LogLevel.Alert);
            }
        }
    }
}
