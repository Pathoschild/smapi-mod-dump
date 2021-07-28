/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using MoreEnchantments.Enchantments;
using MoreEnchantments.Patches;
using Spacechase.Shared.Harmony;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;

namespace MoreEnchantments
{
    internal class Mod : StardewModdingAPI.Mod, IAssetEditor
    {
        public static Mod Instance;

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Strings\\EnchantmentNames");
        }

        public void Edit<T>(IAssetData asset)
        {
            asset.AsDictionary<string, string>().Data.Add("MoreLures", "A-lure-ing");
        }

        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;
            Log.Monitor = this.Monitor;

            BaseEnchantment.GetAvailableEnchantments().Add(new MoreLuresEnchantment());

            HarmonyPatcher.Apply(this,
                new FishingRodPatcher()
            );
        }
    }
}
