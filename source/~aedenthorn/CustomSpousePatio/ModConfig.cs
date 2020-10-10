/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace CustomSpousePatio
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public Point DefaultSpouseAreaLocation { get; set; } = new Point(71, 10);
    }
}
