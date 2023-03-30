/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotDrawInMenuPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotDrawInMenuPatcher"/> class.</summary>
    internal SlingshotDrawInMenuPatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(
            nameof(Slingshot.drawInMenu),
            new[]
            {
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool),
            });
    }

    #region harmony patches

    /// <summary>Draw slingshot cooldown.</summary>
    [HarmonyPostfix]
    private static void SlingshotDrawInMenuPostfix(
        SpriteBatch spriteBatch, Vector2 location, float scaleSize, StackDrawType drawStackNumber, bool drawShadow)
    {
        if (SlingshotsModule.State.SlingshotCooldown <= 0)
        {
            return;
        }

        var cooldownPct = SlingshotsModule.State.SlingshotCooldown / ItemIDs.SlingshotCooldown;
        var drawingAsDebris = drawShadow && drawStackNumber == StackDrawType.Hide;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (!drawShadow || drawingAsDebris || (Game1.activeClickableMenu is ShopMenu && scaleSize == 1f))
        {
            return;
        }

        var (x, y) = location;
        spriteBatch.Draw(
            Game1.staminaRect,
            new Rectangle(
                (int)x,
                (int)y + (Game1.tileSize - (cooldownPct * Game1.tileSize)),
                Game1.tileSize,
                cooldownPct * Game1.tileSize),
            Color.Red * 0.66f);
    }

    #endregion harmony patches
}
