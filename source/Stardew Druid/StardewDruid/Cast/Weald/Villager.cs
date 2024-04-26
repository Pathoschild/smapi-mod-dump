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
using StardewDruid.Data;
using StardewDruid.Journal;
using StardewValley;
using System.Xml.Linq;

namespace StardewDruid.Cast.Weald
{
    internal class Villager : CastHandle
    {

        NPC riteWitness;

        public Villager(Vector2 target,  NPC witness)
            : base(target)
        {

            riteWitness = witness;

        }

        public override void CastEffect()
        {

            int friendship = 0;

            if (Mod.instance.questHandle.IsComplete(QuestHandle.clearLesson))
            {
                
                friendship = 25;

            }

            ModUtility.GreetVillager(targetPlayer, riteWitness, friendship);

            ReactionData.ReactTo(riteWitness, "Weald", friendship);

        }

    }

}
