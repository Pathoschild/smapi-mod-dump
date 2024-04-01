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
using StardewDruid.Event;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Cast.Weald
{
    internal class Rockfall : CastHandle
    {

        public float damage;

        public string terrain;

        public Rockfall(Vector2 target, float Damage, string Terrain)
            : base(target)
        {
            castCost = 1;
            damage = Damage;
            terrain = Terrain;
        }

        public override void CastEffect()
        {

            SpellHandle rockfall = new(targetLocation, targetVector * 64, Game1.player.Position, 2, 1, -1, damage/3, 1);

            rockfall.type = SpellHandle.barrages.rockfall;

            rockfall.terrain = 2;

            if(terrain == "ground")
            {

                rockfall.debris = 1;

            }

            Mod.instance.spellRegister.Add(rockfall);

            castFire = true;

        }

    }

}
