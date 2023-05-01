/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Weapons.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Dark Sword.</summary>
[XmlType("Mods_DaLion_CursedEnchantment")]
public class CursedEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override bool IsSecondaryEnchantment()
    {
        return true;
    }

    /// <inheritdoc />
    public override bool IsForge()
    {
        return false;
    }

    /// <inheritdoc />
    public override int GetMaximumLevel()
    {
        return 1;
    }

    /// <inheritdoc />
    public override bool ShouldBeDisplayed()
    {
        return false;
    }

    /// <inheritdoc />
    protected override void _OnEquip(Farmer who)
    {
        base._OnEquip(who);
        if (!who.mailReceived.Contains("gotDarkSword"))
        {
            who.mailReceived.Add("gotDarkSword");
        }

        EventManager.Enable<CurseUpdateTickedEvent>();
    }

    /// <inheritdoc />
    protected override void _OnUnequip(Farmer who)
    {
        base._OnUnequip(who);
        EventManager.Disable<CurseUpdateTickedEvent>();
    }

    /// <inheritdoc />
    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        base._OnMonsterSlay(m, location, who);
        if (who.CurrentTool is not MeleeWeapon { InitialParentTileIndex: ItemIDs.DarkSword } darkSword)
        {
            return;
        }

        darkSword.Increment(DataKeys.CursePoints);
        if (darkSword.Read<int>(DataKeys.CursePoints) >= 50 && !who.hasOrWillReceiveMail("viegoCurse"))
        {
            who.mailForTomorrow.Add("viegoCurse");
        }
    }
}
