/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Events.Display.RenderedWorld;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using Buff = DaLion.Shared.Enums.Buff;

#endregion using directives

[UsedImplicitly]
internal sealed class YobaRenderedWorldEvent : RenderedWorldEvent
{
    /// <summary>Initializes a new instance of the <see cref="YobaRenderedWorldEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal YobaRenderedWorldEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => CombatModule.State.YobaShieldHealth > 0 && Game1.buffsDisplay.hasBuff((int)Buff.YobasBlessing);

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        var player = Game1.player;
        var position = player.Position -
                       new Vector2(
                           Game1.viewport.X + (int)((Game1.tileSize - (player.Sprite.SpriteWidth * 4 / 3)) * 1.5f),
                           Game1.viewport.Y + (int)((Game1.tileSize + (player.Sprite.SpriteHeight / 2)) * 1.5f));
        e.SpriteBatch.Draw(
            Textures.ShieldTx,
            position,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            1.5f,
            SpriteEffects.None,
            1000f);
    }
}
