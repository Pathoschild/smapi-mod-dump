using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace Cobalt.Framework
{
    internal class CobaltSprinklerObject : SObject, ISaveElement
    {
        internal const int INDEX = 901;
        public const string NAME = "Cobalt Sprinkler";
        public const string DESCRIPTION = "Waters the 48 adjacent tiles every morning.";
        public const int PRICE = 2000;
        public const string TYPE = "Crafting";
        public const int CATEGORY = CraftingCategory;
        public const int EDIBILITY = -300;

        private Texture2D icon = ModEntry.instance.Helper.Content.Load<Texture2D>("cobalt-sprinkler.png");

        public CobaltSprinklerObject()
        {
            this.ParentSheetIndex = INDEX;
            this.Name = NAME;
            this.Price = PRICE;
            this.Type = TYPE;
            this.Category = CATEGORY;
            this.Edibility = EDIBILITY;
            this.CanBeSetDown = true;
            this.CanBeGrabbed = true; // ?
        }

        public object getReplacement()
        {
            return new SObject(tileLocation, 645);
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            var ret = new Dictionary<string, string>();
            return ret;
        }

        public void rebuild(Dictionary<string, string> saveData, object replacement)
        {
            this.ParentSheetIndex = INDEX;
            this.Name = NAME;
            this.Price = PRICE;
            this.Type = TYPE;
            this.Category = CATEGORY;
            this.Edibility = EDIBILITY;
            this.CanBeSetDown = true;
            this.CanBeGrabbed = true; // ?
            this.TileLocation = ((SObject)replacement).TileLocation;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            var v = base.getBoundingBox(tileLocation);
            v.Width = v.Height = Game1.tileSize;
            return v;
        }

        public override void DayUpdate(GameLocation location)
        {
            Log.trace("Sprinkling");
            this.health = 10;
            //if (!Game1.isRaining || !location.isOutdoors)
            {
                foreach (Vector2 tile in GetCoverage(this.TileLocation))
                {
                    if (location.terrainFeatures.ContainsKey(tile) && location.terrainFeatures[tile] is HoeDirt dirt)
                        dirt.state.Value = 1;
                }
                location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 2176, Game1.tileSize * 5, Game1.tileSize * 5), 60f, 4, 100, this.TileLocation * Game1.tileSize + new Vector2((float)(-Game1.tileSize * 3 + Game1.tileSize), (float)(-Game1.tileSize * 2)), false, false)
                {
                    color = Color.White * 0.4f,
                    delayBeforeAnimationStart = Game1.random.Next(1000),
                    id = this.TileLocation.X * 4000f + this.TileLocation.Y
                });
            }
        }

        /// <summary>Get the cobalt sprinkler coverage.</summary>
        /// <param name="origin">The tile position containing the sprinkler.</param>
        public static IEnumerable<Vector2> GetCoverage(Vector2 origin)
        {
            for (int x = -3; x <= 3; x++)
            {
                for (int y = -3; y <= 3; y++)
                    yield return origin + new Vector2(x, y);
            }
        }
    }
}
