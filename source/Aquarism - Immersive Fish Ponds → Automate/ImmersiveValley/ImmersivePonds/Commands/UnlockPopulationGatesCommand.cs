/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Ponds.Commands;

#region using directives

using Common;
using Common.Commands;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class UnlockPopulationGatesCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal UnlockPopulationGatesCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "unlock_gates";

    /// <inheritdoc />
    public override string Documentation =>
        "Unlock all population gates for the nearest pond and set max occupants to the maximum value.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length > 0)
            Log.W("Additional arguments will be ignored.");

        var ponds = Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                !p.isUnderConstruction())
            .ToHashSet();
        if (ponds.Count <= 0)
        {
            Log.W("You don't own any Fish Ponds.");
            return;
        }

        var nearest = Game1.player.GetClosestBuilding(out _, ponds);
        if (nearest is null)
        {
            Log.W("There are no ponds nearby.");
            return;
        }

        if (nearest.fishType.Value < 0)
        {
            Log.W("The nearest pond does not have a registered fish type. Try dropping a fish in it first.");
            return;
        }

        if (nearest.HasUnlockedFinalPopulationGate())
        {
            Log.W("The nearest pond has no populatio gates left to unlock.");
            return;
        }

        var data = nearest.GetFishPondData();
        nearest.lastUnlockedPopulationGate.Value = data.PopulationGates.Keys.Max();
        nearest.UpdateMaximumOccupancy();
    }
}