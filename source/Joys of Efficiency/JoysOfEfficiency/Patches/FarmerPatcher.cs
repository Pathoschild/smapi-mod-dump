using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoysOfEfficiency.Utils;
using StardewValley;
using StardewValley.Objects;
using SVObject = StardewValley.Object;

namespace JoysOfEfficiency.Patches
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal class FarmerPatcher
    {
        internal static bool Prefix(ref int __result, ref int item_index)
        {
            __result = CountOfItem(item_index);
            return false;
        }

        private static int CountOfItem(int index)
        {
            return Util.GetNearbyItems(Game1.player)
                .Where(item => item is SVObject obj &&
                               !(obj is Furniture) &&
                               (item.ParentSheetIndex == index || item.Category == index)
                               )
                .Sum(item => item.Stack);
        }
    }
}
