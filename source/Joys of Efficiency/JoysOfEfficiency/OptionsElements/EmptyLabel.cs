using Microsoft.Xna.Framework.Graphics;

namespace JoysOfEfficiency.OptionsElements
{
    internal class EmptyLabel : LabelComponent
    {
        public EmptyLabel() : base("") { }
        public override void draw(SpriteBatch b, int slotX, int slotY){}
    }
}
