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

    public static class CrabPotExtension
    {
        public static bool UsesMagnetBait(this CrabPot pot)
        {
            return pot?.bait.Value is not null && pot.bait.Value.ParentSheetIndex == 703;
        }

        public static bool UsesWildBait(this CrabPot pot)
        {
            return pot?.bait.Value is not null && pot.bait.Value.ParentSheetIndex == 774;
        }

        public static bool UsesMagicBait(this CrabPot pot)
        {
            return pot?.bait.Value is not null && pot.bait.Value.ParentSheetIndex == 908;
        }

        public static bool IsMariner(this Farmer farmer)
        {
            return farmer.professions.Contains(10);
        }

        public static bool IsLuremaster(this Farmer farmer)
        {
            return farmer.professions.Contains(11);
        }
    }

    public class CrabPotQuality : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.DayStarted += delegate { OnDayStarted(); };
        }

        private static void OnDayStarted()
        {
            foreach (var location in Game1.locations)
            {
                foreach (var item in location.Objects.Values)
                {
                    if (item is CrabPot pot)
                    {
                        if (pot != null && pot.heldObject.Value != null && pot.readyForHarvest.Value)
                        {
                            // doing it in two steps in case the object gets replaced with a rainbow shell
                            int quality = DeterminePotQuality(pot);
                            pot.heldObject.Value.Quality = quality;
                        }
                    }
                }
            }
        }

        private static int DeterminePotQuality(CrabPot pot)
        {
            // if it is magic bait, done before trash check so it's never wasted
            if (pot.bait.Value != null && pot.UsesMagicBait())
            {
                // give the crab pot a rainbow shell
                pot.heldObject.Value = new StardewObject(394, 1, false, -1, 0);
            }

            // item is trash
            if (pot.heldObject.Value.ParentSheetIndex >= 168 && pot.heldObject.Value.ParentSheetIndex < 173)
            {
                return 0;
            }

            Farmer farmer = Game1.getFarmer(pot.owner.Value) ?? Game1.MasterPlayer; // set to host if owner somehow doesn't exist

            if (farmer.IsLuremaster() || farmer.IsMariner())
            {
                return 4;
            }

            int multiplier = 1;

            // if it is wild bait
            if (pot.bait.Value != null && pot.UsesWildBait())
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
    }
}