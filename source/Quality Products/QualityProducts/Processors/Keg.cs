using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    public class Keg : Processor
    {
        public Keg() : base(ProcessorType.KEG)
        {
        }

        private void PerformGraphicsAndSounds(Farmer who, Color color)
        {
            who.currentLocation.playSound("Ship");
            who.currentLocation.playSound("bubbles");
            Multiplayer multiplayer = QualityProducts.Instance.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, tileLocation.Value * 64f + new Vector2(0f, -128f), false, false, (tileLocation.Y + 1f) * 64f / 10000f + 0.0001f, 0f, color * 0.75f, 1f, 0f, 0f, 0f, false)
            {
                alphaFade = 0.005f
            });
        }

        protected override bool PerformProcessing(SObject @object, bool probe, Farmer who)
        {
            switch ((int)@object.parentSheetIndex)
            {
                case 262:
                    heldObject.Value = new SObject(Vector2.Zero, 346, "Beer", false, true, false, false);
                    if (!probe)
                    {
                        heldObject.Value.name = "Beer";
                        minutesUntilReady.Value = 1750;
                        PerformGraphicsAndSounds(who, Color.Yellow);
                    }
                    return true;
                case 304:
                    heldObject.Value = new SObject(Vector2.Zero, 303, "Pale Ale", false, true, false, false);
                    if (!probe)
                    {
                        heldObject.Value.name = "Pale Ale";
                        minutesUntilReady.Value = 2250;
                        PerformGraphicsAndSounds(who, Color.Yellow);
                    }
                    return true;
                case 433:
                    if (@object.Stack < 5 && !probe)
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12721"));
                        return false;
                    }
                    heldObject.Value = new SObject(Vector2.Zero, 395, "Coffee", false, true, false, false);
                    if (!probe)
                    {
                        heldObject.Value.name = "Coffee";
                        @object.Stack -= 4;
                        if (@object.Stack <= 0)
                        {
                            who.removeItemFromInventory(@object);
                        }
                        minutesUntilReady.Value = 120;
                        PerformGraphicsAndSounds(who, Color.DarkGray);
                    }
                    return true;
                case 340:
                    heldObject.Value = new SObject(Vector2.Zero, 459, "Mead", false, true, false, false);
                    if (!probe)
                    {
                        heldObject.Value.name = "Mead";
                        minutesUntilReady.Value = 600;
                        PerformGraphicsAndSounds(who, Color.Yellow);
                    }
                    return true;
            }
            switch (@object.Category)
            {
                case -75:
                    heldObject.Value = new SObject(Vector2.Zero, 350, @object.Name + " Juice", false, true, false, false)
                    {
                        Price = (int)(@object.Price * 2.25)
                    };
                    if (!probe)
                    {
                        heldObject.Value.name = @object.Name + " Juice";
                        heldObject.Value.preserve.Value = PreserveType.Juice;
                        heldObject.Value.preservedParentSheetIndex.Value = @object.parentSheetIndex;
                        minutesUntilReady.Value = 6000;
                        PerformGraphicsAndSounds(who, Color.White);
                    }
                    return true;
                case -79:
                    heldObject.Value = new SObject(Vector2.Zero, 348, @object.Name + " Wine", false, true, false, false)
                    {
                        Price = @object.Price * 3
                    };
                    if (!probe)
                    {
                        heldObject.Value.name = @object.Name + " Wine";
                        heldObject.Value.preserve.Value = PreserveType.Wine;
                        heldObject.Value.preservedParentSheetIndex.Value = @object.parentSheetIndex;
                        minutesUntilReady.Value = 10000;
                        PerformGraphicsAndSounds(who, Color.Lavender);
                    }
                    return true;
            }
            return false;
        }
    }
}
