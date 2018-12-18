using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Settings
{
    class PaintSettings : SettingsInterface
    {
        public static bool PaintEnabled;

        public PaintSettings()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            if (File.Exists(Path.Combine(Class1.path, "xnb_node.cmd")))
            {
                PaintEnabled = true;
                //Log.AsyncG("Revitalize: Paint Module Enabled");
            }
            else
            {
                PaintEnabled = false;
                //Log.AsyncG("Revitalize: Paint Module Disabled");
            }
        }

        public void LoadSettings()
        {
           // throw new NotImplementedException();
        }

        public void SaveSettings()
        {
           // throw new NotImplementedException();
        }
    }
}
