/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;

namespace AnimalDialogueFramework
{
    public partial class ModEntry
    {
        private static bool IsAnimal(NPC instance)
        {
            return (Config.HorseEnabled && instance is Horse) || (Config.PetEnabled && instance is Pet) || (Config.ChildEnabled && instance is Child) || (Config.MonsterEnabled && instance is Monster) || (Config.JunimoEnabled && (instance is Junimo || instance is JunimoHarvester));
        }

        private static bool CheckType(bool inst, NPC __instance)
        {
            if (!Config.ModEnabled || !IsAnimal(__instance))
                return inst;
            return false;
        }
        private static bool CheckMonster(bool inst)
        {
            if (!Config.ModEnabled || !Config.MonsterEnabled)
                return inst;
            return false;
        }
    }
}