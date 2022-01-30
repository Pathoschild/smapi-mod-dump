/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/XxHarvzBackxX/fabrication
**
*************************************************/

using System;
using System.Diagnostics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    private Stopwatch sw = new Stopwatch();
    private ModConfig config;
    private Random rand = new Random();
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
        config = helper.ReadConfig<ModConfig>();
    }

    private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
    {
        sw.Start();
    }

    private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;
        if (sw.ElapsedMilliseconds >= 1000 * config.Time)
        {
            Ring r = new Ring();
            int ID = rand.Next(0, 930);
            Item o = new Object(ID, config.Stack);
            while (o.Name is "???" or "Error Item" or "Weeds" || o.getDescription() is "..." or "???")
            {
                o = new Object(rand.Next(0, 930), config.Stack);
            }
            if (o.Category == -96 || o.DisplayName.Contains("Ring") || o.DisplayName is "Iridium Band" or "Immunity Band")
            {
                o = new Ring(ID);
            }
            if (Game1.player.freeSpotsInInventory() != 0)
            {
                Game1.player.addItemToInventory(o);
            }
            else
            {
                Game1.player.currentLocation.debris.Add(new Debris(o, Game1.player.getStandingPosition()));
                Monitor.Log($"Added object {o.Name} at {Game1.player.getStandingPosition()}.");
            }
            sw.Restart();
        }
    }
    class ModConfig
    {
        public int Time { get; set; } = 5; // time in seconds to wait before giving player item
        public int Stack { get; set; } = 1; // amount of random item to give to player
    }
}