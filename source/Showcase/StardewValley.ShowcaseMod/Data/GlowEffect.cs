/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

using Igorious.StardewValley.DynamicApi2.Constants;
using Igorious.StardewValley.DynamicApi2.Utils;
using Microsoft.Xna.Framework;

namespace Igorious.StardewValley.ShowcaseMod.Data
{
    public class GlowEffect
    {
        private string _color;

        public GlowEffect() { }

        public GlowEffect(CategoryID category, int id, Color color) : this(id, color)
        {
            Category = category;
        }

        public GlowEffect(int id, Color color) : this(color)
        {
            ID = id;
        }

        public GlowEffect(Color color)
        {
            Color = ColorHelper.GetName(color);
        }

        public int? ID { get; set; }
        public CategoryID? Category {get; set; }

        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                ColorValue = ColorHelper.TryFindColor(value);
            }
        }

        public Color? ColorValue { get; private set; }
    }
}