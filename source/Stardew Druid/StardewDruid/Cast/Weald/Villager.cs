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
using StardewDruid.Dialogue;
using StardewValley;

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

            if (Mod.instance.rite.castTask.ContainsKey("masterVillager"))
            {

                friendship = 25;

            }

            bool greetVillager = ModUtility.GreetVillager(Mod.instance.rite.caster, riteWitness, friendship);

            if (!Mod.instance.rite.castTask.ContainsKey("masterVillager") && greetVillager)
            {

                Mod.instance.UpdateTask("lessonVillager", 1);

            }

            Reaction.ReactTo(riteWitness, "Weald", friendship);

        }

    }

}
