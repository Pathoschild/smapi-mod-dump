using System.Collections.Generic;

namespace StardewLib
{
    internal interface IStats
    {
        IDictionary<string, object> GetFields();
    }
}
