/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using BetterGarbageCans.Data;
using BetterGarbageCans.Framework;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;

namespace BetterGarbageCans.GamePatch
{
    static class GarbageCanOverrider
   {
        public static void prefix_betterGarbageCans(Town __instance, Location tileLocation, ref Farmer who, ref IList<bool> ___garbageChecked) 
        {
            if (__instance.map.GetLayer("Buildings").Tiles[tileLocation] != null)
            {
                if (__instance.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex == 78)
                {
                    string str = __instance.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
                    int index;
                    if (str == null)
                        index = -1;
                    else
                        index = Convert.ToInt32(str.Split(' ')[1]);
                    
                    if (index >= 0 && index < ___garbageChecked.Count)
                    {
                        
                        if (CanCheckGarbageCan((GARBAGE_CANS)index))
                        {
                            Game1.stats.incrementStat("trashCansChecked", 1);
                            BetterGarbageCansMod.Instance.garbageCans[(GARBAGE_CANS)index].LastTimeChecked = Game1.timeOfDay;
                            ___garbageChecked[index] = true;
                            var foundGarbageHat = CreateSoundAndSparks(__instance, tileLocation, index);
                            CheckForNPCMessages(__instance, tileLocation, ref who);
                            CheckForTreasure(__instance, tileLocation, index, foundGarbageHat, ref who);
                        }
                    }
                }
            }
        }

        private static bool CreateSoundAndSparks(GameLocation location, Location tileLocation, int index)
        {
            Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + 777 + index * 77);
            bool flag1 = Game1.stats.getStat("trashCansChecked") > 20U && random.NextDouble() < 0.01;
            bool flag2 = Game1.stats.getStat("trashCansChecked") > 20U && random.NextDouble() < 0.002;
            int num4 = Utility.getSeasonNumber(Game1.currentSeason) * 17;
            if (flag2)
                location.playSound("explosion", NetAudio.SoundContext.Default);
            else if (flag1)
                location.playSound("crit", NetAudio.SoundContext.Default);
            List<TemporaryAnimatedSprite> temporaryAnimatedSpriteList = new List<TemporaryAnimatedSprite>();
            temporaryAnimatedSpriteList.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", 
                new Microsoft.Xna.Framework.Rectangle(22 + num4, 0, 16, 10), 
                new Vector2((float)tileLocation.X, (float)tileLocation.Y) * 64f 
                + new Vector2(0.0f, -6f) * 4f, false, 0.0f, Color.White)
            {
                interval = flag2 ? 4000f : 1000f,
                motion = flag2 ? new Vector2(4f, -20f) 
                : new Vector2(0.0f, (float)((flag1 ? -7.0 
                : (double)(Game1.random.Next(-1, 3) + (Game1.random.NextDouble() < 0.1 ? -2 : 0))) - 8.0)),
                rotationChange = flag2 ? 0.4f : 0.0f,
                acceleration = new Vector2(0.0f, 0.7f),
                yStopCoordinate = tileLocation.Y * 64 - 24,
                layerDepth = flag2 ? 1f : (float)((tileLocation.Y + 1) * 64 + 2) / 10000f,
                scale = 4f,
                Parent = location,
                shakeIntensity = flag2 ? 0.0f : 1f,
                reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior)(x =>
                {
                    location.removeTemporarySpritesWithID(97654);
                    location.playSound("thudStep", NetAudio.SoundContext.Default);
                    for (int index2 = 0; index2 < 3; ++index2)
                        location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2((float)tileLocation.X, (float)tileLocation.Y) * 64f + new Vector2((float)(index2 * 6), (float)(Game1.random.Next(3) - 3)) * 4f, false, 0.02f, Color.DimGray)
                        {
                            alpha = 0.85f,
                            motion = new Vector2((float)((double)index2 * 0.300000011920929 - 0.600000023841858), -1f),
                            acceleration = new Vector2(1f / 500f, 0.0f),
                            interval = 99999f,
                            layerDepth = (float)((tileLocation.Y + 1) * 64 + 3) / 10000f,
                            scale = 3f,
                            scaleChange = 0.02f,
                            rotationChange = (float)((double)Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
                            delayBeforeAnimationStart = 50
                        });
                }),
                id = 97654f
            });
            if (flag2)
                temporaryAnimatedSpriteList.Last<TemporaryAnimatedSprite>().reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSpriteList.Last<TemporaryAnimatedSprite>().bounce);
            temporaryAnimatedSpriteList.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", 
                new Microsoft.Xna.Framework.Rectangle(22 + num4, 11, 16, 16), 
                new Vector2((float)tileLocation.X, (float)tileLocation.Y) * 64f 
                + new Vector2(0.0f, -5f) * 4f, false, 0.0f, Color.White)
            {
                interval = flag2 ? 999999f : 1000f,
                layerDepth = (float)((tileLocation.Y + 1) * 64 + 1) / 10000f,
                scale = 4f,
                id = 97654f
            });
            for (int index2 = 0; index2 < 5; ++index2)
                temporaryAnimatedSpriteList.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", 
                    new Microsoft.Xna.Framework.Rectangle(22 + Game1.random.Next(4) * 4, 32, 4, 4), 
                    new Vector2((float)tileLocation.X, (float)tileLocation.Y) * 64f 
                    + new Vector2((float)Game1.random.Next(13), (float)(Game1.random.Next(3) - 3)) * 4f, false, 0.0f, Color.White)
                {
                    interval = 500f,
                    motion = new Vector2((float)Game1.random.Next(-2, 3), -5f),
                    acceleration = new Vector2(0.0f, 0.4f),
                    layerDepth = (float)((tileLocation.Y + 1) * 64 + 3) / 10000f,
                    scale = 4f,
                    color = Utility.getRandomRainbowColor((Random)null),
                    delayBeforeAnimationStart = Game1.random.Next(100)
                });
            BetterGarbageCansMod.multiplayer.broadcastSprites(location, temporaryAnimatedSpriteList);
            location.playSound("trashcan", NetAudio.SoundContext.Default);
            
            return flag2;
        }

        private static void CheckForTreasure(GameLocation location, Location tileLocation, int index, bool foundHat, ref Farmer player)
        {
            var playerHat = player.hat.Value;

            if (foundHat && !player.hasItemInInventoryNamed("Garbage Hat")
                && (playerHat == null || (playerHat != null && playerHat.which.Value != 66)))
            {
                player.addItemByMenuIfNecessary(new Hat(66), null);
            }
            else
            {
                Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + 777 + index + Game1.timeOfDay);
                if (random.NextDouble() < BetterGarbageCansMod.Instance.config.baseChancePercent + Game1.player.DailyLuck)
                {
                    Item reward = GetTreasure(index, random);

                    if (reward != null)
                    {
                        Vector2 origin = new Vector2((float)tileLocation.X + 0.5f, (float)(tileLocation.Y - 1)) * 64f;

                        Game1.createItemDebris(reward, origin, 2, location, (int)origin.Y + 64);
                        BetterGarbageCansMod.Instance.garbageCans[(GARBAGE_CANS)index].LastTimeFoundItem = Game1.timeOfDay;
                    }
                }
            }
        }

        private static bool CanCheckGarbageCan(GARBAGE_CANS can)
        {
            bool canCheckForItems = false;

            //Has the garbage can been checked today?
            if (BetterGarbageCansMod.Instance.garbageCans[can].LastTimeChecked == -1) //LastTimeChecked defaults per day to -1
            {
                // You have never checked that can...
                canCheckForItems = true;
            }
            else if (BetterGarbageCansMod.Instance.config.allowGarbageCanRecheck) //You have check the garbage can before.... and you can recheck it
            {
                //See if enough time has pasted since last check
                if (CanCheckBasedOnLastCheckTime(can))
                {                    
                    if (BetterGarbageCansMod.Instance.garbageCans[can].LastTimeFoundItem == -1)
                    {
                        //Enough time has passed, and never found anything...
                        canCheckForItems = true;
                    } 
                    else if (BetterGarbageCansMod.Instance.config.allowMultipleItemsPerDay) // So you have found at least something before
                    {
                        //See if enough time has pasted since last found item
                        canCheckForItems = CanCheckBasedOnLastFoundTime(can);
                    }
                }
            }

            return canCheckForItems;
        }

        private static bool CanCheckBasedOnLastCheckTime(GARBAGE_CANS can)
        {
            return Game1.timeOfDay >= GetWaitTime(BetterGarbageCansMod.Instance.garbageCans[can].LastTimeChecked,
                        BetterGarbageCansMod.Instance.config.WaitTimeIfFoundNothing);
        }

        private static bool CanCheckBasedOnLastFoundTime(GARBAGE_CANS can)
        {
            return Game1.timeOfDay >= GetWaitTime(BetterGarbageCansMod.Instance.garbageCans[can].LastTimeFoundItem,
                BetterGarbageCansMod.Instance.config.WaitTimeIfFoundSomething);
        }

        private static int GetWaitTime(int time, int addMinutes)
        {
            int hours = time / 100;
            int minutes = (time % 100) + addMinutes;

            return ((hours + minutes / 60) * 100) + (minutes % 60);
        }

        private static void CheckForNPCMessages(GameLocation location, Location tileLocation, ref Farmer player)
        {
            bool changedFrienship = false;
            Character character = Utility.isThereAFarmerOrCharacterWithinDistance(new Vector2((float)tileLocation.X, (float)tileLocation.Y), 7, location);
            if (character != null && character is NPC && !(character is Horse))
            {
                BetterGarbageCansMod.SendMulitplayerMessage("TrashCan", Game1.player.Name, character.Name);

                if (character.Name.Equals("Linus"))
                {
                    character.doEmote(32, true);
                    (character as NPC).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Linus"), true, true);
                    player.changeFriendship(BetterGarbageCansMod.Instance.config.LinusFriendshipPoints, character as NPC);
                    changedFrienship = true;
                    BetterGarbageCansMod.SendMulitplayerMessage("LinusTrashCan");
                }
                else if ((character as NPC).Age == 2)
                {
                    character.doEmote(28, true);
                    (character as NPC).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Child"), true, true);                    
                }
                else if ((character as NPC).Age == 1)
                {
                    character.doEmote(8, true);
                    (character as NPC).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Teen"), true, true);                
                }
                else
                {
                    character.doEmote(12, true);
                    (character as NPC).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Adult"), true, true);                
                }
                if (!changedFrienship)
                {
                    player.changeFriendship(BetterGarbageCansMod.Instance.config.FriendshipPoints, character as NPC);
                }

                Game1.drawDialogue(character as NPC);
            }
        }

        private static Item GetCustomTrashTreasure(GARBAGE_CANS index)
        {
            GarbageCan garbageCan = BetterGarbageCansMod.Instance.garbageCans[index];

            // Possible treasure based on selected treasure group selected above.
            List<TrashTreasure> possibleLoot = new List<TrashTreasure>(garbageCan.treasureList)
                .Where(loot => loot.Enabled && loot.IsValid())
                .OrderBy(loot => loot.Chance)
                .ThenBy(loot => loot.Id)
                .ToList();

            if (possibleLoot.Count == 0)
            {
                BetterGarbageCansMod.Instance.Monitor.Log($"   Group: {garbageCan.GarbageCanID}, No Possible Loot Found... check the logic");
            }

            TrashTreasure treasure = possibleLoot.ChooseItem(Game1.random);
            int id = treasure.Id;

            // Lost books have custom handling  -- No default lost books... but someonw might configure them
            if (id == 102) // LostBook Item ID
            {
                if (Game1.player.archaeologyFound == null || !Game1.player.archaeologyFound.ContainsKey(102) || Game1.player.archaeologyFound[102][0] >= 21)
                {
                    possibleLoot.Remove(treasure);
                }
                Game1.showGlobalMessage("You found a lost book. The library has been expanded.");
            }

            // Create reward item
            Item reward;

            if ((id >= 516 && id <= 534) || id == 810 || id == 811)
            {
                reward = new Ring(id);
            }
            else if ((id >= 504 && id <= 515) || id == 804 || id == 806)
            {
                reward = new Boots(id);
            }
            else
            {
                int count = Game1.random.Next(treasure.MinAmount, treasure.MaxAmount);
                reward = (Item)new StardewValley.Object(id, count);
            }

            if (Game1.random.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS", (SpecialOrder)null))
                reward = (Item)new StardewValley.Object(890, 1);

            return reward;
        }

        private static Item GetDefaultTrashTreasure(int index, Random random, int X, int Y)
        {
            int parentSheetIndex = 168;
            switch (random.Next(10))
            {
                case 0:
                    parentSheetIndex = 168;
                    break;
                case 1:
                    parentSheetIndex = 167;
                    break;
                case 2:
                    parentSheetIndex = 170;
                    break;
                case 3:
                    parentSheetIndex = 171;
                    break;
                case 4:
                    parentSheetIndex = 172;
                    break;
                case 5:
                    parentSheetIndex = 216;
                    break;
                case 6:
                    parentSheetIndex = Utility.getRandomItemFromSeason(Game1.currentSeason, X * 653 + Y * 777, false);
                    break;
                case 7:
                    parentSheetIndex = 403;
                    break;
                case 8:
                    parentSheetIndex = 309 + random.Next(3);
                    break;
                case 9:
                    parentSheetIndex = 153;
                    break;
            }

            if (index == 3 && random.NextDouble() < 0.2 + Game1.player.DailyLuck)  
            {
                parentSheetIndex = 535;
                if (random.NextDouble() < 0.05)
                    parentSheetIndex = 749;
            }

            if (index == 4 && random.NextDouble() < 0.2 + Game1.player.DailyLuck)  
            {
                parentSheetIndex = 378 + random.Next(3) * 2;
                random.Next(1, 5);
            }

            if (index == 5 && random.NextDouble() < 0.2 + Game1.player.DailyLuck && Game1.dishOfTheDay != null)  
                parentSheetIndex = Game1.dishOfTheDay.ParentSheetIndex != 217 ? Game1.dishOfTheDay.ParentSheetIndex : 216;

            if (index == 6 && random.NextDouble() < 0.2 +  Game1.player.DailyLuck)  
                parentSheetIndex = 223;
            if (index == 7 && random.NextDouble() < 0.2)
            {
                if (!Utility.HasAnyPlayerSeenEvent(191393))
                    parentSheetIndex = 167;
                if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater") 
                    && !Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja"))
                    parentSheetIndex = random.NextDouble() >= 0.25 ? 270 : 809;
            }

            if (Game1.random.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS", (SpecialOrder)null))
                parentSheetIndex = 890;

            return (Item)new StardewValley.Object(parentSheetIndex, 1);
        }

        internal static Item GetTreasure(int index, Random random, int X = 0, int Y = 0)
        {
            if (BetterGarbageCansMod.Instance.config.useCustomGarbageCanTreasure)
            {
                return GetCustomTrashTreasure((GARBAGE_CANS) index);
            }
            else
            {
                return GetDefaultTrashTreasure(index, random, X, Y);
            }
        }
    }
}
