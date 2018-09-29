using System.Collections.Generic;
using PyTK.CustomElementHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Cobalt.Framework
{
    internal class CobaltSprinklerObject : StardewValley.Object, ISaveElement
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
            parentSheetIndex = INDEX;
            name = NAME;
            price = PRICE;
            type = TYPE;
            category = CATEGORY;
            edibility = EDIBILITY;
            canBeSetDown = true;
            canBeGrabbed = true; // ?
        }

        public object getReplacement()
        {
            return new StardewValley.Object(tileLocation, 645);
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            var ret = new Dictionary<string, string>();
            return ret;
        }

        public void rebuild(Dictionary<string, string> saveData, object replacement)
        {
            parentSheetIndex = INDEX;
            name = NAME;
            price = PRICE;
            type = TYPE;
            category = CATEGORY;
            edibility = EDIBILITY;
            canBeSetDown = true;
            canBeGrabbed = true; // ?
            tileLocation = ( ( StardewValley.Object ) replacement ).tileLocation;
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
                foreach (Vector2 tile in GetCoverage(this.tileLocation))
                {
                    if (location.terrainFeatures.ContainsKey(tile) && location.terrainFeatures[tile] is HoeDirt)
                        (location.terrainFeatures[tile] as HoeDirt).state = 1;
                }
                location.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.animations, new Rectangle(0, 2176, Game1.tileSize * 5, Game1.tileSize * 5), 60f, 4, 100, this.tileLocation * (float)Game1.tileSize + new Vector2((float)(-Game1.tileSize * 3 + Game1.tileSize), (float)(-Game1.tileSize * 2)), false, false)
                {
                    color = Color.White * 0.4f,
                    delayBeforeAnimationStart = Game1.random.Next(1000),
                    id = (float)((double)this.tileLocation.X * 4000.0 + (double)this.tileLocation.Y)
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
