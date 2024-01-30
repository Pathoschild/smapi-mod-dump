/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using AndroidPatcher.Patches;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace AndroidPatcher
{
    public class ModEntry : Mod
    {
        private PatchManager patchManager;

        public override void Entry(IModHelper helper)
        {
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                patchManager = new PatchManager(this.Helper, this.ModManifest, this.Helper.ReadConfig<Config>());
                patchManager.Apply();
            }
            else
            {
                Monitor.Log($"Sorry but mod only support platform Android", LogLevel.Error);
            }
        }
    }
}
