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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;


namespace StardewDruid.Cast
{
    internal class PetBowl : CastHandle
    {

        public PetBowl(Mod mod, Vector2 target, Rite rite)
            : base(mod, target, rite)
        {
            castCost = 0;
        }

        public override void CastWater()
        {

            WateringCan wateringCan = new();

            wateringCan.WaterLeft = 100;

            (targetLocation as Farm).performToolAction(wateringCan, (int)targetVector.X, (int)targetVector.Y);

            ModUtility.AnimateBolt(targetLocation, targetVector);

            Utility.addSprinklesToLocation(targetLocation, (int)targetVector.X - 1, (int)targetVector.Y - 1, 3, 3, 999, 333, Color.White);

            return;

        }

    }

}
