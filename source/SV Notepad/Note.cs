/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/hbc-mods/stardew-valley
**
*************************************************/

using System;

namespace SVNotepad{

	class Note{

		/********************
		**	Properties
		********************/
		public DateTime created { get; set; }
		public DateTime updated { get; set; }
		public string defaultText = "Hello, world!";
		public string defaultTitle = "Untitled";
		public string text { get; set; }
		public string title { get; set; }


		/******************
		**	Constructors
		******************/
		public Note(){
			this.title = this.defaultTitle;
			this.text = this.defaultText;
			this.created = DateTime.MinValue;
			this.updated = DateTime.MinValue;
		}

		public Note( string title, string text ){
			this.title = title;
			this.text = text;
			this.created = DateTime.MinValue;
			this.updated = DateTime.MinValue;
		}

		public Note( string title, string text, DateTime created, DateTime updated ){
			this.title = title;
			this.text = text;
			this.created = created;
			this.updated = updated;
		}


		/********************
		**	Public methods
		********************/
		public void preSave(){
			if( this.created == DateTime.MinValue ){
				this.created = DateTime.Now;
			}
			this.updated = DateTime.Now;
		}
	}
}