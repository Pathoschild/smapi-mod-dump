/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Ponds.Framework.Extensions;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Mods;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
internal static class FishPondExtensions
{
    /// <summary>Determines whether a legendary fish lives in this <paramref name="pond"/>.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="pond"/> houses a legendary fish species, otherwise <see langword="false"/>.</returns>
    internal static bool HasBossFish(this FishPond pond)
    {
        return !string.IsNullOrEmpty(pond.fishType.Value) && pond.GetFishObject().IsBossFish();
    }

    /// <summary>Determines whether this <paramref name="pond"/> is infested with algae.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="pond"/> houses any algae, otherwise <see langword="false"/>.</returns>
    internal static bool HasAlgae(this FishPond pond)
    {
        return !string.IsNullOrEmpty(pond.fishType.Value) && pond.GetFishObject().IsAlgae();
    }

    /// <summary>Determines whether the <paramref name="pond"/>'s radioactivity is sufficient to enrich metallic nuclei.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="pond"/> houses a mutant or radioactive fish species, otherwise <see langword="false"/>.</returns>
    internal static bool IsRadioactive(this FishPond pond)
    {
        return !string.IsNullOrEmpty(pond.fishType.Value) && pond.GetFishObject().IsRadioactiveFish() && pond.FishCount > 1;
    }

    /// <summary>Performs object production.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="r">Optional <see cref="Random"/> number generator.</param>
    /// <returns>The most valuable produce <see cref="Item"/>, if any.</returns>
    internal static Item? GetProduce(this FishPond pond, Random? r = null)
    {
        r ??= new Random(Guid.NewGuid().GetHashCode());
        Item? result = null;
        var fish = pond.GetFishObject();
        var held = pond.DeserializeHeldItems();
        if (pond.output.Value is { } output)
        {
            var o = ItemRegistry.Create<SObject>(
                output.QualifiedItemId,
                output.Stack,
                output.Quality);
            held.Add(o);
        }

        // handle algae, which have no FishPondData, first
        if (fish.IsAlgae())
        {
            return ProduceForAlgae(pond, held, r);
        }

        ProduceFromPondData(pond, held, r);

        // handle roe/ink
        if (fish.QualifiedItemId == QualifiedObjectIds.Coral)
        {
            ProduceForCoral(pond, held, pond.GetRoeChance(fish.Price), r);
        }
        else if (fish.ItemId is "MNF.MoreNewFish_tui" or "MNF.MoreNewFish_la")
        {
            ProduceForTuiLa(pond, held, fish.ItemId, pond.GetRoeChance(fish.Price), r);
        }
        else
        {
            ProduceRoe(pond, fish, held, r);

            // check for enriched metals
            if (pond.IsRadioactive())
            {
                ProduceRadioactive(pond, held);
            }
        }

        if (held.Count == 0)
        {
            return result;
        }

        // choose output
        Utility.consolidateStacks(held);
        result = held.MaxBy(h => h.salePrice()) as SObject;
        if (result is null)
        {
            return result;
        }

        held.Remove(result);
        if (held.Count > 0)
        {
            var serialized = held
                .Take(11)
                .Select(i => $"{i.QualifiedItemId},{i.Stack},{((SObject)i).Quality}");
            Data.Write(pond, DataKeys.ItemsHeld, string.Join(';', serialized));
        }
        else
        {
            Data.Write(pond, DataKeys.ItemsHeld, null);
        }

        if (result.QualifiedItemId != QualifiedObjectIds.Roe)
        {
            return result;
        }

        if (fish.IsBossFish())
        {
            var bosses = pond.ParsePondFishes();
            if (bosses.Choose().Id is { } id)
            {
                fish = ItemRegistry.Create<SObject>($"(O){id}");
            }
        }

        var roe = ItemRegistry.GetObjectTypeDefinition().CreateFlavoredRoe(fish);
        roe.Stack = result.Stack;
        roe.Quality = result.Quality;
        result = roe;
        return result; // don't run original logic
    }

    /// <summary>Gets the number of days required to enrich a given <paramref name="metal"/> resource.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="metal">An ore or bar <see cref="SObject"/>.</param>
    /// <returns>The number of days required to enrich the nucleus of the metal.</returns>
    internal static int GetEnrichmentTime(this FishPond pond, SObject metal)
    {
        var populationFactor = pond.FishCount / 3;
        if (populationFactor == 0)
        {
            return 0;
        }

        var days = 0;
        if (metal.Name.Contains("Copper"))
        {
            days = 13;
        }
        else if (metal.Name.Contains("Iron"))
        {
            days = 8;
        }
        else if (metal.Name.Contains("Gold"))
        {
            days = 5;
        }
        else if (metal.Name.Contains("Iridium"))
        {
            days = 3;
        }

        return Math.Max(days - populationFactor, 1);
    }

    /// <summary>Gives the player fishing experience for harvesting the <paramref name="pond"/>.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="who">The player.</param>
    internal static void RewardExp(this FishPond pond, Farmer who)
    {
        if (Data.ReadAs<bool>(pond, DataKeys.CheckedToday))
        {
            return;
        }

        var bonus = (int)(pond.output.Value is SObject obj
            ? obj.sellToStorePrice() * FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER
            : 0);
        who.gainExperience(Farmer.fishingSkill, FishPond.HARVEST_BASE_EXP + bonus);
    }

    /// <summary>
    ///     Opens an <see cref="ItemGrabMenu"/> instance to allow retrieve multiple items from the
    ///     <paramref name="pond"/>'s chum bucket.
    /// </summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="who">The <see cref="Farmer"/> interacting with the <paramref name="pond"/>.</param>
    /// <returns>Always <see langword="true"/> (required by vanilla code).</returns>
    internal static bool OpenChumBucketMenu(this FishPond pond, Farmer who)
    {
        var held = pond.DeserializeHeldItems();
        if (held.Count == 0)
        {
            if (who.addItemToInventoryBool(pond.output.Value))
            {
                Game1.playSound("coin");
                pond.output.Value = null;
            }
            else
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
            }
        }
        else
        {
            var inventory = new List<Item> { pond.output.Value };
            try
            {
                foreach (var item in held)
                {
                    if (item.QualifiedItemId == QualifiedObjectIds.Roe)
                    {
                        var roe = ItemRegistry.GetObjectTypeDefinition().CreateFlavoredRoe(pond.GetFishObject());
                        roe.Stack = item.Stack;
                        roe.Quality = ((SObject)item).Quality;
                        inventory.Add(roe);
                    }
                    else
                    {
                        inventory.Add(item);
                    }
                }

                Utility.consolidateStacks(inventory);
                var menu = new ItemGrabMenu(inventory, pond).setEssential(false);
                menu.source = ItemGrabMenu.source_fishingChest;
                Game1.activeClickableMenu = menu;
            }
            catch (InvalidOperationException ex)
            {
                Log.W($"ItemsHeld data is invalid. {ex}\nThe data will be reset");
                Data.Write(pond, DataKeys.ItemsHeld, null);
            }
        }

        Data.Write(pond, DataKeys.CheckedToday, true.ToString());
        return true; // expected by vanilla code
    }

    /// <summary>
    ///     Reads the serialized held item list from the <paramref name="pond"/>'s <seealso cref="ModDataDictionary"/> and
    ///     returns a deserialized <see cref="List{T}"/> of <seealso cref="SObject"/>s.
    /// </summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="Item"/>s encoded in the <paramref name="pond"/>'s held items data.</returns>
    internal static List<Item> DeserializeHeldItems(this FishPond pond)
    {
        return Data.Read(pond, DataKeys.ItemsHeld)
            .ParseList<string>(';')
            .Select(s => s?.ParseTuple<string, int, int>())
            .WhereNotNull()
            .Select(t => ItemRegistry.Create(t.Item1, t.Item2, t.Item3))
            .ToList();
    }

    /// <summary>Gets a fish's chance to produce roe in this <paramref name="pond"/>.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="value">The fish's sale value.</param>
    /// <returns>The percentage chance of a fish with the given <paramref name="value"/> to produce roe in this <paramref name="pond"/>.</returns>
    internal static double GetRoeChance(this FishPond pond, int? value)
    {
        const int maxValue = 700;
        var cappedValue = Math.Min(value ?? pond.GetFishObject().Price, maxValue);

        // Mean daily roe value (/w Aquarist profession) by fish value
        // assuming regular-quality roe and fully-populated pond:
        //     30g -> ~324g (~90% roe chance per fish)
        //     700g -> ~1512g (~18% roe chance per fish) <-- capped here
        //     5000g -> ~4050g (~13.5% roe chance per fish)
        const double a = 335d / 4d;
        const double b = 275d / 2d;
        var neighbors = pond.FishCount - 1;
        return a / (cappedValue + b) * (1d + ((neighbors - 1) / 11d)) * Config.RoeProductionChanceMultiplier;
    }

    /// <summary>Parses the stored <see cref="PondFish"/> data from this <paramref name="pond"/>.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns>A <see cref="List{T}"/> of parsed <see cref="PondFish"/>.</returns>
    internal static List<PondFish> ParsePondFishes(this FishPond pond)
    {
        return Data.Read(pond, DataKeys.PondFish).Split(';').Select(PondFish.FromString).WhereNotNull().ToList();
    }

    /// <summary>Resets the <see cref="PondFish"/> data back to default values, effectively erasing stores qualities.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    internal static void ResetPondFishData(this FishPond pond)
    {
        var fish = Enumerable.Repeat(new PondFish(pond.fishType.Value, SObject.lowQuality), pond.FishCount);
        if (pond.HasBossFish() &&
            Data.ReadAs<int>(pond, "FamilyLivingHere", modId: "DaLion.Professions") is var familyLivingHere and > 0)
        {
            fish = fish
                .Take(pond.FishCount - familyLivingHere)
                .Concat(Enumerable.Repeat(
                    new PondFish(Lookups.FamilyPairs[$"(O){pond.fishType.Value}"], SObject.lowQuality),
                    familyLivingHere))
                .ToList();
        }

        Data.Write(pond, DataKeys.PondFish, string.Join(';', fish));
    }

    private static void ProduceFromPondData(this FishPond pond, List<Item> held, Random r)
    {
        var fishPondData = pond.GetFishPondData();
        if (fishPondData is null)
        {
            return;
        }

        foreach (var reward in fishPondData.ProducedItems)
        {
            var qid = reward.ItemId.StartsWith("(O)") ? reward.ItemId : "(O)" + reward.ItemId;
            if (qid is QualifiedObjectIds.Roe or QualifiedObjectIds.SquidInk ||
                pond.currentOccupants.Value < reward.RequiredPopulation ||
                !(r.NextDouble() < Utility.Lerp(0.15f, 0.95f, pond.currentOccupants.Value / 10f)) ||
                !(r.NextDouble() < reward.Chance))
            {
                continue;
            }

            held.Add(ItemRegistry.Create<SObject>(reward.ItemId, r.Next(reward.MinQuantity, reward.MaxQuantity + 1)));
            if (pond.goldenAnimalCracker.Value)
            {
                held.Last().Stack *= 2;
            }
        }
    }

    private static void ProduceRoe(this FishPond pond, SObject fish, List<Item> held, Random r)
    {
        var pondFishes = pond.ParsePondFishes();
        var fishQualities = new int[4];
        foreach (var f in pondFishes)
        {
            if (f?.Quality == SObject.bestQuality)
            {
                fishQualities[3]++;
            }
            else
            {
                fishQualities[f?.Quality ?? 0]++;
            }
        }

        var productionChancePerFish = pond.GetRoeChance(fish.Price);
        var roeQualities = new int[4];
        for (var i = 0; i < 4; i++)
        {
            while (fishQualities[i]-- > 0)
            {
                if (r.NextBool(productionChancePerFish))
                {
                    roeQualities[i]++;
                }
            }
        }

        if (fish.QualifiedItemId == QualifiedObjectIds.Sturgeon)
        {
            for (var i = 0; i < 4; i++)
            {
                roeQualities[i] += r.Next(roeQualities[i]);
            }
        }

        if (roeQualities.Sum() <= 0)
        {
            return;
        }

        var roeIndex = fish.Name.Contains("Squid") ? QualifiedObjectIds.SquidInk : QualifiedObjectIds.Roe;
        for (var i = 3; i >= 0; i--)
        {
            if (roeQualities[i] <= 0)
            {
                continue;
            }

            var producedWithThisQuality = r.Next(roeQualities[i]);
            if (producedWithThisQuality <= 0)
            {
                if (i > 0)
                {
                    roeQualities[i - 1] += roeQualities[i];
                }

                continue;
            }

            held.Add(ItemRegistry.Create<SObject>(
                roeIndex,
                producedWithThisQuality * (pond.goldenAnimalCracker.Value ? 2 : 1),
                quality: i == 3 ? 4 : i));
            if (i > 0)
            {
                roeQualities[i - 1] += roeQualities[i] - producedWithThisQuality;
            }
        }
    }

    private static Item? ProduceForAlgae(this FishPond pond, List<Item> held, Random r)
    {
        var pondAlgae = pond.ParsePondFishes();
        var algaeStacks = new[] { 0, 0, 0 }; // green, white, seaweed
        var population = pondAlgae.Count(f => f?.Id == "153");
        var chance = Utility.Lerp(0.15f, 0.95f, population / (float)pond.currentOccupants.Value);
        for (var i = 0; i < population; i++)
        {
            if (r.NextDouble() < chance)
            {
                algaeStacks[0]++;
            }
        }

        population = pondAlgae.Count(f => f?.Id == "157");
        chance = Utility.Lerp(0.15f, 0.95f, population / (float)pond.currentOccupants.Value);
        for (var i = 0; i < population; i++)
        {
            if (r.NextDouble() < chance)
            {
                algaeStacks[1]++;
            }
        }

        population = pondAlgae.Count(f => f?.Id == "152");
        chance = Utility.Lerp(0.15f, 0.95f, population / (float)pond.currentOccupants.Value);
        for (var i = 0; i < population; i++)
        {
            if (r.NextDouble() < chance)
            {
                algaeStacks[2]++;
            }
        }

        Item? result = null;
        if (algaeStacks.Sum() > 0)
        {
            SObject? greenAlgaeObj = null, whiteAlgaeObj = null, seaweedObj = null;
            if (algaeStacks[0] > 0)
            {
                greenAlgaeObj = ItemRegistry.Create<SObject>(QualifiedObjectIds.GreenAlgae, algaeStacks[0]);
                if (pond.goldenAnimalCracker.Value)
                {
                    greenAlgaeObj.Stack *= 2;
                }

                held.Add(greenAlgaeObj);
            }

            if (algaeStacks[1] > 0)
            {
                whiteAlgaeObj = ItemRegistry.Create<SObject>(QualifiedObjectIds.WhiteAlgae, algaeStacks[1]);
                if (pond.goldenAnimalCracker.Value)
                {
                    whiteAlgaeObj.Stack *= 2;
                }

                held.Add(whiteAlgaeObj);
            }

            if (algaeStacks[2] > 0)
            {
                seaweedObj = ItemRegistry.Create<SObject>(QualifiedObjectIds.Seaweed, algaeStacks[2]);
                if (pond.goldenAnimalCracker.Value)
                {
                    seaweedObj.Stack *= 2;
                }

                held.Add(seaweedObj);
            }

            result = pond.fishType.Value switch
            {
                QualifiedObjectIds.GreenAlgae when greenAlgaeObj is not null => greenAlgaeObj,
                QualifiedObjectIds.WhiteAlgae when whiteAlgaeObj is not null => whiteAlgaeObj,
                QualifiedObjectIds.Seaweed when seaweedObj is not null => seaweedObj,
                _ => null,
            };

            if (result is null)
            {
                var max = algaeStacks.ToList().IndexOfMax();
                result = max switch
                {
                    0 when greenAlgaeObj is not null => greenAlgaeObj,
                    1 when whiteAlgaeObj is not null => whiteAlgaeObj,
                    2 when seaweedObj is not null => seaweedObj,
                    _ => null,
                };
            }
        }

        if (result is not null)
        {
            held.Remove(result);
        }

        Utility.consolidateStacks(held);
        var serialized = held
            .Take(11)
            .Select(p => $"{p.ParentSheetIndex},{p.Stack},0");
        Data.Write(pond, DataKeys.ItemsHeld, string.Join(';', serialized));
        return result;
    }

    private static void ProduceForCoral(this FishPond pond, List<Item> held, double chance, Random r)
    {
        var algaeStacks = new[] { 0, 0, 0 }; // green, white, seaweed
        for (var i = 0; i < pond.FishCount; i++)
        {
            if (r.NextDouble() < chance)
            {
                switch (r.NextAlgae())
                {
                    case QualifiedObjectIds.GreenAlgae:
                        algaeStacks[0]++;
                        break;
                    case QualifiedObjectIds.WhiteAlgae:
                        algaeStacks[1]++;
                        break;
                    case QualifiedObjectIds.Seaweed:
                        algaeStacks[2]++;
                        break;
                }
            }
        }

        if (algaeStacks[0] > 0)
        {
            held.Add(ItemRegistry.Create<SObject>(QualifiedObjectIds.GreenAlgae, algaeStacks[0]));
            if (pond.goldenAnimalCracker.Value)
            {
                held.Last().Stack *= 2;
            }
        }

        if (algaeStacks[1] > 0)
        {
            held.Add(ItemRegistry.Create<SObject>(QualifiedObjectIds.WhiteAlgae, algaeStacks[1]));
            if (pond.goldenAnimalCracker.Value)
            {
                held.Last().Stack *= 2;
            }
        }

        if (algaeStacks[2] > 0)
        {
            held.Add(ItemRegistry.Create<SObject>(QualifiedObjectIds.Seaweed, algaeStacks[2]));
            if (pond.goldenAnimalCracker.Value)
            {
                held.Last().Stack *= 2;
            }
        }
    }

    private static void ProduceRadioactive(this FishPond pond, List<Item> held)
    {
        var heldMetals =
            Data.Read(pond, DataKeys.MetalsHeld)
                .ParseList<string>(';')
                .Select(li => li?.ParseTuple<string, int>())
                .WhereNotNull()
                .Where(li => !string.IsNullOrEmpty(li.Item1))
                .ToList();
        for (var i = heldMetals.Count - 1; i >= 0; i--)
        {
            var (id, timeLeft) = heldMetals[i];
            if (timeLeft > 0)
            {
                continue;
            }

            held.Add(id!.IsOreId()
                ? ItemRegistry.Create<SObject>(QualifiedObjectIds.RadioactiveOre)
                : ItemRegistry.Create<SObject>(QualifiedObjectIds.RadioactiveBar));
            heldMetals.RemoveAt(i);
        }

        Data.Write(
            pond,
            DataKeys.MetalsHeld,
            string.Join(';', heldMetals
                .Select(m => string.Join(',', m.Item1, m.Item2))));
    }

    private static void ProduceForTuiLa(this FishPond pond, List<Item> held, string which, double chance, Random r)
    {
        if (pond.FishCount > 1)
        {
            if (r.NextDouble() < chance && r.NextDouble() < chance)
            {
                held.Add(ItemRegistry.Create<SObject>(QualifiedObjectIds.GalaxySoul));
                if (pond.goldenAnimalCracker.Value)
                {
                    held.Last().Stack *= 2;
                }

                return;
            }

            held.Add(ItemRegistry.Create<SObject>(which == "MNF.MoreNewFish_tui" ? QualifiedObjectIds.VoidEssence : QualifiedObjectIds.SolarEssence));
            if (r.NextDouble() < 0.8)
            {
                held.Last().Stack++;
            }

            if (pond.goldenAnimalCracker.Value)
            {
                held.Last().Stack *= 2;
            }
        }

        held.Add(ItemRegistry.Create<SObject>(which == "MNF.MoreNewFish_tui" ? QualifiedObjectIds.SolarEssence : QualifiedObjectIds.VoidEssence));
        if (r.NextDouble() < 0.8)
        {
            held.Last().Stack++;
        }

        if (pond.goldenAnimalCracker.Value)
        {
            held.Last().Stack *= 2;
        }
    }
}
