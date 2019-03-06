using System;
using AnimalHusbandryMod.animals;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Harmony;
using AnimalHusbandryMod.farmer;
using AnimalHusbandryMod.tools;
using AnimalHusbandryMod.meats;
using DataLoader = AnimalHusbandryMod.common.DataLoader;

namespace AnimalHusbandryMod
{
    public class AnimalHusbandryModEntery : Mod
    {

        internal static IModHelper ModHelper;
        internal static IMonitor monitor;
        internal static DataLoader DataLoader;
        private SButton? _meatCleaverSpawnKey;
        private SButton? _inseminationSyringeSpawnKey;
        private SButton? _feedingBasketSpawnKey;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            monitor = Monitor;

            if (this.Helper.ModRegistry.IsLoaded("DIGUS.BUTCHER"))
            {
                Monitor.Log("Animal Husbandry Mod can't run along side its older version, ButcherMod. " +
                    "You need to copy the 'data' directory from the ButcherMod directory, into the AnimalHusbandryMod directory, then delete the ButcherMod directory. " +
                    "Animal Husbandry Mod won't load until this is done.", LogLevel.Error);
            }
            else
            {
                DataLoader = new DataLoader(helper);
                _meatCleaverSpawnKey = DataLoader.ModConfig.AddMeatCleaverToInventoryKey;
                _inseminationSyringeSpawnKey = DataLoader.ModConfig.AddInseminationSyringeToInventoryKey;
                _feedingBasketSpawnKey = DataLoader.ModConfig.AddFeedingBasketToInventoryKey;

                helper.Events.GameLoop.SaveLoaded += DataLoader.ToolsLoader.ReplaceOldTools;
                helper.Events.GameLoop.SaveLoaded += (x, y) => FarmerLoader.LoadData();
                helper.Events.GameLoop.SaveLoaded += (x, y) => DataLoader.ToolsLoader.LoadMail();

                helper.Events.GameLoop.DayStarted += (x, y) => DataLoader.LivingWithTheAnimalsChannel.CheckChannelDay();

                //TimeEvents.AfterDayStarted += (x, y) => EventsLoader.CheckEventDay();

                if (!DataLoader.ModConfig.DisableMeat)
                {
                    helper.Events.GameLoop.DayStarted += (x, y) => DataLoader.RecipeLoader.MeatFridayChannel.CheckChannelDay();
                    ModHelper.ConsoleCommands.Add("player_addallmeatrecipes", "Add all meat recipes to the player.", DataLoader.RecipeLoader.AddAllMeatRecipes);
                }

                if (_meatCleaverSpawnKey != null || _inseminationSyringeSpawnKey != null || _feedingBasketSpawnKey != null)
                {
                    helper.Events.Input.ButtonPressed += this.OnButtonPressed;
                }

                if (!DataLoader.ModConfig.DisablePregnancy)
                {
                    helper.Events.GameLoop.DayStarted += (x, y) => PregnancyController.CheckForBirth();
                    helper.Events.GameLoop.Saving += (x, y) => PregnancyController.UpdatePregnancy();
                }

                

                var harmony = HarmonyInstance.Create("Digus.AnimalHusbandryMod");

                try
                {
                    var farmAnimalPet = typeof(FarmAnimal).GetMethod("pet");
                    var animalQueryMenuExtendedPet = typeof(AnimalQueryMenuExtended).GetMethod("Pet");
                    harmony.Patch(farmAnimalPet, new HarmonyMethod(animalQueryMenuExtendedPet), null);
                }
                catch (Exception)
                {
                    Monitor.Log("Erro patching the FarmAnimal 'pet' Method. Applying old method of opening the extended animal query menu.", LogLevel.Warn);
                    helper.Events.Display.MenuChanged += (s, e) =>
                    {
                        if (e.NewMenu is AnimalQueryMenu && !(e.NewMenu is AnimalQueryMenuExtended))
                        {
                            Game1.activeClickableMenu = new AnimalQueryMenuExtended(this.Helper.Reflection.GetField<FarmAnimal>(e.NewMenu, "animal").GetValue());
                        }
                    };
                }

                if (!DataLoader.ModConfig.DisableRancherMeatPriceAjust)
                {
                    var sellToStorePrice = typeof(StardewValley.Object).GetMethod("sellToStorePrice");
                    var sellToStorePricePrefix = typeof(MeatOverrides).GetMethod("sellToStorePrice");
                    harmony.Patch(sellToStorePrice, new HarmonyMethod(sellToStorePricePrefix), null);
                }

                if (!DataLoader.ModConfig.DisableMeat)
                {
                    var objectIsPotentialBasicShippedCategory = typeof(StardewValley.Object).GetMethod("isPotentialBasicShippedCategory");
                    var meatOverridesIsPotentialBasicShippedCategory = typeof(MeatOverrides).GetMethod("isPotentialBasicShippedCategory");
                    harmony.Patch(objectIsPotentialBasicShippedCategory, new HarmonyMethod(meatOverridesIsPotentialBasicShippedCategory), null);

                    var objectCountsForShippedCollection = typeof(StardewValley.Object).GetMethod("countsForShippedCollection");
                    var meatOverridesCountsForShippedCollection = typeof(MeatOverrides).GetMethod("countsForShippedCollection");
                    harmony.Patch(objectCountsForShippedCollection, new HarmonyMethod(meatOverridesCountsForShippedCollection), null);
                    
                }

                

                //var addSpecificTemporarySprite = typeof(Event).GetMethod("addSpecificTemporarySprite");
                //var addSpecificTemporarySprite = this.Helper.Reflection.GetMethod(new Event(), "addSpecificTemporarySprite").MethodInfo;
                //var addSpecificTemporarySpritePostfix = typeof(EventsOverrides).GetMethod("addSpecificTemporarySprite");
                //harmony.Patch(addSpecificTemporarySprite, null, new HarmonyMethod(addSpecificTemporarySpritePostfix));

                //var petCheckAction = typeof(Pet).GetMethod("checkAction");
                //var petCheckActionPrefix = typeof(PetOverrides).GetMethod("checkAction");
                //harmony.Patch(petCheckAction, new HarmonyMethod(petCheckActionPrefix), null);
            }
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady) // save is loaded
            {
                if (e.Button == _meatCleaverSpawnKey)
                {
                    Game1.player.addItemToInventory(new MeatCleaver());
                }
                if (e.Button == _inseminationSyringeSpawnKey)
                {
                    Game1.player.addItemToInventory(new InseminationSyringe());
                }
                if (e.Button == _feedingBasketSpawnKey)
                {
                    Game1.player.addItemToInventory(new FeedingBasket());
                }
            }
        }
    }
}
