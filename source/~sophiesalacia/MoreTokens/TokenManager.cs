/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MoreTokens.Tokens;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace MoreTokens;

internal class TokenManager
{
    internal static List<SimpleToken> SimpleTokens = new();

    internal static void InitializeTokens()
    {
        SimpleTokens.Add(new QuarryToken());
        SimpleTokens.Add(new HardModeToken());
    }

    internal static void RegisterTokens()
    {
        InitializeTokens();
        RegisterSimpleTokens();
        RegisterComplexTokens();
    }

    private static void RegisterSimpleTokens()
    {
        foreach (SimpleToken token in SimpleTokens)
        {
            Globals.CpApi.RegisterToken(
                Globals.Manifest,
                token.GetName(),
                token.GetValue
            );
        }
    }

    private static void RegisterComplexTokens()
    {
        throw new NotImplementedException();
    }
}
