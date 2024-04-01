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

namespace SolidFoundations.Framework.Models.ContentPack.Actions
{
    public class QuestionResponseAction
    {
        public string Question { get; set; }
        public List<ResponseAction> Responses { get; set; } = new List<ResponseAction>();
        public bool ShuffleResponseOrder { get; set; }
    }
}
