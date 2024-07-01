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
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Event;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Cast.Fates
{
    public class Levitate : EventHandle
    {

        public NPC npc;

        public Vector2 oldPosition;

        public float oldRotation;

        public float upwards;

        public Levitate(NPC witness)
        {

            npc = witness;

            activeLimit = 5;

            oldPosition = npc.Position;

            oldRotation = npc.rotation;

        }

        public override void EventRemove()
        {

            if (!eventComplete)
            {

                npc.Position = oldPosition;

                npc.rotation = oldRotation;

            }

            base.EventRemove();

        }

        public override void EventDecimal()
        {

            if (!EventActive())
            {
                return;
            }

            npc.Halt();

            decimalCounter++;

            npc.position.X = oldPosition.X;

            npc.rotation += (float)Math.PI / 10;

            if (decimalCounter <= 10)
            {

                npc.position.Y -= 6.4f;

            }
            else
            {

                npc.position.Y += 6.4f;

            }

            if (decimalCounter >= 20)
            {

                npc.Position = oldPosition;

                npc.rotation = oldRotation;

                eventComplete = true;

            }

        }

    }

}
