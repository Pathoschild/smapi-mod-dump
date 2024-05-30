/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#if COMMON_BCINVENTORY

using Leclair.Stardew.Common.Enums;

namespace Leclair.Stardew.Common.Inventory;

public class TransferBehavior {

	public TransferMode Mode;
	public int Quantity;

	public TransferBehavior(TransferMode mode, int quantity) {
		Mode = mode;
		Quantity = quantity;
	}
}

#endif
