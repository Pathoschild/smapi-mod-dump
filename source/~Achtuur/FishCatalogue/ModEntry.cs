/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore;
using AchtuurCore.Events;
using AchtuurCore.Patches;
using FishCatalogue.Drawing;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace FishCatalogue
{
    public class ModEntry : Mod
    {
        internal static Texture2D seasonTexture;
        internal static ModEntry Instance;
        internal ModConfig Config;

        internal static FishCatalogue fishCatalogue;
        internal FishOverlay fishOverlay;
        internal FishSpawnsPage fishSpawnPage;
        public override void Entry(IModHelper helper)
        {

            I18n.Init(helper.Translation);
            ModEntry.Instance = this;

            // HarmonyPatcher.ApplyPatches(this,
            
            // );

            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
            helper.Events.Display.Rendered += this.OnRendered;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.H)
                fishSpawnPage.Enable();
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.player.ActiveItem is null)
            {
                fishOverlay.DisableHuds();
                fishOverlay.SetEnabledState();
                return;
            }

            if (Game1.player.ActiveItem is FishingRod)
                fishOverlay.EnableFishHud();
            else
                fishOverlay.DisableFishHud();

            if (Game1.player.ActiveItem.Name == "Crab Pot")
                fishOverlay.EnableTrapHud();
            else
                fishOverlay.DisableTrapHud();

            fishOverlay.SetEnabledState();
        }

        private void OnRendered(object sender, RenderedEventArgs e)
        {
            fishOverlay.DrawOverlay(e.SpriteBatch);
        }

        private void LoadFishData()
        {
            fishCatalogue = new FishCatalogue();
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            this.Config.createMenu();
            LoadFishData();
            seasonTexture = Instance.Helper.ModContent.Load<Texture2D>("assets/all_seasons_icon.png");
            fishOverlay = new();
            fishSpawnPage = new();
        }
    }
}
