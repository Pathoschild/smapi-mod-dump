using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleBedLocations
{

    class Question
    {
        public static string lastQuestionKey;

        public static void createQuestionDialogue(string question, Response[] answerChoices, string dialogKey)
        {
            lastQuestionKey = dialogKey;
            drawObjectQuestionDialogue(question, answerChoices.ToList<Response>());
        }

        public static void drawObjectQuestionDialogue(string dialogue, List<Response> choices)
        {
            Game1.activeClickableMenu = new ModDialogueBox(dialogue, choices);
            Game1.dialogueUp = true;
            Game1.player.CanMove = false;
        }


        public static bool answerDialogueWrapper(GameLocation loc, Response r)
        {

            answerDialogue(r);
            return true;
        }

        internal static void createQuestionDialogue(string question, Response[] answerChoices, Func<object, object, object> dialogKey)
        {
            lastQuestionKey = dialogKey.ToString();
            drawObjectQuestionDialogue(question, answerChoices.ToList<Response>());
        }



        /*
public virtual bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
{
   if (questionAndAnswer == null)
   {
       return false;
   }
   if (questionAndAnswer.Equals("Mine_Return to level " + Game1.player.deepestMineLevel))
   {
       Game1.mine.enterMine(Game1.player, Game1.player.deepestMineLevel, false);
   }

   if (!(questionAndAnswer == "ExitMine_Yes"))
   {
       return false;
   }


   if (!(questionAndAnswer == "Mine_Enter"))
   {
       return false;
   }
   Game1.enterMine(false, 1, null);




   if (!(questionAndAnswer == "divorce_Yes"))
   {
       return false;
   }
   if (Game1.player.Money >= 50000)
   {
       Game1.player.Money -= 50000;
       Game1.player.divorceTonight = true;
       Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_DivorceBook_Filed", new object[0]));
       return true;
   }
   Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1", new object[0]));

   if (!(questionAndAnswer == "Eat_No"))
   {
       return false;
   }
   Game1.isEating = false;
   Game1.player.completelyStopAnimatingOrDoingAction();




   if (!(questionAndAnswer == "Quest_Yes"))
   {
       return false;
   }
   Game1.questOfTheDay.dailyQuest = true;
   Game1.questOfTheDay.accept();
   Game1.currentBillboard = 0;
   Game1.player.questLog.Add(Game1.questOfTheDay);
   return true;


   if (!(questionAndAnswer == "taxvote_Against"))
   {
       return false;
   }
   Game1.addMailForTomorrow("taxRejected", false, false);
   this.currentEvent.currentCommand++;
   return true;


   if (!(questionAndAnswer == "Marnie_Supplies"))
   {
       return false;
   }
   Game1.activeClickableMenu = new ShopMenu(Utility.getAnimalShopStock(), 0, "Marnie");


   if (!(questionAndAnswer == "ClearHouse_Yes"))
   {
       return false;
   }
   using (List<Vector2>.Enumerator enumerator = Game1.player.getAdjacentTiles().GetEnumerator())
   {
       while (enumerator.MoveNext())
       {
           Vector2 current = enumerator.Current;
           if (this.objects.ContainsKey(current))
           {
               this.objects.Remove(current);
           }
       }
       return true;
   }


   if (!(questionAndAnswer == "Shipping_Yes"))
   {
       return false;
   }
   Game1.player.shipAll();



   if (!(questionAndAnswer == "BuyQiCoins_Yes"))
   {
       return false;
   }

   if (Game1.player.Money >= 1000)
   {
       Game1.player.Money -= 1000;
       Game1.playSound("Pickup_Coin15");
       Game1.player.clubCoins += 100;
       return true;
   }
   Game1.drawObjectDialogue("Error: Not enough money.");



   if (!(questionAndAnswer == "Dungeon_Go"))
   {
       return false;
   }
   Game1.enterMine(false, Game1.mine.mineLevel + 1, "Dungeon");



   if (!(questionAndAnswer == "taxvote_For"))
   {
       return false;
   }
   Game1.shippingTax = true;
   Game1.addMailForTomorrow("taxPassed", false, false);
   this.currentEvent.currentCommand++;
   return true;


   if (!(questionAndAnswer == "CalicoJackHS_Play"))
   {
       return false;
   }
   if (Game1.player.clubCoins >= 1000)
   {
       Game1.currentMinigame = new CalicoJack(-1, true);
       return true;
   }
   Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Club_CalicoJackHS_NotEnoughCoins", new object[0]));


   if (!(questionAndAnswer == "Blacksmith_Upgrade"))
   {
       return false;
   }
   Game1.activeClickableMenu = new ShopMenu(Utility.getBlacksmithUpgradeStock(Game1.player), 0, "ClintUpgrade");
   return true;


   if (!(questionAndAnswer == "ExitMine_Go"))
   {
       return false;
   }
   Game1.enterMine(false, Game1.mine.mineLevel - 1, null);

   if (!(questionAndAnswer == "carpenter_Shop"))
   {
       return false;
   }
   Game1.player.forceCanMove();
   Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");


   if (!(questionAndAnswer == "evilShrineRightDeActivate_Yes"))
   {
       return false;
   }
   if (Game1.player.removeItemsFromInventory(203, 1))
   {
       this.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2((float)(12 * Game1.tileSize + 3 * Game1.pixelZoom), (float)(6 * Game1.tileSize + Game1.pixelZoom)), false, 0f, Color.White)
       {
           interval = 50f,
           totalNumberOfLoops = 99999,
           animationLength = 7,
           layerDepth = (float)(6 * Game1.tileSize) / 10000f + 0.0001f,
           scale = (float)Game1.pixelZoom
       });
       Game1.soundBank.PlayCue("fireball");
       for (int i = 0; i < 20; i++)
       {
           this.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(12f, 6f) * (float)Game1.tileSize + new Vector2((float)Game1.random.Next(-Game1.tileSize / 2, Game1.tileSize), (float)Game1.random.Next(Game1.tileSize / 4)), false, 0.002f, Color.DarkSlateBlue)
           {
               alpha = 0.75f,
               motion = new Vector2(0f, -0.5f),
               acceleration = new Vector2(-0.002f, 0f),
               interval = 99999f,
               layerDepth = (float)(6 * Game1.tileSize) / 10000f + (float)Game1.random.Next(100) / 10000f,
               scale = (float)(Game1.pixelZoom * 3) / 4f,
               scaleChange = 0.01f,
               rotationChange = (float)Game1.random.Next(-5, 6) * 3.14159274f / 256f,
               delayBeforeAnimationStart = i * 25
           });
       }
       Game1.spawnMonstersAtNight = false;
       return true;
   }
   Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_NoOffering", new object[0]));





   if (!(questionAndAnswer == "ClubSeller_I'll"))
   {
       return false;
   }
   if (Game1.player.Money >= 1000000)
   {
       Game1.player.Money -= 1000000;
       Game1.exitActiveMenu();
       Game1.player.forceCanMove();
       Game1.player.addItemByMenuIfNecessaryElseHoldUp(new Object(Vector2.Zero, 127, false), null);
       return true;
   }
   Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_NotEnoughMoney", new object[0]));



   if (!(questionAndAnswer == "ClubCard_That's"))
   {
       return false;
   }


   if (!(questionAndAnswer == "ExitToTitle_Yes"))
   {
       return false;
   }




   if (!(questionAndAnswer == "evilShrineCenter_Yes"))
   {
       return false;
   }
   if (Game1.player.Money >= 30000)
   {
       Game1.player.Money -= 30000;
       Game1.player.wipeExMemories();
       this.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2((float)(7 * Game1.tileSize + 5 * Game1.pixelZoom), (float)(5 * Game1.tileSize + 2 * Game1.pixelZoom)), false, 0f, Color.White)
       {
           interval = 50f,
           totalNumberOfLoops = 99999,
           animationLength = 7,
           layerDepth = (float)(6 * Game1.tileSize) / 10000f + 0.0001f,
           scale = (float)Game1.pixelZoom
       });
       Game1.soundBank.PlayCue("fireball");
       DelayedAction.playSoundAfterDelay("debuffHit", 500);
       int num2 = 0;
       Game1.player.faceDirection(2);
       Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[]
       {
                               new FarmerSprite.AnimationFrame(94, 1500),
                               new FarmerSprite.AnimationFrame(0, 1)
       });
       Game1.player.freezePause = 1500;
       Game1.player.jitterStrength = 1f;
       for (int j = 0; j < 20; j++)
       {
           this.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(7f, 5f) * (float)Game1.tileSize + new Vector2((float)Game1.random.Next(-Game1.tileSize / 2, Game1.tileSize), (float)Game1.random.Next(Game1.tileSize / 4)), false, 0.002f, Color.SlateGray)
           {
               alpha = 0.75f,
               motion = new Vector2(0f, -0.5f),
               acceleration = new Vector2(-0.002f, 0f),
               interval = 99999f,
               layerDepth = (float)(5 * Game1.tileSize) / 10000f + (float)Game1.random.Next(100) / 10000f,
               scale = (float)(Game1.pixelZoom * 3) / 4f,
               scaleChange = 0.01f,
               rotationChange = (float)Game1.random.Next(-5, 6) * 3.14159274f / 256f,
               delayBeforeAnimationStart = j * 25
           });
       }
       for (int k = 0; k < 16; k++)
       {
           foreach (Vector2 current2 in Utility.getBorderOfThisRectangle(Utility.getRectangleCenteredAt(new Vector2(7f, 5f), 2 + k * 2)))
           {
               if (num2 % 2 == 0)
               {
                   this.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(692, 1853, 4, 4), 25f, 1, 16, current2 * (float)Game1.tileSize + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), false, false)
                   {
                       layerDepth = 1f,
                       delayBeforeAnimationStart = k * 50,
                       scale = (float)Game1.pixelZoom,
                       scaleChange = 1f,
                       color = new Color((int)(255 - Utility.getRedToGreenLerpColor(1f / (float)(k + 1)).R), (int)(255 - Utility.getRedToGreenLerpColor(1f / (float)(k + 1)).G), (int)(255 - Utility.getRedToGreenLerpColor(1f / (float)(k + 1)).B)),
                       acceleration = new Vector2(-0.1f, 0f)
                   });
               }
               num2++;
           }
       }
       return true;
   }
   Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_NoOffering", new object[0]));

   if (!(questionAndAnswer == "Mine_No"))
   {
       return false;
   }
   Response[] answerChoices = new Response[]
   {
                           new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No", new object[0])),
                           new Response("Yes", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes", new object[0]))
   };
   this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Mines_ResetMine", new object[0])), answerChoices, "ResetMine");

   if (!(questionAndAnswer == "MinecartGame_Progress"))
   {
       return false;
   }
   Game1.currentMinigame = new MineCart(5, 3);


   Game1.playSound("explosion");
   Game1.flashAlpha = 5f;
   this.characters.Remove(this.getCharacterFromName("Bouncer"));
   if (this.characters.Count > 0)
   {
       this.characters[0].faceDirection(1);
       this.characters[0].setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Sandy_PlayerClubMember", new object[0]), false, false);
       this.characters[0].doEmote(16, true);
   }
   Game1.pauseThenMessage(500, Game1.content.LoadString("Strings\\Locations:Club_Bouncer_PlayerClubMember", new object[0]), false);
   Game1.player.Halt();
   Game1.getCharacterFromName("Mister Qi").setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:MisterQi_PlayerClubMember", new object[0]), false, false);


   if (!(questionAndAnswer == "Smelt_Iron"))
   {
       return false;
   }
   Game1.player.IronPieces -= 10;
   //			this.smeltBar(new Object(Vector2.Zero, 335, "Iron Bar", false, true, false, false), 120);
   //			return true;


   if (!(questionAndAnswer == "mariner_Buy"))
   {
       return false;
   }
   if (Game1.player.Money < 5000)
   {
       Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1", new object[0]));
       return true;
   }
   Game1.player.Money -= 5000;
   Game1.player.addItemByMenuIfNecessary(new Object(460, 1, false, -1, 0)
   {
       specialItem = true
   }, null);
   if (Game1.activeClickableMenu == null)
   {
       Game1.player.holdUpItemThenMessage(new Object(460, 1, false, -1, 0), true);
       return true;
   }
   return true;



   if (!(questionAndAnswer == "Minecart_Bus"))
   {
       return false;
   }
   Game1.player.Halt();
   Game1.player.freezePause = 700;
   Game1.warpFarmer("BusStop", 4, 4, 2);


   if (!(questionAndAnswer == "Mine_Return"))
   {
       return false;
   }
   Game1.enterMine(false, Game1.player.deepestMineLevel, null);




   if (!(questionAndAnswer == "Blacksmith_Process"))
   {
       return false;
   }
   Game1.activeClickableMenu = new GeodeMenu();


   if (!(questionAndAnswer == "Mine_Yes"))
   {
       return false;
   }
   if (Game1.mine != null && Game1.mine.mineLevel > 120)
   {
       Game1.warpFarmer("SkullCave", 3, 4, 2);
       return true;
   }
   Game1.warpFarmer("UndergroundMine", 16, 16, false);

   if (!(questionAndAnswer == "BuyClubCoins_Yes"))
   {
       return false;
   }
   if (Game1.player.Money >= 1000)
   {
       Game1.player.Money -= 1000;
       Game1.player.clubCoins += 10;
       return true;
   }
   Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1", new object[0]));


   if (!(questionAndAnswer == "Museum_Collect"))
   {
       return false;
   }
   Game1.activeClickableMenu = new ItemGrabMenu((this as LibraryMuseum).getRewardsForPlayer(Game1.player), false, true, null, null, "Rewards", new ItemGrabMenu.behaviorOnItemSelect((this as LibraryMuseum).collectedReward), false, false, false, false, false, 0, null);


   //Game1.fadeScreenToBlack();
   //	Game1.exitToTitle = true;



   if (!(questionAndAnswer == "Smelt_Copper"))
   {
       return false;
   }
   Game1.player.CopperPieces -= 10;
   //	this.smeltBar(new Object(Vector2.Zero, 334, "Copper Bar", false, true, false, false), 60);

   if (!(questionAndAnswer == "Minecart_Mines"))
   {
       return false;
   }
   Game1.player.Halt();
   Game1.player.freezePause = 700;
   Game1.warpFarmer("Mine", 13, 9, 1);

   if (!(questionAndAnswer == "ExitMine_Leave"))
   {
       return false;
   }

   if (!(questionAndAnswer == "carpenter_Upgrade"))
   {
       return false;
   }
   //this.houseUpgradeOffer();

   if (!(questionAndAnswer == "divorceCancel_Yes"))
   {
       return false;
   }
   if (Game1.player.divorceTonight)
   {
       Game1.player.divorceTonight = false;
       Game1.player.money += 50000;
       Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_DivorceBook_Cancelled", new object[0]));
       return true;
   }

   if (!(questionAndAnswer == "evilShrineLeft_Yes"))
   {
       return false;
   }
   if (Game1.player.removeItemsFromInventory(74, 1))
   {
       this.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2((float)(2 * Game1.tileSize + 7 * Game1.pixelZoom), (float)(6 * Game1.tileSize + Game1.pixelZoom)), false, 0f, Color.White)
       {
           interval = 50f,
           totalNumberOfLoops = 99999,
           animationLength = 7,
           layerDepth = (float)(6 * Game1.tileSize) / 10000f + 0.0001f,
           scale = (float)Game1.pixelZoom
       });
       for (int l = 0; l < 20; l++)
       {
           this.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(2f, 6f) * (float)Game1.tileSize + new Vector2((float)Game1.random.Next(-Game1.tileSize / 2, Game1.tileSize), (float)Game1.random.Next(Game1.tileSize / 4)), false, 0.002f, Color.LightGray)
           {
               alpha = 0.75f,
               motion = new Vector2(1f, -0.5f),
               acceleration = new Vector2(-0.002f, 0f),
               interval = 99999f,
               layerDepth = (float)(6 * Game1.tileSize) / 10000f + (float)Game1.random.Next(100) / 10000f,
               scale = (float)(Game1.pixelZoom * 3) / 4f,
               scaleChange = 0.01f,
               rotationChange = (float)Game1.random.Next(-5, 6) * 3.14159274f / 256f,
               delayBeforeAnimationStart = l * 25
           });
       }
       Game1.soundBank.PlayCue("fireball");
       this.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(2f, 5f) * (float)Game1.tileSize, false, true, 1f, 0f, Color.White, (float)Game1.pixelZoom, 0f, 0f, 0f, false)
       {
           motion = new Vector2(4f, -2f)
       });
       if (Game1.player.getChildren().Count<Child>() > 1)
       {
           this.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(2f, 5f) * (float)Game1.tileSize, false, true, 1f, 0f, Color.White, (float)Game1.pixelZoom, 0f, 0f, 0f, false)
           {
               motion = new Vector2(4f, -1.5f),
               delayBeforeAnimationStart = 50
           });
       }
       string text = "";
       foreach (NPC current3 in Game1.player.getChildren())
       {
           text = string.Concat(new string[]
           {
                               text,
                               Game1.content.LoadString("Strings\\Locations:WitchHut_Goodbye", new object[0]),
                               ", ",
                               current3.getName(),
                               ". "
           });
       }
       Game1.showGlobalMessage(text);
       Game1.player.getRidOfChildren();
       return true;
   }
   Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_NoOffering", new object[0]));

   if (!(questionAndAnswer == "diary_...I"))
   {
       return false;
   }
   Game1.player.friendships[(Game1.farmEvent as DiaryEvent).NPCname][5] = 1;

   if (!(questionAndAnswer == "Backpack_Yes"))
   {
       return false;
   }
   //	this.tryToBuyNewBackpack();

   if (!(questionAndAnswer == "Smelt_Iridium"))
   {
       return false;
   }
   Game1.player.IridiumPieces -= 10;
   //this.smeltBar(new Object(Vector2.Zero, 337, "Iridium Bar", false, true, false, false), 1440);

   if (!(questionAndAnswer == "upgrade_Yes"))
   {
       return false;
   }
   //this.houseUpgradeAccept();


   if (!(questionAndAnswer == "Backpack_Purchase"))
   {
       return false;
   }
   if (Game1.player.maxItems == 12 && Game1.player.Money >= 2000)
   {
       Game1.player.Money -= 2000;
       Game1.player.maxItems += 12;
       for (int m = 0; m < Game1.player.maxItems; m++)
       {
           if (Game1.player.items.Count <= m)
           {
               Game1.player.items.Add(null);
           }
       }
       Game1.player.holdUpItemThenMessage(new SpecialItem(-1, 99, "Large Pack"), true);
       return true;
   }
   if (Game1.player.maxItems < 36 && Game1.player.Money >= 10000)
   {
       Game1.player.Money -= 10000;
       Game1.player.maxItems += 12;
       Game1.player.holdUpItemThenMessage(new SpecialItem(-1, 99, "Deluxe Pack"), true);
       for (int n = 0; n < Game1.player.maxItems; n++)
       {
           if (Game1.player.items.Count <= n)
           {
               Game1.player.items.Add(null);
           }
       }
       return true;
   }
   if (Game1.player.maxItems != 36)
   {
       Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney2", new object[0]));
       return true;
   }

   if (!(questionAndAnswer == "Sleep_Yes"))
   {
       return false;
   }
   if (this.lightLevel == 0f && Game1.timeOfDay < 2000)
   {
       this.lightLevel = 0.6f;
       Game1.playSound("openBox");
       Game1.NewDay(600f);
   }
   else if (this.lightLevel > 0f && Game1.timeOfDay >= 2000)
   {
       this.lightLevel = 0f;
       Game1.playSound("openBox");
       Game1.NewDay(600f);
   }
   else
   {
       Game1.NewDay(0f);
   }
   Game1.player.mostRecentBed = Game1.player.position;
   Game1.player.doEmote(24);
   Game1.player.freezePause = 2000;

   if (!(questionAndAnswer == "Shaft_Jump"))
   {
       return false;
   }
   if (this is MineShaft)
   {
       (this as MineShaft).enterMineShaft();
       return true;
   }

   if (!(questionAndAnswer == "CalicoJack_Rules"))
   {
       return false;
   }
   Game1.multipleDialogues(new string[]
   {
                   Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Rules1", new object[0]),
                   Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Rules2", new object[0])
   });





   if (!(questionAndAnswer == "Minecart_Town"))
   {
       return false;
   }
   Game1.player.Halt();
   Game1.player.freezePause = 700;
   Game1.warpFarmer("Town", 105, 80, 1);

   if (!(questionAndAnswer == "Eat_Yes"))
   {
       return false;
   }
   Game1.isEating = false;
   Game1.eatHeldObject();




   if (!(questionAndAnswer == "Smelt_Gold"))
   {
       return false;
   }
   Game1.player.GoldPieces -= 10;
   //this.smeltBar(new Object(Vector2.Zero, 336, "Gold Bar", false, true, false, false), 300);



   if (!(questionAndAnswer == "Flute_Change"))
   {
       return false;
   }
   Game1.drawItemNumberSelection("flutePitch", -1);



   if (!(questionAndAnswer == "jukebox_Yes"))
   {
       return false;
   }
   Game1.drawItemNumberSelection("jukebox", -1);
   Game1.jukeboxPlaying = true;

   if (!(questionAndAnswer == "evilShrineRightActivate_Yes"))
   {
       return false;
   }
   if (Game1.player.removeItemsFromInventory(203, 1))
   {
       this.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2((float)(12 * Game1.tileSize + 3 * Game1.pixelZoom), (float)(6 * Game1.tileSize + Game1.pixelZoom)), false, 0f, Color.White)
       {
           interval = 50f,
           totalNumberOfLoops = 99999,
           animationLength = 7,
           layerDepth = (float)(6 * Game1.tileSize) / 10000f + 0.0001f,
           scale = (float)Game1.pixelZoom
       });
       Game1.soundBank.PlayCue("fireball");
       DelayedAction.playSoundAfterDelay("batScreech", 500);
       for (int num3 = 0; num3 < 20; num3++)
       {
           this.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(12f, 6f) * (float)Game1.tileSize + new Vector2((float)Game1.random.Next(-Game1.tileSize / 2, Game1.tileSize), (float)Game1.random.Next(Game1.tileSize / 4)), false, 0.002f, Color.DarkSlateBlue)
           {
               alpha = 0.75f,
               motion = new Vector2(-0.1f, -0.5f),
               acceleration = new Vector2(-0.002f, 0f),
               interval = 99999f,
               layerDepth = (float)(6 * Game1.tileSize) / 10000f + (float)Game1.random.Next(100) / 10000f,
               scale = (float)(Game1.pixelZoom * 3) / 4f,
               scaleChange = 0.01f,
               rotationChange = (float)Game1.random.Next(-5, 6) * 3.14159274f / 256f,
               delayBeforeAnimationStart = num3 * 60
           });
       }
       Game1.player.freezePause = 1501;
       for (int num4 = 0; num4 < 28; num4++)
       {
           this.TemporarySprites.Add(new TemporaryAnimatedSprite(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(540, 347, 13, 13), 50f, 4, 9999, new Vector2(12f, 5f) * (float)Game1.tileSize, false, true, 1f, 0f, Color.White, (float)Game1.pixelZoom, 0f, 0f, 0f, false)
           {
               delayBeforeAnimationStart = 500 + num4 * 25,
               motion = new Vector2((float)(Game1.random.Next(1, 5) * ((Game1.random.NextDouble() < 0.5) ? -1 : 1)), (float)(Game1.random.Next(1, 5) * ((Game1.random.NextDouble() < 0.5) ? -1 : 1)))
           });
       }
       Game1.spawnMonstersAtNight = true;
       return true;
   }
   Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_NoOffering", new object[0]));

   if (!(questionAndAnswer == "Minecart_Quarry"))
   {
       return false;
   }
   Game1.player.Halt();
   Game1.player.freezePause = 700;
   Game1.warpFarmer("Mountain", 124, 12, 2);
   return true;


   if (!(questionAndAnswer == "MinecartGame_Endless"))
   {
       return false;
   }
   Game1.currentMinigame = new MineCart(5, 2);

   if (!(questionAndAnswer == "RemoveIncubatingEgg_Yes"))
   {
       return false;
   }
   Game1.player.ActiveObject = new Object(Vector2.Zero, (this as AnimalHouse).incubatingEgg.Y, null, false, true, false, false);
   Game1.player.showCarrying();
   (this as AnimalHouse).incubatingEgg.Y = -1;
   this.map.GetLayer("Front").Tiles[1, 2].TileIndex = 45;

   if (!(questionAndAnswer == "buyJojaCola_Yes"))
   {
       return false;
   }
   if (Game1.player.Money >= 75)
   {
       Game1.player.Money -= 75;
       Game1.player.addItemByMenuIfNecessary(new Object(167, 1, false, -1, 0), null);
       return true;
   }
   Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1", new object[0]));




   if (!(questionAndAnswer == "Museum_Donate"))
   {
       return false;
   }
   Game1.activeClickableMenu = new MuseumMenu();

   if (!(questionAndAnswer == "WizardShrine_Yes"))
   {
       return false;
   }
   if (Game1.player.Money >= 500)
   {
       Game1.activeClickableMenu = new CharacterCustomization(new List<int>
                   {
                       0,
                       1,
                       2,
                       3,
                       4,
                       5
                   }, new List<int>
                   {
                       0,
                       1,
                       2,
                       3,
                       4,
                       5
                   }, new List<int>
                   {
                       0,
                       1,
                       2,
                       3,
                       4,
                       5
                   }, true);
       Game1.player.Money -= 500;
       return true;
   }
   Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney2", new object[0]));


   if (!(questionAndAnswer == "Mariner_Buy"))
   {
       return false;
   }
   if (Game1.player.Money >= 5000)
   {
       Game1.player.Money -= 5000;
       Game1.player.grabObject(new Object(Vector2.Zero, 460, null, false, true, false, false));
       return true;
   }
   Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1", new object[0]));

   if (!(questionAndAnswer == "Marnie_Purchase"))
   {
       return false;
   }
   Game1.player.forceCanMove();
   Game1.activeClickableMenu = new PurchaseAnimalsMenu(Utility.getPurchaseAnimalStock());


   if (!(questionAndAnswer == "carpenter_Construct"))
   {
       return false;
   }
   Game1.activeClickableMenu = new CarpenterMenu(false);
   return true;

   if (!(questionAndAnswer == "Quest_No"))
   {
       return false;
   }
   Game1.currentBillboard = 0;

   if (!(questionAndAnswer == "Bouquet_Yes"))
   {
       return false;
   }
   if (Game1.player.Money < 500)
   {
       Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1", new object[0]));
       return true;
   }
   if (Game1.player.ActiveObject == null)
   {
       Game1.player.Money -= 500;
       Game1.player.grabObject(new Object(Vector2.Zero, 458, null, false, true, false, false));
       return true;
   }

   if (!(questionAndAnswer == "Blacksmith_Shop"))
   {
       return false;
   }
   Game1.activeClickableMenu = new ShopMenu(Utility.getBlacksmithStock(), 0, "Clint");

   if (!(questionAndAnswer == "CalicoJack_Play"))
   {
       return false;
   }
   if (Game1.player.clubCoins >= 100)
   {
       Game1.currentMinigame = new CalicoJack(-1, false);
       return true;
   }
   Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_NotEnoughCoins", new object[0]));


   if (Game1.mine != null && Game1.mine.mineLevel > 120)
   {
       Game1.warpFarmer("SkullCave", 3, 4, 2);
   }
   else
   {
       Game1.warpFarmer("Mine", 23, 8, false);
   }
   Game1.changeMusicTrack("none");
   return true;
}
*/
        /*
        public static bool answerDialogue(Response answer)
        {
            string[] array = (lastQuestionKey != null) ? lastQuestionKey.Split(new char[]
            {
                ' '
            }) : null;
            string text = (array != null) ? (array[0] + "_" + answer.responseKey) : null;
            if (answer.responseKey.Equals("Move"))
            {
                Game1.player.grabObject(this.actionObjectForQuestionDialogue);
                this.removeObject(this.actionObjectForQuestionDialogue.TileLocation, false);
                this.actionObjectForQuestionDialogue = null;
                return true;
            }
            if (this.afterQuestion != null)
            {
                this.afterQuestion(Game1.player, answer.responseKey);
                this.afterQuestion = null;
                Game1.objectDialoguePortraitPerson = null;
                return true;
            }
            return text != null && this.answerDialogueAction(text, array);
        }
        */

        public static bool answerDialogue(Response answer)
        {
            string a =  answer.responseKey;
            Log.AsyncC("This is my response " + a);
            if (a == "Yes")
            {
                Log.AsyncC(Path.Combine(Class1.playerDataPath,Class1.fileName));
                try
                {
                    string[] fileContents = new string[8]
                    {
                            "Location Name:",
                          Game1.player.currentLocation.name,
                            "xPosition:",
                            Game1.player.getTileX().ToString(),
                            "yPosition",
                           Game1.player.getTileY().ToString(),
                           "Direction",
                           Game1.player.facingDirection.ToString()
                    };
                    File.WriteAllLines(Path.Combine(Class1.playerDataPath, Class1.fileName), fileContents);
                }
                catch (Exception e)
                {
                    Log.AsyncColour(e, ConsoleColor.DarkCyan);
                }
                Log.AsyncG(a.ToString());
                Game1.player.currentLocation.lastQuestionKey = "";
                Farmer.doSleepEmote(Game1.player);
                Game1.NewDay(600f);
                Game1.player.mostRecentBed = StardewValley.Utility.PointToVector2(Game1.player.getTileLocationPoint());
            }
            if (a == "No")
            {

            }
            if (a == "ElseWhere")
            {
                Class1.sleepElseWhere("BusStop", new Microsoft.Xna.Framework.Point(20, 20),3);
                Log.AsyncG("ELSEWHERE");
            }
            return true;
          //  return base.answerDialogue(answer);
        }
    }
}
