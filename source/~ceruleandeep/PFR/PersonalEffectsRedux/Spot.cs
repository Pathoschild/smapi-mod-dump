/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

using System.Linq;

namespace PersonalEffects
{
    public class Spot
    {
        public string NPC;
        public string Location;
        public int X;
        public int Y;
        public int PercentChance;

        public static List<Spot> Spots;
        public static List<ConfigLocation> ConfigLocs;

        public static IModHelper helper;

        public static void Roll(object sender, EventArgs e)
        {
            foreach (Spot ss in Spots)
            {
                // Modworks.Log.Trace($"Checking spot {ss.Location} for {ss.NPC}");
                //despawn old items
                GameLocation l = Game1.getLocationFromName(ss.Location);
                Vector2 pos = new Vector2(ss.X, ss.Y);
                if (l.objects.ContainsKey(pos))
                {
                    //if there's one of our own items here, remove it
                    StardewValley.Object o1 = l.objects[pos];
                    if (o1 != null && o1.displayName != null)
                    {
                        if (o1.displayName.Contains("Panties") || o1.displayName.Contains("Underwear"))
                        {
                            if (Mod.Debug) Modworks.Log.Trace("Despawning item from " + ss.Location);
                            l.objects.Remove(pos);
                        }
                    }
                }
                if (ss.NPC == "Kent")
                {
                    if (Game1.year < 2) continue;
                }

                //spawn a new item, if desireable
                if (!l.objects.ContainsKey(pos))
                {
                    if (Config.GetNPC(ss.NPC).InternalName == "{Unknown NPC}")
                    {
                        // is npc configured?
                        Modworks.Log.Trace($"Attempted to spawn forage for unknown NPC {ss.NPC}");
                    }

                    else if (! Config.GetNPC(ss.NPC).Enabled)
                    {
                        //is npc enabled?
                        Modworks.Log.Trace($"Attempted to spawn forage for disabled NPC {ss.NPC}");
                    }

                    else
                    {

                        int strikepoint = (int)(Modworks.RNG.Next(100) * (1f - Modworks.Player.GetLuckFactorFloat()));
                        int chance = ss.PercentChance;

                        // Modworks.Log.Trace($"Spot {ss.Location} for {ss.NPC}: {strikepoint} <= {chance}");

                        if (strikepoint <= chance)
                        {
                            var npcd = Config.Data[ss.NPC];

                            IJsonAssetsApi api = helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
                            if (api is null)
                            {
                                Modworks.Log.Warn("Could not get JsonAssets API");
                            }
                            else
                            {
                                int variant = new Random(DateTime.Now.Millisecond).Next(2);
                                string gender = npcd.HasMaleItems() ? "m" : "f";
                                string undies;

                                if (gender == "m") {
                                    undies = variant == 1 ? "Underwear" : "Underpants";
                                } else {
                                    undies = variant == 1 ? "Panties" : "Delicates";
                                }

                                string name = $"{ss.NPC}'s {undies}";
                                int iid = api.GetObjectId(name);

                                if (iid == -1)
                                {
                                    Modworks.Log.Warn($"Could not find object {name} for {ss.NPC}");
                                } else
                                {
                                    Modworks.Log.Trace($"Spawning forage item {name} for {ss.NPC}: {iid}");

                                    StardewValley.Object i = (StardewValley.Object)StardewValley.Objects.ObjectFactory.getItemFromDescription(0, iid, 1);
                                    i.IsSpawnedObject = true;
                                    i.ParentSheetIndex = iid;
                                    l.objects.Add(pos, i);

                                }

                            }
                            
                            // Modworks.Log.Trace($"Spawning forage item for {ss.NPC}: " + sid + " at " + ss.Location + " (" + ss.X + ", " + ss.Y + ")" + "Strike: " + strikepoint + ", Chance: " + chance);
                        }
                    }
                }
            }
        }

        public int RollQuality()
        {
            int luv = Modworks.Player.GetFriendshipPoints(NPC);
            int mq = luv / 500;
            int quality = (mq > 0 && Modworks.RNG.Next(100) < 15 * mq) ? 1 : 0;
            if(mq > 1 && Modworks.RNG.Next(100) < 10 * mq){
                quality = 2;
                if(mq > 2 && Modworks.RNG.Next(100) < 6 * mq){
                    quality = 3;
                    if(mq > 3 && Modworks.RNG.Next(100) < 3) quality = 4;
                }
            }
            return quality;
        }

        public static void Setup(IModHelper im_helper)
        {
            helper = im_helper;
            helper.Events.GameLoop.DayStarted += Roll;
            Spots = new List<Spot>();

            foreach (ConfigLocation cl in ConfigLocations.Data)
            {
                bool enabled = true;

                if (cl.LocationGender == "Female" && !Config.GetNPC(cl.NPC).IsFemale) enabled = false;
                if (cl.LocationGender == "Male" && Config.GetNPC(cl.NPC).IsFemale) enabled = false;
                if (cl.LocationType == "Home" && ! Config.GetNPC(cl.NPC).HomeSpots) enabled = false;
                if (cl.LocationType == "Bath" && !Config.GetNPC(cl.NPC).BathSpots) enabled = false;
                if (cl.LocationType == "Other" && !Config.GetNPC(cl.NPC).OtherSpots) enabled = false;

                if (enabled) Spots.Add(new Spot(cl.NPC, cl.Location, cl.X, cl.Y, cl.PercentChance()));
            }
        }    

        public static void Add(Spot spot, string rarity, string loctype, string gender, string description)
        {

            ConfigLocation cl = new ConfigLocation
            {
                Description = description,
                LocationType = loctype,
                LocationGender = gender,
                NPC = spot.NPC,
                Location = spot.Location,
                X = spot.X,
                Y = spot.Y,
                Rarity = rarity
            };
            ConfigLocs.Add(cl);
        }

        public static void WriteLocs(IModHelper helper)
        {
            string directory = (helper.DirectoryPath + System.IO.Path.DirectorySeparatorChar);
            string filepath = Path.Combine(directory, "assets", "new_locations.json");

            File.WriteAllText(filepath, JsonConvert.SerializeObject(ConfigLocs, Formatting.Indented));

        }
        public static void BuildLocs(IModHelper helper)
        {
            ConfigLocs = new List<ConfigLocation>();


            int very_rare = 1; //wrong house o.O - otherspots
            int rare = 2; //wrong part of the house - otherspots
            int normal = 4; //bedroom, usually - homespots
            string npcName;

            if (Config.GetNPC("Sandy").HomeSpots)
            {
                Add(new Spot("Sandy", "SandyHouse", 2, 4, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Sandy", "SandyHouse", 18, 4, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Sandy").OtherSpots)
            {
                Add(new Spot("Sandy", "Desert", 41, 54, rare), "normal", "Other", "Any", "Bench southeast"); //bench southeast
                Add(new Spot("Sandy", "Desert", 2, 51, very_rare), "very_rare", "Other", "Any", "Bench outside oasis"); //bench outside oasis
            }
            npcName = "Sandy";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 2, 11, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                
                    Add(new Spot(npcName, "BathHouse_MensLocker", 10, 26, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                
            }
            if (Config.GetNPC("Gus").HomeSpots)
            {
                Add(new Spot("Gus", "Saloon", 23, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Gus", "Saloon", 15, 5, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Gus").OtherSpots)
            {
                Add(new Spot("Gus", "Saloon", 36, 8, rare), "normal", "Other", "Any", "Storage area"); //storage area
                Add(new Spot("Gus", "Saloon", 6, 6, very_rare), "very_rare", "Other", "Any", "Dining room"); //dining room
            }
            npcName = "Gus";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 4, 28, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 15, 57, very_rare), "very_rare", "Bath", "Any", "");
                
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
               
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 15, 7, very_rare), "very_rare", "Bath", "Male", "");
                
            }
            if (Config.GetNPC("Kent").HomeSpots)
            {
                Add(new Spot("Kent", "SamHouse", 18, 4, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Kent", "SamHouse", 20, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Kent").OtherSpots)
            {
                Add(new Spot("Kent", "SamHouse", 8, 13, rare), "normal", "Other", "Any", "Living room"); //living room
                Add(new Spot("Kent", "Town", 3, 65, very_rare), "very_rare", "Other", "Any", "Fence north of house"); //in fence north of house
            }
            npcName = "Kent";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");;
                Add(new Spot(npcName, "BathHouse_Pool", 4, 28, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 2, 11, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                 
                    Add(new Spot(npcName, "BathHouse_MensLocker", 10, 26, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 15, 7, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Lewis").HomeSpots)
            {
                Add(new Spot("Lewis", "ManorHouse", 19, 4, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Lewis", "ManorHouse", 21, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Lewis").OtherSpots)
            {
                Add(new Spot("Lewis", "ManorHouse", 7, 5, rare), "normal", "Other", "Any", "Kitchen"); //kitchen
                Add(new Spot("Lewis", "AnimalShop", 15, 4, very_rare), "very_rare", "Other", "Any", "Marnie's room"); //marnie's room
            }
            npcName = "Lewis";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 15, 57, very_rare), "very_rare", "Bath", "Any", "");
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 15, 7, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Marnie").HomeSpots)
            {
                Add(new Spot("Marnie", "AnimalShop", 14, 4, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Marnie", "AnimalShop", 16, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Marnie").OtherSpots)
            {
                Add(new Spot("Marnie", "AnimalShop", 16, 17, rare), "normal", "Other", "Any", "By counter"); //by counter
                Add(new Spot("Marnie", "Forest", 83, 16, very_rare), "very_rare", "Other", "Any", "By silo"); //by silo
            }
            npcName = "Marnie";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 2, 11, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 4, 4, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Pam").HomeSpots)
            {
                Add(new Spot("Pam", "Trailer", 16, 5, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Pam", "Trailer", 13, 8, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Pam").OtherSpots)
            {
                Add(new Spot("Pam", "BusStop", 30, 6, rare), "normal", "Other", "Any", ""); //saloon
                Add(new Spot("Pam", "Saloon", 39, 18, very_rare), "very_rare", "Other", "Any", ""); //
            }
            npcName = "Pam";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 4, 28, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 10, 26, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Pierre").HomeSpots)
            {
                Add(new Spot("Pierre", "SeedShop", 26, 7, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Pierre", "SeedShop", 19, 4, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Pierre").OtherSpots)
            {
                Add(new Spot("Pierre", "SeedShop", 7, 17, rare), "normal", "Other", "Any", "Behind counter"); //behind counter
                Add(new Spot("Pierre", "Town", 16, 69, very_rare), "very_rare", "Other", "Any", "By planter"); //by planter
            }
            npcName = "Pierre";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 4, 28, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 4, 4, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 11, 8, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 15, 7, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Elliott").HomeSpots)
            {
                Add(new Spot("Elliott", "ElliottHouse", 10, 7, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Elliott", "ElliottHouse", 6, 4, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Elliott").OtherSpots)
            {
                Add(new Spot("Elliott", "Beach", 47, 25, rare), "normal", "Other", "Any", "Beach"); //beach
                Add(new Spot("Elliott", "Town", 31, 38, very_rare), "very_rare", "Other", "Any", "Picnic table"); //picnic table
            }
            npcName = "Elliott";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 15, 57, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 11, 8, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Linus").HomeSpots)
            {
                Add(new Spot("Linus", "Mountain", 37, 5, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Linus", "Tent", 1, 3, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Linus").OtherSpots)
            {
                Add(new Spot("Linus", "Town", 67, 39, rare), "normal", "Other", "Any", "Riverside north"); //riverside north
                Add(new Spot("Linus", "Town", 47, 68, very_rare), "very_rare", "Other", "Any", "Outside saloon"); //outside saloon
            }
            npcName = "Linus";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 2, 11, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 4, 4, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 11, 8, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Evelyn").HomeSpots)
            {
                Add(new Spot("Evelyn", "JoshHouse", 6, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Evelyn", "JoshHouse", 2, 5, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Evelyn").OtherSpots)
            {
                Add(new Spot("Evelyn", "JoshHouse", 1, 18, rare), "normal", "Other", "Any", ""); //
                Add(new Spot("Evelyn", "JoshHouse", 19, 23, very_rare), "very_rare", "Other", "Any", ""); //
            }
            npcName = "Evelyn";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 15, 57, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("George").HomeSpots)
            {
                Add(new Spot("George", "JoshHouse", 1, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("George", "JoshHouse", 6, 10, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("George").OtherSpots)
            {
                Add(new Spot("George", "JoshHouse", 15, 21, rare), "normal", "Other", "Any", ""); //
                Add(new Spot("George", "JoshHouse", 12, 20, very_rare), "very_rare", "Other", "Any", ""); //
            }
            npcName = "George";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 15, 57, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 4, 4, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Wizard").HomeSpots)
            {
                Add(new Spot("Wizard", "WizardHouse", 8, 7, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Wizard", "WizardHouse", 4, 22, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Wizard").OtherSpots)
            {
                Add(new Spot("Wizard", "WitchHut", 5, 8, rare), "normal", "Other", "Any", ""); //huh
                Add(new Spot("Wizard", "Forest", 4, 22, very_rare), "very_rare", "Other", "Any", "Outside tower"); //outside tower
            }
            npcName = "Wizard";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 4, 28, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 11, 8, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Willy").HomeSpots)
            {
                Add(new Spot("Willy", "FishShop", 6, 4, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Willy", "FishShop", 9, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Willy").OtherSpots)
            {
                Add(new Spot("Willy", "Beach", 13, 38, rare), "normal", "Other", "Any", "Pier"); //pier
                Add(new Spot("Willy", "Beach", 43, 35, very_rare), "very_rare", "Other", "Any", "End of docks"); //end of docks
            }
            npcName = "Willy";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 11, 8, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Haley").HomeSpots)
            {
                Add(new Spot("Haley", "HaleyHouse", 3, 7, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Haley", "HaleyHouse", 8, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Haley").OtherSpots)
            {
                Add(new Spot("Haley", "HaleyHouse", 6, 15, rare), "normal", "Other", "Any", "Living room"); //livingroom
                Add(new Spot("Haley", "Backwoods", 27, 27, very_rare), "very_rare", "Other", "Any", "Backwoods past bus stop"); //backwoods past bus stop
                Add(new Spot("Haley", "JoshHouse", 20, 6, very_rare), "very_rare", "Other", "Any", "Alex's room"); //alex's room :o
            }
            npcName = "Haley";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 11, 8, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 10, 26, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 15, 7, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Emily").HomeSpots)
            {
                Add(new Spot("Emily", "HaleyHouse", 19, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Emily", "HaleyHouse", 13, 8, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Emily").OtherSpots)
            {
                Add(new Spot("Emily", "HaleyHouse", 5, 23, rare), "normal", "Other", "Any", "livingroom"); //livingroom
                Add(new Spot("Emily", "Beach", 4, 7, very_rare), "very_rare", "Other", "Any", "beach, far left"); //beach, far left
            }
            npcName = "Emily";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 4, 28, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 4, 4, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Penny").HomeSpots)
            {
                Add(new Spot("Penny", "Trailer", 1, 8, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Penny", "Trailer", 2, 10, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Penny").OtherSpots)
            {
                Add(new Spot("Penny", "Trailer", 17, 8, rare), "normal", "Other", "Any", "living room"); //living room
                Add(new Spot("Penny", "Town", 59, 18, very_rare), "very_rare", "Other", "Any", "by the community center, on the right"); //by the community center, on the right
            }
            npcName = "Penny";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 4, 28, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 15, 57, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 11, 8, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 10, 26, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Jodi").HomeSpots)
            {
                Add(new Spot("Jodi", "SamHouse", 19, 7, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Jodi", "SamHouse", 19, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Jodi").OtherSpots)
            {
                Add(new Spot("Jodi", "SamHouse", 8, 5, rare), "normal", "Other", "Any", "kitchen"); //kitchen
                Add(new Spot("Jodi", "Town", 44, 48, very_rare), "very_rare", "Other", "Any", "behind the saloon, hidden"); //behind the saloon, hidden
            }
            npcName = "Jodi";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 2, 11, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Leah").HomeSpots)
            {
                Add(new Spot("Leah", "LeahHouse", 3, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Leah", "LeahHouse", 9, 7, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Leah").OtherSpots)
            {
                Add(new Spot("Leah", "LeahHouse", 13, 14, rare), "normal", "Home", "Any", "kitchen"); //kitchen
                Add(new Spot("Leah", "Forest", 11, 7, very_rare), "very_rare", "Other", "Any", "by log blocking combat forest"); //by log blocking combat forest
            }
            npcName = "Leah";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 4, 28, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 15, 57, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 10, 26, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Caroline").HomeSpots)
            {
                Add(new Spot("Caroline", "SeedShop", 27, 5, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Caroline", "SeedShop", 28, 8, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Caroline").OtherSpots)
            {
                Add(new Spot("Caroline", "SeedShop", 24, 22, rare), "normal", "Other", "Any", "big empty room"); //big empty room
                Add(new Spot("Caroline", "SeedShop", 13, 28, very_rare), "very_rare", "Other", "Any", "in store"); //in store
            }
            npcName = "Caroline";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 2, 11, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 4, 4, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 15, 7, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Abigail").HomeSpots)
            {
                Add(new Spot("Abigail", "SeedShop", 5, 8, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Abigail", "SeedShop", 13, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Abigail").OtherSpots)
            {
                Add(new Spot("Abigail", "SeedShop", 40, 17, rare), "normal", "Other", "Any", "altar"); //altar
                Add(new Spot("Abigail", "SebastianRoom", 5, 5, very_rare), "very_rare", "Other", "Any", "sebastian's room"); //sebastian's room :o
            }
            npcName = "Abigail";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 15, 57, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 10, 26, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Maru").HomeSpots)
            {
                Add(new Spot("Maru", "ScienceHouse", 9, 7, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Maru", "ScienceHouse", 5, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Maru").OtherSpots)
            {
                Add(new Spot("Maru", "ScienceHouse", 30, 12, rare), "normal", "Other", "Any", "kitchen"); //kitchen
                Add(new Spot("Maru", "Hospital", 20, 11, very_rare), "very_rare", "Other", "Any", "at clinic"); //at clinic
            }
            npcName = "Maru";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 4, 28, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 11, 8, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 15, 7, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Robin").HomeSpots)
            {
                Add(new Spot("Robin", "ScienceHouse", 15, 4, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Robin", "ScienceHouse", 19, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Robin").OtherSpots)
            {
                Add(new Spot("Robin", "ScienceHouse", 22, 19, rare), "normal", "Other", "Any", "lab"); //lab
                Add(new Spot("Robin", "Backwoods", 42, 10, very_rare), "very_rare", "Other", "Any", "in woods, path behind her house"); //in woods, path behind her house
            }
            npcName = "Robin";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 2, 11, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 11, 8, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 10, 26, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 15, 7, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Alex").HomeSpots)
            {
                Add(new Spot("Alex", "JoshHouse", 13, 7, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Alex", "JoshHouse", 17, 8, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Alex").OtherSpots)
            {
                Add(new Spot("Alex", "Town", 73, 101, rare), "normal", "Other", "Any", "town, by lower-right bridge"); //town, by lower-right bridge
                Add(new Spot("Alex", "HaleyHouse", 4, 8, very_rare), "very_rare", "Other", "Any", "haley's room"); //haley's room
            }
            npcName = "Alex";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 2, 11, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 15, 57, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 11, 8, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 15, 7, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Harvey").HomeSpots)
            {
                Add(new Spot("Harvey", "HarveyRoom", 16, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Harvey", "HarveyRoom", 14, 5, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Harvey").OtherSpots)
            {
                Add(new Spot("Harvey", "Hospital", 20, 11, rare), "normal", "Other", "Any", "at clinic"); //at clinic
                Add(new Spot("Harvey", "Hospital", 4, 14, very_rare), "very_rare", "Other", "Any", "clinic, front counter"); //clinic, front counter
            }
            npcName = "Harvey";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 10, 26, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Sam").HomeSpots)
            {
                Add(new Spot("Sam", "SamHouse", 19, 15, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Sam", "SamHouse", 13, 12, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Sam").OtherSpots)
            {
                Add(new Spot("Sam", "Beach", 17, 13, rare), "normal", "Other", "Any", ""); //
                Add(new Spot("Sam", "ScienceHouse", 3, 6, very_rare), "very_rare", "Other", "Any", "maru's room"); //maru's room :o
            }
            npcName = "Sam";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 2, 11, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 5, 51, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 4, 4, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 10, 26, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Sebastian").HomeSpots)
            {
                Add(new Spot("Sebastian", "SebastianRoom", 10, 7, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Sebastian", "SebastianRoom", 8, 10, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Sebastian").OtherSpots)
            {
                Add(new Spot("Sebastian", "Tunnel", 23, 7, rare), "normal", "Other", "Any", ""); //yep. teh tunnel.
                Add(new Spot("Sebastian", "SeedShop", 3, 8, very_rare), "very_rare", "Other", "Any", "abigail's bedroom"); //abigail's bedroom. :o
            }
            npcName = "Sebastian";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 6, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 4, 28, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 2, 11, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 15, 57, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 4, 4, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 15, 7, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Shane").HomeSpots)
            {
                Add(new Spot("Shane", "AnimalShop", 28, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Shane", "AnimalShop", 22, 7, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Shane").OtherSpots)
            {
                Add(new Spot("Shane", "Town", 101, 24, very_rare), "very_rare", "Other", "Any", "above jojamart"); //above jojamart
                Add(new Spot("Shane", "AnimalShop", 2, 17, rare), "normal", "Other", "Any", "by the fireplace"); //by the fireplace
            }
            npcName = "Shane";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 4, 28, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 11, 8, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 24, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 15, 7, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Clint").HomeSpots)
            {
                Add(new Spot("Clint", "Blacksmith", 9, 6, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Clint", "Blacksmith", 3, 5, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Clint").OtherSpots)
            {
                Add(new Spot("Clint", "Blacksmith", 9, 13, rare), "normal", "Other", "Any", "by forge"); //by forge
                Add(new Spot("Clint", "ArchaeologyHouse", 42, 4, very_rare), "very_rare", "Other", "Any", ""); //wtf
            }
            npcName = "Clint";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 4, 28, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 2, 11, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 1, 25, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 15, 16, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 10, 26, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 15, 7, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
            if (Config.GetNPC("Demetrius").HomeSpots)
            {
                Add(new Spot("Demetrius", "ScienceHouse", 16, 7, normal), "normal", "Home", "Any", "Bedroom"); //br
                Add(new Spot("Demetrius", "ScienceHouse", 14, 5, normal), "normal", "Home", "Any", "Bedroom"); //br
            }
            if (Config.GetNPC("Demetrius").OtherSpots)
            {
                Add(new Spot("Demetrius", "ScienceHouse", 19, 22, rare), "normal", "Other", "Any", "lab"); //lab
                Add(new Spot("Demetrius", "Town", 31, 94, very_rare), "very_rare", "Other", "Any", "hidden, by sewers"); //hidden, by sewers
            }
            npcName = "Demetrius";
            if (Config.GetNPC(npcName).BathSpots)
            {
                Add(new Spot(npcName, "BathHouse_Entry", 1, 7, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 25, 18, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 23, 31, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "BathHouse_Pool", 10, 5, very_rare), "very_rare", "Bath", "Any", "");
                Add(new Spot(npcName, "Railroad", 15, 57, very_rare), "very_rare", "Bath", "Any", "");
                
                {
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 11, 8, very_rare), "very_rare", "Bath", "Female", "");
                    Add(new Spot(npcName, "BathHouse_WomensLocker", 9, 24, very_rare), "very_rare", "Bath", "Female", "");
                }
                
                {
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 6, very_rare), "very_rare", "Bath", "Male", "");
                    Add(new Spot(npcName, "BathHouse_MensLocker", 5, 14, very_rare), "very_rare", "Bath", "Male", "");
                }
            }
        }

        public Spot(string npc, string loc, int x, int y, int chance)
        {
            this.NPC = npc;
            Location = loc;
            X = x;
            Y = y;
            PercentChance = chance;
        }
    }
}
