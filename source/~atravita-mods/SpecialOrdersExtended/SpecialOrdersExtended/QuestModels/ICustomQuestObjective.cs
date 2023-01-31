/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace SpecialOrdersExtended.QuestModels;

/// <summary>
/// An interface that marks custom quest objectives.
/// </summary>
public interface ICustomQuestObjective
{
    /// <summary>
    /// The order this belongs to.
    /// </summary>
    public string Order { get; set; }

    /// <summary>
    /// The position of the order in the list.
    /// This is used to disambiguate multiple of the same order.
    /// </summary>
    public int Position { get; set; }
}

/// <summary>
/// An interface that marks custom quest rewards.
/// </summary>
public interface ICustomQuestReward
{
    /// <summary>
    /// The order this belongs to.
    /// </summary>
    public string Order { get; set; }

    /// <summary>
    /// The position of the order in the list.
    /// This is used to disambiguate multiple of the same order.
    /// </summary>
    public int Position { get; set; }
}
