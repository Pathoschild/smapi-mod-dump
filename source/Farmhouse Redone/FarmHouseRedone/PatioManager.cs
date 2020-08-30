using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using xTile;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone
{
    public static class PatioManager
    {
        public static Patio patio = null;

        public static void applyPatio(Farm farm, string spouse)
        {
            Map patioMap = getPatioMap(spouse, true);
            if(patioMap == null)
            {
                patioMap = getPatioMap("Default");
                if (patioMap == null)
                    return;
            }
            Vector2 standingSpot = new Vector2(2, 4);
            if (patioMap.Properties.ContainsKey("Standing"))
            {
                standingSpot = FarmState.getCoordsFromString(patioMap.Properties["Standing"]);
            }
            patio = new Patio(standingSpot, patioMap, spouse);
            MapUtilities.MapMerger.pasteMap(farm, patio.patioMap, (int)FarmState.spouseOutdoorLocation.X, (int)FarmState.spouseOutdoorLocation.Y);
            //patio.pasteMap(farm, (int)FarmState.spouseOutdoorLocation.X, (int)FarmState.spouseOutdoorLocation.Y);
        }

        public static Map getPatioMap(string spouseName, bool ignoreProblems = false)
        {
            try
            {
                return FarmHouseStates.loader.Load<Map>("Maps/Patio_" + spouseName, StardewModdingAPI.ContentSource.GameContent);
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException)
            {
                if (!ignoreProblems)
                    Logger.Log("No map found at maps/Patio_" + spouseName + " within the game content, using default if provided...");
            }
            try
            {
                return FarmHouseStates.loader.Load<Map>("assets/maps/Patio_" + spouseName + ".tbin", StardewModdingAPI.ContentSource.ModFolder);
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException)
            {
                if (!ignoreProblems)
                    Logger.Log("No map found at assets/maps/Patio_" + spouseName + ".tbin within either the game content or the mod content!", StardewModdingAPI.LogLevel.Error);
            }
            return null;
        }
    }
}
