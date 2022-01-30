/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using StardewValley;
    using StardewValley.Buildings;
    using StardewValley.Characters;
    using StardewValley.Objects;

    public class HorseWrapper
    {
        private readonly HorseOverhaul mod;

        public HorseWrapper(Stable stable, HorseOverhaul mod, Chest saddleBag, int? stableID)
        {
            Stable = stable;

            // has to be assigned right now because the horse gets removed from the character list while it has a rider
            Horse = stable.getStableHorse();
            this.mod = mod;
            SaddleBag = saddleBag;
            StableID = stableID;
        }

        public int? StableID { get; set; }

        public Horse Horse { get; private set; }

        public Stable Stable { get; private set; }

        public Chest SaddleBag { get; set; }

        public bool WasPet { get; set; }

        public bool GotFed { get; set; }

        public bool HasHeater { get; set; }

        public bool GotWater
        {
            get
            {
                return Stable?.modData?.ContainsKey($"{mod.ModManifest.UniqueID}/gotWater") == true;
            }

            set
            {
                if (value)
                {
                    Stable.modData[$"{mod.ModManifest.UniqueID}/gotWater"] = "water";
                }
                else
                {
                    Stable.modData.Remove($"{mod.ModManifest.UniqueID}/gotWater");
                }

                Stable.resetTexture();
            }
        }

        public int Friendship { get => GetFriendship(); set => SetFriendship(value); }

        public void JustGotWater()
        {
            if (!GotWater)
            {
                Friendship += 6;
                GotWater = true;
                mod.Helper.Multiplayer.SendMessage(new StateMessage(this), StateMessage.GotWaterType, modIDs: new[] { mod.ModManifest.UniqueID });
            }
        }

        public void AddHeaterBonus()
        {
            if (!HasHeater && Game1.IsWinter && mod.Config.HorseHeater)
            {
                Friendship += 5;
                HasHeater = true;
                mod.Helper.Multiplayer.SendMessage(new StateMessage(this), StateMessage.GotHeaterType, modIDs: new[] { mod.ModManifest.UniqueID });
            }
        }

        public void JustGotFood(int friendshipAmount)
        {
            if (!GotFed || mod.Config.AllowMultipleFeedingsADay)
            {
                SoundModule.PlayHorseEatSound(this.Horse, mod.Config);

                Friendship += friendshipAmount;
                GotFed = true;
                mod.Helper.Multiplayer.SendMessage(new StateMessage(this), StateMessage.GotFoodType, modIDs: new[] { mod.ModManifest.UniqueID });
            }
        }

        public void JustGotPetted()
        {
            if (!WasPet)
            {
                SoundModule.PlayHorsePettingSound(this.Horse, mod.Config);

                Friendship += 12;
                WasPet = true;
                mod.Helper.Multiplayer.SendMessage(new StateMessage(this), StateMessage.GotPettedType, modIDs: new[] { mod.ModManifest.UniqueID });
            }
        }

        public float GetMovementSpeedBonus()
        {
            int f = Friendship;

            // this is intentionally integer division
            int halfHearts = f / 100;

            float maxSpeed = mod.Config.MaxMovementSpeedBonus;

            return maxSpeed / 10 * halfHearts;
        }

        private int GetFriendship()
        {
            // backwards compatibility
            string moddata = null;
            if (Horse != null)
            {
                Horse.modData.TryGetValue($"{mod.ModManifest.UniqueID}/friendship", out moddata);
            }

            int friendship = 0;

            if (!string.IsNullOrEmpty(moddata))
            {
                friendship = int.Parse(moddata);

                if (friendship > 1000)
                {
                    friendship = 1000;
                }

                // remove old value
                Horse.modData.Remove($"{mod.ModManifest.UniqueID}/friendship");

                Stable.modData[$"{mod.ModManifest.UniqueID}/friendship"] = friendship.ToString();
            }
            else
            {
                Stable.modData.TryGetValue($"{mod.ModManifest.UniqueID}/friendship", out moddata);

                if (!string.IsNullOrEmpty(moddata))
                {
                    friendship = int.Parse(moddata);

                    if (friendship > 1000)
                    {
                        friendship = 1000;
                    }

                    Stable.modData[$"{mod.ModManifest.UniqueID}/friendship"] = friendship.ToString();
                }
                else
                {
                    Stable.modData.Add($"{mod.ModManifest.UniqueID}/friendship", friendship.ToString());
                }
            }

            return friendship;
        }

        private int SetFriendship(int friendship)
        {
            if (friendship > 1000)
            {
                friendship = 1000;
            }

            // backwards compatibility
            if (Horse != null && Horse.modData.ContainsKey($"{mod.ModManifest.UniqueID}/friendship"))
            {
                Horse.modData.Remove($"{mod.ModManifest.UniqueID}/friendship");
            }

            Stable.modData[$"{mod.ModManifest.UniqueID}/friendship"] = friendship.ToString();

            return friendship;
        }
    }
}