/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/paritee/Paritee.StardewValley.Frameworks
**
*************************************************/

using Netcode;
using StardewValley;
using System;

namespace TreatYourAnimals.Framework
{
    class FarmAnimalTreat : CharacterTreat
    {
        private const int FRIENDSHIP_POINTS_MAX = 1000;
        private const int FRIENDSHIP_POINTS_STEP = 6; // matches water bowl for pets
        private const int EXPERIENCE_POINTS = 5; // matches pet() value

        private FarmAnimal FarmAnimal;

        public FarmAnimalTreat(FarmAnimal farmAnimal, ModConfig config) : base(config)
        {
            this.FarmAnimal = farmAnimal;
        }

        public override void GiveTreat()
        {
            this.ReduceActiveItemByOne();
            this.ChangeFriendship();

            // Extras
            this.ChangeFullness();
            this.ChangeHappiness();
            this.GainExperience();

            this.DoEmote();
            this.PlaySound();
        }

        private void ChangeHappiness()
        {
            // same logic used in pet()
            if (this.ReceiveProfessionBoost())
            {
                this.FarmAnimal.happiness.Value = (byte)Math.Min((int)byte.MaxValue, (int)(byte)((NetFieldBase<byte, NetByte>)this.FarmAnimal.happiness) + Math.Max(5, 40 - (int)(byte)((NetFieldBase<byte, NetByte>)this.FarmAnimal.happinessDrain)));
            }
        }

        private void GainExperience()
        {
            // same logic used in pet()
            Game1.player.gainExperience((int)Skill.Skills.Farming, FarmAnimalTreat.EXPERIENCE_POINTS);
        }

        private void ChangeFullness()
        {
            this.FarmAnimal.fullness.Value = byte.MaxValue;
        }

        public override void ChangeFriendship(int points = FarmAnimalTreat.FRIENDSHIP_POINTS_STEP)
        {
            this.FarmAnimal.friendshipTowardFarmer.Value = Math.Max(0, Math.Min(FarmAnimalTreat.FRIENDSHIP_POINTS_MAX, this.FarmAnimal.friendshipTowardFarmer.Value + points));

            string mailId = "farmAnimalLoveMessage" + this.FarmAnimal.myID;

            // Chance to show the "pet loves you" global message
            this.AttemptToExpressLove(this.FarmAnimal, this.FarmAnimal.friendshipTowardFarmer, FarmAnimalTreat.FRIENDSHIP_POINTS_MAX, mailId);
        }

        private bool ReceiveProfessionBoost()
        {
            if (Game1.player.professions.Contains((int)Profession.Professions.Shepherd) && !this.FarmAnimal.isCoopDweller())
            {
                return true;
            }

            if (Game1.player.professions.Contains((int)Profession.Professions.Coopmaster) && this.FarmAnimal.isCoopDweller())
            {
                return true;
            }

            return false;
        }

        public override void DoEmote()
        {
            base.DoEmote(this.FarmAnimal);
        }

        public void RefuseTreat(bool penalty)
        {
            int pointsLoss = penalty ? -1 * (FarmAnimalTreat.FRIENDSHIP_POINTS_STEP / 2) : 0;

            base.RefuseTreat(this.FarmAnimal, pointsLoss);
        }

        public override void PlaySound()
        {
            this.FarmAnimal.makeSound();
        }
    }
}
