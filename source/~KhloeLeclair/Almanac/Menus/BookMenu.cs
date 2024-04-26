/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;
using Leclair.Stardew.Common.UI.SimpleLayout;
using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI.Utilities;

using Leclair.Stardew.Almanac.Pages;

namespace Leclair.Stardew.Almanac.Menus;

internal class BookState {
	public string? LastSection;
	public int TabScroll;
	public Dictionary<string, object>? SectionStates;
}

internal class BookMenu : IClickableMenu {

	// State Storage
	static readonly PerScreen<Dictionary<string, BookState>> PreviousState = new();

	#region Fields

	internal Texture2D background;
	internal readonly ModEntry Mod;
	internal readonly string BookId;

	// Tab Components
	public List<ClickableComponent> TabComponents = new();
	public ClickableTextureComponent btnTabsUp;
	public ClickableTextureComponent btnTabsDown;

	// Sections
	internal readonly IAlmanacPage[] Sections;
	private int SectionIndex = -1;



	public List<ClickableComponent> PageComponents = new();

	#endregion

	#region Constructor

	#endregion

	#region Properties

	internal IAlmanacPage Section => (Sections == null || SectionIndex < 0 || SectionIndex >= Sections.Length) ? null : Sections[SectionIndex];



	internal Models.Style Style => Mod.Theme?.Standard;
	internal Models.Style MagicStyle => Mod.Theme?.Magic;




	#endregion


}
