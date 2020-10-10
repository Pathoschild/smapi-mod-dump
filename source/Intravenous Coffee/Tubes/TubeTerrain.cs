/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mpcomplete/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace Tubes
{
    // A terrain feature representation of our Pneumatic Tube. For our purposes, it's just an object that you can walk through.
    public class TubeTerrain : TerrainFeature, ISaveElement
    {
        internal static Texture2D SpriteSheet;
        internal const int SpriteSize = 48;

        // Index into the SpriteIndex sheet for this tube (e.g. a left-right tube or 4-way tube, etc).
        private int SpriteIndex;
        // Number of clockwise quarter rotations to draw with.
        private int SpriteRotation;

        public TubeTerrain()
        {
            if (Flooring.drawGuide == null)
                Flooring.populateDrawGuide();
        }

        internal static void Init()
        {
            SpriteSheet = TubesMod._helper.Content.Load<Texture2D>(@"Assets/terrain.png");
        }

        internal static void UpdateSpritesInLocation(GameLocation location)
        {
            foreach (var tile in location.terrainFeatures) {
                if (tile.Value is TubeTerrain tube)
                    tube.UpdateSprite(location, tile.Key);
            }
        }

        // ISaveElement overrides

        public Dictionary<string, string> getAdditionalSaveData()
        {
            return new Dictionary<string, string>();
        }

        public object getReplacement()
        {
            return new Flooring(42);
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
        }

        // TerrainFeature overrides

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Rectangle((int)((double)tileLocation.X * (double)Game1.tileSize), (int)((double)tileLocation.Y * (double)Game1.tileSize), Game1.tileSize, Game1.tileSize);
        }

        public override void doCollisionAction(Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation, Character who, GameLocation location)
        {
            base.doCollisionAction(positionOfCollider, speedOfCollision, tileLocation, who, location);
        }

        public override bool isPassable(Character c = null)
        {
            return true;
        }

        public override bool performToolAction(Tool t, int damage, Vector2 tileLocation, GameLocation location = null)
        {
            if (location == null)
                location = Game1.currentLocation;
            if (t == null && damage <= 0 || damage <= 0 && !(t.GetType() == typeof(Pickaxe)) && !(t.GetType() == typeof(Axe)))
                return false;
            Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, 4, false, -1, false, -1);

            Game1.playSound("hammer");

            location.debris.Add(new Debris((Item)new StardewValley.Object(TubeObject.ObjectData.sdvId, 1, false, -1, 0), tileLocation * (float)Game1.tileSize + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2))));
            location.terrainFeatures.Remove(tileLocation);

            // Add a temporary junk object to force a LocationObjectsChanged event. Will be removed there.
            location.objects.Add(tileLocation, new StardewValley.Object(JunkObject.objectData.sdvId, 1, false, -1, 0));
            return false;
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            const float kQuarterClockwise = (float)Math.PI / 2.0f;
            const float KWhyDoesThisScaleWork = 0.35f;

            Vector2 position =
                Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * (float)Game1.tileSize, tileLocation.Y * (float)Game1.tileSize));
            Rectangle source = Game1.getSourceRectForStandardTileSheet(SpriteSheet, SpriteIndex, SpriteSize, SpriteSize);
            Vector2 offset = Vector2.Zero;
            switch (SpriteRotation) {
                case 3: offset = new Vector2(SpriteSize, 0); break;
                case 2: offset = new Vector2(SpriteSize, SpriteSize); break;
                case 1: offset = new Vector2(0, SpriteSize); break;
            }
            spriteBatch.Draw(SpriteSheet, position, new Rectangle?(source), Color.White, SpriteRotation * kQuarterClockwise, offset, KWhyDoesThisScaleWork * Game1.pixelZoom, SpriteEffects.None, 1E-09f);
        }

        // Updates which sprite to use in the terrainSprites sheet, based on connecting tubes.
        private void UpdateSprite(GameLocation location, Vector2 tileLocation)
        {
            SpriteIndex = 0;
            SpriteRotation = 0;

            if (!location.terrainFeatures.ContainsKey(tileLocation)) {
                return;
            }

            Vector2[] neighborLocations = {
                new Vector2(-1, 0) + tileLocation,
                new Vector2(1, 0) + tileLocation,
                new Vector2(0, 1) + tileLocation,
                new Vector2(0, -1) + tileLocation
            };

            bool[] neighborConnections = new bool[4];
            int connections = 0;

            for (int i = 0; i < 4; i++) {
                if ((location.terrainFeatures.TryGetValue(neighborLocations[i], out TerrainFeature tf) && tf is TubeTerrain) ||
                    (location.objects.TryGetValue(neighborLocations[i], out StardewValley.Object obj) && obj is PortObject)) {
                    neighborConnections[i] = true;
                    connections++;
                } else if (TileHelper.TryGetBuildingEntrance(location, neighborLocations[i]) is GameLocation indoors) {
                    neighborConnections[i] = true;
                    connections++;
                }
            }

            // Just for clarity.
            bool hasLeft = neighborConnections[0];
            bool hasRight = neighborConnections[1];
            bool hasBottom = neighborConnections[2];
            bool hasTop = neighborConnections[3];

            switch (connections) {
                case 0:
                    SpriteIndex = 0;
                    break;
                case 1:
                    SpriteIndex = 0;
                    if (hasTop || hasBottom)
                        SpriteRotation = 1;
                    break;
                case 2:
                    if (hasLeft && hasRight) {
                        SpriteIndex = 0;
                    } else if (hasTop && hasBottom) {
                        SpriteIndex = 0;
                        SpriteRotation = 1;
                    } else if (hasLeft && hasTop) {
                        SpriteIndex = 1;
                    } else if (hasTop && hasRight) {
                        SpriteIndex = 1;
                        SpriteRotation = 1;
                    } else if (hasRight && hasBottom) {
                        SpriteIndex = 1;
                        SpriteRotation = 2;
                    } else if (hasBottom && hasLeft) {
                        SpriteIndex = 1;
                        SpriteRotation = 3;
                    }
                    break;
                case 3:
                    SpriteIndex = 2;
                    if (hasLeft && hasTop && hasRight) {
                        // no rotation
                    } else if (hasTop && hasRight && hasBottom) {
                        SpriteRotation = 1;
                    } else if (hasRight && hasBottom && hasLeft) {
                        SpriteRotation = 2;
                    } else if (hasBottom && hasLeft && hasTop) {
                        SpriteRotation = 3;
                    }
                    break;
                case 4:
                    SpriteIndex = 3;
                    break;
            }
        }
    }
}
