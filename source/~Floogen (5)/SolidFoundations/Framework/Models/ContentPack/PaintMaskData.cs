/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Models.ContentPack
{
    public class PaintMaskData
    {
        public string Name { get; set; }
        public int MinBrightness { get; set; } = -100;
        public int MaxBrightness { get; set; } = 100;
    }
}
