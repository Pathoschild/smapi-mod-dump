using Harmony;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ArtifactSpotOverHaul.configs;
using ArtifactSpotOverHaul;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace Mizzion.Injectors
{
    [HarmonyPatch(typeof(GameLocation), "digUpArtifactSpot")]
   public class Injectors
    {

        public static void digUpArtifactSpot(GameLocation location, int xLocation, int yLocation, SFarmer who)
        {
            
            string name = ModEntry.MapName(location);
            Random random = new Random(xLocation * 2000 + yLocation + (int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.daysPlayed);
            int objectIndex = -1;
            Random artifact = new Random();
            int oIndex = artifact.Next(0, ModEntry.instance.needArtifacts.Count);
            double rnd = random.NextDouble();
            double addedChance = ModEntry.instance.artifactLuck();
            if (ModEntry.instance.debugging)
                ModEntry.instance.Monitor.Log($"Rnd: {rnd} Added Chance: {addedChance}", LogLevel.Info);
            //Process now
            if (ModEntry.instance.config.EnableArtifactLuck && ModEntry.instance.needArtifacts.Count != 0)
            {
                foreach(KeyValuePair<int, string> objectInfo in Game1.objectInformation)
                {
                    string[] oInfo = objectInfo.Value.Split('/');
                    if (oInfo[3].Contains("Arch"))
                    {
                        string[] locationChance = oInfo[6].Split(' ');
                        int i = 0;
                        while(i < locationChance.Length)
                        {
                            if(locationChance[i].Equals(name) && rnd < (Convert.ToDouble(locationChance[i + 1], (IFormatProvider)CultureInfo.InvariantCulture) + addedChance))
                            {
                                if (ModEntry.instance.debugging)
                                    ModEntry.instance.Monitor.Log($"Spawned Needed Artifact {ModEntry.instance.needArtifacts[oIndex]}.", LogLevel.Info);
                                objectIndex = ModEntry.instance.needArtifacts[oIndex];
                                break;
                            }
                            i += 2;
                        }
                    }
                    if (objectIndex != -1)
                        break;
                }
            }
            else
            {
                foreach(KeyValuePair<int, string> objectInfo in Game1.objectInformation)
                {
                    string[] oInfo = objectInfo.Value.Split('/');
                    if (oInfo[3].Contains("Arch"))
                    {
                        string[] locationChance = oInfo[6].Split(' ');
                        int i = 0;
                        while(i < locationChance.Length)
                        {
                            if(locationChance[i].Equals(name) && rnd < Convert.ToDouble(locationChance[i + 1], (IFormatProvider)CultureInfo.InvariantCulture))
                            {
                                if (ModEntry.instance.debugging)
                                    ModEntry.instance.Monitor.Log($"Spawned Default Item: {objectInfo.Key}", LogLevel.Info);
                                objectIndex = objectInfo.Key;
                                break;
                            }
                            i += 2;
                        }
                    }
                    if (objectIndex != -1)
                        break;
                }
            }
            if(rnd < 0.2 && !(Game1.currentLocation is Farm) && objectIndex == -1 || rnd < 0.2 && objectIndex == -1)
            {
                objectIndex = 102;
                if (ModEntry.instance.debugging)
                    ModEntry.instance.Monitor.Log($"Changed objectIndex to 102(Lost Book)", LogLevel.Info);
            }
            if(objectIndex == 102 && who.archaeologyFound.ContainsKey(102) && who.archaeologyFound[102][0] >= 21)
            {
                objectIndex = 770;
                if (ModEntry.instance.debugging)
                    ModEntry.instance.Monitor.Log($"Changed objectIndex to 770(Mixed Seeds)", LogLevel.Info);
            }
            //Start Spawning Items.
            if(objectIndex != -1)
            {
                Game1.createObjectDebris(objectIndex, xLocation, yLocation, who.uniqueMultiplayerID);
                who.gainExperience(5, 25);
                if (ModEntry.instance.debugging)
                    ModEntry.instance.Monitor.Log($"objectIndex wasnt -1", LogLevel.Info);
            }
            else if(Game1.currentSeason.Equals("winter") && rnd < 0.5 && !(Game1.currentLocation is Desert))
            {
                if(rnd < 0.4)
                {
                    Game1.createObjectDebris(416, xLocation, yLocation, who.uniqueMultiplayerID);
                    if (ModEntry.instance.debugging)
                        ModEntry.instance.Monitor.Log($"Season was winter. Changed objectIndex to 416(Snow Yam).", LogLevel.Info);
                }
                else
                {
                    Game1.createObjectDebris(412, xLocation, yLocation, who.uniqueMultiplayerID);
                    if (ModEntry.instance.debugging)
                        ModEntry.instance.Monitor.Log($"Season was winter. Changed objectIndex to 412(Winter Root)", LogLevel.Info);
                }
            }
            else
            {
                Dictionary<string, string> locations = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
                string[] mapObjChance = locations[name].Split('/')[8].Split(' ');
                if (mapObjChance.Length == 0 || mapObjChance.Equals("-1"))
                {
                    if (ModEntry.instance.debugging)
                        ModEntry.instance.Monitor.Log($"mapObjChance was either Zero or 01", LogLevel.Info);
                    return;
                }
                int i = 0;
                while(i < mapObjChance.Length)
                {
                    if(rnd < Convert.ToDouble(mapObjChance[i + 1]))
                    {
                        int objI = Convert.ToInt32(mapObjChance[i]);
                        if (Game1.objectInformation.ContainsKey(objI))
                        {
                            if (Game1.objectInformation[objI].Split('/').Contains("Arch") || objI == 102)
                            {
                                if (objI == 102 && who.archaeologyFound.ContainsKey(102) && who.archaeologyFound[102][0] >= 21)
                                {
                                    objI = 770;
                                    if (ModEntry.instance.debugging)
                                        ModEntry.instance.Monitor.Log($"Lost Books exceeded 21 Changed to 770 (Mixed Seeds)", LogLevel.Info);
                                    Game1.createObjectDebris(objI, xLocation, yLocation, who.uniqueMultiplayerID);
                                    break;
                                }
                            }
                        }
                        if (ModEntry.instance.debugging)
                            ModEntry.instance.Monitor.Log($"Created Multiple Objected {objI}", LogLevel.Info);
                        Game1.createMultipleObjectDebris(objI, xLocation, yLocation, random.Next(1, 4), who.uniqueMultiplayerID);
                        break;
                    }
                    i += 2;
                }
            }
        }

        //Test to see if it overwrites the Start Up
        /*
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            return instr
                .MethodReplacer(
                AccessTools.Method(typeof(GameLocation), nameof(GameLocation.digUpArtifactSpot), new Type[] { typeof(GameLocation), typeof(int), typeof(int), typeof(SFarmer) }),
                AccessTools.Method(typeof(Injectors), nameof(Injectors.digUpArtifactSpot))
                );
        }*/
        //Prefix set up for injection
        internal static bool Prefix(GameLocation __instance, int xLocation, int yLocation, SFarmer who)
        {
            digUpArtifactSpot(__instance, xLocation, yLocation, who);
            return false;
        }
    }
}
