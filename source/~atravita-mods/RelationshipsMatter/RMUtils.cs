/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.StringHandler;

using AtraCore.Framework.Caches;

namespace RelationshipsMatter;
internal static class RMUtils
{
    private static Lazy<Dictionary<string, HashSet<string>>> relations = new(GenerateRelationsMap);

    private static IAssetName asset = null!;

    internal static void Init(IGameContentHelper parser)
    {
        asset = parser.ParseAssetName("Data/NPCDispositions");
    }

    internal static void Reset(IReadOnlySet<IAssetName>? assets)
    {
        if ((assets is null || assets.Contains(asset)) && relations.IsValueCreated)
        {
            relations = new(GenerateRelationsMap);
        }
    }

    private static Dictionary<string, HashSet<string>> GenerateRelationsMap()
    {
        Dictionary<string, HashSet<string>>? ret = new();

        Dictionary<string, string>? dispos = Game1.temporaryContent.Load<Dictionary<string, string>>(asset.BaseName);

        foreach((string npc, string dispo) in dispos)
        {
            HashSet<string> relations = new();

            // get the love interest.
            string love = dispo.GetNthChunk('/', 6).ToString();

            if (NPCCache.GetByVillagerName(love) is not null)
            {
                relations.Add(love);
            }

            // get other relatives - this is of the form `name 'relationship'` ie `Marnie 'aunt'`.
            StreamSplit relatives = dispo.GetNthChunk('/', 9).StreamSplit(options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            while (relatives.MoveNext())
            {
                string? relative = relatives.Current.ToString();
                if (NPCCache.GetByVillagerName(relative) is not null)
                {
                    relations.Add(relative);
                }

                _ = relatives.MoveNext();
            }

            ret[npc] = relations;
        }

        return ret;
    }
}
