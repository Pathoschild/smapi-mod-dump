/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbShared;
using BirbShared.APIs;
using BirbShared.Asset;
using BirbShared.Command;
using BirbShared.Config;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BinningSkill
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance;
        internal static Config Config;
        internal static Assets Assets;

        internal static bool RSVLoaded;
        internal static bool AutomateLoaded;
        internal static bool JALoaded;
        internal static bool DGALoaded;

        internal static IJsonAssetsApi JsonAssets;
        internal static IDynamicGameAssetsApi DynamicGameAssets;

        internal ITranslationHelper I18n => this.Helper.Translation;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Log.Init(this.Monitor);

            Config = helper.ReadConfig<Config>();

            Assets = new Assets();
            new AssetClassParser(this, Assets).ParseAssets();

            this.Helper.Events.GameLoop.GameLaunched += this.Event_GameLaunched;
        }

        private void Event_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            RSVLoaded = this.Helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage");
            AutomateLoaded = this.Helper.ModRegistry.IsLoaded("Pathoschild.Automate");
            JALoaded = this.Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets");
            DGALoaded = this.Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets");

            if (JALoaded)
            {
                JsonAssets = this.Helper.ModRegistry
                    .GetApi<IJsonAssetsApi>
                    ("spacechase0.JsonAssets");
                if (JsonAssets is null)
                {
                    Log.Error("Can't access the Json Assets API. Is the mod installed correctly?");
                }
            }

            if (DGALoaded)
            {
                DynamicGameAssets = this.Helper.ModRegistry
                    .GetApi<IDynamicGameAssetsApi>
                    ("spacechase0.DynamicGameAssets");
                if (DynamicGameAssets is null)
                {
                    Log.Error("Can't access the Dynamic Game Assets API. Is the mod installed correctly?");
                }
            }

            new ConfigClassParser(this, Config).ParseConfigs();
            new Harmony(this.ModManifest.UniqueID).PatchAll();
            new CommandClassParser(this.Helper.ConsoleCommands, new Command()).ParseCommands();
            SpaceCore.Skills.RegisterSkill(new BinningSkill());
        }
    }
}
