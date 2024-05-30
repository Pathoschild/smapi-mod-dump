/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/ToolAssembly
**
*************************************************/

using StardewValley.Inventories;
using StardewValley;
using StardewValley.Tools;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

namespace Tool_Assembly
{
    /// <summary>
    /// This API provides you convenient operation of Tool Assembly, 
    /// by allows you to modify the items and create new tools. 
    /// </summary>
    public interface IToolAPI
    {
        /// <summary>
        /// Get the specific <see cref="Inventory"/> that the id represent
        /// </summary>
        /// <param name="id">an id. If exists, can be found in Item.modData with key "ofts.toolAss.id"</param>
        /// <returns>the content of this id as an <see cref="Inventory"/></returns>
        Inventory getToolContentWithID(long id);
        /// <summary>
        /// Get the content inside this tool
        /// </summary>
        /// <param name="tool">the tool</param>
        /// <returns>the content of this tool as an <see cref="Inventory"/></returns>
        Inventory getToolContentWithTool(Item tool);
        /// <summary>
        /// Create a new tool
        /// </summary>
        /// <param name="assignNewInventory">should assign a new id to this tool or leave blank</param>
        /// <returns>a new Tool Assembly</returns>
        GenericTool createNewTool(bool assignNewInventory = false);
        /// <summary>
        /// Create a new tool with specific id. 
        /// </summary>
        /// <param name="id">the id of the tool</param>
        /// <returns>a new Tool Assembly</returns>
        GenericTool createToolWithId(long id);
        /// <summary>
        /// Assign a new inventory. 
        /// </summary>
        /// <returns>the assigned id</returns>
        long assignNewInventory();
        /// <summary>
        /// return whether the specific id exists. 
        /// </summary>
        /// <param name="id">the id</param>
        /// <returns>exists for not</returns>
        bool doesIDExist(long id);
        /// <summary>
        /// Allow items that are not tool to be treated as tool in this mod. Weapons are automatically treated as tool. <br/>
        /// Since this may be too cheaty (its not a shulker box! by the way, 
        /// shulker box only has 27 slot this has 36, even more cheaty!), 
        /// only api can change this setting, users can't. 
        /// </summary>
        /// <param name="QualifiedItemId">The qualified item id of the item that should be treated as tool</param>
        /// <remarks>Note that this only need to be called once, and for compatibility concern, 
        /// this will be synced for all farmhands, meaning guests don't need to call this (but you still can)</remarks>
        void treatThisItemAsTool(string QualifiedItemId);
        /// <summary>
        /// exactly same as <see cref="treatThisItemAsTool(string)"/>
        /// </summary>
        /// <param name="QualifiedItemIds">items to be treated as tool</param>
        void treatTheseItemsAsTool(IEnumerable<string> QualifiedItemIds);
        /// <summary>
        /// Get a function that returns the piority of this tool
        /// When auto swich is enabled, the game will loop through all tools and calculate the piority of each tool.<br/>
        /// </summary>
        /// <param name="type">The <see href = "QualifiedItemId"/> of one specific item, OR, the <see cref="Type.ToString"/> of the category</param>
        /// <returns>a function that returns the piority of this tool<br/>
        /// This function will return a int value of the following:<br/>
        /// -1, tool not applicable in this situation<br/>
        /// 0, can use but not suitable<br/>
        /// 1, suitable <br/>
        /// 2, very suitable<br/>
        /// 3, top piority, only choice<br/>
        /// This function will use the following parameters:<br/>
        /// GameLocation, the current location<br/>
        /// Vector2, location of the current tile<br/>
        /// ResourceClump, the resourceClump at that location<br/>
        /// Item, the tool been examined. Note that this may not be the current tool!<br/>
        /// The final piority will be adjusted according to upgrade level, you don't need to handle these logic by yourself. </returns>
        Func<GameLocation, Vector2, ResourceClump?, Item, int> getToolSwichAlgroithmForThisType(string type);
        /// <summary>
        /// Set the piority calculation function of the specific type. 
        /// When auto swich is enabled, the game will loop through all tools and calculate the piority of each tool.<br/>
        /// </summary>
        /// <param name="type">The <see href = "QualifiedItemId"/> of one specific item, OR, the <see cref="Type.ToString"/> of the category</param>
        /// <param name="func">This function will return a int value of the following:<br/>
        /// -1, tool not applicable in this situation<br/>
        /// 0, can use but not suitable<br/>
        /// 1, suitable <br/>
        /// 2, very suitable<br/>
        /// 3, top piority, only choice<br/>
        /// This function will use the following parameters:<br/>
        /// GameLocation, the current location<br/>
        /// Vector2, location of the current tile<br/>
        /// ResourceClump, the resourceClump at that location<br/>
        /// Item, the tool been examined. Note that this may not be the current tool!<br/>
        /// The final piority will be adjusted according to upgrade level, you don't need to handle these logic by yourself. </param>
        void setToolSwichAlgroithmForThisType(string type, Func<GameLocation, Vector2, ResourceClump?, Item, int> func);
    }
}
