/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using DecidedlyShared.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace MappingExtensionsAndExtraProperties.Models.TileProperties;

public struct LetterType : ITilePropertyData
{
    public static string PropertyKey => "MEEP_Letter_Type";
    private bool hasCustomTexture;
    private Texture2D? texture;
    private int bgType;

    public int BgType => this.bgType;
    public bool HasCustomTexture => this.hasCustomTexture;
    public Texture2D Texture => this.texture;

    public LetterType(int type, Texture2D? texture = null)
    {
        this.texture = texture;
        this.hasCustomTexture = this.texture is not null;

        if (type < 0 || type > 3)
            this.bgType = 1;
        else
            this.bgType = type;
    }
}
