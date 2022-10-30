/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using MoreEnchantments.Enchantments;
using MoreEnchantments.Patches;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace MoreEnchantments
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod Instance;

        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;
            Log.Monitor = this.Monitor;

            helper.Events.Content.AssetRequested += this.OnAssetRequested;

            BaseEnchantment.GetAvailableEnchantments().Add(new MoreLuresEnchantment()); // this gets reset when the language code is reset.

            HarmonyPatcher.Apply(this,
                new FishingRodPatcher()
            );
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Strings\\EnchantmentNames"))
                e.Edit((asset) =>
                {
                    asset.AsDictionary<string, string>().Data.Add("MoreLures", "A-lure-ing");
                });
        }
    }
}
