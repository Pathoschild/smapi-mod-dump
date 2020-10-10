/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace Entoarox.Framework.Indev
{
    internal class PlayerSkillInfo
    {
        /*********
        ** Accessors
        *********/
        public int Experience = 0;
        public int Level = 0;
        public List<string> Professions = new List<string>();
    }
}
