/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Effects;
using SkillfulClothes.Effects.Special;
using SkillfulClothes.Types;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes
{
    /// <summary>
    /// Watches an equipped clothing slot
    /// and replaces items if necessary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    abstract class EquippedClothingItemObserver<T>
        where T: AlphanumericItemId
    {
        bool isSuspended = false;

        string clothingName;

        /// <summary>
        /// Index of the piece of clothing which
        /// is currently equipped by the player
        /// </summary>
        PerScreen<string> _currentItemid { get; } = new PerScreen<string>();

        protected string CurrentItemId
        {
            get => _currentItemid.Value;
            set => _currentItemid.Value = value;
        }

        PerScreen<Item> _currentItem { get; } = new PerScreen<Item>();

        protected Item CurrentItem
        {
            get => _currentItem.Value;
            set => _currentItem.Value = value;
        }

        /// <summary>
        /// The effect of the currently equipped piece of clothing         
        /// </summary>
        PerScreen<IEffect> _currentEffect { get; } = new PerScreen<IEffect>();

        public IEffect CurrentEffect
        {
            get => _currentEffect.Value;
            set => _currentEffect.Value = value;
        }

        public EquippedClothingItemObserver()
        {
            if (!typeof(T).IsAssignableTo(typeof(AlphanumericItemId)))
            {
                throw new ArgumentException("T must be an Enum");
            }

            clothingName = typeof(T).Name;
        }

        public void Update(Farmer farmer)
        {
            string newItemId = GetCurrentItemId(farmer);

            if (CurrentItemId != newItemId)
            {
                ClothingChanged(farmer, newItemId);
            }            
        }

        protected abstract string GetCurrentItemId(Farmer farmer);

        protected abstract Item GetCurrentItem(Farmer farmer);

        protected void ClothingChanged(Farmer farmer, string newItemId)
        {
            bool initialUpdate = CurrentItemId == null;

            CurrentItemId = newItemId;            
            T ev = ItemDefinitions.GetKnownItemById<T>(newItemId);
            if (ev?.ItemName == newItemId)
            {
                Logger.Debug($"{farmer.Name}'s {clothingName} changed to {newItemId}");
            } else
            {
                Logger.Debug($"{farmer.Name}'s {clothingName} changed to {ev.ItemId} {ev.ItemName}");
            }            

            // remove old effect
            if (CurrentEffect != null)
            {
                CurrentEffect.Remove(CurrentItem, EffectChangeReason.ItemRemoved);
                CurrentEffect = null;
            }

            CurrentItem = GetCurrentItem(farmer);

            if (newItemId != null)
            {
                if (ItemDefinitions.GetEffectByItemId<T>(newItemId, out IEffect cEffect))
                {
                    CurrentEffect = cEffect;
                    if (!isSuspended)
                    {
                        CurrentEffect.Apply(CurrentItem, initialUpdate ? EffectChangeReason.DayStart : EffectChangeReason.ItemPutOn);
                    }
                }
                else
                {
                    CurrentEffect = null;
                    Logger.Debug($"Equipped {clothingName} has no effects");
                }
            }            
        }

        /// <summary>
        /// Disable the currently active effect
        /// </summary>
        public void Suspend(Farmer farmer, EffectChangeReason reason)
        {
            if (!isSuspended)
            {
                Logger.Debug($"Suspend {clothingName} effects");
                isSuspended = true;
                CurrentEffect?.Remove(CurrentItem, reason);
            }
        }

        /// <summary>
        /// Re-apply the current effects (after having them suspended)
        /// </summary>
        /// <param name="farmer"></param>
        public void Restore(Farmer farmer, EffectChangeReason reason)
        {
            if (isSuspended)
            {
                Logger.Debug($"Restore {clothingName} effects");
                isSuspended = false;
                CurrentEffect?.Apply(CurrentItem, reason);                
            }
        }

        public void Reset(Farmer farmer)
        {
            CurrentItemId = null;
            CurrentEffect?.Remove(CurrentItem, EffectChangeReason.Reset);
            CurrentEffect = null;
        }

        public bool HasRingEffect(string ringId)
        {
            if (isSuspended) return false;

            if(CurrentEffect is EffectSet set)
            {
                return set.Effects.Any(x => (x is RingEffect re) && ((int)re.Parameters.Ring).ToString() == ringId);
            } else 
            { 
                return (CurrentEffect is RingEffect re) && ((int)re.Parameters.Ring).ToString() == ringId;
            }
        }
    }

    class ShirtObserver : EquippedClothingItemObserver<Shirt>
    {
        protected override string GetCurrentItemId(Farmer farmer)
        {
            return farmer.shirtItem.Value?.ItemId ?? null;
        }

        protected override Item GetCurrentItem(Farmer farmer)
        {
            return farmer.shirtItem.Value;
        }
    }

    class PantsObserver : EquippedClothingItemObserver<Pants>
    {
        protected override string GetCurrentItemId(Farmer farmer)
        {
            return farmer.pantsItem.Value?.ItemId ?? null;
        }

        protected override Item GetCurrentItem(Farmer farmer)
        {
            return farmer.pantsItem.Value;
        }
    }

    class HatObserver : EquippedClothingItemObserver<Types.Hat>
    {
        protected override string GetCurrentItemId(Farmer farmer)
        {
            return farmer.hat.Value?.ItemId ?? null;
        }

        protected override Item GetCurrentItem(Farmer farmer)
        {
            return farmer.hat.Value;
        }
    }
}
