using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modworks = bwdyworks.Modworks;

namespace Polygamy
{
    public class Relationships
    {
        public static PolyData PolyData;

        internal HashSet<NPC> MarryableNPCs;
        internal HashSet<NPC> DateableNPCs;

        internal string LastSpouseRoom = null;

        public Relationships()
        {
            MarryableNPCs = new HashSet<NPC>();
            DateableNPCs = new HashSet<NPC>();
        }

        internal  Farmer Player
        {
            get { return Game1.player; }
        }

        internal long PlayerID
        {
            get { return Game1.player.UniqueMultiplayerID; }
        }

        internal HashSet<string> Dates
        {
            get {
                if(PolyData.PolyDates == null)
                    PolyData.PolyDates = new Dictionary<long, HashSet<string>>();
                if (!PolyData.PolyDates.ContainsKey(PlayerID))
                    PolyData.PolyDates[PlayerID] = new HashSet<string>();
                return PolyData.PolyDates[PlayerID];
            }
        }

        internal Dictionary<string, bwdyworks.GameDate> Engagements
        {
            get
            {
                if (PolyData.PolyEngagements == null)
                    PolyData.PolyEngagements = new Dictionary<long, Dictionary<string, bwdyworks.GameDate>>();
                if (!PolyData.PolyEngagements.ContainsKey(PlayerID))
                    PolyData.PolyEngagements[PlayerID] = new Dictionary<string, bwdyworks.GameDate>();
                return PolyData.PolyEngagements[PlayerID];
            }
        }

        internal HashSet<string> Spouses
        {
            get
            {
                if (PolyData.PolySpouses == null)
                    PolyData.PolySpouses = new Dictionary<long, HashSet<string>>();
                if (!PolyData.PolySpouses.ContainsKey(PlayerID))
                    PolyData.PolySpouses[PlayerID] = new HashSet<string>();
                return PolyData.PolySpouses[PlayerID];
            }
        }

        internal HashSet<string> Divorces
        {
            get
            {
                if (PolyData.PolyDivorces == null)
                    PolyData.PolyDivorces = new Dictionary<long, HashSet<string>>();
                if (!PolyData.PolyDivorces.ContainsKey(PlayerID))
                    PolyData.PolyDivorces[PlayerID] = new HashSet<string>();
                return PolyData.PolyDivorces[PlayerID];
            }
        }

        internal string VanillaSpouse
        {
            get
            {
                return Game1.player.spouse;
            }
            set
            {
                Game1.player.spouse = value;
            }
        }

        internal string PrimarySpouse
        {
            get
            {
                return PolyData.PrimarySpouse;
            }
            set
            {
                PolyData.PrimarySpouse = value;
            }
        }

        internal string TomorrowSpouse
        {
            get
            {
                return PolyData.TomorrowSpouse;
            }
            set
            {
                PolyData.TomorrowSpouse = value;
            }
        }

        //doesn't actually marry, it's just a ceremony to VanillaSpouse
        internal void Wedding(string NPC)
        {
            Game1.player.friendshipData[Game1.player.spouse].WeddingDate = null;
            Game1.weddingToday = true;
            Game1.checkForWedding();
        }

        internal bool IsThisPlayerMarried
        {
            get
            {
                bool married = !string.IsNullOrWhiteSpace(PrimarySpouse);
                if (!married) married = Spouses.Count > 0;
                return married;
            }
        }

        private bool CheckPrimarySpouseProper(string NPC)
        {
            //couple of ways primary can go sideways:
            //if poly priamry is set and vanilla is not
            //if vanilla is set and poly primary is not
            //if the spouse is still in the main poly spouses array when they should be primary
            if ((PrimarySpouse == NPC ^ VanillaSpouse == NPC) || (Spouses.Contains(NPC) && (PrimarySpouse == NPC || VanillaSpouse == NPC))) MakePrimarySpouse(NPC); //fix desynced status
            return PolyData.PrimarySpouse == NPC;
        }

        private bool CheckDivorcedProper(string NPC)
        {
            if (Divorces.Contains(NPC) ^ Modworks.Player.GetFriendshipStatus(NPC) == FriendshipStatus.Divorced) Divorce(NPC); //fix desynced status
            return Divorces.Contains(NPC);
        }

        private bool CheckMarriedProper(string NPC)
        {
            if (Spouses.Contains(NPC) ^ Modworks.Player.GetFriendshipStatus(NPC) == FriendshipStatus.Married) Marry(NPC); //fix desynced status
            return Spouses.Contains(NPC);
        }

        private bool CheckEngagedProper(string NPC)
        {
            if (Engagements.ContainsKey(NPC) ^ Modworks.Player.GetFriendshipStatus(NPC) == FriendshipStatus.Engaged) Engage(NPC); //fix desynced status
            return Engagements.ContainsKey(NPC);
        }

        private bool CheckDatingProper(string NPC)
        {
            if (Dates.Contains(NPC) ^ Modworks.Player.GetFriendshipStatus(NPC) == FriendshipStatus.Dating) Date(NPC); //fix desynced status
            return Dates.Contains(NPC);
        }

        public RelationshipStatus CheckStatusProper(string NPC)
        {
            if (CheckDivorcedProper(NPC)) return RelationshipStatus.DIVORCED;
            if (CheckPrimarySpouseProper(NPC)) return RelationshipStatus.PRIMARYSPOUSE;
            if (CheckMarriedProper(NPC)) return RelationshipStatus.MARRIED;
            if (CheckEngagedProper(NPC)) return RelationshipStatus.ENGAGED;
            if (CheckDatingProper(NPC)) return RelationshipStatus.DATING;
            return RelationshipStatus.DEFAULT;
        }

        public bool IsMarried(string NPC)
        {
            var status = CheckStatusProper(NPC);
            return status == RelationshipStatus.MARRIED || status == RelationshipStatus.PRIMARYSPOUSE;
        }

        public bool IsDating(string NPC)
        {
            var status = CheckStatusProper(NPC);
            return status == RelationshipStatus.DATING || status == RelationshipStatus.ENGAGED;
        }

        public void Forget(string NPC, bool resetFriendship = true)
        {
            if(resetFriendship) Modworks.Player.SetFriendshipPoints(NPC, 0);
            Modworks.Player.SetFriendshipStatus(NPC, FriendshipStatus.Friendly);
            Spouses.Remove(NPC);
            Engagements.Remove(NPC);
            Divorces.Remove(NPC);
            Dates.Remove(NPC);
        }

        public void SetDateable(string NPC, bool dateable)
        {
            Game1.getCharacterFromName(NPC).datable.Value = dateable;
        }

        public void Date(string NPC)
        {
            SetDateable(NPC, true);
            if (Modworks.Player.GetFriendshipPoints(NPC) < 2000)
                Modworks.Player.SetFriendshipPoints(NPC, 2000);
            Modworks.Player.SetFriendshipStatus(NPC, FriendshipStatus.Dating);
            Spouses.Remove(NPC);
            Engagements.Remove(NPC);
            Divorces.Remove(NPC);
            Dates.Add(NPC);
        }

        public void Engage(string NPC, bwdyworks.GameDate weddingDay = null)
        {
            var backup = Game1.player.spouse;
            Game1.player.spouse = NPC;
            SetDateable(NPC, true);
            if (weddingDay == null)
            {
                weddingDay = bwdyworks.GameDate.CreateWeddingDate();
            }
            Modworks.Log.Info("Engaged to " + NPC + ", to be wed on " + weddingDay.GetSeasonString() + " " + weddingDay.Day);
            StorePrimarySpouse();
            if(Modworks.Player.GetFriendshipPoints(NPC) < 2500)
                Modworks.Player.SetFriendshipPoints(NPC, 2500);
            Modworks.Player.SetFriendshipStatus(NPC, FriendshipStatus.Engaged);
            //Game1.player.spouse = NPC;
            Game1.player.friendshipData[NPC].Proposer = Game1.player.UniqueMultiplayerID;
            Game1.player.friendshipData[NPC].WeddingDate = new WorldDate(Game1.year, weddingDay.GetSeasonString().ToLower(), weddingDay.Day);
            Spouses.Remove(NPC);
            Dates.Remove(NPC);
            Divorces.Remove(NPC);
            Engagements.Add(NPC, weddingDay);
            Game1.player.spouse = backup;
        }

        public void Marry(string NPC, bool wedding = false)
        {
            SetDateable(NPC, true);
            if (Modworks.Player.GetFriendshipPoints(NPC) < 2500)
                Modworks.Player.SetFriendshipPoints(NPC, 2500);
            if (Game1.player.HouseUpgradeLevel < 2) Game1.player.HouseUpgradeLevel = 2; //prevent crash
            Modworks.Player.SetFriendshipStatus(NPC, FriendshipStatus.Married);
            Dates.Remove(NPC);
            Engagements.Remove(NPC);
            Divorces.Remove(NPC);
            Spouses.Add(NPC);
            if (string.IsNullOrWhiteSpace(PrimarySpouse)) MakePrimarySpouse(NPC);
        }

        public void MakePrimarySpouse(string NPC)
        {
            if (PrimarySpouse == NPC ^ VanillaSpouse == NPC) StorePrimarySpouse();
            if (!CheckMarriedProper(NPC)) Marry(NPC);
            Spouses.Remove(NPC);
            PrimarySpouse = NPC;
            VanillaSpouse = NPC;
            //only show a spouse room if it exists.
            //todo, better way to tell than a list?
            if (NPC != LastSpouseRoom)
            {
                string[] spousesWithHouseData = new[] { "Abigail", "Penny", "Leah", "Haley", "Maru", "Sebastian", "Alex", "Harvey", "Elliott", "Sam", "Shane", "Emily" };
                if (!spousesWithHouseData.Contains(NPC))
                {
                    //no spouse data. let's fake it
                    Game1.player.spouse = spousesWithHouseData[Modworks.RNG.Next(spousesWithHouseData.Length)];
                    Game1.getFarm().addSpouseOutdoorArea(Game1.player.spouse);
                    Utility.getHomeOfFarmer(Game1.player).showSpouseRoom();
                    LastSpouseRoom = Game1.player.spouse;
                    Game1.player.spouse = NPC;
                }
                else
                {
                    Game1.getFarm().addSpouseOutdoorArea(Game1.player.spouse);
                    Utility.getHomeOfFarmer(Game1.player).showSpouseRoom();
                    LastSpouseRoom = NPC;
                }
                Modworks.Log.Debug("CHANGING SPOUSE ROOM TO " + LastSpouseRoom);
            }
        }

        public void StorePrimarySpouse()
        {
            string s = PrimarySpouse;
            if (string.IsNullOrWhiteSpace(s)) s = VanillaSpouse;
            if (!string.IsNullOrWhiteSpace(s))
            {
                //CheckPrimarySpouseProper(s); //make sure data is valid first
                PrimarySpouse = null;
                VanillaSpouse = null;
                Spouses.Add(s);
                CheckMarriedProper(s); //make sure everything looks right
            }
        }

        public bool IsValidSpouse(string NPC)
        {
            if (string.IsNullOrWhiteSpace(NPC)) return false;
            if (Spouses.Contains(NPC)) return true;
            if (PrimarySpouse == NPC) return true;
            return false;
        }

        public string GetNextPrimarySpouse()
        {
            if (IsValidSpouse(TomorrowSpouse))
            {
                string r = TomorrowSpouse;
                TomorrowSpouse = null;
                return r;
            } else if(Spouses.Count > 0)
            {
                //check for a lucky birthday spouse
                bool birthday = false;
                string result = null;
                foreach(string s in Spouses)
                {
                    var spouseNpc = Game1.getCharacterFromName(s);
                    if (spouseNpc.isBirthday(Game1.currentSeason, Game1.dayOfMonth))
                    {
                        birthday = true;
                        result = s;
                    }
                }
                if (birthday) return result;
                return Spouses.ToArray()[Modworks.RNG.Next(Spouses.Count)];
            } else return PrimarySpouse; //nullcheck the return pls
        }

        public void Divorce(string NPC)
        {
            bool swap = false;
            if (PrimarySpouse == NPC)
            {
                StorePrimarySpouse();
                swap = true;
            }
            Modworks.Player.SetFriendshipPoints(NPC, 0);
            Modworks.Player.SetFriendshipStatus(NPC, FriendshipStatus.Divorced);
            Dates.Remove(NPC);
            Engagements.Remove(NPC);
            Spouses.Remove(NPC);
            Divorces.Add(NPC);
            if(swap) MakePrimarySpouse(GetNextPrimarySpouse());
        }

        public void Kiss(string NPC)
        {
            var npcObject = Game1.getCharacterFromName(NPC);
            var player = Game1.player;
            if (player.IsLocalPlayer)
            {
                int timeOfDay = Game1.timeOfDay;
                if (npcObject.Sprite.CurrentAnimation == null)
                {
                    npcObject.faceDirection(-3);
                }
                if (npcObject.Sprite.CurrentAnimation == null && !npcObject.hasTemporaryMessageAvailable() && npcObject.CurrentDialogue.Count == 0 && Game1.timeOfDay < 2200 && !npcObject.isMoving() && player.ActiveObject == null)
                {
                    npcObject.faceGeneralDirection(player.getStandingPosition());
                    player.faceGeneralDirection(npcObject.getStandingPosition());
                    if (npcObject.FacingDirection == 3 || npcObject.FacingDirection == 1)
                    {
                        int frame = 28;
                        bool flag = true;
                        switch (npcObject.Name)
                        {
                            case "Maru":
                                frame = 28;
                                flag = false;
                                break;
                            case "Harvey":
                                frame = 31;
                                flag = false;
                                break;
                            case "Leah":
                                frame = 25;
                                flag = true;
                                break;
                            case "Elliott":
                                frame = 35;
                                flag = false;
                                break;
                            case "Sebastian":
                                frame = 40;
                                flag = false;
                                break;
                            case "Abigail":
                                frame = 33;
                                flag = false;
                                break;
                            case "Penny":
                                frame = 35;
                                flag = true;
                                break;
                            case "Alex":
                                frame = 42;
                                flag = true;
                                break;
                            case "Sam":
                                frame = 36;
                                flag = true;
                                break;
                            case "Shane":
                                frame = 34;
                                flag = false;
                                break;
                            case "Emily":
                                frame = 33;
                                flag = false;
                                break;
                        }
                        bool flag2 = (flag && npcObject.FacingDirection == 3) || (!flag && npcObject.FacingDirection == 1);
                        if (player.getFriendshipHeartLevelForNPC(npcObject.Name) > 9)
                        {
                            npcObject.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                            {
                                new FarmerSprite.AnimationFrame(frame, Game1.IsMultiplayer ? 1010 : 10, false, flag2, npcObject.haltMe, true)
                            });
                            if (true /* !npcObject.hasBeenKissedToday */)
                            {
                                player.changeFriendship(10, npcObject);
                                /*
                                Game1.Multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6), 2000f, 1, 0, new Vector2(getTileX(), getTileY()) * 64f + new Vector2(16f, -64f), flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
                                {
                                    motion = new Vector2(0f, -0.5f),
                                    alphaFade = 0.01f
                                });
                                */
                                player.currentLocation.playSound("dwop");
                                player.exhausted.Value = false;
                            }
                            //npcObject.hasBeenKissedToday = true;
                            //if not in the 'no animation frame for kissing' list, then show the kissing frame
                            if (!new[] { "Bouncer", "Caroline", "Clint", "Demetrius", "Dwarf", "Evelyn", "George", "Gil", "Governor", "Grandpa", "Gunther", "Gus", "Henchman", "Jas", "Jodi", "Kent", "Krobus", "Lewis", "Linus", "Marlon", "Marnie", "Morris", "Mr. Qi", "Old Mariner", "Pam", "Pierre", "Robin", "Sandy", "Vincent", "Willy", "Wizard" }.Contains(npcObject.Name))
                                npcObject.Sprite.UpdateSourceRect();
                        }
                        else
                        {
                            npcObject.faceDirection((Game1.random.NextDouble() < 0.5) ? 2 : 0);
                            npcObject.doEmote(12);
                        }
                        player.CanMove = false;
                        player.FarmerSprite.PauseForSingleAnimation = false;
                        if ((flag && !flag2) || (!flag && flag2))
                        {
                            player.faceDirection(3);
                        }
                        else
                        {
                            player.faceDirection(1);
                        }
                        player.FarmerSprite.animateOnce(new List<FarmerSprite.AnimationFrame>
                        {
                            new FarmerSprite.AnimationFrame(101, 1000, 0, false, player.FacingDirection == 3),
                            new FarmerSprite.AnimationFrame(6, 1, false, player.FacingDirection == 3, Farmer.completelyStopAnimating)
                        }.ToArray());
                        return;
                    }
                }
            }
        }

        public void ScanForNPCs()
        {
            //clear out old data
            MarryableNPCs.Clear();
            DateableNPCs.Clear();

            //populate dateable, marryable
            var npcs = Modworks.NPCs.GetAllCharacterNames(true);
            foreach(var npc in npcs)
            {
                var status = CheckStatusProper(npc);
                if (status == RelationshipStatus.DEFAULT)
                {
                    DateableNPCs.Add(Game1.getCharacterFromName(npc));
                    Modworks.Log.Trace("found dateable NPC: " + npc);
                    continue;
                } else if (status == RelationshipStatus.DATING)
                {
                    MarryableNPCs.Add(Game1.getCharacterFromName(npc));
                    Modworks.Log.Trace("found marryable NPC: " + npc);
                }    
            }
            Modworks.Log.Debug("Scanned " + npcs.Count + " NPCs.");
            Modworks.Log.Debug("Found " + DateableNPCs.Count + " date-ready NPCs.");
            Modworks.Log.Debug("Found " + MarryableNPCs.Count + " marry-ready NPCs.");
        }

        public enum RelationshipStatus
        {
            DEFAULT = 0,
            DATING = 1,
            ENGAGED = 2,
            MARRIED = 3,
            PRIMARYSPOUSE = 4,
            DIVORCED = 5
        }
    }
}
