/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SolidFoundations.Framework.Models.Backport;
using SolidFoundations.Framework.Models.ContentPack.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Models.ContentPack
{
    public class ExtendedBuildingItemConversion : BuildingItemConversion
    {
        [ContentSerializer(Optional = true)]
        public int MinutesPerConversion = -1;
        internal int? MinutesRemaining;

        public bool ShouldTrackTime;

        [ContentSerializer(Optional = true)]
        public bool RefreshMaxDailyConversions;
        internal int? CachedMaxDailyConversions;

        public new List<ExtendedAdditionalChopDrops> ProducedItems;
    }
}
