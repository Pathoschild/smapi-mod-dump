/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Mail;
using StardewValley;
using StardewValley.Tools;

namespace StardewArchipelago.Stardew
{
    public class StardewWeapon : StardewItem
    {
        public int MinDamage { get; }
        public int MaxDamage { get; }
        public double KnockBack { get; }
        public double Speed { get; }
        public double AddedPrecision { get; }
        public double AddedDefence { get; }
        public int Type { get; }
        public int BaseMineLevel { get; }
        public int MinMineLevel { get; }
        public double AddedAoe { get; }
        public double CriticalChance { get; }
        public double CriticalDamage { get; }

        public StardewWeapon(int id, string name, string description, int minDamage, int maxDamage, double knockBack, double speed, double addedPrecision, double addedDefence, int type, int baseMineLevel, int minMineLevel, double addedAoe, double criticalChance, double criticalDamage, string displayName)
        : base(id, name, /* TODO */1, displayName, description)
        {
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            KnockBack = knockBack;
            Speed = speed;
            AddedPrecision = addedPrecision;
            AddedDefence = addedDefence;
            Type = type;
            BaseMineLevel = baseMineLevel;
            MinMineLevel = minMineLevel;
            AddedAoe = addedAoe;
            CriticalChance = criticalChance;
            CriticalDamage = criticalDamage;
        }

        public override Item PrepareForGivingToFarmer(int amount = 1)
        {
            return new MeleeWeapon(Id);
        }

        public override Item PrepareForRecovery()
        {
            return new MeleeWeaponToRecover(Id);
        }

        public override void GiveToFarmer(Farmer farmer, int amount = 1)
        {
            var weapon = PrepareForGivingToFarmer();
            farmer.addItemByMenuIfNecessary(weapon);
        }

        public override LetterAttachment GetAsLetter(ReceivedItem receivedItem, int amount = 1)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveMeleeWeapon, Id.ToString());
        }

        public override string ToString()
        {
            return $"{Name} [Level {((MeleeWeapon)PrepareForGivingToFarmer()).getItemLevel()} {GetWeaponType()}]";
        }

        private string GetWeaponType()
        {
            return Type switch
            {
                1 => "Dagger",
                2 => "Club",
                _ => "Sword",
            };
        }
    }
}
