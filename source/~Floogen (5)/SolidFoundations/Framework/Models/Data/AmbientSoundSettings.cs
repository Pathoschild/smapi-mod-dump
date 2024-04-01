/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;

namespace SolidFoundations.Framework.Models.Data
{
    public class AmbientSoundSettings
    {
        public Point Source { get; set; }
        public float MaxDistance { get; set; } = 1024f;
    }
}
