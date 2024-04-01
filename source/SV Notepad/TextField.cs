/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/hbc-mods/stardew-valley
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using System.Xml;
using System.Runtime.Serialization;

/*
	Inspired by the TextField class from MissCoriel's "Dear Diary" mod:
	https://github.com/MissCoriel/Dear-Diary
*/

namespace SVNotepad {
	public class TextField : IKeyboardSubscriber {

		/********************
		**	Properties
		********************/
		public bool logging { get; set; } = true;
		public bool multiline { get; set; }
		public bool required { get; set; }
		public bool ScrollBG = false;
		public bool Selected {
			get { return selected; }
			set {
				selected = value;

				if( selected ) {
					Text = Text.Replace( this.cursor, "" );
					Game1.keyboardDispatcher.Subscriber = this;

					if( -1 == this.cursorPos ){
						this.cursorPos = Text.Length;
						this.resetCursorDisplay();
					}

					this.cursorShowing = true;

				} else {
					this.cursorPos = -1;
					this.cursorShowing = false;
				}
			}
		}    

		public int Height { get; set; }
		public int maxlength { get; set; }
		public int Width { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public int contentX;
		public int contentY;
		public int contentWidth;
		public int contentHeight;

		public int CursorPos {
			get { return cursorPos; }
		}

		public string defaultText = "Hello, World!";
		public Dictionary<int,string> Lines { get; set; } = new Dictionary<int, string>(); // Holds <starting position, text> for each line of text
		public string Name { get; set; } = "TextField";
		public string Text { 
			get { return text; }
			set {
				Lines = this.getAllLines();
				text = value;
			}
		}
		public int TextColour { get; set; } = 0;


		private string cursor = "|";
		private int cursorHideTicks = 24;
		private int cursorPos = -1;
		private bool cursorShowing = false;
		private int cursorShowTicks = 35;
		private int cursorTicks = 0;		
		private string newLine = SpriteText.newLine.ToString(); //
		private bool selected = false;
		private string tabInput = "		";
		private string text = "";
		

		/******************
		**	Constructors
		******************/
		public TextField() {
			Text = "";
			X = 0;
			Y = 0;
			Width = 0;
			Height = 0;
			contentX = 0;
			contentY = 0;
			contentWidth = 0;
			contentHeight = 0;

			this.init();
		}

		public TextField( int x, int y, int width, int height, string name, string text = null, bool scrollBG = false, int textColour = 0 ) {
			if( text is null ){
				text = this.defaultText;	
			}
			Text = text;

			if( scrollBG ){
				Width = width - ( IClickableMenu.borderWidth / 2);
				X = x + ( IClickableMenu.borderWidth / 3 );
			}else{
				Width = width;
				X = x;
			}
			Y = y;
			Height = height;
			Name = name;
			ScrollBG = scrollBG;
			TextColour = textColour;

			// Content (safe) area of the textbox (where the text actually appears)
			this.contentX = this.X + SpriteText.getWidthOfString( "X" );
			this.contentY = this.Y + (( 2 * SpriteText.getHeightOfString( "69" ) ) / 4);
			this.contentWidth = this.Width - ( contentX - this.X ); // Ensures same padding both sides
			this.contentHeight = this.Height - ( contentY - this.Y ); // Ensures same padding top/bottom

			this.init();
		}


		/********************
		**	Public methods
		********************/
		public virtual void draw( SpriteBatch spriteBatch ){

			string drawText;

			// Include cursor if field selected
			if( selected ){

				if( this.cursorShowing && ( this.cursorTicks >= this.cursorShowTicks ) ){
					this.cursorShowing = false;
					this.cursorTicks = 0;
				}else if( !this.cursorShowing && ( this.cursorTicks >= this.cursorHideTicks ) ){
					this.cursorShowing = true;
					this.cursorTicks = 0;
				}

				/*
					Because the cursor is an extra character, when it blinks (is added/removed) it
					can cause text to wrap/unrap repeatedly when at the end of the line

					To fix this, we draw a phantom character in place of the cursor when it's in the
					'hide' phase of its blinking cycle. We do _not_ use a space for this because
					that plays havoc with the line overflow checking
				*/
				drawText = this.addTextAtCursor( this.cursorShowing ? this.cursor : "£", false );
				
			}else{
				drawText = Text;
			}

			// SpriteText.drawString( spriteBatch, drawText, this.contentX, this.contentY, 999999, this.contentWidth, this.contentHeight, 0.75f, 0.865f, false, ScrollBG ? 0 : -1, "", TextColour );
			SpriteText.drawString( spriteBatch, drawText, this.contentX, this.contentY, 999999, this.contentWidth, this.contentHeight, 0.75f, 0.865f, false, ScrollBG ? 0 : -1, "", null );

			this.cursorTicks++;
		}

		public void init(){
			this.maxlength = 0;
			this.multiline = false;
			this.required = false;
		}

		public bool isWithinBounds( int x, int y ){
			return( 
				(x >= this.X )
				&& ( x <= ( this.X + this.Width ) )
				&& ( y > this.Y )
				&& ( y <= ( this.Y + this.Height ) )
			);
		}

		public void log( string message, LogLevel level = LogLevel.Debug ){
			if( this.logging ){
				ModEntry.ModMonitor.Log( $"[{Name}] >> {message}", level );
			}
		}

		public bool wouldBeTooHighWith( string newText ){
			return SpriteText.getHeightOfString( this.addTextAtCursor( newText, false ), this.contentWidth ) > this.contentHeight;
		}

		public bool lineWouldBeTooLongWith( string newText ){
			return ( SpriteText.getWidthOfString( this.getLineFromText( this.addTextAtCursor( newText, false ), this.cursorPos + newText.Length ) ) > ( this.contentWidth - Game1.pixelZoom - SpriteText.getWidthOfString( this.cursor ) ) );
		}

		public void receiveLeftClick( int x, int y ){
			int localX = x - this.X;
			int localY = y - this.Y;

			this.log( $"receiveLeftClick >> click received at ({x},{y}), local coords: ({localX},{localY})" );

			int clickedLinePos = -1;
			int clickedColumn = -1;

			int textHeight = 0;
			foreach( KeyValuePair<int,string> line in Lines ){
				textHeight += SpriteText.getHeightOfString( line.Value );
				if( textHeight >= localY ){
					clickedLinePos = line.Key;
					break;
				}
			}
			this.log( $"receiveLeftClick >> clickedLinePos: {clickedLinePos}" );

			// Bail if we couldn't determine the line clicked
			if( clickedLinePos < 0 ){
				return;
			}

			this.log( $"receiveLeftClick >> clicked line text: {Lines[clickedLinePos]}" );

			int textWidth = 0;
			int charactersChecked = 0;
			foreach( char c in Lines[clickedLinePos] ){
				textWidth += SpriteText.getWidthOfString( c.ToString() );

				if( textWidth >= localX ){
					clickedColumn = charactersChecked;
					break;
				}

				charactersChecked++;
			}
			this.log( $"receiveLeftClick >> clickedColumn: {clickedColumn}" );

			int clampedColumnPos = Math.Clamp( clickedColumn, 0, this.getLineLength( clickedLinePos ) );
			
			this.log( $"receiveLeftClick >> clampedColumnPos: {clampedColumnPos}" );
			
			this.cursorPos = clickedLinePos + clampedColumnPos;

			this.log( $"receiveLeftClick >> cursorPos is now {this.cursorPos}" );
		}

		public void processSpecialKey( int value ){
			this.log( $"cursorPos BEFORE special key input: {this.cursorPos}" );
			switch( value ){
				case 8: // Backspace
					if( ( Text.Length == 0 ) || ( this.cursorPos == 0 ) ){
						return;
					}

					if( this.isTabNext( false ) ){
						this.backspace( this.getSVTabLength() );
						return;
					}

					this.backspace();
					return;

				case 9: // Tab
					this.ReceiveTextInput( this.tabInput );
					return;

				case 13: // Enter
					if( this.multiline ){
						/*
							Checks with a fake character entry after the newline
							character because SpriteText.getHeightOfString strips
							off newline characters
						*/
						if( !this.wouldBeTooHighWith( $"{this.newLine}X" ) ){
							Text = this.addTextAtCursor( this.newLine );
						}
					}
					return;

				case 37: // Left arrow
					if( selected && ( this.cursorPos > 0 ) ){
						if( this.isTabNext( false ) ){
							this.cursorPos -= this.getSVTabLength();
							return;
						}
						
						this.cursorPos--;
					}
					return;

				case 38: // Up arrow
					if( selected && ( this.cursorPos > 0 ) ){

						// If on first line, simply move to start
						if( ( Lines.Count < 2 ) || ( this.cursorPos < Lines.Keys.ElementAt( 1 ) ) ){
							this.cursorPos = 0;
							this.log( $"processSpecialKey >> Up arrow >> to the start!" );
							return;
						}

						// Move up by one line (keeping the same character offset from the start)
						// TODO: This may have to check for character width for a more accurate "directly up" movement
						this.log( $"processSpecialKey >> Up arrow >> cursorPos before: {this.cursorPos}", LogLevel.Info );
						int lineStartPosition = this.getLineStartPosition();
						int distanceFromLineStart = this.cursorPos - lineStartPosition;
						int previousLineStartPosition = this.getPreviousLineStartPosition();
						int previousLineLength = this.getLineLength( previousLineStartPosition );

						// If at the beginning of a line (or the previous line is 0 in length), simply go to the start of the one before
						if( distanceFromLineStart <= 0 || previousLineLength <= 1 ){
							this.log( $"processSpecialKey >> Up arrow >> going to start of previous line", LogLevel.Info );
							this.cursorPos = ( previousLineStartPosition >= 0 ) ? previousLineStartPosition : 0; // Don't go beyond start of text
							return;
						}

						// If adding distance would take us over the previous line, stop at end of previous line
						if( previousLineStartPosition + distanceFromLineStart > lineStartPosition ){
							this.log( $"processSpecialKey >> Up arrow >> avoiding exceeding previous line", LogLevel.Warn );
							this.cursorPos = ( lineStartPosition > 0 ) ? lineStartPosition - 1 : 0; // Don't go beyond start of text
						}else{
							this.cursorPos = previousLineStartPosition + distanceFromLineStart;
						}

						this.log( $"processSpecialKey >> Up arrow >> lineStartPosition: {lineStartPosition}", LogLevel.Info );
						this.log( $"processSpecialKey >> Up arrow >> previousLineStartPosition: {previousLineStartPosition}", LogLevel.Info );
						this.log( $"processSpecialKey >> Up arrow >> distanceFromLineStart: {distanceFromLineStart}", LogLevel.Info );
						this.log( $"processSpecialKey >> Up arrow >> cursorPos after: {this.cursorPos}", LogLevel.Info );
						
					}
					return;

				case 39: // Right arrow
					if( selected && ( this.cursorPos < Text.Length ) ){
						if( this.isTabNext() ){
							this.cursorPos += this.getSVTabLength();
							return;
						}
						
						this.cursorPos++;
					}
					return;

				case 40: // Down arrow
					if( selected && ( this.cursorPos < Text.Length ) ){

						// If on last line, simply move to end
						if( ( Lines.Count < 2 ) || ( this.cursorPos >= Lines.Keys.ElementAt( Lines.Count - 1 ) ) ){
							this.cursorPos = Text.Length;
							return;
						}

						// Move down by one line (keeping the same character offset from the start)
						// TODO: This may have to check for character width for a more accurate "directly down" movement
						this.log( $"processSpecialKey >> Down arrow >> cursorPos before: {this.cursorPos}", LogLevel.Info );
						int lineStartPosition = this.getLineStartPosition();
						int distanceFromLineStart = this.cursorPos - lineStartPosition;
						int distanceFromLineEnd = this.getLineEndPosition() - this.cursorPos;
						int nextLineStartPosition = this.getNextLineStartPosition();
						int nextLineEndPosition = this.getLineEndPosition( -1, 1 );

						// If at the end of a line, simply move along by 1
						if( distanceFromLineEnd <= 0 ){
							this.log( $"processSpecialKey >> Down arrow >> going to end of next line", LogLevel.Info );
							this.cursorPos = ( nextLineStartPosition < Text.Length - 1 ) ? nextLineStartPosition : Text.Length - 1; // Don't go beyond end of text
							return;
						}

						// If adding distance would take us past the end of the next line, stop at the end of that line
						if( nextLineStartPosition + distanceFromLineStart > nextLineEndPosition ){
							this.cursorPos = nextLineEndPosition;
							return;
						}

						// If adding distance would take us past the end of the text, stop at the end of the text
						this.cursorPos = Math.Min( Text.Length - 1, nextLineStartPosition + distanceFromLineStart );

						this.log( $"processSpecialKey >> Down arrow >> lineStartPosition: {lineStartPosition}", LogLevel.Info );
						this.log( $"processSpecialKey >> Down arrow >> distanceFromLineStart: {distanceFromLineStart}", LogLevel.Info );
						this.log( $"processSpecialKey >> Down arrow >> distanceFromLineEnd: {distanceFromLineEnd}", LogLevel.Info );
						this.log( $"processSpecialKey >> Down arrow >> nextLineStartPosition: {nextLineStartPosition}", LogLevel.Info );
						this.log( $"processSpecialKey >> Down arrow >> cursorPos after: {this.cursorPos}", LogLevel.Info );

					}
					return;

				case 46: // Delete
					if( ( 0 == Text.Length ) || ( this.cursorPos >= Text.Length ) ){
						return;
					}

					if( this.isTabNext() ){
						this.deleteText( this.getSVTabLength() );
						return;
					}

					this.deleteText();
					return;
				
				case 112: // F1
					if( TextColour == 9 ){
						TextColour = 0;
						return;
					}
					
					TextColour = TextColour + 1;
					return;

				case 121: // F10
					// Debug key to print info to the log
					// C# is apparently broken so we have to rename these vars with a "d" prefix to avoid false naming clashes
					int dLineStartPosition	= this.getLineStartPosition();
					int lineEndPosition		= this.getLineEndPosition();
					int lineLength			= this.getLineLength();

					int previousLineStart	= this.getPreviousLineStartPosition();
					int previousLineEnd		= this.getLineEndPosition( -1, -1 );
					int dPreviousLineLength	= this.getLineLength( previousLineStart );

					int nextLineStart	= this.getNextLineStartPosition();
					int nextLineEnd		= this.getLineEndPosition( -1, 1 );
					int nextLineLength	= this.getLineLength( nextLineStart );

					this.log( $"processSpecialKey >> cursorPos: {this.cursorPos}", LogLevel.Info );
					this.log( $"processSpecialKey >> line start: {dLineStartPosition}", LogLevel.Info );
					this.log( $"processSpecialKey >> line end: {lineEndPosition}", LogLevel.Info );
					this.log( $"processSpecialKey >> line length: {lineLength}" );
					this.log( $"processSpecialKey >> previous line start: {previousLineStart}", LogLevel.Info );
					this.log( $"processSpecialKey >> previous line end: {previousLineEnd}", LogLevel.Info );
					this.log( $"processSpecialKey >> previous line length: {dPreviousLineLength}" );
					this.log( $"processSpecialKey >> next line start: {nextLineStart}", LogLevel.Info );
					this.log( $"processSpecialKey >> next line end: {nextLineEnd}", LogLevel.Info );
					this.log( $"processSpecialKey >> next line length: {nextLineLength}", LogLevel.Info );

					return;
			}
		}

		public void ReceiveCommandInput( char command ){
			this.processSpecialKey( (int)command );
		}

		public void ReceiveSpecialInput(Keys key){
			int value = (int)key;
			this.processSpecialKey( value );
		}

		public void ReceiveTextInput( char inputChar ){
			this.ReceiveTextInput( inputChar.ToString() );
		}

		public void ReceiveTextInput( string text ){
			this.log( $"cursorPos BEFORE text input: {this.cursorPos}" );

			// Remove any cursor characters from the text (that character is reserved)
			if( text.Contains( this.cursor ) ){
				text = text.Replace( this.cursor, "" );
			}

			// Prevent typed newline chars from emulating Enter
			if( text.Contains( this.newLine ) ){
				text = text.Replace( this.newLine, "" );
			}

			// Truncate input string if it would take the text over the maxlength constraint
			if( ( this.maxlength > 0 ) && ( ( Text.Length + text.Length ) > this.maxlength ) ){
				text = text.Substring( 0, this.maxlength - Text.Length );
			}

			// Do nothing if we have no text left
			if( text.Length < 1 ){
				return;
			}

			// Insert the text only if it wont take us over the height of the text box or try
			// to run longer than a line's width without breaking
			if( !this.wouldBeTooHighWith( text ) && !this.lineWouldBeTooLongWith( text ) ){
				Text = this.addTextAtCursor( text );
				this.resetCursorDisplay();
			}
			this.log( $"ReceiveTextInput >> cursor pos AFTER text input: {this.cursorPos}" );
		}


		/*********************
		**	Private methods
		*********************/
		private string addTextAtCursor( string newText, bool moveCursor = true ){

			string oldText = Text;

			string updatedText = ( this.cursorPos < oldText.Length ) ? oldText.Insert( this.cursorPos, newText ) : oldText + newText;

			if( moveCursor ){
				this.cursorPos += newText.Length;
				Game1.playSound("cowboy_monsterhit");
			}

			return updatedText;
		}
		
		private void backspace( int chars = 1 ){	
			/* Acts like backspace key	*/
			Text = Text.Remove( this.cursorPos - chars, chars );
			this.cursorPos -= chars;
			this.resetCursorDisplay();
		}

		private void deleteText( int chars = 1 ){
			/* Acts like Delete key */
			Text = Text.Remove( this.cursorPos, chars );
			this.resetCursorDisplay();
		}

		private bool isTabNext( bool forwards = true ){
			/* Whether a tab is next to the cursor (forwards/backwards) */

			int tabLengthSV = this.getSVTabLength();

			// Ignore if we're too close to one end of the string for a tab to be possible
			if( ( forwards && ( this.cursorPos > ( Text.Length - tabLengthSV ) ) ) || ( this.cursorPos < tabLengthSV ) ){
				return false;
			}

			int startPos = this.cursorPos;
			if( !forwards ){
				startPos -= tabLengthSV;
			}

			return ( Text.Substring( startPos, tabLengthSV ) == this.getSVTab() );
		}

		private int getLastBreakPoint( string text = "", int pointer = -1, bool breakOnSpace = true ){
			if( "" == text ){
				text = this.Text;
			}
			if( -1 == cursorPos ){
				pointer = this.cursorPos;
			}
			if( text.Length > 0 ){
				for( int i = Math.Min( text.Length - 1, pointer ); i > 0; --i ){
					if( text[i].ToString() == newLine || ( breakOnSpace && ( (int)text[i] == 32 ) ) ){
						return i;
					}
				}
			}
			return 0;
		}
	
	
		private string getLine( int pointer = -1, int offset = 0 ){
			if( -1 == pointer ){
				pointer = this.cursorPos;
			}

			return Lines[this.getLineStartPosition( pointer, offset )];
		}
		private int getLineLength( int pointer = -1, int offset = 0 ){
			if( -1 == pointer ){
				pointer = this.cursorPos;
			}
			return this.getLine( pointer, offset ).Length;
		}

		private string getLineFromText( string text = "", int pointer = -1, bool breakOnSpace = true ){
			int lineLength = this.getLineLengthFromText( text, pointer, breakOnSpace );
			int start = this.getLastBreakPoint( text, pointer, breakOnSpace) + 1; // +1 to skip the linebreak character

			return text.Substring( start, lineLength );
		}
		private int getLineLengthFromText( string text = "", int pointer = -1, bool breakOnSpace = true ){
			return Math.Max(0, this.getNextBreakPoint( text, pointer, breakOnSpace ) - ( this.getLastBreakPoint( text, pointer, breakOnSpace ) + 1 ) ); // +1 to skip the linebreak character
		}

		private string getStartOfLine( int pointer = -1 ){
			if( -1 == pointer ){
				pointer = this.cursorPos;
			}

			return Text.Substring( this.getLastBreakPoint( Text, pointer, false ), pointer );
		}

		private string getEndOfLine( int pointer = -1 ){
			if( -1 == pointer ){
				pointer = this.cursorPos;
			}

			return Text.Substring( pointer, this.getNextBreakPoint( Text, pointer, false ) );
		}

		private Dictionary<int, string> getAllLines(){
			Dictionary<int, string> lines = new Dictionary<int, string>();

			this.log( $"getAllLines >> Text length: {Text.Length}" );

			int currentPointer = 0;
			foreach( string line in Text.Split( newLine ) ){
				this.log( $"getAllLines >> pointer: {currentPointer}" );
				this.log( $"getAllLines >> Next line (length: {line.Length}): {line}" );
				lines.Add( currentPointer, line );
				currentPointer += line.Length + 1; // We add +1 here for the newLine character which is omitted due to being used in the Text.split
			}

			/*
				DEBUG
			*/
				int index = 0;
				int lengthSum = 0;
				foreach( KeyValuePair<int,string> line in lines ){
					this.log( $"getAllLines >> line {index}: position {line.Key}" );
					lengthSum += line.Value.Length;
					index++;
				}
				this.log( $"getAllLines >> Text length (again): {Text.Length}" );
				this.log( $"getAllLines >> lines text length: {lengthSum}" );


			return lines;
		}

		// Get the position of the start of the line that 'pointer' is on
		// 'previous' represents the number of lines to count back
		private int getLineStartPosition( int pointer = -1, int offset = 0 ){
			if( !Lines.Any() ){
				return 0;
			}

			if( -1 == pointer ){
				pointer = this.cursorPos;
			}

			this.log( $"getLineStartPosition >> pointer: {pointer}, offset: {offset}" );

			int position = 0;
			int index = 0;
			
			// If we're at the start of a line already, calculating the position is simple
			int lineStartIndex = Lines.Keys.ToList().IndexOf( pointer );
			if( lineStartIndex > -1 ){
				index = lineStartIndex;
				position = Lines.Keys.ToList()[lineStartIndex];
			}else{

				// We're mid-line, go loopin'
				List<int> lineStarts = Lines.Keys.ToList();
				lineStarts.Reverse();
				foreach( int lineStart in lineStarts ){
					this.log( $"getLineStartPosition >> Next line start: {lineStart} (pointer is {pointer})" );

					if( lineStart < pointer ){
						position = lineStart;
						lineStarts.Reverse();
						index = lineStarts.IndexOf( lineStart );
						break;
					}
				}
			}

			// Count back lines if needed
			if( offset != 0 ){
				this.log( $"getLineStartPosition >> offsetting by {offset} (to line {index + offset})" );
				int offsetLine = Math.Clamp( index + offset, 0, Lines.Count - 1 );

				if( offsetLine != ( index + offset ) ){
					this.log( $"getLineStartPosition >> line clamped to {offsetLine}" );
				}

				position = Lines.Keys.ElementAt( offsetLine );
			}

			this.log( $"getLineStartPosition >> returning line start position: {position}", LogLevel.Info );

			return Math.Max( 0, position );
		}

		// Get the position of the start of the line above the one 'pointer' is on
		private int getPreviousLineStartPosition( int pointer = -1 ){
			return getLineStartPosition( pointer, -1 );
		}
		
		// Get the position of the start of the line below the one 'pointer' is on
		private int getNextLineStartPosition( int pointer = -1 ){
			return getLineStartPosition( pointer, 1 );
		}

		// Get the position of the end of the line that 'pointer' is on
		// 'next' represents the number of lines to count forwards
		private int getLineEndPosition( int pointer = -1, int offset = 0 ){
			if( !Lines.Any() ){
				return 0;
			}
	
			int lineStart = this.getLineStartPosition( pointer, offset );

			this.log( $"getLineEndPosition >> line start was {lineStart}" );
			this.log( $"getLineEndPosition >> getting line length for line at {lineStart}: {Lines[lineStart]}" );

			int lineEnd = lineStart + this.getLineLength( lineStart );

			this.log( $"getLineEndPosition >> returning line end position: {lineEnd}", LogLevel.Info );
			return lineEnd;
		}

		private int getNextBreakPoint( string text = "", int pointer = -1, bool breakOnSpace = true ){
			if( "" == text ){
				text = this.Text;
			}
			if( -1 == cursorPos ){
				cursorPos = pointer;
			}
			if( text.Length < 1 ){
				return 0;
			}
			for( int i = cursorPos; i < text.Length; i++ ){
				if( text[i].ToString() == newLine || ( breakOnSpace && ( (int)text[i] == 32 ) ) ){
					return i;
				}
			}
			return text.Length;
		}

		private string getSVTab(){
			/* Get SV repesentation of a tab cinput (SV uses multiple spaces each) */
			return string.Concat( Enumerable.Repeat( this.tabInput, 2 ) );
		}
		
		private int getSVTabLength(){
			return this.getSVTab().Length;
		}

		private void resetCursorDisplay(){
			this.cursorTicks = 0;
			this.cursorShowing = true;
		}


		/*
			Handling of misspelled functions which are required by the keyboard subscriber
		*/
		public void RecieveCommandInput( char command ){ this.ReceiveCommandInput( command ); }
		public void RecieveSpecialInput( Keys key ){ this.ReceiveSpecialInput( key ); }
		public void RecieveTextInput( char inputChar ){ this.ReceiveTextInput( inputChar ); }
		public void RecieveTextInput( string text ){ this.ReceiveTextInput( text ); }
	}
}