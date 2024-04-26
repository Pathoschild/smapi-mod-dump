/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using BZP_Allergies.Apis;
using BZP_Allergies.AssetPatches;
using BZP_Allergies.Config;
using BZP_Allergies.ContentPackFramework;
using BZP_Allergies.HarmonyPatches;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using static BZP_Allergies.AllergenManager;

namespace BZP_Allergies
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        private Harmony Harmony;
        public static ModConfig Config;
        private IModHelper ModHelper;

        public static readonly ISet<string> NpcsThatReactedToday = new HashSet<string>();

        public static readonly string MOD_ID = "BarleyZP.BzpAllergies";

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper modHelper)
        {
            ModHelper = modHelper;
            Initializable.Initialize(Monitor, ModHelper.GameContent, ModHelper.ModContent);

            // allergen manager
            AllergenManager.InitDefaultDicts();

            // events
            modHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
            modHelper.Events.Content.AssetRequested += OnAssetRequested;
            modHelper.Events.GameLoop.DayStarted += OnDayStarted;

            // config
            Config = Helper.ReadConfig<ModConfig>();

            // harmony patches

            Harmony = new(ModManifest.UniqueID);
            Harmony.PatchAll();

            // console commands
            modHelper.ConsoleCommands.Add("bzpa_list_allergens", "Get a list of all possible allergens.", ListAllergens);
            modHelper.ConsoleCommands.Add("bzpa_get_held_allergens", "Get the allergens of the currently-held item.", GetAllergensOfHeldItem);
            modHelper.ConsoleCommands.Add("bzpa_reload", "Reload all content packs.", ReloadPacks);
        }


        /*********
        ** Private methods
        *********/

        /// <inheritdoc cref="IContentEvents.AssetRequested"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                foreach (string a in ALLERGEN_TO_DISPLAY_NAME.Keys)
                {
                    PatchObjects.AddAllergen(e, a);
                }
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Mods/BarleyZP.BzpAllergies/Sprites"))
            {
                e.LoadFromModFile<Texture2D>(PathUtilities.NormalizePath(@"assets/Sprites.png"), AssetLoadPriority.Medium);
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                Monitor.Log("No mod config menu API found.", LogLevel.Debug);
                return;
            }

            // content packs
            LoadContentPacks.LoadPacks(Helper.ContentPacks.GetOwned(), Config);

            // config
            configMenu.Register(
                mod: ModManifest,
                reset: () => {
                    Config = new ModConfig();
                },
                save: () => {
                    Helper.WriteConfig(Config);
                    Config = Helper.ReadConfig<ModConfig>();
                },
                titleScreenOnly: false
            );

            ConfigMenuInit.SetupMenuUI(configMenu, ModManifest);
            foreach (IContentPack pack in Helper.ContentPacks.GetOwned())
            {
                ConfigMenuInit.SetupContentPackConfig(configMenu, ModManifest, pack);
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            NpcsThatReactedToday.Clear();
        }

        private void ListAllergens(string command, string[] args) {

            string result = "\n{Allergen Id}: {Allergen Display Name}";

            foreach (var item in AllergenManager.ALLERGEN_TO_DISPLAY_NAME)
            {
                result += "\n\t" + item.Key + ": " + item.Value;
            }

            Monitor.Log(result, LogLevel.Info);
        }

        private void GetAllergensOfHeldItem(string command, string[] args)
        {
            ISet<string> result = new HashSet<string>();
            Item currItem = Game1.player.CurrentItem;

            if (currItem is StardewValley.Object currObj)
            {
                result = GetAllergensInObject(currObj);
            }

            Monitor.Log(string.Join(", ", result), LogLevel.Info);
        }

        private void ReloadPacks(string command, string[] args)
        {
            AllergenManager.InitDefaultDicts();
            LoadContentPacks.LoadPacks(Helper.ContentPacks.GetOwned(), Config);
            Helper.GameContent.InvalidateCache("Data/Objects");
            Helper.GameContent.InvalidateCache(asset => asset.NameWithoutLocale.StartsWith("Characters/Dialogue/"));
        }
    }
}