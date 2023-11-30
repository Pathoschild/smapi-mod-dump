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
            if (IsRoosting)
            {
                return new List<BetterBirdieBehavior> {
                    new BetterBirdieBehavior(1, () => BetterBirdieTrigger.Sleep, true)
                };
            }

            if (IsInBath)
            {
                return new List<BetterBirdieBehavior> {
                    new BetterBirdieBehavior(1, () => BetterBirdieTrigger.Bathe, true)
                };
            }

            if (IsInWater)
            {
                return new List<BetterBirdieBehavior> {
                    new BetterBirdieBehavior(1000, () => BetterBirdieTrigger.Swim, false),
                    new BetterBirdieBehavior(100, () => BetterBirdieTrigger.Bathe, false),
                    new BetterBirdieBehavior(25, () => BetterBirdieTrigger.Relocate, false),
                    new BetterBirdieBehavior(5, () => BetterBirdieTrigger.FlyAway, false)
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
                new BetterBirdieBehavior(25, () => BetterBirdieTrigger.Relocate, false),
                new BetterBirdieBehavior(5, () => BetterBirdieTrigger.FlyAway, false),
                // Birds who cannot perch can sleep on the group
                new BetterBirdieBehavior(BirdieDef.PerchPreference > 0 ? 0 : 50, () => BetterBirdieTrigger.Sleep, false)
            };
        }
    }
}

