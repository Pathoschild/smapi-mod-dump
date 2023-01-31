/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/Creaturebook
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Creaturebook.Framework.Models
{
    public class Creature
    {
        public int ID;

        public string NameKey;

        public string ScientificName;

        public Vector2[] ImageOffsets;

        public float[] ImageScales;

        public string UseThisItem;

        public string DescKey;

        public string[] OverrideDefaultNaming;

        public string Directory;

        public string PackID;

        public string prefix;

        public bool HasExtraImages;

        public bool HasScientificName;

        public bool HasFunFact;

        public string[] HoverDescKeys;

        public IDictionary<string, string> CategorySpecs;

        public bool DetailedEquality(Creature other)
        {
            if (ID == other.ID)
                if (NameKey == other.NameKey && ScientificName == other.ScientificName && ImageOffsets == other.ImageOffsets && UseThisItem == other.UseThisItem && DescKey == other.DescKey && OverrideDefaultNaming == other.OverrideDefaultNaming && Directory == other.Directory && HasExtraImages == other.HasExtraImages && HasScientificName == other.HasScientificName && HasFunFact == other.HasFunFact && HoverDescKeys == other.HoverDescKeys && CategorySpecs == other.CategorySpecs)
                    return true;
            return false;
        }
    }
}
