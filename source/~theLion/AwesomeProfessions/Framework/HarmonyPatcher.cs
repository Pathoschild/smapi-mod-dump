/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Patches;

namespace TheLion.Stardew.Professions.Framework;

/// <summary>Unified entry point for applying Harmony patches.</summary>
internal class HarmonyPatcher
{
    internal static uint TotalPrefixCount { get; set; }
    internal static uint TotalPostfixCount { get; set; }
    internal static uint TotalTranspilerCount { get; set; }
    internal static uint AppliedPrefixCount { get; set; }
    internal static uint AppliedPostfixCount { get; set; }
    internal static uint AppliedTranspilerCount { get; set; }
    internal static uint IgnoredPrefixCount { get; set; }
    internal static uint IgnoredPostfixCount { get; set; }
    internal static uint IgnoredTranspilerCount { get; set; }
    internal static uint FailedPrefixCount { get; set; }
    internal static uint FailedPostfixCount { get; set; }
    internal static uint FailedTranspilerCount { get; set; }

    /// <summary>Construct an instance.</summary>
    internal HarmonyPatcher(string uniqueID)
    {
        Harmony = new(uniqueID);
    }

    private Harmony Harmony { get; }

    /// <summary>Instantiate and apply one of every <see cref="IPatch" /> class in the assembly using reflection.</summary>
    internal void ApplyAll()
    {
        ModEntry.Log("[HarmonyPatcher]: Gathering patches...", LogLevel.Trace);
        var patches = AccessTools.GetTypesFromAssembly(Assembly.GetAssembly(typeof(IPatch)))
            .Where(t => t.IsAssignableTo(typeof(IPatch)) && !t.IsAbstract).ToList();
        
        ModEntry.Log($"[HarmonyPatcher]: Found {patches.Count} patch classes. Applying patches...", LogLevel.Trace);
        foreach (var patch in patches.Select(t => (IPatch) t.Constructor().Invoke(Array.Empty<object>())))
            patch.Apply(Harmony);

        var message = $"[HarmonyPatcher]: Done.\nApplied {AppliedPrefixCount}/{TotalPrefixCount} prefixes.";
        if (AppliedPrefixCount < TotalPrefixCount) message += $" {IgnoredPrefixCount} ignored. {FailedPrefixCount} failed.";

        message += $"\nApplied {AppliedPostfixCount}/{TotalPostfixCount} postfixes.";
        if (AppliedPostfixCount < TotalPostfixCount) message += $" {IgnoredPostfixCount} ignored. {FailedPostfixCount} failed.";
        
        message += $"\nApplied {AppliedTranspilerCount}/{TotalTranspilerCount} transpilers.";
        if (AppliedTranspilerCount < TotalTranspilerCount) message += $" {IgnoredTranspilerCount} ignored. {FailedTranspilerCount} failed.";

        ModEntry.Log(message, LogLevel.Trace);
    }
}