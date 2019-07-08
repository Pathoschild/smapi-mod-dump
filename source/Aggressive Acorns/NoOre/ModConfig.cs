using JetBrains.Annotations;

namespace NoOre
{
    public interface IModConfig
    {
        bool ReplaceOres { get; }
        bool ReplaceGemNodes { get; }
        bool ReplaceMysticStone { get; }
        bool ReplaceGeodeNodes { get; }
    }


    public class ModConfig : IModConfig
    {
        public bool ReplaceOres { get; [UsedImplicitly] set; }
        public bool ReplaceGemNodes { get; [UsedImplicitly] set; }
        public bool ReplaceMysticStone { get; [UsedImplicitly] set; }
        public bool ReplaceGeodeNodes { get; [UsedImplicitly] set; }
    }
}