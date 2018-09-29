using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JoysOfEfficiency.Utils;
using StardewValley;
using StardewValley.Objects;

namespace JoysOfEfficiency.Patches
{
    using Player = Farmer;
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    internal class FarmerPatcher
    {
        private static bool Prefix(ref bool __result, ref int itemIndex, ref int quantity)
        {
            __result = CountOfItem(itemIndex) >= quantity;
            return false;
        }

        private static int CountOfItem(int itemIndex)
        {
            return Util.GetNearbyItems(Game1.player)
                .Where(item => item is Object obj &&
                               !(obj is Furniture) && 
                               (item.ParentSheetIndex == itemIndex || item.Category == itemIndex))
                .Sum(item => item.Stack);
        }
    }
}
