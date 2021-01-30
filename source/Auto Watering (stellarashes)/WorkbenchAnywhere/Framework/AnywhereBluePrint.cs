/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stellarashes/SDVMods
**
*************************************************/

using StardewValley;

namespace WorkbenchAnywhere.Framework
{
    public class AnywhereBluePrint : BluePrint
    {
        private readonly MaterialStorage _materialStorage;

        internal AnywhereBluePrint(string name, MaterialStorage materialStorage) : base(name)
        {
            _materialStorage = materialStorage;
        }

        public new virtual bool doesFarmerHaveEnoughResourcesToBuild()
        {
            return true;
        }

        public new virtual void consumeResources()
        {
        }
    }
}
