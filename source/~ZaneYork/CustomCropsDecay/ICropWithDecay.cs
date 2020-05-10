using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomCropsDecay
{
    interface ICropWithDecay : ICustomObject
    {
        float decayDays { get; set; }
        SDate harvestDate { get; set; }
        bool canStackWith(ISalable other);
        void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow);
        Item getOne();
    }
}