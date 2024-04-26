/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using stardew_access.Patches;
using stardew_access.Utils;
using StardewModdingAPI.Events;
using StardewValley;

namespace stardew_access.Features;

public class FeatureManager
{
    private static readonly List<FeatureBase> AllFeatures =
    [
        KonamiCode.Instance,
        PlayerTriggered.Instance,
        ReadTile.Instance,
        TileViewer.Instance,
        GridMovement.Instance,
        ObjectTracker.Instance,
        GameStateNarrator.Instance,
        Warnings.Instance,
        Radar.Instance,
    ];

    public static void UpdateEvent(object? sender, UpdateTickedEventArgs e)
    {
        foreach (FeatureBase feature in AllFeatures)
        {
            try
            {
                feature.Update(sender, e);
            }
            catch (Exception exception)
            {
                Log.Error(
                    $"An error occurred while updating {feature.GetType().FullName} feature:\n{exception.Message}\n{exception.StackTrace}");
                throw;
            }
        }
    }

    public static void OnButtonPressedEvent(object? sender, ButtonPressedEventArgs e)
    {
        #region Simulate left and right clicks

        if (!TextBoxPatch.IsAnyTextBoxActive)
        {
            if (Game1.activeClickableMenu != null)
            {
                MouseUtils.SimulateMouseClicks(
                    (x, y) => Game1.activeClickableMenu.receiveLeftClick(x, y),
                    (x, y) => Game1.activeClickableMenu.receiveRightClick(x, y)
                );
            }
            else if (Game1.currentMinigame != null)
            {
                MouseUtils.SimulateMouseClicks(
                    (x, y) => Game1.currentMinigame.receiveLeftClick(x, y),
                    (x, y) => Game1.currentMinigame.receiveRightClick(x, y)
                );
            }
        }

        #endregion

        foreach (FeatureBase feature in AllFeatures)
        {
            try
            {
                if (feature.OnButtonPressed(sender, e)) break;
            }
            catch (Exception exception)
            {
                Log.Error(
                    $"An error occurred in OnButtonPressed of {feature.GetType().FullName} feature:\n{exception.Message}\n{exception.StackTrace}");
                throw;
            }
        }
    }

    public static void OnButtonsChangedEvent(object? sender, ButtonsChangedEventArgs e)
    {
        foreach (FeatureBase feature in AllFeatures)
        {
            try
            {
                feature.OnButtonsChanged(sender, e);
            }
            catch (Exception exception)
            {
                Log.Error(
                    $"An error occurred in OnButtonChangedEvent of {feature.GetType().FullName} feature:\n{exception.Message}\n{exception.StackTrace}");
                throw;
            }
        }
    }

    public static void OnPlayerWarpedEvent(object? sender, WarpedEventArgs e)
    {
        foreach (FeatureBase feature in AllFeatures)
        {
            try
            {
                feature.OnPlayerWarped(sender, e);
            }
            catch (Exception exception)
            {
                Log.Error(
                    $"An error occurred in OnButtonChangedEvent of {feature.GetType().FullName} feature:\n{exception.Message}\n{exception.StackTrace}");
                throw;
            }
        }
    }
}