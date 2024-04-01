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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace StardewDruid.Cast.Mists
{
    public class ShieldEvent : EventHandle
    {

        public ShieldEvent(Vector2 target)
          : base(target)
        {

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 3.0;

        }

        public override void EventTrigger()
        {
            
            Mod.instance.RegisterEvent(this, "shield");

            targetPlayer.temporarilyInvincible = true;

            targetPlayer.temporaryInvincibilityTimer = 1;

            targetPlayer.currentTemporaryInvincibilityDuration = 1000;

        }

    }

}
