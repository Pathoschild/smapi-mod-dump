/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Harmony;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley.Menus;

namespace JsonAssets.Patches
{
    /// <summary>Applies Harmony patches to <see cref="ShopMenu"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class ShopMenuPatcher : BasePatcher
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The last owner for which a <see cref="ShopMenu"/> was constructed.</summary>
        public static string LastShopOwner { get; private set; }


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<ShopMenu>(nameof(ShopMenu.setUpShopOwner)),
                prefix: this.GetHarmonyMethod(nameof(ShopMenuPatcher.Before_SetUpShopOwner))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="ShopMenu.setUpShopOwner"/>.</summary>
        private static void Before_SetUpShopOwner(string who)
        {
            ShopMenuPatcher.LastShopOwner = who;
        }
    }
}
