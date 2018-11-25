using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LadderLocator
{
    class ModEntry : Mod
    {
        private static Texture2D pixelTexture;
        private static List<StardewValley.Object> ladderStones;

        public override void Entry(IModHelper helper)
        {
            pixelTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] colorArray = Enumerable.Range(0, 1).Select<int, Color>((Func<int, Color>)(i => Color.White)).ToArray<Color>();
            pixelTexture.SetData<Color>(colorArray);

            ladderStones = new List<StardewValley.Object>();

            PlayerEvents.Warped += this.PlayerEvents_Warped;
            GameEvents.OneSecondTick += this.GameEvents_OneSecondTick;
            GraphicsEvents.OnPostRenderEvent += this.GraphicsEvents_OnPostRenderEvent;
        }

        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            if (Game1.mine != null)
            {
                bool ladderHasSpawned = Helper.Reflection.GetField<bool>(Game1.mine, "ladderHasSpawned").GetValue();

                if (ladderHasSpawned)
                {
                    ladderStones.Clear();
                }
                else if (ladderStones.Count == 0)
                {
                    findLadders();
                }
            }
        }

        private void findLadders()
        {
            int layerWidth = Game1.mine.map.Layers[0].LayerWidth;
            int layerHeight = Game1.mine.map.Layers[0].LayerHeight;
            int netStonesLeftOnThisLevel = Helper.Reflection.GetField<NetIntDelta>(Game1.mine, "netStonesLeftOnThisLevel").GetValue().Value;
            bool ladderHasSpawned = Helper.Reflection.GetField<bool>(Game1.mine, "ladderHasSpawned").GetValue();
            
            for (int x = 0; x < layerWidth; x++)
            {
                for (int y = 0; y < layerHeight; y++)
                {
                    StardewValley.Object obj = Game1.mine.getObjectAtTile(x, y);

                    if (obj != null && obj.Name.Equals("Stone"))
                    {
                        // ladder chance calculation taken from checkStoneForItems function in MineShaft class
                        Random r = new Random(x * 1000 + y + Game1.mine.mineLevel + (int) Game1.uniqueIDForThisGame / 2);
                        r.NextDouble();
                        double chance = 0.02 + 1.0 / (double) Math.Max(1, netStonesLeftOnThisLevel) + (double) Game1.player.LuckLevel / 100.0 + Game1.dailyLuck / 5.0;

                        if (Game1.mine.characters.Count == 0)
                        {
                            chance += 0.04;
                        }

                        if (!ladderHasSpawned && (netStonesLeftOnThisLevel == 0 || r.NextDouble() < chance))
                        {
                            ladderStones.Add(obj);
                        }
                    }
                }
            }
        }

        private void GraphicsEvents_OnPostRenderEvent(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady) return;

            foreach (StardewValley.Object obj in ladderStones)
            {
                var rect = new Rectangle((int)obj.getLocalPosition(Game1.viewport).X, (int)obj.getLocalPosition(Game1.viewport).Y, 64, 64);
                DrawRectangle(rect, Color.Lime);
            }
        }

        private void DrawRectangle(Rectangle rect, Color color)
        {
            Game1.spriteBatch.Draw(pixelTexture, new Rectangle(rect.Left, rect.Top, rect.Width, 3), color);
            Game1.spriteBatch.Draw(pixelTexture, new Rectangle(rect.Left, rect.Bottom, rect.Width, 3), color);
            Game1.spriteBatch.Draw(pixelTexture, new Rectangle(rect.Left, rect.Top, 3, rect.Height), color);
            Game1.spriteBatch.Draw(pixelTexture, new Rectangle(rect.Right, rect.Top, 3, rect.Height), color);
        }

        private void PlayerEvents_Warped(object sender, EventArgsPlayerWarped e)
        {
            ladderStones.Clear();
        }

    }
}
