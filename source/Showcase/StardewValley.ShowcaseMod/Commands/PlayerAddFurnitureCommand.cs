/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

using Igorious.StardewValley.DynamicApi2;
using Igorious.StardewValley.DynamicApi2.Services;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace Igorious.StardewValley.ShowcaseMod.Commands
{
    public sealed class PlayerAddFurnitureCommand : ConsoleCommand
    {
        public PlayerAddFurnitureCommand(IMonitor monitor) : base(monitor, "player_addfurniture", "Add furniture by ID.") { }

        public void Execute(int furnitureID)
        {
            if (!DataService.Instance.GetFurniture().ContainsKey(furnitureID))
            {
                Error($"Furniture with ID={furnitureID} is not registered!");
                return;
            }

            Game1.player.addItemByMenuIfNecessary(new Furniture(furnitureID, Vector2.Zero));
        }
    }
}