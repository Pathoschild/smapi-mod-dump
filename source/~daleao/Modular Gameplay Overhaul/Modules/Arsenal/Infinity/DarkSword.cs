/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Infinity;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Arsenal.Enchantments;
using StardewValley.Tools;

#endregion using directives

/// <summary>The Dark Sword.</summary>
/// <remarks>Unused.</remarks>
[XmlType("Mods_DaLion_DarkSword")]
public sealed class DarkSword : MeleeWeapon
{
    /// <summary>Initializes a new instance of the <see cref="DarkSword"/> class.</summary>
    public DarkSword()
    : base(Constants.DarkSwordIndex)
    {
        this.AddEnchantment(new CursedEnchantment());
        this.specialItem = true;
    }
}
