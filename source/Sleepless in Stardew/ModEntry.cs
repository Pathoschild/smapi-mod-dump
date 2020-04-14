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
			helper.Events.Input.CursorMoved += this.OnCursorMoved;
			helper.Events.Input.ButtonPressed += this.OnButtonPressed;
			helper.Events.Input.ButtonReleased += this.OnButtonReleased;
			helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
			helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
			helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
			helper.Events.GameLoop.OneSecondUpdateTicking += GameLoop_OneSecondUpdateTicking;
			helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
			helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
			helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
			helper.Events.Display.RenderingHud += Display_RenderingHud;
			helper.Events.Display.RenderedHud += Display_RenderedHud;
			helper.Events.Multiplayer.PeerContextReceived += Multiplayer_PeerContextReceived;
			helper.Events.Multiplayer.PeerDisconnected += Multiplayer_PeerDisconnected;
		}

		private void Multiplayer_PeerContextReceived( object sender, PeerContextReceivedEventArgs e )
		{
			//this.Log( "Context from ID: " + e.Peer.PlayerID );
			//this.Log( Game1.IsMasterGame ? "Master" : "Not-Master" );
			//this.Log( Game1.IsMultiplayer ? "Multi" : "Single" );
			//this.Log( Game1.IsClient ? "Client" : "Not-Client" );
			//this.Log( Game1.IsServer ? "Server" : "Not-Server" );

			if( !Context.IsMainPlayer )
				return;
		}

		private void Multiplayer_PeerDisconnected( object sender, PeerDisconnectedEventArgs e )
		{
			//this.Log( "Disconnect ID: " + e.Peer.PlayerID );

			// Nintendo Switch $300 = 7500 Bells
			// $1 = 25 Bells
		}

		private Color savedEveningColor = Color.Transparent;
		private bool isLateNight = false;
		private bool isTimeFrozen = false;
		private bool hasPassedOut = false;
		private int savedTime = 0;

		private void Display_RenderingHud( object sender, RenderingHudEventArgs e )
		{
			if( isHoveringClock )
			{
				Game1.mouseCursor = 2;
			}
			if( isTimeFrozen )
			{
				savedTime = Game1.timeOfDay;
				Game1.isTimePaused = true;
			}
		}

		private void Display_RenderedHud( object sender, RenderedHudEventArgs e )
		{
			if( isTimeFrozen )
			{
				Game1.timeOfDay = savedTime;
				Game1.isTimePaused = false;
			}
		}

		private void GameLoop_UpdateTicking( object sender, UpdateTickingEventArgs e )
		{
			//if( isTimeFrozen )
			//{
			//	Game1.gameTimeInterval = 0;
			//	//Game1.UpdateGameClock( Game1.currentGameTime );

			//	// NOTE: Can use this function to skip time super fast!
			//	//Game1.performTenMinuteClockUpdate();
			//}
		}

		private void GameLoop_UpdateTicked( object sender, UpdateTickedEventArgs e )
		{
		}

		private void GameLoop_OneSecondUpdateTicking( object sender, OneSecondUpdateTickingEventArgs e )
		{
			if( !Context.IsWorldReady )
				return;

			if( !Context.IsMainPlayer )
				return;

			if( !hasPassedOut )
			{
				// Skip the pass-out check time
				if( Game1.timeOfDay >= 2550 )
				{
					Game1.timeOfDay = Game1.timeOfDay - 2400;
					this.savedEveningColor = Game1.outdoorLight;
				}
			}

			if( Game1.timeOfDay < 600 )
			{
				// Set the outdoor light til morning
				float timeTilMorning = 400f; // From 2am til 6am
				var eveningColorVec = savedEveningColor.ToVector4();
				var ambientColorVec = Game1.ambientLight.ToVector4();
				float progressTime = (float)Math.Abs( ( Game1.timeOfDay <= 600 ? Game1.timeOfDay + 2400 : Game1.timeOfDay ) - 3000 ) / timeTilMorning;
				Color progressColor = Color.Lerp( savedEveningColor, Game1.morningColor, 1.0f - progressTime );
				var progressVec = progressColor.ToVector4();
				Game1.outdoorLight = new Color( new Vector4( ambientColorVec.X * progressVec.X, ambientColorVec.Y * progressVec.Y, ambientColorVec.Z * progressVec.Z, ambientColorVec.W * progressVec.W ) );
				isLateNight = true;
			}
			else
			{
				isLateNight = false;
			}
			//this.Log( "Tick!" );
		}

		private void GameLoop_OneSecondUpdateTicked( object sender, OneSecondUpdateTickedEventArgs e )
		{
			//this.Log( "Ticked: " + e.Ticks );
			if( !Context.IsMainPlayer )
				return;
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
			//if( Game1.IsMasterGame )
			//{
			//if( Context.IsMainPlayer )
			//{
			//	Game1.timeOfDay = 540;
			//}
			//}
			//this.Log( Game1.IsMasterGame ? "Master" : "Not-Master" );
			//this.Log( Game1.IsMultiplayer ? "Multi" : "Single" );
			//this.Log( Game1.IsClient ? "Client" : "Not-Client" );
			//this.Log( Game1.IsServer ? "Sever" : "Not-Server" );
			isLateNight = false;
			hasPassedOut = false;
		}

		public void Log( string message ) {
			this.Monitor.Log( message, LogLevel.Debug );
		}

		private bool skipTimeChange = false;

		private void GameLoop_TimeChanged( object sender, TimeChangedEventArgs e )
		{
			if( !Context.IsWorldReady )
				return;

			if( Context.IsMainPlayer )
			{
				if( isTimeFrozen )
				{
					if( skipTimeChange )
					{
						skipTimeChange = false;
					}
					else
					{
						skipTimeChange = true;
						Game1.timeOfDay = e.OldTime;
					}
				}
			}

			//this.Log( $"{e.OldTime} -> {e.NewTime}" );
			if( !isTimeFrozen && isLateNight && e.OldTime == 550 && e.NewTime == 600 )
			{
				// Make it pass out if the time reached 6am
				if( Context.IsMainPlayer )
				{
					hasPassedOut = true;
					Game1.timeOfDay = 3000;//2600;
					Game1.PassOutNewDay();
					if( Context.IsMultiplayer )
					{
						Game1.server.updateLobbyData();
					}
					Game1.performTenMinuteClockUpdate();
				}
				isLateNight = false;
			}
			if( e.NewTime == 400 )
			{
				Game1.addHUDMessage( new HUDMessage( "It's getting very late...", 2 ) );
				Game1.playSound( "junimoMeep1" );
				if( Game1.IsMultiplayer )
				{
					Game1.chatBox.addInfoMessage( "LATE-NIGHT ALERT: It is 4:00am!" );
				}
			}
			if( e.NewTime == 500 )
			{
				Game1.addHUDMessage( new HUDMessage( "It's getting very very late...", 2 ) );
				Game1.playSound( "junimoMeep1" );
				if( Game1.IsMultiplayer )
				{
					Game1.chatBox.addInfoMessage( "LATE-NIGHT ALERT: It is 4:30am!" );
				}
			}
			if( e.NewTime == 530 )
			{
				Game1.addHUDMessage( new HUDMessage( "So... sleepy.....", 2 ) );
				Game1.playSound( "junimoMeep1" );
				if( Game1.IsMultiplayer )
				{
					Game1.chatBox.addInfoMessage( "LATE-NIGHT ALERT: It is 5am!" );
				}
			}
		}

		private bool isHoveringClock = false;
		private bool isClockPressed = false;

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

			if( !Context.IsPlayerFree )
				return;

			if( !Context.IsMainPlayer )
				return;

			// print button presses to the console window
			//this.Log( $"{Game1.player.Name} pressed {e.Button}." );
			//this.Log( $"{e.Cursor.ScreenPixels.X} {e.Cursor.ScreenPixels.Y} {Game1.viewport.Width} {Game1.viewport.Height}" );

			// resolution 1600x900
			// clock center is (1336, 117)
			// clock radius is 1336 - 1225 = 111
			// box left (1343, 14)
			// box rightbottom (1576, 211)
			float clockRadius = 111f / 1600f;
			Vector2 clockCenter = new Vector2( 1336f / 1600f, 117f / 900f );
			Vector2 boxTopLeft = new Vector2( 1343f / 1600f, 14f / 900f );
			Vector2 boxBottomRight = new Vector2( 1576f / 1600f, 211f / 900f );
			Vector2 cursorRelative = new Vector2( e.Cursor.ScreenPixels.X / (float)Game1.viewport.Width, e.Cursor.ScreenPixels.Y / (float)Game1.viewport.Height );

			if( e.Button.IsUseToolButton() && Vector2.DistanceSquared( cursorRelative, clockCenter ) < clockRadius * clockRadius ||
				( boxTopLeft.X <= cursorRelative.X && cursorRelative.X <= boxBottomRight.X &&
				  boxTopLeft.Y <= cursorRelative.Y && cursorRelative.Y <= boxBottomRight.Y ) )
			{
				this.Helper.Input.Suppress( SButton.MouseLeft );
				isClockPressed = true;
			}
		}

		private void OnButtonReleased( object sender, ButtonReleasedEventArgs e )
		{
			if( !Context.IsWorldReady )
				return;

			if( !Context.IsMainPlayer )
				return;

			if( isClockPressed && e.Button.IsUseToolButton() )
			{
				float clockRadius = 111f / 1600f;
				Vector2 clockCenter = new Vector2( 1336f / 1600f, 117f / 900f );
				Vector2 boxTopLeft = new Vector2( 1343f / 1600f, 14f / 900f );
				Vector2 boxBottomRight = new Vector2( 1576f / 1600f, 211f / 900f );
				Vector2 cursorRelative = new Vector2( e.Cursor.ScreenPixels.X / (float)Game1.viewport.Width, e.Cursor.ScreenPixels.Y / (float)Game1.viewport.Height );

				if( Vector2.DistanceSquared( cursorRelative, clockCenter ) < clockRadius * clockRadius ||
					( boxTopLeft.X <= cursorRelative.X && cursorRelative.X <= boxBottomRight.X &&
					  boxTopLeft.Y <= cursorRelative.Y && cursorRelative.Y <= boxBottomRight.Y ) )
				{
					//Game1.isTimePaused = !Game1.isTimePaused;
					isTimeFrozen = !isTimeFrozen;
					Game1.addHUDMessage( new HUDMessage( isTimeFrozen ? "Time is stopped." : "Time is flowing.", 2 ) );
					Game1.playSound( "junimoMeep1" );
				}
				isClockPressed = false;
			}
		}

		private void OnCursorMoved( object sender, CursorMovedEventArgs e )
		{
			isHoveringClock = false;

			if( !Context.IsWorldReady )
				return;

			if( !Context.IsPlayerFree )
				return;

			if( !Context.IsMainPlayer )
				return;

			float clockRadius = 111f / 1600f;
			Vector2 clockCenter = new Vector2( 1336f / 1600f, 117f / 900f );
			Vector2 boxTopLeft = new Vector2( 1343f / 1600f, 14f / 900f );
			Vector2 boxBottomRight = new Vector2( 1576f / 1600f, 211f / 900f );
			Vector2 cursorRelative = new Vector2( e.NewPosition.ScreenPixels.X / (float)Game1.viewport.Width, e.NewPosition.ScreenPixels.Y / (float)Game1.viewport.Height );

			if( Vector2.DistanceSquared( cursorRelative, clockCenter ) < clockRadius * clockRadius ||
				( boxTopLeft.X <= cursorRelative.X && cursorRelative.X <= boxBottomRight.X &&
				  boxTopLeft.Y <= cursorRelative.Y && cursorRelative.Y <= boxBottomRight.Y ) )
			//if( Vector2.DistanceSquared( cursorRelative, clockCenter ) < clockRadius * clockRadius )
			{
				isHoveringClock = true;
			}
			//this.Log( $"{Game1.mouse}" );
		}
	}
}
