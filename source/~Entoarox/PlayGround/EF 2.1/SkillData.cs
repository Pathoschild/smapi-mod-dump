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

namespace Entoarox.Framework.Core.Skills
{
    class PlayerSkillData
    {
#pragma warning disable CS0649
        public class SkillInfo
        {
            public int Experience;
            public int Level;
            public List<string> Professions;
        }
        public Dictionary<string, SkillInfo> Skills;
#pragma warning restore CS0649
    }
}
