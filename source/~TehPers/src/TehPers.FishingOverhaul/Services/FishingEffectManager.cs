/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using TehPers.Core.Api.DI;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Api.Effects;

namespace TehPers.FishingOverhaul.Services
{
    internal class FishingEffectManager
    {
        public ConditionsCalculator ConditionsCalculator { get; }
        public FishingEffectEntry Entry { get; }
        public IFishingEffect Effect { get; }
        public bool Enabled { get; private set; }

        public FishingEffectManager(
            IGlobalKernel kernel,
            ConditionsCalculator conditionsCalculator,
            FishingEffectEntry entry
        )
        {
            this.ConditionsCalculator = conditionsCalculator
                ?? throw new ArgumentNullException(nameof(conditionsCalculator));
            this.Entry = entry ?? throw new ArgumentNullException(nameof(entry));
            this.Effect = this.Entry.CreateEffect(kernel);
        }

        public bool? UpdateEnabled(FishingInfo fishingInfo)
        {
            switch (this.Enabled)
            {
                case false when this.ConditionsCalculator.IsAvailable(fishingInfo):
                    this.Enabled = true;
                    return true;
                case true when !this.ConditionsCalculator.IsAvailable(fishingInfo):
                    this.Enabled = false;
                    return false;
                default:
                    return null;
            }
        }
    }
}
