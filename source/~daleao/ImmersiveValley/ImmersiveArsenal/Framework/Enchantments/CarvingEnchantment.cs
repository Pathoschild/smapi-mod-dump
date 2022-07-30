/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using StardewValley;
using StardewValley.Monsters;

#endregion using directives

public class CarvingEnchantment : BaseWeaponEnchantment
{
    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        if (Game1.random.NextDouble() < 0.5)
            location.monsterDrop(m, m.getTileX(), m.getTileY(), who);
    }

    public override string GetName()
    {
        return "Carving";
    }
}