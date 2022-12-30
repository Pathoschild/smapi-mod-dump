/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.VirtualProperties;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Overhaul.Modules.Rings.Resonance;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Tool_ResonatingChords
{
    internal static ConditionalWeakTable<Tool, Dictionary<Type, Chord>> Values { get; } = new();

    internal static Chord? Get_ResonatingChord<TEnchantment>(this Tool tool)
        where TEnchantment : BaseWeaponEnchantment
    {
        return Values.TryGetValue(tool, out var dict) && dict.TryGetValue(typeof(TEnchantment), out var chord)
            ? chord
            : null;
    }

    internal static Chord? Get_ResonatingChord(this Tool tool, Type type)
    {
        if (!type.IsAssignableTo(typeof(BaseWeaponEnchantment)))
        {
            ThrowHelper.ThrowInvalidOperationException($"Tried to get the resonating chord for non-enchantment type {type}");
        }

        return Values.TryGetValue(tool, out var dict) && dict.TryGetValue(type, out var chord)
            ? chord
            : null;
    }

    internal static void UpdateResonatingChord<TEnchantment>(this Tool tool, Chord newValue)
        where TEnchantment : BaseWeaponEnchantment
    {
        if (newValue.Root?.EnchantmentType != typeof(TEnchantment))
        {
            ThrowHelper.ThrowInvalidOperationException($"Type mismatch between {newValue} and {typeof(TEnchantment)}.");
        }

        var dict = Values.GetOrCreateValue(tool);
        if (dict.TryGetValue(typeof(TEnchantment), out var oldValue) && oldValue.Amplitude > newValue.Amplitude)
        {
            return;
        }

        dict[typeof(TEnchantment)] = newValue;
        tool.Invalidate();
    }

    internal static void UpdateResonatingChord(this Tool tool, Chord newValue)
    {
        var dict = Values.GetOrCreateValue(tool);
        if (dict.TryGetValue(newValue.Root!.EnchantmentType, out var oldValue) && oldValue.Amplitude > newValue.Amplitude)
        {
            return;
        }

        dict[newValue.Root.EnchantmentType] = newValue;
        tool.Invalidate();
    }

    internal static void UnsetResonatingChord<TEnchantment>(this Tool tool)
        where TEnchantment : BaseWeaponEnchantment
    {
        if (!Values.TryGetValue(tool, out var dict))
        {
            return;
        }

        if (dict.Remove(typeof(TEnchantment)))
        {
            tool.Invalidate();
        }
    }

    internal static void UnsetResonatingChord(this Tool tool, Type type)
    {
        if (!type.IsAssignableTo(typeof(BaseWeaponEnchantment)))
        {
            ThrowHelper.ThrowInvalidOperationException($"Tried to unset the resonating chord of non-enchantment type {type}");
        }

        if (!Values.TryGetValue(tool, out var dict))
        {
            return;
        }

        if (dict.Remove(type))
        {
            tool.Invalidate();
        }
    }

    internal static void UnsetAllResonatingChords(this Tool tool)
    {
        if (Values.Remove(tool))
        {
            tool.Invalidate();
        }
    }
}
