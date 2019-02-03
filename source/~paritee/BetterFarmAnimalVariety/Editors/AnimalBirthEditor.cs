using StardewModdingAPI;
using System.Collections.Generic;

namespace BetterFarmAnimalVariety.Editors
{
    class AnimalBirthEditor : IAssetEditor
    {
        private ModEntry Mod;

        public AnimalBirthEditor(ModEntry mod)
        {
            this.Mod = mod;
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            // change string events
            if (asset.AssetNameEquals("Strings/Events"))
                return true;

            return false;
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            // change string events
            if (asset.AssetNameEquals("Strings/Events"))
            {
                IDictionary<string, string> Events = asset.AsDictionary<string, string>().Data;

                // Remove the short parent type to allow for potential to expand outside the parent's type
                Events["AnimalBirth"] = this.Mod.Helper.Translation.Get("Strings.Events.AnimalBirth");
            }
        }
    }
}
