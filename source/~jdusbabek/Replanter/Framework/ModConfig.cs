/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewLib;

namespace Replanter.Framework
{
    internal class ModConfig : IConfig
    {
        public string KeyBind { get; set; } = "J";

        public bool Free { get; set; }

        public int SeedDiscount { get; set; }

        public string WhoChecks { get; set; } = "spouse";

        public bool EnableMessages { get; set; } = true;

        // If negative, don't add to inventory.
        public float CostPerCropHarvested { get; set; } = 0.5f;

        public bool SellHarvestedCropsImmediately { get; set; }

        public bool WaterCrops { get; set; }

        public string IgnoreList { get; set; } = "591|593|595|597|376";

        public string AlwaysSellList { get; set; } = "";

        public string NeverSellList { get; set; } = "";

        // The X, Y coordinates of a chest, into which surplus items can be deposited.  The farmers inventory will be tried first, unless bypassInventory is true.
        public Vector2 ChestCoords { get; set; } = new Vector2(70, 14);

        // Whether to bypass the user's inventory and try depositing to the chest first.  Will fall back to the inventory if no chest is present.
        public bool BypassInventory { get; set; }

        public string ChestDefs { get; set; } = "258,70,14|282,70,14";

        public bool ClearDeadPlants { get; set; } = true;

        public bool SmartReplantingEnabled { get; set; } = true;
    }
}
