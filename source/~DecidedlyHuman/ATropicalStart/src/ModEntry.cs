/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using DecidedlyShared.Ui;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Minigames;

namespace ATropicalStart;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);

        // All of our Harmony patches to disable interactions while in build mode.
        harmony.Patch(
            AccessTools.Method(typeof(Intro), nameof(Intro.tick)),
            new HarmonyMethod(typeof(Patches), nameof(Patches.Intro_tick_prefix)));
    }
}
