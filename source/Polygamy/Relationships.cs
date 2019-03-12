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

        public static string getCelebrationPositionsForDatables(List<NPC> ExistingSpouses, List<NPC> NewSpouses)
        {
            string text = " ";
            List<string> usedNames = new List<string>();

            //bring them on up!
            //new spouses
            int x = 28;
            int y = 63;
            for (int i2 = 0; i2 < NewSpouses.Count; i2++)
            {
                if (!usedNames.Contains(NewSpouses[i2].Name))
                {
                    text += NewSpouses[i2].Name + " " + (x + (i2 % 3)) + " " + (y - (i2 / 3)) + " " + 2 + " ";
                    usedNames.Add(NewSpouses[i2].Name);
                }
            }

            //old spouses
            x = 27;
            y = 63;
            for (int i2 = 0; i2 < ExistingSpouses.Count; i2++)
            {
                if (!usedNames.Contains(ExistingSpouses[i2].Name))
                {
                    text += ExistingSpouses[i2].Name + " " + (x - ((i2 + 1) % 3)) + " " + (y - ((i2 + 1) / 3)) + " " + 1 + " ";
                    usedNames.Add(ExistingSpouses[i2].Name);
                }
            }

            if (!usedNames.Contains("Sam"))
            {
                text += "Sam 25 65 0 ";
            }
            if (!usedNames.Contains("Sebastian"))
            {
                text += "Sebastian 24 65 0 ";
            }
            if (!usedNames.Contains("Alex"))
            {
                text += "Alex 25 69 0 ";
            }
            if (!usedNames.Contains("Harvey"))
            {
                text += "Harvey 23 67 0 ";
            }
            if (!usedNames.Contains("Elliott"))
            {
                text += "Elliott 32 65 0 ";
            }
            if (!usedNames.Contains("Haley"))
            {
                text += "Haley 26 69 0 ";
            }
            if (!usedNames.Contains("Penny"))
            {
                text += "Penny 23 66 0 ";
            }
            if (!usedNames.Contains("Maru"))
            {
                text += "Maru 24 68 0 ";
            }
            if (!usedNames.Contains("Leah"))
            {
                text += "Leah 33 65 0 ";
            }
            if (!usedNames.Contains("Shane"))
            {
                text += "Shane 32 66 0 ";
            }
            if (!usedNames.Contains("Emily"))
            {
                text += "Emily 30 66 0 ";
            }

            return text;
        }

        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }

        public static string GetWeddingDialogue(int dialogue, List<NPC> NewSpouses, List<NPC> OldSpouses)
        {
            if (dialogue == 0) {
                string pronoun = Game1.player.IsMale ? "he" : "she";
                string spouseList = "";
                if (OldSpouses.Count == 0 && NewSpouses.Count == 1)
                {
                    spouseList += " and " + NewSpouses[0].displayName;
                }
                else
                {
                    if ((OldSpouses.Count + NewSpouses.Count) > 9)
                    {
                        spouseList += " and " + (Game1.player.IsMale ? "his " : "her ") + "bevy of ";
                        bool hasMales = false;
                        bool hasFemales = false;
                        foreach (var nx1 in NewSpouses)
                        {
                            if (nx1.Gender == 0) hasFemales = true;
                            else hasMales = true;
                        }
                        foreach (var nx1 in OldSpouses)
                        {
                            if (nx1.Gender == 0) hasFemales = true;
                            else hasMales = true;
                        }
                        if (hasMales && hasFemales)
                        {
                            spouseList += "significant others";
                        }
                        else if (hasMales)
                        {
                            spouseList += "gentlemen";
                        }
                        else spouseList += "ladies";
                    }
                    else
                    {
                        foreach (var n1 in OldSpouses)
                        {
                            spouseList += n1.displayName + (OldSpouses.Last() == n1 ? "" : ", ");
                        }
                        spouseList = ReplaceLastOccurrence(spouseList, ",", " and");
                        spouseList += " with ";
                        foreach (var n1 in NewSpouses)
                        {
                            spouseList += n1.displayName + (NewSpouses.Last() == n1 ? "" : ", ");
                        }
                        spouseList = ReplaceLastOccurrence(spouseList, ",", " and");
                    }
                }
                return "When @ first arrived in Pelican Town, no one knew if " + pronoun + "'d fit in with our community...#$b#But from this day forward, @ is going to be as much a part of this town as any of us!$h#$b#It is my great honor on this day " + Game1.dayOfMonth + " of " + Game1.CurrentSeasonDisplayName + ", to unite @, " + spouseList + " in the bonds of marriage.";
            }
            if (dialogue == 1) return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5370"); //"Well, let's get right to it!"
            if (dialogue == 2)
            {
                //"@... %spouse... #$b# As the mayor of Pelican Town, and regional bearer of the matrimonial seal, I now pronounce you husband and..., well, husband!$h",
                int husbands = 0;
                int wives = 0;
                if (Game1.player.IsMale) husbands = 1;
                else wives = 1;
                foreach (var n1 in OldSpouses)
                {
                    if (n1.Gender == 0) husbands += 1;
                    else wives += 1;
                }
                foreach (var n1 in NewSpouses)
                {
                    if (n1.Gender == 0) husbands += 1;
                    else wives += 1;
                }

                if (husbands > 0) //there are males involved
                {
                    string h = "husband";
                    if (husbands > 1) h += "s";
                    if(wives > 0) //there are females involved
                    {
                        string w = "wi";
                        if (wives > 1) w += "ves";
                        else w += "fe";
                        return "As the mayor of Pelican Town, and regional bearer of the matrimonial seal, I now pronounce you " + h + " and " + w + "!$h";
                    } else
                    {
                        return "As the mayor of Pelican Town, and regional bearer of the matrimonial seal, I now pronounce you husbands!$h";
                    }
                } else
                {
                    return "As the mayor of Pelican Town, and regional bearer of the matrimonial seal, I now pronounce you wives!$h";
                }


            }
            if (dialogue == 3) return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5379"); //"You may kiss."
            return "WASSUUUUUUP";
        }

        public static string ShowFrameSpouse(int frame, List<NPC> NewSpouses, List<NPC> OldSpouses)
        {
            string ret = "";
            bool alt_frame = false;
            foreach(var n in NewSpouses)
            {
                alt_frame = false;
                if (/*Game1.player.spouse != n.Name && */n.Gender == 0 && frame >= 36 && frame <= 38) alt_frame = true;
                ret += "showFrame " + n.Name + " " + (alt_frame ? frame + 12 : frame) + "/";
            }
            foreach (var n in OldSpouses)
            {
                //n.Sprite.textureUsesFlippedRightForLeft = true;//0 4 24
                //n.Sprite.faceDirection(3);
                //alt_frame = false;
                //if (/*Game1.player.spouse != n.Name && */n.Gender == 0 && frame >= 36 && frame <= 38) alt_frame = true;
                int newframe = frame == 36 ? 0 : frame == 37 ? 4 : GetKissFrame(n);
                ret += "showFrame " + n.Name + " " + newframe/*(alt_frame ? frame + 12 : frame)*/ + "/";
            }
            //Modworks.Log.Alert(ret);
            return ret.Substring(0, ret.Length - 1);
        }

        public static int GetKissFrame(NPC npc)
        {
            int frame = 28;
            switch (npc.Name)
            {
                case "Maru":
                    frame = 28;
                    break;
                case "Harvey":
                    frame = 31;
                    break;
                case "Leah":
                    frame = 25;
                    break;
                case "Elliott":
                    frame = 35;
                    break;
                case "Sebastian":
                    frame = 40;
                    break;
                case "Abigail":
                    frame = 33;
                    break;
                case "Penny":
                    frame = 35;
                    break;
                case "Alex":
                    frame = 42;
                    break;
                case "Sam":
                    frame = 36;
                    break;
                case "Shane":
                    frame = 34;
                    break;
                case "Emily":
                    frame = 33;
                    break;
            }
            return frame;
        }

        public static Event getPolyWeddingEvent(Farmer farmer, List<NPC> ExistingSpouses, List<NPC> NewSpouses)
        {
            Event e = new Event("sweet/-1000 -100/farmer 27 63 2" + getCelebrationPositionsForDatables(ExistingSpouses, NewSpouses) + "Lewis 27 64 2 Marnie 26 65 0 Caroline 29 65 0 Pierre 30 65 0 Gus 31 65 0 Clint 31 66 0 " + ((farmer.friendshipData.ContainsKey("Sandy") && farmer.friendshipData["Sandy"].Points > 0) ? "Sandy 29 66 0 " : "") + "George 26 66 0 Evelyn 25 66 0 Pam 24 66 0 Jodi 32 67 0 " + ((Game1.getCharacterFromName("Kent") != null) ? "Kent 31 67 0 " : "") + "otherFarmers 29 69 0 Linus 29 67 0 Robin 25 67 0 Demetrius 26 67 0 Vincent 26 68 3 Jas 25 68 1" + ((farmer.timesReachedMineBottom > 0) ? " Dwarf 30 67 0" : "") + "/broadcastEvent/" + ShowFrameSpouse(36, NewSpouses, ExistingSpouses) + "/specificTemporarySprite wedding/viewport 27 64 true/pause 4000/speak Lewis \"" + GetWeddingDialogue(0, NewSpouses, ExistingSpouses) + "\"/faceDirection farmer 1/" + ShowFrameSpouse(37, NewSpouses, ExistingSpouses) + "/pause 500/faceDirection Lewis 0/pause 2000/speak Lewis \"" + GetWeddingDialogue(1, NewSpouses, ExistingSpouses) + "\"/move Lewis 0 1 0/playMusic none/pause 1000/showFrame Lewis 20/speak Lewis \"" + GetWeddingDialogue(2, NewSpouses, ExistingSpouses) + "\"/pause 500/speak Lewis \"" + GetWeddingDialogue(3, NewSpouses, ExistingSpouses) + "\"/pause 1000/showFrame 101/" + ShowFrameSpouse(38, NewSpouses, ExistingSpouses) + "/specificTemporarySprite heart 27 63/playSound dwop/pause 2000/specificTemporarySprite wed/warp Marnie -2000 -2000/faceDirection farmer 2/" + ShowFrameSpouse(36, NewSpouses, ExistingSpouses) + "/faceDirection Pam 1 true/faceDirection Evelyn 3 true/faceDirection Pierre 3 true/faceDirection Caroline 1 true/animate Robin false true 500 20 21 20 22/animate Demetrius false true 500 24 25 24 26/move Lewis 0 3 3 true/move Caroline 0 -1 3 false/pause 4000/faceDirection farmer 1/" + ShowFrameSpouse(37, NewSpouses, ExistingSpouses) + "/globalFade/viewport -1000 -1000/pause 1000/message \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5381") + "\"/pause 500/message \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5383") + "\"/pause 4000/waitForOtherPlayers weddingEnd/end wedding", -2, farmer);
            string[] npcNames = new string[NewSpouses.Count];
            for (int i = 0; i < NewSpouses.Count; i++) npcNames[i] = NewSpouses[i].Name;
            for(int i2 = 0; i2 < NewSpouses.Count; i2++)
            {
                //NewSpouses[i2].setTilePosition(28, 63);
                //NewSpouses[i2].faceDirection(2);
                //e.getActorByName(NewSpouses[i2].Name).setTilePosition(28, 63);
                //e.getActorByName(NewSpouses[i2].Name).faceDirection(2);
            }
            return e;
        }

        internal void RollSpouseRoom()
        {
            //only show a spouse room if it exists.
            //todo, better way to tell than a list?
            if (!PolyData.EnableSpouseRoom)
            {
                //Game1.getFarm().addSpouseOutdoorArea("???"); //should remove it
                string backup = Game1.player.spouse;
                StorePrimarySpouse();
                Utility.getHomeOfFarmer(Game1.player).showSpouseRoom();
            }
            else
            {
                string NPC = PrimarySpouse;
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
                    Modworks.Log.Debug("Changing spouse room to " + LastSpouseRoom);
                }
            }
        }

        //doesn't actually marry, it's just a ceremony to VanillaSpouse
        internal void PolyWedding(List<NPC> ExistingSpouses, List<NPC> NewSpouses)
        {
            string polyWeddingRichPresenceNames = "";
            foreach(var n in NewSpouses)
            {
                polyWeddingRichPresenceNames += n.Name + ", ";
            }
            polyWeddingRichPresenceNames = polyWeddingRichPresenceNames.Substring(0, polyWeddingRichPresenceNames.Length - 2);
            Game1.setRichPresence("wedding", polyWeddingRichPresenceNames);
            Game1.player.faceDirection(2);
            Game1.currentLocation = Game1.getLocationFromName("Town");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.player.Position = new Microsoft.Xna.Framework.Vector2(1408f, 1216f);
            Game1.player.currentLocation = Game1.currentLocation;
            Game1.getLocationFromName("Town").currentEvent = getPolyWeddingEvent(Game1.player, ExistingSpouses, NewSpouses);
            Game1.eventUp = true;
            Game1.player.CanMove = false;
            Game1.currentLocation.Map.LoadTileSheets(Game1.mapDisplayDevice);
            Game1.CurrentEvent.exitLocation = Game1.getLocationRequest("Farm");
            for(int i = 0; i < ExistingSpouses.Count; i++)
            {
                Modworks.NPCs.Warp(ExistingSpouses[i], Game1.currentLocation, 1, 1);
            }
            for (int i = 0; i < NewSpouses.Count; i++)
            {
                Modworks.NPCs.Warp(NewSpouses[i], Game1.currentLocation, 1, 1);
            }
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
            //var backup = Game1.player.spouse;
            //Game1.player.spouse = NPC;
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
            //Game1.player.spouse = backup;
        }

        public void Marry(string NPC, bool wedding = false)
        {
            SetDateable(NPC, true);
            if (Modworks.Player.GetFriendshipPoints(NPC) < 2500)
                Modworks.Player.SetFriendshipPoints(NPC, 2500);
            if (Game1.player.HouseUpgradeLevel < 1) Game1.player.HouseUpgradeLevel = 1; //prevent crash
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
            RollSpouseRoom();
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
            //delete the NPC
            var npc = Game1.getCharacterFromName(NPC);
            npc.currentLocation.characters.Remove(npc);
            npc = null;
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
