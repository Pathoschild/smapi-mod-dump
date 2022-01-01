/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/XxHarvzBackxX/airstrike
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Airstrike;
using StardewModdingAPI;
using StardewValley;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        Helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
    }

    private void Input_ButtonsChanged(object sender, StardewModdingAPI.Events.ButtonsChangedEventArgs e)
    {
        if (Context.IsWorldReady)
        {
            if (e.Released.Contains(SButton.MouseRight))
            {
                if (Game1.player.ActiveObject?.Name == "Airstrike Flare")
                {
                    if (!Game1.player.currentLocation.IsOutdoors)
                    {
                        Game1.addHUDMessage(new HUDMessage($"You need to be outside to call an airstrike!", HUDMessage.error_type));
                        return;
                    }
                    else
                    {
                        if (Game1.player.Items[Game1.player.Items.IndexOf(Game1.player.ActiveObject)].Stack > 1)
                        {
                            Game1.player.Items[Game1.player.Items.IndexOf(Game1.player.ActiveObject)].Stack -= 1;
                        }
                        else
                        {
                            Game1.player.Items.Remove(Game1.player.ActiveObject);
                        }
                        Smoke smoke = new Smoke(e.Cursor.Tile, Game1.player.currentLocation);
                    }
                }
            }
        }
    }

    private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
    {
        try
        {
            foreach (Smoke smoke in Smoke.Smokes)
            {
                smoke.Update();
            }
            foreach (Airstrike.Airstrike airstrike in Airstrike.Airstrike.Airstrikes)
            {
                airstrike.Update();
            }
        }
        catch
        {

        }
    }
}