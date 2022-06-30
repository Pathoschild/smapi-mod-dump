/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace SplashText;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.Content.AssetRequested += LoadAssets;

        Globals.EventHelper.GameLoop.ReturnedToTitle +=
            (_, _) => Globals.EventHelper.Display.RenderedActiveMenu += SplashText.RenderSplashText;

        Globals.EventHelper.Display.MenuChanged +=
            (_, _) => Globals.EventHelper.Display.RenderedActiveMenu -= SplashText.RenderSplashText;

        Globals.EventHelper.Display.WindowResized +=
            (_, args) => SplashText.RecalculatePositionAndSize(args.NewSize);

        Globals.EventHelper.Display.RenderedActiveMenu += SplashText.RenderSplashText;
    }

    private static void LoadAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo(Globals.ContentPath))
        {
            e.LoadFromModFile<List<string>>("Assets/splashTexts.json", AssetLoadPriority.Medium);
        }
        else if (e.Name.IsEquivalentTo(Globals.SplashFontPath))
        {
            e.LoadFromModFile<SpriteFont>("Assets/splashFont.xnb", AssetLoadPriority.Medium);
        }
    }
}
