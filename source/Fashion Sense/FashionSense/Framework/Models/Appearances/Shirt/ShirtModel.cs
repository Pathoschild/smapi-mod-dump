/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Models.Appearances.Generic;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FashionSense.Framework.Models.Appearances.Shirt
{
    public class ShirtModel : AppearanceModel
    {
        public Position BodyPosition { get; set; } = new Position() { X = 0, Y = 0 };
        public Size ShirtSize { get; set; }
        public List<int[]> SleeveColors { get; set; }

        internal Color GetSleeveColor(int layer = 0)
        {
            Color sleeveColor = Color.White;
            if (SleeveColors.Count > layer)
            {
                sleeveColor = new Color(GetColorIndex(SleeveColors[layer], 0), GetColorIndex(SleeveColors[layer], 1), GetColorIndex(SleeveColors[layer], 2), GetColorIndex(SleeveColors[layer], 3));
            }
            else if (layer > 0)
            {
                return GetSleeveColor(layer - 1);
            }

            return sleeveColor;
        }

        internal bool HasSleeveColorAtLayer(int layer = 0)
        {
            if (SleeveColors is null)
            {
                return false;
            }

            return SleeveColors.Count > layer;
        }
    }
}
