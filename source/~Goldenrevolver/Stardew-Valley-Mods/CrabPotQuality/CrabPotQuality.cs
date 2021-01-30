/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace CrabPotQuality
{
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Objects;
    using StardewObject = StardewValley.Object;

    public class CrabPotQuality : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.DayStarted += delegate { OnDayStarted(); };
        }

        private void OnDayStarted()
        {
            foreach (var location in Game1.locations)
            {
                foreach (var item in location.Objects.Values)
                {
                    if (item is CrabPot pot)
                    {
                        if (pot != null && pot.heldObject != null && pot.heldObject.Value != null && pot.readyForHarvest)
                        {
                            // doing it in two steps in case the object gets replaced with a rainbow shell
                            int quality = DeterminePotQuality(pot);
                            pot.heldObject.Value.quality.Value = quality;
                        }
                    }
                }
            }
        }

        private int DeterminePotQuality(CrabPot pot)
        {
            // if it is magic bait, done before trash check so it's never wasted
            if (pot.bait != null && pot.bait.Value != null && pot.bait.Value.ParentSheetIndex == 908)
            {
                // give the crab pot a rainbow shell
                pot.heldObject.Value = new StardewObject(394, 1, false, -1, 0);
            }

            // item is trash
            if (pot.heldObject.Value.ParentSheetIndex >= 168 && pot.heldObject.Value.ParentSheetIndex < 173)
            {
                return 0;
            }

            Farmer farmer = Game1.getFarmer(pot.owner);

            if (farmer == null)
            {
                return 0;
            }

            if (IsLuremaster(farmer) || IsMariner(farmer))
            {
                return 4;
            }

            int multiplier = 1;

            // if it is wild bait
            if (pot.bait != null && pot.bait.Value != null && pot.bait.Value.ParentSheetIndex == 774)
            {
                multiplier = 2;
            }

            // foraging formula
            if (Game1.random.NextDouble() < farmer.FishingLevel / 30f * multiplier)
            {
                return 2;
            }
            else if (Game1.random.NextDouble() < farmer.FishingLevel / 15f * multiplier)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private bool IsLuremaster(Farmer farmer)
        {
            return farmer.professions.Contains(11);
        }

        private bool IsMariner(Farmer farmer)
        {
            return farmer.professions.Contains(10);
        }
    }
}