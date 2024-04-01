/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/custom-farm-loader
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
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private static Texture2D CustomFarmIcon;
        private static ClickableTextureComponent CustomFarmButton = null;
        private static CustomFarm CurrentCustomFarm = null;
        public static void Initialize(Mod mod)
        {
            Monitor = mod.Monitor;
            Helper = mod.Helper;
            CustomFarmIcon = Helper.ModContent.Load<Texture2D>("assets/CustomFarmIcon.png");

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(CharacterCustomization), "RefreshFarmTypeButtons"),
               postfix: new HarmonyMethod(typeof(_CharacterCustomizationMenu), nameof(_CharacterCustomizationMenu.refreshFarmTypeButtons_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), new[] { typeof(SpriteBatch) }),
               prefix: new HarmonyMethod(typeof(_CharacterCustomizationMenu), nameof(_CharacterCustomizationMenu.draw_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), new[] { typeof(SpriteBatch) }),
               postfix: new HarmonyMethod(typeof(_CharacterCustomizationMenu), nameof(_CharacterCustomizationMenu.draw_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(CharacterCustomization), "optionButtonClick"),
               prefix: new HarmonyMethod(typeof(_CharacterCustomizationMenu), nameof(_CharacterCustomizationMenu.optionButtonClick_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(CharacterCustomization), "optionButtonClick"),
               postfix: new HarmonyMethod(typeof(_CharacterCustomizationMenu), nameof(_CharacterCustomizationMenu.optionButtonClick_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(CharacterCustomization), "LoadFarmTypeData"),
               postfix: new HarmonyMethod(typeof(_CharacterCustomizationMenu), nameof(_CharacterCustomizationMenu.loadFarmTypeData_Postfix))
            );

            harmony.Patch(
               original: AccessTools.DeclaredMethod(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), new[] { typeof(SpriteBatch) }),
               transpiler: new HarmonyMethod(typeof(_CharacterCustomizationMenu), nameof(draw_Transpiler))
            );

        }

        public static IEnumerable<CodeInstruction> draw_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //Reduce the height of the farmTypeButtons background drawTextureBox
            foreach (var instruction in instructions) {
                if (instruction.LoadsConstant(564))
                    instruction.operand = 389;

                yield return instruction;
            }
        }

        public static void loadFarmTypeData_Postfix(CharacterCustomization __instance, ref int ____farmPages)
        {
            //Disables the native UI since we're going to completely replace that logic
            ____farmPages = 1;
        }

        public static void refreshFarmTypeButtons_Postfix(CharacterCustomization __instance)
        {
            Point baseFarmButton = new Point(__instance.xPositionOnScreen + __instance.width + 4 + 8, __instance.yPositionOnScreen + IClickableMenu.borderWidth);

            int x = baseFarmButton.X + 58;
            int y = baseFarmButton.Y + 389 + 90;

            y = __instance.backButton.bounds.Y - y < 48 ? y - 48 : y;

            if(Helper.ModRegistry.IsLoaded("aedenthorn.StardewRPG")) {
                x = baseFarmButton.X + 8;
                y = baseFarmButton.Y - 12;
            }

            //TODO: Remove Custom _ workaround
            CustomFarmButton = new ClickableTextureComponent("CustomFarm", new Rectangle(x, y, 88, 80), null, "Custom_", CustomFarmIcon, new Rectangle(0, 0, 18, 20), 4f) {
                myID = 549,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            updateCustomFarmButton();
            rearrangeFarmtypeButtons(__instance);
            __instance.farmTypeButtons.Add(CustomFarmButton);

            //Updates sticky elements for gamepad
            __instance.populateClickableComponentList();
        }

        private static void rearrangeFarmtypeButtons(CharacterCustomization menu)
        {
            var buttons = menu.farmTypeButtons;
            menu.farmTypeButtons = buttons.Take(8).ToList();

            buttons[4].bounds.X = buttons[6].bounds.X;
            buttons[4].bounds.Y = buttons[6].bounds.Y;

            buttons[5].bounds.X = buttons[7].bounds.X;
            buttons[5].bounds.Y = buttons[7].bounds.Y;

            menu.farmTypeButtons[6].bounds.Y += 176;
            menu.farmTypeButtons[7].bounds.Y += 176;

            buttons[0].rightNeighborID = buttons[4].myID;
            buttons[1].rightNeighborID = buttons[5].myID;
            buttons[2].rightNeighborID = buttons[6].myID;
            buttons[3].rightNeighborID = buttons[7].myID;

            buttons[4].leftNeighborID = buttons[0].myID;
            buttons[5].leftNeighborID = buttons[1].myID;
            buttons[6].leftNeighborID = buttons[2].myID;
            buttons[7].leftNeighborID = buttons[3].myID;

            //buttons[0].downNeighborID = buttons[1].myID;
            //buttons[1].downNeighborID = buttons[2].myID;
            //buttons[2].downNeighborID = buttons[3].myID;
            buttons[3].downNeighborID = 549;

            //buttons[4].downNeighborID = buttons[5].myID;
            //buttons[5].downNeighborID = buttons[6].myID;
            //buttons[6].downNeighborID = buttons[7].myID;
            buttons[7].downNeighborID = 549;
        }

        public static bool optionButtonClick_Prefix(CharacterCustomization __instance, string name)
        {
            try {
                if (name != "CustomFarm")
                    return true;


                if (__instance.source != CharacterCustomization.Source.NewGame && __instance.source != CharacterCustomization.Source.HostNewFarm)
                    return false;

                Game1.playSound("drumkit6");

                __instance.AddDependency();
                IClickableMenu customFarmSelection = new CustomFarmSelection();
                __instance.SetChildMenu(customFarmSelection);
                (TitleMenu.subMenu = customFarmSelection).exitFunction = delegate {
                    CurrentCustomFarm = (TitleMenu.subMenu as CustomFarmSelection).CurrentCustomFarm;
                    updateCustomFarmButton();

                    TitleMenu.subMenu = __instance;
                    __instance.RemoveDependency();
                    __instance.populateClickableComponentList();
                    if (Game1.options.SnappyMenus) {
                        __instance.setCurrentlySnappedComponentTo(636);
                        __instance.snapCursorToCurrentSnappedComponent();
                    }
                };

                return false;
            } catch (Exception ex) {
                Monitor.Log($"Failed in {nameof(optionButtonClick_Prefix)}:\n{ex}", LogLevel.Error);
                return false;
            }

        }

        public static void optionButtonClick_Postfix(string name)
        {
            updateCustomFarmButton();
        }

            public static bool draw_Prefix(CharacterCustomization __instance, List<ClickableComponent> ___leftSelectionButtons, string ___hoverText, SpriteBatch b)
        {
            if (__instance.source != CharacterCustomization.Source.NewGame && __instance.source != CharacterCustomization.Source.HostNewFarm)
                return true;

            try {
                __instance.farmTypeButtons.Remove(CustomFarmButton);
                CustomFarmButton.draw(b, Color.White, 0.88f);

                //Red farm selection rectangle
                if (!isVanillaFarmSelected() && __instance.farmTypeButtons.Count > 0 && CustomFarmButton != null)
                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), CustomFarmButton.bounds.X - 8, CustomFarmButton.bounds.Y - 4, CustomFarmButton.bounds.Width, CustomFarmButton.bounds.Height + 8, Color.White, 4f, drawShadow: false);

                //1.6 hover logic is a bit weird, so I decided to draw it for this button myself.
                if (CustomFarmButton.containsPoint(Game1.getMouseX(), Game1.getMouseY())) {
                    ___hoverText = null;
                    string text;

                    if (CurrentCustomFarm is not null && !isVanillaFarmSelected())
                        text = CurrentCustomFarm.Name.Replace("_", " ");
                    else
                        text = "Custom";

                    int width = Math.Max((int)Game1.dialogueFont.MeasureString(text).X, 256);
                    IClickableMenu.drawHoverText(b, Game1.parseText(text, Game1.smallFont, width), Game1.smallFont, 0, 0, -1);
                }


            } catch (Exception ex) {
                Monitor.Log($"Failed in {nameof(draw_Prefix)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }

        public static void draw_Postfix(CharacterCustomization __instance, List<ClickableComponent> ___leftSelectionButtons, string ___hoverText, SpriteBatch b)
        {
            if (__instance.source == CharacterCustomization.Source.NewGame || __instance.source == CharacterCustomization.Source.HostNewFarm)
                __instance.farmTypeButtons.Add(CustomFarmButton);
        }

        private static void updateCustomFarmButton()
        {
            var customFarm = CurrentCustomFarm;

            if (CustomFarmButton == null)
                return;

            if (customFarm != null && !isVanillaFarmSelected()) {
                //var description = customFarm.getLocalizedDescription().Replace("_", " ");
                //description = description.Length > 200 ? description.Substring(0, 200) + "..." : description;
                //CustomFarmButton.hoverText = $"{customFarm.Name.Replace("_", " ")}_";
                CustomFarmButton.texture = customFarm.Icon;

            } else {
                CustomFarmButton.hoverText = "Custom_";
                CustomFarmButton.texture = CustomFarmIcon;

            }
        }

        public static bool isVanillaFarmSelected()
            => Game1.whichFarm != 7 ||  Game1.GetFarmTypeID() == "MeadowlandsFarm";
 
    }
}
