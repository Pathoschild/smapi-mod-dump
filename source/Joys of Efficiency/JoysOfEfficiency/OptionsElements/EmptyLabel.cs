/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace JoysOfEfficiency.OptionsElements
{
    internal class EmptyLabel : LabelComponent
    {
        public EmptyLabel() : base("") { }
        public override void draw(SpriteBatch b, int slotX, int slotY){}
    }
}
