/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/hbc-mods/stardew-valley/sv-notepad
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SVNotepad
{
    public class ModEntry : Mod{

		/********************
		**	Properties
		********************/
		public static IMonitor ModMonitor;
		
		private ModConfig Config;
		//private Texture2D NotepadBackgrounds;


		/********************
		**	Public methods
		********************/

		public override void Entry( IModHelper helper ){
			/* Mod entry point, called after mod first loaded */

			this.Config = this.Helper.ReadConfig<ModConfig>();
			// this.NotepadBackgrounds = helper.Content.Load<Texture2D>("LooseSprites/letterBG", ContentSource.GameContent);
			ModMonitor = this.Monitor;

			helper.Events.Input.ButtonPressed += this.onButtonPressed;
		}


		/*********************
		**	Private methods
		*********************/

		private void onButtonPressed( object sender, ButtonPressedEventArgs e ){
			/* Raised after player presses a button on the input device */

			// Ignore if player hasn't loaded a save yet or is in a cutscene/menu
			if( !Context.IsWorldReady || !Context.IsPlayerFree ){
				return;
			}

			if( this.Config.ToggleKey.JustPressed() ){
				this.openNotepadMenu();
			}
		}

		private void openNotepadMenu(){
			/* Toggle Notepad UI */

			if( Game1.activeClickableMenu == null ){
				Game1.activeClickableMenu = new NotepadMenu( this.Helper );
			}
		}
    }
}
