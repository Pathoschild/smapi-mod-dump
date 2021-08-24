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
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FishingMinigames
{
    public class ModEntry : Mod, IAssetEditor
    {
        public static ITranslationHelper translate;
        public static ModConfig config;
        private readonly PerScreen<Minigames> minigame = new PerScreen<Minigames>();
        private Dictionary<string, string> displayNames = new Dictionary<string, string>();
        private bool canStartEditingAssets = false;



        public override void Entry(IModHelper helper)
        {
            translate = Helper.Translation;
            UpdateConfig();
            Minigames.startMinigameTextures = new Texture2D[] {
                Game1.content.Load<Texture2D>("LooseSprites\\boardGameBorder"),
                Game1.content.Load<Texture2D>("LooseSprites\\CraneGame"),
                Game1.content.Load<Texture2D>("LooseSprites\\buildingPlacementTiles") };

            helper.Events.Display.Rendered += Display_Rendered;
            helper.Events.Display.RenderedWorld += Display_RenderedWorld;
            helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
            helper.Events.GameLoop.GameLaunched += GenericModConfigMenuIntegration;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;

            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);//this might summon Cthulhu
            harmony.PatchAll();
        }


        private void GenericModConfigMenuIntegration(object sender, GameLaunchedEventArgs e)     //Generic Mod Config Menu API
        {
            if (Context.IsSplitScreen) return;
            canStartEditingAssets = true;
            Helper.Content.InvalidateCache("Strings/StringsFromCSFiles");
            Helper.Content.InvalidateCache("Data/ObjectInformation");

            var GenericMC = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (GenericMC != null)
            {
                GenericMC.RegisterModConfig(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
                GenericMC.SetDefaultIngameOptinValue(ModManifest, true);
                GenericMC.RegisterLabel(ModManifest, translate.Get("GenericMC.MainLabel"), ""); //All of these strings are stored in the traslation files.
                GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.MainDesc"));
                GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.MainDesc2"));
                if (Constants.TargetPlatform != GamePlatform.Android) GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.MainDescPC"));
                else GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.MainDescOther"));
                GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.MainDesc3"));

                try
                {
                    GenericMC.RegisterClampedOption(ModManifest, translate.Get("GenericMC.Volume"), translate.Get("GenericMC.VolumeDesc"),
                        () => config.VoiceVolume, (int val) => config.VoiceVolume = val, 0, 100);

                    GenericMCPerScreen(GenericMC, 0);
                    GenericMC.RegisterPageLabel(ModManifest, translate.Get("GenericMC.Colors"), translate.Get("GenericMC.Colors"), translate.Get("GenericMC.Colors"));
                    GenericMC.RegisterPageLabel(ModManifest, translate.Get("GenericMC.ItemData"), translate.Get("GenericMC.ItemData"), translate.Get("GenericMC.ItemData"));

                    for (int i = 2; i < 5; i++)
                    {
                        GenericMC.RegisterPageLabel(ModManifest, translate.Get("GenericMC.SplitScreen" + i), translate.Get("GenericMC.SplitScreenDesc"), translate.Get("GenericMC.SplitScreen" + i));
                    }

                    GenericMCPerScreen(GenericMC, 1);
                    GenericMCPerScreen(GenericMC, 2);
                    GenericMCPerScreen(GenericMC, 3);

                    GenericMC.StartNewPage(ModManifest, translate.Get("GenericMC.Colors"));
                    GenericMCColorPicker(GenericMC, ModManifest, translate.Get("GenericMC.MinigameColor"), "");

                    GenericMC.StartNewPage(ModManifest, translate.Get("GenericMC.ItemData"));
                    GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.ItemDataDesc1"));
                    GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.ItemDataDesc2"));
                    GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.ItemDataDesc3"));
                    GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.ItemDataDesc4"));

                    foreach (var item in config.SeeInfoForBelowData)
                    {
                        GenericMC.RegisterLabel(ModManifest, displayNames[item.Key], item.Key);
                        foreach (var effect in item.Value)
                        {
                            GenericMC.RegisterClampedOption(ModManifest, effect.Key, translate.Get("Effects." + effect.Key).Tokens(new { val = "X" }),
                                () => config.SeeInfoForBelowData[item.Key][effect.Key], (float val) => config.SeeInfoForBelowData[item.Key][effect.Key] = (int)Math.Round(val),
                                (effect.Key.Equals("QUALITY", StringComparison.Ordinal) ? -4 : effect.Key.Equals("LIFE", StringComparison.Ordinal) ? 0 : -100),
                                (effect.Key.Equals("QUALITY", StringComparison.Ordinal) ? 4 : effect.Key.Equals("LIFE", StringComparison.Ordinal) ? 50 : 300));
                        }
                    }
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
                GenericMC.StartNewPage(ModManifest, translate.Get("GenericMC.SplitScreen" + (screen + 1)));
            }
            GenericMC.RegisterClampedOption(ModManifest, translate.Get("GenericMC.Pitch"), translate.Get("GenericMC.PitchDesc"),
                () => config.VoicePitch[screen], (int val) => config.VoicePitch[screen] = val, -100, 100);

            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.KeyBinds"), translate.Get("GenericMC.KeyBindsDesc"),
                () => config.KeyBinds[screen], (string val) => config.KeyBinds[screen] = val);

            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.FreeAim"), translate.Get("GenericMC.FreeAimDesc"),
                () => config.FreeAim[screen], (bool val) => config.FreeAim[screen] = val);

            GenericMC.RegisterChoiceOption(ModManifest, translate.Get("GenericMC.StartMinigameStyle"), translate.Get("GenericMC.StartMinigameStyleDesc"),
                () => (config.StartMinigameStyle[screen] == 0) ? translate.Get("GenericMC.Disabled") : (config.StartMinigameStyle[screen] == 1) ? translate.Get("GenericMC.StartMinigameStyle1") : (config.StartMinigameStyle[screen] == 2) ? translate.Get("GenericMC.StartMinigameStyle2") : translate.Get("GenericMC.StartMinigameStyle3"),
                (string val) => config.StartMinigameStyle[screen] = Int32.Parse((val.Equals(translate.Get("GenericMC.Disabled"), StringComparison.Ordinal)) ? "0" : (val.Equals(translate.Get("GenericMC.StartMinigameStyle1"), StringComparison.Ordinal)) ? "1" : (val.Equals(translate.Get("GenericMC.StartMinigameStyle2"), StringComparison.Ordinal)) ? "2" : "3"),
                new string[] { translate.Get("GenericMC.Disabled"), translate.Get("GenericMC.StartMinigameStyle1") });//, translate.Get("GenericMC.StartMinigameStyle2"), translate.Get("GenericMC.StartMinigameStyle3") });//small 'hack' so options appear as name strings, while config.json stores them as integers

            GenericMC.RegisterChoiceOption(ModManifest, translate.Get("GenericMC.EndMinigameStyle"), translate.Get("GenericMC.EndMinigameStyleDesc"),
                () => (config.EndMinigameStyle[screen] == 0) ? translate.Get("GenericMC.Disabled") : (config.EndMinigameStyle[screen] == 1) ? translate.Get("GenericMC.EndMinigameStyle1") : (config.EndMinigameStyle[screen] == 2) ? translate.Get("GenericMC.EndMinigameStyle2") : translate.Get("GenericMC.EndMinigameStyle3"),
                (string val) => config.EndMinigameStyle[screen] = Int32.Parse((val.Equals(translate.Get("GenericMC.Disabled"), StringComparison.Ordinal)) ? "0" : (val.Equals(translate.Get("GenericMC.EndMinigameStyle1"), StringComparison.Ordinal)) ? "1" : (val.Equals(translate.Get("GenericMC.EndMinigameStyle2"), StringComparison.Ordinal)) ? "2" : "3"),
                new string[] { translate.Get("GenericMC.Disabled"), translate.Get("GenericMC.EndMinigameStyle1"), translate.Get("GenericMC.EndMinigameStyle2"), translate.Get("GenericMC.EndMinigameStyle3") });

            GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.EndLoseTreasure"), translate.Get("GenericMC.EndLoseTreasureDesc"),
                () => config.EndLoseTreasureIfFailed[screen], (bool val) => config.EndLoseTreasureIfFailed[screen] = val);
            GenericMC.RegisterClampedOption(ModManifest, translate.Get("GenericMC.EndDamage"), translate.Get("GenericMC.EndDamageDesc"),
                () => config.EndMinigameDamage[screen], (float val) => config.EndMinigameDamage[screen] = val, 0f, 2f);
            GenericMC.RegisterClampedOption(ModManifest, translate.Get("GenericMC.Difficulty"), translate.Get("GenericMC.DifficultyDesc"),
                () => config.MinigameDifficulty[screen], (float val) => config.MinigameDifficulty[screen] = val, 0.1f, 2f);

            if (screen == 0)//only page 0
            {
                GenericMC.RegisterClampedOption(ModManifest, translate.Get("GenericMC.StartMinigameScale"), translate.Get("GenericMC.StartMinigameScale"),
                    () => config.StartMinigameScale, (float val) => config.StartMinigameScale = val, 0.5f, 5f);

                GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.RealisticSizes"), translate.Get("GenericMC.RealisticSizesDesc"),
                    () => config.RealisticSizes, (bool val) => config.RealisticSizes = val);

                if (LocalizedContentManager.CurrentLanguageCode == 0) GenericMC.RegisterSimpleOption(ModManifest, translate.Get("GenericMC.ConvertToMetric"), translate.Get("GenericMC.ConvertToMetricDesc"),
                    () => config.ConvertToMetric, (bool val) => config.ConvertToMetric = val);

                GenericMC.RegisterLabel(ModManifest, translate.Get("GenericMC.FestivalLabel"), "");
                GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.FestivalDesc"));
                GenericMC.RegisterParagraph(ModManifest, translate.Get("GenericMC.FestivalDesc2"));
            }
            GenericMC.RegisterChoiceOption(ModManifest, translate.Get("GenericMC.FestivalMode"), translate.Get("GenericMC.FestivalModeDesc"),
                () => (config.FestivalMode[screen] == 0) ? translate.Get("GenericMC.FestivalModeVanilla") : (config.FestivalMode[screen] == 1) ? translate.Get("GenericMC.FestivalModeSimple") : (config.FestivalMode[screen] == 2) ? translate.Get("GenericMC.FestivalModePerfectOnly") : translate.Get("GenericMC.FestivalModeStartOnly"),
                (string val) => config.FestivalMode[screen] = Int32.Parse((val.Equals(translate.Get("GenericMC.FestivalModeVanilla"), StringComparison.Ordinal)) ? "0" : (val.Equals(translate.Get("GenericMC.FestivalModeSimple"), StringComparison.Ordinal)) ? "1" : (val.Equals(translate.Get("GenericMC.FestivalModePerfectOnly"), StringComparison.Ordinal)) ? "2" : "3"),
                new string[] { translate.Get("GenericMC.FestivalModeVanilla"), translate.Get("GenericMC.FestivalModeSimple"), translate.Get("GenericMC.FestivalModePerfectOnly"), translate.Get("GenericMC.FestivalModeStartOnly") });
        }
        private void GenericMCColorPicker(IGenericModConfigMenuApi GenericMC, IManifest mod, string optionName, string optionDesc)
        {
            Func<Vector2, object, object> colorPickerUpdate =
                (Vector2 pos, object state_) =>
                {
                    var state = state_ as MinigameColor;
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


                    return state;
                };
            Func<SpriteBatch, Vector2, object, object> colorPickerDraw =
                (SpriteBatch b, Vector2 pos, object state_) =>
                {
                    var state = state_ as MinigameColor;
                    int width = Math.Min(Game1.uiViewport.Width / 4, 400);
                    float scale = width / 400f;

                    Rectangle barR = new Rectangle((int)pos.X, (int)pos.Y, width, 24);
                    Rectangle barG = new Rectangle((int)pos.X, (int)pos.Y + 80, width, 24);
                    Rectangle barB = new Rectangle((int)pos.X, (int)pos.Y + 160, width, 24);
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
                    b.Draw(Minigames.startMinigameTextures[0], screenMid, null, state.color, 0f, new Vector2(69f, 37f), 2f * scale, SpriteEffects.None, 0.3f);
                    b.Draw(Minigames.startMinigameTextures[1], screenMid + new Vector2(-50f, 0f), new Rectangle(355, 86, 26, 26), state.color, 0f, new Vector2(13f), 1f * scale, SpriteEffects.None, 0.4f);

                    b.Draw(Minigames.startMinigameTextures[1], screenMid + new Vector2(50f, 0), new Rectangle(322, 82, 12, 12), state.color, 0f, new Vector2(6f), 2f * scale, SpriteEffects.None, 0.4f);
                    b.Draw(Game1.mouseCursors, screenMid, new Rectangle(301, 288, 15, 15), state.color * 0.95f, 0f, new Vector2(7.5f, 7.5f), 2f * scale, SpriteEffects.None, 0.5f);
                    b.DrawString(Game1.smallFont, "5", screenMid, state.color, 0f, Game1.smallFont.MeasureString("5") / 2f, 1f * scale, SpriteEffects.None, 0.51f);
                    return state;
                };
            Action<object> colorPickerSave =
                (object state) =>
                {
                    if (state == null) return;
                    config.MinigameColor = (state as MinigameColor).color;
                };

            GenericMC.RegisterLabel(mod, ".   " + optionName, optionDesc);
            GenericMC.RegisterComplexOption(mod, "", "", colorPickerUpdate, colorPickerDraw, colorPickerSave);
            GenericMC.RegisterLabel(mod, ".", "");
            GenericMC.RegisterLabel(mod, ".", "");
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            UpdateConfig();
        }
        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            minigame.Value.OnModMessageReceived(sender, e);
        }

        private void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e)  //this.Monitor.Log(locationName, LogLevel.Debug);
        {
            if (e.Pressed.Contains(SButton.F5))
            {
                UpdateConfig();
            }
            if (Game1.player.IsLocalPlayer) minigame.Value.Input_ButtonsChanged(sender, e);
        }

        private void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e) //adds item to inv
        {
            if (!Game1.player.IsLocalPlayer) return;

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
                Monitor.Log("Handled Exception in UpdateTicking. Festival: " + Game1.isFestival() + ", Message: " + ex.Message, LogLevel.Trace);
                minigame.Value.EmergencyCancel();
            }
        }

        private void Display_Rendered(object sender, RenderedEventArgs e)//festival 1
        {
            try
            {
                if (minigame.Value.fishingFestivalMinigame == 1) minigame.Value.Display_RenderedAll(e.SpriteBatch);
            }
            catch (Exception ex)
            {
                Monitor.Log("Handled Exception in Rendered. Festival: " + Game1.isFestival() + ", Message: " + ex.Message, LogLevel.Trace);
                minigame.Value.EmergencyCancel();
            }
        }
        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)//regular
        {
            try
            {
                if (minigame.Value.fishingFestivalMinigame != 1) minigame.Value.Display_RenderedAll(e.SpriteBatch);
            }
            catch (Exception ex)
            {
                Monitor.Log("Handled Exception in RenderedWorld. Festival: " + Game1.isFestival() + ", Message: " + ex.Message, LogLevel.Trace);
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
                                displayNames["Bamboo Pole"] = data[itemID];
                                break;
                            case "FishingRod.cs.14046":
                                data[itemID] = translate.Get("Training Rod");
                                displayNames["Training Rod"] = data[itemID];
                                break;
                            case "FishingRod.cs.14047":
                                data[itemID] = translate.Get("Fiberglass Rod");
                                displayNames["Fiberglass Rod"] = data[itemID];
                                break;
                            case "FishingRod.cs.14048":
                                data[itemID] = translate.Get("Iridium Rod");
                                displayNames["Iridium Rod"] = data[itemID];
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
                                itemData[5] = AddEffectDescriptions(itemData[0]);
                                break;
                            case 703://magnet
                            case 774://wild bait
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
                        displayNames[itemData[0]] = translate.Get(itemData[0]);
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
                if (initialText != null) initialText += "\n";
                initialText += translate.Get("Effects." + effect.Key).Tokens(new { val = effect.Value });
            }
            return initialText;
        }


        private void UpdateConfig()
        {
            config = Helper.ReadConfig<ModConfig>();

            //item configs
            //Minigames.itemData = new Dictionary<string, string[]>();
            //foreach (var file in Directory.GetFiles(Helper.DirectoryPath + "/itemConfigs", "*.json").Select(Path.GetFileName).OrderBy(f => f))
            //{
            //    (Helper.Data.ReadJsonFile<Dictionary<string, string[]>>("itemConfigs/" + file) ?? new Dictionary<string, string[]>()).ToList().ForEach(x => Minigames.itemData[x.Key] = x.Value);
            //}
            Minigames.itemData = config.SeeInfoForBelowData;


            try
            {
                Minigames.fishySound = SoundEffect.FromStream(new FileStream(Path.Combine(Helper.DirectoryPath, "assets", "fishy.wav"), FileMode.Open));
            }
            catch (Exception ex)
            {
                Monitor.Log($"error loading fishy.wav: {ex}", LogLevel.Error);
            }

            for (int i = 0; i < 4; i++)
            {
                if (!config.Voice_Test_Ignore_Me[i].Equals(config.VoiceVolume + "/" + config.VoicePitch[i], StringComparison.Ordinal)) //play voice and save it if changed
                {
                    config.Voice_Test_Ignore_Me[i] = config.VoiceVolume + "/" + config.VoicePitch[i];
                    Minigames.fishySound.Play(config.VoiceVolume / 100f * 0.98f, config.VoicePitch[i] / 100f, 0f);
                    Helper.WriteConfig(config);
                }

                try //keybinds
                {
                    if (config.KeyBinds.Equals("") || config.KeyBinds.Equals(" ")) throw new FormatException("String can't be empty.");
                    Minigames.keyBinds[i] = KeybindList.Parse(config.KeyBinds[i]);
                }
                catch (Exception e)
                {
                    string def = "MouseLeft, Space, ControllerX";
                    Minigames.keyBinds[i] = KeybindList.Parse(def);
                    config.KeyBinds[i] = def;
                    Helper.WriteConfig(config);
                    Monitor.Log(e.Message + " Resetting KeyBinds for screen " + (i + 1) + " to default. For key names, see: https://stardewcommunitywiki.com/Modding:Player_Guide/Key_Bindings", LogLevel.Error);
                }

                Minigames.voicePitch[i] = config.VoicePitch[i] / 100f;

                if (Context.IsWorldReady)
                {
                    Minigames.freeAim[i] = config.FreeAim[i];
                    Minigames.startMinigameStyle[i] = config.StartMinigameStyle[i];
                    Minigames.endMinigameStyle[i] = config.EndMinigameStyle[i];
                    Minigames.endCanLoseTreasure[i] = config.EndLoseTreasureIfFailed[i];
                    Minigames.minigameDamage[i] = config.EndMinigameDamage[i];
                    Minigames.minigameDifficulty[i] = config.MinigameDifficulty[i];
                    Minigames.festivalMode[i] = config.FestivalMode[i];
                }
            }
            if (Context.IsWorldReady)
            {
                Minigames.voiceVolume = config.VoiceVolume / 100f;
                Minigames.startMinigameScale = config.StartMinigameScale;
                Minigames.realisticSizes = config.RealisticSizes;
                Minigames.minigameColor = config.MinigameColor;
                if (LocalizedContentManager.CurrentLanguageCode == 0) Minigames.metricSizes = config.ConvertToMetric;
                else Minigames.metricSizes = false;
                Helper.Content.InvalidateCache("Strings/StringsFromCSFiles");
            }
        }
    }
}



[HarmonyPatch(typeof(Tool), "getDescription")]
class Patch
{
    static void Postfix(ref string __result, ref Tool __instance)
    {
        if (__instance is StardewValley.Tools.FishingRod && __instance.UpgradeLevel != 1)//bamboo+ (except training)
        {
            string desc = Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14042");

            desc += "\n" + FishingMinigames.ModEntry.AddEffectDescriptions(__instance.Name);

            if (__instance.UpgradeLevel > 1)//fiber/iridium
            {
                if (__instance.attachments[0] != null)
                {
                    desc += "\n\n" + __instance.attachments[0].DisplayName + ((__instance.attachments[0].quality == 0) ? "" : " (" + FishingMinigames.ModEntry.translate.Get("Mods.Infinite") + ")")
                           + ":\n" + FishingMinigames.ModEntry.AddEffectDescriptions(__instance.attachments[0].Name);
                }
                if (__instance.attachments[1] != null)
                {
                    desc += "\n\n" + __instance.attachments[1].DisplayName + ((__instance.attachments[1].quality == 0) ? "" : " (" + FishingMinigames.ModEntry.translate.Get("Mods.Infinite") + ")")
                           + ":\n" + FishingMinigames.ModEntry.AddEffectDescriptions(__instance.attachments[1].Name);
                }
            }
            if (desc.EndsWith("\n")) desc = desc.Substring(0, desc.Length - 1);
            __result = Game1.parseText(desc, Game1.smallFont, desc.Length * 10);
        }
    }
}

class MinigameColor
{
    public Color color;
    public Vector2 pos = new Vector2(0f);
    public int whichSlider = 0;
}
class DummyMenu : StardewValley.Menus.IClickableMenu
{
    public DummyMenu()
    {
        //this is just to prevent other mods from interfering with minigames
    }
}