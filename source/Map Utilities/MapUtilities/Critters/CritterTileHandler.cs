using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using xTile.Layers;

namespace MapUtilities.Critters
{
    public static class CritterTileHandler
    {
        public static void spawnCitters(GameLocation location)
        {
            foreach(Layer layer in location.map.Layers)
            {
                List<CritterSpawnData> critters = new List<CritterSpawnData>();
                
                //A back layer
                if (layer.Id.Split('_')[0].Equals("Back"))
                {
                    for(int x = 0; x < layer.LayerWidth; x++)
                    {
                        for(int y = 0; y < layer.LayerHeight; y++)
                        {
                            if(layer.Tiles[x,y] != null && layer.Tiles[x, y].Properties.ContainsKey("Critter"))
                            {
                                string[] critterData = layer.Tiles[x, y].Properties["Critter"].ToString().Split(' ');

                                List<string> critterSpawn = new List<string>();

                                for(int i = 0; i < critterData.Length; i++)
                                {
                                    if (critterData[i].EndsWith("%"))
                                    {
                                        if(critterSpawn.Count > 1)
                                        {
                                            CritterSpawnData spawnData = new CritterSpawnData(critterSpawn.ToArray(), new Microsoft.Xna.Framework.Vector2(x, y));
                                            if(spawnData.canSpawn() && Game1.random.NextDouble() <= spawnData.spawnChance)
                                                critters.Add(spawnData);
                                        }
                                        critterSpawn.Clear();
                                    }
                                    critterSpawn.Add(critterData[i]);
                                }
                                CritterSpawnData finalData = new CritterSpawnData(critterSpawn.ToArray(), new Microsoft.Xna.Framework.Vector2(x, y));
                                if (finalData.canSpawn() && Game1.random.NextDouble() <= finalData.spawnChance)
                                    critters.Add(finalData);
                            }
                        }
                    }
                }

                foreach(CritterSpawnData critter in critters)
                {
                    critter.spawnCritter(location);
                }
            }
        }
    }
}
