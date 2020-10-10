/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JessebotX/StardewValleyMods
**
*************************************************/

namespace Sprint_Sprint.Framework.Config
{
    class NoSprintIfTooTiredConfig
    {
        /// <summary> If sprinting is disabled when you are below the <see cref="TiredStamina"/> value </summary>
        public bool Enabled { get; set; } = true;
        /// <summary> Going below this value will prevent you from sprinting if <see cref="Enabled"/> is true </summary>
        public float TiredStamina { get; set; } = 20f;
    }
}
