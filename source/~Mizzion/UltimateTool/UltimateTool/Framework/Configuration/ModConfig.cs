namespace UltimateTool.Framework.Configuration
{
   internal class ModConfig
    {
        //public bool ModEnabled { get; set; } = false;
        public string ActionKey { get; set; } = "Z";
        public int MagnetRadius { get; set; } = 10;
        public int ToolRadius { get; set; } = 1;
        public int ToolLevel { get; set; } = 1;
        public bool ShowGrid { get; set; } = true;
        public ToolConfig Tools { get; set; } = new ToolConfig();
    }
}
