using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace TreeCapitator
{
    public class TreeCapitator : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += onUpdate;
        }

        private Vector2 ToolLocationVector;

        private void onUpdate(object sneder, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;
            if (!Context.IsWorldReady)
                return;
            
            Point ToolLocationPoint;
            if (Game1.player.isRidingHorse())
            {
                ToolLocationVector = Game1.currentCursorTile;
            }
            else
            {
                ToolLocationVector = new Vector2((int) Game1.player.GetToolLocation().X / Game1.tileSize,
                    (int) Game1.player.GetToolLocation().Y / Game1.tileSize);
            }

            if (Game1.player.currentLocation is Farm || Game1.player.currentLocation.IsGreenhouse)
            {
                
                if (Game1.player.currentLocation.terrainFeatures.ContainsKey(ToolLocationVector))
                {

                    if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is GiantCrop)
                    {
                        Tree_Capitator();
                    }

                    if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is Tree)
                    {
                        Tree_Capitator();
                    }
                }
            }

            if (!(Game1.player.currentLocation is Farm) && !Game1.player.currentLocation.IsGreenhouse)
            {
                
                if (Game1.player.currentLocation.terrainFeatures.ContainsKey(ToolLocationVector))
                {
                    if (Game1.player.currentLocation.terrainFeatures[ToolLocationVector] is Tree)
                    {
                        Tree_Capitator();
                    }
                }
            }
        }


        private void Tree_Capitator()
        {
            if(!(Game1.player.UsingTool && Game1.player.CurrentTool is Axe))
                return;
            TerrainFeature obj = Game1.player.currentLocation.terrainFeatures[ToolLocationVector];
            if (obj is Tree tree && tree.health.Value > 1)
                tree.health.Value = 1;
            else if (obj is FruitTree fruitTree && fruitTree.health.Value > 1)
                fruitTree.health.Value = 1;
        }
    }
}