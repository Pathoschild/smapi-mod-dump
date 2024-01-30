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
using System.IO;
using System.Linq;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event
{
    public class Brazier
    {

        public GameLocation location;

        public Vector2 tile;

        public Vector2 position;

        public TemporaryAnimatedSprite brazier;

        public TemporaryAnimatedSprite column;

        public TemporaryAnimatedSprite fire;

        public TemporaryAnimatedSprite light;

        public TemporaryAnimatedSprite shadow;

        public Brazier(GameLocation Location, Vector2 Tile)
        {

            location = Location;

            tile = Tile;

            position = tile * 64;

            light = new(23, 99999f, 6, 1, position, flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false)
            {
                light = true,
                lightRadius =4,
                lightcolor = Color.Black,
                alphaFade = 0.03f - 3f * 0.003f,
                Parent = location
            };

            location.temporarySprites.Add(light);

            Texture2D brazierTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Brazier.png"));

            Texture2D fireTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "RedFire.png"));

            Texture2D shadowTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Shadow.png"));

            brazier = new(0, 99999f, 1, 1, position  - new Vector2(0,64), false, false)
            {

                sourceRect = new(0, 0, 32, 32),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = brazierTexture,

                scale = 2f, //* size,

                layerDepth = 991f,

            };

            location.temporarySprites.Add(brazier);

            column = new(0, 99999f, 1, 1, position, false, false)
            {

                sourceRect = new(0, 32, 32, 32),

                sourceRectStartingPos = new Vector2(0, 32),

                texture = brazierTexture,

                scale = 2f, //* size,

                layerDepth = tile.Y / 10000,

            };

            location.temporarySprites.Add(column);

            fire = new(0, 100f, 4, 99999, position - new Vector2(0, 96), false, false)
            {

                sourceRect = new(0, 0, 32, 32),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = fireTexture,

                scale = 2f, //* size,

                layerDepth = 992f,

            };

            location.temporarySprites.Add(fire);

            shadow = new(0, 99999f, 1, 1, position + new Vector2(-8, 4), false, false)
            {

                sourceRect = new(0, 0, 32, 32),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = shadowTexture,

                scale = 2.5f, //* size,

                layerDepth = tile.Y / 10000 - 0.0001f,

                color = Color.White * 0.25f,

            };

            location.temporarySprites.Add(shadow);

            Layer back = location.map.GetLayer("Back");

            Layer buildings = location.map.GetLayer("Buildings");

            buildings.Tiles[(int)tile.X, (int)tile.Y] = back.Tiles[(int)tile.X, (int)tile.Y];

        }

        public bool reset()
        {

            if(Game1.getLocationFromName(location.Name) == null)
            {
                shutdown();

                return false;

            }

            light.reset();

            brazier.reset();

            fire.reset();

            shadow.reset();

            column.reset();

            return true;

        }

        public void shutdown()
        {

            location.temporarySprites.Remove(light);

            location.temporarySprites.Remove(brazier);

            location.temporarySprites.Remove(fire);

            location.temporarySprites.Remove(shadow);

            location.temporarySprites.Remove(column);

            Layer buildings = location.map.GetLayer("Buildings");

            buildings.Tiles[(int)tile.X, (int)tile.Y] = null;

            location.playSound("fireball");

        }

    }

}
