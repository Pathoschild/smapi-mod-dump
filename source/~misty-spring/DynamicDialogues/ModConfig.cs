/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace DynamicDialogues;

internal class ModConfig
{
    public bool Verbose { get; set; }
    public bool Debug { get; set; }
    public bool ChangeAt { get; set; }
    public int QuestChance { get; set; } = 30;
}