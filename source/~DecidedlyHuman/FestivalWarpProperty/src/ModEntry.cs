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
using DecidedlyShared.Logging;
using DecidedlyShared.Ui;
using DecidedlyShared.UIOld;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using UiElement = DecidedlyShared.Ui.UiElement;

namespace FestivalWarpProperty;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);
        Logger logger = new Logger(this.Monitor);
        Patches.InitialiseLogger(logger);

        logger.Log(
            $"Patching {nameof(GameLocation.checkAction)} with postfix {nameof(Patches.GameLocation_CheckAction_Postfix)}.",
            LogLevel.Info);

        try
        {
            // We'll only be handling interaction warps for now, so we're only interested in GameLocation.checkAction.
            harmony.Patch(
                AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.GameLocation_CheckAction_Postfix)));
        }
        catch (Exception e)
        {
            logger.Log("Patching GameLocation.checkAction with postfix failed.");
            logger.Exception(e);
        }
    }
}
