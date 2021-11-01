/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using System.Linq;
using static StardewValley.FarmerSprite;

namespace BattleRoyale.Patches
{
    class CustomDeathAnimation : Patch
    {
        public static readonly int[] customAnimations = new int[1]
        {
            5555
        };

        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(FarmerSprite), "getAnimationFromIndex");

        public static bool Prefix(int index, FarmerSprite requester)
        {
            if (!customAnimations.Contains(index))
                return true;

            requester.loopThisAnimation = true;

            List<AnimationFrame> outFrames = requester.currentAnimation;
            outFrames.Clear();

            ModEntry.BRGame.Helper.Reflection.GetField<int>(requester, "currentSingleAnimation").SetValue(index);

            switch (index)
            {
                case 5555:
                    requester.loopThisAnimation = false;
                    outFrames.Add(new AnimationFrame(16, 200));
                    outFrames.Add(new AnimationFrame(4, 600));
                    outFrames.Add(new AnimationFrame(5, 2400));
                    break;
            }

            return false;
        }
    }
}
