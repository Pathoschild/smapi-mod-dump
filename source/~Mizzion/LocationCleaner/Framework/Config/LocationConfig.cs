using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LocationCleaner.Framework.Config
{
    internal class LocationConfig
    {
        public List<string> LocationsToClear { get; set; } = new List<string>() { "Farm", "Mountain" };
        public Vector2 ChestLocation { get; set; } = new Vector2(58, 16);//To the left of the house.

        private Objects ObjectConfig { get; set; }= new Objects();
        private Trees TreeConfig { get; set; } = new Trees();
        private TerrainFeatures TerrainConfig { get; set; } = new TerrainFeatures();
        private ResourceClumps ResouceConfig { get; set; } = new ResourceClumps();
    }
}
