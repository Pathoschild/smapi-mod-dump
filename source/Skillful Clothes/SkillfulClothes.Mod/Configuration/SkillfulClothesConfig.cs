/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Configuration
{
    class SkillfulClothesConfig
    {
        public bool EnableShirtEffects { get; set; } = true;
        public bool EnablePantsEffects { get; set; } = true;
        public bool EnableHatEffects { get; set; } = true;

        public bool AllItemsCanBeTailored { get; set; } = false;

        public bool LoadCustomEffectDefinitions { get; set; } = false;

        public bool verboseLogging { get; set; } = false;
    }
}
