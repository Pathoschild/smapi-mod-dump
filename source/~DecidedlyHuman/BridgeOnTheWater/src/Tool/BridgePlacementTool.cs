/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Xml.Serialization;
using BridgeOnTheWater;
using DecidedlyShared.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableBridges.TerrainFeatures;
using StardewValley;

namespace PortableBridges.Tool
{
    [XmlType("Mods_DecidedlyHumanBridgePlacementTool")]
    public class BridgePlacementTool : StardewValley.Tool
    {
        private Lazy<Texture2D> texture = new(() =>
        {
            return Game1.content.Load<Texture2D>("DecidedlyHuman.BridgeOnTheWater/PlacementTool");
        });

        public BridgePlacementTool() : base(I18n.BridgeOnTheWater_Tool_Name(), 0, 0, 0, false)
        {
        }

        protected override Item GetOneNew()
        {
            throw new NotImplementedException();
        }

        protected override string loadDisplayName()
        {
            return I18n.BridgeOnTheWater_Tool_Name();
        }

        protected override string loadDescription()
        {
            return I18n.BridgeOnTheWater_Tool_Description();
        }

        public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
        {
            // Flooring floor = new Flooring(2);
            var bridge = new Bridge();
            var tile = who.GetGrabTile();

            if (!location.terrainFeatures.ContainsKey(tile))
            {
                location.terrainFeatures.Add(tile, bridge);

                foreach (var direction in Directions.vector2)
                {
                    var newTile = tile + direction;
                    if (location.doesTileHavePropertyNoNull((int)newTile.X, (int)newTile.Y, "Water", "Back")
                            .Equals("T") ||
                        location.doesTileHavePropertyNoNull((int)newTile.X, (int)newTile.Y, "Water", "Buildings")
                            .Equals("T"))
                        location.setTileProperty((int)newTile.X, (int)newTile.Y, "Back", "Passable", "T");
                }

                location.setTileProperty((int)tile.X, (int)tile.Y, "Buildings", "Passable", "T");
                location.setTileProperty((int)tile.X, (int)tile.Y, "Back", "Water", "F");
            }

            //BridgeObject bridge = new BridgeObject();
            ////location.objects.Add((StardewValley.Object)bridge);
            //bridge.placementAction(location, x, y, who);

            return true;
        }

        public override void endUsing(GameLocation location, Farmer who)
        {
        }

        public override bool onRelease(GameLocation location, int x, int y, Farmer who)
        {
            return true;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
        }

        public override void draw(SpriteBatch b)
        {
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency,
            float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(Game1.toolSpriteSheet, location + new Vector2(32f, 32f),
                Game1.getSquareSourceRectForNonStandardTileSheet(Game1.toolSpriteSheet, 16, 16,
                    this.IndexOfMenuItemView), color * transparency, 0f, new Vector2(8f, 8f), 4f * scaleSize,
                SpriteEffects.None, layerDepth);
        }
    }
}
