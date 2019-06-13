using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace HDSprites.ContentPack
{
    public class WhenDictionary : Dictionary<string, string> { }

    public class ConfigSchemaConfig
    {
        public string AllowValues { get; set; }
        public string Default { get; set; }
    }

    public class DynamicTokensConfig
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public WhenDictionary When { get; set; } = new WhenDictionary();
    }

    public class ChangesConfig
    {
        public string Action { get; set; }
        public string Target { get; set; }
        public string FromFile { get; set; }
        public Rectangle FromArea { get; set; }
        public Rectangle ToArea { get; set; }
        public string Enabled { get; set; } = "True";
        public string Patchmode { get; set; } = "";
        public WhenDictionary When { get; set; } = new WhenDictionary();
    }
    
    public class ContentConfig
    {
        public Dictionary<string, ConfigSchemaConfig> ConfigSchema { get; set; } = new Dictionary<string, ConfigSchemaConfig>();
        public List<DynamicTokensConfig> DynamicTokens { get; set; } = new List<DynamicTokensConfig>();
        public List<ChangesConfig> Changes { get; set; } = new List<ChangesConfig>();
    }
}
