/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Buffs;

#region using directives

using StardewValley.Buffs;

#endregion using directives

internal sealed class EcologistMaxStaminaBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.EcologistP.MaxStamina";

    internal EcologistMaxStaminaBuff(float intensity = 10f)
        : base(
            id: ID,
            source: "Ecologist",
            displaySource: _I18n.Get("ecologist.title.prestiged" + (Game1.player.IsMale ? ".male" : ".female")),
            duration: 60000,
            effects: GetBuffEffects(intensity))
    {
    }

    private static BuffEffects GetBuffEffects(float added)
    {
        if (Game1.player.buffs.AppliedBuffs.TryGetValue(ID, out var current))
        {
            return new BuffEffects { MaxStamina = { current.effects.MaxStamina.Value + added }, };
        }

        return new BuffEffects { MaxStamina = { added } };
    }
}
