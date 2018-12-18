using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Settings
{
    interface SettingsInterface  
    {
        void Initialize();
        void LoadSettings();
        void SaveSettings();
    }
}
