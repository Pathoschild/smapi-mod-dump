/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/XxHarvzBackxX/airstrike
**
*************************************************/

using StardewValley;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Airstrike
{
    internal class Smoke
    {
        public int SmokeTimer { get; set; } = 100;
        public int AirstrikeTimer { get; set; } = 1000;
        public bool TickTimer { get; set; } = false;
        public Vector2 MainPos { get; set; } = Vector2.Zero;
        public GameLocation MainLoc { get; set; } = null;
        public static List<Smoke> Smokes = new List<Smoke>();
        public Smoke(Vector2 position, GameLocation location)
        {
            MainLoc = location;
            MainPos = position * Game1.tileSize;
            MainLoc.temporarySprites.Add(CreateSmokePlume(MainPos));
            Smokes.Add(this);
        }
        public void Update()
        {
            SmokeTimer -= 1;
            if (SmokeTimer <= 0)
            {
                MainLoc.temporarySprites.Add(AddNewSmokeParticle(MainPos));
                AirstrikeTimer -= 1;
            }
            if (AirstrikeTimer <= 0)
            {
                SmokeTimer = 300;
                Airstrike airstrike = new Airstrike(MainPos, MainLoc);
                Smokes.Remove(this);
            }
        }
        public TemporaryAnimatedSprite CreateSmokePlume(Vector2 position)
        {
            SmokeTimer = 7;
            AirstrikeTimer = 125;
            TickTimer = true;
            return new TemporaryAnimatedSprite(Game1.mouseCursorsName, new Rectangle(372, 1956, 10, 10), position, false, 0.002f, new Color(245, 65, 53))
            {
                alpha = 0.75f,
                motion = new Vector2(0f, -0.5f),
                acceleration = new Vector2(0f, 0f),
                interval = 99999f,
                layerDepth = (position.Y + 1.1f) * 64f / 10000f,
                scale = 2.5f,
                scaleChange = 0.015f,
                rotationChange = Game1.random.Next(-5, 6) * (float)Math.PI / 256f,
                light = true,
                lightcolor = new Color(245, 65, 53),
                lightRadius = 1.5f
            };
        }
        public TemporaryAnimatedSprite AddNewSmokeParticle(Vector2 position)
        {
            SmokeTimer = 7;
            TickTimer = true;
            return new TemporaryAnimatedSprite(Game1.mouseCursorsName, new Rectangle(372, 1956, 10, 10), position, false, 0.002f, new Color(245, 65, 53))
            {
                alpha = 0.75f,
                motion = new Vector2(0f, -0.555f),
                acceleration = new Vector2(0f - Game1.random.Next(10) / 1000f, -0.00555f - (Game1.random.Next(10) / 1000f)),
                interval = 99999f,
                layerDepth = ((position.Y + 1.1f) * 64f / 10000f) - Game1.random.Next(100) / 10000f,
                scale = 2.5f + (Game1.random.Next(10) / 15f),
                scaleChange = 0.015f,
                rotationChange = Game1.random.Next(-5, 6) * (float)Math.PI / 256f,
                light = true,
                lightcolor = new Color(245, 65, 53),
                lightRadius = 1.5f
            };
        }
    }
}
