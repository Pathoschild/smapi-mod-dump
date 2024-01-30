/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewDruid.Cast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewDruid.Dialogue;
using StardewValley;

namespace StardewDruid.Event
{
    public class StaticHandle : EventHandle
    {
        public StaticHandle()
            : base(Vector2.Zero, new StardewDruid.Cast.Rite())
        {

        }

        public override void EventTrigger()
        {

            Mod.instance.eventRegister.Add("static", this);

        }

        public void AddBrazier(GameLocation location, Vector2 tile)
        {

            braziers.Add(new(location, tile));

        }

        public override bool EventActive()
        {
            
            return true;
        
        }

        public override void EventInterval()
        {
            activeCounter++;

            if(activeCounter == 10)
            {

                ResetBraziers();

                activeCounter = 0;

            }

        }

    }

}
