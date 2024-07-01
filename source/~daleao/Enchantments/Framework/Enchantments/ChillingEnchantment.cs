/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Core.Framework.Enchantments;
using DaLion.Core.Framework.Extensions;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Progressively chill enemies on hit for 2 seconds, freezing after stacking 3 times.</summary>
[XmlType("Mods_DaLion_ChillingEnchantment")]
public sealed class ChillingEnchantment : BaseSlingshotEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Chilling_Name();
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        base._OnDealDamage(monster, location, who, ref amount);
        monster.Chill(2000, 0.2f, 0.5f);
        SoundBox.ChillingShot.PlayLocal(location);
    }
}
