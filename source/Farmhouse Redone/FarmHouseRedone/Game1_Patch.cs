using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Buildings;
using Harmony;
using System.Reflection.Emit;

namespace FarmHouseRedone
{
    [HarmonyPriority(Priority.Last)]
    class Game1_warpFarmer_Patch
    {

        public static bool Prefix(LocationRequest locationRequest, int tileX, int tileY, int facingDirectionAfterWarp)
        {
            
            if (locationRequest.Name.Equals("CABIN_ERROR"))
            {
                Logger.Log("Player attempted to warp the a Cabin, but encountered an error!", StardewModdingAPI.LogLevel.Error);
                return false;
            }
            else if (locationRequest.Name.StartsWith("Cabin"))
            {
                Logger.Log("Player warping to Cabin.  Destination: " + locationRequest.ToString() + ", {" + tileX + ", " + tileY + "}");
            }
            return true;
        }
    }

    [HarmonyPriority(Priority.First)]
    class Game1_getLocationRequest_PrePatch
    {
        public static bool Prefix(string locationName, bool isStructure, ref LocationRequest __result)
        {
            if (locationName.ToLower().Equals("here"))
            {
                string name = Game1.currentLocation.isStructure ? Game1.currentLocation.uniqueName : Game1.currentLocation.name;
                __result = new LocationRequest(name, Game1.currentLocation.isStructure, Game1.currentLocation);
                Logger.Log("Location request: " + __result.ToString());
                return false;
            }
            if (locationName.Equals("Cabin") && !(Game1.currentLocation is Cabin))
            {
                Logger.Log("Parsing vague destination 'Cabin' as Cabin_0");
                locationName = "Cabin_0";
            }
            if (locationName.Split('_').Length > 1 && locationName.StartsWith("Cabin"))
            {
                Logger.Log("Warping to cabin...");

                List<Cabin> allCabins = getCabins(Game1.getFarm());
                if (allCabins.Count < 1)
                {
                    Logger.Log("No cabins found!");
                    __result = new LocationRequest("CABIN_ERROR", false, null);
                    return false;
                }

                string identifier = locationName.Split('_')[1].ToLower();

                Cabin resultCabin = getCabinForID(identifier, allCabins);

                if (resultCabin != null)
                {
                    Logger.Log("Warping to cabin...");
                    __result = new LocationRequest("Cabin", true, resultCabin);
                    return false;
                }
                Logger.Log("Cabin was null, using default code instead.");
            }
            return true;
        }

        public static Cabin getCabinForID(string identifier, List<Cabin> cabins)
        {
            int cabinID;
            if (int.TryParse(identifier, out cabinID))
            {
                if (cabinID >= cabins.Count)
                    cabinID = 0;
                Logger.Log("Getting cabin #" + cabinID);
                return cabins[cabinID];
            }

            if (identifier.StartsWith("@"))
            {
                Logger.Log("Searching for " + identifier.Substring(1) + "'s cabin...");
                foreach (Cabin cabin in cabins)
                {
                    if (cabin.owner != null && cabin.owner.Name.ToLower().Equals(identifier.TrimStart('@').ToLower()))
                    {
                        Logger.Log("Found " + identifier.Substring(1) + "'s cabin!");
                        return cabin;
                    }
                }
            }

            if (identifier.ToLower().Equals("home"))
            {
                Logger.Log("Searching for " + Game1.player.name + "'s cabin...");
                foreach (Cabin cabin in cabins)
                {
                    if (cabin.owner != null && cabin.owner == Game1.player)
                    {
                        Logger.Log("Found " + cabin.owner.name + "'s cabin!");
                        return cabin;
                    }
                }
            }
            Logger.Log(identifier + " did not seem to find any valid cabin, using the first cabin instead...");
            return cabins[0];
        }

        public static List<Cabin> getCabins(BuildableGameLocation location)
        {
            List<Cabin> outCabins = new List<Cabin>();
            foreach (Building b in location.buildings)
            {
                if (b.indoors != null && b.indoors.Value is Cabin)
                {
                    outCabins.Add(b.indoors.Value as Cabin);
                }
            }
            return outCabins;
        }
    }

    [HarmonyPriority(Priority.Last)]
    class Game1_getLocationRequest_Patch
    {
        public static void Postfix(string locationName, bool isStructure, ref LocationRequest __result)
        {
            if (locationName.ToLower().Equals("here"))
            {
                string name = Game1.currentLocation.isStructure ? Game1.currentLocation.uniqueName : Game1.currentLocation.name;
                __result = new LocationRequest(name, Game1.currentLocation.isStructure, Game1.currentLocation);
                Logger.Log("Location request: " + __result.ToString());
                return;
            }
            if (locationName.Equals("Cabin") && !(Game1.currentLocation is Cabin))
            {
                Logger.Log("Parsing vague destination 'Cabin' as Cabin_0");
                locationName = "Cabin_0";
            }
            if (locationName.Split('_').Length > 1 && locationName.StartsWith("Cabin"))
            {
                Logger.Log("Warping to cabin...");

                List<Cabin> allCabins = getCabins(Game1.getFarm());
                if(allCabins.Count < 1)
                {
                    Logger.Log("No cabins found!");
                    __result = new LocationRequest("CABIN_ERROR", false, null);
                    return;
                }

                string identifier = locationName.Split('_')[1].ToLower();

                Cabin resultCabin = getCabinForID(identifier, allCabins);

                if (resultCabin != null)
                {
                    Logger.Log("Warping to cabin...");
                    __result = new LocationRequest("Cabin", true, resultCabin);
                    return;
                }
                Logger.Log("Cabin was null, using default code instead.");
            }
            return;
        }

        public static Cabin getCabinForID(string identifier, List<Cabin> cabins)
        {
            int cabinID;
            if (int.TryParse(identifier, out cabinID))
            {
                if (cabinID >= cabins.Count)
                    cabinID = 0;
                Logger.Log("Getting cabin #" + cabinID);
                return cabins[cabinID];
            }

            if (identifier.StartsWith("@"))
            {
                Logger.Log("Searching for " + identifier.Substring(1) + "'s cabin...");
                foreach(Cabin cabin in cabins)
                {
                    if (cabin.owner != null && cabin.owner.Name.ToLower().Equals(identifier.TrimStart('@').ToLower()))
                    {
                        Logger.Log("Found " + identifier.Substring(1) + "'s cabin!");
                        return cabin;
                    }
                }
            }

            if (identifier.ToLower().Equals("home"))
            {
                Logger.Log("Searching for " + Game1.player.name + "'s cabin...");
                foreach (Cabin cabin in cabins)
                {
                    if (cabin.owner != null && cabin.owner == Game1.player)
                    {
                        Logger.Log("Found " + cabin.owner.name + "'s cabin!");
                        return cabin;
                    }
                }
            }
            Logger.Log(identifier + " did not seem to find any valid cabin, using the first cabin instead...");
            return cabins[0];
        }

        public static List<Cabin> getCabins(BuildableGameLocation location)
        {
            List<Cabin> outCabins = new List<Cabin>();
            foreach(Building b in location.buildings)
            {
                if(b.indoors != null && b.indoors.Value is Cabin)
                {
                    outCabins.Add(b.indoors.Value as Cabin);
                }
            }
            return outCabins;
        }
    }

    class Game1_newDayAfterFade_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            int bedSpotCall = findBedSpotCall(codes);
            if(bedSpotCall == -1)
            {
                Logger.Log("Could not find getBedSpot() in Game1_newDayAfterFade!");
                return codes.AsEnumerable();
            }
            CodeInstruction bedSpotInstruction = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmHouseStates), nameof(FarmHouseStates.getMainBedSpot)));
            Logger.Log("Setting index " + bedSpotCall + ":\n" + codes[bedSpotCall].ToString() + "\nTo:\n" + bedSpotInstruction.ToString());
            codes[bedSpotCall] = bedSpotInstruction;
            Logger.Log("Removing indices " + (bedSpotCall - 3) + " - " + (bedSpotCall - 1) + ":");
            for(int removedIndex = bedSpotCall - 3; removedIndex < bedSpotCall; removedIndex++)
            {
                Logger.Log("Index " + removedIndex + ": " + codes[removedIndex].ToString());
            }
            codes.RemoveRange(bedSpotCall - 3, 3);

            return codes.AsEnumerable();
        }

        public static int findBedSpotCall(List<CodeInstruction> codes)
        {
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt && (codes[i].operand != null && codes[i].operand.ToString().Contains("getBedSpot()")))
                {
                    return i;
                    //codes[i] = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(FarmHouseStates), nameof(FarmHouseStates.getBedSpot)));
                    ////codes[i] = new CodeInstruction(OpCodes.Callvirt, nameof(FarmHouseStates.getBedSpot));
                    //Logger.Log("Patched bed location:\n" + codes[i].ToString());
                }
            }
            return -1;
        }
    }
}
