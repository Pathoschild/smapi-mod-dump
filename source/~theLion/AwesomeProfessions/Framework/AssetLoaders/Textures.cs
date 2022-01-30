/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.AssetLoaders;

#region using directives

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Loads custom mod sprites.</summary>
internal static class Textures
{
    public const int RIBBON_WIDTH_I = 22,
        RIBBON_HORIZONTAL_OFFSET_I = -92,
        MAX_ICON_WIDTH_I = 38,
        MAX_ICON_HEIGHT_I = 18;
    public const float RIBBON_SCALE_F = 1.8f;

    #region textures

    public static Texture2D SuperModeGaugeTx { get; private set; } = ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine(
        "assets", "hud",
        ModEntry.Config.UseVintageInterface ? "gauge_vintage.png" : "gauge.png"));

    public static Texture2D SkillBarTx { get; private set; } = ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets",
        "menus", ModEntry.Config.UseVintageInterface ? "skillbars_vintage.png" : "skillbars.png"));

    public static Texture2D HoneyMeadTx { get; private set; } =
        ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "objects", "mead-" + ModEntry.Config.HoneyMeadStyle.ToLower() + ".png"));

    public static Texture2D RibbonTx { get; } =
        ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "sprites", "ribbons.png"));

    public static Texture2D MaxIconTx { get; } =
        ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "menus", "max.png"));

    #endregion textures

    internal static void ReloadGauge()
    {
        SuperModeGaugeTx = ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "hud",
            ModEntry.Config.UseVintageInterface ? "gauge_vintage.png" : "gauge.png"));
    }

    internal static void ReloadSkillBars()
    {
        SkillBarTx = ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "menus",
            ModEntry.Config.UseVintageInterface ? "skillbars_vintage.png" : "skillbars.png"));
    }

    internal static void ReloadHoneyMead()
    {
        HoneyMeadTx = ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "objects",
            "mead-" + ModEntry.Config.HoneyMeadStyle.ToLower() + ".png"));
    }

    internal static bool TryGetMeadSourceRect(int preserveIndex, out Rectangle sourceRectangle)
    {
        sourceRectangle = preserveIndex switch
        {
            376 => new(48, 0, 16, 16), // poppy
            402 => new(0, 16, 16, 16), // sweet pea
            418 => new(16, 16, 16, 16), // crocus
            421 => new(32, 0, 16, 16), // sunflower
            591 => new(0, 0, 16, 16), // tulip
            593 => new(32, 0, 16, 16), // summer spangle
            595 => new(64, 0, 16, 16), // fairy rose
            597 => new(16, 0, 16, 16), // blue jazz
            _ => new(32, 16, 16, 16)
        };

        return sourceRectangle != default;
    }
}