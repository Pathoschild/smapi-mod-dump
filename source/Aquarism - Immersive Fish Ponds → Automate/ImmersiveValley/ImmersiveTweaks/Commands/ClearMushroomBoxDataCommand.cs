/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tweex.Commands;

#region using directives

using Common.Commands;
using Common.Data;
using Extensions;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Locations;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class ClearMushroomBoxDataCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ClearMushroomBoxDataCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "deage_shroom_boxes";

    /// <inheritdoc />
    public override string Documentation => "Clear the age data of every mushroom box in the farm cave.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        foreach (var @object in Game1.locations.OfType<FarmCave>().SelectMany(fc => fc.Objects.Values)
                     .Where(o => o.IsMushroomBox())) ModDataIO.WriteData(@object, "Age", null);
    }
}