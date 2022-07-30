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

namespace FarmVisitors
{
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
                    ModEntry.Mon.Log($"{c.Name} is not in the dictionary.");
                    return false;
                }
                if (player.friendshipData[c.Name].IsMarried())
                {
                    ModEntry.Mon.Log($"{c.Name} is married!");
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
                    ModEntry.Mon.Log($"{c.Name} is not in the dictionary.");
                    return false;
                }
                if (player.friendshipData[c.Name].IsDivorced())
                {
                    ModEntry.Mon.Log($"{c.Name} is divorced!");
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

        internal static string GetIntroDialogue(NPC npcv)
        {
            var r = Game1.random.Next(1,4);
            int type = Game1.random.Next(0,3);
            if(type is 0)
            {
                if(npcv.SocialAnxiety.Equals(0))
                {
                    return ModEntry.Help.Translation.Get($"NPCIntroduce.Outgoing{r}");
                }
                else
                {
                    return ModEntry.Help.Translation.Get($"NPCIntroduce.Shy{r}");
                }
            }
            else
            {
                if (npcv.Manners.Equals(1)) //polite
                {
                    return ModEntry.Help.Translation.Get($"NPCIntroduce.Polite{r}");
                }
                else if (npcv.Manners.Equals(2)) //rude
                {
                    return ModEntry.Help.Translation.Get($"NPCIntroduce.Rude{r}");
                }
                else //neutral
                {
                    return ModEntry.Help.Translation.Get($"NPCIntroduce.Neutral{r}");
                }
            }
        }

        internal static string StringByPersonality(NPC c)
        {
            var r = Game1.random.Next(1, 4);
            int type = Game1.random.Next(0,3);
            if(type is 0)
            {
                if(c.SocialAnxiety.Equals(0))
                {
                    return ModEntry.Help.Translation.Get($"NPCGreet.Outgoing{r}");
                }
                else
                {
                    return ModEntry.Help.Translation.Get($"NPCGreet.Shy{r}");
                }
            }
            else
            {
                if (c.Manners.Equals(1)) //polite
                {
                    return ModEntry.Help.Translation.Get($"NPCGreet.Polite{r}");
                }
                else if (c.Manners.Equals(2)) //rude
                {
                    return ModEntry.Help.Translation.Get($"NPCGreet.Rude{r}");
                }
                else //neutral
                {
                    return ModEntry.Help.Translation.Get($"NPCGreet.Neutral{r}");
                }
            }
        }

        internal static string GetNPCGone()
        {
            return ModEntry.Help.Translation.Get("NPCGoneWhileOutside");
        }

        internal static string NPCGone_Cellar()
        {
            return ModEntry.Help.Translation.Get("NPCGone_Cellar");
        }

        internal static string GetRetireDialogue(NPC c)
        {
            int type = Game1.random.Next(0,3);
            if(type is 0)
            {
                if(c.SocialAnxiety.Equals(0))
                {
                    return ModEntry.Help.Translation.Get($"NPCRetiring.Outgoing");
                }
                else
                {
                    return ModEntry.Help.Translation.Get($"NPCRetiring.Shy");
                }
            }
            else
            {
                if (c.Manners.Equals(1)) //polite
                {
                    return ModEntry.Help.Translation.Get($"NPCRetiring.Polite");
                }
                else if (c.Manners.Equals(2)) //rude
                {
                    return ModEntry.Help.Translation.Get($"NPCRetiring.Rude");
                }
                else //neutral
                {
                    return ModEntry.Help.Translation.Get($"NPCRetiring.Neutral");
                }
            }
        }

        internal static string GetTextOverHead(NPC c)
        {
            var r = Game1.random.Next(1, 4);
            int type = Game1.random.Next(0,3);
            if(type is 0)
            {
                if(c.SocialAnxiety.Equals(0))
                {
                    return ModEntry.Help.Translation.Get($"NPCWalkIn.Outgoing{r}");
                }
                else
                {
                    return ModEntry.Help.Translation.Get($"NPCWalkIn.Shy{r}");
                }
            }
            else
            {
                if (c.Manners.Equals(1)) //polite
                {
                    return ModEntry.Help.Translation.Get($"NPCWalkIn.Polite{r}");
                }
                else if (c.Manners.Equals(2)) //rude
                {
                    return ModEntry.Help.Translation.Get($"NPCWalkIn.Rude{r}");
                }
                else //neutral
                {
                    return ModEntry.Help.Translation.Get($"NPCWalkIn.Neutral{r}");
                }
            }
        }

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

        internal static string TalkAboutFurniture(NPC c)
        {
            var r = Game1.random.Next(1, 4);
            int type = Game1.random.Next(0,3);
            if(type is 0)
            {
                if(c.SocialAnxiety.Equals(0))
                {
                    return ModEntry.Help.Translation.Get($"NPCFurniture.Outgoing{r}");
                }
                else
                {
                    return ModEntry.Help.Translation.Get($"NPCFurniture.Shy{r}");
                }
            }
            else
            {
                if (c.Manners.Equals(1)) //polite
                {
                    return ModEntry.Help.Translation.Get($"NPCFurniture.Polite{r}");
                }
                else if (c.Manners.Equals(2)) //rude
                {
                    return ModEntry.Help.Translation.Get($"NPCFurniture.Rude{r}");
                }
                else //neutral
                {
                    return ModEntry.Help.Translation.Get($"NPCFurniture.Neutral{r}");
                }
            }
        }
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