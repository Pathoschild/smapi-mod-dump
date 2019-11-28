using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace BetterMeteors
{
    public class BetterMeteors : Mod
    {
        private PerformToolActionPatch ptap;
        private BetterMeteorsConfig _config;
        public override void Entry(IModHelper helper)
        {
            //Load up the Config. This will be basic for now, it can be worked on later.
            _config = helper.ReadConfig<BetterMeteorsConfig>();
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            //helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            helper.Events.Player.Warped += SaveLoaded;
            helper.Events.Input.ButtonPressed += ButtonPressed;
        }

        //Events
        public void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;
            if (e.IsDown(SButton.NumPad4) && _config.EnableDebugMode)
            {
                Farm loc = Game1.getFarm();
                try
                {
                    loc.resourceClumps.Add(new ResourceClump(622, 2, 2,
                        new Vector2(Game1.player.getTileX() + 2, Game1.player.getTileY())));
                    Monitor.Log("Meteor should spawn");
                }
                catch (Exception ex)
                {
                    Monitor.Log(ex.ToString());
                }

            }
        }

        public void SaveLoaded(object sender, /*SaveLoadedEventArgs*/WarpedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;
            Farm farm = Game1.getFarm();
            
            /*
            var loc = GetAllLocations();
            //Check the location for resource clumps.
            foreach (var l in loc)
            {
                if (l is Farm mine)
                {
                    for (int i = mine.resourceClumps.Count - 1; i >= 0; --i)
                    {
                        ResourceClump rc = mine.resourceClumps[i];
                        mine.resourceClumps.RemoveAt(i);
                        mine.resourceClumps.Add(new CustomResourceClump(rc));
                        //mine.resourceClumps[i] = new CustomResourceClump(mine.resourceClumps[0]);

                    }*/

            /*
            var mrc = mine.resourceClumps;
            foreach (var m in mrc)
            {
                mrc[m] = CustomResourceClump(m);

            }*/

        }
            }
        }

        public void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            /*
            ptap = new PerformToolActionPatch(Monitor, Helper, _config);
            //Set up Harmony then we can do whatever we want.
            var harmony = HarmonyInstance.Create(Helper.ModRegistry.ModID);

            harmony.Patch(
                original: AccessTools.Method(typeof(ResourceClump), nameof(ResourceClump.performToolAction)),
                prefix: new HarmonyMethod(typeof(PerformToolActionPatch), nameof(PerformToolActionPatch.performToolAction)));*/
        }

        /*
         * 
         * Custom Voids.
         * 
         */
        public static IEnumerable<GameLocation> GetAllLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }
        
    }
}
