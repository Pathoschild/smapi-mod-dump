/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/UnkLegacy/QiSprinklers
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley.Locations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection.Emit;
using QiSprinklers.Framework;


namespace QiSprinklers
{
    public class QiSprinklers : Mod
    {
        public static IModHelper ModHelper;
        public ModConfig Config;
        private Multiplayer mp;

        enum AnimSize
        {
            SMALL,
            MEDIUM,
            LARGE,
            XLARGE
        }

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            Config = Helper.ReadConfig<ModConfig>();
            mp = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Content.AssetEditors.Add(new AssetEditor());

            if (Config.ActivateOnAction)
            {
                Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            }

            if (Config.ActivateOnPlacement)
            {
                Helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // force add sprinkler recipe for people who were level 10 before installing mod
            if (Game1.player.FarmingLevel >= QiSprinklerItem.CRAFTING_LEVEL)
            {
                try
                {
                    Game1.player.craftingRecipes.Add("Qi Sprinkler", 0);
                }
                catch { }
            }

            StardewValley.Object sprinkler;
            foreach (GameLocation location in Game1.locations)
            {
                if (location is GameLocation)
                {
                    foreach (KeyValuePair<Vector2, StardewValley.Object> pair in location.objects.Pairs)
                    {
                        if (location.objects[pair.Key].ParentSheetIndex == QiSprinklerItem.INDEX)
                        {
                            sprinkler = location.objects[pair.Key];
                            int id = (int)sprinkler.TileLocation.X * 4000 + (int)sprinkler.TileLocation.Y;
                        }
                    }
                }
            }
        }

        /// <summary>Raised after the player presses a button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Ingore event if world is not loaded and player is not interacting with the world
            if (!Context.IsPlayerFree)
                return;

            if (e.Button.IsActionButton())
            {
                var tile = e.Cursor.GrabTile;
                if (tile == null) return;

                var obj = Game1.currentLocation.getObjectAtTile((int)tile.X, (int)tile.Y);
                if (obj == null) return;

                var currentItem = Game1.player.CurrentItem;

                // Check if currently holding a Pressure Nozzle and the Sprinkler has no nozzle
                if (currentItem != null && currentItem.parentSheetIndex == 915 && (obj.heldObject.Value == null || obj.heldObject.Value.parentSheetIndex != 915))
                {
                    return;
                }

                ActivateSprinkler(obj);
            }
        }
        /// <summary>Raised after objects are added or removed in a location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            foreach (var pair in e.Added)
            {
                ActivateSprinkler(pair.Value);
            }
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, System.EventArgs e)
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (StardewValley.Object obj in location.Objects.Values)
                {
                    if (obj.ParentSheetIndex == QiSprinklerItem.INDEX)
                    {
                        // add water spray animation
                        location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 2176, 320, 320), 60f, 4, 50, obj.TileLocation * 64 + new Vector2(-192, -208), false, false)
                        {
                            color = Color.White * 0.4f,
                            scale = 7f / 5f,
                            delayBeforeAnimationStart = 0,
                            id = obj.TileLocation.X * 4000f + obj.TileLocation.Y
                        });

                        if (location is Farm || location.IsGreenhouse || location is IslandWest)
                        {
                            for (int index1 = (int)obj.TileLocation.X - Config.SprinklerRange; index1 <= obj.TileLocation.X + Config.SprinklerRange; ++index1)
                            {
                                for (int index2 = (int)obj.TileLocation.Y - Config.SprinklerRange; index2 <= obj.TileLocation.Y + Config.SprinklerRange; ++index2)
                                {
                                    Vector2 key = new Vector2(index1, index2);
                                    // water dirt
                                    if (location.terrainFeatures.ContainsKey(key) && location.terrainFeatures[key] is HoeDirt)
                                    {
                                        (location.terrainFeatures[key] as HoeDirt).state.Value = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            } 
        }

        /// <summary>Attempts to activate the correct sprinkler.</summary>
        /// <param name="sprinkler">The sprinkler object.</param>
        private void ActivateSprinkler(StardewValley.Object sprinkler)
        {
            // This function determines which sprinkler was activated
            if (sprinkler == null) return;

            if (sprinkler.IsSprinkler() || sprinkler.Name.Contains("Sprinkler"))
            {
                /* if (LineSprinklersIsLoaded && sprinkler.Name.Contains("Line"))
                {
                    ActivateLineSprinkler(sprinkler);
                }
                else if (PrismaticToolsApi != null && sprinkler.Name.Contains("Prismatic"))
                {
                    ActivatePrismaticSprinkler(sprinkler);
                }
                else if (BetterSprinklersApi != null)
                {
                    ActivateBetterSprinkler(sprinkler);
                }
                else if (SimpleSprinklerApi != null)
                {
                    ActivateSimpleSprinkler(sprinkler);
                } */
                if (sprinkler.Name.Contains("Qi Sprinkler"))
                {
                    ActivateQiSprinkler(sprinkler);
                }
                else
                {
                    ActivateVanillaSprinkler(sprinkler);
                }
            }
        }

        /// <summary>Attempts to activate the Qi Sprinkler</summary>
        /// <param name="sprinkler">The sprinkler object.</param>
        private void ActivateQiSprinkler(StardewValley.Object sprinkler)
        {
            Vector2 sprinklerTile = sprinkler.TileLocation;
            int SprinklerRange = Config.SprinklerRange;

            PlayAnimation(sprinklerTile, AnimSize.XLARGE);

            sprinklerTile.X -= SprinklerRange;
            sprinklerTile.Y -= SprinklerRange;
            float sprinklerTileYreset = sprinklerTile.Y;

            for (int x = -SprinklerRange; x <= SprinklerRange; x++)
            {
                for (int y = -SprinklerRange; y <= SprinklerRange; y++)
                {
                    WaterTile(sprinklerTile);
                    MaybeFertilizeTile(sprinklerTile);
                    sprinklerTile.Y++;
                }

                sprinklerTile.Y = sprinklerTileYreset;
                WaterTile(sprinklerTile);
                MaybeFertilizeTile(sprinklerTile);
                sprinklerTile.X++;
            }
        }

        /// <summary>Attempts to activate the default sprinkler.</summary>
        /// <param name="sprinkler">The sprinkler object.</param>
        private void ActivateVanillaSprinkler(StardewValley.Object sprinkler)
        {
            Vector2 sprinklerTile = sprinkler.TileLocation;
            int range = sprinkler.GetModifiedRadiusForSprinkler();
            if (range < 0)
            {
                Monitor.Log($"Invalid sprinkler range: {range}", LogLevel.Error);
                return;
            }
            List<Vector2> coverage = sprinkler.GetSprinklerTiles();
            
            foreach(Vector2 v in coverage)
            {
                WaterTile(v);
            }
            switch(range)
            {
                case 0:
                    PlayAnimation(sprinklerTile, AnimSize.SMALL);
                    break;
                case 1:
                    PlayAnimation(sprinklerTile, AnimSize.MEDIUM);
                    break;
                case 2:
                    PlayAnimation(sprinklerTile, AnimSize.LARGE);
                    break;
                default:
                    PlayAnimation(sprinklerTile, AnimSize.XLARGE);
                    break;
            }
        }

        /// <summary>Attempts to water a specific tile.</summary>
        /// <param name="tile">The tile attempting to be watered.</param>
        /// <param name="useWatercanAnimation">True if want to use the watering can animation.</param>
        private void WaterTile(Vector2 tile, bool useWatercanAnimation = false)
        {
            WateringCan can = new WateringCan();
            GameLocation loc = Game1.currentLocation;

            loc.terrainFeatures.TryGetValue(tile, out TerrainFeature terrainFeature);
            if (terrainFeature != null)
                terrainFeature.performToolAction(can, 0, tile, (GameLocation)null);

            loc.Objects.TryGetValue(tile, out StardewValley.Object obj);
            if (obj != null)
                obj.performToolAction(can, (GameLocation)null);

            //Watercan animation (only for sline sprinklers, because default animation don't make any sense here
            if (mp != null && useWatercanAnimation)
            {
                mp.broadcastSprites(loc, new TemporaryAnimatedSprite[]
                {
                    new TemporaryAnimatedSprite(13, tile * (float)Game1.tileSize, Color.White, 10, Game1.random.NextDouble() < 0.5, 70f, 0, -1, -1f, -1, 0)
                    {
                        delayBeforeAnimationStart = 150
                    }
                });
            }
        }

        /// <summary>Attempts to fertilize a tile after the user clicks on a Qi Sprinkler.</summary>
        /// <param name="tile">The tile attempting to be fertilized.</param>
        private void MaybeFertilizeTile(Vector2 tile)
        {
            var playerItem = Game1.player.CurrentItem;

            // Check if the player is holding anything.
            if (playerItem != null)
            {
                var fertilizer = playerItem.parentSheetIndex;
                int[] FertilizerList = { 368, 369, 370, 371, 465, 466, 918, 919, 920 };

                // Check if the player is holding fertilizer.
                if (Array.Exists(FertilizerList, fert => fert == fertilizer))
                {
                    GameLocation loc = Game1.currentLocation;
                    Farmer player = Game1.player;
                    loc.terrainFeatures.TryGetValue(tile, out TerrainFeature terrainFeature);
                    
                    // Check if the tile being fertilized is Hoe'd dirt.
                    if (terrainFeature is HoeDirt)
                    {
                        var dirt = loc.terrainFeatures[tile] as HoeDirt;

                        // Check if player is holding fertilizer.  If they are, THROW IT ON THE GROUND, and remove 1 from inventory.
                        if (dirt.fertilizer.Value == 0)
                        {
                            loc.playSound("dirtyHit");
                            dirt.fertilizer.Value = fertilizer;
                            player.removeItemsFromInventory(fertilizer, 1);
                        }
                    }
                }
            }
        }

        /// <summary>Play the "water cloud" animation after user activates sprinkler.</summary>
        /// <param name="sprinklerTile">The origin tile of the sprinkler.</param>
        /// <param name="size">The size of the sprinkler's range.</param>
        private void PlayAnimation(Vector2 sprinklerTile, AnimSize size)
        {
            if (mp == null)
                return;

            int animDelay = Game1.random.Next(500);
            float animId = (float)((double)sprinklerTile.X * 4000.0 + (double)sprinklerTile.Y);
            Vector2 pos = sprinklerTile * (float)Game1.tileSize;
            int numberOfLoops = 25;

            switch (size)
            {
                case AnimSize.SMALL:
                    mp.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite[]
                    {
                        new TemporaryAnimatedSprite(29, pos + new Vector2(0.0f, (float)(-Game1.tileSize * 3 / 4)), Color.White * 0.5f, 4, false, 60f, numberOfLoops, -1, -1f, -1, 0)
                        {
                            delayBeforeAnimationStart = animDelay,
                            id = animId
                        },
                        new TemporaryAnimatedSprite(29, pos + new Vector2((float)(Game1.tileSize * 3 / 4), 0.0f), Color.White * 0.5f, 4, false, 60f, numberOfLoops, -1, -1f, -1, 0)
                        {
                            rotation = 1.570796f,
                            delayBeforeAnimationStart = animDelay,
                            id = animId
                        },
                        new TemporaryAnimatedSprite(29, pos + new Vector2(0.0f, (float)(Game1.tileSize * 3 / 4)), Color.White * 0.5f, 4, false, 60f, numberOfLoops, -1, -1f, -1, 0)
                        {
                            rotation = 3.141593f,
                            delayBeforeAnimationStart = animDelay,
                            id = animId
                        },
                        new TemporaryAnimatedSprite(29, pos + new Vector2((float)(-Game1.tileSize * 3 / 4), 0.0f), Color.White * 0.5f, 4, false, 60f, numberOfLoops, -1, -1f, -1, 0)
                        {
                            rotation = 4.712389f,
                            delayBeforeAnimationStart = animDelay,
                            id = animId
                        }
                    });
                    break;
                case AnimSize.MEDIUM:
                    pos -= new Vector2(Game1.tileSize, Game1.tileSize);
                    mp.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite[]
                    {
                        new TemporaryAnimatedSprite(Game1.animations.Name, new Rectangle(0, 1984, Game1.tileSize * 3, Game1.tileSize * 3), 60f, 3, numberOfLoops, pos, false, false)
                        {
                            color = Color.White * 0.2f,
                            delayBeforeAnimationStart = animDelay,
                            id = animId
                        }
                    });
                    break;
                case AnimSize.LARGE:
                    pos += new Vector2(-3 * Game1.tileSize + Game1.tileSize, -Game1.tileSize * 2);
                    mp.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite[]
                    {
                        new TemporaryAnimatedSprite(Game1.animations.Name, new Rectangle(0, 2176, Game1.tileSize * 5, Game1.tileSize * 5), 60f, 4, numberOfLoops, pos, false, false)
                        {
                            color = Color.White * 0.2f,
                            delayBeforeAnimationStart = animDelay,
                            id = animId
                        }
                    });
                    break;
                case AnimSize.XLARGE:
                    pos += new Vector2(-4 * Game1.tileSize + Game1.tileSize, -Game1.tileSize * 3);
                    mp.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite[]
                    {
                        new TemporaryAnimatedSprite(Game1.animations.Name, new Rectangle(0, 2176, Game1.tileSize * 5, Game1.tileSize * 5), 60f, 4, numberOfLoops, pos, false, false)
                        {
                            color = Color.White * 0.2f,
                            scale = 7f / 5f,
                            delayBeforeAnimationStart = animDelay,
                            id = animId
                        }
                    });
                    break;
            }
        }
    }

    public class QiSprinklerItem
    {
        public const int INDEX = 1113;
        public const int PRICE = 2000;
        public const int EDIBILITY = -300;
        public const string TYPE = "Crafting";
        public const int CATEGORY = StardewValley.Object.CraftingCategory;
        public const int CRAFTING_LEVEL = 9;
    }
}