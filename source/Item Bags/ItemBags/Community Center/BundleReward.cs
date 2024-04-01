/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace ItemBags.Community_Center
{
    public class BundleReward
    {
        public BundleTask Task { get; }
        public string Id { get; }
        public int Quantity { get; }

        private BundleRewardType RewardType { get; }
        public bool IsBigCraftable { get { return RewardType == BundleRewardType.BigCraftable; } }
        public bool IsRing { get { return RewardType == BundleRewardType.Ring; } }
        public bool IsWeapon { get { return RewardType == BundleRewardType.Weapon; } }
        public bool IsHat { get { return RewardType == BundleRewardType.Hat; } }
        public bool IsClothing { get { return RewardType == BundleRewardType.Clothing; } }
        public bool IsFurniture { get { return RewardType == BundleRewardType.Furniture; } }

        public enum BundleRewardType
        {
            Object,
            BigCraftable,
            Ring,
            Weapon,
            Hat,
            Clothing,
            Furniture
        }

        public BundleReward(BundleTask Task, string Id, int Quantity, BundleRewardType Type)
        {
            this.Task = Task;
            this.Id = Id;
            this.Quantity = Quantity;
            this.RewardType = Type;
        }

        /// <param name="RawData">The raw data string from the game's bundle content. EX: "O 495 30".<para/>
        /// This format is described here: <see cref="https://stardewvalleywiki.com/Modding:Bundles"/></param>
        public BundleReward(BundleTask Task, string RawData)
        {
            this.Task = Task;

            List<string> Entries = RawData.Split(' ').ToList();
            if (Entries[0].Equals("O", StringComparison.CurrentCultureIgnoreCase))
                this.RewardType = BundleRewardType.Object;
            else if (Entries[0].Equals("BO", StringComparison.CurrentCultureIgnoreCase))
                this.RewardType = BundleRewardType.BigCraftable;
            else if (Entries[0].Equals("R", StringComparison.CurrentCultureIgnoreCase))
                this.RewardType = BundleRewardType.Ring;
            else if (Entries[0].Equals("W", StringComparison.CurrentCultureIgnoreCase))
                this.RewardType = BundleRewardType.Weapon;
            else if (Entries[0].Equals("H", StringComparison.CurrentCultureIgnoreCase))
                this.RewardType = BundleRewardType.Hat;
            else if (Entries[0].Equals("C", StringComparison.CurrentCultureIgnoreCase))
                this.RewardType = BundleRewardType.Clothing;
            else if (Entries[0].Equals("F", StringComparison.CurrentCultureIgnoreCase))
                this.RewardType = BundleRewardType.Furniture;
            else
                throw new NotImplementedException(string.Format("Unrecognized Bundle Reward Type: {0}", Entries[0]));

            this.Id = Entries[1];
            this.Quantity = int.Parse(Entries[2]);
        }

        public Item ToItem()
        {
            switch (RewardType)
            {
                case BundleRewardType.BigCraftable:
                    return new Object(Vector2.Zero, Id, false);
                case BundleRewardType.Ring:
                    return new Ring(Id);
                case BundleRewardType.Weapon:
                    return new MeleeWeapon(Id);
                case BundleRewardType.Hat:
                    return new Hat(Id);
                case BundleRewardType.Clothing:
                    return new Clothing(Id);
                case BundleRewardType.Furniture:
                    return new Furniture(Id, Vector2.Zero);
                default:
                    return new Object(Id, Quantity, false, -1, 0);
            }
        }
    }
}
