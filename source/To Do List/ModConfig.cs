/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/eleanoot/stardew-to-do-mod
**
*************************************************/

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
