/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FishingMinigames
{
    public class ModEntry : Mod, IAssetEditor
    {
        public static ITranslationHelper translate;
        public static ModConfig config;
        public static Regex exception = new Regex(@"([^\\]*)\.cs:.*");
        private readonly PerScreen<Minigames> minigame = new PerScreen<Minigames>();
        private Dictionary<string, int> itemIDs = new Dictionary<string, int>();
        private bool canStartEditingAssets = false;

        private enum StartMinigame { DDR, Hangman }


        public override void Entry(IModHelper helper)
        {
            Log.Monitor = Monitor;
            translate = Helper.Translation;
            UpdateConfig(false);
            MinigamesStart.minigameTextures = new Texture2D[] {
                Game1.content.Load<Texture2D>("LooseSprites\\boardGameBorder"),
                Game1.content.Load<Texture2D>("LooseSprites\\CraneGame"),
                Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles"),
                Helper.Content.Load<Texture2D>("assets/Textures.png", ContentSource.ModFolder)};


            helper.Events.Display.Rendered += Display_Rendered;
            helper.Events.Display.RenderingWorld += Display_RenderingWorld;
            helper.Events.Display.RenderedWorld += Display_RenderedWorld;
            helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
            helper.Events.Display.RenderedActiveMenu += GenericModConfigMenuIntegration;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            //helper.Events.Player.Warped += OnWarped;

            helper.ConsoleCommands.Add("startminigametest", "For testing the Start Minigame of the Fishing Minigames mod. If holding a fishing rod/net, its data will be used, otherwise uses a basic one.\n\n" +
                "Usage: startminigametest <fish>\n- fish: ID or name (fuzzy search, single word only) of the fish to use. Random size (between min-max) is used.\n\n" +
                "Usage: startminigametest <I:difficulty> <I:size> [S:boss]\n- difficulty: fish difficulty (int, vanilla = 15-110)\n" +
                "- size: fishSize (int, vanilla = 1-73)\n- boss: true = bossFish, can be blank\nHighest vanilla combo = 110 51 true", this.StartMinigameTest);

            var harmony = new Harmony(ModManifest.UniqueID);//this might summon Cthulhu
            //harmony.PatchAll();
            harmony.Patch(
                original: AccessTools.Method(typeof(Tool), "getDescription"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), "getDescription_Nets")
            );
        }


        private void GenericModConfigMenuIntegration(object sender, RenderedActiveMenuEventArgs e)     //Generic Mod Config Menu API
        {
            Helper.Events.Display.RenderedActiveMenu -= GenericModConfigMenuIntegration;
            if (Context.IsSplitScreen) return;
            canStartEditingAssets = true;
            Helper.Content.InvalidateCache("TileSheets/tools");
            Helper.Content.InvalidateCache("Maps/springobjects");
            Helper.Content.InvalidateCache("Strings/StringsFromCSFiles");
            Helper.Content.InvalidateCache("Data/ObjectInformation");

            var GenericMC = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (GenericMC != null)
            {
                GenericMC.Register(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
                GenericMC.AddSectionTitle(ModManifest, () => translate.Get("GenericMC.MainLabel")); //All of these strings are stored in the traslation files.
                GenericMC.AddParagraph(ModManifest, () => translate.Get("GenericMC.MainDesc"));
                GenericMC.AddParagraph(ModManifest, () => translate.Get("GenericMC.MainDesc2"));
                GenericMC.AddParagraph(ModManifest, () => translate.Get("GenericMC.MainDesc3"));

                try
                {
                    GenericMC.AddNumberOption(ModManifest, () => config.VoiceVolume, (int val) => config.VoiceVolume = val, () => translate.Get("GenericMC.Volume"), () => translate.Get("GenericMC.VolumeDesc"), 0, 100);

                    GenericMCPerScreen(GenericMC, 0);
                    GenericMC.AddPageLink(ModManifest, "colors", () => translate.Get("GenericMC.Colors"), () => translate.Get("GenericMC.Colors"));
                    GenericMC.AddPageLink(ModManifest, "itemData", () => translate.Get("GenericMC.ItemData"), () => translate.Get("GenericMC.ItemData"));

                    GenericMC.AddPageLink(ModManifest, "s2", () => translate.Get("GenericMC.SplitScreen2"), () => translate.Get("GenericMC.SplitScreenDesc"));
                    GenericMC.AddPageLink(ModManifest, "s3", () => translate.Get("GenericMC.SplitScreen3"), () => translate.Get("GenericMC.SplitScreenDesc"));
                    GenericMC.AddPageLink(ModManifest, "s4", () => translate.Get("GenericMC.SplitScreen4"), () => translate.Get("GenericMC.SplitScreenDesc"));
                    GenericMCPerScreen(GenericMC, 1);
                    GenericMCPerScreen(GenericMC, 2);
                    GenericMCPerScreen(GenericMC, 3);

                    GenericMC.AddPage(ModManifest, "colors", () => translate.Get("GenericMC.Colors"));
                    GenericMCColorPicker(GenericMC, ModManifest);
                    GenericMC.AddBoolOption(ModManifest, () => config.BossTransparency, (bool val) => config.BossTransparency = val,
                        () => translate.Get("GenericMC.BossTransparency"), () => translate.Get("GenericMC.BossTransparencyDesc"));

                    GenericMC.AddPage(ModManifest, "itemData", () => translate.Get("GenericMC.ItemData"));
                    GenericMC.AddParagraph(ModManifest, () => translate.Get("GenericMC.ItemDataDesc1"));
                    GenericMC.AddParagraph(ModManifest, () => translate.Get("GenericMC.ItemDataDesc2"));
                    GenericMC.AddParagraph(ModManifest, () => translate.Get("GenericMC.ItemDataDesc3"));
                    GenericMC.AddParagraph(ModManifest, () => translate.Get("GenericMC.ItemDataDesc4"));

                    foreach (var item in config.SeeInfoForBelowData)
                    {
                        Item o = (itemIDs.ContainsKey(item.Key)) ? new StardewValley.Object(itemIDs[item.Key], 1) : null;
                        string space = "";
                        if (o == null)
                        {
                            FishingRod rod = new FishingRod();
                            while (rod.UpgradeLevel < 5)
                            {
                                if (rod.Name.Equals(item.Key))
                                {
                                    o = rod;
                                    break;
                                }
                                rod.UpgradeLevel++;
                            }
                        }
                        if (o != null)
                        {
                            for (int i = 0; i < 26 - (o.DisplayName.Length / 2); i++)
                            {
                                space += " ";
                            }
                            if (o is FishingRod) GenericMC.AddImage(ModManifest, () => Game1.toolSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.toolSpriteSheet, (o as FishingRod).IndexOfMenuItemView, 16, 16), 4);
                            else GenericMC.AddImage(ModManifest, () => Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, o.ParentSheetIndex, 16, 16), 4);
                            GenericMC.AddSectionTitle(ModManifest, () => space + o.DisplayName, () => item.Key);
                        }
                        else
                        {
                            for (int i = 0; i < 26 - (item.Key.Length / 2); i++)
                            {
                                space += " ";
                            }
                            GenericMC.AddSectionTitle(ModManifest, () => space + item.Key, () => item.Key);
                        }

                        foreach (var effect in item.Value)
                        {
                            if (effect.Key.StartsWith("EXTRA_", StringComparison.Ordinal)) GenericMC.AddNumberOption(ModManifest, () => config.SeeInfoForBelowData[item.Key][effect.Key],
                                (int val) => config.SeeInfoForBelowData[item.Key][effect.Key] = val, () => effect.Key, () => translate.Get("Effects.EXTRA").Tokens(new { max = "MAX", chance = "CHANCE" }),
                                0, effect.Key.Equals("EXTRA_MAX", StringComparison.Ordinal) ? 10 : 100);
                            else GenericMC.AddNumberOption(ModManifest, () => config.SeeInfoForBelowData[item.Key][effect.Key], (int val) => config.SeeInfoForBelowData[item.Key][effect.Key] = val,
                                () => effect.Key, () => translate.Get("Effects." + effect.Key).Tokens(new { val = "X" }),
                                (effect.Key.Equals("QUALITY", StringComparison.Ordinal) ? -4 : effect.Key.Equals("LIFE", StringComparison.Ordinal) ? 0 : -100),
                                (effect.Key.Equals("QUALITY", StringComparison.Ordinal) ? 4 : effect.Key.Equals("LIFE", StringComparison.Ordinal) ? 50 : 300));
                        }
                    }

                    //dummy value validation trigger - must be the last thing, so all values are saved before validation
                    GenericMC.AddComplexOption(ModManifest, () => "", () => "", (SpriteBatch b, Vector2 pos) => { }, () => UpdateConfig(true));
                }
                catch (Exception)
                {
                    this.Monitor.Log("Error parsing config data. Please either fix your config.json, or delete it to generate a new one.", LogLevel.Error);
                }
            }
        }
        private void GenericMCPerScreen(IGenericModConfigMenuApi GenericMC, int screen)
        {
            if (screen > 0)//make new page
            {
                GenericMC.AddPage(ModManifest, "s" + (screen + 1), () => translate.Get("GenericMC.SplitScreen" + (screen + 1)));
            }
            GenericMC.AddTextOption(ModManifest, () => config.VoiceType[screen], (string val) => config.VoiceType[screen] = val,
                () => translate.Get("GenericMC.VoiceType"), () => translate.Get("GenericMC.VoiceTypeDesc"), new string[] { "Silent" }.Concat(Minigames.voices.Keys).ToArray());

            GenericMC.AddNumberOption(ModManifest, () => config.VoicePitch[screen], (int val) => config.VoicePitch[screen] = val,
                () => translate.Get("GenericMC.Pitch"), () => translate.Get("GenericMC.PitchDesc"), -100, 100);

            GenericMC.AddTextOption(ModManifest, () => config.KeyBinds[screen], (string val) => config.KeyBinds[screen] = val,
                () => translate.Get("GenericMC.KeyBinds"), () => translate.Get("GenericMC.KeyBindsDesc"));

            GenericMC.AddBoolOption(ModManifest, () => config.FreeAim[screen], (bool val) => config.FreeAim[screen] = val,
                () => translate.Get("GenericMC.FreeAim"), () => translate.Get("GenericMC.FreeAimDesc"));

            GenericMC.AddTextOption(ModManifest, name: () => translate.Get("GenericMC.StartMinigameStyle"),  tooltip: () => translate.Get("GenericMC.StartMinigameStyleDesc"),
                getValue: () => config.StartMinigameStyle[screen].ToString(),
                setValue: value => config.StartMinigameStyle[screen] = int.Parse(value),
                allowedValues: new string[] { "0", "1", "2" },
                formatAllowedValue: value => value == "0" ? translate.Get($"GenericMC.Disabled") : translate.Get($"GenericMC.StartMinigameStyle{value}"));

            GenericMC.AddTextOption(ModManifest, name: () => translate.Get("GenericMC.EndMinigameStyle"), tooltip: () => translate.Get("GenericMC.EndMinigameStyleDesc"),
                getValue: () => config.EndMinigameStyle[screen].ToString(),
                setValue: value => config.EndMinigameStyle[screen] = int.Parse(value),
                allowedValues: new string[] { "0", "1", "2", "3" },
                formatAllowedValue: value => value == "0" ? translate.Get($"GenericMC.Disabled") : translate.Get($"GenericMC.EndMinigameStyle{value}"));

            GenericMC.AddBoolOption(ModManifest, () => config.EndLoseTreasureIfFailed[screen], (bool val) => config.EndLoseTreasureIfFailed[screen] = val,
                () => translate.Get("GenericMC.EndLoseTreasure"), () => translate.Get("GenericMC.EndLoseTreasureDesc"));
            GenericMC.AddNumberOption(ModManifest, () => config.EndMinigameDamage[screen], (float val) => config.EndMinigameDamage[screen] = val,
                () => translate.Get("GenericMC.EndDamage"), () => translate.Get("GenericMC.EndDamageDesc"), 0f, 2f, 0.05f);
            GenericMC.AddNumberOption(ModManifest, () => config.MinigameDifficulty[screen], (float val) => config.MinigameDifficulty[screen] = val,
                () => translate.Get("GenericMC.Difficulty"), () => translate.Get("GenericMC.DifficultyDesc"), 0.1f, 2f, 0.05f);

            GenericMC.AddBoolOption(ModManifest, () => config.TutorialSkip[screen], (bool val) => config.TutorialSkip[screen] = val,
                () => translate.Get("GenericMC.TutorialSkip"), () => translate.Get("GenericMC.TutorialSkipDesc"));

            if (screen == 0)//only page 0
            {
                GenericMC.AddNumberOption(ModManifest, () => config.StartMinigameScale, (float val) => config.StartMinigameScale = val,
                    () => translate.Get("GenericMC.StartMinigameScale"), () => translate.Get("GenericMC.StartMinigameScale"), 0.5f, 5f, 0.05f);

                GenericMC.AddBoolOption(ModManifest, () => config.RealisticSizes, (bool val) => config.RealisticSizes = val,
                    () => translate.Get("GenericMC.RealisticSizes"), () => translate.Get("GenericMC.RealisticSizesDesc"));

                if (LocalizedContentManager.CurrentLanguageCode == 0) GenericMC.AddBoolOption(ModManifest, () => config.ConvertToMetric, (bool val) => config.ConvertToMetric = val,
                    () => translate.Get("GenericMC.ConvertToMetric"), () => translate.Get("GenericMC.ConvertToMetricDesc"));

                GenericMC.AddBoolOption(ModManifest, () => config.FishTankHoldSprites, (bool val) => config.FishTankHoldSprites = val,
                    () => translate.Get("GenericMC.FishTankHold"), () => translate.Get("GenericMC.FishTankHoldDesc"));

                GenericMC.AddSectionTitle(ModManifest, () => translate.Get("GenericMC.FestivalLabel"));
                GenericMC.AddParagraph(ModManifest, () => translate.Get("GenericMC.FestivalDesc"));
                GenericMC.AddParagraph(ModManifest, () => translate.Get("GenericMC.FestivalDesc2"));
            }

            GenericMC.AddTextOption(ModManifest, name: () => translate.Get("GenericMC.FestivalMode"), tooltip: () => translate.Get("GenericMC.FestivalModeDesc"),
                getValue: () => config.FestivalMode[screen].ToString(),
                setValue: value => config.FestivalMode[screen] = int.Parse(value),
                allowedValues: new string[] { "0", "1", "2", "3" },
                formatAllowedValue: value => translate.Get($"GenericMC.FestivalMode{value}"));
        }
        private void GenericMCColorPicker(IGenericModConfigMenuApi GenericMC, IManifest mod)
        {
            MinigameColor state = null;
            void Draw(SpriteBatch b, Vector2 pos)
            {
                if (state == null) state = new MinigameColor() { color = config.MinigameColor, pos = pos, whichSlider = 0 };

                KeybindList click = KeybindList.Parse("MouseLeft");
                int width = Math.Min(Game1.uiViewport.Width / 4, 400);

                byte mousePercentage = (byte)(int)((Utility.Clamp(Game1.getOldMouseX(), pos.X, pos.X + width) - pos.X) / width * 255);

                Rectangle barR = new Rectangle((int)pos.X, (int)pos.Y, width, 24);
                Rectangle barG = new Rectangle((int)pos.X, (int)pos.Y + 80, width, 24);
                Rectangle barB = new Rectangle((int)pos.X, (int)pos.Y + 160, width, 24);

                if ((barR.Contains(Game1.getMouseX(), Game1.getMouseY()) && click.JustPressed()) || (state.whichSlider == 1 && click.IsDown()))
                {
                    state.whichSlider = 1;
                    state.color.R = mousePercentage;
                }
                else if ((barG.Contains(Game1.getMouseX(), Game1.getMouseY()) && click.JustPressed()) || (state.whichSlider == 2 && click.IsDown()))
                {
                    state.whichSlider = 2;
                    state.color.G = mousePercentage;
                }
                else if ((barB.Contains(Game1.getMouseX(), Game1.getMouseY()) && click.JustPressed()) || (state.whichSlider == 3 && click.IsDown()))
                {
                    state.whichSlider = 3;
                    state.color.B = mousePercentage;
                }
                else if (state.whichSlider != 0) state.whichSlider = 0;

                float scale = width / 400f;

                Rectangle posR = new Rectangle((int)(pos.X + (state.color.R / 255f) * (width - 40)), (int)pos.Y, 40, 24);
                Rectangle posG = new Rectangle((int)(pos.X + (state.color.G / 255f) * (width - 40)), (int)pos.Y + 80, 40, 24);
                Rectangle posB = new Rectangle((int)(pos.X + (state.color.B / 255f) * (width - 40)), (int)pos.Y + 160, 40, 24);

                StardewValley.Menus.IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), barR.X, barR.Y, barR.Width, barR.Height, Color.White, Game1.pixelZoom, false);
                StardewValley.Menus.IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), barG.X, barG.Y, barG.Width, barG.Height, Color.White, Game1.pixelZoom, false);
                StardewValley.Menus.IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), barB.X, barB.Y, barB.Width, barB.Height, Color.White, Game1.pixelZoom, false);

                b.Draw(Game1.mouseCursors, new Vector2(posR.X, posR.Y), new Rectangle(420, 441, 10, 6), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
                b.Draw(Game1.mouseCursors, new Vector2(posG.X, posG.Y), new Rectangle(420, 441, 10, 6), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
                b.Draw(Game1.mouseCursors, new Vector2(posB.X, posB.Y), new Rectangle(420, 441, 10, 6), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);

                b.DrawString(Game1.smallFont, "R:" + string.Format("{0,6}", state.color.R), new Vector2(barR.X + width + 20, barR.Y - 4), Color.Black);
                b.DrawString(Game1.smallFont, "G:" + string.Format("{0,6}", state.color.G), new Vector2(barG.X + width + 20, barG.Y - 4), Color.Black);
                b.DrawString(Game1.smallFont, "B:" + string.Format("{0,6}", state.color.B), new Vector2(barB.X + width + 20, barB.Y - 4), Color.Black);

                Vector2 screenMid = new Vector2(pos.X - (width / 1.5f), pos.Y + 90);

                b.Draw(Game1.mouseCursors, screenMid, new Rectangle(31, 1870, 73, 49), state.color, 0f, new Vector2(36.5f, 22.5f), 4f * scale, SpriteEffects.None, 0.2f);
                b.Draw(MinigamesStart.minigameTextures[0], screenMid, null, state.color, 0f, new Vector2(69f, 37f), 2f * scale, SpriteEffects.None, 0.3f);
                b.Draw(MinigamesStart.minigameTextures[1], screenMid + new Vector2(-50f, 0f), new Rectangle(355, 86, 26, 26), state.color, 0f, new Vector2(13f), 1f * scale, SpriteEffects.None, 0.4f);

                b.Draw(MinigamesStart.minigameTextures[1], screenMid + new Vector2(50f, 0), new Rectangle(322, 82, 12, 12), state.color, 0f, new Vector2(6f), 2f * scale, SpriteEffects.None, 0.4f);
                b.Draw(Game1.mouseCursors, screenMid, new Rectangle(301, 288, 15, 15), state.color * 0.95f, 0f, new Vector2(7.5f, 7.5f), 2f * scale, SpriteEffects.None, 0.5f);
                b.DrawString(Game1.smallFont, "5", screenMid + new Vector2(0, 2f), state.color, 0f, Game1.smallFont.MeasureString("5") / 2f, 1f * scale, SpriteEffects.None, 0.51f);

                b.Draw(Game1.mouseCursors, screenMid + new Vector2(width * -0.6f, -45), new Rectangle(395, 497, 3, 8), state.color, 0f, new Vector2(1.5f, 4f), 4f, SpriteEffects.None, 0.98f);
                b.Draw(Game1.mouseCursors, screenMid + new Vector2(width * -0.6f, 0), new Rectangle(407, 1660, 10, 10), state.color, 0f, new Vector2(5f), 3.3f, SpriteEffects.None, 0.98f);
                b.Draw(Game1.mouseCursors, screenMid + new Vector2(width * -0.6f, 45), new Rectangle(473, 36, 24, 24), state.color, 0f, new Vector2(12f), 2f, SpriteEffects.None, 0.98f);
                b.DrawString(Game1.smallFont, "A", screenMid + new Vector2(width * -0.6f, 47) - (Game1.smallFont.MeasureString("A") / 2 * 1.2f), state.color, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 1f); //text
            }
            void Save()
            {
                if (state == null) return;
                config.MinigameColor = state.color;
            }

            GenericMC.AddSectionTitle(mod, () => ".   " + translate.Get("GenericMC.MinigameColor"));
            GenericMC.AddComplexOption(mod, () => "", () => "", Draw, Save, () => 300);
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            UpdateConfig(false);
        }
        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            minigame.Value.OnModMessageReceived(sender, e);
        }
        //private void OnWarped(object sender, WarpedEventArgs e)
        //{
        //    minigame.Value.OnWarped(sender, e);
        //}
        private void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e)  //this.Monitor.Log(locationName, LogLevel.Debug);
        {
            if (!Context.IsWorldReady) return;
            if (e.Pressed.Contains(SButton.F5))
            {
                UpdateConfig(false);
            }
            if (Game1.player.IsLocalPlayer) minigame.Value.Input_ButtonsChanged(sender, e);
        }

        private void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e) //adds item to inv
        {
            if (!Context.IsWorldReady || !Game1.player.IsLocalPlayer) return;

            if (minigame.Value == null)
            {
                minigame.Value = new Minigames(this);
            }

            try
            {
                minigame.Value.GameLoop_UpdateTicking(sender, e);
            }
            catch (Exception ex)
            {
                Monitor.Log("Handled Exception in UpdateTicking. Festival: " + Game1.isFestival() + ", Message: " + ex.Message + " in: " + exception.Match(ex.StackTrace).Value, LogLevel.Trace);
                minigame.Value.EmergencyCancel();
            }
        }

        private void Display_Rendered(object sender, RenderedEventArgs e)//festival 1
        {
            if (!Context.IsWorldReady) return;
            try
            {
                minigame.Value.Display_Rendered(e.SpriteBatch);
            }
            catch (Exception ex)
            {
                Monitor.Log("Handled Exception in Rendered. Festival: " + Game1.isFestival() + ", Message: " + ex.Message + " in: " + exception.Match(ex.StackTrace).Value, LogLevel.Trace);
                minigame.Value.EmergencyCancel();
            }
        }
        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)//regular
        {
            if (!Context.IsWorldReady) return;
            try
            {
                if (minigame.Value.fishingFestivalMinigame != 1) minigame.Value.Display_RenderedAll(e.SpriteBatch);
            }
            catch (Exception ex)
            {
                Monitor.Log("Handled Exception in RenderedWorld. Festival: " + Game1.isFestival() + ", Message: " + ex.Message + " in: " + exception.Match(ex.StackTrace).Value, LogLevel.Trace);
                minigame.Value.EmergencyCancel();
            }
        }


        private void Display_RenderingWorld(object sender, RenderingWorldEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            try
            {
                minigame.Value.Display_RenderingWorld(sender, e);
            }
            catch (Exception ex)
            {
                Monitor.Log("Handled Exception in RenderedWorld. Festival: " + Game1.isFestival() + ", Message: " + ex.Message + " in: " + exception.Match(ex.StackTrace).Value, LogLevel.Trace);
                minigame.Value.EmergencyCancel();
            }
        }


        /// <summary>Get whether this instance can edit the given asset.</summary>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (canStartEditingAssets && (asset.AssetNameEquals("TileSheets/tools") || asset.AssetNameEquals("Maps/springobjects") || asset.AssetNameEquals("Strings/StringsFromCSFiles") || asset.AssetNameEquals("Data/ObjectInformation"))) return true;
            return false;
        }
        /// <summary>Edits the asset if CanEdit</summary>
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Strings/StringsFromCSFiles"))
            {
                translate = Helper.Translation;
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                foreach (string itemID in data.Keys.ToArray())
                {
                    try
                    {
                        switch (itemID)
                        {
                            case "FishingRod.cs.14041":
                                data[itemID] = translate.Get("Rod.Fishing");
                                break;
                            case "FishingRod.cs.14042":
                                data[itemID] = translate.Get("Rod.FishingDesc");
                                break;
                            case "FishingRod.cs.trainingRodDescription":
                                data[itemID] = AddEffectDescriptions("Training Rod", translate.Get("Rod.TrainingDesc"));
                                break;
                            case "FishingRod.cs.14045":
                                data[itemID] = translate.Get("Bamboo Pole");
                                break;
                            case "FishingRod.cs.14046":
                                data[itemID] = translate.Get("Training Rod");
                                break;
                            case "FishingRod.cs.14047":
                                data[itemID] = translate.Get("Fiberglass Rod");
                                break;
                            case "FishingRod.cs.14048":
                                data[itemID] = translate.Get("Iridium Rod");
                                break;
                            case "SkillPage.cs.11598":
                                data[itemID] = translate.Get("Rod.Skill");
                                break;
                            case "FishingRod.cs.14083":
                                if (config.ConvertToMetric) data[itemID] = "{0} cm";
                                break;
                            default:
                                continue;
                        }
                    }
                    catch (Exception)
                    {
                        Monitor.Log("Could not load string for Rod to Net change, line: " + data[itemID] + ". Are the translations missing? Ignore if you removed them intentionally.", LogLevel.Warn);
                    }
                }

            }
            else if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                translate = Helper.Translation;
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                foreach (int itemID in data.Keys.ToArray())
                {
                    try
                    {
                        string[] itemData = data[itemID].Split('/');
                        switch (itemID)
                        {
                            //bait
                            case 685://bait
                            case 774://wild bait
                                itemData[5] = AddEffectDescriptions(itemData[0]);
                                break;
                            case 703://magnet
                            case 908://magic bait
                                itemData[5] = AddEffectDescriptions(itemData[0], itemData[5]);
                                break;
                            //tackle
                            case 686://spinner
                            case 687://dressed
                            case 694://trap
                            case 695://cork
                            case 692://lead
                            case 693://treasure
                            case 691://barbed
                            case 877://quality
                                itemData[4] = translate.Get(itemData[0]);
                                itemData[5] = AddEffectDescriptions(itemData[0]);
                                break;
                            case 856://curiosity
                                itemData[4] = translate.Get(itemData[0]);
                                itemData[5] = AddEffectDescriptions(itemData[0], itemData[5]);
                                break;
                            default:
                                continue;
                        }
                        data[itemID] = string.Join("/", itemData);
                        itemIDs[itemData[0]] = itemID;
                    }
                    catch (Exception)
                    {
                        Monitor.LogOnce("Could not load string for Rod to Net change, line: " + data[itemID] + ". Are the translations missing? Ignore if you removed them intentionally.", LogLevel.Warn);
                    }
                }
            }
            else
            {
                var editor = asset.AsImage();

                Texture2D sourceImage;
                try
                {
                    if (asset.AssetNameEquals("Maps/springobjects"))
                    {
                        sourceImage = Helper.Content.Load<Texture2D>("assets/bait_magnet.png", ContentSource.ModFolder);
                        editor.PatchImage(sourceImage, targetArea: new Rectangle(112, 464, 16, 16));
                        sourceImage = Helper.Content.Load<Texture2D>("assets/tackle_basic.png", ContentSource.ModFolder);
                        editor.PatchImage(sourceImage, targetArea: new Rectangle(304, 448, 80, 16));
                        sourceImage = Helper.Content.Load<Texture2D>("assets/tackle_curiosity.png", ContentSource.ModFolder);
                        editor.PatchImage(sourceImage, targetArea: new Rectangle(256, 560, 16, 16));
                        sourceImage = Helper.Content.Load<Texture2D>("assets/tackle_quality.png", ContentSource.ModFolder);
                        editor.PatchImage(sourceImage, targetArea: new Rectangle(208, 576, 16, 16));
                        sourceImage = Helper.Content.Load<Texture2D>("assets/tackle_spinners.png", ContentSource.ModFolder);
                        editor.PatchImage(sourceImage, targetArea: new Rectangle(224, 448, 32, 16));
                    }
                    else
                    {
                        sourceImage = Helper.Content.Load<Texture2D>("assets/rod_sprites.png", ContentSource.ModFolder);
                        editor.PatchImage(sourceImage, targetArea: new Rectangle(128, 0, 64, 16));
                        sourceImage = Helper.Content.Load<Texture2D>("assets/rod_farmer.png", ContentSource.ModFolder);
                        editor.PatchImage(sourceImage, targetArea: new Rectangle(0, 289, 295, 95));
                    }
                    sourceImage.Dispose();
                }
                catch (Exception)
                {
                    Monitor.Log("Could not load images for the " + ((asset.AssetNameEquals("Maps/springobjects")) ? "bait/tackles" : "fishing nets") + "! Are the assets missing? Ignore if you removed them intentionally.", LogLevel.Warn);
                }
            }
        }
        public static string AddEffectDescriptions(string itemName, string initialText = null)
        {
            foreach (var effect in config.SeeInfoForBelowData[itemName])
            {
                if (effect.Value == 0) continue;
                if (!effect.Key.StartsWith("EXTRA_", StringComparison.Ordinal))
                {
                    if (initialText != null) initialText += "\n";
                    initialText += translate.Get("Effects." + effect.Key).Tokens(new { val = effect.Value });
                }
            }
            if (config.SeeInfoForBelowData[itemName].TryGetValue("EXTRA_MAX", out int max) && config.SeeInfoForBelowData[itemName].TryGetValue("EXTRA_CHANCE", out int chance) && max != 0 && chance != 0)
            {
                if (initialText != null) initialText += "\n";
                initialText += translate.Get("Effects.EXTRA").Tokens(new { max, chance });
            }
            return initialText;
        }


        private void StartMinigameTest(string commandname, string[] args)
        {
            minigame.Value.DebugConsoleStartMinigameTest(args);
        }

        private void UpdateConfig(bool GMCM)
        {
            if (!GMCM) config = Helper.ReadConfig<ModConfig>();

            int fix = 0;
            foreach (var item in config.SeeInfoForBelowData.ToArray())//updates old configs to new format
            {
                foreach (var effect in item.Value.ToArray())
                {
                    if (effect.Key.Equals("DOUBLE", StringComparison.Ordinal))
                    {
                        config.SeeInfoForBelowData[item.Key].Remove(effect.Key);
                        if (item.Key.Equals("Wild Bait", StringComparison.Ordinal))
                        {
                            config.SeeInfoForBelowData[item.Key]["EXTRA_MAX"] = 2;
                            config.SeeInfoForBelowData[item.Key]["EXTRA_CHANCE"] = 20;
                        }
                        else
                        {
                            config.SeeInfoForBelowData[item.Key]["EXTRA_MAX"] = 0;
                            config.SeeInfoForBelowData[item.Key]["EXTRA_CHANCE"] = 0;
                        }
                        fix++;
                    }
                }
            }
            if (fix > 0) Helper.WriteConfig(config);

            Minigames.itemData = config.SeeInfoForBelowData;

            if (Minigames.voices == null)
            {
                Minigames.voices = new Dictionary<string, SoundEffect>();
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(Path.Combine(Helper.DirectoryPath, "assets/audio"));

                    if (!dir.Exists) throw new DirectoryNotFoundException();

                    FileInfo[] files = dir.GetFiles("*.wav");

                    foreach (FileInfo file in files)
                    {
                        Minigames.voices[Path.GetFileNameWithoutExtension(file.Name)] = SoundEffect.FromStream(file.Open(FileMode.Open));
                    }
                    //Minigames.fishySound["Mute"] = SoundEffect.FromStream(new FileStream(Path.Combine(Helper.DirectoryPath, "assets/audio", "Mute.wav"), FileMode.Open));
                }
                catch (Exception ex)
                {
                    Monitor.Log($"error loading audio: {ex}", LogLevel.Error);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (!config.Voice_Test_Ignore_Me[i].Equals(config.VoiceVolume + "/" + config.VoiceType[i] + "/" + config.VoicePitch[i], StringComparison.Ordinal)) //play voice and save it if changed
                {
                    config.Voice_Test_Ignore_Me[i] = config.VoiceVolume + "/" + config.VoiceType[i] + "/" + config.VoicePitch[i];
                    if (Minigames.voices.TryGetValue(config.VoiceType[i], out SoundEffect sfx)) sfx.Play(config.VoiceVolume / 100f * 0.98f, config.VoicePitch[i] / 100f, 0f);
                    Helper.WriteConfig(config);
                }

                try //keybinds
                {
                    if (config.KeyBinds.Equals("") || config.KeyBinds.Equals(" ")) throw new FormatException("String can't be empty.");
                    Minigames.keyBinds[i] = KeybindList.Parse(config.KeyBinds[i]);
                }
                catch (Exception e)
                {
                    string def = "MouseLeft, C, ControllerX";
                    Minigames.keyBinds[i] = KeybindList.Parse(def);
                    config.KeyBinds[i] = def;
                    Helper.WriteConfig(config);
                    Monitor.Log(e.Message + " Resetting KeyBinds for screen " + (i + 1) + " to default. For key names, see: https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings", LogLevel.Error);
                }

                Minigames.voicePitch[i] = config.VoicePitch[i] / 100f;
            }
            if (Context.IsWorldReady)
            {
                Minigames.voiceVolume = config.VoiceVolume / 100f;
                Minigames.voiceType = config.VoiceType;
                Minigames.freeAim = config.FreeAim;
                Minigames.startMinigameStyle = config.StartMinigameStyle;
                Minigames.endMinigameStyle = config.EndMinigameStyle;
                Minigames.endCanLoseTreasure = config.EndLoseTreasureIfFailed;
                Minigames.minigameDamage = config.EndMinigameDamage;
                Minigames.minigameDifficulty = config.MinigameDifficulty;
                Minigames.festivalMode = config.FestivalMode;
                Minigames.realisticSizes = config.RealisticSizes;
                Minigames.fishTankSprites = config.FishTankHoldSprites;
                Minigames.minigameColor = config.MinigameColor;
                MinigamesStart.minigameStyle = config.StartMinigameStyle;
                MinigamesStart.minigameColor = config.MinigameColor;
                MinigamesStart.minigameDifficulty = config.MinigameDifficulty;
                MinigamesStart.startMinigameScale = config.StartMinigameScale;
                MinigamesStart.tutorialSkip = config.TutorialSkip;
                MinigamesStart.bossTransparency = config.BossTransparency;
                if (LocalizedContentManager.CurrentLanguageCode == 0 && Minigames.metricSizes != config.ConvertToMetric)
                {
                    Minigames.metricSizes = config.ConvertToMetric;
                    Helper.Content.InvalidateCache("Strings/StringsFromCSFiles");
                }
                Helper.Content.InvalidateCache("Data/ObjectInformation");
            }
        }
    }
}
