using System;
using System.Collections.Generic;
using StardewValley;

namespace RemoteFridgeStorage.API
{
    public interface ICookingSkillApi
    {
        Func<IList<Item>> setFridgeFunction(Func<IList<Item>> func);
    }
}