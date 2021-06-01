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
    class LocationalEffect : IEffect
    {
        LocationGroup Location { get; }
        IEffect ActualEffect { get; }

        List<EffectDescriptionLine> effectDescription;

        bool isApplied = false;

        public List<EffectDescriptionLine> EffectDescription => effectDescription;

        Item SourceItem { get; set; }

        public LocationalEffect(LocationGroup location, IEffect effect)
        {
            Location = location;
            ActualEffect = effect;
            effectDescription = ActualEffect.EffectDescription.Select(x => new EffectDescriptionLine(x.Icon, x.Text + $" {Location.GetEffectDescriptionSuffix()}")).ToList();
        }

        public void Apply(Item sourceItem, EffectChangeReason reason)
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

        public void Remove(Item sourceItem, EffectChangeReason reason)
        {
            if (isApplied)
            {
                isApplied = false;
                ActualEffect.Remove(sourceItem, reason);
            }

            EffectHelper.Events.LocationChanged -= Events_LocationChanged;
            SourceItem = null;
        }

        private void RevalidateConditions(Item sourceItem, EffectChangeReason reason)
        {
            if (Location.IsActive())
            {
                if (!isApplied)
                {
                    if (SourceItem != null && reason == EffectChangeReason.Reset)
                    {
                        Game1.addHUDMessage(new CustomHUDMessage($"The effect of {SourceItem.DisplayName} is now active", SourceItem, Color.White, TimeSpan.FromSeconds(5)));
                    }

                    isApplied = true;
                    ActualEffect.Apply(sourceItem, reason);
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
                    ActualEffect.Remove(sourceItem, reason);
                }
            }
        }
    }
}
