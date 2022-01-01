/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    internal interface IDrawingProperties
    {
        Vector2 SourceSize { get; }

        Vector2 Offset(float scaleSize);

        Vector2 Origin(float scaleSize);

        float RealScale(float scaleSize);
    }
}