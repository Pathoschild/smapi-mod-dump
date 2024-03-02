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
using Netcode;
using StardewDruid.Event;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StardewDruid.Monster.Boss
{
    public class Prime : Dragon
    {

        public Prime()
        {
        }

        public Prime(Vector2 vector, int CombatModifier)
          : base(vector, CombatModifier, "BlackDragon")
        {
            HardMode();
        }

        public override void HardMode()
        {
            
            Health *= 2;

            MaxHealth = Health;

            cooldownInterval = 40;

            tempermentActive = temperment.aggressive;

        }

    }

}
