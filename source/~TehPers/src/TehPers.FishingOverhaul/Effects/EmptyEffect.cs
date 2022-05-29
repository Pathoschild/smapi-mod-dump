/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Effects;

namespace TehPers.FishingOverhaul.Effects
{
    internal class EmptyEffect : IFishingEffect
    {
        public void Apply(FishingInfo fishingInfo)
        {
        }

        public void Unapply(FishingInfo fishingInfo)
        {
        }

        public void UnapplyAll()
        {
        }
    }
}
