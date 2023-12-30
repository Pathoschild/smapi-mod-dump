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
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Event.World
{
    public class Levitate : EventHandle
    {

        public NPC npc;

        public Vector2 oldPosition;

        public float oldRotation;

        public float upwards;

        public bool positionReset;

        public int levitationCounter;

        public Levitate(Vector2 target, Rite rite, NPC NPC)
            : base(target, rite)
        {

            npc = NPC;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 5;

            oldPosition = npc.position;

            oldRotation = npc.rotation;

        }

        public override void EventTrigger()
        {

            Mod.instance.RegisterEvent(this, "levitate"+npc.Name);

        }

        public override bool EventActive()
        {

            if(levitationCounter >= 20)
            {

                return false;

            }

            if (expireEarly)
            {

                return false;

            }

            if (targetPlayer.currentLocation.Name != targetLocation.Name)
            {

                return false;

            }

            if (expireTime < Game1.currentGameTime.TotalGameTime.TotalSeconds)
            {

                return false;

            }

            return true;

        }

        public override void EventRemove()
        {

            //npc.Position = oldPosition;

            if (!positionReset)
            {
                
                npc.position.Y += upwards;

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

            levitationCounter++;

            npc.position.X = oldPosition.X;

            npc.rotation += (float)Math.PI / 10;

            if (levitationCounter <= 10)
            {

                npc.position.Y -= 6.4f;

            }
            else
            {

                npc.position.Y += 6.4f;

            }

            if (levitationCounter >= 20)
            {

                npc.Position = oldPosition;

                npc.rotation = oldRotation;

                positionReset = true;

                expireEarly = true;

            }
            //npc.faceDirection(activeCounter);

            /*
            if (activeCounter <= 1)
            {

                npc.position.X = oldPosition.X;

                npc.position.Y -= 6.4f;

                upwards += 6.4f;

                

            }

            if(activeCounter == 2)
            {

                npc.position.X = oldPosition.X;

                npc.rotation += (float)Math.PI / 15;

            }

            if (activeCounter == 3)
            {

                float down = upwards / 10;

                npc.position.X = oldPosition.X;

                npc.position.Y += down;

                upwards -= down;

                float radiansLeft = (float)Math.PI - npc.rotation;
               
                if(radiansLeft > 0)
                {

                    npc.rotation += radiansLeft / 10;

                }

            }

            if(activeCounter == 4)
            {

                npc.position.Y += upwards;

                npc.rotation = oldRotation;

                positionReset = true;

                expireEarly = true;

            }*/

        }

    }

}
