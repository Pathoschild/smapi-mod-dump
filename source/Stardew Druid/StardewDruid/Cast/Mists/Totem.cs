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
using StardewValley;

namespace StardewDruid.Cast.Mists
{
    internal class Totem : CastHandle
    {

        public int targetIndex { get; set; }


        public Totem(Vector2 target, Rite rite, int TargetIndex)
            : base(target, rite)
        {

            targetIndex = TargetIndex;

            castCost = 0;
        }

        public override void CastEffect()
        {

            int extractionChance = 1;

            if (!riteData.castTask.ContainsKey("masterTotem"))
            {

                Mod.instance.UpdateTask("lessonTotem", 1);

            }
            else
            {
                extractionChance = randomIndex.Next(1, 3);

            }

            for (int i = 0; i < extractionChance; i++)
            {
                //Game1.createObjectDebris(targetIndex, (int)targetVector.X, (int)targetVector.Y - 1);
                Throw throwObject = new(targetPlayer, targetVector * 64, targetIndex);

                throwObject.ThrowObject();
            
            }

            castFire = true;

            Vector2 boltVector = new(targetVector.X, targetVector.Y - 2);

            ModUtility.AnimateBolt(targetLocation, boltVector);

            return;

        }

    }

}
