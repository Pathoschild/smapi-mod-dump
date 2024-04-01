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
using StardewDruid.Event;
using StardewDruid.Map;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace StardewDruid.Monster.Boss
{
    public class Demonki : Gargoyle
    {

        public Demonki()
        {
        }

        public Demonki(Vector2 vector, int CombatModifier)
          : base(vector, CombatModifier, "Demonki")
        {

        }

        public override void GargoyleSpecial()
        {
            base.GargoyleSpecial();
            specialScheme = SpellHandle.schemes.fire;
        }

    }

}
