using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;


namespace ToDoMod
{
    class ModConfig
    {
        public string OpenListKey { get; set; }

        public bool UseLargerFont { get; set; }

        public bool OpenAtStartup { get; set; }

        public ModConfig()
        {
            this.OpenListKey = Keys.F2.ToString();
            this.UseLargerFont = false;
            this.OpenAtStartup = false;
        }

    }
}
