/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/UnkLegacy/QiSprinklers
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QiSprinklers
{ 
    public class ModConfig
    {
        public int SprinklerRange { get; set; } = 3;
        public bool ActivateOnPlacement { get; set; } = true;
        public bool ActivateOnAction { get; set; } = true;
    }
}