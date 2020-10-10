/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using TehPers.FishingOverhaul.Api.Enums;

namespace TehPers.FishingOverhaul.Api {
    public interface IFishTraits {
        float Difficulty { get; }
        int MaxSize { get; }
        int MinSize { get; }
        FishMotionType MotionType { get; }
    }
}