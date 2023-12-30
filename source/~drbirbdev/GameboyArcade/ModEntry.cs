/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.Reflection;
using BirbCore.APIs;
using BirbCore.Attributes;
using StardewModdingAPI;

namespace GameboyArcade;

[SMod]
public class ModEntry : Mod
{
    [SMod.Instance]
    internal static ModEntry Instance;
    internal static Config Config;
    internal static Command Command;
    internal static Dictionary<string, Dictionary<string, Content>> Content;

    [SMod.Api("spacechase0.DynamicGameAssets", IsRequired = false)]
    internal static IDynamicGameAssetsApi DynamicGameAssets;

    public override void Entry(IModHelper helper)
    {
        Parser.ParseAll(this, Assembly.GetExecutingAssembly());
    }

    public override object GetApi()
    {
        return new GameboyArcadeAPIImpl();
    }

    public static IEnumerable<Content> AllGames()
    {
        foreach (Dictionary<string, Content> modContent in ModEntry.Content.Values)
        {
            foreach (Content content in modContent.Values)
            {
                yield return content;
            }
        }
        yield break;
    }

    public static Content SearchGames(string search)
    {
        foreach (Content content in AllGames())
        {
            if (content.UniqueID == search || content.ModID == search || content.GameID == search || content.Name == search)
            {
                return content;
            }
        }
        return null;
    }

    public static Content GetGame(string modId, string gameId = null)
    {
        if (gameId is null)
        {
            string[] parts = modId.Split("_", 2);
            if (parts.Length != 2)
            {
                Log.Error($"Expected a GameId in the form ModManifest_Game but got something else {modId}");
            }
            modId = parts[0];
            gameId = parts[1];
        }

        return Content?[modId]?[gameId];
    }
}
