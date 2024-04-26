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
using System.Linq;
using SObject = StardewValley.Object;

namespace NeverToxic.StardewMods.YetAnotherFishingMod.Framework
{
    internal class SFishingRod(FishingRod instance)
    {
        public FishingRod Instance { get; set; } = instance;

        private readonly int _initialAttachmentSlotsCount = instance.AttachmentSlotsCount;

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

        public void AutoHook(bool doVibrate)
        {
            if (this.CanHook())
            {
                this.Instance.timePerBobberBob = 1f;
                this.Instance.timeUntilFishingNibbleDone = FishingRod.maxTimeToNibble;
                this.Instance.DoFunction(Game1.player.currentLocation, (int)this.Instance.bobber.X, (int)this.Instance.bobber.Y, 1, Game1.player);

                if (doVibrate)
                    Rumble.rumble(0.95f, 200f);
            }
        }

        public void SpawnBait(List<string> baitIds, int amountOfBait = 1, bool overrideAttachmentLimit = false)
        {
            for (int i = FishingRod.BaitIndex; i < FishingRod.TackleIndex; i++)
            {
                string baitId = baitIds.ElementAtOrDefault(i);

                if (baitId == null || baitId == "" || ItemRegistry.GetDataOrErrorItem(baitId).IsErrorItem)
                    continue;

                if (overrideAttachmentLimit && this.Instance.AttachmentSlotsCount < i + 1)
                {
                    if (Game1.server != null)
                        return;

                    this.Instance.AttachmentSlotsCount = i + 1;
                }
                if (this.Instance.AttachmentSlotsCount > i && this.Instance.attachments.ElementAt(i) == null)
                    this.Instance.attachments[i] = ItemRegistry.Create<SObject>(baitId, amountOfBait);
            }
        }

        public void SpawnTackles(List<string> tackleIds, bool overrideAttachmentLimit = false)
        {
            for (int i = FishingRod.TackleIndex; i < FishingRod.TackleIndex + 2; i++)
            {
                string tackleId = tackleIds.ElementAtOrDefault(i - FishingRod.TackleIndex);

                if (tackleId == null || tackleId == "" || ItemRegistry.GetDataOrErrorItem(tackleId).IsErrorItem)
                    continue;

                if (overrideAttachmentLimit && this.Instance.AttachmentSlotsCount < i + 1)
                {
                    if (Game1.server != null)
                        return;

                    this.Instance.AttachmentSlotsCount = i + 1;
                }

                if (this.Instance.AttachmentSlotsCount > i && this.Instance.attachments.ElementAt(i) == null)
                    this.Instance.attachments[i] = ItemRegistry.Create<SObject>(tackleId);
            }
        }

        public void ResetAttachmentsLimit()
        {
            if (this._initialAttachmentSlotsCount == this.Instance.AttachmentSlotsCount)
                return;

            for (int i = this.Instance.AttachmentSlotsCount; i > this._initialAttachmentSlotsCount; i--)
            {
                this.Instance.attachments[i - 1] = null;
            }

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

        public void InstantBite()
        {
            if (this.Instance.timeUntilFishingBite > 0)
                this.Instance.timeUntilFishingBite = 0f;
        }
    }
}
