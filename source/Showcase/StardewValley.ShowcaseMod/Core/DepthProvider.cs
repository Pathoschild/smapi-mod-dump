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