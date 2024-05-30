/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rtrox/LineSprinklersRedux
**
*************************************************/

using Newtonsoft.Json;

namespace LineSprinklersRedux.Framework.Data
{
    internal class ModData
    {
        private static string DirectionKey => $"{ModConstants.ModKeySpace}/Direction";

        public static SprinklerDirection GetDirection(SObject sprinkler)
        {
            if (!sprinkler.modData.ContainsKey(DirectionKey))
            {
                SetDirection(sprinkler, SprinklerDirection.Right);
            }
            var data = sprinkler.modData[DirectionKey];
            if (Enum.TryParse(typeof(SprinklerDirection), data, out var ret))
            {
                return (SprinklerDirection)ret!;
            }
            return SprinklerDirection.Right;
        }

        public static void SetDirection(SObject sprinkler, SprinklerDirection dir)
        {
            sprinkler.modData[DirectionKey] = dir.ToString();
        }
    

    }
}
