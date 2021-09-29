/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace DeepWoodsMod.API
{
    public interface IDeepWoodsTextures
    {
        Texture2D WoodsObelisk { get; set; }
        Texture2D HealingFountain { get; set; }
        Texture2D IridiumTree { get; set; }
        Texture2D GingerbreadHouse { get; set; }
        Texture2D BushThorns { get; set; }
        Texture2D Unicorn { get; set; }
        Texture2D ExcaliburStone { get; set; }
        Texture2D LakeTilesheet { get; set; }
        Texture2D Festivals { get; set; }
    }
}
