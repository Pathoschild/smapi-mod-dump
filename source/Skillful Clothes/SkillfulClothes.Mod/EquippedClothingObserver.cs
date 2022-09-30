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
    {
        bool isSuspended = false;

        string clothingName;

        /// <summary>
        /// Index of the piece of clothing which
        /// is currently equipped by the player
        /// </summary>
        int? currentIndex;

        Item currentItem;

        /// <summary>
        /// The effect of the currently equipped piece of clothing         
        /// </summary>
        IEffect currentEffect;

        public EquippedClothingItemObserver()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an Enum");
            }

            clothingName = typeof(T).Name;
        }

        public void Update(Farmer farmer)
        {
            int newIndex = GetCurrentIndex(farmer);

            if (currentIndex == null || currentIndex != newIndex)
            {
                ClothingChanged(farmer, newIndex);
            }            
        }

        protected abstract int GetCurrentIndex(Farmer farmer);

        protected abstract Item GetCurrentItem(Farmer farmer);

        protected void ClothingChanged(Farmer farmer, int newIndex)
        {
            bool initialUpdate = !currentIndex.HasValue;

            currentIndex = newIndex;

            T ev = (T)(object)currentIndex;
            Logger.Debug($"{farmer.Name}'s {clothingName} changed to {newIndex} {Enum.GetName(typeof(T), ev)}.");

            // remove old effect
            if (currentEffect != null)
            {
                currentEffect.Remove(currentItem, EffectChangeReason.ItemRemoved);
                currentEffect = null;
            }

            currentItem = GetCurrentItem(farmer);


            if (ItemDefinitions.GetEffectByIndex<T>(currentIndex ?? -1, out currentEffect)) {
                if (!isSuspended)
                {
                    currentEffect.Apply(currentItem, initialUpdate ? EffectChangeReason.DayStart : EffectChangeReason.ItemPutOn);                    
                }
            } else
            {
                currentEffect = null;
                Logger.Debug($"Equipped {clothingName} has no effects");
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
                currentEffect?.Remove(currentItem, reason);
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
                currentEffect?.Apply(currentItem, reason);                
            }
        }

        public void Reset(Farmer farmer)
        {
            currentIndex = null;
            currentEffect?.Remove(currentItem, EffectChangeReason.Reset);
            currentEffect = null;
        }

        public bool HasRingEffect(int ringIndex)
        {
            if (isSuspended) return false;

            if(currentEffect is EffectSet set)
            {
                return set.Effects.Any(x => (x is RingEffect re) && (int)re.Parameters.Ring == ringIndex);
            } else 
            { 
                return (currentEffect is RingEffect re) && (int)re.Parameters.Ring == ringIndex;
            }
        }
    }

    class ShirtObserver : EquippedClothingItemObserver<Shirt>
    {
        protected override int GetCurrentIndex(Farmer farmer)
        {
            return farmer.shirtItem.Value?.ParentSheetIndex ?? -1;
        }

        protected override Item GetCurrentItem(Farmer farmer)
        {
            return farmer.shirtItem.Value;
        }
    }

    class PantsObserver : EquippedClothingItemObserver<Pants>
    {
        protected override int GetCurrentIndex(Farmer farmer)
        {
            return farmer.pantsItem.Value?.ParentSheetIndex ?? -1;
        }

        protected override Item GetCurrentItem(Farmer farmer)
        {
            return farmer.pantsItem.Value;
        }
    }

    class HatObserver : EquippedClothingItemObserver<Types.Hat>
    {
        protected override int GetCurrentIndex(Farmer farmer)
        {
            return farmer.hat.Value?.which ?? -1;
        }

        protected override Item GetCurrentItem(Farmer farmer)
        {
            return farmer.hat.Value;
        }
    }
}
