/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotDrawAttachmentsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotDrawAttachmentsPatcher"/> class.</summary>
    internal SlingshotDrawAttachmentsPatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.drawAttachments));
    }

    #region harmony patches

    /// <summary>Patch to draw Rascal's additional ammo slot.</summary>
    [HarmonyPostfix]
    private static void SlingshotDrawAttachmentsPostfix(Slingshot __instance, SpriteBatch b, int x, int y)
    {
        if (__instance.numAttachmentSlots.Value < 2)
        {
            return;
        }

        if (__instance.attachments[1] is null)
        {
            b.Draw(
                Game1.menuTexture,
                new Vector2(x, y + 64 + 4),
                Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 43),
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.86f);
            return;
        }

        b.Draw(
            Game1.menuTexture,
            new Vector2(x, y + 64 + 4),
            Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10),
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0.86f);
        __instance.attachments[1].drawInMenu(
            b,
            new Vector2(x, y + 64 + 4),
            1f);
    }

    #endregion harmony patches
}
