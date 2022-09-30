/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoeStrout/Farmtronics
**
*************************************************/

namespace Farmtronics.M1 {
	enum Key {
		// (SButton.LeftControl | SButton.RightControl) & SButton.A
		ControlA = 1,
		// (SButton.LeftControl | SButton.RightControl) & SButton.C
		ControlC = 3,
		// (SButton.LeftControl | SButton.RightControl) & SButton.E
		ControlE = 5,
		Backspace = 8,
		Tab = 9,
		Enter = 13,
		RightArrow = 18,
		UpArrow = 19,
		DownArrow = 20,
		LeftArrow = 17,
		FwdDelete = 127
	}
}