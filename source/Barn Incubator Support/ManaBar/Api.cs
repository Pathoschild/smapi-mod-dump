/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace ManaBar
{
    public interface IApi
    {
        int GetMana(Farmer farmer);
        void AddMana(Farmer farmer, int amt);

        int GetMaxMana(Farmer farmer);
        void SetMaxMana(Farmer farmer, int newMaxMana);
    }

    public class Api : IApi
    {
        public int GetMana(Farmer farmer)
        {
            return farmer.GetCurrentMana();
        }

        public void AddMana(Farmer farmer, int amt)
        {
            farmer.AddMana(amt);
        }

        public int GetMaxMana(Farmer farmer)
        {
            return farmer.GetMaxMana();
        }

        public void SetMaxMana(Farmer farmer, int newMaxMana)
        {
            farmer.SetMaxMana(newMaxMana);
        }
    }
}
