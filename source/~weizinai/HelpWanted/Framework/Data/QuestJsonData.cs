/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace HelpWanted.Framework.Data;

internal class QuestJsonData
{
    public QuestType QuestType { get; set; }
    public string Requirement { get; set; } = null!;
    public int Number { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Target { get; set; } = null!;
    public string TargetMessage { get; set; } = null!;
    public string CurrentObjective { get; set; } = null!;
    public string Condition { get; set; } = null!;
}