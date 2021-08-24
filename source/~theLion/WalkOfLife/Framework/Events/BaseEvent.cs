/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System.Linq;
using System.Text.RegularExpressions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	/// <summary>Interface for dynamic events.</summary>
	public abstract class BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public abstract void Hook();

		/// <summary>Unhook this event from the event listener.</summary>
		public abstract void Unhook();

		/// <summary>Get the prefix (first word) of the class name, which *should* correspond to the profession.</summary>
		public string Prefix()
		{
			return Regex.Split(GetType().Name, @"(?<!^)(?=[A-Z])").First();
		}
	}
}