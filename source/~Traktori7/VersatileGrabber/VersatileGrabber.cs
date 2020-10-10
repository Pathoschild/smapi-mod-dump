/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace VersatileGrabber
{
	class VersatileGrabber : SObject
	{
		public VersatileGrabber(SObject sobject) : base (sobject.TileLocation, sobject.ParentSheetIndex)
		{
			
		}

		public SObject ToObject()
		{
			SObject newObject = new SObject(this.TileLocation, this.ParentSheetIndex);
			return newObject;
		}
	}
}
