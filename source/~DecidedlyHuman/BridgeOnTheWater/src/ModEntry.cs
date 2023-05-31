/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using BridgeOnTheWater;
using DecidedlyShared.APIs;
using Microsoft.Xna.Framework.Graphics;
using PortableBridges.TerrainFeatures;
using PortableBridges.Tool;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace PortableBridges
{
    public class ModEntry : Mod
    {
        private IModHelper helper;
        private ISpaceCoreApi spaceCoreApi;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.InputOnButtonPressed;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
            helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            helper.Events.Display.RenderedWorld += this.Display_RenderedWorld;
            I18n.Init(helper.Translation);
            this.helper = helper;
        }

        [EventPriority((EventPriority)int.MinValue)]
        private void Display_RenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            //List<Bridge> bridges = new List<Bridge>();

            //foreach (GameLocation location in Game1.locations)
            //{
            //    foreach (TerrainFeature feature in location.terrainFeatures.Values)
            //    {
            //        if (feature is Bridge)
            //            bridges.Add(feature as Bridge);
            //    }
            //}

            //foreach (Bridge bridge in bridges)
            //{
            //    bridge.draw(e.SpriteBatch, bridge.currentTileLocation);
            //}
        }

        private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            if (this.helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
                this.spaceCoreApi = this.helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");

            if (this.spaceCoreApi == null)
            {
                this.Monitor.Log("Could not get SpaceCore API.", LogLevel.Error);

                return;
            }

            this.spaceCoreApi.RegisterSerializerType(typeof(BridgePlacementTool));
            this.spaceCoreApi.RegisterSerializerType(typeof(Bridge));
        }

        private void GameLoop_UpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            Game1.player.ignoreCollisions = false;
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("DecidedlyHuman.BridgeOnTheWater/PlacementTool"))
                e.LoadFromModFile<Texture2D>("assets/tool.png", AssetLoadPriority.Low);
        }

        private void InputOnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (e.IsDown(SButton.OemCloseBrackets)) Game1.player.addItemByMenuIfNecessary(new BridgePlacementTool());
        }
    }
}
