/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using CommunityToolkit.Diagnostics;

using NetEscapades.EnumGenerators;

using static System.Numerics.BitOperations;

namespace AtraShared.ConstantsAndEnums;

/// <summary>
/// Wallet items as flags....
/// </summary>
[Flags]
[EnumExtensions]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Should be obvious.")]
public enum WalletItems
{
    /// <summary>
    /// No wallet items.
    /// </summary>
    None = 0,

    BearsKnowledge = 0b1,
    ClubCard = 0b1 << 1,
    RustyKey = 0b1 << 2,
    SkullKey = 0b1 << 3,
    SpecialCharm = 0b1 << 4,
    SpringOnion = 0b1 << 5,
    TranslationGuide = 0b1 << 6,
    TownKey = 0b1 << 7,
}

/// <summary>
/// Extensions for the WalletItems enum.
/// </summary>
public static partial class WalletItemsExtensions
{
    private static readonly WalletItems[] _all = GetValues().Where(a => PopCount((uint)a) == 1).ToArray();

    /// <summary>
    /// Gets a span containing all wallet items.
    /// </summary>
    public static ReadOnlySpan<WalletItems> All => new(_all);

    /// <summary>
    /// Checks if this specific farmer has any single wallet item.
    /// </summary>
    /// <param name="farmer">Farmer to check.</param>
    /// <param name="items">Item to check for.</param>
    /// <returns>True if that farmer has this wallet item.</returns>
    public static bool HasSingleWalletItem(this Farmer farmer, WalletItems items)
    {
        Guard.IsEqualTo(PopCount((uint)items), 1);

        switch (items)
        {
            case WalletItems.BearsKnowledge:
                return farmer.eventsSeen.Contains(2120303);
            case WalletItems.ClubCard:
                return farmer.hasClubCard;
            case WalletItems.RustyKey:
                return farmer.hasRustyKey;
            case WalletItems.SkullKey:
                return farmer.hasSkullKey;
            case WalletItems.SpecialCharm:
                return farmer.hasSpecialCharm;
            case WalletItems.SpringOnion:
                return farmer.eventsSeen.Contains(3910979);
            case WalletItems.TranslationGuide:
                return farmer.canUnderstandDwarves;
            case WalletItems.TownKey:
                return farmer.HasTownKey;
        }

        ThrowHelper.ThrowArgumentOutOfRangeException($"{items.ToStringFast()} does not correspond to a single wallet item!");
        return false;
    }
}