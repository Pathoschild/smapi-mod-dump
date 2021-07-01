/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using Harmony;
using Microsoft.Xna.Framework.Graphics;
using PelicanFiber.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace PelicanFiber
{
    public class PelicanFiber : Mod
    {
        /*********
        ** Properties
        *********/
        private SButton MenuKey = SButton.PageDown;
        private Texture2D Websites;
        private static ModConfig Config;
        private bool Unfiltered = true;
        private static ItemUtils ItemUtils;

        /// <summary>The last link opened through the Pelican Fiber menu.</summary>
        private IClickableMenu LastLinkOpened;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // load config
            var config = Config = Helper.ReadConfig<ModConfig>();
            if (!Enum.TryParse(config.KeyBind, true, out MenuKey))
            {
                MenuKey = SButton.PageDown;
                Monitor.Log($"404 Not Found: Error parsing key binding; defaulted to {MenuKey}.");
            }

            Unfiltered = !config.InternetFilter;

            // load textures
            try
            {
                Websites = helper.Content.Load<Texture2D>("assets/websites.png");
            }
            catch (Exception ex)
            {
                Monitor.Log($"400 Bad Request: Could not load image content. {ex}", LogLevel.Error);
            }

            // load utils
            ItemUtils = new ItemUtils(helper.Content, helper.Data, Monitor);

            // hook events
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.MenuChanged += OnMenuChanged;

            // hook Harmony patches
            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            harmony.Patch(
                AccessTools.Method(typeof(ShopMenu), "tryToPurchaseItem"),
                postfix: new HarmonyMethod(GetType(), nameof(After_TryPurchaseItem))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.Button == MenuKey)
                OpenMainMenu();
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (LastLinkOpened != null && e.NewMenu == null && ReferenceEquals(e.OldMenu, LastLinkOpened))
            {
                OpenMainMenu();
                LastLinkOpened = null;
            }
        }


        /// <summary>Open the main Pelican Fiber menu.</summary>
        private void OpenMainMenu()
        {
            try
            {
                var scale = 1.0f;
                if (Game1.uiViewport.Height < 1325)
                    scale = Game1.uiViewport.Height / 1325f;

                Game1.activeClickableMenu = new PelicanFiberMenu(Websites, Helper.Reflection, ItemUtils,
                    Helper.Multiplayer.GetNewID, OnLinkOpened, scale, Unfiltered);
            }
            catch (Exception ex)
            {
                Monitor.Log($"500 Internal Error: {ex}", LogLevel.Error);
            }
        }

        /// <summary>Track the last link menu opened.</summary>
        private void OnLinkOpened()
        {
            LastLinkOpened = Game1.activeClickableMenu;
        }

        /// <summary>Called by Harmony after the <c>ShopMenu.TryToPurchaseItem</c> method.</summary>
        /// <param name="__result">The return value of the original method.</param>
        /// <param name="item">The item being purchased.</param>
        /// <param name="numberToBuy">The number of items to purchase.</param>
        [SuppressMessage("ReSharper", "InconsistentNaming",
            Justification = "The argument names must match those expected by Harmony.")]
        private static void After_TryPurchaseItem(bool __result, Item item, int numberToBuy)
        {
            // if purchased
            if (__result)
            {
                var obj = item as SObject;

                // add bundle
                if (obj?.Category == -425 && item.Name.Contains("Bundle"))
                {
                    ItemUtils.AddBundle(item.SpecialVariable);
                    Game1.player.craftingRecipes.Remove(obj.name); // don't use .Name, since that includes 'Recipe'
                }

                // update achievements if item was purchased
                if (Config.GiveAchievements)
                    switch (item.Category)
                    {
                        // recipes cooked
                        case SObject.CookingCategory:
                            Game1.player.cookedRecipe(item.ParentSheetIndex);
                            Game1.stats.checkForCookingAchievements();
                            break;

                        // fish caught
                        case SObject.FishCategory:
                            Game1.player.caughtFish(item.ParentSheetIndex, 12);
                            break;

                        // minerals found
                        case SObject.GemCategory:
                        case SObject.mineralsCategory:
                            Game1.player.foundMineral(item.ParentSheetIndex);
                            break;

                        // artifacts found
                        case 0 when obj?.Type == "Arch":
                            Game1.player.foundArtifact(item.ParentSheetIndex, numberToBuy);
                            break;
                    }
            }
        }
    }
}