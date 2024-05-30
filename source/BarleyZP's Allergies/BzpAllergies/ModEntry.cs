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
using BZP_Allergies.Config;
using BZP_Allergies.HarmonyPatches;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore.Events;
using SpaceCore.Interface;
using SpaceCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;
using StardewValley;

using static BZP_Allergies.AllergenManager;
using BZP_Allergies.HarmonyPatches.UI;

namespace BZP_Allergies
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        public static ModEntry Instance { get; private set; }

        private Harmony Harmony;
        public ModConfigModel Config;
        public IModHelper ModHelper;
        public ITranslationHelper Translation;

        public static readonly ISet<string> NpcsThatReactedToday = new HashSet<string>();

        public static readonly string MOD_ID = "BarleyZP.BzpAllergies";

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper modHelper)
        {
            Instance = this;

            ModHelper = modHelper;
            Translation = modHelper.Translation;

            // allergen manager
            AllergenManager.InitDefault();

            // events
            modHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
            modHelper.Events.Content.AssetRequested += OnAssetRequested;
            modHelper.Events.GameLoop.DayStarted += OnDayStarted;
            modHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            modHelper.Events.GameLoop.OneSecondUpdateTicking += OnOneSecondUpdateTicking;
            modHelper.Events.Input.ButtonPressed += OnButtonPressed;

            // config
            Config = Helper.ReadConfig<ModConfigModel>();

            // harmony patches
            Harmony = new(ModManifest.UniqueID);
            Harmony.DEBUG = true;  // disable for publishing

            CraftingCooking_Patches.Patch(Harmony);
            FarmerEating_Patches.Patch(Harmony);
            Inventory_Patches.Patch(Harmony);
            NpcDialogue_Patches.Patch(Harmony);
            SkillBook_Patches.Patch(Harmony);
            SpaceCoreCustomCraftingRecipe_Patches.Patch(Harmony);
            UI_Patches.Patch(Harmony);

            // console commands
            modHelper.ConsoleCommands.Add("bzpa_list_allergens", "Get a list of all possible allergens.", ListAllergens);
            modHelper.ConsoleCommands.Add("bzpa_get_held_allergens", "Get the allergens of the currently-held item.", GetAllergensOfHeldItem);
            modHelper.ConsoleCommands.Add("bzpa_player_allergies", "Get the player's allergies.", GetPlayerAllergies);
        }


        /*********
        ** Private methods
        *********/

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (e.Button == Config.AllergyPageButton && Game1.activeClickableMenu == null)
            {
                Game1.activeClickableMenu = new AllergyOptionsMenu(Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2,
                    Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2,
                    800 + IClickableMenu.borderWidth * 2,
                    600 + IClickableMenu.borderWidth * 2,
                    true);
            }
        }

        /// <inheritdoc cref="IContentEvents.AssetRequested"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("BarleyZP.BzpAllergies/Sprites"))
            {
                e.LoadFromModFile<Texture2D>(PathUtilities.NormalizePath(@"assets/Sprites.png"), AssetLoadPriority.Medium);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("BarleyZP.BzpAllergies/AllergyData"))
            {
                e.LoadFrom(() => AllergenManager.ALLERGEN_DATA, AssetLoadPriority.Medium);
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
            // get Generic Mod Config Menu's API (if it's installed)
            var GmcmApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (GmcmApi is null)
            {
                Monitor.Log("GMCM API not found.", LogLevel.Error);
                return;
            }

            // config
            GmcmApi.Register(
                mod: ModManifest,
                reset: () => {
                    Config = new ModConfigModel();
                },
                save: () =>
                {
                    Helper.WriteConfig(Config);
                    Config = Helper.ReadConfig<ModConfigModel>();
                }
            );
            
            ConfigMenuInit.SetupMenuUI(GmcmApi, ModManifest);

            // CP API
            var ContentPatcherApi = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (ContentPatcherApi is null)
            {
                Monitor.Log("CP API not found.", LogLevel.Error);
                return;
            }

            ContentPatcherApi.RegisterToken(ModManifest, "ReadAllergyCookbook", ReadAllergyCookbookToken);
        }

        /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            // make sure all the allergens the player "has" and "discovered" still exist
            ISet<string> has = ModDataSetGet(Game1.player, Constants.ModDataHas);
            ISet<string> discovered = ModDataSetGet(Game1.player, Constants.ModDataDiscovered);
            foreach (string id in has)
            {
                if (!ALLERGEN_DATA_ASSET.ContainsKey(id))
                {
                    ModDataSetRemove(Game1.player, Constants.ModDataHas, id);
                }
            }

            foreach (string id in discovered)
            {
                if (!ALLERGEN_DATA_ASSET.ContainsKey(id))
                {
                    ModDataSetRemove(Game1.player, Constants.ModDataDiscovered, id);
                }
            }
        }

        /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            NpcsThatReactedToday.Clear();
        }

        /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicking"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnOneSecondUpdateTicking(object? sender, OneSecondUpdateTickingEventArgs e)
        {
            if (!Config.HoldingReaction) return;

            bool hasReactionDebuff = Game1.player.hasBuff(Constants.ReactionDebuff);

            StardewValley.Object? held = Game1.player.ActiveObject;
            bool allergic = FarmerIsAllergic(held);
            if (held is not null && allergic)
            {
                if (hasReactionDebuff && !e.IsMultipleOf(300)) return;
                Game1.player.applyBuff(GetAllergicReactionBuff(held.DisplayName, "hold", Config.EatingDebuffLengthSeconds / 3));
                CheckForAllergiesToDiscover(Game1.player, GetAllergensInObject(held));
            }
        }

        private void ListAllergens(string command, string[] args) {

            string result = "\n{Allergen Id}: {Allergen Display Name}";

            foreach (var item in ALLERGEN_DATA_ASSET)
            {
                result += "\n  " + item.Key + ": " + item.Value.DisplayName;
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

        private void GetPlayerAllergies(string command, string[] args)
        {
            ISet<string> has = ModDataSetGet(Game1.player, Constants.ModDataHas);
            ISet<string> discovered = ModDataSetGet(Game1.player, Constants.ModDataDiscovered);

            string result = "\n{Allergen Id}: {Discovered}";
            foreach (string a in has)
            {
                result += "\n  " + a + ": " + discovered.Contains(a);
            }
            
            Monitor.Log(result, LogLevel.Info);
        }
    }
}