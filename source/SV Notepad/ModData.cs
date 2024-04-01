/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/hbc-mods/stardew-valley
**
*************************************************/

using System.Collections.Generic;

namespace SVNotepad{
	class ModData{

		/********************
		**	Properties
		********************/
		public List<Note> Notes { get; set; }
		
		/******************
		**	Constructors
		******************/
		public ModData(){
			this.Notes = new List<Note>();
		}
		
		public ModData( List<Note> Notes ){
			this.Notes = Notes;
		}
	}
}