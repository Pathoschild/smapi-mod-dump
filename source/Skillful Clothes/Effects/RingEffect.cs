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
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects
{
    /// <summary>
    /// An effect which "simulates" the player wearing a specific ring
    /// (if the game checks if the player is wearing a given ring the result will be true
    ///  if a RingEffect for this ring is worn by the player, see HarmonyPatches.isWearingRing)    
    ///  Other ring's effects will not be replicated by this implementation
    /// </summary>
    class RingEffect : SingleEffect
    {
        public RingType Ring { get; }

        public RingEffect(RingType ring)
        {
            Ring = ring;
        }

        protected override EffectDescriptionLine GenerateEffectDescription() => Ring.GetEffectDescription();

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            // not needed
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {            
            // not needed
        }        
    }

    enum RingType
    {        
        SlimeCharmerRing = 520,
        YobaRing = 524,
        SturdyRing = 525,
        BurglarsRing = 526        

    }

    static class RingTypeExtensions
    {
        public static EffectDescriptionLine GetEffectDescription(this RingType ring)
        {
            // todo: get description from original item

            switch (ring)
            {
                case RingType.SlimeCharmerRing:
                    return new EffectDescriptionLine(EffectIcon.None, "Prevents damage from slimes"); // EffectIcon.Slime
                case RingType.YobaRing:
                    return new EffectDescriptionLine(EffectIcon.Yoba, "Occasionally shields the wearer from damage");
                case RingType.SturdyRing:
                    return new EffectDescriptionLine(EffectIcon.None, "Cuts the duration of negative status effects in half");
                case RingType.BurglarsRing:
                    return new EffectDescriptionLine(EffectIcon.None, "Monsters have a greater chance of dropping loot");                
                default:
                    return new EffectDescriptionLine(EffectIcon.None, "Unknown effect");
            }
        }      
    }
}
