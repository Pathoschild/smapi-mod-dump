/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using Microsoft.Xna.Framework;

namespace FashionSense.Framework.Models.Messages
{
    public class ColorKeyChangeMessage
    {
        public long FarmerID { get; set; }
        public string ColorKey { get; set; }
        public Color ColorValue { get; set; }
    }
}
