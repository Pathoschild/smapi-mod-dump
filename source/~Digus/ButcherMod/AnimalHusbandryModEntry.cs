using System;
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.farmer;
using AnimalHusbandryMod.meats;
using AnimalHusbandryMod.tools;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using DataLoader = AnimalHusbandryMod.common.DataLoader;
using SObject = StardewValley.Object;

namespace AnimalHusbandryMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class AnimalHusbandryModEntry : Mod
    {
        internal static IModHelper ModHelper;
        internal static IMonitor monitor;
        internal static DataLoader DataLoader;
        private SButton? _meatCleaverSpawnKey;
        private SButton? _inseminationSyringeSpawnKey;
        private SButton? _feedingBasketSpawnKey;
        private bool IsEnabled = true;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            monitor = Monitor;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            if (this.Helper.ModRegistry.IsLoaded("DIGUS.BUTCHER"))
            {
                Monitor.Log("Animal Husbandry Mod can't run along side its older version, ButcherMod. " +
                    "You need to copy the 'data' directory from the ButcherMod directory, into the AnimalHusbandryMod directory, then delete the ButcherMod directory. " +
                    "Animal Husbandry Mod won't load until this is done.", LogLevel.Error);
                IsEnabled = false;
            }
            else
            {
                DataLoader = new DataLoader(Helper);
                _meatCleaverSpawnKey = DataLoader.ModConfig.AddMeatCleaverToInventoryKey;
                _inseminationSyringeSpawnKey = DataLoader.ModConfig.AddInseminationSyringeToInventoryKey;
                _feedingBasketSpawnKey = DataLoader.ModConfig.AddFeedingBasketToInventoryKey;

                //TimeEvents.AfterDayStarted += (x, y) => EventsLoader.CheckEventDay();

                if (!DataLoader.ModConfig.DisableMeat)
                    ModHelper.ConsoleCommands.Add("player_addallmeatrecipes", "Add all meat recipes to the player.", DataLoader.RecipeLoader.AddAllMeatRecipes);

                if (_meatCleaverSpawnKey != null || _inseminationSyringeSpawnKey != null || _feedingBasketSpawnKey != null)
                    Helper.Events.Input.ButtonPressed += this.OnButtonPressed;

                var harmony = HarmonyInstance.Create("Digus.AnimalHusbandryMod");

                try
                {
                    harmony.Patch(
                        original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.pet)),
                        prefix: new HarmonyMethod(typeof(AnimalQueryMenuExtended), nameof(AnimalQueryMenuExtended.Pet))
                    );
                }
                catch (Exception)
                {
                    Monitor.Log("Error patching the FarmAnimal 'pet' Method. Applying old method of opening the extended animal query menu.", LogLevel.Warn);
                    Helper.Events.Display.MenuChanged += (s, e) =>
                    {
                        if (e.NewMenu is AnimalQueryMenu && !(e.NewMenu is AnimalQueryMenuExtended))
                        {
                            Game1.activeClickableMenu = new AnimalQueryMenuExtended(this.Helper.Reflection.GetField<FarmAnimal>(e.NewMenu, "animal").GetValue());
                        }
                    };
                }

                if (!DataLoader.ModConfig.DisableRancherMeatPriceAjust)
                {
                    harmony.Patch(
                        original: AccessTools.Method(typeof(SObject), nameof(SObject.sellToStorePrice)),
                        prefix: new HarmonyMethod(typeof(MeatOverrides), nameof(MeatOverrides.sellToStorePrice))
                    );
                }

                if (!DataLoader.ModConfig.DisableMeat)
                {
                    harmony.Patch(
                        original: AccessTools.Method(typeof(SObject), nameof(SObject.isPotentialBasicShippedCategory)),
                        prefix: new HarmonyMethod(typeof(MeatOverrides), nameof(MeatOverrides.isPotentialBasicShippedCategory))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(SObject), nameof(SObject.countsForShippedCollection)),
                        prefix: new HarmonyMethod(typeof(MeatOverrides), nameof(MeatOverrides.countsForShippedCollection))
                    );
                }

                //harmony.Patch(
                //    original: AccessTools.Method(typeof(Event), "addSpecificTemporarySprite"),
                //    transpiler: new HarmonyMethod(typeof(EventsOverrides), nameof(EventsOverrides.addSpecificTemporarySprite))
                //);

                //harmony.Patch(
                //    original: AccessTools.Method(typeof(Pet), nameof(Pet.checkAction)),
                //    prefix: new HarmonyMethod(typeof(PetOverrides), nameof(PetOverrides.checkAction))
                //);
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!IsEnabled)
                return;

            DataLoader.ToolsLoader.ReplaceOldTools();
            FarmerLoader.LoadData();
            DataLoader.ToolsLoader.LoadMail();
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!IsEnabled)
                return;

            DataLoader.LivingWithTheAnimalsChannel.CheckChannelDay();
            if (!DataLoader.ModConfig.DisableMeat)
                DataLoader.RecipeLoader.MeatFridayChannel.CheckChannelDay();
            if (!DataLoader.ModConfig.DisablePregnancy)
            {
                PregnancyController.CheckForBirth();
                PregnancyController.UpdatePregnancy();
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!IsEnabled || !Context.IsWorldReady)
                return;

            if (e.Button == _meatCleaverSpawnKey)
                Game1.player.addItemToInventory(new MeatCleaver());

            if (e.Button == _inseminationSyringeSpawnKey)
                Game1.player.addItemToInventory(new InseminationSyringe());
                
            if (e.Button == _feedingBasketSpawnKey)
                Game1.player.addItemToInventory(new FeedingBasket());
        }
    }
}
