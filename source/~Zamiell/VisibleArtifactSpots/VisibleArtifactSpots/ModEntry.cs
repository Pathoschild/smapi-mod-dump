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
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace VisibleArtifactSpots
{
    public class ModEntry : Mod
    {
        // Variables
        private ModConfig config = new();

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();

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
                () => "The way to highlight things.",
                new string[] { "Border", "Bubble", "Both" }
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightArtifactSpots,
                (bool val) => config.HighlightArtifactSpots = val,
                () => "Artifact Spots",
                () => "Whether to highlight artifact spots."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightSeedSpots,
                (bool val) => config.HighlightSeedSpots = val,
                () => "Seed Spots",
                () => "Whether to highlight seed spots."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightWeeds,
                (bool val) => config.HighlightWeeds = val,
                () => "Weeds",
                () => "Whether to highlight weeds."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightTwigs,
                (bool val) => config.HighlightTwigs = val,
                () => "Twigs",
                () => "Whether to highlight twigs."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightStones,
                (bool val) => config.HighlightStones = val,
                () => "Stones",
                () => "Whether to highlight stones."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightCopperNodes,
                (bool val) => config.HighlightCopperNodes = val,
                () => "Copper Nodes",
                () => "Whether to highlight copper nodes."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightIronNodes,
                (bool val) => config.HighlightIronNodes = val,
                () => "Iron Nodes",
                () => "Whether to highlight iron nodes."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightGoldNodes,
                (bool val) => config.HighlightGoldNodes = val,
                () => "Gold Nodes",
                () => "Whether to highlight gold nodes."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightIridiumNodes,
                (bool val) => config.HighlightIridiumNodes = val,
                () => "Iridium Nodes",
                () => "Whether to highlight iridium nodes."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightRadioactiveNodes,
                (bool val) => config.HighlightRadioactiveNodes = val,
                () => "Radioactive Nodes",
                () => "Whether to highlight radioactive nodes."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightCinderNodes,
                (bool val) => config.HighlightCinderNodes = val,
                () => "Cinder Nodes",
                () => "Whether to highlight cinder nodes."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightChests,
                (bool val) => config.HighlightChests = val,
                () => "Chests (in Volcano Dungeon)",
                () => "Whether to highlight chests in the Volcano Dungeon."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightNonPlanted,
                (bool val) => config.HighlightNonPlanted = val,
                () => "Non-Planted Crops",
                () => "Whether to highlight tiles with no crops planted."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightNonWatered,
                (bool val) => config.HighlightNonWatered = val,
                () => "Non-Watered Tiles",
                () => "Whether to highlight tiles that are not watered."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightNonFertilized,
                (bool val) => config.HighlightNonFertilized = val,
                () => "Non-Fertilized Crops",
                () => "Whether to highlight tiles that are not fertilized."
            );

            configMenu.AddBoolOption(
                this.ModManifest,
                () => config.HighlightHoeableTile,
                (bool val) => config.HighlightHoeableTile = val,
                () => "Hoeable Tile",
                () => "Whether to highlight tiles that are hoeable and are not hoed."
            );
        }

        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            CheckLocationObjects(e.SpriteBatch);
            CheckLocationTerrainFeatures(e.SpriteBatch);
            CheckLocationTiles(e.SpriteBatch);
        }

        private void CheckLocationObjects(SpriteBatch spriteBatch)
        {
            foreach (StardewValley.Object obj in Game1.currentLocation.objects.Values)
            {
                if (ShouldHighlightObject(obj))
                {
                    int x = (int)obj.TileLocation.X;
                    int y = (int)obj.TileLocation.Y;
                    HighlightTile(x, y, spriteBatch);
                }
            }
        }

        private bool ShouldHighlightObject(StardewValley.Object obj)
        {
            string description = obj.getDescription();

            // The name of all ore nodes is "Stone", so we need a way to differentiate the types.
            // The naive way is to use the item IDs, but ore nodes can have multiple item IDs.
            // Thus, we can make the code simpler by using the description.
            return (
                (obj.Name == "Artifact Spot" && config.HighlightArtifactSpots)
                || (obj.Name == "Seed Spot" && config.HighlightSeedSpots)
                || (obj.Name == "Weeds" && config.HighlightWeeds && !InDungeon())
                || (obj.Name == "Twig" && config.HighlightTwigs && !InDungeon())
                || (obj.Name == "Stone" && config.HighlightStones && !InDungeon())
                || (obj.Name == "Stone" && description.Contains("copper") && config.HighlightCopperNodes)
                || (obj.Name == "Stone" && description.Contains("iron") && config.HighlightIronNodes)
                || (obj.Name == "Stone" && description.Contains("gold") && config.HighlightGoldNodes)
                || (obj.Name == "Stone" && description.Contains("iridium") && config.HighlightIridiumNodes)
                || (obj.Name == "Stone" && description.Contains("radioactive") && config.HighlightRadioactiveNodes)
                || (obj.Name == "Stone" && description.Contains("cinder") && config.HighlightCinderNodes)
                || (obj is Chest chest && !chest.playerChest.Value && !IsChestOpened(chest) && InVolcanoDungeon() && config.HighlightChests)
            );
        }

        private bool IsChestOpened(Chest chest)
        {
            // "currentLidFrame" is private, so we have to use reflection.
            int currentLidFrame = this.Helper.Reflection.GetField<int>(chest, "currentLidFrame").GetValue();

            // currentLidFrame is 224 on a closed chest.
            // currentLidFrame is 226 on an opened chest.
            return currentLidFrame != 224;
        }

        private bool InDungeon()
        {
            return InMinesOrSkullCavern() || InVolcanoDungeon();
        }

        private bool InMinesOrSkullCavern()
        {
            // The first floor of the mines is "Mine".
            return Game1.currentLocation.Name.StartsWith("UndergroundMine");
        }

        private bool InVolcanoDungeon()
        {
            return Game1.currentLocation.Name.StartsWith("VolcanoDungeon") && Game1.currentLocation.Name != "VolcanoDungeon0";
        }

        private void CheckLocationTerrainFeatures(SpriteBatch spriteBatch)
        {
            foreach (TerrainFeature terrainFeature in Game1.currentLocation.terrainFeatures.Values)
            {
                if (ShouldHighlightTerrainFeature(terrainFeature))
                {
                    int x = (int)terrainFeature.Tile.X;
                    int y = (int)terrainFeature.Tile.Y;
                    HighlightTile(x, y, spriteBatch);
                }
            }
        }

        private bool ShouldHighlightTerrainFeature(TerrainFeature terrainFeature)
        {
            if (terrainFeature is HoeDirt hoeDirt)
            {
                return (
                    (hoeDirt.crop is null && config.HighlightNonPlanted)
                    || (!hoeDirt.isWatered() && config.HighlightNonWatered)
                    || (hoeDirt.fertilizer.Value is null && config.HighlightNonFertilized)
                );
            }

            return false;
        }

        private void CheckLocationTiles(SpriteBatch spriteBatch)
        {
            if (Game1.currentLocation.Name != "Farm" && Game1.currentLocation.Name != "IslandWest")
            {
                return;
            }

            foreach (xTile.Layers.Layer layer in Game1.currentLocation.map.Layers)
            {
                for (int x = 0; x < layer.LayerWidth; x++)
                {
                    for (int y = 0; y < layer.LayerHeight; y++)
                    {
                        if (layer.Tiles[x, y] is not null)
                        {
                            CheckTile(x, y, spriteBatch);
                        }
                    }
                }
            }
        }

        private void CheckTile(int x, int y, SpriteBatch spriteBatch)
        {
            Vector2 tileLocation = new Vector2(x, y);

            if (
                config.HighlightHoeableTile
                && Game1.currentLocation.doesTileHaveProperty(x, y, "Diggable", "Back") is not null
                && Game1.currentLocation.CanPlantSeedsHere("(O)885", x, y, false, out string errorMessage) // "(O)885" is Fiber Seeds, which are plantable in all seasons.
                && Game1.currentLocation.isTilePassable(tileLocation)
                && !Game1.currentLocation.IsTileBlockedBy(tileLocation, ignorePassables: CollisionMask.All)
                && !DoesTileHaveCrop(tileLocation)
            )
            {
                HighlightTile(x, y, spriteBatch);
            }
        }

        private bool DoesTileHaveCrop(Vector2 tileLocation)
        {
            return (
                Game1.currentLocation.terrainFeatures.TryGetValue(tileLocation, out var terrainFeature)
                && terrainFeature is HoeDirt hoeDirt
                && hoeDirt.crop is not null
            );
        }

        private void HighlightTile(int x, int y, SpriteBatch spriteBatch)
        {
            switch (config.HighlightType)
            {
                case "Border":
                    DrawRedBorderAroundObject(x, y, spriteBatch);
                    break;

                case "Bubble":
                    DrawNotificationBubbleAboveObject(x, y, spriteBatch);
                    break;

                case "Both":
                    DrawRedBorderAroundObject(x, y, spriteBatch);
                    DrawNotificationBubbleAboveObject(x, y, spriteBatch);
                    break;

                default:
                    break;
            }
        }

        private void DrawRedBorderAroundObject(int x, int y, SpriteBatch spriteBatch)
        {
            var pos = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64));
            var rect = Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29);
            var fadedWhite = new Color(255, 255, 255, 127);

            // This draw invocation is copied from the tool hit rectangle in "Farmer.cs".
            spriteBatch.Draw(Game1.mouseCursors, pos, rect, fadedWhite, 0f, Vector2.Zero, 1f, SpriteEffects.None, pos.Y / 10000f);
        }

        private void DrawNotificationBubbleAboveObject(int x, int y, SpriteBatch spriteBatch)
        {
            Vector2 objPos = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64));
            Vector2 pos = objPos - new Vector2(0, 32); // 1 tile above where the object is
            Rectangle destinationRectangle = new Rectangle(
                (int)pos.X,
                (int)pos.Y - 32,
                64,
                64
            );
            Rectangle sourceRectangle = new Rectangle(
                16 * 16 % Game1.emoteSpriteSheet.Width,
                16 * 16 / Game1.emoteSpriteSheet.Width * 16,
                16,
                16
            );

            spriteBatch.Draw(
                Game1.emoteSpriteSheet,
                destinationRectangle,
                sourceRectangle,
                Color.White * 0.95f, 0.0f,
                Vector2.Zero,
                SpriteEffects.None,
                (float)((y - 1) * 64) / 10000f
            );
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
