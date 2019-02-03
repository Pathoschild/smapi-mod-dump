using StardewValley.Characters;
using System;

namespace TreatYourAnimals.Framework
{
    class PetTreat : CharacterTreat
    {
        private const int FRIENDSHIP_POINTS_MAX = 1000;
        private const int FRIENDSHIP_POINTS_STEP = 6; // matches water bowl value

        private const int CHOCOLATE_CAKE = 220;

        private Pet Pet;

        public PetTreat(Pet pet, ModConfig config) : base(config)
        {
            this.Pet = pet;
        }

        public override void GiveTreat()
        {
            this.ReduceActiveItemByOne();
            this.ChangeFriendship();
            this.DoEmote();
            this.PlaySound();
        }

        public override void ChangeFriendship(int points = PetTreat.FRIENDSHIP_POINTS_STEP)
        {
            this.Pet.friendshipTowardFarmer = Math.Max(0, Math.Min(PetTreat.FRIENDSHIP_POINTS_MAX, this.Pet.friendshipTowardFarmer + points));

            // Chance to show the "pet loves you" global message
            this.AttemptToExpressLove(this.Pet, this.Pet.friendshipTowardFarmer, PetTreat.FRIENDSHIP_POINTS_MAX, "petLoveMessage");
        }

        public void RefuseTreat(bool penalty = false)
        {
            int pointsLoss = penalty ? -1 * (PetTreat.FRIENDSHIP_POINTS_STEP / 2) : 0;

            base.RefuseTreat(this.Pet, pointsLoss);
        }

        public override void DoEmote()
        {
            base.DoEmote(this.Pet);
        }

        public override void PlaySound()
        {
            this.Pet.playContentSound();
        }
    }
}
