/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionSense.Framework.Models
{
    public class AnimationModel
    {
        internal enum Type
        {
            Unknown,
            Idle,
            Moving
        }

        public int Frame { get; set; }
        public bool OverrideStartingIndex { get; set; }
        public List<Condition> Conditions { get; set; } = new List<Condition>();
        public int Duration { get; set; } = 1000;

        internal bool HasCondition(Condition.Type type)
        {
            return Conditions.Any(c => c.Name == type);
        }

        internal Condition GetConditionByType(Condition.Type type)
        {
            return Conditions.FirstOrDefault(c => c.Name == type);
        }
    }
}
