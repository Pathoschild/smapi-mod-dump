/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Shockah.Kokoro;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.MachineStatus;

public sealed class ModConfig : IVersioned.Modifiable
{
	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ISemanticVersion? Version { get; set; }

	public UIAnchorSide ScreenAnchorSide { get; set; } = UIAnchorSide.BottomLeft;
	public float AnchorInset { get; set; } = 16f;
	public float AnchorOffsetX { get; set; } = 0f;
	public float AnchorOffsetY { get; set; } = 0f;
	public UIAnchorSide PanelAnchorSide { get; set; } = UIAnchorSide.BottomLeft;
	[JsonIgnore] public UIAnchor Anchor => new(ScreenAnchorSide, AnchorInset, new(AnchorOffsetX, AnchorOffsetY), PanelAnchorSide);

	public FlowDirection FlowDirection { get; set; } = FlowDirection.LeftToRightAndBottomToTop;
	public float Scale { get; set; } = 2f;
	public float XSpacing { get; set; } = 4f;
	public float YSpacing { get; set; } = 4f;
	[JsonIgnore] public Vector2 Spacing => new(XSpacing, YSpacing);
	public int MaxColumns { get; set; } = 6;

	public bool ShowItemBubble { get; set; } = true;
	public float BubbleItemCycleTime { get; set; } = 2f;
	public MachineRenderingOptions.BubbleSway BubbleSway { get; set; } = MachineRenderingOptions.BubbleSway.Wave;

	public SplitScreenScreens SplitScreenScreens { get; set; } = SplitScreenScreens.First;
	public KeybindList VisibilityKeybind { get; set; } = new KeybindList();
	public float FocusedAlpha { get; set; } = 1f;
	public float NormalAlpha { get; set; } = 0.3f;

	public MachineRenderingOptions.Grouping Grouping { get; set; } = MachineRenderingOptions.Grouping.ByMachine;
	public List<MachineRenderingOptions.Sorting> Sorting { get; set; } = [
		MachineRenderingOptions.Sorting.ReadyFirst,
		MachineRenderingOptions.Sorting.WaitingFirst,
		MachineRenderingOptions.Sorting.ByMachineAZ,
		MachineRenderingOptions.Sorting.ByItemAZ
	];

	public bool ShowReady { get; set; } = true;
	public List<string> ShowReadyExceptions { get; set; } = [];

	public bool ShowWaiting { get; set; } = false;
	public List<string> ShowWaitingExceptions { get; set; } = ["*|Cask", "*|Keg", "*|Preserves Jar", "*|Crab Pot", "*|Crystalarium"];

	public bool ShowBusy { get; set; } = false;
	public List<string> ShowBusyExceptions { get; set; } = [];

	[JsonIgnore]
	public IReadOnlyList<IWildcardPattern> ShowReadyExceptionPatterns
	{
		get => ShowReadyExceptions.Select(WildcardPatterns.Parse).ToList();
		set => ShowReadyExceptions = value.Select(p => p.Pattern).ToList();
	}

	[JsonIgnore]
	public IReadOnlyList<IWildcardPattern> ShowWaitingExceptionPatterns
	{
		get => ShowWaitingExceptions.Select(WildcardPatterns.Parse).ToList();
		set => ShowWaitingExceptions = value.Select(p => p.Pattern).ToList();
	}

	[JsonIgnore]
	public IReadOnlyList<IWildcardPattern> ShowBusyExceptionPatterns
	{
		get => ShowBusyExceptions.Select(WildcardPatterns.Parse).ToList();
		set => ShowBusyExceptions = value.Select(p => p.Pattern).ToList();
	}
}