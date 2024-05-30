/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Types;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes
{
    /// <summary>
    /// Bundles the separate clothing item observers
    /// </summary>
    class ClothingObserver
    {
        ShirtObserver shirtObserver;
        PantsObserver pantsObserver;
        HatObserver hatObserver;

        public ClothingObserver()
        {
            if (EffectHelper.Config.EnableShirtEffects)
            {
                shirtObserver = new ShirtObserver();
                Logger.Info("Shirt effects will be active");
            }
            else
            {
                ItemDefinitions.ShirtEffects.Clear();
                Logger.Info("Shirt effects have been disabled");
            }

            if (EffectHelper.Config.EnablePantsEffects)
            {
                pantsObserver = new PantsObserver();
                Logger.Info("Pants effects will be active");
            }
            else
            {
                ItemDefinitions.PantsEffects.Clear();
                Logger.Info("Pants effects have been disabled");
            }

            if (EffectHelper.Config.EnableHatEffects)
            {
                hatObserver = new HatObserver();
                Logger.Info("Hat effects will be active");
            }
            else
            {
                ItemDefinitions.HatEffects.Clear();
                Logger.Info("Hat effects have been disabled");
            }
        }

        public bool HasRingEffect(string ringId)
        {
           return (shirtObserver?.HasRingEffect(ringId) ?? false) || (pantsObserver?.HasRingEffect(ringId) ?? false) || (hatObserver?.HasRingEffect(ringId) ?? false);
        }

        public void Reset(Farmer farmer)
        {
            shirtObserver?.Reset(Game1.player);
            pantsObserver?.Reset(Game1.player);
            hatObserver?.Reset(Game1.player);
        }

        public void Restore(Farmer farmer, EffectChangeReason reason)
        {
            // restore active effects
            shirtObserver?.Restore(farmer, reason);
            pantsObserver?.Restore(farmer, reason);
            hatObserver?.Restore(farmer, reason);
        }

        public void Suspend(Farmer farmer, EffectChangeReason reason)
        {
            // remove active effects, so that value changes do not get saved
            shirtObserver?.Suspend(farmer, reason);
            pantsObserver?.Suspend(farmer, reason);
            hatObserver?.Suspend(farmer, reason);
        }

        public void Update(Farmer farmer)
        {
            shirtObserver?.Update(farmer);
            pantsObserver?.Update(farmer);
            hatObserver?.Update(farmer);
        }
    }
}
