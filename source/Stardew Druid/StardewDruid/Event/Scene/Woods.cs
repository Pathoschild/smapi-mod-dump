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
using StardewDruid.Cast;
using StardewDruid.Character;
using StardewDruid.Map;
using StardewValley;
using System.Collections.Generic;

namespace StardewDruid.Event.Scene
{
    public class Woods : EventHandle
    {

        public Woods(Vector2 target,  Quest quest)
          : base(target)
        {
            questData = quest;
        }

        public override void EventTrigger()
        {
            Mod.instance.CompleteQuest(questData.name);
        }

    }

}
