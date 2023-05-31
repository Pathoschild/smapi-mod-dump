/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewRainbowCursor
**
*************************************************/

// Copyright 2023 Jamie Taylor

using System;
using HarmonyLib;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Reflection;
using System.Reflection.Emit;
using StardewValley.Menus;
using StardewValley.Monsters;

namespace RainbowCursor {
    internal sealed class ModEntry : Mod {
        internal static IMonitor sMonitor = null!;
        internal static Texture2D myCursors = null!;
        internal static ModEntry instance = null!;

        internal ModConfig config = new ModConfig();
        internal ColorPalette currentColorPalette = null!;
        internal ColorPaletteRegistry paletteRegistry = null!;

        public override void Entry(IModHelper helper) {
            sMonitor = Monitor;
            instance = this;
            myCursors = helper.ModContent.Load<Texture2D>("cursors.png");
            I18n.Init(helper.Translation);
            config = Helper.ReadConfig<ModConfig>();
            ColorPalette prismaticPalette = new(Id: "prismatic", ProvidedBy: ModManifest,
                GetName: I18n.Palette_Prismatic_Name, Colors: new(Utility.PRISMATIC_COLORS));
            currentColorPalette = prismaticPalette;
            paletteRegistry = new(this, defaultPalette: prismaticPalette);
            RegisterPalette(prismaticPalette);
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.ConsoleCommands.Add("rainbowcursor-test", "Run some Rainbow Cursor test code. (Adds some test color palettes)", this.CliTest);
            ContentPackLoader.LoadContentPacks(this);
        }

        // test code that can be activated via the rainbowcursor-test CLI command
        private void CliTest(string command, string[] args) {
            IRainbowCursorAPI? api = Helper.ModRegistry.GetApi<IRainbowCursorAPI>(ModManifest.UniqueID);
            if (api is null) {
                // something is seriously wrong
                Monitor.Log($"Could not get my own API using my own manifest ID ({ModManifest.UniqueID})", LogLevel.Error);
                return;
            }
            api.AddColorPalette("test", () => "test 1", new List<Color> { Color.Black, Color.White }, () => "Test Palette", () => "This is a test color palette.");
            api.AddColorPalette("test", () => "test 2", new List<Color> { Color.Black, Color.White });
            api.AddColorPalette("test-null", () => "test empty colors", new List<Color> { });
        }

        public override IRainbowCursorAPI GetApi(IModInfo mod) {
            return new APIWrapper(mod.Manifest);
        }

        internal void RegisterPalette(ColorPalette p) {
            Monitor.Log(I18n.Status_AddingPalette(paletteId: p.Id, paletteName: p.GetName(), providedBy: p.ProvidedBy.Name), LogLevel.Debug);
            paletteRegistry.Add(p);
            // because when we read config we won't have installed any palettes besides
            // the default... so any time we register a new palette, refresh the current
            // palette in case it's the newly added one.
            // TODO: probably can move to after blocks where new palettes get registered
            RefreshCurrentPalette();
        }

        internal void RefreshCurrentPalette() {
            currentColorPalette = paletteRegistry.Get(config.Palette);
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
            ModConfig.RegisterGMCM(this);
            InstallPatch();
        }
        private void InstallPatch() {
            var harmony = new Harmony("jltaylor-us.RainbowCursor");
            Monitor.Log("Patching Game1.drawMouseCursor", LogLevel.Debug);
            try {
                harmony.Patch(AccessTools.Method(typeof(Game1), nameof(Game1.drawMouseCursor)),
                    transpiler: new HarmonyMethod(GetType(), nameof(ReplaceDrawCall)));
            } catch (Exception ex) {
                Monitor.Log("transpiler patch on Game1.drawMouseCursor failed:  " + ex.Message, LogLevel.Warn);
            }
            Monitor.Log("Patching IClickableMenu.drawMouse", LogLevel.Debug);
            try {
                harmony.Patch(AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.drawMouse)),
                    transpiler: new HarmonyMethod(GetType(), nameof(ReplaceDrawCall)));
            } catch (Exception ex) {
                Monitor.Log("transpiler patch on IClickableMenu.drawMouse failed:  " + ex.Message, LogLevel.Warn);
            }
            if (Helper.ModRegistry.Get("Annosz.UiInfoSuite2") is IModInfo info) {
                Monitor.Log("Patching UIInfoSuite2 DrawMouseCursor", LogLevel.Debug);
                try {
                    harmony.Patch(AccessTools.Method("UIInfoSuite2.Infrastructure.Tools:DrawMouseCursor"),
                        transpiler: new HarmonyMethod(GetType(), nameof(ReplaceDrawCall)));
                } catch (Exception ex) {
                    Monitor.Log("transpiler patch on UIInfoSuite2.Infrastructure.Tools:DrawMouseCursor failed:  " + ex.Message, LogLevel.Warn);
                }
            }
        }

        private static IEnumerable<CodeInstruction> ReplaceDrawCall(IEnumerable<CodeInstruction> instructions) {
            MethodInfo drawMethod = AccessTools.Method(typeof(SpriteBatch), nameof(SpriteBatch.Draw),
                new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) });
            int replaced = 0;
            foreach (var instruction in instructions) {
                if (instruction.Calls(drawMethod)) {
                    replaced++;
                    MethodInfo m = AccessTools.Method(typeof(ModEntry), nameof(DrawTheCursor));
                    yield return new CodeInstruction(OpCodes.Call, m);
                } else {
                    yield return instruction;
                }
            }
            sMonitor.Log($"replaced {replaced} call(s) to Draw", LogLevel.Debug);
        }

        private Color GetCurrentColor(ModConfig config, List<Color> colors) {
            float ms = (float)Game1.currentGameTime.TotalGameTime.TotalMilliseconds * config.Speed / 1000f;
            int idx0 = ((int)ms) % colors.Count;
            if (!config.Fade) {
                return colors[idx0];
            }
            int idx1 = (idx0 + 1) % colors.Count;
            float t = ms % 1f;
            Color result = new Color(
                Utility.Lerp(colors[idx0].R / 255f, colors[idx1].R / 255f, t),
                Utility.Lerp(colors[idx0].G / 255f, colors[idx1].G / 255f, t),
                Utility.Lerp(colors[idx0].B / 255f, colors[idx1].B / 255f, t));
            return result;

        }
        private Color GetCurrentColor() {
            return GetCurrentColor(this.config, this.currentColorPalette.Colors);
        }

        private static Rectangle? AdjustSourceRect(Rectangle? sourceRect) {
            if (sourceRect.HasValue && sourceRect.Value.X == 0 && sourceRect.Value.Y == 16) {
                // special case for the pointer when controller is in use
                return new(0, 0, sourceRect.Value.Width, sourceRect.Value.Height);
            }
            return sourceRect;
        }

        // This has to take exactly the same arguments at the call we're replacing in the transpiler
        private static void DrawTheCursor(SpriteBatch spriteBatch, Texture2D originalCursors, Vector2 position, Rectangle? originalSourceRect, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
            Rectangle? sourceRect = AdjustSourceRect(originalSourceRect);
            if (!ModEntry.instance.config.Enabled || !sourceRect.HasValue || sourceRect.Value.Top != 0 || sourceRect.Value.Right > 128 || Game1.mouseCursorTransparency == 0f) {
                spriteBatch.Draw(originalCursors, position, originalSourceRect, color, rotation, origin, scale, effects, layerDepth);
                return;
            }
            Color newColor = ModEntry.instance.GetCurrentColor();
            newColor.A = color.A;
            spriteBatch.Draw(myCursors, position, sourceRect, newColor, rotation, origin, scale, effects, layerDepth);
            Rectangle overlaidRect = new(sourceRect.Value.X, 16, sourceRect.Value.Width, sourceRect.Value.Height);
            spriteBatch.Draw(myCursors, position, overlaidRect, color, rotation, origin, scale, effects, layerDepth);
        }

        internal void DrawPreview(SpriteBatch spriteBatch, Vector2 pos, ModConfig withConfig) {
            if (withConfig.Enabled) {
                List<Color> colors = this.paletteRegistry.Get(withConfig.Palette).Colors;
                Color newColor = GetCurrentColor(withConfig, colors);
                spriteBatch.Draw(myCursors, pos, new Rectangle(0, 0, 128, 16), newColor, 0f, Vector2.Zero, 4.0f, SpriteEffects.None, 1f);
                spriteBatch.Draw(myCursors, pos, new Rectangle(0, 16, 128, 16), Color.White, 0f, Vector2.Zero, 4.0f, SpriteEffects.None, 1f);
            } else {
                spriteBatch.Draw(Game1.mouseCursors, pos, new Rectangle(0, 0, 128, 16), Color.White, 0f, Vector2.Zero, 4.0f, SpriteEffects.None, 1f);
            }
        }
    }

    public class APIWrapper : IRainbowCursorAPI {
        protected readonly IManifest providedByManifest;
        public APIWrapper(IManifest providedByManifest) {
            this.providedByManifest = providedByManifest;
        }
        public void AddColorPalette(string id,
                                    Func<string> getName,
                                    List<Color> colors,
                                    Func<string?>? getTitle = null,
                                    Func<string?>? getDescription = null) {
            ModEntry.instance.RegisterPalette(new ColorPalette(TransformId(id), providedByManifest, getName, colors, getTitle, getDescription));
        }
        virtual protected string TransformId(string id) {
            return id;
        }
    }
}

