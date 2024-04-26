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
using StardewValley;

namespace Common.UI;

public class RootElement : Container
{
    public override Vector2 LocalPosition { get; set; } = Vector2.Zero;
    public override int Width => Game1.viewport.Width;
    public override int Height => Game1.viewport.Height;
}