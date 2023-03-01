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

namespace FashionSense.Framework.Models.Appearances
{
    public class AppearanceMetadata
    {
        public AppearanceModel Model { get; set; }
        public Color Color { get; set; }

        public AppearanceMetadata(AppearanceModel model, Color color)
        {
            Model = model;
            Color = color;
        }
    }
}
