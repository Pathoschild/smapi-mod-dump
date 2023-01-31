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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SplashText;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.Content.AssetRequested += LoadAssets;

        Globals.EventHelper.GameLoop.ReturnedToTitle += 
            (_, _) =>
            {
                SplashText.GetRandomSplashText();
                Globals.EventHelper.Display.RenderedActiveMenu += SplashText.RenderSplashText;
            };

        Globals.EventHelper.Display.WindowResized +=
            (_, args) => SplashText.RecalculatePositionAndSize(args.NewSize);

        Globals.EventHelper.Display.RenderedActiveMenu += SplashText.RenderSplashText;

        Globals.EventHelper.GameLoop.OneSecondUpdateTicked += RecalculatePositionAndSizeUponGameLaunch;
    }

    private static void RecalculatePositionAndSizeUponGameLaunch(object sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!IsTitleMenuInteractable())
            return;

        SplashText.GetRandomSplashText();
        Globals.EventHelper.GameLoop.OneSecondUpdateTicked -= RecalculatePositionAndSizeUponGameLaunch;
    }

    private static bool IsTitleMenuInteractable()
    {
        if (Game1.activeClickableMenu is not TitleMenu titleMenu || TitleMenu.subMenu != null)
            return false;

        var method = Globals.Helper.Reflection.GetMethod(titleMenu, "ShouldAllowInteraction", false);
        return method is not null ? method.Invoke<bool>() : Globals.Helper.Reflection.GetField<bool>(titleMenu, "titleInPosition").GetValue();
    }

    private static void LoadAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Globals.ContentPath))
        {
            e.LoadFromModFile<List<string>>("Assets/splashTexts.json", AssetLoadPriority.Medium);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(Globals.SplashFontPath))
        {
            e.LoadFromModFile<SpriteFont>("Assets/splashFont.xnb", AssetLoadPriority.Medium);
        }
    }
}
