/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewValley;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Gui;

namespace TehPers.FishingOverhaul.Services
{
    internal interface ICustomBobberBarFactory
    {
        CustomBobberBar? Create(
            FishingInfo fishingInfo,
            FishEntry fishEntry,
            Item fishItem,
            float fishSizePercent,
            bool treasure,
            int bobber,
            bool fromFishPond
        );
    }
}