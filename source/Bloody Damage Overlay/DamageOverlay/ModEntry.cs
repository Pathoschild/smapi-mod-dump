/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-damage-overlay/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using StardewModdingAPI;

namespace DamageOverlay
    {

    public class ModEntry : Mod
        {
        DamageOverlay damageOverlay;

        public override void Entry(IModHelper helper) {
            Log.Monitor = this.Monitor;
            damageOverlay = new DamageOverlay(this);

            helper.Events.GameLoop.GameLaunched += damageOverlay.OnGameLaunched;
            helper.Events.Display.RenderedWorld += damageOverlay.OnRenderedWorld;
            helper.Events.Display.WindowResized += damageOverlay.OnWindowResized;
            helper.Events.Input.ButtonsChanged += damageOverlay.OnButtonsChanged;

            helper.ConsoleCommands.Add("dover", "Damage Overlay debug commands.\nSyntax: dover <subcommand> <args>", damageOverlay.OnCommand);
            }

        }
    }
