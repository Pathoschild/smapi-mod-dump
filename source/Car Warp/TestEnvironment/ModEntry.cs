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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace TestEnvironment;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.ConsoleCommands.Add("sophie.spouse.status", "Lists friendship status for the player with their spouse.", (_, _) =>
        {
            if (!Context.IsWorldReady || Game1.player.spouse is null or "") return;

            Monitor.Log($"Current relationship status: {Game1.player.friendshipData[Game1.player.spouse].Status}");
        });
    }
}
