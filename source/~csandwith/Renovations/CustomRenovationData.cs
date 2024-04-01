/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.HomeRenovations;
using System;

namespace Renovations
{
    public class CustomRenovationData
    {
        public string gameLocation;
        public string mapPath;
        public Rect sourceRect = null;
        public Rect destRect = null;

        public Rectangle? SourceRect()
        {
            return sourceRect == null ? null : new Rectangle?(new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height));
        }
        public Rectangle? DestRect()
        {
            return destRect == null ? null : new Rectangle?(new Rectangle(destRect.X, destRect.Y, destRect.Width, destRect.Height));
        }
    }
}