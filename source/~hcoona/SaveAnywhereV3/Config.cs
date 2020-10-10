/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hcoona/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Input;

namespace SaveAnywhereV3
{
    internal class Config
    {
        public Keys SaveBindingKey { get; set; } = Keys.K;

        public Keys LoadBindingKey { get; set; } = Keys.L;

        public bool AutoSavingEnabled { get; set; } = false;

        public double AutoSavingIntervalMinutes { get; set; } = 5;
    }
}
