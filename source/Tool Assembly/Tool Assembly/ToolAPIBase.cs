/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/ToolAssembly
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Inventories;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace Tool_Assembly
{
    public class ToolAPIBase : IToolAPI
    {
        public long assignNewInventory()
        {
            Inventory inv = new();
            inv.AddRange(new List<Item>(36));
            ModEntry.metaData.Add(ModEntry.topIndex.Value, inv);
            ModEntry.indices.Add(ModEntry.topIndex.Value++, 0);
            return ModEntry.topIndex.Value - 1;
        }

        public GenericTool createNewTool(bool assignNewInventory = false)
        {
            if (assignNewInventory)
            {
                return createToolWithId(this.assignNewInventory());
            }
            else
            {
                return ItemRegistry.Create<GenericTool>("ofts.toolAss");
            }
        }

        public GenericTool createToolWithId(long id)
        {
            GenericTool tool = createNewTool();
            tool.modData.Add("ofts.toolAss.id", id.ToString());
            return tool;
        }

        public bool doesIDExist(long id) => ModEntry.metaData.ContainsKey(id);

        public Inventory getToolContentWithID(long id) => ModEntry.metaData[id];

        public Inventory getToolContentWithTool(Item tool) => 
            getToolContentWithID(long.Parse(tool.modData["ofts.toolAss.id"]));

        public Func<GameLocation, Vector2, ResourceClump?, Item, int> getToolSwichAlgroithmForThisType(string type)
        {
            return ToolSwitchHandler.switchLogic.TryGetValue(type, out var func) ? func : (a, b, c, d) => -1;
        }

        public void setToolSwichAlgroithmForThisType(string type, Func<GameLocation, Vector2, ResourceClump?, Item, int> func)
        {
            if(!ToolSwitchHandler.switchLogic.TryAdd(type, func)) ToolSwitchHandler.switchLogic[type] = func;
        }

        public void treatTheseItemsAsTool(IEnumerable<string> QualifiedItemIds) => ModEntry.items.AddRange(QualifiedItemIds);

        public void treatThisItemAsTool(string QualifiedItemId) => ModEntry.items.Add(QualifiedItemId);
    }
}
