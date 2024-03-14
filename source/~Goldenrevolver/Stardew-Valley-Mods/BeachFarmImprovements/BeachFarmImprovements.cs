/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace BeachFarmImprovements
{
    using StardewModdingAPI;
    using StardewValley;
    using System;

    public class BeachFarmImprovements : Mod
    {
        public BeachFarmImprovementsConfig Config { get; set; }

        public static IManifest Manifest { get; set; }

        // this mod is still early in development

        // TODO spawn ship
        // TODO farm expansion
        // TODO make driftwood piles destructable

        // TODO maybe add mussel rocks from ginger island

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<BeachFarmImprovementsConfig>();
            Manifest = this.ModManifest;

            BeachFarmImprovementsConfig.VerifyConfigValues(Config, this);

            helper.Events.GameLoop.GameLaunched += delegate
            {
                BeachFarmImprovementsConfig.SetUpModConfigMenu(Config, this);
            };

            //helper.Events.Content.AssetRequested += OnAssetRequested;

            Patcher.PatchAll(this);
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

        // in 1.5 these strings existed in StringsFromCSFiles
        // TODO not in 1.6 alpha, check again 1.6 full release
        //"FarmImprovement_Guy_1": "Pssst... hey, mainlander. Listen up. If you've got coin... me and my boys can improve your farm. Interested?",
        //"FarmImprovement_Guy_2": "Tell me more...",
        //"FarmImprovement_Guy_3": "No, thanks.",
        //"FarmImprovement_Guy_4": "I can get my boys to add a little extra space on your farm. It'll just be... 500,000g. What do you say?",
        //"FarmImprovement_Guy_5": "I can get my boys to improve the sand, allowing you to use sprinklers.  I'll just need 100,000g. What do you say?",
        //"FarmImprovement_Guy_6": "Heh heh heh... perfect. You can expect the changes tomorrow morning.",

        public static bool HasUnlockedPirateVisits { get => Utility.doesAnyFarmerHaveMail("gotBoatPainting"); }

        private const string hasUnlockedSprinklersInSandKey = "HasUnlockedSprinklersInSand";

        private const string hasUnlockedFarmExpansionKey = "HasUnlockedFarmExpansion";

        public static bool HasUnlockedSprinklersInSand
        {
            get
            {
                return Game1.MasterPlayer.modData.ContainsKey($"{Manifest.UniqueID}/{hasUnlockedSprinklersInSandKey}");
            }
            set
            {
                Game1.MasterPlayer.modData[$"{Manifest.UniqueID}/{hasUnlockedSprinklersInSandKey}"] = "true";
            }
        }

        public static bool HasUnlockedFarmExpansion
        {
            get
            {
                return Game1.MasterPlayer.modData.ContainsKey($"{Manifest.UniqueID}/{hasUnlockedFarmExpansionKey}");
            }
            set
            {
                Game1.MasterPlayer.modData[$"{Manifest.UniqueID}/{hasUnlockedFarmExpansionKey}"] = "true";
            }
        }
    }
}