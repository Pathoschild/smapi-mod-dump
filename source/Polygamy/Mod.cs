using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Modworks = bwdyworks.Modworks;
using System.Linq;

namespace Polygamy
{
    public class Mod : StardewModdingAPI.Mod
    {
        internal static bool Debug = false;
        [System.Diagnostics.Conditional("DEBUG")]
        public void EntryDebug() { Debug = true; }
        internal static string Module;

        public static Relationships Relationships;

        public override void Entry(IModHelper helper)
        {
            Module = helper.ModRegistry.ModID;
            EntryDebug();
            if (!Modworks.InstallModule(Module, Debug)) return;

            Modworks.Events.NPCCheckAction += Events_NPCCheckAction;
            Modworks.Events.TileCheckAction += Events_TileCheckAction;

            Relationships = new Relationships();
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
            Relationships = new Relationships();
            helper.ConsoleCommands.Add("polygamy", "'polygamy help' for more info", PolygamyCommand);

        }

        private void Events_TileCheckAction(object sender, bwdyworks.Events.TileCheckActionEventArgs args)
        {
            if(args.Action == "DivorceBook")
            {
                args.Cancelled = true;
                int optionCount = Relationships.Spouses.Count;
                List<Response> responses = new List<Response>();

                foreach(var s in Relationships.Spouses)
                {
                    responses.Add(new Response(s, s));
                }
                if (Relationships.PrimarySpouse != null) responses.Add(new Response(Relationships.PrimarySpouse, Relationships.PrimarySpouse));
                responses.Add(new Response("cancel", "I don't want to divorce anyone."));

                Modworks.Menus.AskQuestion("Who would you like to divorce? (50,000g)", responses.ToArray(), DivorceQuestionCallback1);
            }
        }

        private void DivorceQuestionCallback1(Farmer who, string what)
        {
            if (what != "cancel")
            {
                if (Game1.player.Money > 50000)
                {
                    Game1.player.Money -= 50000;
                    Relationships.Divorce(what);
                    Game1.showGlobalMessage("You are no longer married to " + what);
                }
                else
                {
                    Game1.showRedMessage("You cannot afford this divorce.");
                }
            }
        }

        private void Events_NPCCheckAction(object sender, bwdyworks.Events.NPCCheckActionEventArgs args)
        {
            if (args.Cancelled) return; //someone else already ate this one

            //do we care about this NPC for our purposes?
            var targetedNPCs = Modworks.NPCs.GetAllCharacterNames(true, false, args.Farmer.currentLocation);
            if (!targetedNPCs.Contains(args.NPC.Name)) return;

            //let's run the tests
            Modworks.Log.Trace("Polygamy, beginning interaction with dateable NPC");
            NPC n2 = args.NPC;
            //are we holding an item
            if (Game1.player.ActiveObject != null)
            {
                //is it the bouquet?
                if (Game1.player.ActiveObject.ParentSheetIndex == 458)
                {
                    Modworks.Log.Trace("Polygamy, we are beginning a bouquet check");
                    if (!Relationships.DateableNPCs.Contains(n2))
                    {
                        Modworks.Log.Trace("Polygamy, refusing date because not dateable (or not available)");
                        //REFUSE, NOT DATEABLE
                        //either divorced, already married, already dating, or not available
                        args.Cancelled = true;
                        n2.CurrentDialogue.Push(new Dialogue((n2.Gender == 1) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3970") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3971"), n2));
                        Game1.drawDialogue(n2);
                        return;
                    }
                    else
                    {
                        if (Modworks.Player.GetFriendshipPoints(n2.Name) >= 2000) //ready for relationship!
                        {
                            Modworks.Log.Trace("Polygamy, happy to date!");
                            //LETTUCE DATE
                            args.Cancelled = true;
                            n2.faceTowardFarmerForPeriod(5000, 60, false, Game1.player);
                            Relationships.Date(n2.Name);
                            n2.CurrentDialogue.Push(new Dialogue((Game1.random.NextDouble() < 0.5) ? Game1.LoadStringByGender(n2.Gender, "Strings\\StringsFromCSFiles:NPC.cs.3962") : Game1.LoadStringByGender(n2.Gender, "Strings\\StringsFromCSFiles:NPC.cs.3963"), n2));
                            Game1.player.reduceActiveItemByOne();
                            Game1.player.completelyStopAnimatingOrDoingAction();
                            n2.doEmote(20);
                            Game1.drawDialogue(n2);
                            Relationships.ScanForNPCs();
                            return;
                        }
                        else
                        {
                            Modworks.Log.Trace("Polygamy, refusing date because friendship is too low");
                            //REFUSE, DONT KNOW YOU WELL ENOUGH
                            args.Cancelled = true;
                            n2.CurrentDialogue.Push(new Dialogue((Game1.random.NextDouble() < 0.5) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3958") : Game1.LoadStringByGender(n2.Gender, "Strings\\StringsFromCSFiles:NPC.cs.3959"), n2));
                            Game1.drawDialogue(n2);
                            return;
                        }
                    }
                }
                //or is the pendant?
                else if (Game1.player.ActiveObject.ParentSheetIndex == 460)
                {
                    Modworks.Log.Trace("Polygamy, we are beginning a pendant check");
                    if (Game1.player.HouseUpgradeLevel < 1)
                    {
                        Modworks.Log.Trace("Polygamy, rejecting proposal because house is too small");
                        //REFUSE, NOWHERE TO LIVE
                        args.Cancelled = true;
                        if (Game1.random.NextDouble() < 0.1) n2.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3972"), n2));
                        else n2.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Pickaxe.cs.14194") + "$h", n2));
                        Game1.drawDialogue(n2);
                        return;
                    }
                    else
                    {
                        if (!Relationships.MarryableNPCs.Contains(n2))
                        {
                            Modworks.Log.Trace("Polygamy, rejecting proposal because NPC is not dateable (or not available)");
                            //REFUSE, NOT MARRYABLE
                            //either divorced, already married, not dating, or not available
                            args.Cancelled = true;
                            n2.CurrentDialogue.Push(new Dialogue((n2.Gender == 1) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3970") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3971"), n2));
                            Game1.drawDialogue(n2);
                            return;
                        }
                        else
                        {
                            if (Modworks.Player.GetFriendshipPoints(n2.Name) >= 2499) //ready for marriage!
                            {
                                Modworks.Log.Trace("Polygamy, so happy to get engaged!");
                                //LETTUCE MARRY
                                args.Cancelled = true;
                                Game1.changeMusicTrack("none");
                                n2.CurrentDialogue.Clear();
                                Relationships.Engage(n2.Name);
                                Modworks.Player.SetFriendshipPoints(n2.Name, 2500);
                                Dialogue d1, d2, d3;
                                bool dialogueOk = Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue").ContainsKey(n2.Name + "0");
                                if (!dialogueOk)
                                {
                                    //we're marrying something that doesn't have accept dialogue. default dialog
                                    if (n2.Gender == 0)
                                    { //male
                                        d1 = new Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")["Sebastian" + "0"], n2);
                                        d2 = new Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")["Alex" + "1"], n2);
                                    }
                                    else
                                    { //female
                                        d1 = new Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")["Alex" + "0"], n2);
                                        d2 = new Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")["Leah" + "0"], n2);
                                    }
                                }
                                else
                                {
                                    d1 = new Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")[n2.Name + "0"], n2);
                                    d2 = new Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")[n2.Name + "1"], n2);
                                }
                                d3 = new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3980"), n2);
                                n2.CurrentDialogue.Push(d1);
                                n2.CurrentDialogue.Push(d2);
                                n2.CurrentDialogue.Push(d3);
                                Game1.player.reduceActiveItemByOne();
                                Game1.player.completelyStopAnimatingOrDoingAction();
                                Game1.drawDialogue(n2);
                                Relationships.ScanForNPCs();
                                return;
                            }
                            else
                            {
                                Modworks.Log.Trace("Polygamy, rejecting proposal because friendship is too low");
                                //REFUSE, DONT KNOW YOU WELL ENOUGH
                                args.Cancelled = true;
                                n2.CurrentDialogue.Push(new Dialogue((Game1.random.NextDouble() < 0.5) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3972") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3973"), n2));
                                Game1.drawDialogue(n2);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void PolygamyCommand(string command, string[] parameters)
        {
            if (parameters[0] == "togglespouseroom")
            {
                Relationships.PolyData.EnableSpouseRoom = !Relationships.PolyData.EnableSpouseRoom;
                Relationships.RollSpouseRoom();
                return;
            }
            if (parameters.Length < 2 || parameters[0] == "help")
            {
                var exampleNames = new[] { "Pierre", "Robin", "Sandy", "Pam", "Jodi", "Kent", "Caroline", "Clint", "Evelyn", "Gus", "Demetrius", "Lewis", "Marnie", "Wizard" };
                string exampleName = exampleNames[Modworks.RNG.Next(exampleNames.Length)];
                Monitor.Log($"Polygamy commands:\npolygamy flirt {exampleName} - would make '{exampleName}' a dateable NPC.\npolygamy unflirt {exampleName} - would make '{exampleName}' no longer dateable.\npolygamy roll {exampleName} - would make '{exampleName}' your 'official' spouse tomorrow\npolygamy marry {exampleName} - would immediately start a wedding with '{exampleName}'\npolygamy date {exampleName} - would make '{exampleName}' date you\npolygamy breakup {exampleName} - would make '{exampleName}' no longer dating you\npolygamy divorce {exampleName} - would, well, divorce '{exampleName}'\npolygamy undivorce {exampleName} - would make '{exampleName}' forget they were ever married to you\npolygamy togglespouseroom - toggles the spouse room on or off", LogLevel.Info);
                return;
            }
            NPC npc = Game1.getCharacterFromName(parameters[1]);
            if (npc == null)
            {
                Monitor.Log("Could not identify NPC: " + parameters[1], LogLevel.Alert);
                return;
            }
            if (!Context.CanPlayerMove && !(Game1.activeClickableMenu is StardewValley.Menus.SocialPage))
            {
                Monitor.Log("Polygamy commands are disabled while menus are active.", LogLevel.Alert);
                return;
            } else if (parameters[0] == "kiss")
            {
                Relationships.Kiss(npc.Name);
                return;
            } else if(Game1.getLocationFromName(Game1.player.homeLocation.Value) == Game1.currentLocation)
            {
                Monitor.Log("Polygamy commands are disabled while in the FarmHouse map.", LogLevel.Alert);
                return;
            }

            if (parameters[0] == "flirt") {
                Relationships.SetDateable(npc.Name, true);
                Monitor.Log("NPC " + parameters[1] + " is now datable.", LogLevel.Info);
                return;
            } else if (parameters[0] == "unflirt")
            {
                Relationships.SetDateable(npc.Name, false);
                Monitor.Log("NPC " + parameters[1] + " is no longer datable.", LogLevel.Info);
                return;
            }
            else if (parameters[0] == "date")
            {
                Relationships.Date(npc.Name);
                Monitor.Log("NPC " + parameters[1] + " is now dating you.", LogLevel.Info);
                return;
            }
            else if (parameters[0] == "roll")
            {
                if (Relationships.IsMarried(npc.Name))
                {
                    Relationships.TomorrowSpouse = npc.Name;
                    Monitor.Log("NPC " + parameters[1] + " will be your roll tomorrow.", LogLevel.Info);
                }
                else Monitor.Log("NPC " + parameters[1] + " is not married to you.", LogLevel.Alert);
                return;
            }  else if (parameters[0] == "marry")
            {
                if (Relationships.IsMarried(npc.Name))
                    Monitor.Log("NPC " + parameters[1] + " is already married to you.", LogLevel.Alert);
                else {
                    Relationships.Marry(npc.Name);
                    Monitor.Log("NPC " + parameters[1] + " is now married to you.", LogLevel.Info);
                    return;
                }
                return;
            } else if (parameters[0] == "breakup")
            {
                if (Relationships.IsDating(npc.Name))
                {
                    Relationships.Forget(npc.Name, false);
                    Monitor.Log("NPC " + parameters[1] + " is no longer dating you.", LogLevel.Info);
                } else Monitor.Log("NPC " + parameters[1] + " was not dating you.", LogLevel.Alert);
                return;
            } else if (parameters[0] == "divorce")
            {
                if (Relationships.IsMarried(npc.Name))
                {
                    Relationships.Divorce(npc.Name);
                    Monitor.Log("NPC " + parameters[1] + " is no longer married to you.", LogLevel.Info);
                }
                else Monitor.Log("NPC " + parameters[1] + " was not married to you.", LogLevel.Alert);
                return;
            }
            else if (parameters[0] == "undivorce")
            {
                if(Relationships.CheckStatusProper(npc.Name) == Relationships.RelationshipStatus.DIVORCED)
                {
                    Relationships.Forget(npc.Name);
                    Monitor.Log("NPC " + parameters[1] + " no longer remembers having been married to you.", LogLevel.Info);
                } else Monitor.Log("NPC " + parameters[1] + " is not divorced from you.", LogLevel.Alert);
                return;
            }
            else
            {
                Monitor.Log("Unknown command: " + parameters[0], LogLevel.Alert);
            }
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {

            Relationships.PolyData = Helper.Data.ReadJsonFile<PolyData>("Saves/polySpouse." + Constants.SaveFolderName + ".json");
            if (Relationships.PolyData == null)
            {
                Relationships.PolyData = new PolyData();
                Relationships.PolyData.EnableSpouseRoom = true;
            }
            else Monitor.Log("Polygamy loaded.");
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            Monitor.Log("Polygamy saved.");
            Helper.Data.WriteJsonFile("Saves/polySpouse." + Constants.SaveFolderName + ".json", Relationships.PolyData);
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (Relationships.IsThisPlayerMarried) { 
                //add more pendants to Pierre's
                (Game1.getLocationFromName("SeedShop") as StardewValley.Locations.SeedShop).itemsToStartSellingTomorrow.Add(new StardewValley.Object(Vector2.Zero, 460, 1));
            }

            Relationships.ScanForNPCs();

            if (Relationships.IsThisPlayerMarried)
            {
                var nextPrimarySpouse = Game1.getCharacterFromName(Relationships.GetNextPrimarySpouse());
                var lastPrimarySpouse = Game1.getCharacterFromName(Relationships.PrimarySpouse);
                Relationships.MakePrimarySpouse(nextPrimarySpouse.Name);
                Monitor.Log("Primary spouse for today: " + nextPrimarySpouse.Name);

                var farmHouseName = (Game1.getLocationFromName(Game1.player.homeLocation.Value) as StardewValley.Locations.FarmHouse).Name;

                //put the primary in the kitchen (seems this is the only way to get the primary's schedule to fire? wtf?)
                var kitchenSpot = (Game1.getLocationFromName(Game1.player.homeLocation.Value) as StardewValley.Locations.FarmHouse).getKitchenStandingSpot();
                Modworks.NPCs.Warp(nextPrimarySpouse, farmHouseName, kitchenSpot);
                FixSpouseSchedule(Game1.getLocationFromName(Game1.player.homeLocation.Value), nextPrimarySpouse);

                //if previous isn't null, stick them in bed with the player
                var bedSpot = (Game1.getLocationFromName(Game1.player.homeLocation.Value) as StardewValley.Locations.FarmHouse).getSpouseBedSpot();
                if (lastPrimarySpouse != null)
                {
                    Modworks.NPCs.Warp(lastPrimarySpouse, farmHouseName, bedSpot);
                    FixSpouseSchedule(Game1.getLocationFromName(Game1.player.homeLocation.Value), lastPrimarySpouse, true);
                }

                //random dogpile in bed
                if(Relationships.Spouses.Count > 1)
                {
                    Monitor.Log("we got spouses");
                    foreach (var spouseName in Relationships.Spouses)
                    {
                        //if (lastPrimarySpouse != null && spouseName == lastPrimarySpouse.Name) continue;
                        //random dogpile in bed
                        if (Modworks.RNG.Next(100) < Math.Max(100 - Relationships.Spouses.Count * 7, 3))
                        {
                            var npcObject = Game1.getCharacterFromName(spouseName);
                            Modworks.NPCs.Warp(npcObject, farmHouseName, (Game1.getLocationFromName(farmHouseName) as StardewValley.Locations.FarmHouse).getBedSpot());
                            //Monitor.Log("putting " + spouseName + " in bed (player side)");
                            var pos = npcObject.Position;
                            pos.X += (float)(-32f + Modworks.RNG.NextDouble() * 96f); //vary the position
                            npcObject.Position = pos;
                        } else if (Modworks.RNG.Next(100) < Math.Max(100 - Relationships.Spouses.Count * 7, 3))
                        {
                            var npcObject = Game1.getCharacterFromName(spouseName);
                            Modworks.NPCs.Warp(npcObject, farmHouseName, bedSpot);
                            //Monitor.Log("putting " + spouseName + " in bed (spouse side)");
                            var pos = npcObject.Position;
                            pos.X += (float)(-32f + Modworks.RNG.NextDouble() * 96f); //vary the position
                            npcObject.Position = pos;
                        }
                        else //or around the house at random
                        {
                            //Monitor.Log("putting " + spouseName + " around the house");
                            NPC otherSpouseNPC = Game1.getCharacterFromName(spouseName);
                            //find a free tile to position them on
                            GameLocation l = Game1.getLocationFromName(Game1.player.homeLocation.Value) as StardewValley.Locations.FarmHouse;
                            var p = Modworks.Locations.FindPathableAndClearTile(l, otherSpouseNPC.getTileLocationPoint());
                            if (p != Point.Zero)
                            {
                                Modworks.NPCs.Warp(otherSpouseNPC, l, p);
                            }
                            //and fix their schedule
                            FixSpouseSchedule(l, otherSpouseNPC, true);
                        }
                    }
                }
            }

            //check for wedding date?
            List<string> toWed = new List<string>();
            //Modworks.Log.Alert("ENGAGEMENTS COUNT: " + Relationships.Engagements.Count);
            foreach (var ee in Relationships.Engagements.Keys)
            {
                //Monitor.Log("We have an engagement with " + ee);
                if (Relationships.Engagements[ee].IsToday())
                {
                    //Monitor.Log("ENGAGEMENT IS GO!");
                    toWed.Add(ee);
                }
            }
            if (toWed.Count > 0) {
                List<NPC> oldSpouses = new List<NPC>();
                Relationships.StorePrimarySpouse();
                foreach (string nxs in Relationships.Spouses)
                {
                    //Modworks.Log.Alert("adding oldspouse " + nxs);
                    oldSpouses.Add(Game1.getCharacterFromName(nxs));
                }

                List<NPC> newSpouses = new List<NPC>();
                foreach (string newSpouse in toWed)
                {
                    GameLocation l = Game1.getLocationFromName(Game1.player.homeLocation.Value) as StardewValley.Locations.FarmHouse;
                    var npc = Game1.getCharacterFromName(newSpouse);
                    var p = Modworks.Locations.FindPathableAndClearTile(l, Point.Zero);
                    if (p != Point.Zero)
                    {
                        Modworks.NPCs.Warp(npc, l, p);
                    } else
                    {
                        p = Modworks.Locations.FindPathableAndClearTile(Game1.getLocationFromName("Town"), Point.Zero);
                        Modworks.NPCs.Warp(npc, l, p);
                    }
                    Relationships.Marry(newSpouse);
                    Relationships.MakePrimarySpouse(newSpouse);
                    newSpouses.Add(npc);
                    Game1.player.friendshipData[Game1.player.spouse].WeddingDate = null;
                }
                Relationships.PolyWedding(oldSpouses, newSpouses);
            }
        }



        public void FixSpouseSchedule(GameLocation l, NPC npc, bool poly = false)
        {
            if (poly)
            {
                npc.DefaultPosition = new Vector2(npc.getTileX() * 64, npc.getTileY() * 64);
                npc.DefaultMap = npc.currentLocation.Name;
                if (Modworks.RNG.Next(2) == 1)
                {
                    var p = Modworks.Locations.FindPathableAndClearTile(l, npc.getTileLocationPoint());
                    npc.controller = new PathFindController(npc, l, p, Modworks.RNG.Next(4));
                }
            } else
            {
                string text = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
                if ((npc.Name.Equals("Penny") && (text.Equals("Tue") || text.Equals("Wed") || text.Equals("Fri"))) || (npc.Name.Equals("Maru") && (text.Equals("Tue") || text.Equals("Thu"))) || (npc.Name.Equals("Harvey") && (text.Equals("Tue") || text.Equals("Thu"))))
                {
                    npc.setNewDialogue("MarriageDialogue", "jobLeave_", -1, add: false, clearOnMovement: true);
                }
                if (!Game1.isRaining)
                {
                    npc.setNewDialogue("MarriageDialogue", "funLeave_", -1, add: false, clearOnMovement: true);
                }
                npc.followSchedule = false;
                npc.endOfRouteMessage.Value = null;
                if (!Game1.player.divorceTonight.Value) npc.marriageDuties();
            }
        }


        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            //update poly spouses, don't leave them stagnant
            foreach (var spouse in Relationships.PolyData.PolySpouses[Game1.player.UniqueMultiplayerID])
            {
                NPC spouseNpc = Game1.getCharacterFromName(spouse);
                //only redirect if not moving
                if (!Modworks.NPCs.IsPathing(spouseNpc))
                {
                    if (Modworks.RNG.Next(5) == 1)
                    {

                        if (spouseNpc != null && spouseNpc.currentLocation == null)
                        {
                            //we lost them! let's fix it
                            Game1.getLocationFromName(Game1.player.homeLocation.Value).addCharacterAtRandomLocation(spouseNpc);
                            Monitor.Log(spouseNpc.Name + " went home.");
                        }
                        else
                        {
                            if (spouseNpc == null) continue;
                            GameLocation l = spouseNpc.currentLocation;
                            bool warped = false;
                            if (l.farmers.Count == 0) //noone's looking. we could move them to an adjacent map.
                            {
                                if (Modworks.RNG.Next(3) == 0)
                                {
                                    Warp w = l.warps[Modworks.RNG.Next(l.warps.Count)];
                                    GameLocation l2 = Game1.getLocationFromName(w.TargetName);
                                    if (l2.farmers.Count == 0) //but only if we're not looking here either. have to skip the NPCBarriers.
                                    {
                                        l.characters.Remove(spouseNpc);
                                        l2.addCharacterAtRandomLocation(spouseNpc);
                                        Monitor.Log(spouseNpc.Name + " moved to " + l2.Name);
                                        l = spouseNpc.currentLocation;
                                        warped = true;
                                    }
                                }
                            }
                            var p = Modworks.Locations.FindPathableAndClearTile(l, spouseNpc.getTileLocationPoint(), warped ? 8 : 0);
                            if (p != Point.Zero)
                            {
                                spouseNpc.willDestroyObjectsUnderfoot = false;
                                spouseNpc.controller = new PathFindController(spouseNpc, l, new Point((int)p.X, (int)p.Y), -1, OnSpouseWalkComplete, 100);
                            }
                        }
                    }
                }
            }
        }

        public void OnSpouseWalkComplete(Character c, GameLocation l)
        {
            //c.controller = null;
        }

        public List<Rectangle> GetVanillaHouseWallRects()
        {
            List<Rectangle> list = new List<Rectangle>();
            switch (Game1.player.HouseUpgradeLevel)
            {
                case 0:
                    list.Add(new Rectangle(1, 1, 10, 3));
                    break;
                case 1:
                    list.Add(new Rectangle(1, 1, 17, 3));
                    list.Add(new Rectangle(18, 6, 2, 2));
                    list.Add(new Rectangle(20, 1, 9, 3));
                    break;
                case 2:
                case 3:
                    list.Add(new Rectangle(1, 1, 12, 3));
                    list.Add(new Rectangle(15, 1, 13, 3));
                    list.Add(new Rectangle(13, 3, 2, 2));
                    list.Add(new Rectangle(1, 10, 10, 3));
                    list.Add(new Rectangle(13, 10, 8, 3));
                    list.Add(new Rectangle(21, 15, 2, 2));
                    list.Add(new Rectangle(23, 10, 11, 3));
                    break;
            }
            return list;
        }
    }
}