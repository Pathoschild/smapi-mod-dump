using System;
using System.Collections.Generic;
using StardewValley;

namespace RemoteFridgeStorage.apis
{
    public interface ICookingSkillApi
    {
        Func<IList<Item>> setFridgeFunction(Func<IList<Item>> func);
    }
}