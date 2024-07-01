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
using StardewDruid.Data;
using StardewDruid.Render;
using StardewValley;

namespace StardewDruid.Monster
{
    public class DarkBrute : DarkRogue
    {

        public DarkBrute()
        {


        }

        public DarkBrute(Vector2 vector, int CombatModifier, string name = "DarkBrute")
          : base(vector, CombatModifier, name)
        {

        }


        public override void LoadOut()
        {

            baseMode = 2;

            baseJuice = 3;
            
            basePulp = 30;

            cooldownInterval = 180;

            DarkWalk();

            DarkFlight();

            DarkSmash();

            DarkBrawl();

            weaponRender = new();

            weaponRender.melee = false;

            overHead = new(16, -144);

            loadedOut = true;

        }

    }

}

