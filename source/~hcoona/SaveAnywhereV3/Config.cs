using Microsoft.Xna.Framework.Input;

namespace SaveAnywhereV3
{
    internal class Config
    {
        public Keys SaveBindingKey { get; set; } = Keys.K;

        public Keys LoadBindingKey { get; set; } = Keys.L;

        public bool AutoSavingEnabled { get; set; } = false;

        public double AutoSavingIntervalMinutes { get; set; } = 5;
    }
}
