using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SunscreenMod
{
    class Reactions
    {
        /*********
        ** Accessors
        *********/
        private static IModHelper Helper => ModEntry.Instance.Helper;
        private static IMonitor Monitor => ModEntry.Instance.Monitor;
        private static ModConfig Config => ModConfig.Instance;

        private readonly static ITranslationHelper i18n = Helper.Translation;


        /*********
        ** Constants
        *********/
        private static readonly string Ah = i18n.Get("Reaction.Ah"); // "Ah!"
        private static readonly string DearMe = i18n.Get("Reaction.DearMe"); // "Dear me"
        private static readonly string DearYoba = i18n.Get("Reaction.DearYoba"); // "Dear Yoba..."
        private static readonly string DotDotDot = i18n.Get("Reaction.DotDotDot"); // "..."
        private static readonly string Eek = i18n.Get("Reaction.Eek"); // "Eek!"
        private static readonly string ExclMarks = i18n.Get("Reaction.ExclMarks"); // "!!!"
        private static readonly string Gasp = i18n.Get("Reaction.Gasp"); // "*gasp*"
        private static readonly string HaHaHa = i18n.Get("Reaction.HaHaHa"); // "Ha ha ha!"
        private static readonly string HolyCrap = i18n.Get("Reaction.HolyCrap"); // "Holy crap"
        private static readonly string HolyYoba = i18n.Get("Reaction.HolyYoba"); // "Holy Yoba!"
        private static readonly string Lobster = i18n.Get("Reaction.Lobster"); // "As red as a lobster!"
        private static readonly string LOL = i18n.Get("Reaction.LOL"); // "LOL"
        private static readonly string NiceTan = i18n.Get("Reaction.NiceTan"); // "Nice "tan"..."
        private static readonly string OhDear = i18n.Get("Reaction.OhDear"); // "Oh dear"
        private static readonly string OhMy = i18n.Get("Reaction.OhMy"); // "Oh my"
        private static readonly string OhMyGoodness = i18n.Get("Reaction.OhMyGoodness"); // "Oh my goodness!"
        private static readonly string OhNo = i18n.Get("Reaction.OhNo"); // "Oh no!"
        private static readonly string Oof = i18n.Get("Reaction.Oof"); // "Oof..."
        private static readonly string Ouch = i18n.Get("Reaction.Ouch"); // "Ouch!"
        private static readonly string PoorName = i18n.Get("Reaction.PoorName", new { name = Game1.player.Name }); // "Poor {{name}}..."
        private static readonly string SurfaceDweller = i18n.Get("Reaction.SurfaceDweller"); // "Hehe... surface dweller"
        private static readonly string SweetYoba = i18n.Get("Reaction.SweetYoba"); // "Sweet Yoba!"
        private static readonly string ThatsGottaHurt = i18n.Get("Reaction.ThatsGottaHurt"); // "That's gotta hurt"
        private static readonly string Tomato = i18n.Get("Reaction.Tomato"); // "A tomato!?"
        private static readonly string TooMuchSun = i18n.Get("Reaction.TooMuchSun"); // "Too much sun?"
        private static readonly string Wow = i18n.Get("Reaction.Wow"); // "Wow!"
        private static readonly string WowName = i18n.Get("Reaction.WowName", new { name = Game1.player.Name }); // "Wow, {{name}}..."
        private static readonly string WTF = i18n.Get("Reaction.WTF"); // "What the Ferngill?!"
        private static readonly string Yikes = i18n.Get("Reaction.Yikes"); // "Yikes"

        /// <summary>Dictionary of string arrays containing possible reactions from each vanilla villager.</summary>
        private static Dictionary<string, string[]> VillagerReactions { get; } = new Dictionary<string, string[]>()
        {
            { "Abigail", new string[] {
                ThatsGottaHurt, Yikes, WowName, LOL, WTF
            } },
            { "Alex", new string[] {
                ThatsGottaHurt, LOL, NiceTan, Oof
            } },
            { "Caroline", new string[] {
                OhNo, OhMyGoodness, OhMyGoodness, PoorName
            } },
            { "Clint", new string[] {
                Yikes, Wow, Oof
            } },
            { "Demetrius", new string[] {
                SweetYoba, HolyYoba, Tomato, ExclMarks
            } },
            { "Dwarf", new string[] {
                ExclMarks, Ah, Ah, DotDotDot, SurfaceDweller, SurfaceDweller
            } },
            { "Elliott", new string[] {
                OhMy, OhMy, TooMuchSun, PoorName
            } },
            { "Emily", new string[] {
                OhNo, OhNo, Yikes, HolyYoba, PoorName
            } },
            { "Evelyn", new string[] {
                Gasp, DearMe, DearMe, OhNo
            } },
            { "George", new string[] {
                Ouch, Ouch, ExclMarks, Wow
            } },
            { "Gil", new string[] {
                ExclMarks, DotDotDot, ThatsGottaHurt, Oof
            } },
            { "Gus", new string[] {
                TooMuchSun, TooMuchSun, PoorName, ThatsGottaHurt, Ouch
            } },
            { "Haley", new string[] {
                Eek, LOL, LOL, ExclMarks, Gasp
            } },
            { "Harvey", new string[] {
                TooMuchSun, OhNo, OhDear, OhDear, Gasp
            } },
            { "Jas", new string[] {
                Eek, Eek, OhNo, Wow, Ah
            } },
            { "Jodi", new string[] {
                OhMyGoodness, DearYoba, OhNo, Gasp
            } },
            { "Kent", new string[] {
                Ouch, Oof, Oof, ExclMarks, DotDotDot
            } },
            { "Krobus", new string[] {
                HolyYoba, Ah, Ah, WowName
            } },
            { "Leah", new string[] {
                WTF, WTF, Yikes, Ouch, HolyCrap
            } },
            { "Lewis", new string[] {
                Gasp, OhDear, OhDear, Ouch, ExclMarks
            } },
            { "Linus", new string[] {
                PoorName, PoorName, OhDear, DotDotDot, Ah
            } },
            { "Marnie", new string[] {
                OhNo, Gasp, Gasp, DearMe, TooMuchSun, Ah
            } },
            { "Maru", new string[] {
                WowName, WowName, LOL, ThatsGottaHurt, ExclMarks
            } },
            { "Mister Qi", new string[] {
                DotDotDot, ExclMarks, WowName
            } },
            { "Pam", new string[] {
                SweetYoba, ThatsGottaHurt, ThatsGottaHurt, WowName, Oof
            } },
            { "Penny", new string[] {
                OhMyGoodness, OhNo, OhDear, Eek, TooMuchSun
            } },
            { "Pierre", new string[] {
                Yikes, Yikes, Ouch, WTF, ExclMarks
            } },
            { "Robin", new string[] {
                OhNo, OhNo, Gasp, Yikes, WowName
            } },
            { "Sam", new string[] {
                LOL, ThatsGottaHurt, WowName, HolyCrap, HolyCrap
            } },
            { "Sandy", new string[] {
                OhNo, Ouch, TooMuchSun, TooMuchSun, OhMyGoodness
            } },
            { "Sebastian", new string[] {
                NiceTan, NiceTan, ThatsGottaHurt, Oof, Wow
            } },
            { "Shane", new string[] {
                Yikes, Yikes, WowName, WTF, DotDotDot
            } },
            { "Vincent", new string[] {
                Ah, Wow, ExclMarks, HaHaHa, HaHaHa
            } },
            { "Willy", new string[] {
                Oof, PoorName, Lobster, Lobster, Ouch
            } },
            { "Wizard", new string[] {
                ExclMarks, OhMy, Ah, Ah, OhDear
            } }
        };

        /// <summary>String array containing possible reactions from each NPC children.</summary>
        private static readonly string[] ChildReactions = new string[] {
            Eek, Wow, ExclMarks, Gasp, Ah, DotDotDot, HaHaHa
        };

        /// <summary>String arrays containing possible reactions from a generic or unknown villager.</summary>
        private static readonly string[] GenericReactions = new string[] {
            Ouch, Wow, ExclMarks, OhNo, Ah, DotDotDot, TooMuchSun
        };

        /// <summary>Maximum tile distance that villagers can see and react to sunburn.</summary>
        private const int ReactRadius = 12;


        /*********
        ** Fields
        *********/
        /// <summary>Seeded random instance.</summary>
        private readonly Random rnd = new Random();

        /// <summary>List of NPCs who have already reacted since last player warp.</summary>
        private readonly List<string> NPCsWhoReacted = new List<string>();


        /*********
        ** Private methods
        *********/
        /// <summary>Returns a random object from an array of arbitrary length.</summary>
        private T PickOne<T>(T[] array)
        {
            return array[rnd.Next(0, array.Length)];
        }

        /// <summary>Checks whether a given NPC has already reacted to player sunburn.</summary>
        /// <param name="npc">The NPC to check.</param>
        /// <returns>true if already reacted, false if not</returns>
        private bool HasReacted(NPC npc)
        {
            return NPCsWhoReacted.Contains(npc.Name);
        }

        /// <summary>Checks whether the player is in view of a given NPC.</summary>
        /// <param name="playerX">Player X tile position</param>
        /// <param name="playerY">Player Y tile position</param>
        /// <param name="npcX">NPC X tile position</param>
        /// <param name="npcY">NPC Y tile position</param>
        /// <param name="facing">NPC facing direction: 0 up - 1 right - 2 down - 3 left</param>
        /// <returns>true if facing player, false if not</returns>
        private bool CanSeePlayer(int playerX, int playerY, int npcX, int npcY, int facing)
        {
            if (facing == 0 && playerY >= npcY || //Looking up, player is further down
                facing == 1 && playerX <= npcX || //Looking right, player is further left
                facing == 2 && playerY <= npcY || //Looking down, player is further up
                facing == 3 && playerX >= npcX) //Looking left, player is further right
            {
                return false;
            }
            return true;
        }

        /// <summary>Makes an NPC jump and give a speech bubble reaction.</summary>
        /// <param name="npc">The NPC to perform a reaction</param>
        private void DoReact(NPC npc)
        {
            string reaction = GetReaction(npc);
            npc.jump(JumpHeight());
            npc.showTextAboveHead(reaction);
            Monitor.Log($"NPC {npc.Name} reacted: \"{reaction}\"", Config.DebugMode ? LogLevel.Debug : LogLevel.Trace);
        }

        /// <summary>Returns a random jump height value between 4 and 8.</summary>
        /// <returns>Jump height</returns>
        private float JumpHeight()
        {
            return (float)(4 + 4 * rnd.NextDouble()); //Range from 4 to 8
        }

        /// <summary>Returns a random sunburn reaction text for a given villager</summary>
        /// <param name="npc">The NPC to get an appropriate reaction for</param>
        /// <returns>Reaction text</returns>
        private string GetReaction(NPC npc)
        {
            if (npc is Child)
            {
                return PickOne(ChildReactions);
            }
            else if (npc.Name == "Krobus" && SDate.Now().DayOfWeek.ToString() == "Friday")
            {
                return rnd.NextDouble() > 0.5 ? DotDotDot : ExclMarks; //Krobus doesn't speak on Fridays
            }
            else if (VillagerReactions.Keys.Contains(npc.Name))
            {
                return PickOne(VillagerReactions[npc.Name]);
            }
            return PickOne(GenericReactions);
        }


        /*********
        ** Public methods
        *********/
        /// <summary>Clear the stored list of NPCs who have already reacted.</summary>
        public void ClearReacts()
        {
            NPCsWhoReacted.Clear();
        }

        /// <summary>Causes any nearby NPCs who can see the player to perform sunburn reactions.</summary>
        public void NearbyNPCsReact()
        {
            int playerX = Game1.player.getTileX();
            int playerY = Game1.player.getTileY();
            Vector2 playerTile = Game1.player.getTileLocation();

            foreach (NPC npc in Game1.player.currentLocation.getCharacters())
            {
                if ((npc is Child || npc.isVillager()) && //Don't want slimes or pets to react!
                    !HasReacted(npc) && //Has not already reacted recently
                    (playerTile - npc.getTileLocation()).Length() <= ReactRadius && //NPC is in distance range to react
                    CanSeePlayer(playerX, playerY, npc.getTileX(), npc.getTileY(), npc.FacingDirection)) //Not facing away from the player
                {
                    DelayedAction.functionAfterDelay(() => DoReact(npc), rnd.Next(100, 500)); //Stagger reactions a bit when there's many NPCs
                    NPCsWhoReacted.Add(npc.Name);
                }
            }
        }
    }
}