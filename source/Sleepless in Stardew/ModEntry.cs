using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SleeplessInStardew
{
	public class ModEntry : Mod
	{
		/*********
        ** Public methods
        *********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry( IModHelper helper )
		{
			helper.Events.Input.ButtonPressed += this.OnButtonPressed;
			helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
			helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
			helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
			helper.Events.GameLoop.OneSecondUpdateTicking += GameLoop_OneSecondUpdateTicking;
			helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
			helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
		}

		private Color savedEveningColor = Color.Transparent;
		private bool isLateNight = false;

		private void GameLoop_UpdateTicked( object sender, UpdateTickedEventArgs e )
		{
		}

		private void GameLoop_UpdateTicking( object sender, UpdateTickingEventArgs e )
		{
		}

		private void GameLoop_OneSecondUpdateTicking( object sender, OneSecondUpdateTickingEventArgs e )
		{
			if( Context.IsWorldReady )
			{
				// Skip the pass-out check time
				if( Game1.timeOfDay >= 2550 )
				{
					Game1.timeOfDay = Game1.timeOfDay - 2400;
					this.savedEveningColor = Game1.outdoorLight;
					isLateNight = true;
				}

				if( isLateNight && Game1.timeOfDay == 600 )
				{
					// Make it pass out if the time reached 6am
					Game1.timeOfDay = Game1.timeOfDay + 2400;
				}
				else if( Game1.timeOfDay < 600 )
				{
					// Set the outdoor light til morning
					float timeTilMorning = 400f; // From 2am til 6am
					var eveningColorVec = savedEveningColor.ToVector4();
					var ambientColorVec = Game1.ambientLight.ToVector4();
					float progressTime = (float)Math.Abs( ( Game1.timeOfDay <= 600 ? Game1.timeOfDay + 2400 : Game1.timeOfDay ) - 3000 ) / timeTilMorning;
					Color progressColor = Color.Lerp( savedEveningColor, Game1.morningColor, 1.0f - progressTime );
					var progressVec = progressColor.ToVector4();
					Game1.outdoorLight = new Color( new Vector4( ambientColorVec.X * progressVec.X, ambientColorVec.Y * progressVec.Y, ambientColorVec.Z * progressVec.Z, ambientColorVec.W * progressVec.W ) );
				}
			}
			//this.Log( "Tick!" );
		}

		private void GameLoop_DayEnding( object sender, DayEndingEventArgs e )
		{
			//this.Log( "Day Ending" );
		}

		private void GameLoop_DayStarted( object sender, DayStartedEventArgs e )
		{
			//this.Log( "Day Started" );
			//this.Log( Game1.outdoorLight.ToString() );
			//Game1.timeOfDay = 2550;
			isLateNight = false;
		}

		public void Log( string message ) {
			this.Monitor.Log( message, LogLevel.Debug );
		}

		private void GameLoop_TimeChanged( object sender, TimeChangedEventArgs e )
		{
			//this.Log( $"{e.OldTime} -> {e.NewTime}" );
		}


		/*********
        ** Private methods
        *********/
		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed( object sender, ButtonPressedEventArgs e )
		{
			// ignore if player hasn't loaded a save yet
			if( !Context.IsWorldReady )
				return;

			// print button presses to the console window
			//this.Log( $"{Game1.player.Name} pressed {e.Button}." );
		}
	}
}
