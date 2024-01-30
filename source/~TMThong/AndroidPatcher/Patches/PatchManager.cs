/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidPatcher.Patches
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


            this.Patches.Add(new FarmerPatch());
        }
        public void Apply()
        {
            foreach (var patch in Patches)
            {
                var p = patch;
                p.Apply(Harmony);

            }
        }
    }
}
