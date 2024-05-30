/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/1Avalon/Ore-Detector
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OreDetector
{
    public class SaveModel
    {
        public List<string> blacklistedNames = new List<string>();

        public List<string> discoveredMaterials = new List<string>();

        public List<string> discoveredmaterialsQualifiedIds = new List<string>();
    }
}
