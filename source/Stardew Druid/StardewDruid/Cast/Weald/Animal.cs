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

namespace StardewDruid.Cast.Weald
{
    internal class Animal : CastHandle
    {

        FarmAnimal riteWitness;

        public Animal(Vector2 target, Rite rite, FarmAnimal animal)
            : base(target, rite)
        {

            riteWitness = animal;

        }
        public override void CastEffect()
        {

            ModUtility.PetAnimal(riteData.caster, riteWitness);

        }
    }

}
