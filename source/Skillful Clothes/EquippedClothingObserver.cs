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
    abstract class EquippedClothingObserver<T>
    {
        bool isSuspended = false;

        string clothingName;

        /// <summary>
        /// Index of the piece of clothing which
        /// is currently equipped by the player
        /// </summary>
        int? currentIndex;

        /// <summary>
        /// The effect of the currently equipped piece of clothing         
        /// </summary>
        IEffect currentEffect;

        Dictionary<T, IEffect> effects;

        public EquippedClothingObserver(Dictionary<T, IEffect> effects, IModHelper helper)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an Enum");
            }

            clothingName = typeof(T).Name;
            this.effects = effects;
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

        protected void ClothingChanged(Farmer farmer, int newIndex)
        {
            currentIndex = newIndex;

            T ev = (T)(object)currentIndex;
            Logger.Debug($"{farmer.Name}'s {clothingName} changed to {newIndex} {Enum.GetName(typeof(T), ev)}.");

            // remove old effect
            if (currentEffect != null)
            {
                currentEffect.Remove(farmer);
                currentEffect = null;
            }            

            if (effects.TryGetValue(ev, out currentEffect)) {
                if (!isSuspended)
                {
                    currentEffect.Apply(farmer);                    
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
        public void Suspend(Farmer farmer)
        {
            if (!isSuspended)
            {
                Logger.Debug($"Suspend {clothingName} effects");
                isSuspended = true;
                currentEffect?.Remove(farmer);
            }
        }

        public void Restore(Farmer farmer)
        {
            if (isSuspended)
            {
                Logger.Debug($"Restore {clothingName} effects");
                isSuspended = false;
                currentEffect?.Apply(farmer);                
            }
        }

        public void Reset(Farmer farmer)
        {
            currentIndex = null;
            currentEffect?.Remove(farmer);
            currentEffect = null;
        }
    }

    class ShirtObserver : EquippedClothingObserver<Shirt>
    {
        public ShirtObserver(IModHelper helper)
            : base(PredefinedEffects.ShirtEffects, helper)
        {
            // --
        }

        protected override int GetCurrentIndex(Farmer farmer)
        {
            return farmer.shirtItem.Value?.parentSheetIndex ?? -1;
        }
    }

    class PantsObserver : EquippedClothingObserver<Pants>
    {
        protected override int GetCurrentIndex(Farmer farmer)
        {
            return farmer.pantsItem.Value?.parentSheetIndex ?? -1;
        }

        public PantsObserver(IModHelper helper)
            : base(PredefinedEffects.PantsEffects, helper)
        {
            // --
        }
    }

    class HatObserver : EquippedClothingObserver<Types.Hat>
    {
        public HatObserver(IModHelper helper)
            : base(PredefinedEffects.HatEffects, helper)
        {
            // --
        }

        protected override int GetCurrentIndex(Farmer farmer)
        {
            return farmer.hat.Value?.which ?? -1;
        }
    }
}
