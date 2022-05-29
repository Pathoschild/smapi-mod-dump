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
using StardewModdingAPI;

namespace CropGrowthInformation
{
    public class TextData
    {
        public Color color;
        public string text;

        public TextData(string _text, Color _color)
        {
            text = _text;
            color = _color;
        }
    }
}