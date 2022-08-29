/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Shockah.MailPersistenceFramework
{
	public enum MailBackground
	{
		Classic = 0,
		Notepad = 1,
		Pyramids = 2
	}

	public enum MailTextColor
	{
		DarkRed = -1,
		Black = 0,
		SkyBlue = 1,
		Red = 2,
		BlueViolet = 3,
		White = 4,
		OrangeRed = 5,
		LimeGreen = 6,
		Cyan = 7,
		DarkestGray = 8
	}

	public enum MailApiAttribute
	{
		/// <summary>
		/// Custom tags for a given mail, which can be used to determine the mail type and override some of its attributes at runtime.
		/// </summary>
		/// <remarks>
		/// The value is of type <c>IReadOnlyDictionary&lt;string, string&gt;</c>.<br/>
		/// On input, <c>object</c> type is allowed and will be converted into <c>IReadOnlyDictionary&lt;string, string&gt;</c>.<br/>
		/// <br/>
		/// This attribute cannot be overriden.
		/// </remarks>
		Tags,

		/// <summary>
		/// The mail's title.<br/>
		/// If a mail has a title, it will be visible in the collections menu and will not be removed from the save data after being read.
		/// </summary>
		/// <remarks>
		/// The value is of type <c>string?</c>.<br/>
		/// <br/>
		/// This attribute can be overriden with a <c>Action&lt;string, string, string?, Action&lt;string?&gt;&gt;</c> delegate.<br/>
		/// Parameter #1: <c>string modUniqueID</c> - The mod's unique ID.<br/>
		/// Parameter #2: <c>string mailID</c> - The mail's ID.<br/>
		/// Parameter #3: <c>string? title</c> - The mail's current title.<br/>
		/// Parameter #4: <c>Action&lt;string?&gt; @override</c> - A delegate to call to override the value.<br/>
		/// See <see cref="IMailPersistenceFrameworkApi.RegisterMailAttributeOverrides"/> for more details.
		/// </remarks>
		Title,

		/// <summary>Text of the given mail.</summary>
		/// <remarks>
		/// The value is of type <c>string</c>.<br/>
		/// <br/>
		/// This attribute can be overriden with a <c>Action&lt;string, string, string, Action&lt;string&gt;&gt;</c> delegate.<br/>
		/// Parameter #1: <c>string modUniqueID</c> - The mod's unique ID.<br/>
		/// Parameter #2: <c>string mailID</c> - The mail's ID.<br/>
		/// Parameter #3: <c>string text</c> - The mail's current text.<br/>
		/// Parameter #4: <c>Action&lt;string&gt; @override</c> - A delegate to call to override the value.<br/>
		/// See <see cref="IMailPersistenceFrameworkApi.RegisterMailAttributeOverrides"/> for more details.
		/// </remarks>
		Text,

		/// <summary>The items attached to the mail.</summary>
		/// <remarks>
		/// The value is of type <c>IReadOnlyList&lt;Item&gt;</c>.<br/>
		/// On input, <c>IEnumerable&lt;Item&gt;</c> type is allowed and will be converted.<br/>
		/// On input, <c>Item</c> type is allowed and will be wrapped.<br/>
		/// <br/>
		/// This attribute can be overriden with a <c>Action&lt;string, string, IReadOnlyList&lt;Item&gt;, Action&lt;IEnumerable&lt;Item&gt;&gt;&gt;</c> delegate.<br/>
		/// Parameter #1: <c>string modUniqueID</c> - The mod's unique ID.<br/>
		/// Parameter #2: <c>string mailID</c> - The mail's ID.<br/>
		/// Parameter #3: <c>IReadOnlyList&lt;Item&gt; items</c> - The mail's current attached items.<br/>
		/// Parameter #4: <c>Action&lt;IEnumerable&lt;Item&gt;&gt; @override</c> - A delegate to call to override the value.<br/>
		/// See <see cref="IMailPersistenceFrameworkApi.RegisterMailAttributeOverrides"/> for more details.
		/// </remarks>
		Items,

		/// <summary>A recipe name attached to the mail.</summary>
		/// <remarks>
		/// The value is of type <c>string</c>.<br/>
		/// <br/>
		/// This attribute can be overriden with a <c>Action&lt;string, string, string?, Action&lt;string?&gt;&gt;</c> delegate.<br/>
		/// Parameter #1: <c>string modUniqueID</c> - The mod's unique ID.<br/>
		/// Parameter #2: <c>string mailID</c> - The mail's ID.<br/>
		/// Parameter #3: <c>string? recipe</c> - The mail's current attached recipe.<br/>
		/// Parameter #4: <c>Action&lt;string?&gt; @override</c> - A delegate to call to override the value.<br/>
		/// See <see cref="IMailPersistenceFrameworkApi.RegisterMailAttributeOverrides"/> for more details.
		/// </remarks>
		Recipe,

		/// <summary>The background ID to use for this mail.</summary>
		/// <remarks>
		/// The value is of type <see cref="int"/>.<br/>
		/// On input, <c>MailBackground</c> type is allowed and will be casted.<br/>
		/// See <see cref="MailBackground"/> for allowed values.<br/>
		/// <br/>
		/// This attribute can be overriden with a <c>Action&lt;string, string, int, Action&lt;int&gt;&gt;</c> delegate.<br/>
		/// Parameter #1: <c>string modUniqueID</c> - The mod's unique ID.<br/>
		/// Parameter #2: <c>string mailID</c> - The mail's ID.<br/>
		/// Parameter #3: <c>int backgroundID</c> - The mail's current background.<br/>
		/// Parameter #4: <c>Action&lt;int&gt; @override</c> - A delegate to call to override the value.<br/>
		/// See <see cref="IMailPersistenceFrameworkApi.RegisterMailAttributeOverrides"/> for more details.
		/// </remarks>
		Background,

		/// <summary>The text color ID to use for this mail.</summary>
		/// <remarks>
		/// The value is of type <see cref="int"/>.<br/>
		/// On input, <c>MailTextColor</c> type is allowed and will be casted.<br/>
		/// See <see cref="MailTextColor"/> for allowed values.<br/>
		/// <br/>
		/// This attribute can be overriden with a <c>Action&lt;string, string, int?, Action&lt;int?&gt;&gt;</c> delegate.<br/>
		/// Parameter #1: <c>string modUniqueID</c> - The mod's unique ID.<br/>
		/// Parameter #2: <c>string mailID</c> - The mail's ID.<br/>
		/// Parameter #3: <c>int? textColor</c> - The mail's current text color.<br/>
		/// Parameter #4: <c>Action&lt;int?&gt; @override</c> - A delegate to call to override the value.<br/>
		/// See <see cref="IMailPersistenceFrameworkApi.RegisterMailAttributeOverrides"/> for more details.
		/// </remarks>
		TextColor,
	}

	public interface IMailPersistenceFrameworkApi
	{
		/// <summary>
		/// Registers mod overrides for mails.
		/// </summary>
		/// <param name="mod">The mod's manifest.</param>
		/// <param name="overrides">Overrides to use for each attribute</param>
		void RegisterMailAttributeOverrides(
			IManifest mod,
			IReadOnlyDictionary<int /* MailApiAttribute */, Delegate> overrides
		);

		/// <summary>
		/// Sends a mail to the specified player.
		/// </summary>
		/// <param name="mod">The mod's manifest.</param>
		/// <param name="addressee">The player to the send the mail to.</param>
		/// <param name="attributes">The mail's <see cref="MailApiAttribute">attributes</see>.</param>
		/// <returns>The created mail's ID, which can be used with other methods of this API.</returns>
		string SendMail(IManifest mod, Farmer addressee, IReadOnlyDictionary<int /* MailApiAttribute */, object?> attributes);

		/// <summary>
		/// Sends a mail to the local player.
		/// </summary>
		/// <param name="mod">The mod's manifest.</param>
		/// <param name="attributes">The mail's <see cref="MailApiAttribute">attributes</see>.</param>
		/// <returns>The created mail's ID, which can be used with other methods of this API.</returns>
		string SendMailToLocalPlayer(IManifest mod, IReadOnlyDictionary<int /* MailApiAttribute */, object?> attributes);

		/// <summary>
		/// Gets the IDs of all of the existing mails sent for this mod.
		/// </summary>
		/// <param name="modUniqueID">The mod's unique ID.</param>
		/// <returns>An <see cref="IEnumerable{string}"/> of existing mails.</returns>
		IEnumerable<string> GetMailIDs(string modUniqueID);

		/// <summary>
		/// Tests if a given mail exists.
		/// </summary>
		/// <param name="modUniqueID">The mod's unique ID.</param>
		/// <param name="mailID">The mail's ID.</param>
		/// <returns>Whether a given mail exists.</returns>
		bool HasMail(string modUniqueID, string mailID);

		/// <summary>
		/// Gets the <see cref="Farmer"/> address of a mail.
		/// </summary>
		/// <param name="modUniqueID">The mod's unique ID.</param>
		/// <param name="mailID">The mail's ID.</param>
		/// <returns>The <see cref="Farmer"/> addressee of the mail.</returns>
		/// <exception cref="ArgumentException">When a given mail does not exist.</exception>
		Farmer GetMailAddressee(string modUniqueID, string mailID);

		/// <summary>
		/// Gets the mail's tags.
		/// </summary>
		/// <param name="modUniqueID">The mod's unique ID.</param>
		/// <param name="mailID">The mail's ID.</param>
		/// <returns>The mail's tags.</returns>
		/// <exception cref="ArgumentException">When a given mail does not exist.</exception>
		IReadOnlyDictionary<string, string> GetMailTags(string modUniqueID, string mailID);

		/// <summary>
		/// Gets the attributes of a mail.
		/// </summary>
		/// <param name="modUniqueID">The mod's uniqueID.</param>
		/// <param name="mailID">The mail's ID.</param>
		/// <returns>The attributes of the mail.</returns>
		/// <exception cref="ArgumentException">When a given mail does not exist.</exception>
		IReadOnlyDictionary<int /* MailApiAttribute */, object?> GetMailAttributes(string modUniqueID, string mailID);
	}
}
