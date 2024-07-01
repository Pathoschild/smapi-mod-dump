/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.Config;

internal class OnlineTimeConfig
{
    public bool Enable;
    public int XOffset;
    public int YOffset;
    public Color TextColor;

    public OnlineTimeConfig(bool enable, int xOffset, int yOffset, Color textColor)
    {
        this.Enable = enable;
        this.XOffset = xOffset;
        this.YOffset = yOffset;
        this.TextColor = textColor;
    }
}