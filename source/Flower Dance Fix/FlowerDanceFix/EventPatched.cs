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
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using Microsoft.Xna.Framework;
using System.Data.SqlClient;

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
                    List<NetDancePartner> upperLine = new List<NetDancePartner>();
                    List<NetDancePartner> lowerLine = new List<NetDancePartner>();

                    List<string> poolUpper = new List<string>();
                    List<string> poolLower   = new List<string>();

                    List<NPC> charList = new List<NPC>();
                    Utility.getAllCharacters(charList);

                    //Prevents selection of tourist datable characters based on config
                    if (Config.AllowTouristPartners.Equals(false))
                    {
                        foreach (NPC character in charList)
                        {
                            if (character.datable.Equals(true) && !character.homeRegion.Equals(2))
                            {
                                charList.Remove(character);
                                Monitor.Log($"Successfully removed tourist NPC {character.Name} from leftoverMales dancer pool. Configurable by AllowTouristPartners.", LogLevel.Trace);
                                break;
                            }
                        }
                    }

                    //Populates "leftoverGender" lists with all datable NPCs of each respective gender for selection, configurable switch for nonbinary characters
                    foreach (NPC character in charList)
                    {

                        if (character.datable.Equals(true) && character.Age != 2)
                        {
                            int intgender = character.Gender;

                            Monitor.Log($"{character.Name}'s gender is evaluated as {intgender}.", LogLevel.Trace);

                            //if (Config.ForceHeteroPartners.Equals(false))
                            //{
                               // try
                                //{
                                    //Monitor.Log("You shouldn't be seeing this at all, I disabled mixed gender lines", LogLevel.Alert);
                                //}
                                //catch (Exception e)
                                //{
                                    //Monitor.Log($"Failed to add {character} to a random dance line pool using mixed-gendered lines. Exception: {e}", LogLevel.Warn);
                                //}
                            //}
                            //else
                            {
                                switch (intgender)
                                {
                                    case 0:
                                        poolLower.Add(character.Name);
                                        Monitor.Log($"Successfully added {character.Name} to poolLower dancer pool.", LogLevel.Trace);
                                        break;

                                    case 1:
                                        poolUpper.Add(character.Name);
                                        Monitor.Log($"Successfully added {character.Name} to poolUpper dancer pool.", LogLevel.Trace);
                                        break;

                                    //case 2:
                                        //if (Config.AllowNonBinaryPartners.Equals(true))
                                        //{
                                            //try
                                            //{
                                                //check to see if nonbinary dance partner has custom FDF sprites
                                                //load custom FDF sprites if available

                                                //add nonbinary partner to random leftoverGender list

                                                // int g = rnd.Next(1);
                                                // if (g == 0)
                                                // {
                                                //     leftoverMales.Add(character.Name);
                                                //     Monitor.Log("Successfully added nonbinary NPC " + character.Name + " randomly to leftoverMales dancer pool.", LogLevel.Trace);
                                                // }
                                                // else
                                                // {
                                                //     leftoverFemales.Add(character.Name);
                                                //     Monitor.Log("Successfully added nonbinary NPC " + character.Name + " randomly to leftoverFemales dancer pool.", LogLevel.Trace);
                                                // }

                                                //break;
                                            //}
                                            //catch (Exception)
                                            //{
                                                //Monitor.Log($"Failed to find custom FDF sprites for {character.Name} and cannot add that NPC to dancer pools.", LogLevel.Debug);
                                                //continue;
                                            //}
                                        //}
                                        //else
                                        //{
                                            //Monitor.Log($"Failed to add nonbinary NPC {character.Name} to a leftoverGender dancer pool due to config- AllowNonBinaryPartners = false.", LogLevel.Debug);
                                            //break;
                                        //}
                                }
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    Monitor.Log("Finished adding NPCs to poolLine dancer pools.", LogLevel.Debug);

                    //Removes blacklisted datables from dancer pool lists based on config
                    if (String.IsNullOrEmpty(Config.DancerBlackList).Equals(false))
                    {
                        try
                        {
                            List<string> blackList = new List<string>(Config.DancerBlackList.Split('/').ToList());

                            IEnumerable<string> toRemoveLower = blackList.Intersect(poolLower);
                            foreach (string k in toRemoveLower.ToList())
                            {
                                poolLower.Remove(k);
                                blackList.Remove(k);
                                Monitor.Log($"Successfully removed blacklisted NPC {k} from dancer pool.", LogLevel.Trace);
                            }

                            IEnumerable<string> toRemoveUpper = blackList.Intersect(poolUpper);
                            foreach (string j in toRemoveUpper.ToList())
                            {
                                poolUpper.Remove(j);
                                blackList.Remove(j);
                                Monitor.Log($"Successfully removed blacklisted NPC {j} from dancer pool.", LogLevel.Trace);
                            }

                            //Logs blacklisting activity to monitor
                            if (blackList.Any().Equals(false))
                            {
                                Monitor.Log("Successfully removed all blacklisted NPCs from dancer pool.", LogLevel.Trace);
                            }
                            else
                            {
                                string blackListError = string.Join(", ", blackList);
                                Monitor.Log($"Failed to remove the following blacklisted NPCs from dancer pool: {blackListError}. Please check that NPCs are referenced by key, and seperated by a single forward slash.", LogLevel.Debug);
                            }
                        }
                        catch (Exception)
                        {
                            Monitor.Log("Flower Dance Fix failed to parse dancer blacklist. Please check that NPCs are referenced by key, and seperated by a single forward slash.", LogLevel.Debug);
                        }
                    }

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
                            upperLine.Add(f2.dancePartner);
                            if (f2.dancePartner.IsVillager())
                            {
                                poolUpper.Remove(f2.dancePartner.TryGetVillager().Name);
                            }
                            lowerLine.Add(new NetDancePartner(f2));

                            Monitor.Log($"Made a pair of farmer {f2.displayName} and NPC {f2.dancePartner} and successfully entered pair into NetDancePartner.", LogLevel.Trace);
                        }
                        else if (f2.dancePartner.GetGender() == 0)
                        {
                            lowerLine.Add(f2.dancePartner);
                            if (f2.dancePartner.IsVillager())
                            {
                                poolLower.Remove(f2.dancePartner.TryGetVillager().Name);
                            }
                            upperLine.Add(new NetDancePartner(f2));

                            Monitor.Log($"Made a pair of farmer {f2.displayName} and NPC {f2.dancePartner} and successfully entered pair into NetDancePartner.", LogLevel.Trace);
                        }
                        else if (f2.dancePartner.IsFarmer())
                        {
                            farmers.Remove(f2.dancePartner.TryGetFarmer());

                            Monitor.Log($"Made a pair of farmer {f2.displayName} and farmer {f2.dancePartner.TryGetFarmer()} and successfully entered pair into NetDancePartner.", LogLevel.Trace);
                        }
                        else
                        {
                            Monitor.Log($"Did not add farmer {f2.displayName} to NetDancePairs because they did not have a partner.", LogLevel.Trace);
                        }
                    }

                    //Generates NPC-NPC pairs
                    while ((upperLine.Count < Config.MaxDancePairs) && poolUpper.Any() && poolLower.Any())
                    {
                        int rF = rnd.Next(poolUpper.Count);
                        string upperDancer = poolUpper[rF];

                        //Random pair generation- config moderated
                        if (Config.NPCsHaveRandomPartners.Equals(true))
                        {
                            try
                            {
                                int r = rnd.Next(poolLower.Count);
                                string lowerDancer = poolLower[r];

                                upperLine.Add(new NetDancePartner(upperDancer));
                                lowerLine.Add(new NetDancePartner(lowerDancer));

                                poolUpper.Remove(upperDancer);
                                poolLower.Remove(lowerDancer);

                                Monitor.Log($"Randomly made a pair with {upperDancer} and {lowerDancer} and successfully entered pair into NetDancePartner.", LogLevel.Debug);
                            }
                            catch (Exception)
                            {
                                Monitor.Log("Failed to fill NetDancePartner with random M-F pairs.", LogLevel.Debug);
                                break;
                            }
                        }
                        //"Love Interest" pair generation, followed by random pair generation for any remainders
                        else
                        {
                            if (hasVanillaLoveInterest(upperDancer).Equals(true) && getCustomLoveInterest(upperDancer) != null && Utility.getLoveInterest(upperDancer).Equals(getCustomLoveInterest(upperDancer)) && poolLower.Contains(getCustomLoveInterest(upperDancer)))
                            {
                                string loveInterestLowerVanilla = Utility.getLoveInterest(upperDancer);

                                upperLine.Add(new NetDancePartner(upperDancer));
                                lowerLine.Add(new NetDancePartner(loveInterestLowerVanilla));
                                poolLower.Remove(loveInterestLowerVanilla);
                                poolUpper.Remove(upperDancer);

                                Monitor.Log($"Used vanilla \"Love Interest\" method to make a pair with {upperDancer} and {loveInterestLowerVanilla} and successfully entered pair into NetDancePartner.", LogLevel.Debug);
                            }
                            else if (getCustomLoveInterest(upperDancer) != null && poolLower.Contains(getCustomLoveInterest(upperDancer)))
                            {
                                string loveInterestLowerCustom = getCustomLoveInterest(upperDancer);

                                upperLine.Add(new NetDancePartner(upperDancer));
                                lowerLine.Add(new NetDancePartner(loveInterestLowerCustom));
                                poolUpper.Remove(upperDancer);
                                poolLower.Remove(loveInterestLowerCustom);
                                Monitor.Log($"Used custom \"Love Interest\" method to make a pair with {upperDancer} and {loveInterestLowerCustom} and successfully entered pair into NetDancePartner.", LogLevel.Debug);
                            }
                            else
                            {
                                int rM = rnd.Next(poolLower.Count);
                                string randomLower = poolLower[rM];

                                if (poolUpper.Contains(getCustomLoveInterest(randomLower)) && upperDancer != getCustomLoveInterest(randomLower))
                                {
                                    Monitor.Log($"Could not make a random pair of {upperDancer} and {randomLower} because {randomLower} has a valid love interest partner available for selection. {upperDancer} will be shuffled back into selection pool.", LogLevel.Trace);
                                    continue;
                                }
                                else
                                {
                                    upperLine.Add(new NetDancePartner(upperDancer));
                                    lowerLine.Add(new NetDancePartner(randomLower));

                                    poolUpper.Remove(upperDancer);
                                    poolLower.Remove(randomLower);

                                    Monitor.Log($"Randomly made a pair with {upperDancer} and {randomLower} and successfully entered pair into NetDancePartner.", LogLevel.Debug);
                                }
                            }
                        }
                    }

                    if (poolUpper.Any())
                    {
                        string unselectedUpper = String.Join(", ", poolUpper);
                        Monitor.Log($"After pair generation, poolUpper contains the following NPCs not selected for dance: {unselectedUpper}", LogLevel.Trace);
                    }

                    if (poolLower.Any())
                    {
                        string unselectedLower = String.Join(", ", poolLower);
                        Monitor.Log($"After pair generation, poolLower contains the following NPCs not selected for dance: {unselectedLower}", LogLevel.Trace);
                    }


                    //Generates spring24.json "mainEvent" value
                    string rawFestivalData = "";
                    try
                    {
                        StringBuilder buildFestivalData = new StringBuilder();

                        buildFestivalData.Append("pause 500/playMusic none/pause 500/globalFade/viewport -1000 -1000/loadActors MainEvent/warp farmer1 5 21/warp farmer2 11 21/warp farmer3 23 21/warp farmer4 12 21/faceDirection farmer1 2/faceDirection farmer3 2/faceDirection farmer4 2/faceDirection farmer 2");
                        buildFestivalData.Append(CustomDance.BuildEventWarpBlock(upperLine));
                        buildFestivalData.Append(CustomDance.BuildShowFrameBlock(upperLine));
                        buildFestivalData.Append("/viewport 14 25 clamp true/pause 2000/playMusic FlowerDance/pause 600");
                        buildFestivalData.Append(CustomDance.BuildAnimateBlock1(upperLine));
                        buildFestivalData.Append(CustomDance.BuildAnimateBlock2(upperLine));
                        buildFestivalData.Append("/pause 9600");
                        buildFestivalData.Append(CustomDance.BuildGiantOffsetBlock(upperLine));
                        buildFestivalData.Append(CustomDance.BuildAnimateBlock3(upperLine));
                        buildFestivalData.Append("/pause 7600");
                        buildFestivalData.Append(CustomDance.BuildStopAnimationBlock(upperLine));
                        buildFestivalData.Append("/pause 3000/globalFade/viewport -1000 -1000/message \"That was fun! Time to go home...\"/waitForOtherPlayers festivalEnd/end");

                        rawFestivalData = buildFestivalData.ToString();

                    }
                    catch (Exception e)
                    {
                        Monitor.Log("Failed to generate a custom Flower Dance template- reverting to vanilla Flower Dance Template. Exception: " + e, LogLevel.Debug);
                        rawFestivalData = __instance.GetFestivalDataForYear("mainEvent");
                    }

                    Monitor.Log(rawFestivalData, LogLevel.Trace);

                    int i = 1;

                    while (i <= Config.MaxDancePairs && i <= upperLine.Count())
                    {
                        string upperDancerScript = ((!upperLine[i - 1].IsVillager()) ? ("farmer" + Utility.getFarmerNumberFromFarmer(upperLine[i - 1].TryGetFarmer())) : upperLine[i - 1].TryGetVillager().Name);
                        string lowerDancerScript = ((!lowerLine[i - 1].IsVillager()) ? ("farmer" + Utility.getFarmerNumberFromFarmer(lowerLine[i - 1].TryGetFarmer())) : lowerLine[i - 1].TryGetVillager().Name);
                        rawFestivalData = rawFestivalData.Replace("Girl" + (i), upperDancerScript);
                        rawFestivalData = rawFestivalData.Replace("Guy" + (i), lowerDancerScript);
                        i++;
                    }

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

                    Monitor.Log(rawFestivalData, LogLevel.Trace);

                    string[] newCommands = (__instance.eventCommands = rawFestivalData.Split('/'));
                }

                catch (Exception ex)
                {
                    Monitor.Log($"Failed in {nameof(setUpFestivalMainEvent_FDF)}:\n{ex}", LogLevel.Error);
                }
            }
        }
        public static string getCustomLoveInterest(string character)
        {
            try
            {
                NPC Target = Game1.getCharacterFromName(character);

                //Test if love interest exists
                if (Target.loveInterest.Equals(null) || Target.loveInterest.Equals("null") || Target.loveInterest.Equals(""))
                {
                    return null;
                }

                NPC TargetLoveInterest = Game1.getCharacterFromName(Target.loveInterest);

                //Test if custom love interests are mutual
                if (TargetLoveInterest.loveInterest != null && TargetLoveInterest.loveInterest != "null" && TargetLoveInterest.loveInterest != "" && TargetLoveInterest.loveInterest.Equals(Target.Name) && Target.loveInterest.Equals(TargetLoveInterest.Name))
                {
                    return TargetLoveInterest.Name;
                }

                //If exists and is not mutual, return null
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Monitor.Log($"Failed to get a valid custom love interest value for {character}. Love interest value may be mispelled, or an uninstalled custom NPC. {e}", LogLevel.Debug);
                return null;
            }
        }
        public static bool hasVanillaLoveInterest(string character)
        {
            NPC Target = Game1.getCharacterFromName(character);

            if (Utility.getLoveInterest(character) == null)
            {
                return false;
            }
            else
                return true;
        }
      //public static void command_changeSprite_FDF(StardewValley.Event __instance, GameLocation location, GameTime time, string[] split)
      //    {
      //        if (__instance.isSpecificFestival("spring24"))
      //            {
      //                __instance.getActorByName(split[1]).Sprite.LoadTexture(split[1]);
      //                __instance.currentCommand++;
      //            };
      //    }
    }
}
