/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elfuun1/FlowerDanceFix
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Harmony;

namespace FlowerDanceFix
{
    public class EventPatched
    {
        public static IMonitor Monitor;
        public static IModHelper Helper;
        public static ModConfig Config;

        public static void Initialize(IMonitor monitor, ModConfig config, IModHelper helper)
        {
            Monitor = monitor;
            Config = config;
            Helper = helper;
        }

        public static void setUpFestivalMainEvent_FDF(StardewValley.Event __instance)
        {
            {
                if (Monitor is null || __instance is null || !__instance.isSpecificFestival("spring24"))
                {
                    return;
                }
                try
                {
                    /*
                    //Reflection to access protected Game1.Multiplayer
                    Multiplayer multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                    */
                    //Sets up random number generation for later use
                    Random rnd = new Random();
                    Random rnd2 = new Random();

                    //Sets up lists for pair generation
                    List<NetDancePartner> females = new List<NetDancePartner>();
                    List<NetDancePartner> males = new List<NetDancePartner>();

                    List<string> leftoverFemales = new List<string>();
                    List<string> leftoverMales = new List<string>();

                    List<NPC> charList = new List<NPC>();
                    Utility.getAllCharacters(charList);


                    List<string> nonBinary = new List<string>();
                    List<string> usesFDFSprites = new List<string>();

                    //Populates "leftoverGender" lists with all datable NPCs of each respective gender for selection, configurable switch for nonbinary characters
                    foreach (NPC character in charList)
                    {

                        if (character.datable.Equals(true))
                        {
                            int intgender = character.Gender;

                            Monitor.Log(character.name + "'s gender is evaluated as " + intgender, LogLevel.Trace);

                            switch (intgender)
                            {
                                case 0:
                                    leftoverMales.Add(character.Name);
                                    Monitor.Log("Successfully added " + character.Name + " to leftoverMales dancer pool.", LogLevel.Trace);
                                    break;

                                case 1:
                                    leftoverFemales.Add(character.Name);
                                    Monitor.Log("Successfully added " + character.Name + " to leftoverFemales dancer pool.", LogLevel.Trace);
                                    break;

                                case 2:
                                    if (Config.AllowNonBinaryPartners.Equals(true))
                                    {
                                        try
                                        {
                                            //check to see if nonbinary dance partner has custom FDF sprites
                                            //load custom FDF sprites if available

                                            //add nonbinary partner to random leftoverGender list

                                            // int g = rnd.Next(1);
                                            // if (g == 0)
                                            // {
                                            //     leftoverMales.Add(character.Name);
                                            //     nonBinary.Add(character.Name);
                                            //     usesFDFSprites.Add(character.Name); 
                                            //     Monitor.Log("Successfully added nonbinary NPC " + character.Name + " randomly to leftoverMales dancer pool.", LogLevel.Trace);
                                            // }
                                            // else
                                            // {
                                            //     leftoverFemales.Add(character.Name);
                                            //     nonBinary.Add(character.Name);
                                            //     usesFDFSprites.Add(character.Name); 
                                            //     Monitor.Log("Successfully added nonbinary NPC " + character.Name + " randomly to leftoverFemales dancer pool.", LogLevel.Trace);
                                            // }

                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            Monitor.Log("Failed to find custom FDF sprites for " + character.name + " and cannot add that NPC to dancer pools.", LogLevel.Debug);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        Monitor.Log("Failed to add nonbinary NPC " + character.Name + " to a leftoverGender dancer pool due to config- AllowNonBinaryPartners = false.", LogLevel.Debug);
                                        break;
                                    }
                            }
                            continue;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    Monitor.Log("Finished adding the following NPCs to the leftoverFemales selection pool: " + string.Join(", ", leftoverFemales), LogLevel.Trace);
                    Monitor.Log("Finished adding the following NPCs to the leftoverMales selection pool: " + string.Join(", ", leftoverMales), LogLevel.Trace);

                    List<string> Tourists = new List<string>();

                    //Prevents selection of tourist datable characters based on config
                    if (Config.AllowTouristPartners.Equals(false))
                    {
                        foreach (NPC character in charList)
                        {
                            if (character.datable.Equals(true) && !character.homeRegion.Equals(2))
                            {
                                Tourists.Add(character.name);

                                int intgender = character.gender;
                                switch (intgender)
                                {
                                    case 0:
                                        leftoverMales.Remove(character.Name);
                                        Monitor.Log("Successfully removed" + character.Name + " from leftoverMales dancer pool. Configurable by AllowTouristPartners.", LogLevel.Trace);
                                        break;
                                    case 1:
                                        leftoverFemales.Remove(character.Name);
                                        Monitor.Log("Successfully removed " + character.Name + " from leftoverFemales dancer pool. Configurable by AllowTouristPartners.", LogLevel.Trace);
                                        break;
                                    case 2:
                                        if (Config.AllowNonBinaryPartners.Equals(true))
                                        {

                                        }
                                        else
                                        {
                                            Monitor.Log("This is a big error- please contact elfuun on discord (elfuun#8783) or on the FDF mod page to report! (remove nb using tourist config when nb config disabled)", LogLevel.Alert);
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    //Removes blacklisted datables from "leftoverGender" lists based on config
                    if (!String.IsNullOrEmpty(Config.DancerBlackList))
                    {
                        try
                        {
                            List<string> blackList = Config.DancerBlackList.Split('/').ToList();

                            IEnumerable<string> toRemoveMale = blackList.Intersect(leftoverMales);
                            foreach (string i in toRemoveMale.ToList())
                            {
                                leftoverMales.Remove(i);
                                blackList.Remove(i);
                                Monitor.Log("Successfully removed blacklisted NPC " + i + " from dancer pool.", LogLevel.Trace);
                            }

                            IEnumerable<string> toRemoveFemale = blackList.Intersect(leftoverFemales);
                            foreach (string j in toRemoveFemale.ToList())
                            {
                                leftoverFemales.Remove(j);
                                blackList.Remove(j);
                                Monitor.Log("Successfully removed blacklisted NPC " + j + " from dancer pool.", LogLevel.Trace);
                            }

                            //Logs blacklisting activity to monitor
                            if (blackList.Count < 1)
                            {
                                Monitor.Log("Successfully removed all blacklisted NPCs from dancer pool.", LogLevel.Trace);
                            }
                            else
                            {
                                if (Config.AllowTouristPartners.Equals(false))
                                {
                                    List<string> blackListTouristError = new List<string>();
                                    IEnumerable<string> blackListTourist = blackList.Intersect(Tourists);
                                    foreach (string i in blackListTourist.ToList())
                                    {
                                        blackListTouristError.Add(i);
                                        blackList.Remove(i);
                                        Tourists.Remove(i);
                                    }
                                    if (blackListTouristError.Count < 1)
                                    {
                                        string stringBlackListTouristError = string.Join(", ", blackListTouristError);
                                        Monitor.Log("The following blacklisted NPCs were already removed due to config- AllowTouristPartners = false : " + stringBlackListTouristError, LogLevel.Debug);
                                    }
                                }
                                if (Config.AllowNonBinaryPartners.Equals(false))
                                {
                                    List<string> blackListNBError = new List<string>();
                                    IEnumerable<string> blackListNB = blackList.Intersect(Tourists);
                                    foreach (string i in blackListNB.ToList())
                                    {
                                        blackListNBError.Add(i);
                                        blackList.Remove(i);
                                        nonBinary.Remove(i);
                                    }
                                    if (blackListNBError.Count < 1)
                                    {
                                        string stringBlackListNBError = string.Join(", ", blackListNBError);
                                        Monitor.Log("The following blacklisted NPCs were already removed due to config- AllowNonBinaryPartners = false : " + stringBlackListNBError, LogLevel.Debug);
                                    }
                                }
                                string blackListError = string.Join(", ", blackList);
                                Monitor.Log("Failed to remove the following blacklisted NPCs from dancer pool:" + blackListError + ". Please check that NPCs are referenced by key, and seperated by a single forward slash.", LogLevel.Debug);
                            }
                        }
                        catch (Exception e)
                        {
                            Monitor.Log("Flower Dance Fix failed to parse dancer blacklist. Please check that NPCs are referenced by key, and seperated by a single forward slash. " + e, LogLevel.Debug);
                        }
                    }

                    Monitor.Log("FDF has finalized the dance pools before pair generation.", LogLevel.Debug);
                    Monitor.Log("The following NPCs can dance in the top \"female\" line: " + string.Join(", ", leftoverFemales), LogLevel.Debug);
                    Monitor.Log("The following NPCs can dance in the bottom \"male\" line: " + string.Join(", ", leftoverMales), LogLevel.Debug);

                    //Adds farmer-farmer and farmer-NPC pairs to dancelist- vanilla code
                    List<Farmer> farmers = (from f in Game1.getOnlineFarmers()
                                            orderby f.UniqueMultiplayerID
                                            select f).ToList();
                    while (farmers.Count > 0)
                    {
                        Farmer f2 = farmers[0];
                        farmers.RemoveAt(0);

                        /*
                        if (multiplayer.isDisconnecting(f2) || f2.dancePartner.Value == null)
                        {
                            continue;
                        }
                        */

                        if (f2.dancePartner.GetGender() == 1)
                        {
                            females.Add(f2.dancePartner);
                            if (f2.dancePartner.IsVillager())
                            {
                                leftoverFemales.Remove(f2.dancePartner.TryGetVillager().Name);
                            }
                            males.Add(new NetDancePartner(f2));

                            Monitor.Log("Made a pair of farmer" + f2.name + " and NPC " + f2.dancePartner + " and successfully entered pair into NetDancePartner", LogLevel.Trace);
                        }
                        if (f2.dancePartner.GetGender() == 0)
                        {
                            males.Add(f2.dancePartner);
                            if (f2.dancePartner.IsVillager())
                            {
                                leftoverMales.Remove(f2.dancePartner.TryGetVillager().Name);
                            }
                            females.Add(new NetDancePartner(f2));

                            Monitor.Log("Made a pair of farmer" + f2.name + " and NPC " + f2.dancePartner + " and successfully entered pair into NetDancePartner", LogLevel.Trace);
                        }
                        if (f2.dancePartner.IsFarmer())
                        {
                            farmers.Remove(f2.dancePartner.TryGetFarmer());

                            Monitor.Log("Made a pair of farmer" + f2.name + " and farmer " + f2.dancePartner.TryGetFarmer() + " and successfully entered pair into NetDancePartner", LogLevel.Trace);
                        }
                        else if (f2.dancePartner.Equals(null))
                        {
                            Monitor.Log("Did not add farmer " + f2.name + " to NetDancePairs because they did not have a partner", LogLevel.Trace);
                        }
                    }

                    //Generates NPC-NPC pairs
                    do
                    {
                        int rF = rnd.Next(leftoverFemales.Count);
                        string female = leftoverFemales[rF];

                        //Random pair generation- config moderated
                        if (Config.NPCsHaveRandomPartners.Equals(true))
                        {
                            try
                            {
                                int r = rnd.Next(leftoverMales.Count);
                                string randomMale = leftoverMales[r];

                                females.Add(new NetDancePartner(female));
                                males.Add(new NetDancePartner(randomMale));

                                leftoverFemales.Remove(female);
                                leftoverMales.Remove(randomMale);

                                Monitor.Log("Randomly made a pair with " + female + " and " + randomMale + " and successfully entered pair into NetDancePartner", LogLevel.Trace);
                            }
                            catch (Exception)
                            {
                                Monitor.Log("Failed to fill NetDancePartner with random pairs.", LogLevel.Error);
                                break;
                            }
                        }

                        //"Love Interest" pair generation, followed by random pair generation for any remainders
                        if (Config.NPCsHaveRandomPartners.Equals(false))
                        {
                            //Vanilla love interest pair generation
                            //Checks if vanilla love interest matches custom love interest- rolls over to custom love interest method if different
                            if (hasVanillaLoveInterest(female).Equals(true) && getCustomLoveInterest(female) != null && Utility.getLoveInterest(female).Equals(getCustomLoveInterest(female)) && leftoverMales.Contains(getCustomLoveInterest(female)))
                            {
                                if (leftoverMales.Contains(getCustomLoveInterest(female)))
                                {
                                    string loveInterestMale = Utility.getLoveInterest(female);

                                    females.Add(new NetDancePartner(female));
                                    males.Add(new NetDancePartner(loveInterestMale));
                                    leftoverMales.Remove(loveInterestMale);
                                    leftoverFemales.Remove(female);

                                    Monitor.Log("Used vanilla \"Love Interest\" method to make a pair with " + female + " and " + loveInterestMale + " and successfully entered pair into NetDancePartner.", LogLevel.Trace);
                                }
                            }
                            //Custom Love Interest Pair Generation
                            else if (getCustomLoveInterest(female) != null)
                            {
                                string loveInterestMale = getCustomLoveInterest(female);
                                females.Add(new NetDancePartner(female));
                                males.Add(new NetDancePartner(loveInterestMale));
                                leftoverMales.Remove(loveInterestMale);
                                leftoverFemales.Remove(female);

                                Monitor.Log("Used custom \"Love Interest\" method to make a pair with " + female + " and " + loveInterestMale + " and successfully entered pair into NetDancePartner.", LogLevel.Trace);
                            }
                            //Random Pair Generation for NPCs 
                            else
                            {
                                int rM = rnd.Next(leftoverMales.Count);
                                string randomMale = leftoverMales[rM];

                                females.Add(new NetDancePartner(female));
                                males.Add(new NetDancePartner(randomMale));

                                leftoverFemales.Remove(female);
                                leftoverMales.Remove(randomMale);

                                Monitor.Log("Randomly made a pair with " + female + " and " + randomMale + " and successfully entered pair into NetDancePartner", LogLevel.Trace);
                            }
                        }
                    }
                    while ((females.Count < Config.MaxDancePairs) && leftoverFemales.Any() && leftoverMales.Any());

                    if (!leftoverFemales.Count.Equals(0))
                    {
                        string unselectedLOFemales = string.Join(", ", leftoverFemales);
                        Monitor.Log("After pair generation, the following NPCs not selected to dance in the top \"female\" line: " + unselectedLOFemales, LogLevel.Trace);
                    }

                    if (!leftoverMales.Count.Equals(0))
                    {
                        string unselectedLOMales = string.Join(", ", leftoverMales);
                        Monitor.Log("After pair generation, the following NPCs not selected to dance in the bottom \"male\" line: " + unselectedLOMales, LogLevel.Trace);
                    }

                    {
                        //Generates spring24.json "mainEvent" value

                        string rawFestivalData = __instance.GetFestivalDataForYear("mainEvent");
                        int i = 1;
                        do
                        {
                            string female2 = ((!females[i - 1].IsVillager()) ? ("farmer" + Utility.getFarmerNumberFromFarmer(females[i - 1].TryGetFarmer())) : females[i - 1].TryGetVillager().Name);
                            string male = ((!males[i - 1].IsVillager()) ? ("farmer" + Utility.getFarmerNumberFromFarmer(males[i - 1].TryGetFarmer())) : males[i - 1].TryGetVillager().Name);
                            rawFestivalData = rawFestivalData.Replace("Girl" + (i), female2);
                            rawFestivalData = rawFestivalData.Replace("Guy" + (i), male);
                            i++;
                        }
                        while (i <= Config.MaxDancePairs && i <= females.Count());

                        Regex regex = new Regex("showFrame (?<farmerName>farmer\\d) 44");
                        Regex showFrameGirl = new Regex("showFrame (?<farmerName>farmer\\d) 40");
                        Regex animation1Guy = new Regex("animate (?<farmerName>farmer\\d) false true 600 44 45");
                        Regex animation1Girl = new Regex("animate (?<farmerName>farmer\\d) false true 600 43 41 43 42");
                        Regex animation2Guy = new Regex("animate (?<farmerName>farmer\\d) false true 300 46 47");
                        Regex animation2Girl = new Regex("animate (?<farmerName>farmer\\d) false true 600 46 47");
                        rawFestivalData = regex.Replace(rawFestivalData, "showFrame $1 12/faceDirection $1 0");
                        rawFestivalData = showFrameGirl.Replace(rawFestivalData, "showFrame $1 0/faceDirection $1 2");
                        rawFestivalData = animation1Guy.Replace(rawFestivalData, "animate $1 false true 600 12 13 12 14");
                        rawFestivalData = animation1Girl.Replace(rawFestivalData, "animate $1 false true 596 4 0");
                        rawFestivalData = animation2Guy.Replace(rawFestivalData, "animate $1 false true 150 12 13 12 14");
                        rawFestivalData = animation2Girl.Replace(rawFestivalData, "animate $1 false true 600 0 3");
                        string[] newCommands = (__instance.eventCommands = rawFestivalData.Split('/'));

                    }
                }

                catch (Exception ex)
                {
                    Monitor.Log($"Failed in {nameof(setUpFestivalMainEvent_FDF)}:\n{ex}", LogLevel.Error);
                }
            }
        }
        public static bool hasFDFSprites(NPC character)
        {
            try
            {
                Texture2D FDFSpriteTestLoad = Helper.Content.Load<Texture2D>("assets/" + character.name + "_FDF.png", ContentSource.ModFolder);
                return true;
            }
            catch
            {
                Monitor.Log("Failed to load custom FDF sprites for " + character.name + ".", LogLevel.Debug);
                return false;
            }
        }
        public static bool hasVanillaLoveInterest(string character)
        {
            if (Utility.getLoveInterest(character).Equals(null))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string getCustomLoveInterest(string character)
        {
            try
            {
                NPC Target = Game1.getCharacterFromName(character);

                if (Target.loveInterest.Equals(null))
                {
                    return null;
                }

                NPC TargetLoveInterest = Game1.getCharacterFromName(Target.loveInterest);
                if (Target.loveInterest.Equals(TargetLoveInterest.name) && TargetLoveInterest.loveInterest.Equals(Target.name))
                {
                    return TargetLoveInterest.name;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
           
        }
    }
}
