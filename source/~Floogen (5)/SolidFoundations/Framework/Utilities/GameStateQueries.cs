/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using StardewValley;
using StardewValley.Delegates;

namespace SolidFoundations.Framework.Utilities
{
    public class GameStateQueries
    {
        internal static void Register()
        {
            GameStateQuery.Register("IS_PLAYER_HOLDING_ITEM", GameStateQueries.query_IS_PLAYER_HOLDING_ITEM);
            GameStateQuery.Register("IS_PLAYER_HOLDING_ANYTHING", GameStateQueries.query_IS_PLAYER_HOLDING_ANYTHING);
            GameStateQuery.Register("IS_PLAYER_HOLDING_TOOL", GameStateQueries.query_IS_PLAYER_HOLDING_TOOL);
        }

        public static bool query_IS_PLAYER_HOLDING_ITEM(string[] query, GameStateQueryContext context)
        {
            if (int.TryParse(query[2], out int itemId) is false)
            {
                return false;
            }

            int requiredStack = 1;
            if (query.Length > 3 && int.TryParse(query[3], out requiredStack) is false)
            {
                requiredStack = 1;
            }

            return GameStateQuery.Helpers.WithPlayer(context.Player, query[1], (Farmer target_farmer) => target_farmer.ActiveObject is Item item && item.ParentSheetIndex == itemId && item.Stack >= requiredStack);
        }

        public static bool query_IS_PLAYER_HOLDING_ANYTHING(string[] query, GameStateQueryContext context)
        {
            return GameStateQuery.Helpers.WithPlayer(context.Player, query[1], (Farmer target_farmer) => target_farmer.ActiveObject is not null);
        }

        public static bool query_IS_PLAYER_HOLDING_TOOL(string[] query, GameStateQueryContext context)
        {
            return GameStateQuery.Helpers.WithPlayer(context.Player, query[1], (Farmer target_farmer) => target_farmer.CurrentTool is not null);
        }
    }
}
