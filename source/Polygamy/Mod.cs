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

            Relationships = new Relationships();
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
            Relationships = new Relationships();
            helper.ConsoleCommands.Add("polygamy", "'polygamy help' for more info", PolygamyCommand);

        }

        public void PolygamyCommand(string command, string[] parameters)
        {
            if(parameters.Length < 2 || parameters[0] == "help")
            {
                var exampleNames = new[] { "Pierre", "Robin", "Sandy", "Pam", "Jodi", "Kent", "Caroline", "Clint", "Evelyn", "Gus", "Demetrius", "Lewis", "Marnie", "Wizard" };
                string exampleName = exampleNames[Modworks.RNG.Next(exampleNames.Length)];
                Monitor.Log($"Polygamy commands:\npolygamy flirt {exampleName} - would make '{exampleName}' a dateable NPC.\npolygamy unflirt {exampleName} - would make '{exampleName}' no longer dateable.\npolygamy roll {exampleName} - would make '{exampleName}' your 'official' spouse tomorrow\npolygamy marry {exampleName} - would immediately start a wedding with '{exampleName}'\npolygamy date {exampleName} - would make '{exampleName}' date you\npolygamy breakup {exampleName} - would make '{exampleName}' no longer dating you\npolygamy divorce {exampleName} - would, well, divorce '{exampleName}'\npolygamy undivorce {exampleName} - would make '{exampleName}' forget they were ever married to you", LogLevel.Info);
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
            } else if (parameters[0] == "date")
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
            Relationships.ScanForNPCs();

            if (Relationships.IsThisPlayerMarried)
            {
                if(Relationships.Spouses.Count == 0)
                {
                    //let's just do a safety fix here in case you just divorced all but one who wasn't primary
                    Relationships.MakePrimarySpouse(Relationships.GetNextPrimarySpouse());
                    return;
                }
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
                            Monitor.Log("putting " + spouseName + " in bed (player side)");
                            var pos = npcObject.Position;
                            pos.X += (float)(-32f + Modworks.RNG.NextDouble() * 96f); //vary the position
                            npcObject.Position = pos;
                        } else if (Modworks.RNG.Next(100) < Math.Max(100 - Relationships.Spouses.Count * 7, 3))
                        {
                            var npcObject = Game1.getCharacterFromName(spouseName);
                            Modworks.NPCs.Warp(npcObject, farmHouseName, bedSpot);
                            Monitor.Log("putting " + spouseName + " in bed (spouse side)");
                            var pos = npcObject.Position;
                            pos.X += (float)(-32f + Modworks.RNG.NextDouble() * 96f); //vary the position
                            npcObject.Position = pos;
                        }
                        else //or around the house at random
                        {
                            Monitor.Log("putting " + spouseName + " around the house");
                            NPC otherSpouseNPC = Game1.getCharacterFromName(spouseName);
                            //find a free tile to position them on
                            GameLocation l = Game1.getLocationFromName(Game1.player.homeLocation.Value) as StardewValley.Locations.FarmHouse;
                            var p = FindSpotForNPC(l, l is StardewValley.Locations.FarmHouse, otherSpouseNPC.getTileLocationPoint());
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
            foreach (var ee in Relationships.Engagements.Keys)
            {
                Monitor.Log("We have an engagement with " + ee);
                if (Relationships.Engagements[ee].IsToday())
                {
                    Monitor.Log("ENGAGEMENT IS GO!");
                    toWed.Add(ee);
                }
            }
            foreach(string newSpouse in toWed)
            {
                Relationships.Marry(newSpouse);
                Relationships.MakePrimarySpouse(newSpouse);
                Relationships.Wedding(newSpouse);
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
                    var p = FindSpotForNPC(l, l is StardewValley.Locations.FarmHouse, npc.getTileLocationPoint());
                    npc.controller = new PathFindController(npc, l, new Point(p.X, p.Y), Modworks.RNG.Next(4));
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
                if (Modworks.RNG.Next(5) == 1)
                {
                    NPC spouseNpc = Game1.getCharacterFromName(spouse);
                    if (spouseNpc.currentLocation == null)
                    {
                        //we lost them! let's fix it
                        Game1.getLocationFromName(Game1.player.homeLocation.Value).addCharacterAtRandomLocation(spouseNpc);
                        Monitor.Log(spouseNpc.Name + " went home.");
                    }
                    else
                    {
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
                        var p = FindSpotForNPC(l, l is StardewValley.Locations.FarmHouse, spouseNpc.getTileLocationPoint(), warped ? 8 : 0);
                        if (p != Point.Zero)
                        {
                            spouseNpc.willDestroyObjectsUnderfoot = false;
                            spouseNpc.controller = new PathFindController(spouseNpc, l, new Point((int)p.X, (int)p.Y), -1, OnSpouseWalkComplete, 100);
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

        public Point FindSpotForNPC(GameLocation l, bool checkVanillaHouseWalls, Point p, int radius = 0)
        {
            Point randomPoint = Point.Zero;
            for (int i = 0; i < 100; i++)
            {
                int sizeX = l.map.GetLayer("Back").TileWidth;
                int sizeY = l.map.GetLayer("Back").TileHeight;
                if(radius > 0)
                    randomPoint = new Point((p.X - radius) + Modworks.RNG.Next(radius * 2), (p.Y - radius) + Modworks.RNG.Next(radius * 2));
                else
                    randomPoint = new Point(Modworks.RNG.Next(sizeX), Modworks.RNG.Next(sizeY));
                bool unacceptable = false;
                unacceptable = (l.getTileIndexAt(randomPoint.X, randomPoint.Y, "Back") == -1 || !l.isTileLocationTotallyClearAndPlaceable(randomPoint.X, randomPoint.Y) || (checkVanillaHouseWalls && Utility.pointInRectangles(GetVanillaHouseWallRects(), randomPoint.X, randomPoint.Y)));
                if (!unacceptable) return randomPoint;
            }
            return Point.Zero;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.IsActionButton())
            {
                if (Context.IsPlayerFree)
                {
                    //snag a list of the NPCs we are interested in
                    var targetedNPCs = Modworks.NPCs.GetAllCharacterNames(true, false, Game1.player.currentLocation);

                    //let's play my favorite game: WHO ARE WE CLICKING
                    Vector2 actionPos = e.Cursor.GrabTile;
                    foreach (string n in targetedNPCs)
                    {
                        if (!string.IsNullOrWhiteSpace(n))
                        {
                            NPC n2 = Game1.getCharacterFromName(n);
                            if (n2 != null)
                            {
                                if (n2.currentLocation != null && n2.currentLocation.Name == Game1.currentLocation.Name)
                                {
                                    if (n2.getTileX() == actionPos.X && n2.getTileY() == actionPos.Y)
                                    {
                                        //are we holding an item
                                        if (Game1.player.ActiveObject != null)
                                        {
                                            //is it the bouquet?
                                            if(Game1.player.ActiveObject.ParentSheetIndex == 458)
                                            {
                                                if (!Relationships.DateableNPCs.Contains(n2))
                                                {
                                                    //REFUSE, NOT DATEABLE
                                                    //either divorced, already married, already dating, or not available
                                                    Helper.Input.Suppress(e.Button);
                                                    n2.CurrentDialogue.Push(new Dialogue((n2.Gender == 1) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3970") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3971"), n2));
                                                    Game1.drawDialogue(n2);
                                                    return;
                                                }
                                                else { 
                                                    if (Modworks.Player.GetFriendshipPoints(n2.Name) >= 2000) //ready for relationship!
                                                    {
                                                        //LETTUCE DATE
                                                        Helper.Input.Suppress(e.Button);
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
                                                        //REFUSE, DONT KNOW YOU WELL ENOUGH
                                                        Helper.Input.Suppress(e.Button);
                                                        n2.CurrentDialogue.Push(new Dialogue((Game1.random.NextDouble() < 0.5) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3958") : Game1.LoadStringByGender(n2.Gender, "Strings\\StringsFromCSFiles:NPC.cs.3959"), n2));
                                                        Game1.drawDialogue(n2);
                                                        return;
                                                    }
                                                }
                                            }
                                            //or is the pendant?
                                            else if(Game1.player.ActiveObject.ParentSheetIndex == 460)
                                            {
                                                if (Game1.player.HouseUpgradeLevel < 2)
                                                {
                                                    //REFUSE, NOWHERE TO LIVE
                                                    Helper.Input.Suppress(e.Button);
                                                    if(Game1.random.NextDouble() < 0.1)
                                                    {
                                                        n2.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3972"), n2));
                                                    } else
                                                    {
                                                        n2.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Pickaxe.cs.14194") + "$h", n2));
                                                    }
                                                    Game1.drawDialogue(n2);
                                                    return;
                                                }
                                                else
                                                {
                                                    if (!Relationships.MarryableNPCs.Contains(n2))
                                                    {
                                                        //REFUSE, NOT MARRYABLE
                                                        //either divorced, already married, not dating, or not available
                                                        Helper.Input.Suppress(e.Button);
                                                        n2.CurrentDialogue.Push(new Dialogue((n2.Gender == 1) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3970") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3971"), n2));
                                                        Game1.drawDialogue(n2);
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        if (Modworks.Player.GetFriendshipPoints(n2.Name) >= 2499) //ready for marriage!
                                                        {
                                                            //LETTUCE MARRY
                                                            Helper.Input.Suppress(e.Button);
                                                            Game1.changeMusicTrack("none");
                                                            n2.CurrentDialogue.Clear();
                                                            Relationships.Engage(n2.Name);
                                                            Modworks.Player.SetFriendshipPoints(n2.Name, 2500);
                                                            Dialogue d1, d2, d3;
                                                            bool dialogueOk = Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue").ContainsKey(n2.Name + "0");
                                                            if (!dialogueOk)
                                                            {
                                                                //we're marrying something that doesn't have accept dialogue. default dialog
                                                                if(n2.Gender == 0)
                                                                { //male
                                                                    d1 = new Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")["Sebastian" + "0"], n2);
                                                                    d2 = new Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")["Alex" + "1"], n2);
                                                                } else
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
                                                            //REFUSE, DONT KNOW YOU WELL ENOUGH
                                                            Helper.Input.Suppress(e.Button);
                                                            n2.CurrentDialogue.Push(new Dialogue((Game1.random.NextDouble() < 0.5) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3972") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3973"), n2));
                                                            Game1.drawDialogue(n2);
                                                            return;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //to kiss (doesn't use normal grabTile target)
                    if (Game1.player.ActiveObject == null)
                    {
                        var facingTarget = Modworks.Player.GetFacingTileCoordinate();
                        var standingTarget = Modworks.Player.GetStandingTileCoordinate();
                        var key = Game1.currentLocation.Name + "." + facingTarget[0] + "." + facingTarget[1];
                        //check if npc is in front of player
                        foreach (string n in targetedNPCs)
                        {
                            if (!string.IsNullOrWhiteSpace(n))
                            {
                                NPC n2 = Game1.getCharacterFromName(n);
                                if (n2 != null)
                                {
                                    if (n2.currentLocation != null && n2.currentLocation.Name == Game1.currentLocation.Name)
                                    {
                                        bool npcHere = n2.getTileX() == facingTarget[0] && n2.getTileY() == facingTarget[1];
                                        if (!npcHere) npcHere = n2.getTileX() == standingTarget[0] && n2.getTileY() == standingTarget[1];
                                        if (npcHere)
                                        {
                                            Relationships.RelationshipStatus rs = Relationships.CheckStatusProper(n2.Name);
                                            if (rs == Relationships.RelationshipStatus.DATING || rs == Relationships.RelationshipStatus.MARRIED || rs == Relationships.RelationshipStatus.PRIMARYSPOUSE)
                                                Relationships.Kiss(n2.Name);
                                            break; //we don't return here because a dialog/etc starting will automatically cancel the kiss
                                                   //so this actually works really cleanly just like this
                                        }
                                    }
                                }
                            }
                        }
                    }

                    /*


                    //to marry
                    //dating?
                    //holding the mermaid's pendant?
                    else if (Game1.player.ActiveObject != null && Game1.player.ActiveObject.ParentSheetIndex == 460)
                    {
                        if (Game1.player.spouse != null) //we only need polygamy for second+ spouse
                        {
                            var target = ModUtil.GetLocalPlayerFacingTileCoordinate();
                            var key = Game1.currentLocation.Name + "." + target[0] + "." + target[1];
                            //check if npc is in front of player
                            NPC tnpc = null;
                            foreach (NPC n in MarryableNPCs)
                            {
                                if (n.getTileX() == target[0] && n.getTileY() == target[1] && n.currentLocation != null && n.currentLocation.Name == Game1.currentLocation.Name)
                                {
                                    tnpc = n;
                                    break;
                                }
                            }
                            if (tnpc == null || tnpc.Name == Game1.player.spouse) return;
                            if (ModUtil.GetFriendshipPoints(tnpc.Name) >= 2500) //ready for marriage!
                            {
                                Helper.Input.Suppress(e.Button);
                                //if so we can override the dialogue here if conditions are met
                                Game1.changeMusicTrack("none");
                                //demote current spouse to side piece\
                                if (Game1.player.HouseUpgradeLevel < 2) Game1.player.HouseUpgradeLevel = 2; //prevent crash
                                if (Game1.player.spouse != null)
                                {
                                    //push the spouse into a poly slot
                                    PolyData.PolySpouses[Game1.player.UniqueMultiplayerID].Add(Game1.player.spouse);
                                }
                                //marry the new one while they're still interesting
                                //WITH wedding ceremony (as opposed to hotswapping without)
                                ModUtil.SetFriendshipPoints(tnpc.Name, 2500);
                                Game1.player.spouse = tnpc.Name;
                                PolyData.PrimarySpouse = tnpc.Name;
                                tnpc.CurrentDialogue.Clear();
                                tnpc.CurrentDialogue.Push(new Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")[tnpc.Name + "0"], tnpc));
                                tnpc.CurrentDialogue.Push(new Dialogue(Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue")[tnpc.Name + "1"], tnpc));
                                tnpc.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3980"), tnpc));
                                Game1.player.reduceActiveItemByOne();
                                Game1.player.completelyStopAnimatingOrDoingAction();
                                Game1.drawDialogue(tnpc);
                                //DO WEDDIN' NAO!
                                Game1.player.friendshipData[Game1.player.spouse].WeddingDate = null;
                                Game1.weddingToday = true;
                                Game1.player.friendshipData[Game1.player.spouse].Status = FriendshipStatus.Engaged;
                                Game1.checkForWedding();
                                Game1.player.friendshipData[Game1.player.spouse].Status = FriendshipStatus.Married;
                            }
                        }
                    }

                    */
                }
            }
        }
    }
}