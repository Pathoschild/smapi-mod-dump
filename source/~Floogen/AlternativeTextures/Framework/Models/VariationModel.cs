/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

namespace AlternativeTextures.Framework.Models
{
    public class VariationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float ChanceWeight { get; set; } = 1f;
        public List<string> Keywords { get; set; } = new List<string>();
        public List<AnimationModel> Animation { get; set; } = new List<AnimationModel>();
        public List<int[]> Tints { get; set; } = new List<int[]>();

        public bool HasAnimation()
        {
            return Animation.Count() > 0;
        }

        public bool HasTint()
        {
            return Tints.Count() > 0;
        }
    }
}
