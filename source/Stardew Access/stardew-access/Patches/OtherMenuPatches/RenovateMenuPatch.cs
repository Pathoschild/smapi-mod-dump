/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class RenovateMenuPatch : IPatch
{
    private static bool firstTimeInNamingMenu = true;

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(RenovateMenu), "draw"),
            prefix: new HarmonyMethod(typeof(RenovateMenuPatch), nameof(RenovateMenuPatch.DrawPatch))
        );
    }

    private static void DrawPatch(RenovateMenu __instance, HouseRenovation ____renovation, bool ___freeze)
    {
        try
        {
            if (Game1.globalFade || ___freeze) return;
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

            if (firstTimeInNamingMenu)
            {
                firstTimeInNamingMenu = false;
                Game1.options.setGamepadMode("auto");
                Log.Debug("[RenovateMenuPatch] Gamepad mode set to 'auto'");
            }

            if (MainClass.Config.PrimaryInfoKey.JustPressed())
            {
                MoveMouseCursorToAreaAndSpeak(____renovation);
                return;
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in renovate menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }

    private static void MoveMouseCursorToAreaAndSpeak(HouseRenovation ____renovation)
    {
        // Move the mouse cursor to the renovation area
        float areaX = Utility.ModifyCoordinateForUIScale(____renovation.renovationBounds[0][0].X) + 0.5f;
        float areaY = Utility.ModifyCoordinateForUIScale(____renovation.renovationBounds[0][0].Y) + 0.5f;
        Vector2 pan = GetPanAmountForTile(new Vector2(areaX, areaY), 4);
        Game1.panScreen((int)pan.X, (int)pan.Y);
        Game1.setMousePosition(x: (int)(areaX * Game1.tileSize) - Game1.viewport.X, y: (int)(areaY * Game1.tileSize) - Game1.viewport.Y, ui_scale: false);

        List<string> areasInfo = new();
        for (int i = 0; i < ____renovation.renovationBounds[0].Count; i++)
        {
            Rectangle area = ____renovation.renovationBounds[0][i];
            areasInfo.Add(Translator.Instance.Translate("menu-renovate-area_dimension_info", tokens: new
            {
                index = i + 1,
                tile_x = area.X,
                tile_y = area.Y,
                width = area.Width,
                height = area.Height,
            }, translationCategory: TranslationCategory.Menu));
        }

        string toSpeak = Translator.Instance.Translate("menu-renovate-info", tokens: new
        {
            areas_info = string.Join("\n", areasInfo)
        }, translationCategory: TranslationCategory.Menu);
        MainClass.ScreenReader.Say(toSpeak, true);
    }

    private static Vector2 GetPanAmountForTile(Vector2 tile, int offset = 0)
    {
        Vector2 pan = Vector2.Zero;

        if (((tile.X - offset) * Game1.tileSize) < Game1.viewport.X)
            pan.X = ((tile.X - offset) * Game1.tileSize) - Game1.viewport.X;
        else if (((tile.X + offset) * Game1.tileSize) > (Game1.viewport.X + Game1.viewport.Width))
            pan.X = ((tile.X + offset) * Game1.tileSize) - Game1.viewport.X - Game1.viewport.Width;

        if (((tile.Y - offset) * Game1.tileSize) < Game1.viewport.Y)
            pan.Y = ((tile.Y - offset) * Game1.tileSize) - Game1.viewport.Y;
        else if (((tile.Y + offset) * Game1.tileSize) > (Game1.viewport.Y + Game1.viewport.Height))
            pan.Y = ((tile.Y + offset) * Game1.tileSize) - Game1.viewport.Y - Game1.viewport.Height;

        return pan;
    }

    internal static void Cleanup()
    {
        firstTimeInNamingMenu = true;
        Game1.options.setGamepadMode("force_on");
        Log.Debug("[RenovateMenuPatch] Gamepad mode set to 'force_on'");
    }
}
