/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using HarmonyLib;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Custom_Farm_Loader.Lib;

// "CharacterCustomization" is the SDV Menu for creating/customizing your character.
// When CharacterCustomization.sources is Source.NewGame or Source.HostNewFarm it becomes also the menu for selecting your farm
// In that case we append our own custom farmTypeButton which opens a CustomFarmSelectionMenu



namespace Custom_Farm_Loader.Menus
{
    public class _CharacterCustomizationMenu
    {
        public static void Initialize(Mod mod)
        {
            var harmony = new Harmony(mod.ModManifest.UniqueID);
            CCM_Patches.Initialize(mod.Monitor, mod.Helper);


            harmony.Patch(
               original: AccessTools.Method(typeof(CharacterCustomization), "RefreshFarmTypeButtons"),
               postfix: new HarmonyMethod(typeof(CCM_Patches), nameof(CCM_Patches.refreshFarmTypeButtons_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), new[] { typeof(SpriteBatch) }),
               prefix: new HarmonyMethod(typeof(CCM_Patches), nameof(CCM_Patches.draw_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), new[] { typeof(SpriteBatch) }),
               postfix: new HarmonyMethod(typeof(CCM_Patches), nameof(CCM_Patches.draw_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(CharacterCustomization), "optionButtonClick"),
               prefix: new HarmonyMethod(typeof(CCM_Patches), nameof(CCM_Patches.optionButtonClick_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(CharacterCustomization), "LoadFarmTypeData"),
               postfix: new HarmonyMethod(typeof(CCM_Patches), nameof(CCM_Patches.loadFarmTypeData_Postfix))
            );
        }

    }

    public class CCM_Patches
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private static Texture2D CustomFarmIcon;
        private static ClickableTextureComponent CustomFarmButton = null;

        public static void Initialize(IMonitor monitor, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;

            CustomFarmIcon = Helper.ModContent.Load<Texture2D>("assets/CustomFarmIcon.png");
        }

        public static void loadFarmTypeData_Postfix(CharacterCustomization __instance, ref int ____farmPages)
        {
            //Disables the native UI since we're going to completely replace that logic
            ____farmPages = 1;
        }

        public static void refreshFarmTypeButtons_Postfix(CharacterCustomization __instance)
        {
            Point baseFarmButton = new Point(__instance.xPositionOnScreen + __instance.width + 4 + 8, __instance.yPositionOnScreen + IClickableMenu.borderWidth);
            CustomFarmButton = new ClickableTextureComponent("CustomFarm", new Rectangle(baseFarmButton.X + 112, baseFarmButton.Y + 616, 88, 80), null, "Custom", CustomFarmIcon, new Rectangle(0, 0, 18, 20), 4f)
            {
                myID = 548,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            __instance.farmTypeButtons.Add(CustomFarmButton);

            //Updates sticky elements for gamepad
            __instance.populateClickableComponentList();
        }

        public static bool optionButtonClick_Prefix(CharacterCustomization __instance, string name)
        {
            try
            {
                if (name != "CustomFarm")
                    return true;


                if (__instance.source != CharacterCustomization.Source.NewGame && __instance.source != CharacterCustomization.Source.HostNewFarm)
                    return false;

                Game1.playSound("drumkit6");

                __instance.AddDependency();
                IClickableMenu customFarmSelection = new CustomFarmSelection(Game1.whichFarm);
                __instance.SetChildMenu(customFarmSelection);
                (TitleMenu.subMenu = customFarmSelection).exitFunction = delegate
                {
                    TitleMenu.subMenu = __instance;
                    __instance.RemoveDependency();
                    __instance.populateClickableComponentList();
                    if (Game1.options.SnappyMenus)
                    {
                        __instance.setCurrentlySnappedComponentTo(636);
                        __instance.snapCursorToCurrentSnappedComponent();
                    }
                };

                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(optionButtonClick_Prefix)}:\n{ex}", LogLevel.Error);
                return false;
            }

        }

        public static bool draw_Prefix(CharacterCustomization __instance, List<ClickableComponent> ___leftSelectionButtons, string ___hoverText, SpriteBatch b)
        {
            if (__instance.source != CharacterCustomization.Source.NewGame && __instance.source != CharacterCustomization.Source.HostNewFarm)
                return true;

            try
            {
                __instance.farmTypeButtons.Remove(CustomFarmButton);
                CustomFarmButton.draw(b, Color.White, 0.88f);

                if (Game1.whichFarm == 7 && __instance.farmTypeButtons.Count > 0 && CustomFarmButton != null)
                {
                    //Red farm selection rectangle
                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), CustomFarmButton.bounds.X - 8, CustomFarmButton.bounds.Y - 4, CustomFarmButton.bounds.Width, CustomFarmButton.bounds.Height + 8, Color.White, 4f, drawShadow: false);

                }

            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(draw_Prefix)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }
        public static void draw_Postfix(CharacterCustomization __instance, List<ClickableComponent> ___leftSelectionButtons, string ___hoverText, SpriteBatch b)
        {
            if (__instance.source == CharacterCustomization.Source.NewGame || __instance.source == CharacterCustomization.Source.HostNewFarm)
                __instance.farmTypeButtons.Add(CustomFarmButton);
        }
    }
}
