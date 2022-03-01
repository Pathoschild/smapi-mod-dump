/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Microsoft.Xna.Framework;
using SkillfulClothes.Types;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Special
{
    /// <summary>
    /// Activates an encapsulated effect if the player is at a given location
    /// </summary>
    class LocationalEffect : SingleEffect<LocationalEffectParameters>
    {        
        List<EffectDescriptionLine> effectDescription;

        bool isApplied = false;

        public override List<EffectDescriptionLine> EffectDescription => effectDescription;

        Item SourceItem { get; set; }

        protected override EffectDescriptionLine GenerateEffectDescription() => null;

        public override void ReloadParameters()
        {
            effectDescription = Parameters.Effect.EffectDescription.Select(x => new EffectDescriptionLine(x.Icon, x.Text + Parameters.Location.GetEffectDescriptionSuffix())).ToList();
        }

        public LocationalEffect(LocationalEffectParameters parameters)
            : base(parameters)
        {            
            // --
        }

        public LocationalEffect(LocationGroup location, IEffect actualEffect)
            : base(LocationalEffectParameters.With(location, actualEffect))
        {
            // --
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            SourceItem = sourceItem;

            RevalidateConditions(sourceItem, reason);

            EffectHelper.Events.LocationChanged -= Events_LocationChanged;
            EffectHelper.Events.LocationChanged += Events_LocationChanged;
        }

        private void Events_LocationChanged(object sender, ValueChangeEventArgs<GameLocation> e)
        {
           RevalidateConditions(SourceItem, EffectChangeReason.Reset);            
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            if (isApplied)
            {
                isApplied = false;
                Parameters.Effect.Remove(sourceItem, reason);
            }

            EffectHelper.Events.LocationChanged -= Events_LocationChanged;
            SourceItem = null;
        }

        private void RevalidateConditions(Item sourceItem, EffectChangeReason reason)
        {
            if (Parameters.Location.IsActive())
            {
                if (!isApplied)
                {
                    if (SourceItem != null && reason == EffectChangeReason.Reset)
                    {
                        Game1.addHUDMessage(new CustomHUDMessage($"The effect of {SourceItem.DisplayName} is now active", SourceItem, Color.White, TimeSpan.FromSeconds(5)));
                    }

                    isApplied = true;
                    Parameters.Effect.Apply(sourceItem, reason);
                }
            }
            else
            {
                if (isApplied)
                {
                    if (SourceItem != null && reason == EffectChangeReason.Reset)
                    {
                        Game1.addHUDMessage(new CustomHUDMessage($"The effect of {SourceItem.DisplayName} wore off", SourceItem, Color.White, TimeSpan.FromSeconds(5)));
                    }

                    isApplied = false;
                    Parameters.Effect.Remove(sourceItem, reason);
                }
            }
        }
    }

    public class LocationalEffectParameters : IEffectParameters
    {
        public LocationGroup Location { get; set; } = LocationGroup.None;
        public IEffect Effect { get; set; } = new NullEffect();

        public static LocationalEffectParameters With(LocationGroup location, IEffect actualEffect)
        {
            return new LocationalEffectParameters() { Location = location, Effect = actualEffect };
        }
    }
}
