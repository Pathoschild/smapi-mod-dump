/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/wrongcoder/UnscheduledEvaluation
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace UnscheduledEvaluation;

public class ModEntry : Mod
{
    private ModConfig? _config;

    public override void Entry(IModHelper helper)
    {
        _config = Helper.ReadConfig<ModConfig>();

        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.Patch(
            original: AccessTools.Method(typeof(Event), nameof(Event.exitEvent)),
            postfix: new HarmonyMethod(typeof(AddEvaluationMailPatch), nameof(AddEvaluationMailPatch.Postfix))
        );
        if (_config.EnableAlwaysActiveShrinePatch)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.checkAction)),
                prefix: new HarmonyMethod(typeof(AlwaysActiveShrinePatch), nameof(AlwaysActiveShrinePatch.Prefix)),
                postfix: new HarmonyMethod(typeof(AlwaysActiveShrinePatch), nameof(AlwaysActiveShrinePatch.Postfix))
            );
        }

        helper.Events.Content.AssetRequested += OnAssetRequested;
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("Strings/Locations")) PatchStringsLocations(e);
        if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Farm")) PatchDataEventsFarm(e);
        if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/FarmHouse")) PatchDataEventsFarmHouse(e);
    }

    private void PatchStringsLocations(AssetRequestedEventArgs e)
    {
        e.Edit(asset =>
        {
            var data = asset.AsDictionary<string, string>().Data;
            var grandpaNote = _config?.GrandpaNote;
            if (grandpaNote != null)
            {
                data["Farm_GrandpaNote"] = grandpaNote;
            }
        });
    }

    private void PatchDataEventsFarm(AssetRequestedEventArgs e)
    {
        e.Edit(asset =>
        {
            var data = asset.AsDictionary<string, string>().Data;
            // Update Grandpa candles on shrine: 2146991
            // This event is marked unseen by Event.DefaultCommands.GrandpaEvaluation2
            const string origShrineCandlesKey = "2146991/y 3/H";
            const string newShrineCandlesKey = "2146991/e 558291/H";
            if (_config?.SkippableShrineCandles ?? false) PrependEventCommand(data, origShrineCandlesKey, "skippable");
            RenameKey(data, origShrineCandlesKey, newShrineCandlesKey);
        });
    }

    private void PatchDataEventsFarmHouse(AssetRequestedEventArgs e)
    {
        e.Edit(asset =>
        {
            var data = asset.AsDictionary<string, string>().Data;
            // First Grandpa evaluation: 558291
            // Refers to two years spent on the farm
            const string origEvaluation1Key = "558291/y 3/H";
            const string newEvaluation1Key = "558291/e 321777/t 600 620/H";
            if (_config?.SkippableEvaluation1 ?? false) PrependEventCommand(data, origEvaluation1Key, "skippable");
            RenameKey(data, origEvaluation1Key, newEvaluation1Key);
            // Subsequent Grandpa evaluations: 558292
            const string origEvaluation2Key = "558292/e 321777/t 600 620/H";
            const string newEvaluation2Key = $"558292/e 321777/t 600 620/HostMail {AddEvaluationMailPatch.Evaluation1Mail}/H";
            const string newEvaluation2KeyVanilla = $"558292/e 321777/t 600 620/!HostMail {AddEvaluationMailPatch.Evaluation2Mail}/H";
            if (_config?.SkippableEvaluation2 ?? false) PrependEventCommand(data, origEvaluation2Key, "skippable");
            RenameKey(data, origEvaluation2Key, newEvaluation2Key, newEvaluation2KeyVanilla);
            // Diamond placed on shrine marks 321777 as seen
        });
    }

    private void RenameKey<TKey, TValue>(IDictionary<TKey, TValue> data, TKey oldKey, params TKey[] newKeys)
    {
        var oldKeyMissing = !data.ContainsKey(oldKey);
        var newKeyExists = newKeys.Any(data.ContainsKey);
        if (oldKeyMissing || newKeyExists)
        {
            var m = $"Skipping rename \"{oldKey}\" (missing={oldKeyMissing}) to \"{newKeys}\" (exists={newKeyExists})";
            Monitor.Log(m, LogLevel.Error);
            return;
        }

        var value = data[oldKey];
        data.Remove(oldKey);
        foreach (var newKey in newKeys)
        {
            data[newKey] = value;
        }
    }

    private void PrependEventCommand(IDictionary<string, string> data, string eventKey, string eventCommand)
    {
        if (!data.TryGetValue(eventKey, out var eventScript))
        {
            Monitor.Log($"Skipping prepend to missing key \"{eventKey}\"", LogLevel.Error);
            return;
        }

        var parts = eventScript.Split('/', 4);
        var insertIndex = $"{parts[0]}/{parts[1]}/{parts[2]}/".Length;
        var prependedEventScript = eventScript.Insert(insertIndex, $"{eventCommand}/");
        data[eventKey] = prependedEventScript;
    }
}