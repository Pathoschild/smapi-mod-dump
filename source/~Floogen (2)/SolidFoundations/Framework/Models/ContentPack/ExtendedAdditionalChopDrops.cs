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
    public class ExtendedAdditionalChopDrops : AdditionalChopDrops
    {
        [ContentSerializer(Optional = true)]
        public int Quality { get; set; }

        [ContentSerializer(Optional = true)]
        public string PreserveType { get; set; }

        [ContentSerializer(Optional = true)]
        public string PreserveID { get; set; }

        [ContentSerializer(Optional = true)]
        public int AddPrice { get; set; }

        [ContentSerializer(Optional = true)]
        public float MultiplyPrice { get; set; } = 1f;

        [ContentSerializer(Optional = true)]
        public string[] ModDataFlags { get; set; }
    }
}
