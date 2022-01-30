/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoeStrout/Farmtronics
**
*************************************************/

/*	MiniscriptKeywords.cs

This file defines a little Keywords class, which contains all the 
MiniScript reserved words (break, for, etc.).  It might be useful 
if you are doing something like syntax coloring, or want to make 
sure some user-entered identifier isn’t going to conflict with a 
reserved word.

*/
using System;

namespace Miniscript {
	public static class Keywords {
		public static string[] all = {
			"break",
			"continue",
			"else",
			"end",
			"for",
			"function",
			"if",
			"in",
			"isa",
			"new",
			"null",
			"then",
			"repeat",
			"return",
			"while",
			"and",
			"or",
			"not",
			"true",
			"false"
		};

		public static bool IsKeyword(string text) {
			return Array.IndexOf(all, text) >= 0;
		}

	}
}

