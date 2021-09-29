/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tesla1889tv/ControlValleyMod
**
*************************************************/

/*
 * ControlValley
 * Stardew Valley Support for Twitch Crowd Control
 * Copyright (C) 2021 TerribleTable
 * LGPL v2.1
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System.Globalization;
using System.Threading;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ControlValley
{
    public class ModEntry : Mod
    {
        private ControlClient client = null;

        public override void Entry(IModHelper helper)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            if (Context.IsMultiplayer)
            {
                this.Monitor.Log("Crowd Control is unavailable in multiplayer. Skipping mod.", LogLevel.Info);
                return;
            }

            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            if (client == null) return;
            Helper.Events.GameLoop.Saved -= client.OnSaved;
            Helper.Events.GameLoop.Saving -= client.OnSaving;
            Helper.Events.Player.Warped -= client.OnWarped;
            client.Stop();
            client = null;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            if (!Context.IsWorldReady || client != null) return;
            client = new ControlClient();
            Helper.Events.GameLoop.Saved += client.OnSaved;
            Helper.Events.GameLoop.Saving += client.OnSaving;
            Helper.Events.Player.Warped += client.OnWarped;
            new Thread(new ThreadStart(client.NetworkLoop)).Start();
            new Thread(new ThreadStart(client.RequestLoop)).Start();
        }
    }
}
