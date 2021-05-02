/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods/-/tree/master/WalkOfLife
**
*************************************************/

namespace TheLion.AwesomeProfessions
{
	/// <summary>Interface for dynamic events.</summary>
	internal interface IEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		void Hook();

		/// <summary>Unhook this event from the event listener.</summary>
		void Unhook();
	}
}