/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jasisco5/UncannyValleyMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UncannyValleyMod
{
    public sealed class ModSaveData
    {
        public bool weaponObtained { get; set; } = false;
        public bool isBasementOpen { get; set; } = false;
        public int ExampleNumber { get; set; } = 0;
        public Dictionary<int, bool> questsObtained { get; set; } = new Dictionary<int, bool>();
    }
}
