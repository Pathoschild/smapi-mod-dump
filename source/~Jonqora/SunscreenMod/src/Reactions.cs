using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunscreenMod
{
    class Reactions
    {
        //ACCESSORS
        protected static IModHelper Helper => ModEntry.Instance.Helper;
        protected static IMonitor Monitor => ModEntry.Instance.Monitor;

        protected static ITranslationHelper i18n = Helper.Translation;

        //CONSTANTS
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
        private static readonly string WowName = i18n.Get("Reaction.WowName", new {name = Game1.player.Name} ); // "Wow, {{name}}..."
        private static readonly string WTF = i18n.Get("Reaction.WTF"); // "What the Ferngill?!"
        private static readonly string Yikes = i18n.Get("Reaction.Yikes"); // "Yikes"

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

        private static readonly string[] ChildReactions = new string[] {
            Eek, Wow, ExclMarks, Gasp, Ah, DotDotDot, HaHaHa
        }; 
        
        private static readonly string[] GenericReactions = new string[] {
            Ouch, Wow, ExclMarks, OhNo, Ah, DotDotDot, TooMuchSun
        };

        private static readonly int ReactRadius = 12;

        //FIELDS
        private readonly Random rnd = new Random();

        private readonly List<string> NPCsWhoReacted = new List<string>();

        //PRIVATE METHODS
        private T PickOne<T>(T[] array)
        {
            return array[rnd.Next(0, array.Length)];
        }

        private bool HasReacted(NPC npc)
        {
            return NPCsWhoReacted.Contains(npc.Name);
        }

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

        private void DoReact(NPC npc)
        {
            npc.jump(JumpHeight());
            npc.showTextAboveHead(GetReaction(npc));
        }

        private float JumpHeight()
        {
            return (float)(4 + 4 * rnd.NextDouble()); //Range from 4 to 8
        }

        //PUBLIC METHODS
        public string GetReaction(NPC npc)
        {
            if (npc.Name == "Krobus" && SDate.Now().DayOfWeek.ToString() == "Friday")
            {
                return rnd.NextDouble() > 0.5 ? DotDotDot : ExclMarks; //Krobus doesn't speak on Fridays
            }
            else if (npc is Child)
            {
                return PickOne(ChildReactions);
            }
            else if (VillagerReactions.Keys.Contains(npc.Name))
            {
                return PickOne(VillagerReactions[npc.Name]);
            }
            else
            {
                return PickOne(GenericReactions);
            }
        }

        public void ClearReacts()
        {
            NPCsWhoReacted.Clear();
        }

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
