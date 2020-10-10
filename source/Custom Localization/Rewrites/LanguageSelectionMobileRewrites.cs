/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ZaneYork/SDV_CustomLocalization
**
*************************************************/

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
        public class SetupButtonsRewrite
        {
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
        public class SetCurrentItemIndexRewrite
        {
            public static void Prefix(object __instance)
            {
                Rectangle mainBox = ModEntry.Reflection.GetField<Rectangle>(__instance, "mainBox").GetValue();
                int buttonHeight = ModEntry.Reflection.GetField<int>(__instance, "buttonHeight").GetValue();
                int buttonWidth = ModEntry.Reflection.GetField<int>(__instance, "buttonWidth").GetValue();
                for (short i = 0; i < ModEntry.ModConfig.locales.Length; i++)
                {
                    ModConfig.Locale locale = ModEntry.ModConfig.locales[i];
                    List<ClickableComponent> languages = ModEntry.Reflection.GetField<List<ClickableComponent>>(__instance, "languages").GetValue();
                    languages.Add(new ClickableComponent(
                        new Rectangle(mainBox.X + 0x10, (mainBox.Y + 0x10) + (buttonHeight * ModEntry.ModConfig.OriginLocaleCount + i), buttonWidth, buttonHeight),
                        locale.Name, null));
                    if ((int)(CurrentLanguageCode) == locale.CodeEnum)
                    {
                        ModEntry.Reflection.GetField<string>(__instance, "languageCodeName").SetValue(locale.Name);
                    }
                }
            }
        }
        public class ReleaseLeftClickRewrite
        {
            public static bool Prefix(object __instance, int x, int y)
            {
                object scrollArea = ModEntry.Reflection.GetField<object>(__instance, "scrollArea").GetValue();
                if (scrollArea == null || !ModEntry.Reflection.GetField<bool>(scrollArea, "havePanelScrolled").GetValue())
                {
                    List<ClickableComponent> languages = ModEntry.Reflection.GetField<List<ClickableComponent>>(__instance, "languages").GetValue();
                    foreach (ClickableComponent language in languages)
                    {
                        if (language.containsPoint(x, y))
                        {
                            ModEntry.Reflection.GetField<bool>(__instance, "languageChosen").SetValue(true);
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
                            ModEntry.Reflection.GetMethod(__instance, "exitThisMenu").Invoke(true);
                        }
                    }
                }
                if (scrollArea == null)
                    return false;
                ModEntry.Reflection.GetMethod(scrollArea, "releaseLeftClick").Invoke(x, y);
                return false;
            }
        }
        public class DrawRewrite
        {
            public static bool Prefix(object __instance, SpriteBatch b)
            {
                int width = ModEntry.Reflection.GetField<int>(__instance, "width").GetValue();
                int height = ModEntry.Reflection.GetField<int>(__instance, "height").GetValue();
                Utility.getTopLeftPositionForCenteringOnScreen(width, height - 100, 0, 0);
                SpriteBatch spriteBatch = b;
                Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
                Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
                Rectangle bounds = viewport.Bounds;
                Color color = Color.Multiply(Color.Black, 0.6f);
                spriteBatch.Draw(fadeToBlackRect, bounds, color);
                Rectangle mainBox = (Rectangle)__instance.GetType().GetField("mainBox", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                IClickableMenu.drawTextureBox(b, (int)mainBox.X, (int)mainBox.Y, (int)mainBox.Width, (int)mainBox.Height, Color.White);
                object scrollArea = ModEntry.Reflection.GetField<object>(__instance, "scrollArea").GetValue();
                if (scrollArea != null)
                {
                    object newScrollbar = ModEntry.Reflection.GetField<object>(__instance, "newScrollbar").GetValue();
                    ModEntry.Reflection.GetMethod(newScrollbar, "draw").Invoke(b);
                    ModEntry.Reflection.GetMethod(scrollArea, "setUpForScrollBoxDrawing").Invoke(b, 1f);
                }
                List<ClickableComponent> languages = ModEntry.Reflection.GetField<List<ClickableComponent>>(__instance, "languages").GetValue();
                foreach (ClickableComponent language in languages)
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
                    ModEntry.Reflection.GetMethod(scrollArea, "finishScrollBoxDrawing").Invoke(b, 1f);
                ClickableTextureComponent upperRightCloseButton = ModEntry.Reflection.GetField<ClickableTextureComponent>(__instance, "upperRightCloseButton").GetValue();
                if (upperRightCloseButton != null && ModEntry.Reflection.GetMethod(__instance, "shouldDrawCloseButton").Invoke<bool>())
                {
                    upperRightCloseButton.draw(b);
                }
                return false;
            }
        }
    }
}
