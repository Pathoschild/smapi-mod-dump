/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hakej/Animal-Pet-Status
**
*************************************************/

using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalPetStatus
{
    public class AnimalPetStatus : Mod
    {
        // CONSTANTS
        public const string ASSETS_PATH = "Assets";

        // MOD SETTINGS
        public bool IsActive;
        public bool IsMoving = false;
        public Vector2 Position;

        // INPUT
        public SButton ToggleButton;
        public SButton MoveButton;

        // TEXT
        public Color textColor = Color.Black;

        // NEEDED CLASSES
        public ModConfig Config;
        public Drawer Drawer;

        // BACKGROUND
        public Texture2D BackgroundTop;
        public Texture2D BackgroundMiddle;
        public Texture2D BackgroundBottom;

        public bool WereAllAnimalsPetToday = false;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            Position = Config.Position;
            IsActive = Config.IsActive;
            ToggleButton = Config.ToggleButton;
            MoveButton = Config.MoveButton;

            BackgroundTop = helper.Content.Load<Texture2D>(Path.Combine(ASSETS_PATH, "background_top.png"), ContentSource.ModFolder);
            BackgroundMiddle = helper.Content.Load<Texture2D>(Path.Combine(ASSETS_PATH, "background_middle.png"), ContentSource.ModFolder);
            BackgroundBottom = helper.Content.Load<Texture2D>(Path.Combine(ASSETS_PATH, "background_bottom.png"), ContentSource.ModFolder);

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.GameLoop.DayStarted += DayStarted;
            helper.Events.Input.ButtonReleased += OnButtonReleased;
            helper.Events.Input.CursorMoved += CursorMoved;
        }

        private void CursorMoved(object sender, CursorMovedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (!IsActive)
                return;

            if (!IsMoving)
                return;

            Position = e.NewPosition.ScreenPixels;
            Config.Position = Position;
            Helper.WriteConfig(Config);
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            WereAllAnimalsPetToday = false;
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button.IsActionButton())
            {
                if (!DoesFarmHasAnyAnimals())
                    return;

                if (!IsAnyAnimalNotPet() && !WereAllAnimalsPetToday)
                {
                    WereAllAnimalsPetToday = true;
                    Notificator.NotifyWithJingle();
                }
            }

            if (e.Button == MoveButton)
            {
                IsMoving = false;
            }
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Drawer = new Drawer(Game1.spriteBatch, Game1.smallFont);
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (!IsActive)
                return;

            if (WereAllAnimalsPetToday)
                return;

            if (!DoesFarmHasAnyAnimals())
                return;

            if (!Game1.player.currentLocation.isFarm)
                return;

            var notPetAnimals = Game1.getFarm().getAllFarmAnimals()
                .Where(a => !a.wasPet)
                .OrderBy(a => a.Name);

            Drawer.DrawAnimalNamesWithBackground(notPetAnimals, Position, BackgroundTop, BackgroundMiddle, BackgroundBottom);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == ToggleButton)
            {
                IsActive = !IsActive;
                Config.IsActive = IsActive;
                Helper.WriteConfig(Config);
            }

            if (e.Button == MoveButton)
                IsMoving = true;
        }

        private bool DoesFarmHasAnyAnimals()
        {
            return Game1.getFarm().getAllFarmAnimals().Count() != 0;
        }
        private bool IsAnyAnimalNotPet()
        {
            return Game1.getFarm().getAllFarmAnimals().Any(a => !a.wasPet);
        }
    }
}