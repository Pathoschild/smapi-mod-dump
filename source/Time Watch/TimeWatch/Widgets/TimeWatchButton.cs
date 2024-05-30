/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KyuubiRan/TimeWatch
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using TimeWatch.Data;
using TimeWatch.Utils;

namespace TimeWatch.Widgets;

public class TimeWatchButton : IDisposable
{
    private bool _rendered;

    private static IModHelper Helper => ModHelpers.Helper;

    private readonly PerScreen<ClickableTextureComponent> _timeWatch = new()
    {
        Value = new ClickableTextureComponent(
            new Rectangle(0, 0, 64, 64),
            Helper.ModContent.Load<Texture2D>("assets/image/time_watch.png"),
            new Rectangle(0, 0, 128, 128), 0.5f
        )
    };

    // private readonly bool _biggerBackpackLoaded;

    public TimeWatchButton()
    {
        Helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
        // Helper.Events.Input.ButtonPressed += OnButtonPressed;
        Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        // _biggerBackpackLoaded = Helper.ModRegistry.IsLoaded("spacechase0.BiggerBackpack");
    }

    private void OnRenderedActiveMenu(object? sender, EventArgs e)
    {
        if (!_rendered)
            return;

        Draw();
    }

    // private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    // {
    //     if (!_rendered)
    //         return;
    // }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        _rendered = Game1.activeClickableMenu is GameMenu { currentTab: 0 };
    }

    private void Draw()
    {
        if (!_rendered)
            return;

        var component = _timeWatch.Value;
        component.bounds.X = Game1.activeClickableMenu.xPositionOnScreen - 60;
        component.bounds.Y = Game1.activeClickableMenu.yPositionOnScreen + 100;

        _timeWatch.Value = component;
        _timeWatch.Value.draw(Game1.spriteBatch);

        Game1.activeClickableMenu.drawMouse(Game1.spriteBatch);

        if (_timeWatch.Value.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
        {
            var tw = TimeWatchManager.CurrentPlayerTimeWatch;
            var maxStr = MagicTimeWatch.MaxStorableTime > 0
                ? MagicTimeWatch.MaxStorableTimeSpan.ToString()
                : I18n.Item_TimeWatch_StoreTime_Unlimited();

            var remainingStr = MagicTimeWatch.DailyMaximumStorableTime > 0
                ? (MagicTimeWatch.DailyMaximumStorableTimeSpan - MagicTimeWatch.TodayWorldSeekedTime).ToString()
                : I18n.Item_TimeWatch_StoreTime_Unlimited();
            
            var s = I18n.Item_TimeWatchTooltip().Format(tw.StoredTimeSpan, maxStr, remainingStr);
            IClickableMenu.drawHoverText(Game1.spriteBatch, s, Game1.dialogueFont);
        }
    }

    public void Dispose()
    {
        _rendered = false;
        Helper.Events.Display.RenderedActiveMenu -= OnRenderedActiveMenu;
        // Helper.Events.Input.ButtonPressed -= OnButtonPressed;
        Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;

        GC.SuppressFinalize(this);
    }
}