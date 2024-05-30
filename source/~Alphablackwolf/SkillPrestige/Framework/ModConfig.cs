/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using SkillPrestige.Logging;

namespace SkillPrestige.Framework
{
    /// <summary>Represents options for this mod.</summary>
    [Serializable]
    internal class ModConfig
    {
        /// <summary>The logging verbosity for the mod. A log level set to Verbose will log all entries.</summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;

        /// <summary>Whether testing mode is enabled, which adds testing specific commands to the system.</summary>
        public bool TestingMode { get; set; }
    }
}
