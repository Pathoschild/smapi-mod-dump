/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/XxHarvzBackxX/airstrike
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Airstrike
{
    internal class Airstrike
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public GameLocation Location { get; set; } = null;
        public int Timer { get; set; } = 250;
        public bool TickTimer { get; set; } = false;
        public static List<Airstrike> Airstrikes { get; set; } = new List<Airstrike>();
        public Airstrike(Vector2 position, GameLocation location)
        {
            Position = position;
            Location = location;
            Airstrikes.Add(this);
            CallAirstrike();
        }
        public void Update()
        {
            if (TickTimer)
            {
                Timer -= 1;
            }
            if (Timer <= 0)
            {
                DoExplosion();
                Airstrikes.Remove(this);
            }
        }
        public void CallAirstrike()
        {
            Location.playSoundAt("furnace", Position);
            Timer = 250;
            TickTimer = true;
        }
        public void DoExplosion()
        {
            Location.playSoundAt("fireball", Position / Game1.tileSize);
            Location.explode(Position / Game1.tileSize, 14, Game1.player, true, 45);
        }
    }
}
