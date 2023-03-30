/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/inkle/ink
**
*************************************************/

using System.Collections.Generic;

namespace Ink.Parsed
{
	public class Stitch : FlowBase
	{
        public override FlowLevel flowLevel { get { return FlowLevel.Stitch; } }

        public Stitch (Identifier name, List<Parsed.Object> topLevelObjects, List<Argument> arguments, bool isFunction) : base(name, topLevelObjects, arguments, isFunction)
		{
		}
	}
}

