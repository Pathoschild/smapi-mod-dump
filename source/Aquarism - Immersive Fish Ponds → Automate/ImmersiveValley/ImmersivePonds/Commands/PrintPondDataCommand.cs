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
internal sealed class PrintPondDataCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintPondDataCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "data";

    /// <inheritdoc />
    public override string Documentation => "Print all mod data fields for the nearest pond.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length > 0)
            Log.W("Additional arguments will be ignored.");

        var ponds = Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                !p.isUnderConstruction())
            .ToHashSet();
        if (ponds.Count < 0)
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
            var daysEmpty = ModDataIO.ReadFrom<int>(nearest, "DaysEmpty");
            Log.I($"Empty for {daysEmpty} days.");
            return;
        }

        var fish = nearest.GetFishObject();
        var message = $"{fish.Name} pond's mod data:";

        var fishQualities = ModDataIO.ReadFrom(nearest, "FishQualities").ParseList<int>()!;
        message += "\n\tFish qualities:" +
                   $"\n\t\t- Regular: {fishQualities[0]}" +
                   $"\n\t\t- Silver: {fishQualities[1]}" +
                   $"\n\t\t- Gold: {fishQualities[2]}" +
                   $"\n\t\t- Iridium: {fishQualities[3]}";

        if (fish.HasContextTag("fish_legendary"))
        {
            var familyLivingHere = ModDataIO.ReadFrom<int>(nearest, "FamilyLivingHere");
            message += $"\n\n\tExtended family members: {familyLivingHere}";
            if (familyLivingHere > 0)
            {
                var familyQualities = ModDataIO.ReadFrom(nearest, "FamilyQualities").ParseList<int>()!;
                message += "\n\n\tFamily member qualities:" +
                           $"\n\t\t- Regular: {familyQualities[0]}" +
                           $"\n\t\t- Silver: {familyQualities[1]}" +
                           $"\n\t\t- Gold: {familyQualities[2]}" +
                           $"\n\t\t- Iridium: {familyQualities[3]}";
            }
        }
        else if (fish.IsAlgae())
        {

            var seaweedLivingHere = ModDataIO.ReadFrom<int>(nearest, "SeaweedLivingHere");
            var greenAlgaeLivingHere = ModDataIO.ReadFrom<int>(nearest, "GreenAlgaeLivingHere");
            var whiteAlgaeLivingHere = ModDataIO.ReadFrom<int>(nearest, "WhiteAlgaeLivingHere");
            message += "\n\n\tAlgae species living here:" +
                       $"\n\t\t- Seaweed: {seaweedLivingHere}" +
                       $"\n\t\t- Green Algae: {greenAlgaeLivingHere}" +
                       $"\n\t\t- White Algae: {whiteAlgaeLivingHere}";
        }

        var held = ModDataIO.ReadFrom(nearest, "ItemsHeld").ParseList<string>(";");
        if (held?.Count is > 0)
        {
            message += "\n\n\tAdditional items held:";
            foreach (var item in held)
            {
                var (index, stack, quality) = item.ParseTuple<int, int, int>()!.Value;
                var @object = new SObject(index, stack);
#pragma warning disable CS8509
                var qualityString = quality switch
#pragma warning restore CS8509
                {
                    SObject.lowQuality => "regular",
                    SObject.medQuality => "silver",
                    SObject.highQuality => "gold",
                    SObject.bestQuality => "iridium"
                };
                message += $"\n\t\t- {@object.Name} x{stack} ({qualityString})";
            }
        }
        else
        {
            message += "\n\n\tThe pond holds no items.:";
        }

        var hasOrHasnt = ModDataIO.ReadFrom<bool>(nearest, "CheckedToday") ? "has" : "hasn't";
        message += $"\n\n\tThe pond {hasOrHasnt} been checked today.";
        Log.I(message);
    }
}