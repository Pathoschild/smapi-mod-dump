using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    public class OilMaker : Processor
    {
        public OilMaker() : base(ProcessorType.OIL_MAKER)
        {
        }

        private void PerformGraphicsAndSounds(Farmer who, Color color)
        {
            who.currentLocation.playSound("bubbles");
            who.currentLocation.playSound("sipTea");
            Multiplayer multiplayer = QualityProducts.Instance.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, tileLocation.Value * 64f + new Vector2(0f, -128f), false, false, (tileLocation.Y + 1f) * 64f / 10000f + 0.0001f, 0f, color * 0.75f, 1f, 0f, 0f, 0f, false)
            {
                alphaFade = 0.005f
            });
        }

        protected override bool PerformProcessing(SObject @object, bool probe, Farmer who)
        {
            switch (@object.ParentSheetIndex)
            {
                case 270:
                    heldObject.Value = new SObject(Vector2.Zero, 247, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 1000;
                        PerformGraphicsAndSounds(who, Color.Yellow);
                    }
                    return true;
                case 421:
                    heldObject.Value = new SObject(Vector2.Zero, 247, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 60;
                        PerformGraphicsAndSounds(who, Color.Yellow);
                    }
                    return true;
                case 430:
                    heldObject.Value = new SObject(Vector2.Zero, 432, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 360;
                        PerformGraphicsAndSounds(who, Color.Yellow);
                    }
                    return true;
                case 431:
                    heldObject.Value = new SObject(247, 1, false, -1, 0);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 3200;
                        PerformGraphicsAndSounds(who, Color.Yellow);
                    }
                    return true;
            }
            return false;
        }
    }
}
