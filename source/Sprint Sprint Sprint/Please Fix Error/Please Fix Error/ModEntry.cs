using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Linq;

namespace Please_Fix_Error
{
    class ModEntry : Mod
    {
        /// <summary> The random number generator. </summary>
        private Random RNG;
        /// <summary> Store all loaded mod ids. </summary>
        private string[] ModIDs;

        #region Methods

        /// <summary> The mod's entry point. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            // instantiate
            this.RNG = new Random();

            /* Hook Events */
            this.Helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            this.Helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
        }

        /// <summary> 
        /// Raised after the game is launched, right before the first update tick. 
        /// This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.  
        /// </summary>
        /// <param name="sender"> The object sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ModIDs = this.Helper.ModRegistry.GetAll().Select(m => m.Manifest.UniqueID).ToArray();
        }

        /// <summary> Raised before/after the game state is updated, once per second. </summary>
        /// <param name="sender"> The object sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree || !Context.IsWorldReady)
                return;

            if (e.IsMultipleOf(((uint)this.RNG.Next(1,30))))
                this.Monitor.Log(ErrorMessage(), LogLevel.Error);
        }

        /// <summary> Generate an error message. </summary>
        /// <returns> Returns an error message. </returns>
        private string ErrorMessage()
        {
            switch (this.RNG.Next(3))
            {
                case 0:
                    return "An error occured in the base update loop: System.NullReferenceException: Object reference not set to an instance of an object." +
                    "\nat StardewValley.Buildings.Stable.getSourceRectForMenu() in " +
                    $"C:\\GitlabRunner\\builds\\Gq5qA5P4\\0\\ConcernedApe\\stardewvalley\\Farmer\\Farmer\\Buildings\\Stable.cs:line {this.RNG.Next(1,133)}" +
                    "\nat StardewValley.Buildings.Building.Update(GameTime time) in " +
                    $"C:\\GitlabRunner\\builds\\Gq5qA5P4\\0\\ConcernedApe\\stardewvalley\\Farmer\\Farmer\\Buildings\\Building.cs:line {this.RNG.Next(1,980)}" +
                    "\nat StardewValley.Buildings.Stable.Update(GameTime time) in " +
                    $"C:\\GitlabRunner\\builds\\Gq5qA5P4\\0\\ConcernedApe\\stardewvalley\\Farmer\\Farmer\\Buildings\\Stable.cs:line {this.RNG.Next(1,133)}" +
                    "\nat StardewValley.Locations.BuildableGameLocation.UpdateWhenCurrentLocation(GameTime time) in " +
                    "C:\\GitlabRunner\\builds\\Gq5qA5P4\\0\\ConcernedApe\\stardewvalley\\Farmer\\Farmer\\Locations\\BuildableGameLocation.cs:line 212" +
                    $"\nat StardewValley.Farm.UpdateWhenCurrentLocation_PatchedBy<{ModIDs[this.RNG.Next(ModIDs.Length)]}>(Object time, GameTime)" +
                    "\nat StardewValley.Game1.UpdateLocations(GameTime time) in " +
                    $"C:\\GitlabRunner\\builds\\Gq5qA5P4\\0\\ConcernedApe\\stardewvalley\\Farmer\\Farmer\\Game1.cs:line {this.RNG.Next(1,133)}" +
                    "\nat StardewValley.Game1._update(GameTime gameTime) in " +
                    $"C:\\GitlabRunner\\builds\\Gq5qA5P4\\0\\ConcernedApe\\stardewvalley\\Farmer\\Farmer\\Game1.cs:line {this.RNG.Next(1,14935)}" +
                    "\nat StardewValley.Game1.Update(GameTime gameTime) in " +
                    $"C:\\GitlabRunner\\builds\\Gq5qA5P4\\0\\ConcernedApe\\stardewvalley\\Farmer\\Farmer\\Game1.cs:line {this.RNG.Next(1, 14935)}" +
                    "\nat StardewModdingAPI.Framework.SGame.Update(GameTime gameTime) in " +
                    $"C:\\source\\_Stardew\\SMAPI\\src\\SMAPI\\Framework\\SGame.cs:line {this.RNG.Next(1, 1611)}";
                case 1:
                    return "An error occured in the overridden draw loop: System.NullReferenceException: Object reference not set to an instance of an object." +
                    "\nat StardewValley.AnimatedSprite.UpdateSourceRect() in " +
                    $"C:\\GitlabRunner\\builds\\Gq5qA5P4\\0\\ConcernedApe\\stardewvalley\\Farmer\\Farmer\\AnimatedSprite.cs:line {this.RNG.Next(1, 527)}" +
                    "\nat StardewValley.Characters.Horse.draw(SpriteBatch b) in " +
                    $"C:\\GitlabRunner\\builds\\Gq5qA5P4\\0\\ConcernedApe\\stardewvalley\\Farmer\\Farmer\\Characters\\Horse.cs:line {this.RNG.Next(1, 715)}" +
                    "\nat StardewValley.GameLocation.drawCharacters(SpriteBatch b) in " +
                    $"C:\\GitlabRunner\\builds\\Gq5qA5P4\\0\\ConcernedApe\\stardewvalley\\Farmer\\Farmer\\Locations\\GameLocation.cs:line {this.RNG.Next(1, 10647)}" +
                    "\nat StardewValley.GameLocation.draw(SpriteBatch b) in " +
                    $"C:\\GitlabRunner\\builds\\Gq5qA5P4\\0\\ConcernedApe\\stardewvalley\\Farmer\\Farmer\\Locations\\GameLocation.cs:line {this.RNG.Next(1, 10647)}" +
                    "\nat StardewValley.Locations.BuildableGameLocation.draw(SpriteBatch b) in " +
                    $"C:\\GitlabRunner\\builds\\Gq5qA5P4\\0\\ConcernedApe\\stardewvalley\\Farmer\\Farmer\\Locations\\BuildableGameLocation.cs:line {this.RNG.Next(1, 586)}" +
                    $"\nat StardewValley.Farm.draw_PatchedBy<{ModIDs[this.RNG.Next(ModIDs.Length)]}>(Object b, SpriteBatch)" +
                    "\nat StardewModdingAPI.Framework.SGame.DrawImpl(GameTime gameTime, RenderTarget2D target_screen) in " +
                    $"C:\\source\\_Stardew\\SMAPI\\src\\SMAPI\\Framework\\SGame.cs:line {this.RNG.Next(1, 1611)}" +
                    "\nat StardewModdingAPI.Framework.SGame._draw(GameTime gameTime, RenderTarget2D target_screen) in " +
                    $"C:\\source\\_Stardew\\SMAPI\\src\\SMAPI\\Framework\\SGame.cs:line {this.RNG.Next(1, 1611)}";
                default:
                    return "An error occured in the base update loop: System.NullReferenceException: Object reference not set to an instance of an object." +
                    "\nat StardewValley.Game1.UpdateOther(GameTime time)" +
                    "\nat StardewValley.Game1._update(GameTime gameTime)" +
                    $"\nat StardewModdingAPI.Framework.SGame.Update(GameTime gameTime) in " +
                    $"C:\\source\\_Stardew\\SMAPI\\src\\SMAPI\\Framework\\SGame.cs:line {this.RNG.Next(1, 1611)}";
            }
        }

        #endregion
    }
}
