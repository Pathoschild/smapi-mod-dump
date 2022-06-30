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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SolidFoundations.Framework.Models.ContentPack.Actions.SpecialAction;

namespace SolidFoundations.Framework.Models.ContentPack.Actions
{
    public class ModifyInventoryAction
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public int MinCount { get; set; } = 1;
        public int MaxCount { get; set; } = 1;
        public int Quality { get; set; }
        public OperationName Operation { get; set; } = OperationName.Add;
    }
}
