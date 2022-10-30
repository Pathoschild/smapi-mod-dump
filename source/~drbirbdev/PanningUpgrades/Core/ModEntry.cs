/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using BirbShared.APIs;
using BirbShared;
using BirbShared.Config;
using BirbShared.Command;
using BirbShared.Asset;
using HarmonyLib;
using System.IO;

namespace PanningUpgrades
{
    internal class ModEntry : Mod
    {
        public static ModEntry Instance;
        public static Config Config;
        public static Assets Assets;

        public static IJsonAssetsApi JsonAssets;
        public static ISpaceCore SpaceCore;

        internal ITranslationHelper I18n => this.Helper.Translation;

        public override void Entry(IModHelper helper)
        {
            ModEntry.Instance = this;
            Log.Init(this.Monitor);

            ModEntry.Config = helper.ReadConfig<Config>();
            ModEntry.Assets = new Assets();

            this.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;

            new AssetClassParser(this, Assets).ParseAssets();
        }


        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            new ConfigClassParser(this, Config).ParseConfigs();
            new Harmony(this.ModManifest.UniqueID).PatchAll();
            new CommandClassParser(this.Helper.ConsoleCommands, new Command()).ParseCommands();

            JsonAssets = this.Helper.ModRegistry
                .GetApi<IJsonAssetsApi>
                ("spacechase0.JsonAssets");
            if (JsonAssets is null)
            {
                Log.Error("Can't access the Json Assets API. Is the mod installed correctly?");
                return;
            }

            SpaceCore = this.Helper.ModRegistry
                .GetApi<ISpaceCore>
                ("spacechase0.SpaceCore");
            if (SpaceCore is null)
            {
                Log.Error("Can't access the SpaceCore API. Is the mod installed correctly?");
                return;
            }

            SpaceCore.RegisterSerializerType(typeof(UpgradeablePan));

            JsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets", "PanHats"));
        }
    }
}
