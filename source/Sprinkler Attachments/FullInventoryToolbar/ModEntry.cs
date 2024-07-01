/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mushymato/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Reflection.Emit;
using System.Globalization;


namespace FullInventoryToolbar
{
    public enum ToolbarArrangement
    {
        Horizontal = 1,
        Vertical = 2,
    }
    public class ModConfig
    {
        public ToolbarArrangement Arrangement = ToolbarArrangement.Horizontal;
        public bool HideToolbarItemBoxes = false;
        public bool HideToolbarBackground = false;
        public int ToolbarRowCount = -1;

        public void Reset()
        {
            Arrangement = ToolbarArrangement.Horizontal;
            HideToolbarItemBoxes = false;
            HideToolbarBackground = false;
            ToolbarRowCount = -1;
        }
    }
    internal sealed class ModEntry : Mod
    {
        private static IMonitor? mon;
        private const int ToolbarHeight = 72;
        private const int ToolbarWidth = 776;
        private static bool ToolbarIconsLoaded = false;
        private const int ToolbarIcons_V_123_Offset = 62;

        public static ModConfig? Config;
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            mon = Monitor;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        public static void Log(string msg, LogLevel level = LogLevel.Debug)
        {
            mon!.Log(msg, level);
        }


        /// <summary>
        /// Apply <see cref="GamePatches"/> on game launch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            SetupConfig();
            ToolbarIconsLoaded = Helper.ModRegistry.IsLoaded("furyx639.ToolbarIcons");
            Harmony patcher = new(ModManifest.UniqueID);
            Patch(patcher);
        }

        private void SetupConfig()
        {
            if (Helper.ModRegistry.GetApi<Integration.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu") is Integration.IGenericModConfigMenuApi GMCM)
            {
                GMCM.Register(
                    mod: ModManifest,
                    reset: () =>
                    {
                        Config!.Reset();
                        Helper.WriteConfig(Config!);
                    },
                    save: () =>
                    {
                        Helper.WriteConfig(Config!);
                    },
                    titleScreenOnly: false
                );
                GMCM.AddNumberOption(
                    ModManifest,
                    getValue: () => { return (int)Config!.Arrangement; },
                    setValue: (value) => { Config!.Arrangement = (ToolbarArrangement)value; },
                    formatValue: (value) => { return Helper.Translation.Get($"config.ToolbarArrangement.{value}"); },
                    name: () => Helper.Translation.Get("config.ToolbarArrangement.name"),
                    tooltip: () => Helper.Translation.Get("config.ToolbarArrangement.description"),
                    min: 1, max: 2
                );
                GMCM.AddNumberOption(
                    ModManifest,
                    getValue: () => { return Config!.ToolbarRowCount; },
                    setValue: (value) => { Config!.ToolbarRowCount = value; },
                    formatValue: (value) => { return value == -1 ? Helper.Translation.Get("config.ToolbarRowCount.auto") : (value * Farmer.hotbarSize).ToString(); },
                    name: () => Helper.Translation.Get("config.ToolbarRowCount.name"),
                    tooltip: () => Helper.Translation.Get("config.ToolbarRowCount.description"),
                    min: -1, max: 3
                );
                GMCM.AddBoolOption(
                    ModManifest,
                    getValue: () => { return Config!.HideToolbarItemBoxes; },
                    setValue: (value) => { Config!.HideToolbarItemBoxes = value; },
                    name: () => Helper.Translation.Get("config.HideToolbarItemBoxes.name"),
                    tooltip: () => Helper.Translation.Get("config.HideToolbarItemBoxes.description")
                );
                GMCM.AddBoolOption(
                    ModManifest,
                    getValue: () => { return Config!.HideToolbarBackground; },
                    setValue: (value) => { Config!.HideToolbarBackground = value; },
                    name: () => Helper.Translation.Get("config.HideToolbarBackground.name"),
                    tooltip: () => Helper.Translation.Get("config.HideToolbarBackground.description")
                );
            }
            else
            {
                Helper.WriteConfig(Config!);
            }
        }

        private static void Patch(Harmony harmony)
        {
            try
            {
                // harmony.Patch(
                //     original: AccessTools.DeclaredConstructor(typeof(Toolbar), Array.Empty<Type>()),
                //     postfix: new HarmonyMethod(typeof(ModEntry), nameof(Toolbar_constructor_Postfix))
                // );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(Toolbar), nameof(Toolbar.gameWindowSizeChanged)),
                    postfix: new HarmonyMethod(typeof(ModEntry), nameof(Toolbar_gameWindowSizeChanged_Postfix))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(Toolbar), nameof(Toolbar.isWithinBounds)),
                    prefix: new HarmonyMethod(typeof(ModEntry), nameof(Toolbar_isWithinBounds_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(Toolbar), nameof(Toolbar.draw)),
                    prefix: new HarmonyMethod(typeof(ModEntry), nameof(Toolbar_draw_Prefix)),
                    postfix: new HarmonyMethod(typeof(ModEntry), nameof(Toolbar_draw_Postfix)),
                    transpiler: new HarmonyMethod(typeof(ModEntry), nameof(Toolbar_draw_Transpiler))
                );
                harmony.Patch(
                    original: AccessTools.DeclaredMethod(typeof(Game1), nameof(Game1.pressSwitchToolButton)),
                    transpiler: new HarmonyMethod(typeof(ModEntry), nameof(Toolbar_pressSwitchToolButton_Transpiler))
                );
            }
            catch (Exception err)
            {
                Log($"Failed to patch FullInventoryToolbar:\n{err}", LogLevel.Error);
            }
        }

        private static int AlignY(int yPositionOnScreen, int row)
        {
            switch (Config!.Arrangement)
            {
                case ToolbarArrangement.Vertical:
                    int alignY = row * ToolbarHeight;
                    if (ToolbarIconsLoaded)
                        alignY += ToolbarIcons_V_123_Offset;
                    alignY *= (yPositionOnScreen > Game1.uiViewport.Height / 2) ? -1 : 1;
                    return alignY;
                default:
                    return 0;
            }
        }

        private static int AlignX(int row)
        {
            return Config!.Arrangement switch
            {
                ToolbarArrangement.Horizontal => ((row % 2 == 1) ? (row / 2 + 1) : -(row / 2)) * ToolbarWidth,
                _ => 0,
            };
        }

        private static void InitializeExtraButtons(Toolbar __instance)
        {
            try
            {
                // __instance.xPositionOnScreen -= ToolbarWidth;
                // __instance.width *= 3;
                for (int i = __instance.buttons.Count; i < GetMaxItems(); i++)
                {
                    __instance.buttons.Add(
                        new ClickableComponent(
                            new Rectangle(-65, -65, 64, 64),
                            i.ToString() ?? ""
                        )
                    );
                }
            }
            catch (Exception err)
            {
                Log($"Error in InitializeExtraButtons:\n{err}", LogLevel.Error);
            }
        }

        private static void Toolbar_gameWindowSizeChanged_Postfix(Toolbar __instance, Rectangle oldBounds, Rectangle newBounds)
        {
            try
            {
                for (int i = Farmer.hotbarSize; i < __instance.buttons.Count; i++)
                {
                    UpdateButtonBounds(__instance, i);
                }
            }
            catch (Exception err)
            {
                Log($"Error in Toolbar_gameWindowSizeChanged_Postfix:\n{err}", LogLevel.Error);
            }
        }

        private static void UpdateButtonBounds(Toolbar tb, int i)
        {
            tb.buttons[i].bounds.X = Game1.uiViewport.Width / 2 - 384 + i % Farmer.hotbarSize * 64 + AlignX(i / Farmer.hotbarSize);
            tb.buttons[i].bounds.Y = tb.yPositionOnScreen - 96 + 8 + AlignY(tb.yPositionOnScreen, i / Farmer.hotbarSize);
        }

        private static bool Toolbar_isWithinBounds_Prefix(Toolbar __instance, ref bool __result, int x, int y)
        {
            try
            {
                int maxItems = GetMaxItems();
                if (__instance.buttons.Count < maxItems)
                    InitializeExtraButtons(__instance);
                int rowCount = maxItems / Farmer.hotbarSize;
                Rectangle firstBounds;
                Rectangle lastBounds;
                if (Config!.Arrangement == ToolbarArrangement.Horizontal)
                {
                    firstBounds = __instance.buttons[Farmer.hotbarSize * ((rowCount - 1) / 2) * 2].bounds;
                    lastBounds = __instance.buttons[Farmer.hotbarSize * (rowCount / 2 + 1) - 1].bounds;
                }
                else
                {
                    if (__instance.yPositionOnScreen > Game1.uiViewport.Height / 2)
                    {
                        firstBounds = __instance.buttons[Farmer.hotbarSize * (rowCount - 1)].bounds;
                        lastBounds = __instance.buttons[Farmer.hotbarSize - 1].bounds;
                    }
                    else
                    {
                        firstBounds = __instance.buttons[0].bounds;
                        lastBounds = __instance.buttons[maxItems - 1].bounds;
                    }
                }
                __result = new Rectangle(
                    firstBounds.X, firstBounds.Y,
                    lastBounds.Right - firstBounds.Left,
                    lastBounds.Bottom - firstBounds.Top
                ).Contains(x, y);
                return false;
            }
            catch (Exception err)
            {
                Log($"Error in Toolbar_gameWindowSizeChanged_Postfix:\n{err}", LogLevel.Error);
                return true;
            }
        }


        private static void Toolbar_draw_Prefix(Toolbar __instance, SpriteBatch b)
        {
            try
            {
                if (__instance.buttons.Count < GetMaxItems())
                    InitializeExtraButtons(__instance);
            }
            catch (Exception err)
            {
                Log($"Error in Toolbar_draw_Postfix:\n{err}", LogLevel.Error);
            }
        }


        private static void Toolbar_draw_Postfix(Toolbar __instance, SpriteBatch b)
        {
            try
            {
                if (Farmer.hotbarSize < GetMaxItems())
                    DrawToolbarRows(__instance, b);
            }
            catch (Exception err)
            {
                Log($"Error in Toolbar_draw_Postfix:\n{err}", LogLevel.Error);
            }
        }

        private static void DrawToolbarBox(SpriteBatch b, Texture2D texture, Rectangle sourceRect, int x, int y, int width, int height, Color color, float scale = 1f, bool drawShadow = true, float draw_layer = -1f)
        {
            if (Config!.HideToolbarBackground)
                return;
            int rowCountM1 = GetMaxItems() / Farmer.hotbarSize - 1;
            if (Config!.Arrangement == ToolbarArrangement.Vertical)
            {
                if (ToolbarIconsLoaded)
                {
                    // draw the original toolbar box
                    IClickableMenu.drawTextureBox(b, texture, sourceRect, x, y, width, height, color, scale: scale, drawShadow: drawShadow, draw_layer: draw_layer);
                    // set the right y & height for 2 rows of toolbar box
                    height = ToolbarHeight * rowCountM1 + (height - ToolbarHeight);
                    if (y > Game1.uiViewport.Height / 2)
                    {
                        y -= ToolbarHeight * rowCountM1 + ToolbarIcons_V_123_Offset;
                    }
                    else
                    {
                        y += ToolbarHeight + ToolbarIcons_V_123_Offset;
                    }
                }
                else
                {
                    height += ToolbarHeight * rowCountM1;
                    if (y > Game1.uiViewport.Height / 2)
                        y -= ToolbarHeight * rowCountM1;

                }
            }
            else
            {
                width += ToolbarWidth * rowCountM1;
                x -= ToolbarWidth * (rowCountM1 / 2);
            }
            IClickableMenu.drawTextureBox(b, texture, sourceRect, x, y, width, height, color, scale: scale, drawShadow: drawShadow, draw_layer: draw_layer);
        }

        private static void DrawItemBox(SpriteBatch b, Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color)
        {
            if (!Config!.HideToolbarItemBoxes || Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 56) == sourceRect)
                b.Draw(texture, position, sourceRect, color);
        }

        private static void DrawToolbarRows(Toolbar tb, SpriteBatch b)
        {
            if (Game1.activeClickableMenu != null)
                return;

            int maxItems = GetMaxItems();
            int i;
            for (i = Farmer.hotbarSize; i < maxItems; i++)
            {
                UpdateButtonBounds(tb, i);
                Vector2 toDraw = new(Game1.uiViewport.Width / 2 - 384 + i % Farmer.hotbarSize * 64 + AlignX(i / Farmer.hotbarSize), tb.yPositionOnScreen - 96 + 8 + AlignY(tb.yPositionOnScreen, i / Farmer.hotbarSize));
                DrawItemBox(b, Game1.menuTexture, toDraw, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, (Game1.player.CurrentToolIndex == i) ? 56 : 10), Color.White * tb.transparency);
            }

            int highestItem = 0;
            for (i = Farmer.hotbarSize; i < maxItems; i++)
            {
                if (Game1.player.Items.Count > i && Game1.player.Items[i] != null)
                    highestItem = i;
            }

            for (i = Farmer.hotbarSize; i < (highestItem + 1); i++)
            {
                if (Game1.player.Items.Count > i && Game1.player.Items[i] != null)
                {
                    tb.buttons[i].scale = Math.Max(1f, tb.buttons[i].scale - 0.025f);
                    Vector2 toDraw2 = new(Game1.uiViewport.Width / 2 - 384 + i % Farmer.hotbarSize * 64 + AlignX(i / Farmer.hotbarSize), tb.yPositionOnScreen - 96 + 8 + AlignY(tb.yPositionOnScreen, i / Farmer.hotbarSize));
                    Game1.player.Items[i].drawInMenu(b, toDraw2, (Game1.player.CurrentToolIndex == i) ? 0.9f : (tb.buttons[i].scale * 0.8f), tb.transparency, 0.88f);
                }
            }

            if (tb.hoverItem != null)
            {
                IClickableMenu.drawToolTip(b, tb.hoverItem.getDescription(), tb.hoverItem.DisplayName, tb.hoverItem);
                tb.hoverItem = null;
            }
        }

        private static IEnumerable<CodeInstruction> Toolbar_draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                CodeMatcher matcher = new(instructions, generator);

                // hijack IClickableMenu.drawTextureBox call
                matcher = matcher.Start()
                .MatchStartForward(new CodeMatch[]{
                    new(
                        OpCodes.Call,
                        AccessTools.Method(
                            typeof(IClickableMenu), nameof(IClickableMenu.drawTextureBox),
                            new Type[]{
                                typeof(SpriteBatch), typeof(Texture2D), typeof(Rectangle),
                                typeof(int), typeof(int), typeof(int), typeof(int),
                                typeof(Color), typeof(float), typeof(bool), typeof(float)
                            }
                        )
                    ),
                    new(OpCodes.Ldc_I4_0)
                })
                ;
                matcher.Instruction.operand = AccessTools.Method(typeof(ModEntry), nameof(DrawToolbarBox));

                // replace drawing of item boxes
                matcher = matcher
                .MatchStartForward(new CodeMatch[]{
                    new(OpCodes.Callvirt, AccessTools.Method(
                        typeof(SpriteBatch), nameof(SpriteBatch.Draw),
                        new Type[]{
                            typeof(Texture2D), typeof(Vector2),
                            typeof(Rectangle?), typeof(Color)
                        }
                    )),
                    new(OpCodes.Call),
                    new(OpCodes.Ldfld, AccessTools.Field(typeof(Options), nameof(Options.gamepadControls)))
                })
                .SetInstruction(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(DrawItemBox))));

                // block drawing of hoverItem, to let draw postfix handle it
                matcher = matcher
                .MatchStartForward(new CodeMatch[]{
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, AccessTools.Field(typeof(Toolbar), nameof(Toolbar.hoverItem))),
                    new(OpCodes.Brfalse_S)
                })
                ;

                matcher.Instruction.opcode = OpCodes.Nop;
                matcher.Instruction.operand = null;
                matcher.Advance(1);
                matcher.Instruction.opcode = OpCodes.Nop;
                matcher.Instruction.operand = null;
                matcher.Advance(1);
                matcher.Instruction.opcode = OpCodes.Br_S;

                return matcher.Instructions();
            }
            catch (Exception err)
            {
                Log($"Error in Toolbar_draw_Transpiler:\n{err}", LogLevel.Error);
                return instructions;
            }
        }

        /// <summary>
        /// Get max inventory items, either Game1.player.MaxItems or Farmer.maxInventorySpace
        /// </summary>
        /// <returns></returns>
        public static int GetMaxItems()
        {
            int maxItems = Farmer.maxInventorySpace;
            if (Game1.player != null)
                maxItems = Game1.player.MaxItems;
            if (Config!.ToolbarRowCount != -1)
                maxItems = Math.Min(Config!.ToolbarRowCount * Farmer.hotbarSize, maxItems);
            return maxItems;
        }

        private static IEnumerable<CodeInstruction> Toolbar_pressSwitchToolButton_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                CodeMatcher matcher = new(instructions, generator);

                // change 3 checks for 12 to 36, and 2 checks for 11 to 35
                matcher = matcher.Start()
                .MatchStartForward(new CodeMatch[]{
                    new(OpCodes.Add),
                    new(OpCodes.Ldc_I4_S, (sbyte) 12),
                    new(OpCodes.Rem)
                })
                ;
                matcher.Advance(1).SetInstruction(new CodeInstruction(
                    OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(GetMaxItems))
                ));

                matcher = matcher
                .MatchEndForward(new CodeMatch[]{
                    new(OpCodes.Call, AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.player))),
                    new(OpCodes.Ldc_I4_S, (sbyte) 11),
                })
                ;
                matcher.SetInstructionAndAdvance(new CodeInstruction(
                    OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(GetMaxItems))
                ))
                .Insert(new CodeInstruction[]
                {
                    new(OpCodes.Ldc_I4, 1),
                    new(OpCodes.Sub)
                });

                matcher = matcher
                .MatchStartForward(new CodeMatch[]{
                    new(OpCodes.Add),
                    new(OpCodes.Ldc_I4_S, (sbyte) 12),
                    new(OpCodes.Rem)
                })
                ;
                // matcher.InstructionAt(1).operand = (sbyte)Farmer.maxInventorySpace;
                matcher.Advance(1).SetInstruction(new CodeInstruction(
                    OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(GetMaxItems))
                ));

                matcher = matcher
                .MatchEndForward(new CodeMatch[]{
                    new(OpCodes.Call, AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.player))),
                    new(OpCodes.Ldc_I4_S, (sbyte) 11),
                })
                ;
                matcher.SetInstructionAndAdvance(new CodeInstruction(
                    OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(GetMaxItems))
                ))
                .Insert(new CodeInstruction[]
                {
                    new(OpCodes.Ldc_I4, 1),
                    new(OpCodes.Sub)
                });

                matcher = matcher
                .MatchStartForward(new CodeMatch[]{
                    new(OpCodes.Ldloc_3),
                    new(OpCodes.Ldc_I4_S, (sbyte) 12),
                    new(OpCodes.Blt_S)
                })
                ;
                matcher.Advance(1).SetInstruction(new CodeInstruction(
                    OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(GetMaxItems))
                ));

                return matcher.Instructions();
            }
            catch (Exception err)
            {
                Log($"Error in Toolbar_draw_Transpiler:\n{err}", LogLevel.Error);
                return instructions;
            }
        }
    }
}
