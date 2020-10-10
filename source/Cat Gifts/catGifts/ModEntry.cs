/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/skuldomg/catGifts
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;
using StardewValley.TerrainFeatures;

namespace catGifts
{
    public class ModEntry : Mod
    {
        private List<int> lowGifts;
        private List<int> midGifts;
        private List<int> hiGifts;
        private int giftsGiven = 0;
        private bool highTierYesterday = false;
        private bool giftToday = false;
        private bool warpedToday = false;
        private int giftId = 0;
        private Vector2 tile = new Vector2(-1, -1);
        private StardewValley.Characters.Pet thePet = null;
        public static bool msgDisplayed = false;
        public static bool isCat = true;

        // Configuration options
        private int THRESHOLD_1 = -1;
        private int THRESHOLD_2 = -1;
        private int THRESHOLD_3 = -1;
        private int GIFT_CHANCE_1 = -1;
        private int GIFT_CHANCE_2 = -1;
        private int GIFT_CHANCE_3 = -1;
        private int FARMHAND_CHANCE = -1;
        private int LOW_CHANCE = -1;
        private int MID_CHANCE = -1;
        private int HI_CHANCE = -1;
        private int MAX_WEEKLY_GIFTS = -1;

        // TODO: Add custom items, ie. dead bird, dead mouse, other trash, ...
        public override void Entry(IModHelper helper)
        {
            // Initialize config fields and load cat/dog icons
            ModConfig config = helper.ReadConfig<ModConfig>();
            helper.Content.AssetEditors.Add(new IconEditor(helper));

            this.THRESHOLD_1 = config.THRESHOLD_1;
            this.THRESHOLD_2 = config.THRESHOLD_2;
            this.THRESHOLD_3 = config.THRESHOLD_3;
            this.GIFT_CHANCE_1 = config.GIFT_CHANCE_1;
            this.GIFT_CHANCE_2 = config.GIFT_CHANCE_2;
            this.GIFT_CHANCE_3 = config.GIFT_CHANCE_3;
            this.FARMHAND_CHANCE = config.FARMHAND_CHANCE;
            this.LOW_CHANCE = config.LOW_CHANCE;
            this.MID_CHANCE = config.MID_CHANCE;
            this.HI_CHANCE = config.HI_CHANCE;
            this.MAX_WEEKLY_GIFTS = config.MAX_WEEKLY_GIFTS;

            // Safety checks
            if (this.THRESHOLD_1 > this.THRESHOLD_2 || this.THRESHOLD_1 > this.THRESHOLD_3 || this.THRESHOLD_2 > this.THRESHOLD_3 || this.THRESHOLD_1 < 0 || this.THRESHOLD_1 > 1000 ||
                this.THRESHOLD_2 < 0 || this.THRESHOLD_2 > 1000 || this.THRESHOLD_3 < 0 || this.THRESHOLD_3 > 1000)
            {
                this.THRESHOLD_1 = 300;
                this.THRESHOLD_2 = 1000;
                this.THRESHOLD_3 = 1000;
            }
            if (this.GIFT_CHANCE_1 < 0 || this.GIFT_CHANCE_1 > 100)
                this.GIFT_CHANCE_1 = 0;
            if (this.GIFT_CHANCE_2 < 0 || this.GIFT_CHANCE_2 > 100)
                this.GIFT_CHANCE_2 = 20;
            if (this.GIFT_CHANCE_3 < 0 || this.GIFT_CHANCE_3 > 100)
                this.GIFT_CHANCE_3 = 34;
            if (this.LOW_CHANCE < 0 || this.LOW_CHANCE > 100)
                this.LOW_CHANCE = 40;
            if (this.MID_CHANCE < 0 || this.MID_CHANCE > 100)
                this.MID_CHANCE = 40;
            if (this.HI_CHANCE < 0 || this.HI_CHANCE > 100)
                this.HI_CHANCE = 20;
            if (LOW_CHANCE + MID_CHANCE + HI_CHANCE != 100)
            {
                LOW_CHANCE = 40;
                MID_CHANCE = 40;
                HI_CHANCE = 20;
            }
            if (this.MAX_WEEKLY_GIFTS < 0 || this.MAX_WEEKLY_GIFTS > 7)
                this.MAX_WEEKLY_GIFTS = 3;
            if (this.FARMHAND_CHANCE < 0 || this.FARMHAND_CHANCE > 100)
                this.FARMHAND_CHANCE = 30;

            // Initialize gift lists
            lowGifts = new List<int>();
            midGifts = new List<int>();
            hiGifts = new List<int>();

            for (int i = 0; i < 803; i++)
            {
                switch (i)
                {
                    case 152:
                    case 153:
                    case 167:
                    case 168:
                    case 169:
                    case 171:
                    case 172:                                        
                    case 344:
                    case 372:
                    case 382:
                    case 388:
                    case 390:
                    case 404:
                    case 684:
                    case 685:
                    case 702:
                    case 718:
                    case 719:
                    case 721:
                    case 722:
                    case 766:
                    case 767:
                        lowGifts.Add(i);
                        break;

                    case 296: // Spring
                    case 399: // Spring
                        if (Game1.currentSeason.Equals("Spring"))
                            lowGifts.Add(i);
                        break;

                    case 396: // Summer
                    case 406: // Summer
                        if (Game1.currentSeason.Equals("Summer"))
                            lowGifts.Add(i);
                        break;

                    case 408: // Fall
                    case 410: // Fall
                        if (Game1.currentSeason.Equals("Fall"))
                            lowGifts.Add(i);
                        break;

                    case 412: // Winter
                    case 416: // Winter
                        if (Game1.currentSeason.Equals("Winter"))
                            lowGifts.Add(i);
                        break;

                    case 78:
                    case 129:
                    case 131:
                    case 132:
                    case 142:
                    case 144:
                    case 145:
                    case 157:
                    case 174:
                    case 176:
                    case 180:
                    case 182:
                    case 340:
                    case 393:
                    case 420:
                    case 421:
                    case 422:
                    case 440:
                    case 442:
                    case 444:
                    case 535:
                    case 634:
                    case 635:
                    case 636:
                    case 637:
                    case 638:
                    case 716:
                    case 717:
                    case 720:
                    case 723:
                    case 774:
                        midGifts.Add(i);
                        break;

                    case 16: // Spring
                    case 18: // Spring
                    case 20: // Spring
                    case 22: // Spring
                    case 427: // Spring
                    case 429: // Spring
                    case 472: // Spring
                    case 473: // Spring
                    case 474: // Spring
                    case 475: // Spring
                    case 476: // Spring
                    case 477: // Spring
                    case 478: // Spring
                    case 495: // Spring
                        if (Game1.currentSeason.Equals("Spring"))
                            midGifts.Add(i);
                        break;

                    case 453: // Summer
                    case 455: // Summer
                    case 302: // Summer                    
                    case 479: // Summer
                    case 480: // Summer
                    case 481: // Summer
                    case 482: // Summer
                    case 483: // Summer
                    case 484: // Summer
                    case 485: // Summer
                    case 496: // Summer
                        if (Game1.currentSeason.Equals("Summer"))
                            midGifts.Add(i);
                        break;

                    case 299: // Fall
                    case 301: // Fall
                    case 488: // Fall
                    case 489: // Fall
                    case 490: // Fall
                    case 491: // Fall
                    case 492: // Fall
                    case 493: // Fall
                    case 494: // Fall                    
                    case 497: // Fall
                        if (Game1.currentSeason.Equals("Fall"))
                            midGifts.Add(i);
                        break;

                    case 431: // Summer/Fall
                    case 487: // Summer/Fall  
                        if (Game1.currentSeason.Equals("Summer") || Game1.currentSeason.Equals("Fall"))
                            midGifts.Add(i);
                        break;

                    case 414: // Winter
                    case 498: // Winter
                        if (Game1.currentSeason.Equals("Winter"))
                            midGifts.Add(i);
                        break;

                    case 60:
                    case 62:
                    case 64:
                    case 66:
                    case 68:
                    case 70:
                    case 72:
                    case 80:
                    case 86:
                    case 103:
                    case 198:
                    case 202:
                    case 209:
                    case 212:
                    case 213:
                    case 219:
                    case 225:
                    case 226:
                    case 227:
                    case 228:
                    case 242:
                    case 305:
                    case 446:
                        hiGifts.Add(i);
                        break;

                    case 433: // Spring / Summer
                        if (Game1.currentSeason.Equals("Spring") || Game1.currentSeason.Equals("Summer"))
                            midGifts.Add(i);
                        break;

                    case 425: // Fall
                        if (Game1.currentSeason.Equals("Fall"))
                            midGifts.Add(i);
                        break;
                }
            }

            Helper.Events.GameLoop.DayStarted += this.AfterDayStarted;
            Helper.Events.Player.Warped += this.Warped;
            Helper.Events.GameLoop.TimeChanged += TimeChanged;            
        }

        public void TimeChanged(object sender, EventArgs e)
        {
            // Replace achievement star back to vanilla texture
            if(Game1.hudMessages.Count == 0 && msgDisplayed) { 
                msgDisplayed = false;
                Helper.Content.InvalidateCache(@"LooseSprites\Cursors.xnb");
            }
        }

        // If the cat gave a gift, warp him next to it the first time the player enters the farm        
        public void Warped(object sender, EventArgs e)
        {
            if (Game1.currentLocation is Farm && giftToday && !warpedToday)
            {
                foreach (NPC pet in Game1.getLocationFromName("Farm").getCharacters())
                {
                    if (pet is StardewValley.Characters.Cat || pet is StardewValley.Characters.Dog)
                    {
                        //this.Monitor.Log("Found pet for warping.");
                        thePet = (StardewValley.Characters.Pet)pet;
                    }
                }

                if (thePet != null)
                {
                    if (thePet is StardewValley.Characters.Cat) {

                        // Have a chance of Dusty digging up something instead of the cat
                        if (Game1.currentLocation.characters.Contains(Game1.getCharacterFromName("Dusty", true)) && Game1.random.Next(1, 10) > 8)
                            this.DogSpawn(null, Game1.getCharacterFromName("Dusty", true));
                        else {
                            int x = (int)Game1.player.Position.X / 64;
                            int y = (int)Game1.player.Position.Y / 64;

                            // Spawn gift                                                

                            // Remove old gift if there's still one on the floor
                            OverlaidDictionary obs = Game1.getLocationFromName("Farm").Objects;
                            Vector2 spawnPos = new Vector2(x, y + 1);

                            for(int i=0; i<obs.Count(); i++)
                            {
                                Vector2 currentPos = obs.Keys.ElementAt(i);

                                if (currentPos == spawnPos)                                    
                                    obs.Remove(obs.Keys.ElementAt(i));                                
                            }

                            Game1.getLocationFromName("Farm").dropObject(new StardewValley.Object(giftId, 1, false, -1, 0), spawnPos * 64f, Game1.viewport, true, (Farmer)null);                        

                            // Warp cat
                            // Check if field is free
                            Vector2 warpPos = new Vector2(x + 1, y + 2);
                            Vector2 safePos = new Vector2(x + 1, y + 2);

                            // If field is free, warp cat there
                            if (Game1.getLocationFromName("Farm").isTileLocationTotallyClearAndPlaceableIgnoreFloors(warpPos)) {
                                //this.Monitor.Log(warpPos.X + "/" + warpPos.Y + " is free, warping cat there");
                                thePet.Position = warpPos * 64f;
                            }
                            else
                            {
                                // Otherwise, find a nearby free location. If we find one, warp the cat there. Otherwise, just don't warp.
                                safePos = this.FindSafePosition(warpPos);
                                if (safePos != warpPos)
                                    thePet.Position = safePos * 64f;
                            }

                           // this.Monitor.Log("Warped him.");
                            warpedToday = true;

                            Game1.playSound("cat");
                        }
                    }

                    if (thePet is StardewValley.Characters.Dog)
                    {
                        // Have a chance of Dusty digging something up instead of the dog
                        if (Game1.currentLocation.characters.Contains(Game1.getCharacterFromName("Dusty", true)) && Game1.random.Next(1, 10) > 8)
                            this.DogSpawn(null, Game1.getCharacterFromName("Dusty", true));
                        else
                            this.DogSpawn(thePet, null);
                    }

                    String dog = "";

                    if (thePet is StardewValley.Characters.Dog) { 
                        dog = " Search your farm carefully to find it!";
                        isCat = false;
                    }

                    msgDisplayed = true;                    
                    Helper.Content.InvalidateCache(@"LooseSprites\Cursors.xnb");                    
                    HUDMessage msg = new HUDMessage(thePet.Name + " brought you a gift." + dog, 1);
                    Game1.addHUDMessage(msg);
                }
                //else
                    //this.Monitor.Log("Didn't find the pet.");
            }
        }

        // Check if the player gets a gift
        public void AfterDayStarted(object sender, EventArgs e)
        {
            bool hasCat = false;
            bool hasDog = false;
            StardewValley.Characters.Pet theCat = null;
            StardewValley.Characters.Pet theDog = null;
            warpedToday = false;
            GameLocation theFarm = Game1.getLocationFromName("Farm");

            tile = theFarm.getRandomTile();
            // Find a free tile to generate dirt                
            while (!theFarm.isTileLocationTotallyClearAndPlaceable(tile))
            {
                //this.Monitor.Log("Searching for a clear tile...");
                tile = theFarm.getRandomTile();
                //this.Monitor.Log("Checking tile " + tile.X + "/" + tile.Y + " ...");
            }

            // Look for a cat or a dog
            foreach (NPC pet in Game1.getLocationFromName("Farm").getCharacters())
            {
                if (pet is StardewValley.Characters.Cat)
                {
                    //this.Monitor.Log("Player has a cat (on farm).");
                    hasCat = true;
                    theCat = (StardewValley.Characters.Pet)pet;
                }
                if (pet is StardewValley.Characters.Dog)
                {
                    //this.Monitor.Log("Player has a dog (on farm).");
                    hasDog = true;
                    theDog = (StardewValley.Characters.Pet)pet;
                }
            }
            foreach (NPC pet in Game1.getLocationFromName("FarmHouse").getCharacters())
            {
                if (pet is StardewValley.Characters.Cat)
                {
                    //this.Monitor.Log("Player has a cat (in house).");
                    hasCat = true;
                    theCat = (StardewValley.Characters.Pet)pet;
                }
                if (pet is StardewValley.Characters.Dog)
                {
                    //this.Monitor.Log("Player has a dog (in house).");
                    hasDog = true;
                    theDog = (StardewValley.Characters.Pet)pet;
                }
            }

            if (hasCat)
            {
              
                // Determine gift chance
                int giftChance = 0;

                if (theCat.friendshipTowardFarmer.Value < THRESHOLD_1)
                    giftChance = GIFT_CHANCE_1;
                else if (theCat.friendshipTowardFarmer.Value >= THRESHOLD_1 && theCat.friendshipTowardFarmer.Value < THRESHOLD_2)
                    giftChance = GIFT_CHANCE_2;
                else if (theCat.friendshipTowardFarmer.Value >= THRESHOLD_2 && theCat.friendshipTowardFarmer.Value <= THRESHOLD_3)
                    giftChance = GIFT_CHANCE_3;

                // if the player is a farmhand, relationship with the cat is broken. Use predetermined chance for farmhands
                if (!Game1.player.IsMainPlayer) {
                    //this.Monitor.Log("Player is a farmhand.");
                    giftChance = FARMHAND_CHANCE;
                }

                // Reset gifts given counter, max gifts per week -> 3
                if (Game1.dayOfMonth % 7 == 0)
                    giftsGiven = 0;

                giftToday = false;

                giftId = 0;

                //this.Monitor.Log("Did the cat give a gift?\nFriendship: " + catFriendship + "\nGift chance: " + giftChance + "\nGifts received this week: " + giftsGiven);

                // Draw a random gift ID. For clarity we do this in multiple steps
                // Step 1: Determine if cat gives a gift at all                                
                int random = Game1.random.Next(0, 100);

                //this.Monitor.Log("Random number: " + random + " --- must be larger than " + (100 - giftChance));

                if (random > (100 - giftChance) && !Game1.isRaining && giftsGiven <= MAX_WEEKLY_GIFTS)
                {
                    //this.Monitor.Log("Cat will give a gift ... maybe :3 (may still not happen if this is the second consecutive high tier gift)");

                    // Step 2: Determine quality: low = mid > high -> 40% / 40% / 20%
                    int rand = Game1.random.Next(0, 100);

                    // Pick a random item
                    if (rand <= LOW_CHANCE)
                    {
                        //this.Monitor.Log("Low quality");
                        giftId = lowGifts.ElementAt(Game1.random.Next(lowGifts.Count - 1));
                        highTierYesterday = false;
                    }
                    else if (rand > LOW_CHANCE && rand <= (LOW_CHANCE + MID_CHANCE))
                    {
                        //this.Monitor.Log("Medium quality");
                        giftId = midGifts.ElementAt(Game1.random.Next(midGifts.Count - 1));
                        highTierYesterday = false;
                    }
                    else if (rand > (100 - HI_CHANCE) && !highTierYesterday)
                    {
                        //this.Monitor.Log("High quality! :3");
                        giftId = hiGifts.ElementAt(Game1.random.Next(hiGifts.Count - 1));
                        highTierYesterday = true;
                    }

                    giftsGiven++;
                    giftToday = true;
                }
                //else
                    //this.Monitor.Log("No gift for you. You come back 10 years.");
            }
            else if (hasDog)
            {
                //this.Monitor.Log("Found a clear tile at " + tile.X + "/" + tile.Y);

                // Spawn gift at its location

                // Determine gift chance
                int giftChance = 0;

                if (theDog.friendshipTowardFarmer.Value < THRESHOLD_1)
                    giftChance = GIFT_CHANCE_1;
                else if (theDog.friendshipTowardFarmer.Value >= THRESHOLD_1 && theDog.friendshipTowardFarmer.Value < THRESHOLD_2)
                    giftChance = GIFT_CHANCE_2;
                else if (theDog.friendshipTowardFarmer.Value >= THRESHOLD_2 && theDog.friendshipTowardFarmer.Value <= THRESHOLD_3)
                    giftChance = GIFT_CHANCE_3;

                // if the player is a farmhand, relationship with the dog is broken. Use predetermined chance for farmhands
                if (!Game1.player.IsMainPlayer)
                {
                    //this.Monitor.Log("Player is a farmhand.");
                    giftChance = FARMHAND_CHANCE;
                }

                // Reset gifts given counter, max gifts per week -> 3
                if (Game1.dayOfMonth % 7 == 0)
                    giftsGiven = 0;

                giftToday = false;

                giftId = 0;

                //this.Monitor.Log("Did the dog give a gift?\nFriendship: " + dogFriendship + "\nGift chance: " + giftChance + "\nGifts received this week: " + giftsGiven);

                // Draw a random gift ID. For clarity we do this in multiple steps
                // Step 1: Determine if cat gives a gift at all                                
                int random = Game1.random.Next(0, 100);

                //this.Monitor.Log("Random number: " + random + " --- must be larger than " + (100 - giftChance));

                if (random > (100 - giftChance) && !Game1.isRaining && giftsGiven <= MAX_WEEKLY_GIFTS)
                {
                    //this.Monitor.Log("Dog will give a gift ... maybe :3 (may still not happen if this is the second consecutive high tier gift)");

                    // Step 2: Determine quality: low = mid > high -> 40% / 40% / 20%
                    int rand = Game1.random.Next(0, 100);

                    // Pick a random item
                    if (rand <= LOW_CHANCE)
                    {
                        //this.Monitor.Log("Low quality");
                        giftId = lowGifts.ElementAt(Game1.random.Next(lowGifts.Count - 1));
                        highTierYesterday = false;
                    }
                    else if (rand > LOW_CHANCE && rand <= (LOW_CHANCE + MID_CHANCE))
                    {
                        //this.Monitor.Log("Medium quality");
                        giftId = midGifts.ElementAt(Game1.random.Next(midGifts.Count - 1));
                        highTierYesterday = false;
                    }
                    else if (rand > (100 - HI_CHANCE) && !highTierYesterday)
                    {
                        //this.Monitor.Log("High quality! :3");
                        giftId = hiGifts.ElementAt(Game1.random.Next(hiGifts.Count - 1));
                        highTierYesterday = true;
                    }

                    giftsGiven++;
                    giftToday = true;
                }
                //else
                    //this.Monitor.Log("No gift for you. You come back 10 years.");
            }
            //else
                //this.Monitor.Log("Player doesn't have a pet.");

        }

        private Vector2 FindSafePosition(Vector2 pos)
        {
            // Find a clear location around the specified position
            GameLocation theFarm = Game1.getLocationFromName("Farm");

            // Check the 5 surrounding circles
            for (int i = 1; i <= 5; i++)
            {
                // Check above
                if (theFarm.isTileLocationTotallyClearAndPlaceable((int)pos.X, (int)pos.Y - i))// (new xTile.Dimensions.Location((int)pos.X, (int)pos.Y - i), Game1.viewport))
                    return new Vector2(pos.X, pos.Y - i);

                // Check below
                if (theFarm.isTileLocationTotallyClearAndPlaceable((int)pos.X, (int)pos.Y + i)) //(new xTile.Dimensions.Location((int)pos.X, (int)pos.Y + i), Game1.viewport))
                    return new Vector2(pos.X, pos.Y + i);

                // Check left
                if (theFarm.isTileLocationTotallyClearAndPlaceable((int)pos.X - i, (int)pos.Y)) //(new xTile.Dimensions.Location((int)pos.X - i, (int)pos.Y), Game1.viewport))
                    return new Vector2(pos.X - i, pos.Y);

                // Check right
                if (theFarm.isTileLocationTotallyClearAndPlaceable((int)pos.X + i, (int)pos.Y)) //(new xTile.Dimensions.Location((int)pos.X + i, (int)pos.Y), Game1.viewport))
                    return new Vector2(pos.X + i, pos.Y);

                // Check top right
                if (theFarm.isTileLocationTotallyClearAndPlaceable((int)pos.X + i, (int)pos.Y - i)) //(new xTile.Dimensions.Location((int)pos.X + i, (int)pos.Y - i), Game1.viewport))
                    return new Vector2(pos.X + i, pos.Y - i);

                // Check top left
                if (theFarm.isTileLocationTotallyClearAndPlaceable((int)pos.X - i, (int)pos.Y - i)) //(new xTile.Dimensions.Location((int)pos.X - i, (int)pos.Y - i), Game1.viewport))
                    return new Vector2(pos.X - i, pos.Y - i);

                // Check below right
                if (theFarm.isTileLocationTotallyClearAndPlaceable((int)pos.X + i, (int)pos.Y + i)) //(new xTile.Dimensions.Location((int)pos.X + i, (int)pos.Y + i), Game1.viewport))
                    return new Vector2(pos.X + i, pos.Y + i);

                // Check below left
                if (theFarm.isTileLocationTotallyClearAndPlaceable((int)pos.X - i, (int)pos.Y + i)) //(new xTile.Dimensions.Location((int)pos.X - i, (int)pos.Y + i), Game1.viewport))
                    return new Vector2(pos.X - i, pos.Y + i);
            }
            //this.Monitor.Log("Didn't find a safe position near the treasure.");
            return pos;
        }

        private void DogSpawn(StardewValley.Characters.Pet thePet = null, NPC theNPC = null)
        {
            // Spawn gift            

            // Remove old gift if there's still one on the floor
            OverlaidDictionary obs = Game1.getLocationFromName("Farm").Objects;
            Vector2 spawnPos = new Vector2(tile.X, tile.Y);

            for (int i = 0; i < obs.Count(); i++)
            {
                Vector2 currentPos = obs.Keys.ElementAt(i);

                if (currentPos == spawnPos)
                    obs.Remove(obs.Keys.ElementAt(i));
            }

            Game1.getLocationFromName("Farm").dropObject(new StardewValley.Object(giftId, 1, false, -1, 0), spawnPos * 64f, Game1.viewport, true, (Farmer)null);
            //this.Monitor.Log("Object dropped!");

            // Convert drop location to dirt (if we would do this beforehand, spawn would be impeded)                        
            Game1.getLocationFromName("Farm").terrainFeatures[tile] = new HoeDirt();

            // Warp dog                        
            Vector2 warpPos = this.FindSafePosition(tile);

            // If we find a safe location near the treasure, warp the pet
            if (warpPos != tile)
            {
                if (theNPC == null)
                    thePet.Position = warpPos * 64f;
                else if (thePet == null)
                    theNPC.Position = warpPos * 64f;             
            }

            this.Monitor.Log("Warped him ... most likely, to "+warpPos.X+"/"+warpPos.Y);
            warpedToday = true;

            Game1.playSound("dog_bark");
        }
    }
}
