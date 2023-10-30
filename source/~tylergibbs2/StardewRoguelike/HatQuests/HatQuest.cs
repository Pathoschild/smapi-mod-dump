/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Objects;

namespace StardewRoguelike.HatQuests
{
    #pragma warning disable format
    public enum HatQuestType
    {
        TOPHAT        = 2,
        HARD_HAT      = 27,
        SQUIRE_HELMET = 51,
        FISHING_HAT   = 55,
        CHEF_HAT      = 61,
        GARBAGE_HAT   = 66,
        WARRIOR_HELM  = 93
    }
    #pragma warning restore format

    public class HatQuest
    {
        public int BarrelsDestroyed { get; set; } = 0;

        public static int GoldInBank => Game1.player.Money;

        public int DamageTaken { get; set; } = 0;

        public int DamageDealt { get; set; } = 0;

        public int HealthHealed { get; set; } = 0;

        public int FishCaught { get; set; } = 0;

        public int GoldMined { get; set; } = 0;

        private bool GaveHat { get; set; } = false;

        private static readonly Dictionary<int, string> HatData = Game1.content.Load<Dictionary<int, string>>("Data\\hats");

        public HatQuestType Which { get; }

        public HatQuest(HatQuestType which)
        {
            Which = which;
        }

        public static bool HasHat(int hatId)
        {
            return Game1.player.hat.Value is not null && Game1.player.hat.Value.which.Value == hatId;
        }

        public static bool HasBuffFor(HatQuestType which)
        {
            return HasHat((int)which);
        }

        public static List<HatQuest> PickThree(Random? random = null)
        {
            random ??= Game1.random;

            var result = new List<HatQuest>();
            var hats = Enum.GetValues<HatQuestType>().ToList();
            for (int i = 0; i < 3; i++)
            {
                var hat = hats[random.Next(hats.Count)];
                hats.Remove(hat);
                result.Add(new HatQuest(hat));
            }

            return result;
        }

        public bool IsComplete()
        {
            if (GaveHat)
                return true;

            return Which switch
            {
                HatQuestType.TOPHAT => GoldInBank >= 2000,
                HatQuestType.HARD_HAT => GoldMined >= 20,
                HatQuestType.SQUIRE_HELMET => DamageTaken >= 250,
                HatQuestType.FISHING_HAT => FishCaught >= 3,
                HatQuestType.CHEF_HAT => HealthHealed >= 150,
                HatQuestType.GARBAGE_HAT => BarrelsDestroyed >= 15,
                HatQuestType.WARRIOR_HELM => DamageDealt >= 1000,
                _ => throw new NotImplementedException("Hat type invalid")
            };
        }

        public void GiveHat()
        {
            if (GaveHat)
                return;

            Hat hat = new((int)Which);
            Debris debris = new(hat, Game1.player.getStandingPosition(), Game1.player.getStandingPosition() + new Vector2(64 * (float)(Game1.random.NextDouble() * 2 - 1), 64 * (float)(Game1.random.NextDouble() * 2 - 1)));
            Game1.currentLocation.debris.Add(debris);

            Game1.chatBox.addMessage(I18n.HatQuest_Completed(), Color.Gold);
            Game1.playSound("achievement");

            GaveHat = true;
        }

        public string GetHatBuffDescription()
        {
            return Which switch
            {
                HatQuestType.TOPHAT => I18n.HatQuest_Tophat_Buff(),
                HatQuestType.HARD_HAT => I18n.HatQuest_HardHat_Buff(),
                HatQuestType.SQUIRE_HELMET => I18n.HatQuest_SquireHelmet_Buff(),
                HatQuestType.FISHING_HAT => I18n.HatQuest_FishingHat_Buff(),
                HatQuestType.CHEF_HAT => I18n.HatQuest_ChefHat_Buff(),
                HatQuestType.GARBAGE_HAT => I18n.HatQuest_GarbageCan_Buff(),
                HatQuestType.WARRIOR_HELM => I18n.HatQuest_WarriorHelm_Buff(),
                _ => throw new NotImplementedException("Hat type invalid")
            };
        }

        public string GetHatQuestDetails()
        {
            return Which switch
            {
                HatQuestType.TOPHAT => I18n.HatQuest_Tophat_Quest(),
                HatQuestType.HARD_HAT => I18n.HatQuest_HardHat_Quest(),
                HatQuestType.SQUIRE_HELMET => I18n.HatQuest_SquireHelmet_Quest(),
                HatQuestType.FISHING_HAT => I18n.HatQuest_FishingHat_Quest(),
                HatQuestType.CHEF_HAT => I18n.HatQuest_ChefHat_Quest(),
                HatQuestType.GARBAGE_HAT => I18n.HatQuest_GarbageCan_Quest(),
                HatQuestType.WARRIOR_HELM => I18n.HatQuest_WarriorHelm_Quest(),
                _ => throw new NotImplementedException("Hat type invalid")
            };
        }

        public static bool HasQuest()
        {
            return Game1.player.get_FarmerActiveHatQuest() is not null;
        }

        public static bool IsActive(HatQuestType which)
        {
            return Game1.player.get_FarmerActiveHatQuest()?.Which == which;
        }

        public string GetHatName()
        {
            string[] split = HatData[(int)Which].Split('/');
            return split[0];
        }

        public bool IsBigHat()
        {
            string[] split = HatData[(int)Which].Split('/');
            return split[2] == "hide";
        }

        public Rectangle GetHatSourceRect()
        {
            return new((int)Which * 20 % FarmerRenderer.hatsTexture.Width, (int)Which * 20 / FarmerRenderer.hatsTexture.Width * 20 * 4, 20, 20);
        }
    }
}
