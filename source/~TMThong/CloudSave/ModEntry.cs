/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using CloudSave.Framework.Patch;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CloudSave
{
    public class ModEntry : Mod
    {
        internal Config config;

        private PatchManager PatchManager;

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<Config>();
            PatchManager = new PatchManager(helper, this.ModManifest, config);
            PatchManager.Apply();
        }
    }
}
