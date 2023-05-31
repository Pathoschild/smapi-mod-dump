/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons;

/// <summary>The current state of the <see cref="VirtueQuest"/>.</summary>
public enum VirtueQuestState
{
    /// <summary>The quest has not been started.</summary>
    NotStarted,

    /// <summary>The quest is in progress.</summary>
    InProgress,

    /// <summary>The quest has been completed.</summary>
    Completed,
}
