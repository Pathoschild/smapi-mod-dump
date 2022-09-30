/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/FarmhouseVisits
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Collections.Generic;
using lv = StardewModdingAPI.LogLevel;

namespace FarmVisitors
{
    public enum DialogueType
    {
        Introduce,
        WalkIn,
        Furniture,
        Greet,
        Retiring,
        Thanking,
        Rejected
    }
    internal class Values
    {
        internal static bool IsMarriedToPlayer(NPC c)
        {
            if (c is null)
            {
                return false;
            }
            else
            {         
                Farmer player = Game1.MasterPlayer;
                if (!player.friendshipData.ContainsKey(c.Name))
                {
                    ModEntry.Log(
                        $"{c.Name} is not in the dictionary.", 
                        lv.Trace);
                    return false;
                }
                if (player.friendshipData[c.Name].IsMarried())
                {
                    ModEntry.Log(
                        $"{c.Name} is married!", 
                        lv.Trace);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal static bool IsDivorced(NPC c)
        {
            if (c is null)
            {
                return false;
            }
            else
            {         
                Farmer player = Game1.MasterPlayer;
                if (!player.friendshipData.ContainsKey(c.Name))
                {
                    /*ModEntry.Log($"Divorced NPC check: {c.Name} is not in the dictionary.");
                     * taking this out because a message like this is already sent by IsMarried (to avoid dupes and not confuse myself in the future)
                     */
                    return false;
                }
                if (player.friendshipData[c.Name].IsDivorced())
                {
                    ModEntry.Log(
                        $"{c.Name} is divorced!", 
                        lv.Trace);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal static bool IsVisitor(string c)
        {
            if (c.Equals(ModEntry.VisitorName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// Checks if a NPC is free to visit player.
        /// </summary>
        /// <param name="who">NPC to check</param>
        /// <param name="IsRandom">If it's a randomly-chosen visit.</param>
        /// <returns></returns>
        internal static bool IsFree(NPC who, bool IsRandom)
        {
            //if character is robin and there's a construction ongoing
            if (who.Name == "Robin" && Game1.getFarm().isThereABuildingUnderConstruction())
            {
                ModEntry.Log(
                    $"{who.displayName} is building in the Farm. Visit will be cancelled.", 
                    lv.Trace);
                return false;
            }

            var isHospitalDay = Utility.IsHospitalVisitDay(who.Name);
            var visitedToday = ModEntry.TodaysVisitors.Contains(who.Name);
            var visitingIsland = Game1.IsVisitingIslandToday(who.Name);
            var isSleeping = who.isSleeping.Value;

            if (visitedToday && !IsRandom)
            {
                ModEntry.Log(
                    $"{who.displayName} has already visited the Farm today!",
                    lv.Trace);
            }
            if (isHospitalDay)
            {
                ModEntry.Log(
                    $"{who.displayName} has a hospital visit scheduled today. They won't visit the farmer.", 
                    lv.Trace);
            }
            if (visitingIsland)
            {
                ModEntry.Log(
                    $"{who.displayName} is visiting the island today!",
                    lv.Trace);
            }
            if (isSleeping)
            {
                ModEntry.Log(
                    $"{who.displayName} is sleeping right now.",
                    lv.Trace);
            }

            if (IsRandom)
            {
                return !visitedToday && !isHospitalDay && !visitingIsland && !isSleeping;
            }
            else
            {
                return !isHospitalDay && !visitingIsland && !isSleeping;
            }
        }
        
        /// <summary>
        /// Retrieves a dialogue of the specified type.
        /// </summary>
        /// <param name="c">The NPC whose info to use.</param>
        /// <param name="which">The type of dialogue.</param>
        /// <returns></returns>
        internal static string GetDialogueType(NPC c, DialogueType which)
        {
            //allowed values: Introduce, Rejected, Greet, Retiring, WalkIn, Furniture
            
            var r = Game1.random.Next(1, 4);
            int type = Game1.random.Next(0, 3);
            if (type is 0)
            {
                if (c.SocialAnxiety.Equals(0))
                {
                    return ModEntry.TL.Get($"NPC{which}.Outgoing{r}");
                }
                else
                {
                    return ModEntry.TL.Get($"NPC{which}.Shy{r}");
                }
            }
            else
            {
                if (c.Manners.Equals(1)) //polite
                {
                    return ModEntry.TL.Get($"NPC{which}.Polite{r}");
                }
                else if (c.Manners.Equals(2)) //rude
                {
                    return ModEntry.TL.Get($"NPC{which}.Rude{r}");
                }
                else //neutral
                {
                    return ModEntry.TL.Get($"NPC{which}.Neutral{r}");
                }
            }
        }

        /// <summary>
        /// Returns dialogue related to gifting.
        /// </summary>
        /// <param name="c">Character whose data is used.</param>
        /// <returns></returns>
        internal static string GetGiftDialogue(NPC c)
        {
            var r = Game1.random.Next(1, 6);

            if(Game1.random.Next(0,4).Equals(1))
            {
                if (c.SocialAnxiety.Equals(0))
                {
                    return ModEntry.TL.Get($"NPCGift.Normal{r}");
                }
                else
                {
                    return ModEntry.TL.Get($"NPCGift.Kind{r}");
                }
            }

            if (c.Manners.Equals(1)) //polite
            {
                return ModEntry.TL.Get($"NPCGift.Kind{r}");
            }
            else if (c.Manners.Equals(2)) //rude
            {
                return ModEntry.TL.Get($"NPCGift.Lax{r}");
            }
            else
            {
                return ModEntry.TL.Get($"NPCGift.Normal{r}");
            }
        }
        
        /// <summary>
        /// Returns translation string, for when NPC leaves and player isn't home.
        /// </summary>
        /// <param name="IsCellar">Whether the player is in the Cellar.</param>
        /// <returns></returns>
        internal static string GetNPCGone(bool IsCellar)
        {
            if(IsCellar)
            {
                return ModEntry.TL.Get("NPCGone_Cellar");
            }
            else
            {
                return ModEntry.TL.Get("NPCGoneWhileOutside");
            }
        }
        
        /// <summary>
        /// Return gift IDs depending on season.
        /// </summary>
        /// <returns></returns>
        internal static string GetSeasonalGifts()
        {
            string defaultgifts = "419 246 423 216 247 688 176 180 436 174 182 184 424 186 438"; //Vinegar, Wheat Flour, Rice, Bread, Oil, Warp Totem: Farm, small Egg (2 types),Large Egg (2 types), Goat Milk, Milk, Cheese, Large Milk, L. Goat Milk
            if (Game1.IsSpring)
            {
                return $"{defaultgifts} 400 252 222 190 610 715"; //Strawberry, Rhubarb, Rhubarb Pie, Cauliflower, Fruit Salad, Lobster
            }
            else if (Game1.IsSummer)
            {
                return $"{defaultgifts} 636 256 258 270 690 260"; //Peach, Tomato, Blueberry, Corn, Warp Totem: Beach, Hot Pepper
            }
            else if (Game1.IsFall)
            {
                return $"{defaultgifts} 276 284 282 395 689 613"; //Pumpkin, Beet, Cranberries, Coffee, Warp Totem: Mountains, Apple
            }
            else if (Game1.IsWinter)
            {
                return $"{defaultgifts} 414 144 147 178 787 440"; //Crystal Fruit, Pike, Herring, Hay, Battery Pack, Wool
            }
            else
            {
                return defaultgifts;
            }
        }
        
        /// <summary>
        /// Get a random index from the player's furniture list.
        /// </summary>
        /// <returns></returns>
        internal static string GetRandomFurniture()
        {
            var list = ModEntry.FurnitureList;

            //if there's no furniture, return null
            if (list is null)
            {
                return null;
            }

            //choose random index and return itsdisplayname
            int amount = list.Count;
            var r = Game1.random.Next(0, (amount + 1));
            return list[r];
        }
        
        /// <summary>
        /// Updates furniture list.
        /// </summary>
        /// <param name="farmHouse">The farmhouse whose data to use.</param>
        /// <returns></returns>
        internal static List<string> UpdateFurniture(FarmHouse farmHouse)
        {
            List<string> templist = new();
            foreach (Furniture f in farmHouse.furniture)
            {
                //templist.Add(f.DisplayName);
                templist.Add($"\"{f.DisplayName.ToLower()}\"");
            }
            return templist;
        }
    }
}