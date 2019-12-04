using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
        private static ModConfig Config;
        private static Texture2D pixelTexture;
        private static List<StardewValley.Object> ladderStones;
        private static bool nextIsShaft;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            pixelTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] colorArray = Enumerable.Range(0, 1).Select<int, Color>((Func<int, Color>)(i => Color.White)).ToArray<Color>();
            pixelTexture.SetData<Color>(colorArray);

            ladderStones = new List<StardewValley.Object>();
            
            Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            Helper.Events.Display.Rendered += this.OnRendered;
            Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            Helper.Events.Player.Warped += this.OnWarped;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (e.IsOneSecond)
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

                    if (Config.ForceShafts && Game1.mine.getMineArea(-1) == 121 && !nextIsShaft && ladderStones.Count > 0)
                    {
                        Random mineRandom = Helper.Reflection.GetField<Random>(Game1.mine, "mineRandom").GetValue();

                        Random r = Clone(mineRandom);

                        double next = r.NextDouble();

                        while (next >= 0.2)
                        {
                            next = r.NextDouble();
                            mineRandom.NextDouble();
                        }

                        nextIsShaft = true;
                    }
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
                        double chance = 0.02 + 1.0 / (double) Math.Max(1, netStonesLeftOnThisLevel) + (double) Game1.player.LuckLevel / 100.0 + Game1.player.DailyLuck / 5.0;

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

        private void OnRendered(object sender, RenderedEventArgs e)
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

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == Config.ToggleShaftsKey)
            {
                Config.ForceShafts = !Config.ForceShafts;

                if (Config.ForceShafts)
                {
                    Game1.addHUDMessage(new HUDMessage("Force shafts toggled on", 2));
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage("Force shafts toggled off", 2));
                }

                Helper.WriteConfig<ModConfig>(Config);
            }
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            ladderStones.Clear();
            nextIsShaft = false;
        }

        private T Clone<T>(T source)
        {
            IFormatter fmt = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                fmt.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)fmt.Deserialize(stream);
            }
        }

    }
}
