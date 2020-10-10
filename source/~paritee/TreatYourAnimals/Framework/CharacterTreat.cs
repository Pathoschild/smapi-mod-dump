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
using System;
using System.Collections.Generic;

namespace TreatYourAnimals.Framework
{
    abstract class CharacterTreat
    {
        public const int INEDIBLE_THRESHOLD = -300;

        private const string STRINGS_LOVE_MESSAGE = "Strings\\Characters:PetLovesYou";

        private enum Poisons
        {
            PaleAle = 303,
            Beer = 346,
            Wine = 348,
            Mead = 459,
            EnergyTonic = 349,
            MuscleRemedy = 351,
            Coffee = 395,
            ChocolateCake = 220,
            CoffeeBean = 433,
            OilOfGarlic = 772,
            IridiumMilk = 803,
            Garlic = 248,
            SpringOnion = 399,
            Leek = 20,
            Cherry = 638,
        }

        protected ModConfig Config;

        public CharacterTreat(ModConfig config)
        {
            this.Config = config;
        }

        public void ReduceActiveItemByOne()
        {
            Game1.player.reduceActiveItemByOne();
        }

        protected void AttemptToExpressLove(Character character, int points, int pointsThreshold, string mailKey)
        {
            if (points >= pointsThreshold && !Game1.player.mailReceived.Contains(mailKey))
            {
                // "PetLovesYou": "{0} loves you. <"
                Game1.showGlobalMessage(Game1.content.LoadString(CharacterTreat.STRINGS_LOVE_MESSAGE, character.displayName));
                Game1.player.mailReceived.Add(mailKey);
            }
        }

        public void RefuseTreat(Character character, int points)
        {
            if (points != 0)
            {
                this.ChangeFriendship(points);
            }

            this.DoEmote(character, Emote.Emotes.Angry);
            this.PlaySound();
        }

        public abstract void GiveTreat();

        public abstract void ChangeFriendship(int points);

        public abstract void DoEmote();

        public void DoEmote(Character character, Emote.Emotes emote = Emote.Emotes.Heart)
        {
            character.doEmote((int)emote, true);
        }

        public abstract void PlaySound();

        public bool IsPoisonous(StardewValley.Object item)
        {
            if (item.Edibility < 0)
            {
                return true;
            }

            // Animal poisons
            List<int> poisons = new List<int>((int[])Enum.GetValues(typeof(Poisons)));

            return poisons.Contains(item.parentSheetIndex);
        }

    }
}
