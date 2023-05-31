/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCurrentToolIndexSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCurrentToolIndexSetterPatcher"/> class.</summary>
    internal FarmerCurrentToolIndexSetterPatcher()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.CurrentToolIndex));
    }

    #region harmony patches

    /// <summary>Auto-equip cursed weapon.</summary>
    [HarmonyPrefix]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for inner functions.")]
    private static void FarmerCurrentToolIndexPostfix(Farmer __instance, ref int value)
    {
        if (!__instance.IsCursed() ||
            value < 0 || value >= __instance.Items.Count ||
            __instance.Items[value] is not MeleeWeapon weapon ||
            weapon.InitialParentTileIndex == ItemIDs.DarkSword || weapon.isScythe())
        {
            return;
        }

        var darkSword = __instance.Items.FirstOrDefault(item => item is MeleeWeapon
        {
            InitialParentTileIndex: ItemIDs.DarkSword
        });

        if (darkSword is null)
        {
            if (WeaponsModule.Config.CanStoreRuinBlade)
            {
                return;
            }

            Log.W(
                $"[WPNZ]: Cursed farmer {__instance.Name} is not carrying the Dark Sword. A new copy will be forcefully added.");
            darkSword = new MeleeWeapon(ItemIDs.DarkSword);
            if (!__instance.addItemToInventoryBool(darkSword))
            {
                Game1.createItemDebris(darkSword, __instance.getStandingPosition(), -1, __instance.currentLocation);
            }

            return;
        }

        if (Game1.random.NextDouble() > getCurseChance(darkSword.Read<int>(DataKeys.CursePoints)))
        {
            return;
        }

        value = __instance.Items.IndexOf(darkSword);

        double getCurseChance(int x)
        {
            return (0.0008888889 * x) + (0.000002222222 * x * x);
        }
    }

    #endregion harmony patches
}
