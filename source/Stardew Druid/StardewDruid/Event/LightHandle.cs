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

namespace StardewDruid.Event
{
    public class LightHandle
    {

        public GameLocation location;

        public Vector2 tile;

        public Vector2 position;

        public TemporaryAnimatedSprite brazier;

        public TemporaryAnimatedSprite column;

        public TemporaryAnimatedSprite fire;

        public TemporaryAnimatedSprite light;

        public TemporaryAnimatedSprite shadow;

        public int source;

        public int timer;

        public bool initiated;

        public bool completed;

        public Random randomIndex;

        public LightHandle(GameLocation Location, Vector2 Tile, int Source = 0, int Timer = -1)
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

            switch (source) {

                default: // brazier

                    light = new(23, 9999f, 6, 1, position, flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false)
                    {
                        texture = Game1.animations,
                        light = true,
                        lightRadius = 4,
                        lightcolor = Color.Black,
                        //alphaFade = 0.03f - 3f * 0.003f,
                        Parent = location
                    };

                    location.temporarySprites.Add(light);

                    Texture2D brazierTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Brazier.png"));

                    Texture2D redFire = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Fire.png"));

                    brazier = new(0, 99999f, 1, 1, position - new Vector2(0, 64), false, false)
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

                        texture = redFire,

                        scale = 2f, //* size,

                        layerDepth = 992f,

                    };

                    location.temporarySprites.Add(fire);

                    Microsoft.Xna.Framework.Rectangle shadowRect = Mod.instance.iconData.CursorRect(Data.IconData.cursors.shadow);

                    shadow = new(0, 99999f, 1, 1, position + new Vector2(-8, 4), false, false)
                    {

                        sourceRect = shadowRect,

                        sourceRectStartingPos = new Vector2(shadowRect.X, shadowRect.Y),

                        texture = Mod.instance.iconData.cursorTexture,

                        scale = 2.5f, //* size,

                        layerDepth = tile.Y / 10000 - 0.0001f,

                        color = Color.White * 0.25f,

                    };

                    location.temporarySprites.Add(shadow);

                    Layer back = location.map.GetLayer("Back");

                    Layer buildings = location.map.GetLayer("Buildings");

                    buildings.Tiles[(int)tile.X, (int)tile.Y] = back.Tiles[(int)tile.X, (int)tile.Y];

                    completed = true;

                    break;
            
            }

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

            switch (source)
            {

                default: // brazier

                    light.reset();

                    brazier.reset();

                    fire.reset();

                    shadow.reset();

                    column.reset();

                    break;

            }

            return true;

        }

        public void shutdown()
        {

            switch (source)
            {

                default: // brazier

                    location.temporarySprites.Remove(light);

                    location.temporarySprites.Remove(brazier);

                    location.temporarySprites.Remove(fire);

                    location.temporarySprites.Remove(shadow);

                    location.temporarySprites.Remove(column);

                    Layer buildings = location.map.GetLayer("Buildings");

                    buildings.Tiles[(int)tile.X, (int)tile.Y] = null;

                    break;

            }

            location.playSound("fireball");

        }

    }

}
