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
using StardewDruid.Cast;
using StardewDruid.Event;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.Crops;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StardewDruid.Cast.Effect
{
    public class Gravity : EventHandle
    {


        public Dictionary<Vector2, GravityTarget> gravityWells = new();

        public Gravity()
        {

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 5;

        }

        public virtual void AddTarget(GameLocation location, Vector2 tile, int timer, float radius = 256)
        {

            if (gravityWells.ContainsKey(tile))
            {
                return;
            }

            gravityWells.Add(tile, new(location, tile, timer, radius));

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + timer;

        }

        public override void EventDecimal()
        {

            List<StardewValley.Monsters.Monster> procced = new();

            for (int g = gravityWells.Count - 1; g >= 0; g--)
            {

                KeyValuePair<Vector2, GravityTarget> gravityWell = gravityWells.ElementAt(g);

                if ((gravityWell.Value.counter <= 0))
                {

                    gravityWells.Remove(gravityWell.Key);

                }

                gravityWell.Value.counter--;

                Vector2 gravityCenter = gravityWell.Value.tile * 64;

                List<StardewValley.Monsters.Monster> victims = ModUtility.MonsterProximity(gravityWell.Value.location, new() { gravityCenter, }, gravityWell.Value.radius);

                foreach(StardewValley.Monsters.Monster victim in victims)
                {

                    if(procced.Contains(victim)) { continue; }

                    if(victim.stunTime.Value <= 0)
                    {
                        victim.stunTime.Set(1000);
                    }

                    victim.Position = ModUtility.PathMovement(victim.Position, gravityCenter, 7);

                }

                if (!gravityWell.Value.comet)
                {

                    if (Mod.instance.rite.castActive && Mod.instance.rite.castType == Rite.rites.stars)
                    {

                        //Mod.instance.rite.CastComet(gravityWell.Value.location, gravityWell.Value.tile);

                        gravityWell.Value.comet = true;

                        continue;

                    }

                    if (Mod.instance.rite.chargeActive && Mod.instance.rite.chargeType == Rite.charges.starsCharge)
                    {

                        //Mod.instance.rite.CastComet(gravityWell.Value.location, gravityWell.Value.tile);

                        gravityWell.Value.comet = true;

                        continue;

                    }

                }

            }

        }

    }

    public class GravityTarget
    {

        public Vector2 tile;

        public GameLocation location;

        public int counter;

        public int limit;

        public float radius;

        public bool comet;

        public GravityTarget(GameLocation Location, Vector2 Tile, int timer, float Radius = 256)
        {

            tile = Tile;

            counter = timer * 10;

            location = Location;

            radius = Radius;

        }

    }

}
