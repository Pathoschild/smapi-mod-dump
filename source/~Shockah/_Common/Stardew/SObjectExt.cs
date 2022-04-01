/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewValley.Objects;
using System.Diagnostics.CodeAnalysis;
using SObject = StardewValley.Object;

namespace Shockah.CommonModCode.Stardew
{
	public static class SObjectExt
	{
		public static bool TryGetAnyHeldObject(this SObject self, [NotNullWhen(true)] out SObject? heldObject)
		{
			var anyHeldObject = self.GetAnyHeldObject();
			if (anyHeldObject is null)
			{
				heldObject = null;
				return false;
			}
			else
			{
				heldObject = anyHeldObject;
				return true;
			}
		}

		public static SObject? GetAnyHeldObject(this SObject self)
		{
			if (self.heldObject.Value is not null)
				return self.heldObject.Value;
			if (self is CrabPot crabPot && crabPot.bait.Value is not null)
				return crabPot.bait.Value;
			return null;
		}
	}
}
