/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.Commands;

#region using directives

using DaLion.Shared.Commands;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ClearGemstonesCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ClearGemstonesCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ClearGemstonesCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "clear_gemstones", "clear" };

    /// <inheritdoc />
    public override string Documentation => "Remove all gemstones from the selected infinity band.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (!Globals.InfinityBandIndex.HasValue)
        {
            Log.W("The Infinity Band is not loaded.");
            return;
        }

        if (Game1.player.CurrentItem is not CombinedRing combined || combined.ParentSheetIndex != Globals.InfinityBandIndex.Value)
        {
            Log.W("You must select an Infinity Band first.");
            return;
        }

        combined.combinedRings.Clear();
    }
}
