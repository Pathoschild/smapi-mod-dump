/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Events;
using AchtuurCore.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using Microsoft.Xna.Framework;
using AchtuurCore;
using StardewValley.Network;
using FasterMiniObelisk.Patches;
using StardewValley;
using System.Linq;
using SObject = StardewValley.Object;
using System.Collections.Generic;

namespace FasterMiniObelisk
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance;
        internal ModConfig Config;


        Vector2 oldPos = Vector2.Zero;

        public static void DoMiniObeliskWarp(Farmer who, Vector2 targetPos)
        {
            Game1.player.freezePause = 50;

            ModEntry.Instance.oldPos = who.Position;
            who.setTileLocation(targetPos);

        }


        public override void Entry(IModHelper helper)
        {

            I18n.Init(helper.Translation);
            ModEntry.Instance = this;

            HarmonyPatcher.ApplyPatches(this,
                new ObeliskWarpPatch()
            );

            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            //if (oldPos != Vector2.Zero)
                //Game1.UpdateViewPort(true, new Point((int) oldPos.X, (int) oldPos.Y));
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            this.Config.createMenu();
        }
    }
}
