using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals;
using MailFrameworkMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using Harmony;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.farmer;
using AnimalHusbandryMod.tools;
using AnimalHusbandryMod.meats;
using StardewValley.Characters;
using DataLoader = AnimalHusbandryMod.common.DataLoader;

namespace AnimalHusbandryMod
{
    public class AnimalHusbandryModEntery : Mod
    {

        internal static IModHelper ModHelper;
        internal static IMonitor monitor;
        internal static DataLoader DataLoader;
        private string _meatCleaverSpawnKey;
        private string _inseminationSyringeSpawnKey;
        private string _feedingBasketSpawnKey;

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

                SaveEvents.AfterLoad += DataLoader.ToolsLoader.ReplaceOldTools;
                SaveEvents.AfterLoad += (x, y) => FarmerLoader.LoadData();
                SaveEvents.AfterLoad += (x, y) => DataLoader.ToolsLoader.LoadMail();

                TimeEvents.AfterDayStarted += (x, y) => DataLoader.LivingWithTheAnimalsChannel.CheckChannelDay();

                //TimeEvents.AfterDayStarted += (x, y) => EventsLoader.CheckEventDay();

                if (!DataLoader.ModConfig.DisableMeat)
                {
                    TimeEvents.AfterDayStarted += (x, y) => DataLoader.RecipeLoader.MeatFridayChannel.CheckChannelDay();
                    ModHelper.ConsoleCommands.Add("player_addallmeatrecipes", "Add all meat recipes to the player.", DataLoader.RecipeLoader.AddAllMeatRecipes);
                }

                if (_meatCleaverSpawnKey != null || _inseminationSyringeSpawnKey != null || _feedingBasketSpawnKey != null)
                {
                    ControlEvents.KeyPressed += this.ControlEvents_KeyPress;
                }

                if (!DataLoader.ModConfig.DisablePregnancy)
                {
                    TimeEvents.AfterDayStarted += (x, y) => PregnancyController.CheckForBirth();
                    SaveEvents.BeforeSave += (x, y) => PregnancyController.UpdatePregnancy();
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
                    MenuEvents.MenuChanged += (s, e) =>
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
        /// <summary>The method invoked when the player presses a keyboard button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void ControlEvents_KeyPress(object sender, EventArgsKeyPressed e)
        {
            if (Context.IsWorldReady) // save is loaded
            {
                
                if (_meatCleaverSpawnKey != null && e.KeyPressed == (Keys)Enum.Parse(typeof(Keys), _meatCleaverSpawnKey.ToUpper()))
                {
                    Game1.player.addItemToInventory(new MeatCleaver());
                }
                if (_inseminationSyringeSpawnKey != null && e.KeyPressed == (Keys)Enum.Parse(typeof(Keys), _inseminationSyringeSpawnKey.ToUpper()))
                {
                    Game1.player.addItemToInventory(new InseminationSyringe());
                }
                if (_feedingBasketSpawnKey != null && e.KeyPressed == (Keys)Enum.Parse(typeof(Keys), _feedingBasketSpawnKey.ToUpper()))
                {
                    Game1.player.addItemToInventory(new FeedingBasket());
                }
            }
        }
    }
}
