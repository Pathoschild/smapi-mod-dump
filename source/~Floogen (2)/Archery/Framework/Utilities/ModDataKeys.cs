/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

namespace Archery.Framework.Utilities
{
    public class ModDataKeys
    {
        // Weapon related
        internal const string WEAPON_FLAG = "PeacefulEnd.Archery.Weapon";
        internal const string IS_LOADED_FLAG = "PeacefulEnd.Archery.Weapon.IsLoaded";
        internal const string IS_USING_SPECIAL_ATTACK_FLAG = "PeacefulEnd.Archery.Weapon.IsUsingSpecialAttack";

        // Item related
        internal const string AMMO_FLAG = "PeacefulEnd.Archery.Ammo";
        internal const string RECIPE_FLAG = "PeacefulEnd.Archery.Recipe";

        // Special attack related
        internal const string SPECIAL_ATTACK_SNAPSHOT_COUNT = "PeacefulEnd.Archery.SpecialAttack.Snapshot.Count";
    }
}
