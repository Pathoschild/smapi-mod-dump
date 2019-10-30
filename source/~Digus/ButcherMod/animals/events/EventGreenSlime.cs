using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

namespace AnimalHusbandryMod.animals.events
{
    public class EventGreenSlime : GreenSlime
    {
        public EventGreenSlime()
        {
        }

        public EventGreenSlime(Vector2 position) : base(position)
        {
        }

        public EventGreenSlime(Vector2 position, int mineLevel) : base(position, mineLevel)
        {
        }

        public EventGreenSlime(Vector2 position, Color color) : base(position, color)
        {
        }

        public override void behaviorAtGameTick(GameTime time)
        {
        }

        public override void updateMovement(GameLocation location, GameTime time)
        {
        }
    }
}
