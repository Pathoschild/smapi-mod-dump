/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Tiles;
using Object = StardewValley.Object;

namespace OmniTools
{
    public class OmniToolsAPI : IOmniToolsAPI
    {
        public Tool SmartSwitch(Tool tool, GameLocation gameLocation, Vector2 tile)
        {
            return ModEntry.SmartSwitch(tool, gameLocation, tile);
        }

        public Tool SwitchTool(Tool tool, Type toolType)
        {
            return ModEntry.SwitchTool(tool, toolType);
        }
        public Tool SwitchForResourceClump(Tool tool, ResourceClump clump)
        {
            return ModEntry.SwitchForClump(tool, clump);
        }
        public Tool SwitchForObject(Tool tool, Object obj)
        {
            return ModEntry.SwitchForObject(tool, obj);
        }
        public Tool SwitchForTerrainFeature(Tool tool, TerrainFeature tf)
        {
            return ModEntry.SwitchForTerrainFeature(tool, tf);
        }
        public Tool SwitchForFarmAnimal(Tool tool, FarmAnimal animal)
        {
            return ModEntry.SwitchForAnimal(tool, animal);
        }
        public bool IsOmniTool(Tool tool)
        {
            return tool.modData.ContainsKey(ModEntry.toolsKey);
        }
        public string[] GetToolNames(Tool tool)
        {
            if (!tool.modData.TryGetValue(ModEntry.toolsKey, out var toolsString))
                return null;
            return JsonConvert.DeserializeObject<List<ToolInfo>>(toolsString).Select(i => i.displayName).ToArray();
        }
        public Tool[] GetTools(Tool tool)
        {

            return ModEntry.GetToolsFromTool(tool);
        }
    }

    public interface IOmniToolsAPI
    {
        /// <summary>Switch tools based on a tile being acted upon.</summary>
        /// <param name="tool">The omni-tool to be switched.</param>
        /// <param name="gameLocation">The current game location.</param>
        /// <param name="tile">The coordinates of the tile being acted upon.</param>
        /// <returns>The altered tool or <c>null</c> if no appropriate change is detected.</returns>
        public Tool SmartSwitch(Tool tool, GameLocation gameLocation, Vector2 tile);

        /// <summary>Switch tools based on a <see cref="StardewValley.TerrainFeatures.ResourceClump">resource clump</see>.</summary>
        /// <param name="tool">The omni-tool to be switched.</param>
        /// <param name="clump">The <see cref="StardewValley.TerrainFeatures.ResourceClump">resource clump</see>.</param>
        /// <returns>The altered tool or <c>null</c> if no appropriate change is detected.</returns>
        public Tool SwitchForResourceClump(Tool tool, ResourceClump clump);

        /// <summary>Switch tools based on an <see cref="StardewValley.Object">Object</see>.</summary>
        /// <param name="tool">The omni-tool to be switched.</param>
        /// <param name="clump">The <see cref="StardewValley.Object">Object</see>.</param>
        /// <returns>The altered tool or <c>null</c> if no appropriate change is detected.</returns>
        public Tool SwitchForObject(Tool tool, Object obj);

        /// <summary>Switch tools based on an <see cref="StardewValley.TerrainFeatures.TerrainFeature">terrain feature</see>.</summary>
        /// <param name="tool">The omni-tool to be switched.</param>
        /// <param name="clump">The <see cref="StardewValley.TerrainFeatures.TerrainFeature">terrain feature</see>.</param>
        /// <returns>The altered tool or <c>null</c> if no appropriate change is detected.</returns>
        public Tool SwitchForTerrainFeature(Tool tool, TerrainFeature tf);
        
        /// <summary>Switch tools based on a <see cref="StardewValley.FarmAnimal">farm animal</see>.</summary>
        /// <param name="tool">The omni-tool to be switched.</param>
        /// <param name="clump">The <see cref="StardewValley.FarmAnimal">farm animal</see>.</param>
        /// <returns>The altered tool or <c>null</c> if no appropriate change is detected.</returns>
        public Tool SwitchForFarmAnimal(Tool tool, FarmAnimal animal);

        /// <summary>Switch tools to a specific tool type.</summary>
        /// <param name="tool">The omni-tool to be switched.</param>
        /// <param name="toolType">The tool type to switch to.</param>
        /// <returns>The altered tool or <c>null</c> if no tool of the type is found in the omni-tool.</returns>
        public Tool SwitchTool(Tool tool, Type toolType);

        /// <summary>Check if a tool is an omni-tool.</summary>
        /// <param name="tool">The omni-tool to be checked.</param>
        /// <returns>True if the tool is an omni-tool.</returns>
        public bool IsOmniTool(Tool tool);

        /// <summary>Get names of tools stored in an omni-tool.</summary>
        /// <param name="tool">The omni-tool.</param>
        /// <returns>An array of tool display names stored in the omni-tool (does not include the tool itself).</returns>
        public string[] GetToolNames(Tool tool);

        /// <summary>Get tools stored in an omni-tool.</summary>
        /// <param name="tool">The omni-tool.</param>
        /// <returns>An array of tools stored in the omni-tool (does not include the tool itself).</returns>
        public Tool[] GetTools(Tool tool);
    }
}