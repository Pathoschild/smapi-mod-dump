/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/unidarkshin/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System.Timers;
using StardewValley.TerrainFeatures;
using StardewValley.Monsters;
using StardewValley.Locations;
using System.Collections;
using System.IO;
using Netcode;

namespace CooperativeEnhanced
{
    /// <summary>The mod entry point.</summary>
    /// 
    public class ModEntry : Mod
    {
        public static Random rnd;
        public static Mod instance;
        ModConfig config;

        public double CMult;
        private IEnumerator<int> expPts;

        public ModEntry()
        {
            instance = this;
        }

        public override void Entry(IModHelper helper)
        {
            rnd = new Random();

            helper.Events.GameLoop.SaveLoaded += SaveEvents_AfterLoad;
            helper.Events.GameLoop.OneSecondUpdateTicked += GameEvents_OneSecondTick;
            helper.Events.Display.Rendered += GraphicsEvents_OnPostRenderEvent;
            
        }

        private void GraphicsEvents_OnPostRenderEvent(object sender, EventArgs e)
        {
            SpriteBatch b = Game1.spriteBatch;

            if (NearPlayer())
            {
                
                Rectangle destinationRectangle = new Rectangle(Game1.viewport.MaxCorner.X - 151, 51, 150, 50);
                b.Draw(Game1.staminaRect, destinationRectangle, new Rectangle(0, 0, 150, 50), Color.Black);
                //b.DrawString();
            }

        }

        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {

            if (!expPts.Equals(Game1.player.experiencePoints.GetEnumerator()))
            {
                IEnumerator<int> curPts = Game1.player.experiencePoints.GetEnumerator();
                int x = 0;

                do
                {
                    int change = curPts.Current - expPts.Current;
                    if (change > 0 && NearPlayer())
                    {
                        Game1.player.gainExperience(x, (int)(change * config.coopMult));
                    }

                    x++;
                } while (expPts.MoveNext() && curPts.MoveNext());

                expPts = Game1.player.experiencePoints.GetEnumerator();
            }
        }

        private bool NearPlayer()
        {
            foreach (Farmer f in Game1.getOnlineFarmers())
            {
                if (Game1.player != f && Game1.player.currentLocation == f.currentLocation && Vector2.Distance(Game1.player.getTileLocation(), f.getTileLocation()) <= 20.0f)
                {
                    return true;
                }

            }
            return false;
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            config = instance.Helper.Data.ReadJsonFile<ModConfig>($"Data/{Constants.SaveFolderName}.json") ?? new ModConfig();
            CMult = config.coopMult;

            if (!File.Exists($"Data/{Constants.SaveFolderName}.json"))
                instance.Helper.Data.WriteJsonFile<ModConfig>($"Data/{Constants.SaveFolderName}.json", config);

            expPts = Game1.player.experiencePoints.GetEnumerator();
        }


    }

    public class ModConfig
    {
        public double coopMult { get; set; } = 1.25;
    }
}