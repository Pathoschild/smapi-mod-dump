using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using static StardewValley.LocalizedContentManager;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class LanguageSelectionMobileRewrites
    {
        [HarmonyPatch(typeof(LanguageSelectionMobile))]
        [HarmonyPatch("SetupButtons")]
        public class SetupButtonsRewrite
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count(); i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i].operand == ModEntry.ModConfig.OriginLocaleCount)
                    {
                        if(i + 3 < codes.Count() && codes[i + 3].opcode == OpCodes.Mul)
                        {
                            codes[i].operand = (sbyte)(ModEntry.ModConfig.OriginLocaleCount + ModEntry.ModConfig.locales.Length);
                        }
                    }
                    else if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == ModEntry.ModConfig.OriginLocaleCount)
                    {
                        codes[i].operand = (float)(ModEntry.ModConfig.OriginLocaleCount + ModEntry.ModConfig.locales.Length);
                    }
                }
                return codes.AsEnumerable();
            }
        }
        [HarmonyPatch(typeof(LanguageSelectionMobile))]
        [HarmonyPatch("setCurrentItemIndex")]
        public class SetCurrentItemIndexRewrite
        {
            [HarmonyPrefix]
            public static void Prefix(LanguageSelectionMobile __instance)
            {
                Rectangle mainBox = (Rectangle)__instance.GetType().GetField("mainBox", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                int buttonHeight = (int)__instance.GetType().GetField("buttonHeight", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                for(short i = 0; i < ModEntry.ModConfig.locales.Length; i++)
                {
                    ModConfig.Locale locale = ModEntry.ModConfig.locales[i];
                    __instance.languages.Add(new ClickableComponent(
                        new Rectangle(mainBox.X + 0x10, (mainBox.Y + 0x10) + (buttonHeight * ModEntry.ModConfig.OriginLocaleCount + i), __instance.buttonWidth, buttonHeight),
                        locale.Name, null));
                    if ((int)(CurrentLanguageCode) == locale.CodeEnum)
                    {
                        __instance.GetType().GetField("languageCodeName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, locale.Name);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(LanguageSelectionMobile))]
        [HarmonyPatch("releaseLeftClick")]
        public class ReleaseLeftClickRewrite
        {
            [HarmonyPrefix]
            public static bool Prefix(LanguageSelectionMobile __instance, int x, int y)
            {
                MobileScrollbox scrollArea = (MobileScrollbox)__instance.GetType().GetField("scrollArea", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                if (scrollArea == null || !scrollArea.havePanelScrolled)
                {
                    foreach (ClickableComponent language in __instance.languages)
                    {
                        if (language.containsPoint(x, y))
                        {
                            __instance.GetType().GetField("languageChosen", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, true);
                            Game1.playSound("select");
                            switch (language.name)
                            {
                                case "English":
                                    CurrentLanguageCode = LanguageCode.en;
                                    break;
                                case "French":
                                    CurrentLanguageCode = LanguageCode.fr;
                                    break;
                                case "German":
                                    CurrentLanguageCode = LanguageCode.de;
                                    break;
                                case "Hungarian":
                                    CurrentLanguageCode = LanguageCode.hu;
                                    break;
                                case "Italian":
                                    CurrentLanguageCode = LanguageCode.it;
                                    break;
                                case "Japanese":
                                    CurrentLanguageCode = LanguageCode.ja;
                                    break;
                                case "Korean":
                                    CurrentLanguageCode = LanguageCode.ko;
                                    break;
                                case "Portuguese":
                                    CurrentLanguageCode = LanguageCode.pt;
                                    break;
                                case "Russian":
                                    CurrentLanguageCode = LanguageCode.ru;
                                    break;
                                case "Spanish":
                                    CurrentLanguageCode = LanguageCode.es;
                                    break;
                                case "Turkish":
                                    CurrentLanguageCode = LanguageCode.tr;
                                    break;
                                default:
                                    ModConfig.Locale locale = ModEntry.ModConfig.GetByName(language.name);
                                    if(locale != null)
                                    {
                                        CurrentLanguageCode = (LocalizedContentManager.LanguageCode)locale.CodeEnum;
                                        break;
                                    }
                                    else
                                    {
                                        CurrentLanguageCode = LanguageCode.en;
                                    }
                                    break;
                            }
                            __instance.exitThisMenu(true);
                        }
                    }
                }
                if (scrollArea == null)
                    return false;
                scrollArea.releaseLeftClick(x, y);
                return false;
            }
        }
        [HarmonyPatch(typeof(LanguageSelectionMobile))]
        [HarmonyPatch("draw")]
        [HarmonyPatch(new Type[] { typeof(SpriteBatch) })]
        public class DrawRewrite
        {
            [HarmonyPrefix]
            public static bool Prefix(LanguageSelectionMobile __instance, SpriteBatch b)
            {
                Utility.getTopLeftPositionForCenteringOnScreen(__instance.width, __instance.height - 100, 0, 0);
                SpriteBatch spriteBatch = b;
                Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
                Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
                Rectangle bounds = viewport.Bounds;
                Color color = Color.Multiply(Color.Black, 0.6f);
                spriteBatch.Draw(fadeToBlackRect, bounds, color);
                Rectangle mainBox = (Rectangle)__instance.GetType().GetField("mainBox", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                IClickableMenu.drawTextureBox(b, (int)mainBox.X, (int)mainBox.Y, (int)mainBox.Width, (int)mainBox.Height, Color.White);
                MobileScrollbox scrollArea = (MobileScrollbox)__instance.GetType().GetField("scrollArea", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                if (scrollArea != null)
                {
                    MobileScrollbar newScrollbar = (MobileScrollbar)__instance.GetType().GetField("newScrollbar", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                    newScrollbar.draw(b);
                    scrollArea.setUpForScrollBoxDrawing(b, 1f);
                }
                foreach (ClickableComponent language in __instance.languages)
                {
                    int num1 = -1;
                    switch (language.name)
                    {
                        case "English":
                            num1 = 0;
                            break;
                        case "Spanish":
                            num1 = 1;
                            break;
                        case "Portuguese":
                            num1 = 2;
                            break;
                        case "Russian":
                            num1 = 3;
                            break;
                        case "Chinese":
                            num1 = 4;
                            break;
                        case "Japanese":
                            num1 = 5;
                            break;
                        case "German":
                            num1 = 6;
                            break;
                        case "French":
                            num1 = 7;
                            break;
                        case "Korean":
                            num1 = 8;
                            break;
                        case "Turkish":
                            num1 = 9;
                            break;
                        case "Italian":
                            num1 = 10;
                            break;
                        case "Hungarian":
                            num1 = 11;
                            break;
                    }
                    if(num1 >= 0)
                    {
                        int num2 = (num1 <= 6 ? num1 * 78 : (num1 - 7) * 78) + (language.label != null ? 39 : 0);
                        int num3 = num1 > 6 ? 174 : 0;
                        Texture2D texture = (Texture2D)__instance.GetType().GetField("texture", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                        b.Draw(texture, language.bounds, new Rectangle?(new Rectangle(num3, num2, 174, 40)), Color.White, 0.0f, new Vector2(0.0f, 0.0f), (SpriteEffects)0, 0.0f);
                    }
                    else
                    {
                        Texture2D texture = (Texture2D)__instance.GetType().GetField("texture", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                        b.Draw(texture, language.bounds, new Rectangle?(new Rectangle(0, language.label != null ? 39 : 0, 174, 40)), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 0, 0.0f);
                        int xOffset = 18 * language.bounds.Width / 174;
                        int yOffset = 8 * language.bounds.Height / 40;
                        b.Draw(texture, new Rectangle(language.bounds.X + xOffset, language.bounds.Y + yOffset, language.bounds.Width - xOffset * 2, language.bounds.Height - yOffset * 2), new Rectangle(18, language.label != null ? 47 : 8, 14, 24), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 0, 0.0f);
                        ModConfig.Locale locale = ModEntry.ModConfig.GetByName(language.name);
                        Vector2 measureSize = Game1.dialogueFont.MeasureString(locale.DisplayName);
                        b.DrawString(Game1.dialogueFont, locale.DisplayName, new Vector2(language.bounds.X + (language.bounds.Width - measureSize.X) / 2, language.bounds.Y + (language.bounds.Height - measureSize.Y) / 2), new Color(206, 82, 82));
                    }
                }
                if (scrollArea != null)
                    scrollArea.finishScrollBoxDrawing(b, 1f);
                if ((__instance.upperRightCloseButton != null) && __instance.shouldDrawCloseButton())
                {
                    __instance.upperRightCloseButton.draw(b);
                }
                return false;
            }
        }
    }
}
