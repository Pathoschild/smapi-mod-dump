/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using System.Collections.Generic;

namespace SolidFoundations.Framework.Models.ContentPack
{
    public class InputFilter
    {
        public List<RestrictedItem> RestrictedItems;

        public string FilteredItemMessage;

        public string InputChest;

        public class RestrictedItem
        {
            public List<string> RequiredTags { get; set; }
            public int MaxAllowed { get; set; } = -1;
            public bool RejectWhileProcessing { get; set; }
            public string Condition { get; set; }
            public string[] ModDataFlags { get; set; }
        }
    }
}
