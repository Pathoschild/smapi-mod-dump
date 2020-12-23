/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/instafluff/SpelldewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

// Detective Mod Idears
// - Stardewmong Us: Mark a character each day and then find out if they were right
// - Spelling Dew: Each character gives you a letter that spells where the missing person is hiding
// - Carmen Stardewego: Each character gives you a hint to another character that eventually leads to the missing person
// - Stardew Raider: Each character gives you a little hint to the location of a relic
// - Spelling Dew Valley: Each character gives you a letter that spells which person is the right one. And target person gives you a bad letter

// IDEARS!
// LowlandVik: Is there anyway to include something like "The person name includes chr() and last seen near <direction>?"
// LowlandVik: Or maybe how close a person is to another npc =\

// THE DETECTIVE PLAN
// - the suspect list
// - lots of hiding locations
// - times/schedules for characters
// - different dialogue clues
// - gift someone to mark found!

// Characters: Abigail, Alex, Caroline, Clint, Demetrius, Elliott, Emily, Evelyn, George, Gus, Haley, Harvey, Jas, Jodi, Leah, Lewis, Linus, Marnie, Maru, Pam, Penny, Pierre, Robin, Sam, Sebastian, Shane, Vincent, Willy

/* Example:
 * Abigail - I think they talked to Gus about something?
 * Gus - They seemed to be interested in going to a
 * 
*/



// FUTURE:
// - items/clues for list


namespace SpelldewValley
{
    public class ModEntry : Mod
    {
        public override void Entry( IModHelper helper )
        {
			helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
			helper.Events.Input.ButtonPressed += Input_ButtonPressed;
			helper.Events.Input.ButtonReleased += Input_ButtonReleased;
			helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
			helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;

			//// create with random manifest data
			//IContentPack contentPack = this.Helper.ContentPacks.CreateFake(
			//   directoryPath: Path.Combine( this.Helper.DirectoryPath, "content-pack" ),
			//);

			//// create with given manifest fields
			//IContentPack contentPack = this.Helper.ContentPacks.CreateTemporary(
			//   directoryPath: Path.Combine( this.Helper.DirectoryPath, "content-pack" ),
			//   id: Guid.NewGuid().ToString( "N" ),
			//   name: "temporary content pack",
			//   description: "...",
			//   author: "...",
			//   version: new SemanticVersion( 1, 0, 0 )
			//);
		}

		Random random = new Random();
		string[] characters = new string[] { "Abigail", "Alex", "Caroline", "Clint", "Demetrius", "Elliott", "Emily", "Evelyn", "George", "Gus", "Haley", "Harvey", "Jas", "Jodi", "Leah", "Lewis", "Linus", "Marnie", "Maru", "Pam", "Penny", "Pierre", "Robin", "Sam", "Sebastian", "Shane", "Vincent", "Willy" };
		string[] numbers = new string[] { "Zero", "One", "Two", "Three", "Four", "Five" };
		Dictionary<string, string> dialogue = new Dictionary<string, string>();

		private void GameLoop_GameLaunched( object sender, GameLaunchedEventArgs e )
		{
			// TODO: Try setting a schedule and see if it can trigger a location-specific dialogue!

			Console.WriteLine( "Game Has Launched" );

			var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>( "Pathoschild.ContentPatcher" );
			api.RegisterToken( this.ModManifest, "PlayerName", () =>
			{
				if( Context.IsWorldReady )
					return new[] { Game1.player.Name };
				if( SaveGame.loaded?.player != null )
					return new[] { SaveGame.loaded.player.Name }; // lets token be used before save is fully loaded
				return null;
			} );

			var SUSPECT = characters[ random.Next( characters.Length ) ].ToLower();
			var letters = SUSPECT.ToCharArray().OrderBy( x => random.Next() ).ToArray();
			var index = 0;

			foreach( var character in characters )
			{
				//dialogue.Add( character, string.Format( "TESTING {0} DIALOGUE", character ) );
				if( character.ToLower() == SUSPECT )
				{
					dialogue.Add( character, "Aww you found me!" );
				}
				else
				{
					dialogue.Add( character, string.Format( "The suspect's name includes the letter {0}", letters[ index ] ) );
				}
				api.RegisterToken( this.ModManifest, character + "Intro", () => {
					var ch = character;
					if( Context.IsWorldReady )
						return new[] { dialogue[ ch ] };
					if( SaveGame.loaded?.player != null )
						return new[] { dialogue[ ch ] }; // lets token be used before save is fully loaded
					return null;
				} );
				api.RegisterToken( this.ModManifest, character + "Rain", () => {
					var ch = character;
					if( Context.IsWorldReady )
						return new[] { dialogue[ ch ] };
					if( SaveGame.loaded?.player != null )
						return new[] { dialogue[ ch ] }; // lets token be used before save is fully loaded
					return null;
				} );
				for( var i = 0; i < 6; i++ )
				{
					api.RegisterToken( this.ModManifest, character + numbers[ i ], () => {
						var ch = character;
						if( Context.IsWorldReady )
							return new[] { dialogue[ ch ] };
						if( SaveGame.loaded?.player != null )
							return new[] { dialogue[ ch ] }; // lets token be used before save is fully loaded
						return null;
					} );
				}
				index = ( index + 1 ) % letters.Length;
			}

			foreach( IContentPack contentPack in this.Helper.ContentPacks.GetOwned() )
			{
				this.Monitor.Log( $"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}" );
			}
		}

		private void Input_ButtonPressed( object sender, ButtonPressedEventArgs e )
		{
			if( Context.IsWorldReady && Context.CanPlayerMove )
			{
			}
		}

		private void Input_ButtonReleased( object sender, ButtonReleasedEventArgs e )
		{
			if( Context.IsWorldReady && Context.CanPlayerMove )
			{
			}
		}

		private void GameLoop_UpdateTicking( object sender, UpdateTickingEventArgs e )
		{
			if( Context.IsWorldReady && Context.CanPlayerMove )
			{
			}
		}

		private void GameLoop_UpdateTicked( object sender, UpdateTickedEventArgs e )
		{
			if( Context.IsWorldReady && Context.CanPlayerMove )
			{
			}
		}
	}
}
