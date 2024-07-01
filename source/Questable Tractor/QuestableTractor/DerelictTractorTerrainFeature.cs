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

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Stardew.QuestableTractor
{
    /// <summary>
    ///   draws and handles collisions for the derelict tractor
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is probably not a great way to get where we're trying to go.
    ///   It has two main hacks in it.  The first is that it gets created and
    ///   destroyed every day to avoid it being saved into the save file
    ///   and the second is that we actually have to have two of these in
    ///   order to have a 2x1-tile collision boundary.  (So the rightmost
    ///   one's draw function does nothing).
    ///  </para>
    /// </remarks>
    public class DerelictTractorTerrainFeature
        : TerrainFeature
    {
        private Texture2D? texture;
        public const string DerelictTractorPetFinderId = "QuestableTractor.DerelictTractor";
        private bool farmhandHasFoundTractor = false;


        public DerelictTractorTerrainFeature()
            : base(needsTick: false)
        {
        }

        public static void PlaceInField(ModEntry mod)
        {
            if (!Game1.IsMasterGame) return;

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
                    mod.LogError($"No clear spot could be found to place the derelict tractor.");
                    return;
                }

                mod.LogInfo($"Derelict tractor placed at {position}");
                Game1.player.modData[ModDataKeys.DerelictPosition] = FormattableString.Invariant($"{position.X},{position.Y}");
            }

            Place(mod, position);
        }

        public static Vector2 GetClearSpotForTractor(ModEntry mod)
        {
            // Find a spot behind a tree on the West side of the map
            bool isGrandpasFarmModRunning = mod.IsRunningGrandpasFarm;
            var farm = Game1.getFarm();
            var trees = farm.terrainFeatures.Values.OfType<Tree>()
                .Where(t => t.growthStage.Value == Tree.treeStage)
                .Where(t => !isGrandpasFarmModRunning || t.Tile.X > 43); // avoid actual far west side on this farm, as it is inaccessible.
            foreach (var eastSideTree in trees.OrderBy(tf => tf.Tile.X))
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
            //  This technique is kinda dumb, but whatev's.  This mod isn't a good match for fully-developed farms.
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
            if (!Game1.IsMasterGame) return;

            mod.LogInfoOnce($"Derelict tractor is in the garage");
            Place(mod, new Vector2(garage.tileX.Value + 1, garage.tileY.Value + 1));
        }

        private static void Place(ModEntry mod, Vector2 position)
        {
            mod.LogInfoOnce($"Derelict tractor drawn at {position}");

            Game1.getFarm().removeObject(position, showDestroyedObject: false);
            Game1.getFarm().removeObject(position + new Vector2(1, 0), showDestroyedObject: false);
            Game1.getFarm().terrainFeatures[position] = new DerelictTractorTerrainFeature();
            Game1.getFarm().terrainFeatures[position + new Vector2(1, 0)] = new DerelictTractorTerrainFeature();
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


        // This might seem like a thing, but it's not -- the game code will only call this
        //  if the thing it's checking against impinges on the tile we're on, so you can't
        //  really make a bounding box bigger than 64x64.
        //public override Rectangle getBoundingBox()
        //{
        //    var r = new Rectangle((int)this.Tile.X * 64, (int)this.Tile.Y * 64, 64*2, 64);
        //    return r;
        //}

        public override bool isPassable(Character c)
        {
            return false;
        }

        public override bool performToolAction(Tool t, int damage, Vector2 tileLocation)
        {
            if (!ModEntry.Instance.RestoreTractorQuestController.IsStartedByMasterPlayer)
            {
                if (Game1.IsMasterGame)
                {
                    Game1.drawObjectDialogue(L("This looks like an old tractor.  Perhaps it could help you out around the farm, but it's been out in the weather a long time.  It'll need some fixing.  Maybe somebody in town can help?"));
                    ModEntry.Instance.RestoreTractorQuestController.CreateQuestNew(Game1.player);
                }
                else if (!this.farmhandHasFoundTractor)
                {
                    Game1.drawObjectDialogue(LF($"This looks like an old tractor.  You should tell {Game1.MasterPlayer.Name} about this thing."));
                    this.farmhandHasFoundTractor = true;
                }
            }

            return base.performToolAction(t, damage, tileLocation);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (this.Location is null)
            {
                // ModEntry.Instance.LogError("In DerelictTractorTerrainFeature.draw and this.Location was null");
                return;
            }

            if (this.Location.terrainFeatures.TryGetValue(this.Tile-new Vector2(1,0), out var tfAtLeft) && tfAtLeft is DerelictTractorTerrainFeature)
            {
                // See hack alert in the class description
                return;
            }

            if (this.texture is null)
            {
                this.texture = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/rustyTractor.png");
            }

            Rectangle tileSheetRect = Game1.getSourceRectForStandardTileSheet(this.texture, 0, 16, 16);
            tileSheetRect.Width = 32;
            tileSheetRect.Height = 32;
            spriteBatch.Draw(this.texture,
                              Game1.GlobalToLocal(Game1.viewport, (this.Tile - new Vector2(0,1)) * 64f),
                              tileSheetRect,
                              color: Color.White,
                              rotation: 0f,
                              origin: Vector2.Zero,
                              scale: 4f,
                              effects: SpriteEffects.None,
                              layerDepth: this.Tile.Y * 64f / 10000f + this.Tile.X / 100000f);
        }
    }
}
