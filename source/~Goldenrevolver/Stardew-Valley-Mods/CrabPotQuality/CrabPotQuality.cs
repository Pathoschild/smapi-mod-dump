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
        public static CrabPotQualityConfig Config { get; set; }

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<CrabPotQualityConfig>();

            Helper.Events.GameLoop.GameLaunched += delegate { CrabPotQualityConfig.SetUpModConfigMenu(Config, this); };

            Helper.Events.GameLoop.DayStarted += delegate { OnDayStarted(); };
        }

        private static void OnDayStarted()
        {
            Utility.ForEachLocation(delegate (GameLocation location)
            {
                foreach (var item in location.Objects.Values)
                {
                    if (item is CrabPot pot)
                    {
                        if (pot != null && pot.heldObject.Value != null && pot.readyForHarvest.Value)
                        {
                            // do quality calculation and assignment in two steps in case the object gets replaced with a rainbow shell
                            int quality = DeterminePotQuality(pot);
                            pot.heldObject.Value.Quality = quality;
                        }
                    }
                }
                return true;
            });
        }

        private static int DeterminePotQuality(CrabPot pot)
        {
            // if it is magic bait, done before trash check so it's never wasted
            if (Config.EnableMagicBaitEffect && pot.bait.Value != null && pot.UsesMagicBait())
            {
                // give the crab pot a rainbow shell
                pot.heldObject.Value = ItemRegistry.Create("(O)394") as StardewObject;
            }

            // item is trash
            //if (pot.heldObject.Value.ParentSheetIndex >= 168 && pot.heldObject.Value.ParentSheetIndex < 173)
            switch (pot.heldObject.Value.QualifiedItemId)
            {
                case "(O)168":
                case "(O)169":
                case "(O)170":
                case "(O)171":
                case "(O)172":
                    return 0;
            }

            Farmer farmer = Game1.getFarmer(pot.owner.Value) ?? Game1.MasterPlayer; // set to host if owner somehow doesn't exist

            if (Config.LuremasterPerkForcesIridiumQuality && farmer.IsLuremaster())
            {
                return 4;
            }
            else if (Config.MarinerPerkForcesIridiumQuality && farmer.IsMariner())
            {
                return 4;
            }

            int multiplier = 1;

            // if it is wild bait
            if (Config.EnableWildBaitEffect && pot.bait.Value != null && pot.UsesWildBait())
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