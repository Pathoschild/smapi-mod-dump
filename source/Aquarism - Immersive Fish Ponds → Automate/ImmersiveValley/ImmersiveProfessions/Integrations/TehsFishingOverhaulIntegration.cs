/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using Common.Integrations;
using Extensions;
using Framework;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#endregion using directives

internal class TehsFishingOverhaulIntegration : BaseIntegration<ISimplifiedFishingAPI>
{
    // Mail flags added by TFO to track legendary fish progress
    private static readonly Dictionary<int, string> legendaryFlags = new()
    {
        [159] = "TehPers.FishingOverhaul/crimsonfishCaught",
        [160] = "TehPers.FishingOverhaul/anglerCaught",
        [163] = "TehPers.FishingOverhaul/legendCaught",
        [682] = "TehPers.FishingOverhaul/mutantCarpCaught",
        [775] = "TehPers.FishingOverhaul/glacierfishCaught"
    };

    // Conversation topics added by [TFO] Recatchable Legendaries to track delays
    private static readonly List<string> recatchableLegendariesTopics = new()
    {
        "TehPers.RecatchableLegendaries/crimsonfishDelay",
        "TehPers.RecatchableLegendaries/anglerDelay",
        "TehPers.RecatchableLegendaries/legendDelay",
        "TehPers.RecatchableLegendaries/mutantCarpDelay",
        "TehPers.RecatchableLegendaries/glacierfishDelay"
    };

    private static readonly Func<object?, double> getTreasureBaseChance;
    private static readonly Func<object?, double> getTreasurePirateFactor;

    private readonly IModEvents _Events;
    private readonly object? _RawApi;

    /// <summary>
    ///     Lazily initializes the static getter fields. This is done lazily in case Teh's Fishing
    ///     Overhaul isn't loaded and the types do not exist. By using expressions instead of
    ///     reflection, we can avoid most of the overhead of dynamically accessing fields.
    /// </summary>
    static TehsFishingOverhaulIntegration()
    {
        // Lazily create common expressions
        var commonExpressions = new Lazy<(ParameterExpression, Expression)>(() =>
        {
            var fishingApiType = AccessTools.TypeByName("TehPers.FishingOverhaul.Services.FishingApi");

            var simplifiedApiParam = Expression.Parameter(typeof(object), "fishingApi");
            var castedApi = Expression.Convert(simplifiedApiParam, fishingApiType);
            var treasureConfigField = Expression.Field(castedApi, "treasureConfig");
            var treasureChancesProp = Expression.Property(treasureConfigField, "TreasureChances");

            return (simplifiedApiParam, treasureChancesProp);
        });

        // Lazily create expressions
        var getTreasureBaseChanceLazily = new Lazy<Func<object?, double>>(() =>
        {
            var (simplifiedApiParam, treasureChancesProp) = commonExpressions.Value;
            var baseChanceProp = Expression.Property(treasureChancesProp, "BaseChance");
            return Expression.Lambda<Func<object?, double>>(baseChanceProp, simplifiedApiParam)
                .Compile();
        });

        var getTreasurePirateFactorLazily = new Lazy<Func<object?, double>>(() =>
        {
            var (simplifiedApiParam, treasureChancesProp) = commonExpressions.Value;
            var pirateFactorProp = Expression.Property(treasureChancesProp, "PirateFactor");
            return Expression.Lambda<Func<object?, double>>(pirateFactorProp, simplifiedApiParam)
                .Compile();
        });

        // Set the static fields
        getTreasureBaseChance = fishingApi => getTreasureBaseChanceLazily.Value(fishingApi);
        getTreasurePirateFactor = fishingApi => getTreasurePirateFactorLazily.Value(fishingApi);
    }

    public TehsFishingOverhaulIntegration(
        IModRegistry modRegistry,
        IModEvents events
    ) : base("Teh's Fishing Overhaul", "TehPers.FishingOverhaul", "3.2.0",
        modRegistry)
    {
        _Events = events;
        _RawApi = modRegistry.GetApi("TehPers.FishingOverhaul");
    }

    public void Register()
    {
        AssertLoaded();

        // add Fisher perks
        ModApi.ModifyChanceForFish(
            (who, chance) => who.CurrentTool is FishingRod rod &&
                             rod.getBaitAttachmentIndex() != 703 // magnet
                             && who.HasProfession(Profession.Fisher)
                ? 1 - Math.Pow(1 - chance, 2.0)
                : chance);

        // remove Pirate perks
        ModApi.ModifyChanceForTreasure(
            (who, chance) => who.professions.Contains(9)
                ? chance - getTreasureBaseChance(_RawApi) * getTreasurePirateFactor(_RawApi)
                : chance);

        // manage legendary caught flags
        bool? hadPrestigedAngler = null;
        _Events.GameLoop.UpdateTicking += (_, _) =>
        {
            // check the state of the prestiged angler profession
            var hasPrestigedAngler = Game1.player.HasProfession(Profession.Angler, true);
            switch (hadPrestigedAngler, hasPrestigedAngler)
            {
                // prestiged status was just lost
                case (not false, false):
                    {
                        // Add flags for all legendary fish that have been caught
                        foreach (var (id, flag) in legendaryFlags)
                            if (Game1.player.fishCaught.ContainsKey(id) && !Game1.player.mailReceived.Contains(flag))
                                Game1.player.mailReceived.Add(flag);

                        break;
                    }

                // has the prestiged status
                case (_, true):
                    {
                        // remove all legendary caught flags so they can be caught again
                        // note: does not remove the fish from the collections tab
                        foreach (var flag in legendaryFlags.Values) Game1.player.RemoveMail(flag);

                        // if Recatchable Legendaries is installed, reset the conversation topics
                        if (ModRegistry.IsLoaded("TehPers.RecatchableLegendaries"))
                            foreach (var topic in recatchableLegendariesTopics)
                                Game1.player.activeDialogueEvents.Remove(topic);

                        break;
                    }
            }

            // update previous state
            hadPrestigedAngler = hasPrestigedAngler;
        };
    }
}