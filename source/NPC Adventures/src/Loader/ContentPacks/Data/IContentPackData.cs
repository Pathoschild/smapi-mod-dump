using System.Collections.Generic;

namespace NpcAdventure.Loader.ContentPacks.Data
{
    internal interface IContentPackData
    {
        bool validate(out Dictionary<string, string> errors);
    }
}