/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

namespace BuffFramework
{
    public interface IBuffFrameworkAPI
    {
        public void UpdateBuffs();
    }

    public class BuffFrameworkAPI : IBuffFrameworkAPI
    {
        public void UpdateBuffs()
        {
            ModEntry.UpdateBuffs();
        }
    }
}