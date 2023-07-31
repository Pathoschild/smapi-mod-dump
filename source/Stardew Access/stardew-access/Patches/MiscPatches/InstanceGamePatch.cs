/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

namespace stardew_access.Patches
{
    internal class InstanceGamePatch
    {
        internal static void ExitPatch()
        {
            MainClass.ScreenReader?.CloseScreenReader();
        }
    }
}
