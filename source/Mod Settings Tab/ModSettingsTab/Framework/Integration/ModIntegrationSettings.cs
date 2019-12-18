using System.Collections.Generic;

namespace ModSettingsTab.Framework.Integration
{
    public class ModIntegrationSettings
    {
        public I18N Description { get; set; } = new I18N();
        public List<Param> Config { get; set; } = new List<Param>();
    }
}