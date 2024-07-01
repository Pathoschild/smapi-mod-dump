/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Enchantments.Framework.Buffs;

#region using directives

using DaLion.Core.Framework;
using DaLion.Enchantments.Framework.Enchantments;

#endregion using directives

internal sealed class ExplosiveBuff : StackableBuff
{
    internal const string ID = "DaLion.Enchantments.Buffs.Explosive";
    private const int SHEET_INDEX = 59;

    internal ExplosiveBuff(Func<int> getStacks)
        : base(
            id: ID,
            getStacks: getStacks,
            maxStacks: ExplosiveEnchantment.MaxRadius,
            source: "Explosive",
            displaySource: I18n.Enchantments_Explosive_Name(),
            duration: 17,
            iconTexture: Game1.buffsIcons,
            iconSheetIndex: SHEET_INDEX,
            getDescription: stacks => I18n.Enchantments_Explosive_Desc(stacks))
    {
    }
}
