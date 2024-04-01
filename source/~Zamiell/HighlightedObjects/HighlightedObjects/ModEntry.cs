/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace HighlightedObjects
{
    public class HighlightedJars : Mod
    {
        private const int EXCLAMATION_EMOTE = 16;

        private ModConfig config = new();

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => config = new ModConfig(),
                save: () => this.Helper.WriteConfig(config)
            );

            configMenu.AddTextOption(
                this.ModManifest,
                () => config.HighlightType,
                (string val) => config.HighlightType = val,
                () => "Highlight type",
                () => "The way to display empty objects.",
                new string[] { "Highlight", "Bubble" }
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightJars,
                (bool val) => config.HighlightJars = val,
                () => "Preserves Jars",
                () => "Whether to highlight Preserves Jars."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightKegs,
                (bool val) => config.HighlightKegs = val,
                () => "Kegs",
                () => "Whether to highlight Kegs."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightCasks,
                (bool val) => config.HighlightCasks = val,
                () => "Casks",
                () => "Whether to highlight Casks."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightCrystalariums,
                (bool val) => config.HighlightCrystalariums = val,
                () => "Crystalariums",
                () => "Whether to highlight Crystalariums."
            );
        }

        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (Game1.currentLocation == null)
            {
                return;
            }

            var highlightableObjects = Game1.currentLocation.objects.Values.Where(o =>
                (
                    (config.HighlightJars && o.Name == "Preserves Jar")
                    || (config.HighlightKegs && o.Name == "Keg")
                    || (config.HighlightCasks && o.Name == "Cask")
                    || (config.HighlightCrystalariums && o.Name == "Crystalarium")
                )
                && o.MinutesUntilReady <= 0
                && !o.readyForHarvest.Value
            ).ToList();

            foreach (var highlightableObject in highlightableObjects)
            {
                var globalPos = new Vector2((float)(highlightableObject.TileLocation.X * 64), (float)(Math.Abs(highlightableObject.TileLocation.Y * 64 - 64)));
                Vector2 local = Game1.GlobalToLocal(Game1.viewport, globalPos);
                Rectangle destinationRectangle = new Rectangle((int)local.X, (int)local.Y - 32, 64, 64);

                switch (config.HighlightType)
                {
                    case "Highlight":
                        e.SpriteBatch.Draw(
                            Game1.bigCraftableSpriteSheet,
                            new Vector2(local.X + 32, local.Y + 32),
                            new Rectangle?(StardewValley.Object.getSourceRectForBigCraftable(highlightableObject.ParentSheetIndex)),
                            Color.Red * 0.50f,
                            0.0f,
                            new Vector2(8f, 8f),
                            4f,
                            SpriteEffects.None,
                            (float)((highlightableObject.TileLocation.Y - 1) * 64) / 10000f
                        );
                        break;

                    case "Bubble":
                        e.SpriteBatch.Draw(
                            Game1.emoteSpriteSheet,
                            destinationRectangle,
                            new Rectangle(EXCLAMATION_EMOTE * 16 % Game1.emoteSpriteSheet.Width, EXCLAMATION_EMOTE * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16),
                            Color.White * 0.95f, 0.0f,
                            Vector2.Zero,
                            SpriteEffects.None,
                            (float)((highlightableObject.TileLocation.Y - 1) * 64) / 10000f
                        );
                        break;
                }
            }
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
