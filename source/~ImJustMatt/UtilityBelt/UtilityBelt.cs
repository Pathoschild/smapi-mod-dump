/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace UtilityBelt;

using System.Linq;
using StardewValley.Objects;
using StardewValley.Tools;

/// <summary>
/// 
/// </summary>
internal class UtilityBelt
{
    private Chest _chest;

    public UtilityBelt(GenericTool tool)
    {
        this.Tool = tool;
    }

    private GenericTool Tool { get; }

    private Chest Chest
    {
        get
        {
            return this._chest ??= (this.Tool.attachments[0] = new Chest()) as Chest;
        }
    }

    private TTool GetTool<TTool>()
    {
        return this.Chest.items.OfType<TTool>().SingleOrDefault();
    }
}