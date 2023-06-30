/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Mini-Bars
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniBars.Framework.Rendering
{
    public class BarInformations
    {
        public string monsterName;
        public string monsterType;
        public bool editableInGame;
        public Texture2D texture;
        public Color barColor;
        public Color borderColor;
        public Color hpColor;
        public int heigth;
    }
}
