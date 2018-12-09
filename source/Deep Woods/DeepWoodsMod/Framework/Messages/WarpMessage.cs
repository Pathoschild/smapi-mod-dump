using Microsoft.Xna.Framework;

namespace DeepWoodsMod.Framework.Messages
{
    internal class WarpMessage
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public Vector2 EnterLocation { get; set; }
    }
}
