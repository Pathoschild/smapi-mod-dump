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

namespace FlowerDanceFix
{
    class OriginalMethod
    {
        /*
        public static IMonitor Monitor;
        public static IModHelper Helper;
        public static ModConfig Config;

        public static void Initialize(IMonitor monitor, ModConfig config, IModHelper helper)
        {
            Monitor = monitor;
            Config = config;
            Helper = helper;
        }

        public void setUpFestivalMainEvent_Original(StardewValley.Event __Instance)
        {

            if (!__Instance.isSpecificFestival("spring24"))
            {
                return;
            }
            List<NetDancePartner> females = new List<NetDancePartner>();
            List<NetDancePartner> males = new List<NetDancePartner>();
            List<string> leftoverFemales = new List<string> { "Abigail", "Penny", "Leah", "Maru", "Haley", "Emily" };
            List<string> leftoverMales = new List<string> { "Sebastian", "Sam", "Elliott", "Harvey", "Alex", "Shane" };
            List<Farmer> farmers = (from f in Game1.getOnlineFarmers()
                                    orderby f.UniqueMultiplayerID
                                    select f).ToList();
            while (farmers.Count > 0)
            {
                Farmer f2 = farmers[0];
                farmers.RemoveAt(0);

                
                //Reflection to access protected list disconnectingFarmers

                List<long> disconnectingFarmers = Helper.Reflection
                 .GetField<List<long>>(typeof(Multiplayer), "disconnectingFarmers");

                long[] disconnectingFarmersArray = disFA;
                    
                 long disFA = typeof(IReflectedField<long>) disconnectingFarmers;

                //Setting up structs for disconnection handling

                bool isDisconnectingIUD (long iud)
                {
                    return disconnectingFarmersArray.Contains(iud);
                }

                bool isDisconnectingFarmer (Farmer farmer)
                {
                    return isDisconnectingIUD(farmer.UniqueMultiplayerID);
                }

                //Disconnection handling

                if (isDisconnectingFarmer(f2) || f2.dancePartner.Value == null)
                {
                    continue;
                }
                

                //Adds Farmer-Female NPC dance pairs to NetDancePartner
                if (f2.dancePartner.GetGender() == 1)
                {
                    females.Add(f2.dancePartner);
                    if (f2.dancePartner.IsVillager())
                    {
                        leftoverFemales.Remove(f2.dancePartner.TryGetVillager().Name);
                    }
                    males.Add(new NetDancePartner(f2));
                }
                else
                {

                    //Adds Farmer-Male NPC dance pairs to NetDancePartner
                    males.Add(f2.dancePartner);
                    if (f2.dancePartner.IsVillager())
                    {
                        leftoverMales.Remove(f2.dancePartner.TryGetVillager().Name);
                    }
                    females.Add(new NetDancePartner(f2));
                }

                //Adds Farmer-Farmer NPC dance pairs to NetDancePartner
                if (f2.dancePartner.IsFarmer())
                {
                    farmers.Remove(f2.dancePartner.TryGetFarmer());
                }
            }
            while (females.Count < 6)
            {
                string female = leftoverFemales.Last();
                if (leftoverMales.Contains(Utility.getLoveInterest(female)))
                {
                    females.Add(new NetDancePartner(female));
                    males.Add(new NetDancePartner(Utility.getLoveInterest(female)));
                }
                leftoverFemales.Remove(female);
            }
            string rawFestivalData = __Instance.GetFestivalDataForYear("mainEvent");
            for (int i = 1; i <= 6; i++)
            {

                //Gotta figure out how to get Utility.getFarmerNumberFromFarmer

                string female2 = ((!females[i - 1].IsVillager()) ? ("farmer" + Utility.getFarmerNumberFromFarmer(females[i - 1].TryGetFarmer())) : females[i - 1].TryGetVillager().Name);
                string male = ((!males[i - 1].IsVillager()) ? ("farmer" + Utility.getFarmerNumberFromFarmer(males[i - 1].TryGetFarmer())) : males[i - 1].TryGetVillager().Name);
                rawFestivalData = rawFestivalData.Replace("Girl" + i, female2);
                rawFestivalData = rawFestivalData.Replace("Guy" + i, male);
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
            string[] newCommands = (__Instance.eventCommands = rawFestivalData.Split('/'));
        }

        public static bool HasIslandAttire(NPC character)
        {
            try
            {
                Game1.temporaryContent.Load<Texture2D>("Characters\\" + NPC.getTextureNameForCharacter(character.name.Value) + "_Beach");
                if (character != null && character.Name == "Lewis")
                {
                    foreach (Farmer farmer in Game1.getAllFarmers())
                    {
                        if (farmer != null && farmer.activeDialogueEvents != null && farmer.activeDialogueEvents.ContainsKey("lucky_pants_lewis"))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        //From NPC.cs, line 3755
        private void finishEndOfRouteAnimation()
        {
            _finishingEndOfRouteBehavior = _startedEndOfRouteBehavior;
            _startedEndOfRouteBehavior = null;
            if (_finishingEndOfRouteBehavior == "change_beach")
            {
                shouldWearIslandAttire.Value = true;
                currentlyDoingEndOfRouteAnimation = false;
            }
            else if (_finishingEndOfRouteBehavior == "change_normal")
            {
                shouldWearIslandAttire.Value = false;
                currentlyDoingEndOfRouteAnimation = false;
            }

        }

        //From NPC.cs, line 3062
        public virtual void wearIslandAttire()
        {
            try
            {
                Sprite.LoadTexture("Characters\\" + getTextureNameForCharacter(name.Value) + "_Beach");
            }
            catch (ContentLoadException)
            {
                Sprite.LoadTexture("Characters\\" + getTextureNameForCharacter(name.Value));
            }
            isWearingIslandAttire = true;
            resetPortrait();
        }

        public virtual void wearNormalClothes()
        {
            Sprite.LoadTexture("Characters\\" + getTextureNameForCharacter(name.Value));
            isWearingIslandAttire = false;
            resetPortrait();
        }
        */
    }
}
