/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCrystalariumMod
{
    public class CustomCloner
    {
        public string ModUniqueID;
        public string Name;
        public bool GetObjectBackOnChange;
        public bool GetObjectBackImmediately;
        public bool UsePfmForInput;
        public Dictionary<object, int> CloningData;
        public Dictionary<int, int> CloningDataId = new Dictionary<int, int>();
    }
}
