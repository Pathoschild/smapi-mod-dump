/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/paritee/Paritee.StardewValley.Frameworks
**
*************************************************/

using StardewValley;
using StardewValley.Characters;
using System;

namespace TreatYourAnimals.Framework
{
    class HorseTreat : CharacterTreat
    {
        private const int FRIENDSHIP_POINTS_MAX = 2000; // 2000 = Bouquet; 2500 = Sea Amulet
        private const int FRIENDSHIP_POINTS_STEP = 12; // same percentage as the other animals

        private Horse Horse;

        public HorseTreat(Horse horse, ModConfig config) : base(config)
        {
            this.Horse = horse;
        }

        public override void GiveTreat()
        {
            this.ReduceActiveItemByOne();
            this.ChangeFriendship();
            this.DoEmote();
            this.PlaySound();
        }

        public override void ChangeFriendship(int points = HorseTreat.FRIENDSHIP_POINTS_STEP)
        {
            // Block all friendship for the horse
            if (!this.Config.EnableHorseFriendship)
            {
                return;
            }

            // WARNING:
            // - Completing socialize quests will also boost your Horse's friendship poiints
            // - Counts towards friendship achievements
            // - Affects percentage that contributes towards percentGameComplete
            // - Will not show up on the social menu (@TODO: make this happen?)

            // A horse's name gets appended with spaces if it matches an already existing NPC name
            // Horse.cs:nameHorse(string name)
            // if (allCharacter.isVillager() && allCharacter.Name.Equals(name))
            //    name += " ";
            if (!Game1.player.friendshipData.ContainsKey(this.Horse.Name))
            {
                Game1.player.friendshipData.Add(this.Horse.Name, new Friendship());
            }

            // Treat the horse as if it's a social NPC
            Game1.player.changeFriendship(points, this.Horse);

            // Chance to show the "pet loves you" global message
            this.AttemptToExpressLove(this.Horse, Game1.player.getFriendshipLevelForNPC(this.Horse.Name), HorseTreat.FRIENDSHIP_POINTS_MAX, "horseLoveMessage");
        }

        public void RefuseTreat(bool penalty)
        {
            int pointsLoss = penalty ? -1 * (HorseTreat.FRIENDSHIP_POINTS_STEP / 2) : 0;

            base.RefuseTreat(this.Horse, pointsLoss);
        }

        public override void DoEmote()
        {
            base.DoEmote(this.Horse);
        }

        public override void PlaySound()
        {
            Game1.playSound("grunt");
        }
    }
}
