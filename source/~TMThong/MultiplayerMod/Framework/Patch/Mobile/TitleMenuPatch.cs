/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Monsters;
using MultiplayerMod.Framework.Mobile.Menus;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    public class TitleMenuPatch : IPatch
    {
        private readonly Type PATCH_TYPE = typeof(TitleMenu);
        public void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(PATCH_TYPE, "setUpIcons", new Type[0], null), null, new HarmonyMethod(AccessTools.Method(base.GetType(), "Postfix_setUpIcons", null, null)), null);
            //harmony.Patch(AccessTools.Method(PATCH_TYPE, "ForceSubmenu"), postfix: new HarmonyMethod(GetType(), nameof(postfix_ForceSubmenu)));
            harmony.Patch(AccessTools.Method(PATCH_TYPE, "update", new Type[]
            {
                typeof(GameTime)
            }, null), null, new HarmonyMethod(AccessTools.Method(base.GetType(), "Postfix_update", null, null)), null);
            harmony.Patch(AccessTools.Method(PATCH_TYPE, "receiveLeftClick", new Type[]
            {
                typeof(int),
                typeof(int),
                typeof(bool)
            }, null), new HarmonyMethod(AccessTools.Method(base.GetType(), "Postfix_receiveLeftClick", null, null)), null, null);

            harmony.Patch(AccessTools.Method(PATCH_TYPE, "CloseSubMenu"), prefix: new HarmonyMethod(this.GetType(), nameof(prefix_CloseSubMenu)));
        }

        private static void postfix_ForceSubmenu(TitleMenu __instance, IClickableMenu menu)
        {
            ModUtilities.Helper.Reflection.GetField<int>(__instance, "buttonsToShow").SetValue(4);
        }
        private static void Postfix_setUpIcons(TitleMenu __instance)
        {
            __instance.buttons.Clear();
            Texture2D value = ModUtilities.Helper.Reflection.GetField<Texture2D>(__instance, "titleButtonsTexture", true).GetValue();
            int num = 74;
            int num2 = num * 4 * 3;
            num2 += 72;
            int num3 = __instance.width / 2 - num2 / 2;
            __instance.buttons.Add(new ClickableTextureComponent("New", new Rectangle(num3, __instance.height - 174 - 24, num * 3, 174), null, "", value, new Rectangle(0, 187, 74, 58), 3f, false)
            {
                myID = 81115,
                rightNeighborID = 81116,
                upNeighborID = 81111
            });
            num3 += (num + 8) * 3;
            __instance.buttons.Add(new ClickableTextureComponent("Load", new Rectangle(num3, __instance.height - 174 - 24, 222, 174), null, "", value, new Rectangle(74, 187, 74, 58), 3f, false)
            {
                myID = 81116,
                leftNeighborID = 81115,
                rightNeighborID = -7777,
                upNeighborID = 81111
            });
            num3 += (num + 8) * 3;
            __instance.buttons.Add(new ClickableTextureComponent("Co-op", new Rectangle(num3, __instance.height - 174 - 24, 222, 174), null, "", value, new Rectangle(148, 187, 74, 58), 3f, false)
            {
                myID = 81119,
                leftNeighborID = 81116,
                rightNeighborID = 81117
            });
            num3 += (num + 8) * 3;
            __instance.buttons.Add(new ClickableTextureComponent("Exit", new Rectangle(num3, __instance.height - 174 - 24, 222, 174), null, "", value, new Rectangle(222, 187, 74, 58), 3f, false)
            {
                myID = 81117,
                leftNeighborID = 81119,
                rightNeighborID = 81118,
                upNeighborID = 81111
            });
        }
        private static bool setUpIcons_prefix(TitleMenu __instance)
        {
            IModHelper helper = ModUtilities.Helper;
            __instance.buttons.Clear();
            int buttonWidth = 74;
            int mainButtonSetWidth = buttonWidth * 4 * 3;
            mainButtonSetWidth += 72;
            int curx = __instance.width / 2 - mainButtonSetWidth / 2;
            __instance.buttons.Add(new ClickableTextureComponent("New", new Rectangle(curx, __instance.height - 174 - 24, buttonWidth * 3, 174), null, "", __instance.titleButtonsTexture, new Rectangle(0, 187, 74, 58), 3f, false)
            {
                myID = 81115,
                rightNeighborID = 81116,
                upNeighborID = 81111
            });
            curx += (buttonWidth + 8) * 3;
            __instance.buttons.Add(new ClickableTextureComponent("Load", new Rectangle(curx, __instance.height - 174 - 24, 222, 174), null, "", __instance.titleButtonsTexture, new Rectangle(74, 187, 74, 58), 3f, false)
            {
                myID = 81116,
                leftNeighborID = 81115,
                rightNeighborID = -7777,
                upNeighborID = 81111
            });
            curx += (buttonWidth + 8) * 3;
            __instance.buttons.Add(new ClickableTextureComponent("Co-op", new Rectangle(curx, __instance.height - 174 - 24, 222, 174), null, "", __instance.titleButtonsTexture, new Rectangle(148, 187, 74, 58), 3f, false)
            {
                myID = 81119,
                leftNeighborID = 81116,
                rightNeighborID = 81117
            });
            curx += (buttonWidth + 8) * 3;
            __instance.buttons.Add(new ClickableTextureComponent("Exit", new Rectangle(curx, __instance.height - 174 - 24, 222, 174), null, "", __instance.titleButtonsTexture, new Rectangle(222, 187, 74, 58), 3f, false)
            {
                myID = 81117,
                leftNeighborID = 81119,
                rightNeighborID = 81118,
                upNeighborID = 81111
            });
            float viewportY = helper.Reflection.GetField<float>(__instance, "viewportY").GetValue();
            int zoom = __instance.ShouldShrinkLogo() ? 2 : 3;

            helper.Reflection.GetField<Rectangle>(__instance, "eRect").SetValue(new Rectangle(__instance.width / 2 - 200 * zoom + 251 * zoom, -300 * zoom - (int)(viewportY / 3f) * zoom + 26 * zoom, 42 * zoom, 68 * zoom));
            helper.Reflection.GetField<Rectangle>(__instance, "screwRect").SetValue(new Rectangle(__instance.width / 2 + 150 * zoom, -300 * zoom - (int)(viewportY / 3f) * zoom + 80 * zoom, 5 * zoom, 5 * zoom));
            helper.Reflection.GetField<Rectangle>(__instance, "cornerRect").SetValue(new Rectangle(__instance.width / 2 - 200 * zoom, -300 * zoom - (int)(viewportY / 3f) * zoom + 165 * zoom, 20 * zoom, 20 * zoom));
            helper.Reflection.GetField<Rectangle>(__instance, "r_hole_rect").SetValue(new Rectangle(__instance.width / 2 - 21 * zoom, -300 * zoom - (int)(viewportY / 3f) * zoom + 39 * zoom, 10 * zoom, 11 * zoom));
            helper.Reflection.GetField<Rectangle>(__instance, "r_hole_rect2").SetValue(new Rectangle(__instance.width / 2 - 35 * zoom, -300 * zoom - (int)(viewportY / 3f) * zoom + 24 * zoom, 7 * zoom, 7 * zoom));
            __instance.populateLeafRects();
            __instance.backButton = new ClickableTextureComponent(__instance.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11739"), new Rectangle(__instance.width + -198 - 48, __instance.height - 81 - 24, 198, 81), null, "", __instance.titleButtonsTexture, new Rectangle(296, 252, 66, 27), 3f, false)
            {
                myID = 81114
            };
            __instance.aboutButton = new ClickableTextureComponent(__instance.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11740"), new Rectangle(__instance.width + -66 - 48, __instance.height - 75 - 24, 66, 75), null, "", __instance.titleButtonsTexture, new Rectangle(8, 458, 22, 25), 3f, false)
            {
                myID = 81113,
                upNeighborID = 81118,
                leftNeighborID = -7777
            };
            __instance.languageButton = new ClickableTextureComponent(__instance.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11740"), new Rectangle(__instance.width + -66 - 48, __instance.height - 150 - 48, 81, 75), null, "", __instance.titleButtonsTexture, new Rectangle(52, 458, 27, 25), 3f, false)
            {
                myID = 81118,
                downNeighborID = 81113,
                leftNeighborID = -7777,
                upNeighborID = 81112
            };
            __instance.skipButton = new ClickableComponent(new Rectangle(__instance.width / 2 - 261, __instance.height / 2 - 102, 249, 201), __instance.menuContent.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11741"));
            int globalXOffset = helper.Reflection.GetField<int>(__instance, "globalXOffset").GetValue();
            if (globalXOffset > __instance.width)
            {
                helper.Reflection.GetField<int>(__instance, "globalXOffset").SetValue(__instance.width);
            }
            globalXOffset = helper.Reflection.GetField<int>(__instance, "globalXOffset").GetValue();
            foreach (ClickableTextureComponent clickableTextureComponent in __instance.buttons)
            {
                clickableTextureComponent.bounds.X = clickableTextureComponent.bounds.X + globalXOffset;
            }
            if (Game1.options.gamepadControls && Game1.options.snappyMenus)
            {
                __instance.populateClickableComponentList();
                __instance.snapToDefaultClickableComponent();
            }
            return true;
        }
        private static void Postfix_update(GameTime time, TitleMenu __instance)
        {
            int value = ModUtilities.Helper.Reflection.GetField<int>(__instance, "buttonsToShow", true).GetValue();
            int globalXOffset = ModUtilities.Helper.Reflection.GetField<int>(__instance, "globalXOffset").GetValue();
            string whichSubMenu = ModUtilities.Helper.Reflection.GetField<string>(__instance, "whichSubMenu").GetValue();
            bool isTransitioningButtons = ModUtilities.Helper.Reflection.GetField<bool>(__instance, "isTransitioningButtons").GetValue();
            bool? hasRoomAnotherFarm = ModUtilities.Helper.Reflection.GetField<bool?>(__instance, "hasRoomAnotherFarm").GetValue();
            if (!(value >= 4))
            {
                TitleMenuPatch.showButtonsTimer -= time.TotalGameTime.Milliseconds;
                if (TitleMenuPatch.showButtonsTimer <= 0 && value >= 1 && value < 4)
                {
                    ModUtilities.Helper.Reflection.GetField<int>(__instance, "buttonsToShow", true).SetValue(value + 1);
                    Game1.playSound("Cowboy_gunshot");
                }
            }

            int buttonsDX = ModUtilities.Helper.Reflection.GetField<int>(__instance, "buttonsDX").GetValue();
            if (buttonsDX > 0 && globalXOffset >= __instance.width)
            {
                if (whichSubMenu.Equals("Co-op"))
                {
                    if (hasRoomAnotherFarm != null)
                    {
                        TitleMenu.subMenu = new MultiplayerMod.Framework.Mobile.Menus.SCoopMenuMobile();
                        Game1.changeMusicTrack("title_night", false, Game1.MusicContext.Default);
                        ModUtilities.Helper.Reflection.GetField<bool>(__instance, "isTransitioningButtons").SetValue(false);
                        ModUtilities.Helper.Reflection.GetField<int>(__instance, "buttonsDX").SetValue(0);
                    }
                }
                if (!isTransitioningButtons)
                {
                    ModUtilities.Helper.Reflection.GetField<string>(__instance, "whichSubMenu").SetValue("");
                }
            }
        }



        private static void Postfix_receiveLeftClick(TitleMenu __instance, int x, int y)
        {
            foreach (ClickableTextureComponent clickableTextureComponent in __instance.buttons)
            {
                if (clickableTextureComponent.name == "Exit" && clickableTextureComponent.bounds.Contains(x, y))
                {

                }
            }
        }
        private static int showButtonsTimer = 333;


        private static bool prefix_CloseSubMenu(TitleMenu __instance)
        {
            if (TitleMenu.subMenu.readyToClose())
            {
                if(TitleMenu.subMenu is SFarmhandMenu)
                {
                    TitleMenu.subMenu = new MultiplayerMod.Framework.Mobile.Menus.SCoopMenuMobile();
                    Game1.changeMusicTrack("title_night", false, Game1.MusicContext.Default);
                    return true;
                }

                IReflectedField<int> buttonsDX = ModUtilities.Helper.Reflection.GetField<int>(__instance, "buttonsDX");
                buttonsDX.SetValue(-1);
                if (TitleMenu.subMenu is AboutMenu || TitleMenu.subMenu is LanguageSelectionMenu)
                {
                    TitleMenu.subMenu = null;
                    buttonsDX.SetValue(0);
                    return true;
                }
                IReflectedField<bool> isTransitioningButtons = ModUtilities.Helper.Reflection.GetField<bool>(__instance, "isTransitioningButtons");
                isTransitioningButtons.SetValue(true);
                if (TitleMenu.subMenu is LoadGameMenu || TitleMenu.subMenu is SCoopGameMenu || TitleMenu.subMenu is SCoopMenuMobile || TitleMenu.subMenu is SLoadGameMenu || TitleMenu.subMenu is SFarmhandMenu)
                {
                    IReflectedField<bool> transitioningFromLoadScreen = ModUtilities.Helper.Reflection.GetField<bool>(__instance, "transitioningFromLoadScreen");
                    transitioningFromLoadScreen.SetValue(true);
                }
                TitleMenu.subMenu = null;
                Game1.changeMusicTrack("spring_day_ambient", false, Game1.MusicContext.Default);
            }
            return true;
        }
    }
}
