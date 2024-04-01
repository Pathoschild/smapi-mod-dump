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
    internal class Campfire : CastHandle
    {

        public Campfire(Vector2 target)
            : base(target)
        {

            castCost = 0;

        }

        public override void CastEffect()
        {

            int currentStack = 0;

            if (!Mod.instance.rite.castTask.ContainsKey("masterCookout"))
            {

                currentStack = Mod.instance.UpdateTask("lessonCookout", 1);

            }
            else if (!Mod.instance.rite.castTask.ContainsKey("masterRecipe"))
            {

                currentStack = 2;

            }

            if (currentStack >= 2)
            {

                ModUtility.LearnRecipe(targetPlayer);

                Mod.instance.TaskSet("masterRecipe", 1);

            }

            Vector2 newVector = new(targetVector.X, targetVector.Y);

            if (targetLocation.objects.ContainsKey(targetVector))
            {
                targetLocation.objects.Remove(targetVector);

            }

            Torch campFire = new("278",true)
            {
                Fragility = 1,
                destroyOvernight = true
            };

            targetLocation.objects.Add(newVector, campFire);

            //campFire.placementAction(targetLocation, (int)newVector.X*64, (int)newVector.Y*64, targetPlayer);

            Game1.playSound("fireball");

            ModUtility.AnimateBolt(targetLocation, targetVector * 64 + new Vector2(32));

            castFire = true;

            castCost = 24;

            castLimit = true;

        }

    }

}
