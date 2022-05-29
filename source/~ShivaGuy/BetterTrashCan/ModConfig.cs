/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ShivaGuy/StardewMods
**
*************************************************/

namespace BetterTrashCan
{
    public class ModConfig
    {
        public Progression progression { get; set; } = Progression.Linear;
    }

    public enum Progression
    {
        Linear, Exponential
    }
}