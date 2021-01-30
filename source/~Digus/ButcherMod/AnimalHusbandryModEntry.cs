/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.farmer;
using AnimalHusbandryMod.meats;
using AnimalHusbandryMod.recipes;
using AnimalHusbandryMod.tools;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace AnimalHusbandryMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class AnimalHusbandryModEntry : Mod
    {
        internal static IModHelper ModHelper;
        internal static IMonitor monitor;
        internal static DataLoader DataLoader;
        private bool _isEnabled = true;


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
            helper.Events.GameLoop.Saving += OnSaving;
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
                _isEnabled = false;
            }
            else
            {
                DataLoader = new DataLoader(Helper);
                try
                {
                    DataLoader.LoadContentPacksCommand();
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Error while trying to load the content packs. Your custom animals might not work as intended.\n{ex}",LogLevel.Error);
                }

                if (!DataLoader.ModConfig.DisableMeat)
                {
                    ModHelper.ConsoleCommands.Add("player_addallmeatrecipes", "Add all meat recipes to the player.", DataLoader.RecipeLoader.AddAllMeatRecipes);
                    if (DataLoader.ModConfig.Softmode)
                    {
                        ModHelper.ConsoleCommands.Add("player_addmeatwand", "Add Meat Wand to inventory.", (n, d) => Game1.player.addItemToInventory(ToolsFactory.GetMeatCleaver()));
                    }
                    else
                    {
                        ModHelper.ConsoleCommands.Add("player_addmeatcleaver", "Add Meat Cleaver to inventory.", (n, d) => Game1.player.addItemToInventory(ToolsFactory.GetMeatCleaver()));
                    }
                }

                if (!DataLoader.ModConfig.DisablePregnancy)
                {
                    ModHelper.ConsoleCommands.Add("player_addinseminationsyringe", "Add Insemination Syringe to inventory.", (n, d) => Game1.player.addItemToInventory(ToolsFactory.GetInseminationSyringe()));
                }

                if (!DataLoader.ModConfig.DisableTreats)
                {
                    ModHelper.ConsoleCommands.Add("player_addfeedingbasket", "Add Feeding Basket to inventory.", (n, d) => Game1.player.addItemToInventory(ToolsFactory.GetFeedingBasket()));
                }

                if (!DataLoader.ModConfig.DisableAnimalContest)
                {
                    ModHelper.ConsoleCommands.Add("player_addparticipantribbon", "Add Participant Ribbon to inventory.", (n, d) => Game1.player.addItemToInventory(ToolsFactory.GetParticipantRibbon()));
                }

                ModHelper.ConsoleCommands.Add("config_create_customanimaltemplates", "Add custom animal templates in the data\\animal.json file for every loaded custom animal.", DataLoader.AddCustomAnimalsTemplateCommand);
                ModHelper.ConsoleCommands.Add("config_reload_contentpacks_animalhusbandrymod", "Reload all content packs for animal husbandry mod.",DataLoader.LoadContentPacksCommand);
                ModHelper.ConsoleCommands.Add("world_removealltools_animalhusbandrymod", "Remove all custom tools added by the animal husbandry mod.",DataLoader.ToolsLoader.RemoveAllToolsCommand);

                if (DataLoader.ModConfig.AddMeatCleaverToInventoryKey != null || DataLoader.ModConfig.AddInseminationSyringeToInventoryKey != null || DataLoader.ModConfig.AddFeedingBasketToInventoryKey != null)
                {
                    Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
                }

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
                    harmony.Patch(
                        original: AccessTools.Method(typeof(TrashBear), "updateItemWanted"),
                        prefix: new HarmonyMethod(typeof(TrashBearOverrides), nameof(TrashBearOverrides.updateItemWanted))
                    );

                    harmony.Patch(
                        original: AccessTools.Method(typeof(Axe), nameof(Axe.getOne)),
                        prefix: new HarmonyMethod(typeof(MeatCleaverOverrides), nameof(MeatCleaverOverrides.getOne))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(Axe), "loadDisplayName"),
                        postfix: new HarmonyMethod(typeof(MeatCleaverOverrides), nameof(MeatCleaverOverrides.loadDisplayName))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(Axe), "loadDescription"),
                        postfix: new HarmonyMethod(typeof(MeatCleaverOverrides), nameof(MeatCleaverOverrides.loadDescription))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(Tool), nameof(Tool.canBeTrashed)),
                        postfix: new HarmonyMethod(typeof(MeatCleaverOverrides), nameof(MeatCleaverOverrides.canBeTrashed))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(Axe), nameof(Axe.beginUsing)),
                        prefix: new HarmonyMethod(typeof(MeatCleaverOverrides), nameof(MeatCleaverOverrides.beginUsing))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(Axe), nameof(Axe.DoFunction)),
                        prefix: new HarmonyMethod(typeof(MeatCleaverOverrides), nameof(MeatCleaverOverrides.DoFunction))
                    );
                }

                if (!DataLoader.ModConfig.DisablePregnancy)
                {
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.getOne)),
                        prefix: new HarmonyMethod(typeof(InseminationSyringeOverrides), nameof(InseminationSyringeOverrides.getOne))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), "loadDisplayName"),
                        postfix: new HarmonyMethod(typeof(InseminationSyringeOverrides), nameof(InseminationSyringeOverrides.loadDisplayName))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), "loadDescription"),
                        postfix: new HarmonyMethod(typeof(InseminationSyringeOverrides), nameof(InseminationSyringeOverrides.loadDescription))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(Tool), nameof(Tool.canBeTrashed)),
                        postfix: new HarmonyMethod(typeof(InseminationSyringeOverrides), nameof(InseminationSyringeOverrides.canBeTrashed))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.beginUsing)),
                        prefix: new HarmonyMethod(typeof(InseminationSyringeOverrides), nameof(InseminationSyringeOverrides.beginUsing))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.DoFunction)),
                        prefix: new HarmonyMethod(typeof(InseminationSyringeOverrides), nameof(InseminationSyringeOverrides.DoFunction))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.canThisBeAttached)),
                        prefix: new HarmonyMethod(typeof(InseminationSyringeOverrides), nameof(InseminationSyringeOverrides.canThisBeAttached))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.attach)),
                        prefix: new HarmonyMethod(typeof(InseminationSyringeOverrides), nameof(InseminationSyringeOverrides.attach))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.drawAttachments)),
                        prefix: new HarmonyMethod(typeof(InseminationSyringeOverrides), nameof(InseminationSyringeOverrides.drawAttachments))
                    );
                }

                if (!DataLoader.ModConfig.DisableTreats)
                {
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.getOne)),
                        prefix: new HarmonyMethod(typeof(FeedingBasketOverrides), nameof(FeedingBasketOverrides.getOne))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), "loadDisplayName"),
                        postfix: new HarmonyMethod(typeof(FeedingBasketOverrides), nameof(FeedingBasketOverrides.loadDisplayName))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), "loadDescription"),
                        postfix: new HarmonyMethod(typeof(FeedingBasketOverrides), nameof(FeedingBasketOverrides.loadDescription))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(Tool), nameof(Tool.canBeTrashed)),
                        postfix: new HarmonyMethod(typeof(FeedingBasketOverrides), nameof(FeedingBasketOverrides.canBeTrashed))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.beginUsing)),
                        prefix: new HarmonyMethod(typeof(FeedingBasketOverrides), nameof(FeedingBasketOverrides.beginUsing))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.DoFunction)),
                        prefix: new HarmonyMethod(typeof(FeedingBasketOverrides), nameof(FeedingBasketOverrides.DoFunction))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.canThisBeAttached)),
                        prefix: new HarmonyMethod(typeof(FeedingBasketOverrides), nameof(FeedingBasketOverrides.canThisBeAttached))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.attach)),
                        prefix: new HarmonyMethod(typeof(FeedingBasketOverrides), nameof(FeedingBasketOverrides.attach))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.drawAttachments)),
                        prefix: new HarmonyMethod(typeof(FeedingBasketOverrides), nameof(FeedingBasketOverrides.drawAttachments))
                    );
                }

                harmony.Patch(
                    original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate)),
                    postfix: new HarmonyMethod(typeof(FarmAnimalOverrides), nameof(FarmAnimalOverrides.dayUpdate)),
                    transpiler: new HarmonyMethod(typeof(FarmAnimalOverrides), nameof(FarmAnimalOverrides.dayUpdate_Transpiler))
                );

                if (!DataLoader.ModConfig.DisableAnimalContest)
                {
                    harmony.Patch(
                        original: AccessTools.Method(typeof(Game1), nameof(Game1.shouldTimePass)),
                        postfix: new HarmonyMethod(typeof(EventsOverrides), nameof(EventsOverrides.shouldTimePass))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(Event), "addSpecificTemporarySprite"),
                        postfix: new HarmonyMethod(typeof(EventsOverrides),  nameof(EventsOverrides.addSpecificTemporarySprite))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(Event), nameof(Event.skipEvent)),
                        postfix: new HarmonyMethod(typeof(EventsOverrides), nameof(EventsOverrides.skipEvent))
                    );

                    harmony.Patch(
                        original: AccessTools.Method(typeof(Pet), nameof(Pet.checkAction)),
                        prefix: new HarmonyMethod(typeof(PetOverrides), nameof(PetOverrides.checkAction))
                    );

                    harmony.Patch(
                        original: AccessTools.Method(typeof(Pet), nameof(Pet.update), new[] { typeof(GameTime), typeof(GameLocation) }),
                        prefix: new HarmonyMethod(typeof(PetOverrides), nameof(PetOverrides.update))
                    );

                    harmony.Patch(
                        original: AccessTools.Method(typeof(Pet), nameof(Pet.draw), new[] { typeof(SpriteBatch) }),
                        prefix: new HarmonyMethod(typeof(PetOverrides), nameof(PetOverrides.draw))
                    );

                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.getOne)),
                        prefix: new HarmonyMethod(typeof(ParticipantRibbonOverrides), nameof(ParticipantRibbonOverrides.getOne))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), "loadDisplayName"),
                        postfix: new HarmonyMethod(typeof(ParticipantRibbonOverrides), nameof(ParticipantRibbonOverrides.loadDisplayName))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), "loadDescription"),
                        postfix: new HarmonyMethod(typeof(ParticipantRibbonOverrides), nameof(ParticipantRibbonOverrides.loadDescription))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(Tool), nameof(Tool.canBeTrashed)),
                        postfix: new HarmonyMethod(typeof(ParticipantRibbonOverrides), nameof(ParticipantRibbonOverrides.canBeTrashed))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.beginUsing)),
                        prefix: new HarmonyMethod(typeof(ParticipantRibbonOverrides), nameof(ParticipantRibbonOverrides.beginUsing))
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(MilkPail), nameof(MilkPail.DoFunction)),
                        prefix: new HarmonyMethod(typeof(ParticipantRibbonOverrides), nameof(ParticipantRibbonOverrides.DoFunction))
                    );
                }
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!_isEnabled)
                return;

            DataLoader.ToolsLoader.ReplaceOldTools();
            FarmerLoader.LoadData();
            FarmerLoader.MoveOldPregnancyData();
            FarmerLoader.MoveOldAnimalStatusData();
            DataLoader.ToolsLoader.LoadMail();
            DataLoader.AnimalData.FillLikedTreatsIds();
            EventsLoader.EventListener();
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!_isEnabled)
                return;
            if (!DataLoader.ModConfig.DisableAnimalContest)
            {
                EventsLoader.CheckEventDay();
            }

            DataLoader.LivingWithTheAnimalsChannel.CheckChannelDay();
            if (!DataLoader.ModConfig.DisableMeat)
            {
                DataLoader.RecipeLoader.MeatFridayChannel.CheckChannelDay();
            }
            if (!DataLoader.ModConfig.DisablePregnancy)
            {
                PregnancyController.CheckForBirth();
            }
        }

        /// <summary>Raised before the game is saved.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (!_isEnabled)
                return;

            if (Context.IsMainPlayer)
            {
                ItemUtility.RemoveModdedItemAnywhere(ParticipantRibbonOverrides.ParticipantRibbonKey);
            }

            EventsLoader.CheckUnseenEvents();

            if (!DataLoader.ModConfig.DisablePregnancy && Context.IsMainPlayer)
            {
                PregnancyController.UpdatePregnancy();
            }
            FarmerLoader.SaveData();
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!_isEnabled || !Context.IsWorldReady)
                return;

            if (e.Button == DataLoader.ModConfig.AddMeatCleaverToInventoryKey)
                Game1.player.addItemToInventory(ToolsFactory.GetMeatCleaver());

            if (e.Button == DataLoader.ModConfig.AddInseminationSyringeToInventoryKey)
                Game1.player.addItemToInventory(ToolsFactory.GetInseminationSyringe());
                
            if (e.Button == DataLoader.ModConfig.AddFeedingBasketToInventoryKey)
                Game1.player.addItemToInventory(ToolsFactory.GetFeedingBasket());
        }
    }
}
