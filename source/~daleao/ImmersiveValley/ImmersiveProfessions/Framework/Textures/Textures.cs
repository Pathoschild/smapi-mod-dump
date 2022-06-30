/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Textures;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
public static class Textures
{
    internal const int RIBBON_WIDTH_I = 22, STARS_WIDTH_I = 20, SINGLE_STAR_WIDTH_I = 8,
        PROGRESSION_HORIZONTAL_OFFSET_I = -82, PROGRESSION_VERTICAL_OFFSET_I = -70;
    internal const float RIBBON_SCALE_F = 1.8f, STARS_SCALE_F = 3f;

    #region textures

    public static Texture2D PointerTx { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/HudPointer");

    public static Texture2D MaxIconTx { get; set; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/MaxFishSizeIcon");

    public static Texture2D ProgressionTx { get; set; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/PrestigeProgression");

    public static Texture2D BarsTx { get; set; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/SkillBars");

    public static Texture2D SpriteTx =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/SpriteSheet");

    public static Texture2D MeterTx { get; set; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/UltimateMeter");

    #endregion textures
}