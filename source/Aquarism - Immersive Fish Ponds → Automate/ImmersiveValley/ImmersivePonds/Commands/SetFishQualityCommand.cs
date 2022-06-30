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
using Common.Data;
using Common.Extensions;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System.Linq;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class SetFishQualityCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetFishQualityCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "set_quality";

    /// <inheritdoc />
    public override string Documentation => "Set the quality of all fish in the nearest pond.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length != 1)
        {
            Log.W("You must specify a quality (`low`, `med`, `high` or `best`).");
            return;
        }

        if (!args[0].IsIn("low", "med", "high", "best"))
        {
            Log.W("Quality should be one of `low`, `med`, `high` or `best`");
            return;
        }

        if (!Game1.player.currentLocation.Equals(Game1.getFarm()))
        {
            Log.W("You must be at the farm to do this.");
            return;
        }

        var ponds = Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                !p.isUnderConstruction())
            .ToHashSet();
        if (!ponds.Any())
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

#pragma warning disable CS8509
        var newQuality = args[0] switch
#pragma warning restore CS8509
        {
            "low" or "normal" or "regular" or "white" => SObject.lowQuality,
            "med" or "silver" => SObject.medQuality,
            "high" or "gold" => SObject.highQuality,
            "best" or "iridium" => SObject.bestQuality
        };

        var familyCount = ModDataIO.ReadDataAs<int>(nearest, "FamilyLivingHere");
        var familyQualities = new int[4];
        if (familyCount > nearest.FishCount)
        {
            Log.W("FamilyLivingHere data is invalid. The data will be reset.");
            familyCount = 0;
            ModDataIO.WriteData(nearest, "FamilyLivingHere", null);
        }

        if (familyCount > 0)
        {
            familyQualities[newQuality == 4 ? 3 : newQuality] += familyCount;
            ModDataIO.WriteData(nearest, "FamilyQualities", string.Join(',', familyQualities));
        }

        var fishQualities = new int[4];
        fishQualities[newQuality == 4 ? 3 : newQuality] += nearest.FishCount - familyCount;
        ModDataIO.WriteData(nearest, "FishQualities", string.Join(',', fishQualities));
    }
}