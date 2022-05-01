/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalEffects
{
    public class Spot
    {
        private readonly string NPC;
        private readonly string Location;
        private readonly string LocationType;
        private readonly int X;
        private readonly int Y;
        private readonly int PercentChance;
        private readonly string ForSVE;

        private static List<Spot> Spots;
        private static IModHelper helper;

        private static readonly List<string> Labels = new() {"Panties", "Delicates", "Underpants", "Underwear"};

        private static void Roll(object sender, EventArgs e)
        {
            string hint = null;
            var isSVE = helper.ModRegistry.IsLoaded("FlashShifter.SVECode");
            foreach (var ss in Spots)
            {
                Modworks.Log.Trace($"Checking spot {ss.Location} for {ss.NPC}");
                switch (isSVE)
                {
                    case true when ss.ForSVE == "no":
                    case false when ss.ForSVE == "only":
                        continue;
                }
                
                //despawn old items
                var l = Game1.getLocationFromName(ss.Location);
                var pos = new Vector2(ss.X, ss.Y);
                if (l.objects.ContainsKey(pos))
                {
                    //if there's one of our own items here, remove it
                    var o1 = l.objects[pos];
                    if (o1 is {displayName: { }})
                    {
                        foreach (var p in Labels.Where(p => o1.DisplayName.Contains(p)))
                        {
                            Modworks.Log.Trace($"    Clearing {o1.DisplayName} from {ss.Location}");
                            l.objects.Remove(pos);
                        }
                    }
                }

                if (ss.NPC == "Kent" && Game1.year < 2) continue;

                // see if we want to spawn item
                if (l.objects.ContainsKey(pos)) continue;
                if (NPCConfig.GetNPC(ss.NPC).InternalName == "{Unknown NPC}") continue;
                if (!NPCConfig.GetNPC(ss.NPC).Enabled) continue;
                
                // are we in luck?
                var rnd = (int) (Modworks.RNG.Next(100) * (1f - Player.GetLuckFactorFloat()));
                var chance = ss.PercentChance;
                if (rnd > chance) continue;
                
                // spawn item
                var npcData = NPCConfig.Data[ss.NPC];

                var api = helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
                if (api is null)
                {
                    Modworks.Log.Warn("Could not get JsonAssets API");
                    continue;
                }

                var undies = Undies(npcData);
                var name = $"{ss.NPC}'s {undies}";
                var iid = api.GetObjectId(name);

                if (iid == -1) continue;

                Modworks.Log.Trace($"    Spawning forage item {name} for {ss.NPC}: {ss.Location} {ss.X} {ss.Y} [{rnd}/{chance}]");

                var i = (StardewValley.Object) StardewValley.Objects.ObjectFactory.getItemFromDescription(0, iid, 1);
                i.IsSpawnedObject = true;
                i.ParentSheetIndex = iid;
                l.objects.Add(pos, i);

                
                // drop a hint about what's spawned
                if (hint is not null) continue;
                if (!Mod.Config.GiveHints) continue;
                if (Game1.player.getFriendshipHeartLevelForNPC(NPCConfig.GetNPC(ss.NPC).InternalName) <= 0) continue;
                rnd = (int) (Modworks.RNG.Next(100) * (1f - Player.GetLuckFactorFloat()));
                if (rnd > 30) continue;

                switch (ss.LocationType)
                {
                    case "Home":
                        var gender = npcData.HasMaleItems() ? "his" : "her";
                        hint = $"Check out what {ss.NPC} left around {gender} house... or someone's house";
                        break;
                    case "Bath":
                        if (!Game1.isLocationAccessible("BathHouse_Entry")) continue;
                        hint = $"{ss.NPC} has been at the bathhouse";
                        break;
                    case "Other":
                        hint = $"{ss.NPC}'s {undies.ToLower()} could be anywhere, good luck!";
                        break;
                }

                SendMessage(hint);

                // Modworks.Log.Trace($"Spawning forage item for {ss.NPC}: " + sid + " at " + ss.Location + " (" + ss.X + ", " + ss.Y + ")" + "Strike: " + strikepoint + ", Chance: " + chance);
            }
        }

        private static string Undies(ConfigNPC npcData)
        {
            string gender;
            var variant = Game1.random.Next(2);
            if (!Mod.Config.BothGenders)
            {
                gender = npcData.HasMaleItems() ? "m" : "f";
            }
            else
            {
                gender = Game1.random.Next(2) == 1 ? "m" : "f";
            }

            string undies;
            if (gender == "m")
            {
                undies = variant == 1 ? "Underwear" : "Underpants";
            }
            else
            {
                undies = variant == 1 ? "Panties" : "Delicates";
            }

            return undies;
        }

        public static void SendMessage(string msg) {
            Game1.addHUDMessage(new HUDMessage(msg, 3) {
                noIcon = true,
                timeLeft = HUDMessage.defaultTime
            });

            // try {
            //     var multiplayer = Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            //     multiplayer.broadcastGlobalMessage("Strings\\StringsFromCSFiles:"+msg);
            // }
            // catch (InvalidOperationException) {
            //     BetterJunimos.SMonitor.Log($"SendMessage: multiplayer unavailable", LogLevel.Error);
            // }
        }

        public static void Setup(IModHelper im_helper)
        {
            helper = im_helper;
            helper.Events.GameLoop.DayStarted += Roll;
            Spots = new List<Spot>();

            foreach (var cl in ConfigLocations.Data)
            {
                var enabled = !(cl.LocationGender == "Female" && !NPCConfig.GetNPC(cl.NPC).IsFemale);
                if (cl.LocationGender == "Male" && NPCConfig.GetNPC(cl.NPC).IsFemale) enabled = false;
                switch (cl.LocationType)
                {
                    case "Home" when !NPCConfig.GetNPC(cl.NPC).HomeSpots:
                    case "Bath" when !NPCConfig.GetNPC(cl.NPC).BathSpots:
                    case "Other" when !NPCConfig.GetNPC(cl.NPC).OtherSpots:
                        enabled = false;
                        break;
                }

                if (enabled) Spots.Add(new Spot(cl.NPC, cl.Location, cl.LocationType, cl.X, cl.Y, cl.PercentChance(), cl.ForSVE));
            }
        }

        private Spot(string npc, string loc, string locType, int x, int y, int chance, string forSVE)
        {
            NPC = npc;
            Location = loc;
            LocationType = locType;
            X = x;
            Y = y;
            PercentChance = chance;
            ForSVE = forSVE;
        }
    }
}