/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeLove
{
    /// <summary>The mod entry point.</summary>
    public class Misc
    {
        private static Dictionary<string, int> topOfHeadOffsets = new Dictionary<string, int>();

        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static ModConfig Config;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, ModConfig config, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;
            Config = config;
        }

        public static Dictionary<string, NPC> GetSpouses(Farmer farmer, int all)
        {
            Dictionary<string, NPC> spouses = new Dictionary<string, NPC>();
            if (all < 0)
            {
                NPC ospouse = farmer.getSpouse();
                if (ospouse != null)
                {
                    spouses.Add(ospouse.Name, ospouse);
                }
            }
            foreach (string friend in farmer.friendshipData.Keys)
            {
                if (Game1.getCharacterFromName(friend, true) != null && farmer.friendshipData[friend].IsMarried() && (all > 0 || friend != farmer.spouse))
                {
                    spouses.Add(friend, Game1.getCharacterFromName(friend, true));
                }
            }

            return spouses;
        }

        internal static void ResetDivorces()
        {
            if (!ModEntry.config.PreventHostileDivorces)
                return;
            List<string> friends = Game1.player.friendshipData.Keys.ToList();
            foreach(string f in friends)
            {
                if(Game1.player.friendshipData[f].Status == FriendshipStatus.Divorced)
                {
                    Monitor.Log($"Wiping divorce for {f}");
                    if (Game1.player.friendshipData[f].Points < 8 * 250)
                        Game1.player.friendshipData[f].Status = FriendshipStatus.Friendly;
                    else
                        Game1.player.friendshipData[f].Status = FriendshipStatus.Dating;
                }
            }
        }

        public static Dictionary<string, Dictionary<string, string>> relationships = new Dictionary<string, Dictionary<string, string>>();

        public static void SetNPCRelations()
        {
            relationships.Clear();
            Dictionary<string, string> NPCDispositions = Helper.Content.Load<Dictionary<string, string>>("Data\\NPCDispositions", ContentSource.GameContent);
            foreach(KeyValuePair<string,string> kvp in NPCDispositions)
            {
                string[] relations = kvp.Value.Split('/')[9].Split(' ');
                if (relations.Length > 0)
                {
                    relationships.Add(kvp.Key, new Dictionary<string, string>());
                    for (int i = 0; i < relations.Length; i += 2)
                    {
                        try
                        {
                            relationships[kvp.Key].Add(relations[i], relations[i + 1].Replace("'", ""));
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        public static string GetRandomSpouse(Farmer f)
        {
            var spouses = GetSpouses(f, 1);
            if (spouses.Count == 0)
                return null;
            ShuffleDic(ref spouses);
            return spouses.Keys.ToArray()[0];
        }

        public static void PlaceSpousesInFarmhouse(FarmHouse farmHouse)
        {
            Farmer farmer = farmHouse.owner;

            if (farmer == null)
                return;

            List<NPC> allSpouses = GetSpouses(farmer, 1).Values.ToList();

            if (allSpouses.Count == 0)
            {
                Monitor.Log("no spouses");
                return;
            }

            ShuffleList(ref allSpouses);

            List<string> bedSpouses = new List<string>();
            string kitchenSpouse = null;

            foreach (NPC spouse in allSpouses)
            {

                int type = ModEntry.myRand.Next(0, 100);

                Monitor.Log($"spouse rand {type}, bed: {ModEntry.config.PercentChanceForSpouseInBed} kitchen {ModEntry.config.PercentChanceForSpouseInKitchen}");
                
                if(type < ModEntry.config.PercentChanceForSpouseInBed)
                {
                    if (bedSpouses.Count < 1 && (ModEntry.config.RoommateRomance || !farmer.friendshipData[spouse.Name].IsRoommate()) && HasSleepingAnimation(spouse.Name))
                    {
                        Monitor.Log("made bed spouse: " + spouse.Name);
                        bedSpouses.Add(spouse.Name);
                    }

                }
                else if(type < ModEntry.config.PercentChanceForSpouseInBed + ModEntry.config.PercentChanceForSpouseInKitchen)
                {
                    if (kitchenSpouse == null)
                    {
                        Monitor.Log("made kitchen spouse: " + spouse.Name);
                        kitchenSpouse = spouse.Name;
                    }
                }
                else if(type < ModEntry.config.PercentChanceForSpouseInBed + ModEntry.config.PercentChanceForSpouseInKitchen + ModEntry.config.PercentChanceForSpouseAtPatio)
                {
                    if (!Game1.isRaining && !Game1.IsWinter && !spouse.Name.Equals("Krobus") && spouse.Schedule == null)
                    {
                        Monitor.Log("made patio spouse: " + spouse.Name);
                        spouse.setUpForOutdoorPatioActivity();
                    }
                }
            }

            foreach (NPC j in allSpouses) 
            { 
                Monitor.Log("placing " + j.Name);

                Point spouseRoomSpot = new Point(-1, -1); 
                
                if(Integrations.customSpouseRoomsAPI != null)
                {
                    spouseRoomSpot = Integrations.customSpouseRoomsAPI.GetSpouseTile(j);
                    Monitor.Log($"Got custom spouse spot {spouseRoomSpot}");
                }
                else if(farmer.spouse == j.Name)
                {
                    spouseRoomSpot = farmHouse.GetSpouseRoomSpot();
                }

                if (!farmHouse.Equals(j.currentLocation))
                {
                    Monitor.Log("not in farm house");
                    continue;
                }

                Monitor.Log("in farm house");
                j.shouldPlaySpousePatioAnimation.Value = false;

                if (bedSpouses.Count > 0 && bedSpouses.Contains(j.Name))
                {
                    Monitor.Log($"putting {j.Name} in bed");
                    j.position.Value = GetSpouseBedPosition(farmHouse, j.Name);
                }
                else if (kitchenSpouse == j.Name && !IsTileOccupied(farmHouse, farmHouse.getKitchenStandingSpot(), j.Name))
                {
                    Monitor.Log($"{j.Name} is in kitchen");

                    j.setTilePosition(farmHouse.getKitchenStandingSpot());
                    j.setRandomAfternoonMarriageDialogue(Game1.timeOfDay, farmHouse, false);
                }
                else if (spouseRoomSpot.X > -1 && !IsTileOccupied(farmHouse, spouseRoomSpot, j.Name))
                {
                    Monitor.Log($"{j.Name} is in spouse room");
                    j.setTilePosition(spouseRoomSpot);
                    j.setSpouseRoomMarriageDialogue();
                }
                else 
                { 
                    j.setTilePosition(farmHouse.getRandomOpenPointInHouse(ModEntry.myRand));
                    j.faceDirection(ModEntry.myRand.Next(0, 4));
                    Monitor.Log($"{j.Name} spouse random loc {j.getTileLocationPoint()}");
                    j.setRandomAfternoonMarriageDialogue(Game1.timeOfDay, farmHouse, false);
                }
            }
        }

        private static bool IsTileOccupied(GameLocation location, Point tileLocation, string characterToIgnore)
        {
            Rectangle tileLocationRect = new Rectangle(tileLocation.X * 64 + 1, tileLocation.Y * 64 + 1, 62, 62);

            for (int i = 0; i < location.characters.Count; i++)
            {
                if (location.characters[i] != null && !location.characters[i].Name.Equals(characterToIgnore) && location.characters[i].GetBoundingBox().Intersects(tileLocationRect))
                {
                    Monitor.Log($"Tile {tileLocation} is occupied by {location.characters[i].Name}");

                    return true;
                }
            }
            return false;
        }

        public static Point GetSpouseBedEndPoint(FarmHouse fh, string name)
        {
            var bedSpouses = GetBedSpouses(fh);

            Point bedStart = fh.GetSpouseBed().GetBedSpot();
            int bedWidth = GetBedWidth();
            bool up = fh.upgradeLevel > 1;

            int x = (int)(bedSpouses.IndexOf(name) / (float)(bedSpouses.Count) * (bedWidth - 1));
            return new Point(bedStart.X + x, bedStart.Y);
        }
        public static Vector2 GetSpouseBedPosition(FarmHouse fh, string name)
        {
            var allBedmates = GetBedSpouses(fh);

            Point bedStart = GetBedStart(fh);
            int x = 64 + (int)((allBedmates.IndexOf(name) + 1) / (float)(allBedmates.Count + 1) * (GetBedWidth() - 1) * 64);
            return new Vector2(bedStart.X * 64 + x, bedStart.Y * 64 + ModEntry.bedSleepOffset - (GetTopOfHeadSleepOffset(name) * 4));
        }

        public static Point GetBedStart(FarmHouse fh)
        {
            if (fh?.GetSpouseBed()?.GetBedSpot() == null)
                return Point.Zero;
            return new Point(fh.GetSpouseBed().GetBedSpot().X - 1, fh.GetSpouseBed().GetBedSpot().Y - 1);
        }

        public static bool IsInBed(FarmHouse fh, Rectangle box)
        {
            int bedWidth = GetBedWidth();
            Point bedStart = GetBedStart(fh);
            Rectangle bed = new Rectangle(bedStart.X * 64, bedStart.Y * 64, bedWidth * 64, 3 * 64);

            if (box.Intersects(bed))
            {
                return true;
            }
            return false;
        }
        public static int GetBedWidth()
        {
            if (Integrations.bedTweaksAPI != null)
            {
                return Integrations.bedTweaksAPI.GetBedWidth();
            }
            else
            {
                return 3;
            }
        }
        public static List<string> GetBedSpouses(FarmHouse fh)
        {
            if (ModEntry.config.RoommateRomance)
                return GetSpouses(fh.owner, 1).Keys.ToList();

            return GetSpouses(fh.owner, 1).Keys.ToList().FindAll(s => !fh.owner.friendshipData[s].RoommateMarriage);
        }

        public static List<string> ReorderSpousesForSleeping(List<string> sleepSpouses)
        {
            List<string> configSpouses = ModEntry.config.SpouseSleepOrder.Split(',').Where(s => s.Length > 0).ToList();
            List<string> spouses = new List<string>();
            foreach (string s in configSpouses)
            {
                if (sleepSpouses.Contains(s))
                    spouses.Add(s);
            }

            foreach (string s in sleepSpouses)
            {
                if (!spouses.Contains(s))
                {
                    spouses.Add(s);
                    configSpouses.Add(s);
                }
            }
            string configString = string.Join(",", configSpouses);
            if (configString != ModEntry.config.SpouseSleepOrder)
            {
                ModEntry.config.SpouseSleepOrder = configString;
                Helper.WriteConfig(ModEntry.config);
            }

            return spouses;
        }


        public static int GetTopOfHeadSleepOffset(string name)
        {
            if (topOfHeadOffsets.ContainsKey(name))
            {
                return topOfHeadOffsets[name];
            }
            //Monitor.Log($"dont yet have offset for {name}");
            int top = 0;

            if (name == "Krobus")
                return 8;

            Texture2D tex = Helper.Content.Load<Texture2D>($"Characters/{name}", ContentSource.GameContent);

            int sleepidx;
            string sleepAnim = SleepAnimation(name);
            if (sleepAnim == null || !int.TryParse(sleepAnim.Split('/')[0], out sleepidx))
                sleepidx = 8;

            if ((sleepidx * 16) / 64 * 32 >= tex.Height)
            {
                sleepidx = 8;
            }


            Color[] colors = new Color[tex.Width * tex.Height];
            tex.GetData(colors);

            //Monitor.Log($"sleep index for {name} {sleepidx}");

            int startx = (sleepidx * 16) % 64;
            int starty = (sleepidx * 16) / 64 * 32;

            //Monitor.Log($"start {startx},{starty}");

            for (int i = 0; i < 16 * 32; i++)
            {
                int idx = startx + (i % 16) + (starty + i / 16) * 64;
                if (idx >= colors.Length)
                {
                    Monitor.Log($"Sleep pos couldn't get pixel at {startx + i % 16},{starty + i / 16} ");
                    break;
                }
                Color c = colors[idx];
                if (c != Color.Transparent)
                {
                    top = i / 16;
                    break;
                }
            }
            topOfHeadOffsets.Add(name, top);
            return top;
        }


        public static bool HasSleepingAnimation(string name)
        {
            string sleepAnim = SleepAnimation(name);
            if (sleepAnim == null || !sleepAnim.Contains("/"))
                return false;

            if (!int.TryParse(sleepAnim.Split('/')[0], out int sleepidx))
                return false;

            Texture2D tex = Helper.Content.Load<Texture2D>($"Characters/{name}", ContentSource.GameContent);
            //Monitor.Log($"tex height for {name}: {tex.Height}");

            if (sleepidx / 4 * 32 >= tex.Height)
            {
                return false;
            }
            return true;
        }

        private static string SleepAnimation(string name)
        {
            string anim = null;
            if (Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions").ContainsKey(name.ToLower() + "_sleep"))
            {
                anim = Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions")[name.ToLower() + "_sleep"];
            }
            else if (Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions").ContainsKey(name + "_Sleep"))
            {
                anim = Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions")[name + "_Sleep"];
            }
            return anim;
        }


        internal static void NPCDoAnimation(NPC npc, string npcAnimation)
        {
            Dictionary<string, string> animationDescriptions = Helper.Content.Load<Dictionary<string, string>>("Data\\animationDescriptions", ContentSource.GameContent);
            if (!animationDescriptions.ContainsKey(npcAnimation))
                return;

            string[] rawData = animationDescriptions[npcAnimation].Split('/');
            var animFrames = Utility.parseStringToIntArray(rawData[1], ' ');
 
            List<FarmerSprite.AnimationFrame> anim = new List<FarmerSprite.AnimationFrame>();
            for (int i = 0; i < animFrames.Length; i++)
            {
                    anim.Add(new FarmerSprite.AnimationFrame(animFrames[i], 100, 0, false, false, null, false, 0));
            }
            Monitor.Log($"playing animation {npcAnimation} for {npc.Name}");
            npc.Sprite.setCurrentAnimation(anim);
        }

        public static void ResetSpouses(Farmer f)
        {
            Dictionary<string, NPC> spouses = GetSpouses(f,1);
            if (f.spouse == null)
            {
                if(spouses.Count > 0)
                {
                    Monitor.Log("No official spouse, setting official spouse to: " + spouses.First().Key);
                    f.spouse = spouses.First().Key;
                }
            }

            foreach (string name in f.friendshipData.Keys)
            {
                if (f.friendshipData[name].IsEngaged())
                {
                    Monitor.Log($"{f.Name} is engaged to: {name} {f.friendshipData[name].CountdownToWedding} days until wedding");
                    if (f.friendshipData[name].WeddingDate.TotalDays < new WorldDate(Game1.Date).TotalDays)
                    {
                        Monitor.Log("invalid engagement: " + name);
                        f.friendshipData[name].WeddingDate.TotalDays = new WorldDate(Game1.Date).TotalDays + 1;
                    }
                    if(f.spouse != name)
                    {
                        Monitor.Log("setting spouse to engagee: " + name);
                        f.spouse = name;
                    }
                }
                if (f.friendshipData[name].IsMarried() && f.spouse != name)
                {
                    //Monitor.Log($"{f.Name} is married to: {name}");
                    if (f.spouse != null && f.friendshipData[f.spouse] != null && !f.friendshipData[f.spouse].IsMarried() && !f.friendshipData[f.spouse].IsEngaged())
                    {
                        Monitor.Log("invalid ospouse, setting ospouse to " + name);
                        f.spouse = name;
                    }
                    if (f.spouse == null)
                    {
                        Monitor.Log("null ospouse, setting ospouse to " + name);
                        f.spouse = name;
                    }
                }
            }
        }
        public static void SetAllNPCsDatable()
        {
            if (!ModEntry.config.RomanceAllVillagers)
                return;
            Farmer f = Game1.player;
            if (f == null)
            {
                return;
            }
            foreach (string friend in f.friendshipData.Keys)
            {
                NPC npc = Game1.getCharacterFromName(friend);
                if (npc != null && !npc.datable.Value && npc is NPC && !(npc is Child) && (npc.Age == 0 || npc.Age == 1))
                {
                    Monitor.Log($"Making {npc.Name} datable.");
                    npc.datable.Value = true;
                }
            }
        }

 
        public static void ShuffleList<T>(ref List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ModEntry.myRand.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static void ShuffleDic<T1,T2>(ref Dictionary<T1,T2> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ModEntry.myRand.Next(n + 1);
                var value = list[list.Keys.ToArray()[k]];
                list[list.Keys.ToArray()[k]] = list[list.Keys.ToArray()[n]];
                list[list.Keys.ToArray()[n]] = value;
            }
        }
    }
}