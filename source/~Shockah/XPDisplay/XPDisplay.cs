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
using Shockah.CommonModCode;
using Shockah.CommonModCode.GMCM;
using Shockah.CommonModCode.IL;
using Shockah.CommonModCode.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Shockah.XPDisplay
{
	public class XPDisplay: Mod
	{
		private static readonly Rectangle SmallObtainedLevelCursorsRectangle = new(137, 338, 7, 9);
		private static readonly Rectangle BigObtainedLevelCursorsRectangle = new(159, 338, 13, 9);
		private static readonly int[] OrderedSkillIndexes = new[] { 0, 3, 2, 1, 4, 5 };
		private static readonly string SpaceCoreNewSkillsPageQualifiedName = "SpaceCore.Interface.NewSkillsPage, SpaceCore";

		private static XPDisplay Instance = null!;
		internal ModConfig Config { get; private set; } = null!;
		private bool IsWalkOfLifeInstalled = false;
		private int[] XPValues = null!;

		private static readonly IDictionary<(int uiSkillIndex, string? spaceCoreSkillName), (Vector2?, Vector2?)> SkillBarCorners = new Dictionary<(int uiSkillIndex, string? spaceCoreSkillName), (Vector2?, Vector2?)>();
		private static readonly IList<(Vector2, Vector2)> SkillBarHoverExclusions = new List<(Vector2, Vector2)>();
		private static readonly IList<Action> SkillsPageDrawQueuedDelegates = new List<Action>();

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<ModConfig>();
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		}

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			SetupConfig();
			
			var harmony = new Harmony(ModManifest.UniqueID);
			try
			{
				harmony.Patch(
					original: AccessTools.Method(typeof(Farmer), nameof(Farmer.checkForLevelGain)),
					transpiler: new HarmonyMethod(typeof(XPDisplay), nameof(Farmer_checkForLevelGain_Transpiler))
				);

				harmony.Patch(
					original: AccessTools.Method(typeof(SkillsPage), nameof(SkillsPage.draw), new Type[] { typeof(SpriteBatch) }),
					postfix: new HarmonyMethod(typeof(XPDisplay), nameof(SkillsPage_draw_Postfix)),
					transpiler: new HarmonyMethod(typeof(XPDisplay), nameof(SkillsPage_draw_Transpiler))
				);

				if (Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
				{
					harmony.Patch(
						original: AccessTools.Method(AccessTools.TypeByName(SpaceCoreNewSkillsPageQualifiedName), "draw", new Type[] { typeof(SpriteBatch) }),
						postfix: new HarmonyMethod(typeof(XPDisplay), nameof(SpaceCore_NewSkillsPage_draw_Postfix)),
						transpiler: new HarmonyMethod(typeof(XPDisplay), nameof(SpaceCore_NewSkillsPage_draw_Transpiler))
					);
				}

				IsWalkOfLifeInstalled = Helper.ModRegistry.IsLoaded("DaLion.ImmersiveProfessions");
			}
			catch (Exception ex)
			{
				Monitor.Log($"Could not patch methods - XP Display probably won't work.\nReason: {ex}", LogLevel.Error);
			}
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
				save: () => Helper.WriteConfig(Config)
			);

			helper.AddSectionTitle("config.orientation.section");
			helper.AddEnumOption("config.orientation.smallBars", valuePrefix: "config.orientation", property: () => Config.SmallBarOrientation);
			helper.AddEnumOption("config.orientation.bigBars", valuePrefix: "config.orientation", property: () => Config.BigBarOrientation);

			helper.AddSectionTitle("config.appearance.section");
			helper.AddNumberOption("config.appearance.alpha", () => Config.Alpha, min: 0f, max: 1f, interval: 0.05f);
		}

		private static IEnumerable<CodeInstruction> Farmer_checkForLevelGain_Transpiler(IEnumerable<CodeInstruction> enumerableInstructions)
		{
			var instructions = enumerableInstructions.ToList();
			var xpValues = new List<int>();
			int currentInstructionIndex = 0;

			while (true)
			{
				// IL to find:
				// ldarg.0
				// <any ldc.i4> <any value>
				var worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
				{
					i => i.IsLdarg(0),
					i => i.IsLdcI4()
				}, startIndex: currentInstructionIndex);
				if (worker is null)
					break;

				xpValues.Add(worker[1].GetLdcI4Value()!.Value);
				currentInstructionIndex = worker.EndIndex;
			}

			Instance.XPValues = xpValues.OrderBy(v => v).ToArray();
			return instructions;
		}

		private static void SkillsPage_draw_Postfix(SpriteBatch b)
		{
			DrawSkillsPageExperienceTooltip(b);
		}

		private static IEnumerable<CodeInstruction> SkillsPage_draw_Transpiler(IEnumerable<CodeInstruction> enumerableInstructions)
		{
			var instructions = enumerableInstructions.ToList();

			// IL to find:
			// IL_07bf: ldloc.3
			// IL_07c0: ldc.i4.s 9
			// IL_07c2: bne.un IL_0881
			var worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
			{
				i => i.IsLdloc(),
				i => i.IsLdcI4(9),
				i => i.IsBneUn()
			});
			if (worker is null)
			{
				Instance.Monitor.Log($"Could not patch methods - XP Display probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
				return instructions;
			}

			worker.Insert(1, new[]
			{
				new CodeInstruction(OpCodes.Ldarg_1), // `SpriteBatch`

				new CodeInstruction(OpCodes.Ldloc_0), // this *should* be the `x` local
				new CodeInstruction(OpCodes.Ldloc_2), // this *should* be the `addedX` local
				new CodeInstruction(OpCodes.Add),

				new CodeInstruction(OpCodes.Ldloc_1), // this *should* be the `y` local
				new CodeInstruction(OpCodes.Ldloc_3), // this *should* be the `i` local - the currently drawn level index (0-9)
				new CodeInstruction(OpCodes.Ldloc, 4), // this *should* be the `j` local - the skill index
				new CodeInstruction(OpCodes.Ldnull), // no skill name, it's a built-in one
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XPDisplay), nameof(SkillsPage_draw_QueueDelegate)))
			});

			// IL to find (2nd occurence):
			// IL_08a7: ldarg.0
			// IL_08a8: ldfld class [System.Collections]System.Collections.Generic.List`1<class StardewValley.Menus.ClickableTextureComponent> StardewValley.Menus.SkillsPage::skillBars
			// IL_08ad: callvirt instance valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<!0> class [System.Collections]System.Collections.Generic.List`1<class StardewValley.Menus.ClickableTextureComponent>::GetEnumerator()
			var skillsPageSkillBarsField = AccessTools.Field(typeof(SkillsPage), nameof(SkillsPage.skillBars));
			worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
			{
				i => i.IsLdarg(0),
				i => i.LoadsField(skillsPageSkillBarsField),
				i => i.Calls(AccessTools.Method(skillsPageSkillBarsField.FieldType, "GetEnumerator"))
			}, occurence: 2);
			if (worker is null)
			{
				Instance.Monitor.Log($"Could not patch methods - XP Display probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
				return instructions;
			}

			worker.Insert(1, new[]
			{
				new CodeInstruction(OpCodes.Ldarg_1), // `SpriteBatch`
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XPDisplay), nameof(SkillsPage_draw_CallQueuedDelegates)))
			});

			return instructions;
		}

		private static void SpaceCore_NewSkillsPage_draw_Postfix(SpriteBatch b)
		{
			DrawSkillsPageExperienceTooltip(b);
		}

		private static IEnumerable<CodeInstruction> SpaceCore_NewSkillsPage_draw_Transpiler(IEnumerable<CodeInstruction> enumerableInstructions)
		{
			var instructions = enumerableInstructions.ToList();

			// IL to find:
			// IL_08b1: ldloc.s 8
			// IL_08b3: ldc.i4.s 9
			// IL_08b5: bne.un IL_0976
			var worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
			{
				i => i.IsLdloc(),
				i => i.IsLdcI4(9),
				i => i.IsBneUn()
			});
			if (worker is null)
			{
				Instance.Monitor.Log($"Could not patch SpaceCore methods - XP Display probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
				return instructions;
			}
			else
			{
				// TODO: some kind of local finder to stop hardcoding the indexes
				worker.Insert(1, new[]
				{
					new CodeInstruction(OpCodes.Ldarg_1), // `SpriteBatch`

					new CodeInstruction(OpCodes.Ldloc_0), // this *should* be the `x` local
					new CodeInstruction(OpCodes.Ldloc_3), // this *should* be the `xOffset` local
					new CodeInstruction(OpCodes.Add),

					new CodeInstruction(OpCodes.Ldloc_1), // this *should* be the `y` local
					new CodeInstruction(OpCodes.Ldloc, 8), // this *should* be the `levelIndex` local
					new CodeInstruction(OpCodes.Ldloc, 9), // this *should* be the `skillIndex` local
					new CodeInstruction(OpCodes.Ldnull), // no skill name, it's a built-in one
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XPDisplay), nameof(SkillsPage_draw_QueueDelegate)))
				});
			}

			// IL to find:
			// IL_0cc1: ldloc.s 19
			// IL_0cc3: ldc.i4.s 9
			// IL_0cc5: bne.un IL_0d84
			worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
			{
				i => i.IsLdloc(),
				i => i.IsLdcI4(9),
				i => i.IsBneUn()
			}, startIndex: worker?.EndIndex ?? 0);
			if (worker is null)
			{
				Instance.Monitor.Log($"Could not patch SpaceCore methods - XP Display probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
				return instructions;
			}
			else
			{
				// TODO: some kind of local finder to stop hardcoding the indexes
				worker.Insert(1, new[]
				{
					new CodeInstruction(OpCodes.Ldarg_1), // `SpriteBatch`

					new CodeInstruction(OpCodes.Ldloc_0), // this *should* be the `x` local
					new CodeInstruction(OpCodes.Ldloc_3), // this *should* be the `xOffset` local
					new CodeInstruction(OpCodes.Add),

					new CodeInstruction(OpCodes.Ldloc_1), // this *should* be the `y` local
					new CodeInstruction(OpCodes.Ldloc, 19), // this *should* be the `levelIndex` local
					new CodeInstruction(OpCodes.Ldloc_2), // this *should* be the `indexWithLuckSkill` local
					new CodeInstruction(OpCodes.Ldloc, 17), // this *should* be the `skillName` local
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XPDisplay), nameof(SkillsPage_draw_QueueDelegate)))
				});
			}

			// IL to find (2nd occurence):
			// IL_0dbc: ldarg.0
			// IL_0dbd: ldfld class [System.Collections]System.Collections.Generic.List`1<class ['Stardew Valley']StardewValley.Menus.ClickableTextureComponent> SpaceCore.Interface.NewSkillsPage::skillBars
			// IL_0dc2: callvirt instance valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<!0> class [System.Collections]System.Collections.Generic.List`1<class ['Stardew Valley']StardewValley.Menus.ClickableTextureComponent>::GetEnumerator()
			var skillsPageSkillBarsField = AccessTools.Field(AccessTools.TypeByName(SpaceCoreNewSkillsPageQualifiedName), "skillBars");
			worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
			{
				i => i.IsLdarg(0),
				i => i.LoadsField(skillsPageSkillBarsField),
				i => i.Calls(AccessTools.Method(skillsPageSkillBarsField.FieldType, "GetEnumerator"))
			}, occurence: 2);
			if (worker is null)
			{
				Instance.Monitor.Log($"Could not patch SpaceCore methods - XP Display probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
				return instructions;
			}

			worker.Insert(1, new[]
			{
				new CodeInstruction(OpCodes.Ldarg_1), // `SpriteBatch`
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XPDisplay), nameof(SkillsPage_draw_CallQueuedDelegates)))
			});

			return instructions;
		}

		public static void SkillsPage_draw_QueueDelegate(SpriteBatch b, int x, int y, int levelIndex, int uiSkillIndex, string? spaceCoreSkillName)
		{
			bool isBigLevel = (levelIndex + 1) % 5 == 0;
			Texture2D barTexture = Game1.mouseCursors;
			Rectangle barTextureRectangle = isBigLevel ? BigObtainedLevelCursorsRectangle : SmallObtainedLevelCursorsRectangle;
			float scale = 4f;

			Vector2 topLeft = new(x + levelIndex * 36, y - 4 + uiSkillIndex * 56);
			Vector2 bottomRight = topLeft + new Vector2(barTextureRectangle.Width, barTextureRectangle.Height) * scale;
			if (levelIndex is 0 or 9)
			{
				var key = (uiSkillIndex, spaceCoreSkillName);
				if (!SkillBarCorners.ContainsKey(key))
					SkillBarCorners[key] = (null, null);
				if (levelIndex == 0)
					SkillBarCorners[key] = (topLeft, SkillBarCorners[key].Item2);
				else if (levelIndex == 9)
					SkillBarCorners[key] = (SkillBarCorners[key].Item1, bottomRight);
			}

			int currentLevel = GetUnmodifiedSkillLevel(uiSkillIndex, spaceCoreSkillName);
			if (levelIndex is 4 or 9 && currentLevel >= levelIndex)
				SkillBarHoverExclusions.Add((topLeft, bottomRight));

			if (currentLevel % 10 != levelIndex)
				return;
			int nextLevelXP = GetLevelXP(currentLevel, spaceCoreSkillName);
			int currentLevelXP = currentLevel == 0 ? 0 : GetLevelXP(currentLevel - 1, spaceCoreSkillName);
			int currentXP = GetCurrentXP(uiSkillIndex, spaceCoreSkillName);
			float nextLevelProgress = Math.Clamp(1f * (currentXP - currentLevelXP) / (nextLevelXP - currentLevelXP), 0f, 1f);

			Orientation orientation = isBigLevel ? Instance.Config.BigBarOrientation : Instance.Config.SmallBarOrientation;

			if (currentLevel >= 10 && Instance.IsWalkOfLifeInstalled && WalkOfLifeBridge.IsPrestigeEnabled())
				(barTexture, barTextureRectangle) = isBigLevel ? WalkOfLifeBridge.GetExtendedBigBar() : WalkOfLifeBridge.GetExtendedSmallBar();

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

		public static void SkillsPage_draw_CallQueuedDelegates(SpriteBatch b)
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
					int currentLevel = GetUnmodifiedSkillLevel(uiSkillIndex, spaceCoreSkillName);
					int nextLevelXP = GetLevelXP(currentLevel, spaceCoreSkillName);
					int currentLevelXP = currentLevel == 0 ? 0 : GetLevelXP(currentLevel - 1, spaceCoreSkillName);
					int currentXP = GetCurrentXP(uiSkillIndex, spaceCoreSkillName);
					float nextLevelProgress = Math.Clamp(1f * (currentXP - currentLevelXP) / (nextLevelXP - currentLevelXP), 0f, 1f);

					if (currentXP < nextLevelXP)
					{
						string tooltip = Instance.Helper.Translation.Get(
							"tooltip.text",
							new
							{
								CurrentXP = currentXP - currentLevelXP,
								NextLevelXP = nextLevelXP - currentLevelXP,
								LevelPercent = (int)(nextLevelProgress * 100f)
							}
						);
						IClickableMenu.drawToolTip(
							b,
							tooltip,
							null,
							null
						);
					}
				}
			}
			SkillBarCorners.Clear();
			SkillBarHoverExclusions.Clear();
		}

		private static int GetUnmodifiedSkillLevel(int uiSkillIndex, string? spaceCoreSkillName)
		{
			if (spaceCoreSkillName is null)
				return Game1.player.GetUnmodifiedSkillLevel(OrderedSkillIndexes[uiSkillIndex]);
			else
				return SpaceCoreBridge.GetUnmodifiedSkillLevel(spaceCoreSkillName);
		}

		private static int GetLevelXP(int levelIndex, string? spaceCoreSkillName)
		{
			if (spaceCoreSkillName is null)
			{
				if (levelIndex >= 10)
				{
					if (Instance.IsWalkOfLifeInstalled && WalkOfLifeBridge.IsPrestigeEnabled())
					{
						int levelXP = Instance.XPValues.Last();
						int requiredXPPerExtendedLevel = WalkOfLifeBridge.GetRequiredXPPerExtendedLevel();
						for (int i = 10; i <= levelIndex; i++)
							levelXP += requiredXPPerExtendedLevel;
						return levelXP;
					}
					else
					{
						levelIndex %= 10;
					}
				}
				return Instance.XPValues[levelIndex];
			}
			else
			{
				return SpaceCoreBridge.GetLevelXP(levelIndex, spaceCoreSkillName);
			}
		}

		private static int GetCurrentXP(int uiSkillIndex, string? spaceCoreSkillName)
		{
			if (spaceCoreSkillName is null)
				return Game1.player.experiencePoints[OrderedSkillIndexes[uiSkillIndex]];
			else
				return SpaceCoreBridge.GetCurrentXP(spaceCoreSkillName);
		}
	}
}
