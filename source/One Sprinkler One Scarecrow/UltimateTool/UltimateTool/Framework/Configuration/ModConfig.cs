using UltimateTool.Framework.Configuration;

namespace UltimateTool.Framework.Configuration
{
   internal class ModConfig
    {
        public bool ModEnabled { get; set; } = false;
        public string ActionKey { get; set; } = "Z";
        public string GrowKey { get; set; } = "X";
        public string MineClearKey { get; set; } = "V";
        public int MagnetRadius { get; set; } = 378;
        public int ToolRadius { get; set; } = 5;
        public IToolConfig ITools { get; set; } = new IToolConfig();
    }
}
