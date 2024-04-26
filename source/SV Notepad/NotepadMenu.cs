/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/hbc-mods/stardew-valley/sv-notepad
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley.BellsAndWhistles;
namespace SVNotepad {
	class NotepadMenu : IClickableMenu {

		/********************
		**	Properties
		********************/
		private ClickableTextureComponent DeleteButton;
		private ClickableTextureComponent NextButton;
		private ClickableTextureComponent OkButton;
		private ClickableTextureComponent PrevButton;
		private IModHelper helper;
		private int activeNoteIndex = -1;
		private int buttonRowHeight = Game1.tileSize;
		private readonly List<ClickableTextureComponent> ActionButtons = new List<ClickableTextureComponent>();
		private List<Note> Notes = new List<Note>();
		private string modDirectory;
		private string saveKey = "herebecush-svnotepad";
		private TextField NoteText;
		private TextField NoteTitle;


		/******************
		** Static methods
		******************/
		public static void drawDialogueBox( int x, int y, int width, int height ){
			/* Draws a dialogue box whose bounds are at the specified dimensions, reverting
			the default SV offsets applied to the dialogue box drawing function */
			Game1.drawDialogueBox( x - 16, y - 80, width + 26 , height + 95, false, true );
		}


		/******************
		**	Constructors
		******************/
		public NotepadMenu( IModHelper helper ) : base( 0, 0, Game1.viewport.Width - DayTimeMoneyBox.width, Game1.viewport.Height ){
			this.helper = helper;
			this.modDirectory = this.helper.DirectoryPath;

			this.setTransform();
			this.loadNotes();
			this.prepareActiveNote();
			this.NoteText.Selected = true; // Put the selection cursor in the note text field by default
		}

		/********************
		**	Public methods
		********************/
		public override void draw( SpriteBatch b ){
			/* Draw menu to the screen */

			// Menu background
			NotepadMenu.drawDialogueBox( this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
			
			// Title background
			if( !this.NoteTitle.ScrollBG ){
				NotepadMenu.drawDialogueBox( this.NoteTitle.X, this.NoteTitle.Y, this.NoteTitle.Width, this.NoteTitle.Height );
			}
			this.NoteTitle.draw( b );

			// Text background
			if( !this.NoteText.ScrollBG ){
				NotepadMenu.drawDialogueBox( this.NoteText.X, this.NoteText.Y, this.NoteText.Width, this.NoteText.Height );
			}
			this.NoteText.draw( b );
			
			foreach( ClickableTextureComponent button in this.ActionButtons ){
				button.draw( b );
			}

			this.drawMouse( b );
		}

		public void log( string message, LogLevel level = LogLevel.Debug ){
			ModEntry.ModMonitor.Log( $"[NotepadMenu] >> {message}", level );
		}
		
		public void setTransform(){
			this.xPositionOnScreen = 0;
			this.yPositionOnScreen = 0;
			this.width = (int)( ( Game1.viewport.Width - DayTimeMoneyBox.width ) * Game1.options.zoomLevel );
			this.height = (int)( Game1.viewport.Height * Game1.options.zoomLevel );
		}

		public override void gameWindowSizeChanged( Rectangle oldBounds, Rectangle newBounds ){
			base.gameWindowSizeChanged(oldBounds, newBounds);
			this.setTransform();
			this.prepareNoteFields();
			this.initialize( this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, true );
		}

		public override void receiveKeyPress( Keys key ){
			/* Handle special key presses */
			
			int value = (int)key;
		
			switch( value ){
				// Keys to pass straight through to the active text field
				// case 13 (Enter) is handled by TextField.cs
				case 9:		// Tab
				case 37:	// Left arrow
				case 38:	// Up arrow
				case 39:	// Right arrow
				case 40:	// Down arrow
				case 46:	// Delete
				case 121:	// F10
					if( this.NoteTitle.Selected ){
						this.NoteTitle.processSpecialKey( value );
					}else if( this.NoteText.Selected ){
						this.NoteText.processSpecialKey( value );
						this.log( $"receiveKeyPress >> note CursorPos AFTER special key input: {this.NoteText.CursorPos}" );
					}
					return;

				// Escape - save + close via 'ok' action
				case 27:
					this.doAction( "ok" );
					return;
			}
		}
		
		public override void receiveLeftClick( int x, int y, bool playSound = true ){
			foreach( ClickableTextureComponent button in this.ActionButtons ){
				if( button.containsPoint( x, y ) ){
					this.doAction( button.name.ToLower() );
					return;
				}
			}

			if( this.NoteTitle.isWithinBounds( x, y ) ){
				this.NoteTitle.Selected = true;
				this.NoteText.Selected = false;
				return;
			}

			if( this.isWithinBounds( x, y ) ){
				this.NoteText.Selected = true;
				this.NoteTitle.Selected = false;
				this.NoteText.receiveLeftClick( x, y );
			}
		}


		/*********************
		**	Private methods
		*********************/
		private void clearNote(){
			this.helper.Data.WriteSaveData<ModData>( this.saveKey, null );
			this.NoteTitle.Text = "";
			
			if( !(this.NoteText is null) ){
				this.NoteText.Text = "";
			}

			if( this.activeNoteIndex >= 0 ){
				this.Notes.ElementAt( this.activeNoteIndex ).title = "";
				this.Notes.ElementAt( this.activeNoteIndex ).text = "";
				this.activeNoteIndex = -1;
			}
		}

		private void doAction( string action ){
			/* Action handler - will receive the lowercase name of the clicked action button */
			
			switch( action ){
				case "ok":
					this.saveNote();
					this.exitThisMenu();
					//Game1.addHUDMessage( new HUDMessage( "Warning: Changes to your notepad are only saved when you sleep!", "" ) );
					Game1.addHUDMessage( new HUDMessage( "Warning: Changes to your notepad are only saved when you sleep!" ) );
					return;
				
				case "delete":
					this.clearNote();
					return;
			}
		}

		private void loadNotes(){
			var saved = this.helper.Data.ReadSaveData<ModData>( this.saveKey );

			if( !( saved is null ) ){
				this.Notes = saved.Notes;
			}else{
				this.Notes = new List<Note>();
			}
		}

		private void prepareActiveNote( int index = 0 ){
			if( !this.Notes.Any() ){
				this.activeNoteIndex = -1;
			}else{
				if( index >= this.Notes.Count ){
					index = this.Notes.Count - 1;
				}

				this.activeNoteIndex = index;
			}
			this.prepareNoteFields();
		}

		private void prepareButtons( int Y = 0 ){
			if( 0 == Y ){
				return;
			}

			int buttonWidth		= this.buttonRowHeight;
			int buttonHeight	= buttonWidth;

			this.OkButton = new ClickableTextureComponent( "Ok", new Rectangle(this.xPositionOnScreen + ( this.width / 2 ) - ( buttonWidth * 2 ), Y, buttonWidth, buttonHeight), "", null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
			this.DeleteButton = new ClickableTextureComponent( "Delete", new Rectangle( this.xPositionOnScreen + ( this.width / 2 ) + buttonWidth, Y, buttonWidth, buttonHeight), "", null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, 47), 1f );
			this.PrevButton = new ClickableTextureComponent( "Prev", new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder, Y, buttonWidth, buttonHeight), "", "Next Note", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
			this.NextButton = new ClickableTextureComponent( "Next", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, Y, buttonWidth, buttonHeight), "", "Previous Note", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);
		
			this.ActionButtons.Clear();
			this.ActionButtons.Add( this.OkButton );
			this.ActionButtons.Add( this.DeleteButton );
		}

		private void prepareNoteFields(){
			Note activeNote;

			if( this.activeNoteIndex < 0 ){
				activeNote = new Note();
			}else{
				activeNote = this.Notes[activeNoteIndex];
			}

			// Keep existing field values in case we're just resizing the window
			string titleText, textText;

			if( ( this.NoteTitle is null ) || ( this.NoteTitle.Text is null ) ){
				titleText = activeNote.title;
			}else{
				titleText = this.NoteTitle.Text;
			}
			if( ( this.NoteText is null ) || ( this.NoteText.Text is null ) ){
				textText = activeNote.text;
			}else{
				textText = this.NoteText.Text;
			}

			int fieldX		= xPositionOnScreen + IClickableMenu.borderWidth;
			int titleY		= yPositionOnScreen + IClickableMenu.borderWidth;
			int fieldWidth	= this.width - ( 2 * ( fieldX - xPositionOnScreen )); // Ensures same gap on each side of the field
			int fieldHeight = SpriteText.getHeightOfString( "69" ) * 2;

			int textY		= titleY + fieldHeight + ( IClickableMenu.borderWidth / 2 );
			int textHeight	= this.height - textY - this.buttonRowHeight - (3 * ( IClickableMenu.borderWidth / 2 ) );
			

			this.NoteTitle = new TextField( fieldX, titleY, fieldWidth, fieldHeight, "NoteTitle", titleText );
			this.NoteTitle.maxlength = 35;

			this.NoteText = new TextField( fieldX, textY, fieldWidth, textHeight, "NoteText", textText );
			this.NoteText.multiline = true;

			this.prepareButtons( textY + textHeight + ( IClickableMenu.borderWidth / 2 ) );
		}

		private void saveNote(){
			ModData modData = new ModData();

			// Update active note's info if not empty
			if( !String.IsNullOrEmpty( this.NoteTitle.Text ) || !String.IsNullOrEmpty( this.NoteText.Text ) ){

				// Default title
				if( String.IsNullOrEmpty( this.NoteTitle.Text ) ){
					this.NoteTitle.Text = "Untitled";
				}

				if( this.activeNoteIndex < 0 ){
					this.Notes.Add( new Note( this.NoteTitle.Text, this.NoteText.Text ) );
				}else{
					this.Notes.ElementAt( this.activeNoteIndex ).title = this.NoteTitle.Text;
					this.Notes.ElementAt( this.activeNoteIndex ).text = this.NoteText.Text;
				}
			}

			// Remove empty notes
			if( this.Notes.Any() ){
				for( int n = this.Notes.Count - 1; n >= 0; n-- ){
					Note note = this.Notes[n];

					// Remove any empty + unedited notes
					if( ( String.IsNullOrEmpty( note.title ) && String.IsNullOrEmpty( note.text ) ) || ( note.title == note.defaultTitle ) && ( note.text == note.defaultText ) ){
						this.Notes.RemoveAt( n );
					}else{
						this.Notes[n].preSave();
					}
				}
			}

			modData.Notes = this.Notes;		
			this.helper.Data.WriteSaveData<ModData>( this.saveKey, modData );
		}
	}
}