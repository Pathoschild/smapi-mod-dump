/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using xTile.Layers;
using xTile.Tiles;
using static System.Net.WebRequestMethods;

namespace StardewDruid.Event
{
    public class WispHandle
    {

        public GameLocation location;

        public Vector2 tile;

        public Vector2 position;

        public TemporaryAnimatedSprite wisp;

        public TemporaryAnimatedSprite light;

        public int source;

        public int timer;

        public bool initiated;

        public bool completed;

        public Random randomIndex;

        public int activation;

        public WispHandle(GameLocation Location, Vector2 Tile, int Source = 0, int Timer = -1)
        {

            location = Location;

            tile = Tile;

            position = tile * 64;

            source = Source;

            timer = Timer;

            randomIndex = new();

            initiate();

        }

        public void initiate()
        {

            light = new(23, 9999f, 1, 2, position, flicker: false, (randomIndex.Next(2) == 0) ? true : false)
            {
                texture = Game1.animations,
                light = true,
                lightRadius = 2,
                lightcolor = Color.Black,
                alpha = 0.6f,
                Parent = location,
            };

            location.temporarySprites.Add(light);

            bool wispFlip = randomIndex.Next(2) == 0;

            Texture2D wispTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Wisp.png"));

            wisp = new(0, 200f, 4, 80, position - new Vector2(32,64), false, false)
            {

                sourceRect = new(0, (source - 1) * 32, 32, 32),

                sourceRectStartingPos = new Vector2(0,(source-1)*32),

                texture = wispTexture,

                scale = 4, //* size,

                layerDepth = 992f,

                alpha = 0.6f,

                flipped = wispFlip,

            };

            location.temporarySprites.Add(wisp);

            initiated = true;

        }

        public bool reset()
        {

            if (!initiated)
            {

                initiate();

            }

            if(timer != -1)
            {

                timer--;

                if(timer <= 0)
                {

                    shutdown();

                    return false;

                }

            }

            if(Game1.getLocationFromName(location.Name) == null)
            {
                shutdown();

                return false;

            }

            if (!location.temporarySprites.Contains(wisp))
            {
                if(light != null)
                {
                            
                    location.temporarySprites.Remove(light);

                }

                initiate();

                completed = false;

            };

            return true;

        }

        public void shutdown()
        {
            
            location.temporarySprites.Remove(light);

            location.temporarySprites.Remove(wisp);

            location.playSound("fireball");

        }

    }

}
