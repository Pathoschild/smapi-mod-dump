/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.ProjectFluent
{
	/// <summary>A value usable with the Project Fluent functions.</summary>
	/// <remarks>
	/// Although this interface is exposed in the API, and some methods take those as values,
	/// you should almost never create custom types implementing this interface (unless you know what you are doing).
	/// </remarks>
	public interface IFluentFunctionValue
	{
		/// <summary>Returns the value as the underlying Project Fluent library type. This is an implementation detail, and should not be used, unless you know what you are doing.</summary>
		object /* IFluentType */ AsFluentValue();

		/// <summary>Returns the value as a <see cref="string"/>.</summary>
		string AsString();

		/// <summary>Returns the value as an <see cref="int"/>, or <c>null</c> if it cannot be converted.</summary>
		int? AsIntOrNull();

		/// <summary>Returns the value as a <see cref="long"/>, or <c>null</c> if it cannot be converted.</summary>
		long? AsLongOrNull();

		/// <summary>Returns the value as a <see cref="float"/>, or <c>null</c> if it cannot be converted.</summary>
		float? AsFloatOrNull();

		/// <summary>Returns the value as a <see cref="double"/>, or <c>null</c> if it cannot be converted.</summary>
		double? AsDoubleOrNull();
	}
}