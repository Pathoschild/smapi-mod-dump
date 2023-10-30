/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Newtonsoft.Json;
using StardewModdingAPI;

namespace CropGrowthAdjustments.Types
{
    public class Adjustments
    {
        public List<CropAdjustment> CropAdjustments { get; set; }
        
        [JsonIgnore] 
        public IContentPack ContentPack { get; set; }
    }
}