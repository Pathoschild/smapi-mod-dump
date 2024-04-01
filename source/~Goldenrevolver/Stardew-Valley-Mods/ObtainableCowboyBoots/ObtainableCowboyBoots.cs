/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ObtainableCowboyBoots
{
    using StardewModdingAPI;
    using System;

    public class ObtainableCowboyBoots : Mod
    {
        public ObtainableCowboyBootsConfig Config { get; set; }

        public static IManifest Manifest { get; set; }

        internal static readonly string obtainedCowboyBootsKey = $"{Manifest?.UniqueID}/ObtainedCowboyBoots";

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ObtainableCowboyBootsConfig>();
            Manifest = this.ModManifest;

            ObtainableCowboyBootsConfig.VerifyConfigValues(Config, this);

            helper.Events.GameLoop.GameLaunched += delegate
            {
                ObtainableCowboyBootsConfig.SetUpModConfigMenu(Config, this);
            };

            helper.Events.GameLoop.SaveLoaded += delegate { ResetCowboyHats(); };
            helper.Events.GameLoop.ReturnedToTitle += delegate { ResetCowboyHats(); };

            Patcher.PatchAll(this);
        }

        private static void ResetCowboyHats()
        {
            Patcher.pastCowboyHats.Clear();
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