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
        internal static bool MargoLoaded;

        internal static IJsonAssetsApi JsonAssets;
        internal static IDynamicGameAssetsApi DynamicGameAssets;
        internal static IMargo MargoAPI;

        internal ITranslationHelper I18n => this.Helper.Translation;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Log.Init(this.Monitor);

            Config = helper.ReadConfig<Config>();

            Assets = new Assets();
            new AssetClassParser(this, Assets).ParseAssets();

            this.Helper.Events.GameLoop.GameLaunched += this.Event_GameLaunched;
            this.Helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
        }

        private void Event_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            new ConfigClassParser(this, Config).ParseConfigs();
            new Harmony(this.ModManifest.UniqueID).PatchAll();
            new CommandClassParser(this.Helper.ConsoleCommands, new Command()).ParseCommands();

            RSVLoaded = this.Helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage");
            AutomateLoaded = this.Helper.ModRegistry.IsLoaded("Pathoschild.Automate");
            JALoaded = this.Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets");
            DGALoaded = this.Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets");
            MargoLoaded = this.Helper.ModRegistry.IsLoaded("DaLion.Overhaul");

            // Register binning skill after checking if MARGO is loaded.
            SpaceCore.Skills.RegisterSkill(new BinningSkill());

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

            if (MargoLoaded)
            {
                MargoAPI = this.Helper.ModRegistry.GetApi<IMargo>("DaLion.Overhaul");
                if (MargoAPI is null)
                {
                    Log.Error("Can't access the MARGO API. Is the mod installed correctly?");
                }
            }
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (MargoAPI is not null)
            {
                string id = SpaceCore.Skills.GetSkill("drbirbdev.Binning").Id;
                MargoAPI.RegisterCustomSkillForPrestige(id);
            }
        }
    }
}
