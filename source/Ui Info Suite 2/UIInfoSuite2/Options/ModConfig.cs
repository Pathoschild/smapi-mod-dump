/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UiInfoSuite2
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIInfoSuite.Options
{
    class ModConfig
    {
        public bool ShowOptionsTabInMenu { get; set; } = true;
        public string ApplyDefaultSettingsFromThisSave { get; set; } = "JohnDoe_123456789";
    }
}
