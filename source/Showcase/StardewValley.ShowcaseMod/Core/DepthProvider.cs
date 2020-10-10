/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

namespace Igorious.StardewValley.ShowcaseMod.Core
{
    public sealed class DepthProvider
    {
        private float _initialDepth;
        private readonly float _step;

        public DepthProvider(float initialDepth, float step = 0.0000001f)
        {
            _initialDepth = initialDepth;
            _step = step;
        }

        public float GetDepth()
        {
            var result = _initialDepth;
            _initialDepth += _step;
            return result;
        }
    }
}