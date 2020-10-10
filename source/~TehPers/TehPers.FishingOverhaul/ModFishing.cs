/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Linq;
using System.Text;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;
using StardewValley.Tools;
using TehPers.Core.Api.Weighted;
using TehPers.Core.Gui;
using TehPers.Core.Gui.Base.Units;
using TehPers.Core.Gui.SDV.Components;
using TehPers.Core.Gui.SDV.Units;
using TehPers.Core.Helpers.Static;
using TehPers.Core.Json;
using TehPers.Core.Rewrite;
using TehPers.FishingOverhaul.Configs;
using TehPers.FishingOverhaul.Patches;
using SObject = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TehPers.FishingOverhaul {
    public class ModFishing : Mod {
        public static ModFishing Instance { get; private set; }

        public FishingApi Api { get; private set; }
        public ConfigMain MainConfig { get; private set; }
        public ConfigFish FishConfig { get; private set; }
        public ConfigFishTraits FishTraitsConfig { get; private set; }
        public ConfigTreasure TreasureConfig { get; private set; }

        internal FishingRodOverrider Overrider { get; set; }
        internal HarmonyInstance Harmony { get; private set; }

        public override void Entry(IModHelper helper) {
            ModFishing.Instance = this;
            this.Api = new FishingApi();
            //TehMultiplayerApi.GetApi(this).RegisterItem(Objects.Coal, new FishingRodManager());

            // Make sure TehPers.Core isn't loaded as it's not needed anymore
            if (helper.ModRegistry.IsLoaded("TehPers.Core")) {
                this.Monitor.Log("Delete TehCore, it's not needed anymore. Your game will probably crash with it installed anyway.", LogLevel.Error);
            }

            // Load the configs
            this.LoadConfigs();

            // Make sure this mod is enabled
            if (!this.MainConfig.ModEnabled) {
                return;
            }

            // Apply patches
            this.Harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            Type targetType = AssortedHelpers.GetSDVType(nameof(NetAudio));
            this.Harmony.Patch(targetType.GetMethod(nameof(NetAudio.PlayLocal)), new HarmonyMethod(typeof(NetAudioPatches).GetMethod(nameof(NetAudioPatches.Prefix))), null);

            // Override fishing
            this.Overrider = new FishingRodOverrider(this);

            // Events
            this.Helper.Events.Display.RenderedHud += this.PostRenderHud;
            this.Helper.Events.Input.ButtonPressed += (sender, e) => {
                if (e.Button != SButton.F5)
                    return;

                this.Monitor.Log("Reloading configs...", LogLevel.Info);
                this.LoadConfigs();
                this.Monitor.Log("Done", LogLevel.Info);
            };

            // Trash data
            this.Api.AddTrashData(new DefaultTrashData());
            this.Api.AddTrashData(new SpecificTrashData(new[] { 152 }, 1, "Beach")); // Seaweed
            this.Api.AddTrashData(new SpecificTrashData(new[] { 153 }, 1, "Farm", invertLocations: true)); // Green Algae
            this.Api.AddTrashData(new SpecificTrashData(new[] { 157 }, 1, "BugLand")); // White Algae
            this.Api.AddTrashData(new SpecificTrashData(new[] { 157 }, 1, "Sewers")); // White Algae
            this.Api.AddTrashData(new SpecificTrashData(new[] { 157 }, 1, "WitchSwamp")); // White Algae
            this.Api.AddTrashData(new SpecificTrashData(new[] { 157 }, 1, "UndergroundMines")); // White Algae
            this.Api.AddTrashData(new SpecificTrashData(new[] { 797 }, 0.01D, "Submarine")); // Pearl
            this.Api.AddTrashData(new SpecificTrashData(new[] { 152 }, 0.99D, "Submarine")); // Seaweed

            // this.LoadGuis();

            /*ControlEvents.KeyPressed += (sender, pressed) => {
                if (pressed.KeyPressed == Keys.NumPad7) {
                    Menu menu = new Menu(Game1.viewport.Width / 6, Game1.viewport.Height / 6, 2 * Game1.viewport.Width / 3, 2 * Game1.viewport.Height / 3);

                    menu.MainElement.AddChild(new TextElement {
                        Text = "Test Menu",
                        Color = Color.Black,
                        Size = new BoxVector(0, 50, 1F, 0F),
                        Scale = new Vector2(3, 3),
                        HorizontalAlignment = Alignment.MIDDLE,
                        VerticalAlignment = Alignment.TOP
                    });

                    menu.MainElement.AddChild(new TextboxElement {
                        Location = new BoxVector(0, 100, 0, 0)
                    });

                    Game1.activeClickableMenu = menu;
                }
            };*/
        }

        public override object GetApi() {
            return this.Api;
        }

        private void LoadConfigs() {
            IJsonApi jsonApi = this.GetCoreApi().GetJsonApi();

            // Load configs
            this.MainConfig = jsonApi.ReadOrCreate<ConfigMain>("config.json");
            this.TreasureConfig = jsonApi.ReadOrCreate<ConfigTreasure>("treasure.json", this.MainConfig.MinifyConfigs);
            this.FishConfig = jsonApi.ReadOrCreate("fish.json", () => {
                // Populate fish data
                ConfigFish config = new ConfigFish();
                config.PopulateData();
                return config;
            }, this.MainConfig.MinifyConfigs);
            this.FishTraitsConfig = jsonApi.ReadOrCreate("fishTraits.json", () => {
                // Populate fish traits data
                ConfigFishTraits config = new ConfigFishTraits();
                config.PopulateData();
                return config;
            }, this.MainConfig.MinifyConfigs);

            // Load config values
            FishingRod.maxTackleUses = ModFishing.Instance.MainConfig.DifficultySettings.MaxTackleUses;
        }

        private void LoadGuis() {
            IGuiApi guiApi = this.GetCoreApi().GetGuiApi();

            this.Helper.Events.Input.ButtonReleased += (sender, pressed) => {
                if (pressed.Button != SButton.Y) {
                    return;
                }

                MenuComponent menu = new MenuComponent {
                    Location = GuiVectors.Centered,
                    Size = new ResponsiveVector2<GuiInfo>(new PercentParentUnit(0.75f), new PercentParentUnit(0.75f))
                };

                menu.AddChild(new LabelComponent(menu) {
                    Location = new ResponsiveVector2<GuiInfo>(GuiVectors.Centered.X, GuiVectors.SameAsParent.Y),
                    Scale = Vector2.One * 2f,
                    Text = "Internet Exploder 11"
                });

                Game1.activeClickableMenu = guiApi.ConvertMenu(menu);
            };
        }

        #region Events
        private void PostRenderHud(object sender, EventArgs eventArgs) {
            if (!this.MainConfig.ShowFishingData || Game1.eventUp || !(Game1.player.CurrentTool is FishingRod rod))
                return;

            Color textColor = Color.White;
            SpriteFont font = Game1.smallFont;

            // Draw the fishing GUI to the screen
            float boxWidth = 0;
            float lineHeight = font.LineSpacing;
            Vector2 boxTopLeft = new Vector2(this.MainConfig.HudTopLeftX, this.MainConfig.HudTopLeftY);
            Vector2 boxBottomLeft = boxTopLeft;

            // Setup the sprite batch
            SpriteBatch batch = Game1.spriteBatch;
            batch.End();
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // Draw streak
            string streakText = ModFishing.Translate("text.streak", this.Api.GetStreak(Game1.player));
            batch.DrawStringWithShadow(font, streakText, boxBottomLeft, textColor, 1f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(streakText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            // Get info on all the possible fish
            IWeightedElement<int?>[] possibleFish = this.Api.GetPossibleFish(Game1.player).Where(e => e.Value != null).ToArray();
            double fishChance = possibleFish.SumWeights();

            // Limit the number of displayed fish
            int trimmed = possibleFish.Length - 5;
            if (trimmed > 1) {
                possibleFish = possibleFish.Take(5).ToArray();
            }

            // Draw treasure chance
            string treasureText = ModFishing.Translate("text.treasure", ModFishing.Translate("text.percent", this.Api.GetTreasureChance(Game1.player, rod)));
            batch.DrawStringWithShadow(font, treasureText, boxBottomLeft, textColor, 1f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(treasureText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            // Draw trash chance
            string trashText = ModFishing.Translate("text.trash", ModFishing.Translate("text.percent", 1f - fishChance));
            batch.DrawStringWithShadow(font, trashText, boxBottomLeft, textColor, 1f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(trashText).X);
            boxBottomLeft += new Vector2(0, lineHeight);

            if (possibleFish.Any()) {
                // Draw info for each fish
                const float iconScale = Game1.pixelZoom / 2f;
                foreach (IWeightedElement<int?> fishData in possibleFish) {
                    // Skip trash
                    if (fishData.Value == null)
                        continue;

                    // Get fish ID
                    int fish = fishData.Value.Value;

                    // Don't draw hidden fish
                    if (this.Api.IsHidden(fish))
                        continue;

                    // Draw fish icon
                    Rectangle source = GameLocation.getSourceRectForObject(fish);
                    batch.Draw(Game1.objectSpriteSheet, boxBottomLeft, source, Color.White, 0.0f, Vector2.Zero, iconScale, SpriteEffects.None, 1F);
                    lineHeight = Math.Max(lineHeight, source.Height * iconScale);

                    // Draw fish information
                    string chanceText = ModFishing.Translate("text.percent", fishData.GetWeight());
                    string fishText = $"{this.Api.GetFishName(fish)} - {chanceText}";
                    batch.DrawStringWithShadow(font, fishText, boxBottomLeft + new Vector2(source.Width * iconScale, 0), textColor, 1F);
                    boxWidth = Math.Max(boxWidth, font.MeasureString(fishText).X + source.Width * iconScale);

                    // Update destY
                    boxBottomLeft += new Vector2(0, lineHeight);
                }
            }

            if (trimmed > 0) {
                batch.DrawStringWithShadow(font, $"+{trimmed}...", boxBottomLeft, textColor, 1f);
                boxBottomLeft += new Vector2(0, lineHeight);
            }

            // Draw the background rectangle
            batch.Draw(DrawHelpers.WhitePixel, new Rectangle((int) boxTopLeft.X, (int) boxTopLeft.Y, (int) boxWidth, (int) boxBottomLeft.Y), null, new Color(0, 0, 0, 0.25F), 0f, Vector2.Zero, SpriteEffects.None, 0.85F);

            // Debug info
            StringBuilder text = new StringBuilder();
            if (text.Length > 0) {
                batch.DrawStringWithShadow(Game1.smallFont, text.ToString(), boxBottomLeft, Color.White, 0.8F);
            }

            batch.End();
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
        }
        #endregion

        #region Static Helpers
        public static string Translate(string key, params object[] formatArgs) {
            Translation translation = ModFishing.Instance.Helper.Translation.Get(key);
            return translation.HasValue() ? string.Format(translation.ToString(), formatArgs) : key;
        }
        #endregion
    }
}
