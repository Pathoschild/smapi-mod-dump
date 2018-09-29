using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverlastingBaitsAndUnbreakableTacklesMod
{
    public enum BaitTackle
    {
        [Description("Everlasting Bait")]
        EverlastingBait = 685,
        [Description("Everlasting Wild Bait")]
        EverlastingWildBait = 774,
        [Description("Everlasting Magnet")]
        EverlastingMagnet = 703,
        [Description("Unbreakable Spinner")]
        UnbreakableSpinner = 686,
        [Description("Unbreakable Lead Bobber")]
        UnbreakableLeadBobber = 692,
        [Description("Unbreakable Trap Bobber")]
        UnbreakableTrapBobber = 694,
        [Description("Unbreakable Cork Bobber")]
        UnbreakableCorkBobber = 695,
        [Description("Unbreakable Treasure Hunter")]
        UnbreakableTreasureHunter = 693,
        [Description("Unbreakable Barbed Hook")]
        UnbreakableBarbedHook = 691,
        [Description("Unbreakable Dressed Spinner")]
        UnbreakableDressedSpinner = 687
    }
}
