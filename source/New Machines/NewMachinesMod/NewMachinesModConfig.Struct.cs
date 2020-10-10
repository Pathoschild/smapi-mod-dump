/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.NewMachinesMod.Data;
using Newtonsoft.Json;

namespace Igorious.StardewValley.NewMachinesMod
{
    public partial class NewMachinesModConfig
    {
        public class BundlesInformation
        {
            #region	Constructors

            [JsonConstructor]
            public BundlesInformation() { }

            public BundlesInformation(IEnumerable<OverridedBundleInformation> added, IEnumerable<OverridedBundleInformation> removed)
            {
                Added = added.ToList();
                Removed = removed.ToList();
            }

            #endregion

            #region	Properties

            [JsonProperty]
            public List<OverridedBundleInformation> Added { get; set; } = new List<OverridedBundleInformation>();

            [JsonProperty]
            public List<OverridedBundleInformation> Removed { get; set; } = new List<OverridedBundleInformation>();

            #endregion
        }

        public enum LocalizationString
        {
            TankRequiresWater,
        }

        public List<WarpTotemInformation> Totems { get; set; } = new List<WarpTotemInformation>();
        public List<MachineInformation> SimpleMachines { get; set; } = new List<MachineInformation>();
        public List<OverridedMachineInformation> MachineOverrides { get; set; } = new List<OverridedMachineInformation>();
        public MachineInformation Tank { get; set; }
        public MachineInformation Mixer { get; set; }
        public MachineInformation Separator { get; set; }
        public MachineInformation Churn { get; set; }
        public MachineInformation Fermenter { get; set; }
        public List<CookingRecipeInformation> CookingRecipes { get; set; } = new List<CookingRecipeInformation>();
        public List<CraftingRecipeInformation> CraftingRecipes { get; set; } = new List<CraftingRecipeInformation>();
        public List<ItemInformation> ItemOverrides { get; set; } = new List<ItemInformation>();
        public List<ItemInformation> Items { get; set; } = new List<ItemInformation>();
        public List<CropInformation> Crops { get; set; } = new List<CropInformation>();
        public Dictionary<LocalizationString, string> LocalizationStrings { get; set; } = new Dictionary<LocalizationString, string>();
        public List<GiftPreferences> GiftPreferences { get; set; } = new List<GiftPreferences>();
        public BundlesInformation Bundles { get; set; } = new BundlesInformation();
    }
}
