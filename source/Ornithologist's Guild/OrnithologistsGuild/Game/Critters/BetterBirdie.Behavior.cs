/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.Collections.Generic;

namespace OrnithologistsGuild.Game.Critters
{
    public class BetterBirdieBehavior
    {
        public int Weight;
        public Func<BetterBirdieTrigger> Action;
        public bool Immediate;

        public BetterBirdieBehavior(int weight, Func<BetterBirdieTrigger> action, bool immediate)
        {
            Weight = weight;
            Action = action;
            Immediate = immediate;
        }
    }

    public partial class BetterBirdie : StardewValley.BellsAndWhistles.Critter
    {
        public List<BetterBirdieBehavior> GetContextualBehavior()
        {
            // TODO if (Type == BirdType.Default) {

            if (IsRoosting)
            {
                return new List<BetterBirdieBehavior> {
                    new BetterBirdieBehavior(1, () => BetterBirdieTrigger.Sleep, true)
                };
            }

            if (IsBathing || Environment.isWaterTile((int)TileLocation.X, (int)TileLocation.Y))
            {
                return new List<BetterBirdieBehavior> {
                    new BetterBirdieBehavior(1, () => BetterBirdieTrigger.Bathe, true)
                };
            }

            if (IsPerched)
            {
                return new List<BetterBirdieBehavior> {
                    new BetterBirdieBehavior(200, () => BetterBirdieTrigger.Peck, false),
                    new BetterBirdieBehavior(50, () =>
                    {
                        Flip();
                        return BetterBirdieTrigger.Stop;
                    }, false),
                    new BetterBirdieBehavior(25, () => BetterBirdieTrigger.Relocate, false),
                    new BetterBirdieBehavior(5, () => BetterBirdieTrigger.FlyAway, false)
                };
            }

            return new List<BetterBirdieBehavior> {
                new BetterBirdieBehavior(200, () => BetterBirdieTrigger.Walk, false),
                new BetterBirdieBehavior(200, () => BetterBirdieTrigger.Hop, false),
                new BetterBirdieBehavior(100, () => BetterBirdieTrigger.Peck, false),
                new BetterBirdieBehavior(50, () => BetterBirdieTrigger.Sleep, false),
                new BetterBirdieBehavior(25, () => BetterBirdieTrigger.Relocate, false),
                new BetterBirdieBehavior(5, () => BetterBirdieTrigger.FlyAway, false)
            };

            // TODO Relocate and RelocateToWater? Or a way to specify per birdie so water birds are more likely to be in the water
            // (I think we can use a WaterPreference that is a double for likelihood to spawn/relocate to water?)
        }
    }
}

