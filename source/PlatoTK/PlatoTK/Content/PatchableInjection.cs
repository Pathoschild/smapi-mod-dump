/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatoTK.Content
{
    internal abstract class PatchableInjection<TAsset> : AssetInjection<TAsset>
    {
        public readonly Rectangle? SourceArea;

        public readonly Rectangle? TargetArea;
        public PatchableInjection(
            IPlatoHelper helper,
            string assetName,
            TAsset value,
            InjectionMethod method,
            Rectangle? sourceArea = null,
            Rectangle? targetArea = null,
            string conditions = "")
            : base(helper, assetName, value, method, conditions)
        {
            SourceArea = sourceArea;
            TargetArea = targetArea;
        }
    }
}
