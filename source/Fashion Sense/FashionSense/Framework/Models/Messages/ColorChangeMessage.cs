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
    public class ColorChangeMessage
    {
        public long FarmerID { get; }
        public string ColorKey { get; }
        public Color ColorValue { get; }

        public ColorChangeMessage(long farmerID, string colorKey, Color colorValue)
        {
            FarmerID = farmerID;
            ColorKey = colorKey;
            ColorValue = colorValue;
        }
    }
}
