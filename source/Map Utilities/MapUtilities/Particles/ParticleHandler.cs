using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace MapUtilities.Particles
{
    public static class ParticleHandler
    {
        public static List<ParticleSystem> systems;

        public static void init()
        {
            systems = new List<ParticleSystem>();
        }

        public static void updateParticleSystems(GameLocation location)
        {
            systems.Clear();
            Map map = location.map;
            foreach(Layer layer in map.Layers)
            {
                string layerName = layer.Id.Split('_')[0];
                if (layerName.Equals("Back") || layerName.Equals("Buildings") || layerName.Equals("Front") || layerName.Equals("AlwaysFront"))
                {
                    for(int x = 0; x < layer.LayerWidth; x++)
                    {
                        for(int y = 0; y < layer.LayerHeight; y++)
                        {
                            if (layer.Tiles[x, y] != null && layer.Tiles[x, y].Properties.ContainsKey("Particles"))
                            {
                                string[] particles = layer.Tiles[x, y].Properties["Particles"].ToString().Split(' ');
                                foreach (string particleSystemName in particles)
                                {
                                    ParticleSystem tileSystem = new ParticleSystem(particleSystemName);
                                    tileSystem.tileLocation = new Vector2(x, y);
                                    systems.Add(tileSystem);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void draw(SpriteBatch b)
        {
            foreach(ParticleSystem system in systems)
            {
                system.draw(b);
            }
        }

        public static void update(GameTime time, GameLocation location)
        {
            foreach (ParticleSystem system in systems)
            {
                system.update(time, location);
            }
        }
    }
}
