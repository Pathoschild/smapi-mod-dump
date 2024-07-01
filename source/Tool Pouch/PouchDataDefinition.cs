/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Brandon22Adams/ToolPouch
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.ItemTypeDefinitions;

namespace ToolPouch
{
    public class PouchDataDefinition : BaseItemDataDefinition
    {
        public PouchDataDefinition()
        {
        }

        internal static PouchData GetSpecificData(string id)
        {
            return Game1.content.Load<Dictionary<string, PouchData>>("CodeWordZ.ToolPouch/Pouches")[id];
        }

        public override string Identifier => "(CWZ)";

        public override string StandardDescriptor => "CWZ";


        public override IEnumerable<string> GetAllIds()
        {
            return Game1.content.Load<Dictionary<string, PouchData>>("CodeWordZ.ToolPouch/Pouches").Keys;
        }

        public override bool Exists(string itemId)
        {
            return Game1.content.Load<Dictionary<string, PouchData>>("CodeWordZ.ToolPouch/Pouches").ContainsKey(itemId);
        }

        public override ParsedItemData GetData(string itemId)
        {
            var data = GetSpecificData(itemId);
            return new ParsedItemData(this, itemId, data.TextureIndex, data.Texture, "Pouch." + itemId, data.DisplayName, data.Description, StardewValley.Object.equipmentCategory, null, data, false);
        }

        public override Item CreateItem(ParsedItemData data)
        {
            return new Pouch(data.ItemId);
        }

        public override Rectangle GetSourceRect(ParsedItemData data, Texture2D texture, int spriteIndex)
        {
            int w = texture.Width / 16;
            return new Rectangle(spriteIndex % w * 16, spriteIndex / w * 16, 16, 16);
        }
    }
}
