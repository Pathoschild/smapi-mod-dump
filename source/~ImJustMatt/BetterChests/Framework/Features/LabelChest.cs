/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Features;

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewMods.Common.Helpers;
using StardewValley.Menus;

/// <summary>
///     Draw chest label to the screen.
/// </summary>
internal sealed class LabelChest : IFeature
{
#nullable disable
    private static IFeature Instance;
#nullable enable

    private readonly IModHelper _helper;

    private bool _isActivated;

    private LabelChest(IModHelper helper)
    {
        this._helper = helper;
    }

    /// <summary>
    ///     Initializes <see cref="LabelChest" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <returns>Returns an instance of the <see cref="LabelChest" /> class.</returns>
    public static IFeature Init(IModHelper helper)
    {
        return LabelChest.Instance ??= new LabelChest(helper);
    }

    /// <inheritdoc />
    public void Activate()
    {
        if (this._isActivated)
        {
            return;
        }

        this._isActivated = true;
        this._helper.Events.Display.RenderedActiveMenu += LabelChest.OnRenderedActiveMenu;
        this._helper.Events.Display.RenderedHud += LabelChest.OnRenderedHud;
    }

    /// <inheritdoc />
    public void Deactivate()
    {
        if (!this._isActivated)
        {
            return;
        }

        this._isActivated = false;
        this._helper.Events.Display.RenderedActiveMenu -= LabelChest.OnRenderedActiveMenu;
        this._helper.Events.Display.RenderedHud -= LabelChest.OnRenderedHud;
    }

    private static void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        if (Game1.activeClickableMenu is not ItemGrabMenu itemGrabMenu
         || string.IsNullOrWhiteSpace(BetterItemGrabMenu.Context?.ChestLabel))
        {
            return;
        }

        var bounds = Game1.smallFont.MeasureString(BetterItemGrabMenu.Context.ChestLabel).ToPoint();

        IClickableMenu.drawHoverText(
            e.SpriteBatch,
            BetterItemGrabMenu.Context.ChestLabel,
            Game1.smallFont,
            overrideX: itemGrabMenu.xPositionOnScreen - bounds.X - IClickableMenu.borderWidth,
            overrideY: itemGrabMenu.yPositionOnScreen
                     - IClickableMenu.borderWidth
                     - BetterItemGrabMenu.TopPadding
                     - Game1.tileSize);
    }

    private static void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (!Context.IsPlayerFree)
        {
            return;
        }

        var pos = CommonHelpers.GetCursorTile();
        if ((!Game1.currentLocation.Objects.TryGetValue(pos, out var obj)
          && !Game1.currentLocation.Objects.TryGetValue(pos - new Vector2(0, -1), out obj))
         || !Storages.TryGetOne(obj, out var storage)
         || string.IsNullOrWhiteSpace(storage.ChestLabel))
        {
            return;
        }

        IClickableMenu.drawHoverText(e.SpriteBatch, storage.ChestLabel, Game1.smallFont);
    }
}