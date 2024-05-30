/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rtrox/LineSprinklersRedux
**
*************************************************/

using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Machines;
using StardewValley.GameData.Objects;

namespace LineSprinklersRedux.Framework.Data
{
    public class ObjectInformation
    {
        public string? Id { get; set; }

        public BigCraftableData? Object { get; set; }

        public string? Recipe { get; set; }

        public string? RecyclerOutput { get; set; }
    }
}
