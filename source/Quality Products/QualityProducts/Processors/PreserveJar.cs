using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    public class PreserveJar : Processor
    {
        public PreserveJar() : base(ProcessorType.PRESERVE_JAR)
        {
        }

        private void PerformGraphicsAndSounds(Farmer who, Color color)
        {
            who.currentLocation.playSound("Ship");
            Multiplayer multiplayer = QualityProducts.Instance.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, tileLocation.Value * 64f + new Vector2(0f, -128f), false, false, (tileLocation.Y + 1f) * 64f / 10000f + 0.0001f, 0f, color * 0.75f, 1f, 0f, 0f, 0f, false)
            {
                alphaFade = 0.005f
            });
        }

        protected override bool PerformProcessing(SObject @object, bool probe, Farmer who)
        {
            switch (@object.Category)
            {
                case -75:
                    heldObject.Value = new SObject(Vector2.Zero, 342, "Pickled " + @object.Name, false, true, false, false)
                    {
                        Price = 50 + @object.Price * 2
                    };
                    if (!probe)
                    {
                        heldObject.Value.name = "Pickled " + @object.Name;
                        heldObject.Value.preserve.Value = PreserveType.Pickle;
                        heldObject.Value.preservedParentSheetIndex.Value = @object.parentSheetIndex;
                        minutesUntilReady.Value = 4000;
                        PerformGraphicsAndSounds(who, Color.White);
                    }
                    return true;
                case -79:
                    heldObject.Value = new SObject(Vector2.Zero, 344, @object.Name + " Jelly", false, true, false, false)
                    {
                        Price = 50 + @object.Price * 2
                    };
                    if (!probe)
                    {
                        minutesUntilReady.Value = 4000;
                        heldObject.Value.name = @object.Name + " Jelly";
                        heldObject.Value.preserve.Value = PreserveType.Jelly;
                        heldObject.Value.preservedParentSheetIndex.Value = @object.parentSheetIndex;
                        PerformGraphicsAndSounds(who, Color.LightBlue);
                    }
                    return true;
            }
            return false;
        }
    }
}
