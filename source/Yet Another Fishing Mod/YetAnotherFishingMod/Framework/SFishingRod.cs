/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Tools;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace NeverToxic.StardewMods.YetAnotherFishingMod.Framework
{
    internal class SFishingRod(FishingRod instance)
    {
        public FishingRod Instance { get; set; } = instance;

        private readonly int _initialAttachmentSlotsCount = instance.AttachmentSlotsCount;

        private readonly SObject _initialBait = instance.GetBait();

        private readonly SObject _initialTackle = instance.GetTackle();

        private readonly List<BaseEnchantment> _addedEnchantments = [];

        private bool CanHook()
        {
            return
                this.Instance.isNibbling &&
                !this.Instance.hit &&
                !this.Instance.isReeling &&
                !this.Instance.pullingOutOfWater &&
                !this.Instance.fishCaught &&
                !this.Instance.showingTreasure
            ;
        }

        public void AutoHook()
        {
            if (this.CanHook())
            {
                this.Instance.timePerBobberBob = 1f;
                this.Instance.timeUntilFishingNibbleDone = FishingRod.maxTimeToNibble;
                this.Instance.DoFunction(Game1.player.currentLocation, (int)this.Instance.bobber.X, (int)this.Instance.bobber.Y, 1, Game1.player);
                Rumble.rumble(0.95f, 200f);
            }
        }

        public void SpawnBait(string baitId)
        {
            this.Instance.attachments[0] = ItemRegistry.Create<SObject>(baitId);
        }

        public void SpawnTackle(string tackleId)
        {
            this.Instance.attachments[1] = ItemRegistry.Create<SObject>(tackleId);
        }

        public void ResetAttachments()
        {
            if (this.Instance.AttachmentSlotsCount >= 1)
                this.Instance.attachments[0] = this._initialBait;
            if (this.Instance.AttachmentSlotsCount >= 2)
                this.Instance.attachments[1] = this._initialTackle;

            this.Instance.AttachmentSlotsCount = this._initialAttachmentSlotsCount;
        }

        public void AddEnchantment(BaseEnchantment enchantment)
        {
            this.Instance.enchantments.Add(enchantment);
            this._addedEnchantments.Add(enchantment);
        }

        public void ResetEnchantments()
        {
            foreach (BaseEnchantment enchantment in this._addedEnchantments)
                this.Instance.enchantments.Remove(enchantment);
        }

        public void InfiniteBait()
        {
            SObject bait = this.Instance.GetBait();
            if (bait is not null)
                bait.Stack = bait.maximumStackSize();
        }

        public void InfiniteTackle()
        {
            SObject tackle = this.Instance.GetTackle();
            if (tackle is not null)
                tackle.uses.Value = 0;
        }

        public void InstantBite()
        {
            if (this.Instance.timeUntilFishingBite > 0)
                this.Instance.timeUntilFishingBite = 0f;
        }
    }
}
