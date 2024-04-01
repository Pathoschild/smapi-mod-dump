/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace CrabPotQuality
{
    using StardewModdingAPI;
    using System;

    public class CrabPotQuality : Mod
    {
        public static CrabPotQualityConfig Config { get; set; }

        public static IManifest Manifest { get; set; }

        public static CrabPotQuality Mod { get; set; }

        public override void Entry(IModHelper helper)
        {
            Mod = this;
            Config = Helper.ReadConfig<CrabPotQualityConfig>();
            Manifest = ModManifest;

            Helper.Events.GameLoop.GameLaunched += delegate { CrabPotQualityConfig.SetUpModConfigMenu(Config, this); };

            Patcher.PatchAll(this, Config);
        }

        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }
    }
}