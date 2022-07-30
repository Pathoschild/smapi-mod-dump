/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YTSC/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using HarmonyLib;
using System.Reflection;
using QualityFishPonds.Patch;
using System;

namespace QualityFishPonds
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance;
        public static string fishPondIdKey;
        internal Config config;
        private Harmony harmony;
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            harmony = new(Helper.ModRegistry.ModID);
            fishPondIdKey = $"{Helper.ModRegistry.ModID}(FishPondID)";
            FishPondPatchs.Initialize(this.Monitor);
            FishingRodPatchs.Initialize(this.Monitor);
            config = helper.ReadConfig<Config>();
            Helper.Events.GameLoop.DayStarted += OnDayStarted;          
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Farm farm = Game1.getFarm();
            foreach(Building building in farm.buildings)
            {           
                if(building is FishPond || building.GetType().IsSubclassOf(typeof(FishPond)))
                {
                    FishPond pond = (FishPond)building;                     
                    if (!pond.modData.ContainsKey(fishPondIdKey))
                    {
                        string fishQualities = "";           
                        if (pond.FishCount > 0)
                        {
                            fishQualities = "0";
                            for (int x = 1; x < pond.FishCount; x++)
                                fishQualities += "0";                            
                        }                   
                        pond.modData.Add(fishPondIdKey, fishQualities);                      
                    }                  
                }
            }
        }
    }
}
