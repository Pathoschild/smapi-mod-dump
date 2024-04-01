/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public class DerelictTractorTerrainFeature
        : TerrainFeature
    {
        private readonly Texture2D texture;
        private readonly Vector2 tile;
        private readonly ModEntry mod;

        public DerelictTractorTerrainFeature(ModEntry mod, Texture2D texture, Vector2 tile)
            : base(needsTick: false)
        {
            this.texture = texture;
            this.tile = tile;
            this.mod = mod;
        }

        public static void PlaceInField(ModEntry mod)
        {
            Game1.player.modData.TryGetValue(ModDataKeys.DerelictPosition, out string? positionAsString);
            if (positionAsString is null || !TryParse(positionAsString, out Vector2 position))
            {
                if (positionAsString is not null)
                {
                    mod.LogError($"Invalid value for {ModDataKeys.DerelictPosition}: {positionAsString} -- finding a new position");
                }

                position = GetClearSpotForTractor(mod);
                if (position == new Vector2())
                {
                    // Hope for better luck tomorrow
                    mod.LogError("No clear spot could be found to place the derelict tractor.");
                    return;
                }

                mod.LogInfo($"Derelict tractor placed at {position}");
                Game1.player.modData[ModDataKeys.DerelictPosition] = FormattableString.Invariant($"{position.X},{position.Y}");
            }
            Place(mod, position);
        }

        public static Vector2 GetClearSpotForTractor(ModEntry mod)
        {
            // Find a spot under a tree on the West side of the map
            var farm = Game1.getFarm();
            foreach (var eastSideTree in farm.terrainFeatures.Values.OfType<Tree>().Where(t => t.growthStage.Value == Tree.treeStage).OrderBy(tf => tf.Tile.X))
            {
                bool anyCollisions = false;
                List<Vector2> tilesToClear = new List<Vector2>();
                // note that trees are 3-wide, and this only looks at the leftmost pair, so we're leaving a little money on the table.
                foreach (var offset in new Vector2[] { new Vector2(0, -2), new Vector2(1, -2) })
                {
                    var posToCheck = eastSideTree.Tile + offset;
                    var objAtOffset = farm.getObjectAtTile((int)posToCheck.X, (int)posToCheck.Y);
                    if (objAtOffset is null)
                    {
                        if (!farm.CanItemBePlacedHere(posToCheck))
                        {
                            anyCollisions = true;
                            break;
                        }
                        // Else it's clear
                    }
                    else if (objAtOffset.Category == -999)
                    {
                        tilesToClear.Add(posToCheck);
                    }
                    else
                    {
                        anyCollisions = true;
                        break;
                    }
                }

                if (!anyCollisions)
                {
                    foreach (var tileToClear in tilesToClear)
                    {
                        // Not calling farm.removeObject because it does things that don't make sense when you're
                        // really just un-making a spot.
                        farm.objects.Remove(tileToClear);
                    }

                    return eastSideTree.Tile + new Vector2(0, -2);
                }
            }

            // No tree is around.  We're probably dealing with an old save,  Try looking for any clear space.
            //  This technique is kinda dumb, but whatev's.  This mod is going to suck with a fully-developed farm.
            for (int i = 0; i < 10000; ++i)
            {
                Vector2 positionToCheck = new Vector2(Game1.random.Next(farm.map.DisplayWidth / 64), Game1.random.Next(farm.map.DisplayHeight / 64));
                if (farm.CanItemBePlacedHere(positionToCheck) && farm.CanItemBePlacedHere(positionToCheck + new Vector2(1, 0)))
                {
                    return positionToCheck;
                }
            }

            return new Vector2();
        }

        public static void PlaceInGarage(ModEntry mod, Stable garage)
        {
            mod.LogInfoOnce($"Derelict tractor is in the garage");
            Place(mod, new Vector2(garage.tileX.Value + 1, garage.tileY.Value + 1));
        }

        private static void Place(ModEntry mod, Vector2 position)
        {
            mod.LogInfoOnce($"Derelict tractor drawn at {position}");
            var derelictTractorTexture = mod.Helper.ModContent.Load<Texture2D>("assets/rustyTractor.png");

            var tf = new DerelictTractorTerrainFeature(mod, derelictTractorTexture, position);

            Game1.getFarm().removeObject(position, showDestroyedObject: false);
            Game1.getFarm().removeObject(position + new Vector2(1, 0), showDestroyedObject: false);
            Game1.getFarm().terrainFeatures[position] = tf;
            Game1.getFarm().terrainFeatures[position + new Vector2(1, 0)] = tf;
        }

        private static bool TryParse(string s, out Vector2 position)
        {
            string[] split = s.Split(",");
            if (split.Length == 2
                && int.TryParse(split[0], out int x)
                && int.TryParse(split[1], out int y))
            {
                position = new Vector2(x, y);
                return true;
            }
            else
            {
                position = new Vector2();
                return false;
            }
        }


        public override Rectangle getBoundingBox()
        {
            var r = new Rectangle((int)this.tile.X * 64, (int)this.tile.Y * 64, 64*2, 64);
            return r;
        }

        public override bool isPassable(Character c)
        {
            return false;
        }

        public override bool performToolAction(Tool t, int damage, Vector2 tileLocation)
        {
            if (!Game1.player.questLog.Any(q => q is RestoreTractorQuest))
            {
                Game1.drawObjectDialogue("This looks like an old tractor.  Perhaps it could help you out around the farm, but it's been out in the weather a long time.  It'll need some fixing.  Maybe somebody in town can help?");
                this.mod.RestoreTractorQuestController.CreateQuestNew();
            }

            return base.performToolAction(t, damage, tileLocation);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            Rectangle tileSheetRect = Game1.getSourceRectForStandardTileSheet(this.texture, 0, 16, 16);
            tileSheetRect.Width = 32;
            tileSheetRect.Height = 32;
            spriteBatch.Draw(this.texture,
                              Game1.GlobalToLocal(Game1.viewport, (this.tile - new Vector2(0,1)) * 64f),
                              tileSheetRect,
                              color: Color.White,
                              rotation: 0f,
                              origin: Vector2.Zero,
                              scale: 4f,
                              effects: SpriteEffects.None,
                              layerDepth: this.tile.Y * 64f / 10000f + this.tile.X / 100000f);
        }
    }
}
