/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace AnimalDialogueFramework
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool PetEnabled { get; set; } = true;
        public bool HorseEnabled { get; set; } = true;
        public bool ChildEnabled { get; set; } = true;
        public bool JunimoEnabled { get; set; } = true;
        public bool MonsterEnabled { get; set; } = true;
    }
}
