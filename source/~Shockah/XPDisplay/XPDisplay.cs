/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Shockah.XPDisplay
{
	public class XPDisplay : BaseMod<ModConfig>, IXPDisplayApi
	{
		private static readonly Rectangle SmallUnobtainedLevelCursorsRectangle = new(129, 338, 7, 9);
		private static readonly Rectangle SmallObtainedLevelCursorsRectangle = new(137, 338, 7, 9);
		private static readonly Rectangle BigObtainedLevelCursorsRectangle = new(159, 338, 13, 9);
		private static readonly Rectangle BigUnobtainedLevelCursorsRectangle = new(145, 338, 13, 9);
		private const float IconToBarSpacing = 3;
		private const float LevelNumberToBarSpacing = 2;
		private const float BarSegmentSpacing = 2;

		private const int FPS = 60;
		private static readonly int[] OrderedSkillIndexes = new[] { 0, 3, 2, 1, 4, 5 };
		private static readonly string SpaceCoreNewSkillsPageQualifiedName = "SpaceCore.Interface.NewSkillsPage, SpaceCore";
		private static readonly string SpaceCoreSkillsQualifiedName = "SpaceCore.Skills, SpaceCore";

		internal static XPDisplay Instance = null!;
		private bool IsWalkOfLifeInstalled = false;
		private bool IsMargoInstalled = false;

		private static readonly Dictionary<(int uiSkillIndex, string? spaceCoreSkillName), (Vector2?, Vector2?)> SkillBarCorners = new();
		private static readonly List<(Vector2, Vector2)> SkillBarHoverExclusions = new();
		private static readonly List<Action> SkillsPageDrawQueuedDelegates = new();

		private static readonly Lazy<Func<Toolbar, List<ClickableComponent>>> ToolbarButtonsGetter = new(() => AccessTools.DeclaredField(typeof(Toolbar), "buttons").EmitInstanceGetter<Toolbar, List<ClickableComponent>>());
		private static readonly PerScreen<Dictionary<ISkill, (int Level, int XP)>> SkillsToRecheck = new(() => new());

		private readonly List<Func<Item, (int? SkillIndex, string? SpaceCoreSkillName)?>> ToolSkillMatchers = new()
		{
			o => o is Hoe or WateringCan or MilkPail or Shears || (o is MeleeWeapon && o.Name.Contains("Scythe")) ? (Farmer.farmingSkill, null) : null,
			o => o is Pickaxe ? (Farmer.miningSkill, null) : null,
			o => o is Axe ? (Farmer.foragingSkill, null) : null,
			o => o is FishingRod ? (Farmer.fishingSkill, null) : null,
			o => o is Sword or Slingshot || (o is MeleeWeapon && !o.Name.Contains("Scythe")) ? (Farmer.combatSkill, null) : null,
		};

		private readonly PerScreen<ISkill?> ToolbarCurrentSkill = new(() => null);
		private readonly PerScreen<float> ToolbarActiveDuration = new(() => 0f);
		private readonly PerScreen<float> ToolbarAlpha = new(() => 0f);
		private readonly PerScreen<Item?> LastCurrentItem = new(() => null);
		private readonly PerScreen<string?> ToolbarTooltip = new(() => null);

		public override void OnEntry(IModHelper helper)
		{
			Instance = this;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.Display.RenderingHud += OnRenderingHud;
			helper.Events.Display.RenderedHud += OnRenderedHud;
		}

		public override void MigrateConfig(ISemanticVersion? configVersion, ISemanticVersion modVersion)
		{
			// do nothing, for now
		}

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			SetupConfig();

			IsWalkOfLifeInstalled = Helper.ModRegistry.IsLoaded("DaLion.ImmersiveProfessions");
			IsMargoInstalled = Helper.ModRegistry.IsLoaded("DaLion.Overhaul");
			var harmony = new Harmony(ModManifest.UniqueID);

			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(SkillsPage), nameof(SkillsPage.draw), new Type[] { typeof(SpriteBatch) }),
				postfix: new HarmonyMethod(typeof(XPDisplay), nameof(SkillsPage_draw_Postfix)),
				transpiler: new HarmonyMethod(typeof(XPDisplay), nameof(SkillsPage_draw_Transpiler))
			);
			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
				prefix: new HarmonyMethod(typeof(XPDisplay), nameof(Farmer_gainExperience_Prefix))
			);
			harmony.TryPatch(
				monitor: Monitor,
				original: () => AccessTools.Method(typeof(Farmer), "performFireTool"),
				prefix: new HarmonyMethod(typeof(XPDisplay), nameof(Farmer_performFireTool_Prefix))
			);

			if (Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
			{
				harmony.TryPatch(
					monitor: Monitor,
					original: () => AccessTools.Method(AccessTools.TypeByName(SpaceCoreNewSkillsPageQualifiedName), "draw", new Type[] { typeof(SpriteBatch) }),
					postfix: new HarmonyMethod(typeof(XPDisplay), nameof(SpaceCore_NewSkillsPage_draw_Postfix)),
					transpiler: new HarmonyMethod(typeof(XPDisplay), nameof(SpaceCore_NewSkillsPage_draw_Transpiler))
				);
				harmony.TryPatch(
					monitor: Monitor,
					original: () => AccessTools.Method(AccessTools.TypeByName(SpaceCoreSkillsQualifiedName), "AddExperience"),
					prefix: new HarmonyMethod(typeof(XPDisplay), nameof(SpaceCore_Skills_AddExperience_Prefix))
				);
			}
		}

		private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
		{
			foreach (var (skill, lastKnown) in SkillsToRecheck.Value)
			{
				float xpChangedDuration = Instance.Config.ToolbarSkillBar.XPChangedDurationInSeconds;
				if (lastKnown.XP == skill.GetXP(Game1.player))
					xpChangedDuration = 0f;

				float levelChangedDuration = Instance.Config.ToolbarSkillBar.LevelChangedDurationInSeconds;
				if (lastKnown.Level >= skill.GetBaseLevel(Game1.player))
					levelChangedDuration = 0f;
				else if (!string.IsNullOrEmpty(Instance.Config.LevelUpSoundName))
					Game1.playSound(Instance.Config.LevelUpSoundName);

				if (!Instance.Config.ToolbarSkillBar.IsEnabled)
					continue;

				var maxDuration = Math.Max(xpChangedDuration, levelChangedDuration);
				if (maxDuration > 0f)
				{
					Instance.ToolbarCurrentSkill.Value = skill;
					Instance.ToolbarActiveDuration.Value = maxDuration;
				}
			}
			SkillsToRecheck.Value.Clear();

			if (Config.ToolbarSkillBar.IsEnabled)
			{
				if (ToolbarActiveDuration.Value > 0f)
				{
					bool shouldCountDown = !Config.ToolbarSkillBar.AlwaysShowCurrentTool;
					if (!shouldCountDown)
					{
						var skill = GetSkillForItem(Game1.player.CurrentItem);
						if (skill != ToolbarCurrentSkill.Value)
							shouldCountDown = true;
					}

					if (shouldCountDown)
						ToolbarActiveDuration.Value = Math.Max(ToolbarActiveDuration.Value - 1f / FPS, 0f);
					if (ToolbarActiveDuration.Value <= 0f && Config.ToolbarSkillBar.AlwaysShowCurrentTool)
					{
						var skill = GetSkillForItem(Game1.player.CurrentItem);
						if (skill is not null)
						{
							ToolbarCurrentSkill.Value = skill;
							ToolbarActiveDuration.Value = 0.1f;
						}
					}
				}

				var targetAlpha = ToolbarActiveDuration.Value > 0f ? 1f : 0f;
				ToolbarAlpha.Value += (targetAlpha - ToolbarAlpha.Value) * 0.15f;
				if (ToolbarAlpha.Value <= 0.01f)
					ToolbarAlpha.Value = 0f;
				else if (ToolbarAlpha.Value >= 0.99f)
					ToolbarAlpha.Value = 1f;
			}

			if (!ReferenceEquals(Game1.player.CurrentItem, LastCurrentItem.Value))
			{
				if (Config.ToolbarSkillBar.IsEnabled && (Config.ToolbarSkillBar.AlwaysShowCurrentTool || Config.ToolbarSkillBar.ToolSwitchDurationInSeconds > 0f))
				{
					var skill = GetSkillForItem(Game1.player.CurrentItem);
					if (skill is not null)
					{
						ToolbarCurrentSkill.Value = skill;
						ToolbarActiveDuration.Value = Config.ToolbarSkillBar.AlwaysShowCurrentTool ? 0.1f : Config.ToolbarSkillBar.ToolSwitchDurationInSeconds;
					}
					else
					{
						ToolbarActiveDuration.Value = 0f;
					}
				}
				LastCurrentItem.Value = Game1.player.CurrentItem;
			}
		}

		private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
		{
			ToolbarTooltip.Value = null;
			if (!Config.ToolbarSkillBar.IsEnabled)
				return;
			if (ToolbarAlpha.Value <= 0f)
				return;
			if (Game1.activeClickableMenu is not null)
				return;

			var skill = ToolbarCurrentSkill.Value;
			if (skill is null)
				return;

			var toolbar = GetToolbar();
			if (toolbar is null)
				return;

			var buttons = ToolbarButtonsGetter.Value(toolbar);
			int toolbarMinX = buttons.Select(b => b.bounds.X).Min();
			int toolbarMaxX = buttons.Select(b => b.bounds.X).Max();
			int toolbarMinY = buttons.Select(b => b.bounds.Y).Min();
			Rectangle toolbarBounds = new(toolbarMinX, toolbarMinY, toolbarMaxX - toolbarMinX + 64, 64);

			var viewportBounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
			bool drawBarAboveToolbar = toolbarBounds.Center.Y >= viewportBounds.Center.Y;
			Vector2 barPosition = new(
				toolbarBounds.Center.X,
				drawBarAboveToolbar ? toolbarBounds.Top - Config.ToolbarSkillBar.SpacingFromToolbar : toolbarBounds.Bottom + Config.ToolbarSkillBar.SpacingFromToolbar
			);
			DrawSkillBar(skill, e.SpriteBatch, drawBarAboveToolbar ? UIAnchorSide.Bottom : UIAnchorSide.Top, barPosition, Config.ToolbarSkillBar.Scale, ToolbarAlpha.Value);
		}

		private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
		{
			if (ToolbarTooltip.Value is not null)
			{
				IClickableMenu.drawToolTip(e.SpriteBatch, ToolbarTooltip.Value, null, null);
				ToolbarTooltip.Value = null;
			}
		}

		public void RegisterToolSkillMatcher(Func<Item, (int? SkillIndex, string? SpaceCoreSkillName)?> matcher)
		{
			ToolSkillMatchers.Insert(0, matcher);
		}

		private void SetupConfig()
		{
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api is null)
				return;
			var helper = new GMCMI18nHelper(api, ModManifest, Helper.Translation);

			api.Register(
				ModManifest,
				reset: () => Config = new ModConfig(),
				save: () =>
				{
					WriteConfig();
					LogConfig();
				}
			);

			helper.AddTextOption("config.levelUpSoundName", () => Config.LevelUpSoundName ?? "", value => Config.LevelUpSoundName = string.IsNullOrEmpty(value) ? null : value);

			helper.AddSectionTitle("config.partialBar.section");
			helper.AddEnumOption("config.partialBar.smallBars", valuePrefix: "config.orientation", property: () => Config.SmallBarOrientation);
			helper.AddEnumOption("config.partialBar.bigBars", valuePrefix: "config.orientation", property: () => Config.BigBarOrientation);
			helper.AddNumberOption("config.partialBar.alpha", () => Config.Alpha, min: 0f, max: 1f, interval: 0.05f);

			helper.AddSectionTitle("config.toolbar.section");
			helper.AddBoolOption("config.toolbar.enabled", () => Config.ToolbarSkillBar.IsEnabled);
			helper.AddNumberOption("config.toolbar.scale", () => Config.ToolbarSkillBar.Scale, min: 0.2f, max: 12f, interval: 0.05f);
			helper.AddNumberOption("config.toolbar.spacingFromToolbar", () => Config.ToolbarSkillBar.SpacingFromToolbar, min: -32f, max: 128, interval: 1f);
			helper.AddBoolOption("config.toolbar.showIcon", () => Config.ToolbarSkillBar.ShowIcon);
			helper.AddBoolOption("config.toolbar.showLevelNumber", () => Config.ToolbarSkillBar.ShowLevelNumber);
			helper.AddBoolOption("config.toolbar.alwaysShowCurrentTool", () => Config.ToolbarSkillBar.AlwaysShowCurrentTool);
			helper.AddNumberOption("config.toolbar.toolSwitchDurationInSeconds", () => Config.ToolbarSkillBar.ToolSwitchDurationInSeconds, min: 0f, max: 15f, interval: 0.5f);
			helper.AddNumberOption("config.toolbar.toolUseDurationInSeconds", () => Config.ToolbarSkillBar.ToolUseDurationInSeconds, min: 0f, max: 15f, interval: 0.5f);
			helper.AddNumberOption("config.toolbar.xpChangedDurationInSeconds", () => Config.ToolbarSkillBar.XPChangedDurationInSeconds, min: 0f, max: 15f, interval: 0.5f);
			helper.AddNumberOption("config.toolbar.levelChangedDurationInSeconds", () => Config.ToolbarSkillBar.LevelChangedDurationInSeconds, min: 0f, max: 15f, interval: 0.5f);
		}

		private ISkill? GetSkillForItem(Item? item)
		{
			if (item is null)
				return null;

			foreach (var matcher in ToolSkillMatchers)
			{
				var skill = matcher(item);
				if (skill is null)
					continue;
				return SkillExt.GetSkill(skill.Value.SkillIndex, skill.Value.SpaceCoreSkillName);
			}

			return null;
		}

		private static Toolbar? GetToolbar()
			=> Game1.onScreenMenus.OfType<Toolbar>().FirstOrDefault();

		private void DrawSkillBar(ISkill skill, SpriteBatch b, UIAnchorSide anchorSide, Vector2 position, float scale, float alpha)
		{
			int currentLevel = skill.GetBaseLevel(Game1.player);
			int nextLevelXP = skill.GetLevelXP(currentLevel + 1);
			int currentLevelXP = skill.GetLevelXP(currentLevel);
			int currentXP = skill.GetXP(Game1.player);
			float nextLevelProgress = Math.Clamp(1f * (currentXP - currentLevelXP) / (nextLevelXP - currentLevelXP), 0f, 1f);

			var icon = Config.ToolbarSkillBar.ShowIcon ? skill.Icon : null;

			var barSize = new Vector2(
				SmallObtainedLevelCursorsRectangle.Size.X * 8 + BigObtainedLevelCursorsRectangle.Size.X * 2 + BarSegmentSpacing * 9
					+ (icon is null ? 0f : icon.Rectangle.Width + IconToBarSpacing)
					+ (Config.ToolbarSkillBar.ShowLevelNumber ? NumberSprite.getWidth(99) - 2 + LevelNumberToBarSpacing : 0f),
				BigObtainedLevelCursorsRectangle.Size.Y
			) * scale;
			var wholeToolbarTopLeft = position - anchorSide.GetAnchorOffset(barSize);

			float xOffset = 0;
			if (icon is not null)
			{
				Vector2 iconSize = new(icon.Rectangle.Width, icon.Rectangle.Height);
				var iconPosition = wholeToolbarTopLeft + UIAnchorSide.Center.GetAnchorOffset(iconSize) * scale;
				b.Draw(icon.Texture, iconPosition + new Vector2(-1, 1) * scale, icon.Rectangle, Color.Black * alpha * 0.3f, 0f, iconSize / 2f, scale, SpriteEffects.None, 0f);
				b.Draw(icon.Texture, iconPosition, icon.Rectangle, Color.White * alpha, 0f, iconSize / 2f, scale, SpriteEffects.None, 0f);
				xOffset += icon.Rectangle.Width + IconToBarSpacing;
			}

			for (int levelIndex = 0; levelIndex < 10; levelIndex++)
			{
				if (levelIndex != 0)
					xOffset += BarSegmentSpacing;

				bool isBigLevel = (levelIndex + 1) % 5 == 0;
				Texture2D obtainedBarTexture;
				Texture2D unobtainedBarTexture;
				Rectangle obtainedBarTextureRectangle;
				Rectangle unobtainedBarTextureRectangle;

				void UpdateExtendedLevelTextures(int level)
				{
					obtainedBarTexture = Game1.mouseCursors;
					unobtainedBarTexture = Game1.mouseCursors;
					obtainedBarTextureRectangle = isBigLevel ? BigObtainedLevelCursorsRectangle : SmallObtainedLevelCursorsRectangle;
					unobtainedBarTextureRectangle = isBigLevel ? BigUnobtainedLevelCursorsRectangle : SmallUnobtainedLevelCursorsRectangle;

					if (level >= 10 && level > levelIndex + 10)
					{
						if (Instance.IsWalkOfLifeInstalled && WalkOfLifeBridge.IsPrestigeEnabled())
							(obtainedBarTexture, obtainedBarTextureRectangle) = isBigLevel ? WalkOfLifeBridge.GetExtendedBigBar()!.Value : WalkOfLifeBridge.GetExtendedSmallBar()!.Value;
						else if (Instance.IsMargoInstalled && MargoBridge.IsPrestigeEnabled())
							(obtainedBarTexture, obtainedBarTextureRectangle) = isBigLevel ? MargoBridge.GetExtendedBigBar()!.Value : MargoBridge.GetExtendedSmallBar()!.Value;
					}
				}

				UpdateExtendedLevelTextures(currentLevel);

				var backgroundBarTexture = currentLevel > levelIndex ? obtainedBarTexture : unobtainedBarTexture;
				var backgroundBarTextureRectangle = currentLevel > levelIndex ? obtainedBarTextureRectangle : unobtainedBarTextureRectangle;

				var topLeft = wholeToolbarTopLeft + new Vector2(xOffset * scale, 0);
				b.Draw(backgroundBarTexture, topLeft + new Vector2(-1, 1) * scale, backgroundBarTextureRectangle, Color.Black * alpha * 0.3f, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
				b.Draw(backgroundBarTexture, topLeft, backgroundBarTextureRectangle, Color.White * alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
				xOffset += backgroundBarTextureRectangle.Width;

				if (currentLevel % 10 != levelIndex)
					continue;

				UpdateExtendedLevelTextures(currentLevel + 1);

				Vector2 partialBarPosition;
				Rectangle partialBarTextureRectangle;
				switch (isBigLevel ? Instance.Config.BigBarOrientation : Instance.Config.SmallBarOrientation)
				{
					case Orientation.Horizontal:
						int rectangleWidthPixels = (int)(obtainedBarTextureRectangle.Width * nextLevelProgress);
						partialBarPosition = topLeft;
						partialBarTextureRectangle = new(
							obtainedBarTextureRectangle.Left,
							obtainedBarTextureRectangle.Top,
							rectangleWidthPixels,
							obtainedBarTextureRectangle.Height
						);
						break;
					case Orientation.Vertical:
						int rectangleHeightPixels = (int)(obtainedBarTextureRectangle.Height * nextLevelProgress);
						partialBarPosition = topLeft + new Vector2(0f, (obtainedBarTextureRectangle.Height - rectangleHeightPixels) * scale);
						partialBarTextureRectangle = new(
							obtainedBarTextureRectangle.Left,
							obtainedBarTextureRectangle.Top + obtainedBarTextureRectangle.Height - rectangleHeightPixels,
							obtainedBarTextureRectangle.Width,
							rectangleHeightPixels
						);
						break;
					default:
						throw new ArgumentException($"{nameof(Orientation)} has an invalid value.");
				}

				b.Draw(obtainedBarTexture, partialBarPosition, partialBarTextureRectangle, Color.White * alpha * Instance.Config.Alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			}

			if (Config.ToolbarSkillBar.ShowLevelNumber)
			{
				xOffset += LevelNumberToBarSpacing;
				int buffedLevel = skill.GetBuffedLevel(Game1.player);
				bool isModifiedSkill = buffedLevel != currentLevel;

				Color textColor = Color.SandyBrown;
				if (currentLevel == 20 && ((Instance.IsWalkOfLifeInstalled && WalkOfLifeBridge.IsPrestigeEnabled()) || (Instance.IsMargoInstalled && MargoBridge.IsPrestigeEnabled())))
					textColor = Color.Cornsilk;
				if (isModifiedSkill)
					textColor = Color.LightGreen;
				if (buffedLevel == 0)
					textColor *= 0.75f;

				Vector2 levelNumberPosition = wholeToolbarTopLeft + new Vector2(xOffset + 2f + NumberSprite.getWidth(buffedLevel) / 2f, NumberSprite.getHeight() / 2f) * scale;
				NumberSprite.draw(buffedLevel, b, levelNumberPosition + new Vector2(-1, 1) * scale, Color.Black * alpha * 0.35f, 1f, 0f, 1f, 0);
				NumberSprite.draw(buffedLevel, b, levelNumberPosition, textColor * alpha, 1f, 0f, 1f, 0);
			}

			if (nextLevelXP != int.MaxValue)
			{
				Vector2 mouse = new(Game1.getMouseX(), Game1.getMouseY());
				if (mouse.X >= wholeToolbarTopLeft.X && mouse.Y >= wholeToolbarTopLeft.Y && mouse.X < wholeToolbarTopLeft.X + barSize.X && mouse.Y < wholeToolbarTopLeft.Y + barSize.Y)
				{
					ToolbarTooltip.Value = GetSkillTooltip(skill);
					ToolbarActiveDuration.Value = Math.Max(ToolbarActiveDuration.Value, 1f);
				}
			}
		}

		private string GetSkillTooltip(ISkill skill)
		{
			int currentLevel = skill.GetBaseLevel(Game1.player);
			int nextLevelXP = skill.GetLevelXP(currentLevel + 1);
			int currentXP = skill.GetXP(Game1.player);
			int currentLevelXP = skill.GetLevelXP(currentLevel);
			float nextLevelProgress = Math.Clamp(1f * (currentXP - currentLevelXP) / (nextLevelXP - currentLevelXP), 0f, 1f);

			return Helper.Translation.Get(
				"tooltip.text",
				new
				{
					CurrentXP = currentXP - currentLevelXP,
					NextLevelXP = nextLevelXP - currentLevelXP,
					LevelPercent = (int)(nextLevelProgress * 100f)
				}
			);
		}

		private void DrawSkillTooltip(SpriteBatch b, ISkill skill)
			=> IClickableMenu.drawToolTip(b, GetSkillTooltip(skill), null, null);

		private static void Farmer_performFireTool_Prefix(Farmer __instance)
		{
			if (__instance != Game1.player)
				return;
			if (!Instance.Config.ToolbarSkillBar.IsEnabled)
				return;

			if (Instance.Config.ToolbarSkillBar.ToolUseDurationInSeconds > 0f)
			{
				var skill = Instance.GetSkillForItem(Game1.player.CurrentItem);
				if (skill is null)
				{
					Instance.ToolbarActiveDuration.Value = 0f;
				}
				else
				{
					Instance.ToolbarCurrentSkill.Value = skill;
					Instance.ToolbarActiveDuration.Value = Instance.Config.ToolbarSkillBar.ToolUseDurationInSeconds;
				}
			}
		}

		private static void Farmer_gainExperience_Prefix(Farmer __instance, int which, ref (int Level, int XP) __state)
		{
			if (__instance != Game1.player)
				return;
			var skill = new VanillaSkill(which);
			SkillsToRecheck.Value[skill] = (skill.GetBaseLevel(__instance), skill.GetXP(__instance));
		}

		private static void SpaceCore_Skills_AddExperience_Prefix(Farmer farmer, string skillName)
		{
			if (farmer != Game1.player)
				return;
			var skill = new SpaceCoreSkill(skillName);
			SkillsToRecheck.Value[skill] = (skill.GetBaseLevel(farmer), skill.GetXP(farmer));
		}

		private static void SkillsPage_draw_Postfix(SpriteBatch b)
		{
			DrawSkillsPageExperienceTooltip(b);
		}

		private static IEnumerable<CodeInstruction> SkillsPage_draw_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.Do(matcher =>
					{
						return matcher
							.Find(
								ILMatches.AnyLdloc,
								ILMatches.LdcI4(9),
								ILMatches.BneUn
							)
							.PointerMatcher(SequenceMatcherRelativeElement.First)
							.ExtractLabels(out var labels)
							.Insert(
								SequenceMatcherPastBoundsDirection.Before, true,

								new CodeInstruction(OpCodes.Ldarg_1).WithLabels(labels), // `SpriteBatch`

								new CodeInstruction(OpCodes.Ldloc_0), // this *should* be the `x` local
								new CodeInstruction(OpCodes.Ldloc_2), // this *should* be the `addedX` local
								new CodeInstruction(OpCodes.Add),

								new CodeInstruction(OpCodes.Ldloc_1), // this *should* be the `y` local
								new CodeInstruction(OpCodes.Ldloc_3), // this *should* be the `i` local - the currently drawn level index (0-9)
								new CodeInstruction(OpCodes.Ldloc, 4), // this *should* be the `j` local - the skill index
								new CodeInstruction(OpCodes.Ldnull), // no skill name, it's a built-in one
								new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XPDisplay), nameof(SkillsPage_draw_QueueDelegate)))
							);
					})
					.Do(matcher =>
					{
						var skillsPageSkillBarsField = AccessTools.Field(typeof(SkillsPage), nameof(SkillsPage.skillBars));
						return matcher
							.Repeat(2, matcher =>
							{
								return matcher
									.Find(
										ILMatches.Ldarg(0),
										ILMatches.Ldfld(skillsPageSkillBarsField),
										ILMatches.Call(AccessTools.Method(skillsPageSkillBarsField.FieldType, "GetEnumerator"))
									);
							})
							.PointerMatcher(SequenceMatcherRelativeElement.First)
							.ExtractLabels(out var labels)
							.Insert(
								SequenceMatcherPastBoundsDirection.Before, true,
								new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XPDisplay), nameof(SkillsPage_draw_CallQueuedDelegates))).WithLabels(labels)
							);
					})
					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}

		private static void SpaceCore_NewSkillsPage_draw_Postfix(SpriteBatch b)
		{
			DrawSkillsPageExperienceTooltip(b);
		}

		private static IEnumerable<CodeInstruction> SpaceCore_NewSkillsPage_draw_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.Do(matcher =>
					{
						return matcher
							.Find(
								ILMatches.AnyLdloc,
								ILMatches.LdcI4(9),
								ILMatches.BneUn
							)
							.Do(matcher =>
							{
								return matcher
									.PointerMatcher(SequenceMatcherRelativeElement.First)
									.ExtractLabels(out var labels)
									.Insert(
										SequenceMatcherPastBoundsDirection.Before, true,

										new CodeInstruction(OpCodes.Ldarg_1).WithLabels(labels), // `SpriteBatch`

										new CodeInstruction(OpCodes.Ldloc_0), // this *should* be the `x` local
										new CodeInstruction(OpCodes.Ldloc_3), // this *should* be the `xOffset` local
										new CodeInstruction(OpCodes.Add),

										new CodeInstruction(OpCodes.Ldloc_1), // this *should* be the `y` local
										new CodeInstruction(OpCodes.Ldloc, 8), // this *should* be the `levelIndex` local
										new CodeInstruction(OpCodes.Ldloc, 9), // this *should* be the `skillIndex` local
										new CodeInstruction(OpCodes.Ldnull), // no skill name, it's a built-in one
										new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XPDisplay), nameof(SkillsPage_draw_QueueDelegate)))
									);
							})
							.Find(
								ILMatches.AnyLdloc,
								ILMatches.LdcI4(9),
								ILMatches.BneUn
							)
							.Do(matcher =>
							{
								return matcher
									.PointerMatcher(SequenceMatcherRelativeElement.First)
									.ExtractLabels(out var labels)
									.Insert(
										SequenceMatcherPastBoundsDirection.Before, true,

										new CodeInstruction(OpCodes.Ldarg_1).WithLabels(labels), // `SpriteBatch`

										new CodeInstruction(OpCodes.Ldloc_0), // this *should* be the `x` local
										new CodeInstruction(OpCodes.Ldloc_3), // this *should* be the `xOffset` local
										new CodeInstruction(OpCodes.Add),

										new CodeInstruction(OpCodes.Ldloc_1), // this *should* be the `y` local
										new CodeInstruction(OpCodes.Ldloc, 19), // this *should* be the `levelIndex` local
										new CodeInstruction(OpCodes.Ldloc_2), // this *should* be the `indexWithLuckSkill` local
										new CodeInstruction(OpCodes.Ldloc, 17), // this *should* be the `skillName` local
										new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XPDisplay), nameof(SkillsPage_draw_QueueDelegate)))
									);
							});
					})
					.Do(matcher =>
					{
						var skillsPageSkillBarsField = AccessTools.Field(AccessTools.TypeByName(SpaceCoreNewSkillsPageQualifiedName), "skillBars");
						return matcher
							.Repeat(2, matcher =>
							{
								return matcher
									.Find(
										ILMatches.Ldarg(0),
										ILMatches.Ldfld(skillsPageSkillBarsField),
										ILMatches.Call(AccessTools.Method(skillsPageSkillBarsField.FieldType, "GetEnumerator"))
									);
							})
							.PointerMatcher(SequenceMatcherRelativeElement.First)
							.ExtractLabels(out var labels)
							.Insert(
								SequenceMatcherPastBoundsDirection.Before, true,
								new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XPDisplay), nameof(SkillsPage_draw_CallQueuedDelegates))).WithLabels(labels)
							);
					})
					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch SpaceCore methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}

		public static void SkillsPage_draw_QueueDelegate(SpriteBatch b, int x, int y, int levelIndex, int uiSkillIndex, string? spaceCoreSkillName)
		{
			int skillIndex = OrderedSkillIndexes.Length > uiSkillIndex ? OrderedSkillIndexes[uiSkillIndex] : uiSkillIndex;
			ISkill skill = SkillExt.GetSkill(skillIndex, spaceCoreSkillName);

			bool isBigLevel = (levelIndex + 1) % 5 == 0;
			Texture2D barTexture = Game1.mouseCursors;
			Rectangle barTextureRectangle = isBigLevel ? BigObtainedLevelCursorsRectangle : SmallObtainedLevelCursorsRectangle;
			float scale = 4f;

			Vector2 topLeft = new(x + levelIndex * 36, y - 4 + uiSkillIndex * 56);
			Vector2 bottomRight = topLeft + new Vector2(barTextureRectangle.Width, barTextureRectangle.Height) * scale;

			int currentLevel = skill.GetBaseLevel(Game1.player);
			int nextLevelXP = skill.GetLevelXP(currentLevel + 1);
			if (levelIndex is 4 or 9 && currentLevel >= levelIndex)
				SkillBarHoverExclusions.Add((topLeft, bottomRight));

			if (nextLevelXP != int.MaxValue && levelIndex is 0 or 9)
			{
				var key = (uiSkillIndex, spaceCoreSkillName);
				if (!SkillBarCorners.ContainsKey(key))
					SkillBarCorners[key] = (null, null);
				if (levelIndex == 0)
					SkillBarCorners[key] = (topLeft, SkillBarCorners[key].Item2);
				else if (levelIndex == 9)
					SkillBarCorners[key] = (SkillBarCorners[key].Item1, bottomRight);
			}

			if (currentLevel % 10 != levelIndex)
				return;
			int currentLevelXP = skill.GetLevelXP(currentLevel);
			int currentXP = skill.GetXP(Game1.player);
			float nextLevelProgress = Math.Clamp(1f * (currentXP - currentLevelXP) / (nextLevelXP - currentLevelXP), 0f, 1f);

			Orientation orientation = isBigLevel ? Instance.Config.BigBarOrientation : Instance.Config.SmallBarOrientation;

			if (currentLevel >= 10)
			{
				if (Instance.IsWalkOfLifeInstalled && WalkOfLifeBridge.IsPrestigeEnabled())
					(barTexture, barTextureRectangle) = isBigLevel ? WalkOfLifeBridge.GetExtendedBigBar()!.Value : WalkOfLifeBridge.GetExtendedSmallBar()!.Value;
				else if (Instance.IsMargoInstalled && MargoBridge.IsPrestigeEnabled())
					(barTexture, barTextureRectangle) = isBigLevel ? MargoBridge.GetExtendedBigBar()!.Value : MargoBridge.GetExtendedSmallBar()!.Value;
			}

			Vector2 barPosition;
			switch (orientation)
			{
				case Orientation.Horizontal:
					int rectangleWidthPixels = (int)(barTextureRectangle.Width * nextLevelProgress);
					barPosition = topLeft;
					barTextureRectangle = new(
						barTextureRectangle.Left,
						barTextureRectangle.Top,
						rectangleWidthPixels,
						barTextureRectangle.Height
					);
					break;
				case Orientation.Vertical:
					int rectangleHeightPixels = (int)(barTextureRectangle.Height * nextLevelProgress);
					barPosition = topLeft + new Vector2(0f, (barTextureRectangle.Height - rectangleHeightPixels) * scale);
					barTextureRectangle = new(
						barTextureRectangle.Left,
						barTextureRectangle.Top + barTextureRectangle.Height - rectangleHeightPixels,
						barTextureRectangle.Width,
						rectangleHeightPixels
					);
					break;
				default:
					throw new ArgumentException($"{nameof(Orientation)} has an invalid value.");
			}
			SkillsPageDrawQueuedDelegates.Add(() => b.Draw(barTexture, barPosition, barTextureRectangle, Color.White * Instance.Config.Alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.87f));
		}

		public static void SkillsPage_draw_CallQueuedDelegates()
		{
			foreach (var @delegate in SkillsPageDrawQueuedDelegates)
				@delegate();
			SkillsPageDrawQueuedDelegates.Clear();
		}

		private static void DrawSkillsPageExperienceTooltip(SpriteBatch b)
		{
			int mouseX = Game1.getMouseX();
			int mouseY = Game1.getMouseY();
			bool isHoverExcluded = SkillBarHoverExclusions.Any(e => mouseX >= e.Item1.X && mouseY >= e.Item1.Y && mouseX < e.Item2.X && mouseY < e.Item2.Y);
			if (!isHoverExcluded)
			{
				(int uiSkillIndex, string? spaceCoreSkillName)? hoveredUiSkill = SkillBarCorners
					.Where(kv => kv.Value.Item1 is not null && kv.Value.Item2 is not null)
					.Where(kv => mouseX >= kv.Value.Item1!.Value.X && mouseY >= kv.Value.Item1!.Value.Y && mouseX < kv.Value.Item2!.Value.X && mouseY < kv.Value.Item2!.Value.Y)
					.FirstOrNull()
					?.Key;
				if (hoveredUiSkill is not null)
				{
					var (uiSkillIndex, spaceCoreSkillName) = hoveredUiSkill.Value;
					int skillIndex = OrderedSkillIndexes.Length > uiSkillIndex ? OrderedSkillIndexes[uiSkillIndex] : uiSkillIndex;
					ISkill skill = SkillExt.GetSkill(skillIndex, spaceCoreSkillName);

					int currentLevel = skill.GetBaseLevel(Game1.player);
					int nextLevelXP = skill.GetLevelXP(currentLevel + 1);
					if (nextLevelXP != int.MaxValue)
						Instance.DrawSkillTooltip(b, skill);
				}
			}
			SkillBarCorners.Clear();
			SkillBarHoverExclusions.Clear();
		}
	}
}