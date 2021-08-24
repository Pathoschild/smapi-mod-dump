/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheLion.Stardew.Tools
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		public static IModRegistry ModRegistry { get; set; }
		public static IReflectionHelper Reflection { get; set; }
		public static Configs.ToolConfig Config { get; set; }
		
		private Framework.Effects.AxeEffect AxeFx { get; set; }
		private Framework.Effects.PickaxeEffect PickaxeFx { get; set; }

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// get mod registry
			ModRegistry = Helper.ModRegistry;

			// get and verify configs.json
			Config = Helper.ReadConfig<Configs.ToolConfig>();
			VerifyModConfig();

			// get reflection interface
			Reflection = Helper.Reflection;

			// hook events
			Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			Helper.Events.Input.ButtonReleased += OnButtonReleased;

			// create and patch Harmony instance
			var harmony = new Harmony("thelion.AwesomeTools");
			harmony.PatchAll(Assembly.GetExecutingAssembly());

			// add commands for debugging (or cheating)
			Helper.ConsoleCommands.Add("player_settoolsupgrade", "Set the upgrade level of all upgradeable tools in the player's inventory." + GetCommandUsage(), SetToolsUpgrade);
		}

		/// <summary>The event called after the first game update, once all mods are loaded.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// instantiate awesome tool effects
			AxeFx = new Framework.Effects.AxeEffect(Config.AxeConfig, ModRegistry);
			PickaxeFx = new Framework.Effects.PickaxeEffect(Config.PickaxeConfig, ModRegistry);

			// add Generic Mod Config Menu integration
			new Integrations.GenericModConfigMenuIntegrationForAwesomeTools(
				getConfig: () => Config,
				reset: () =>
				{
					Config = new Configs.ToolConfig();
					Helper.WriteConfig(Config);
				},
				saveAndApply: () =>
				{
					Helper.WriteConfig(Config);
				},
				log: Monitor.Log,
				modRegistry: ModRegistry,
				manifest: ModManifest
			).Register();
		}

		/// <summary>Raised after the player pressed/released a keyboard, mouse, or controller button.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
		{
			if (Game1.activeClickableMenu != null || !e.Button.IsUseToolButton())
			{
				return;
			}

			Farmer who = Game1.player;
			Tool tool = who.CurrentTool;
			if (who.toolPower <= 0 || ((tool is not Axe || !AxeFx.Config.EnableAxeCharging) && (tool is not Pickaxe || !PickaxeFx.Config.EnablePickaxeCharging))) return;

			GameLocation location = Game1.currentLocation;
			Vector2 actionTile = new((int)(who.GetToolLocation().X / Game1.tileSize), (int)(who.GetToolLocation().Y / Game1.tileSize));
			DelayedAction shockwave = new(Config.ShockwaveDelay, () =>
			{
				switch (tool)
				{
					case Axe:
						AxeFx.SpreadToolEffect(tool, actionTile, Config.StaminaCostMultiplier, location, who);
						break;

					case Pickaxe:
						PickaxeFx.SpreadToolEffect(tool, actionTile, Config.StaminaCostMultiplier, location, who);
						break;
				}
			});
			Game1.delayedActions.Add(shockwave);
		}

		/// <summary>Check for and fix invalid tool settings.</summary>
		private void VerifyModConfig()
		{
			if (Config.AxeConfig.RadiusAtEachPowerLevel.Count() < 4)
			{
				Monitor.Log("Missing values in configs.json AxeConfig.RadiusAtEachPowerLevel. The default values will be restored.", LogLevel.Warn);
				Config.AxeConfig.RadiusAtEachPowerLevel = new List<int>() { 1, 2, 3, 4 };
			}
			else if (Config.AxeConfig.RadiusAtEachPowerLevel.Any(i => i < 0))
			{
				Monitor.Log("Illegal negative value for shockwave radius in configs.json AxeConfig.RadiusAtEachPowerLevel. Those values will be replaced with zero.", LogLevel.Warn);
				Config.AxeConfig.RadiusAtEachPowerLevel = Config.AxeConfig.RadiusAtEachPowerLevel.Select(x => x < 0 ? 0 : x).ToList();
			}

			if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Count() < 4)
			{
				Monitor.Log("Missing values in configs.json PickaxeConfig.RadiusAtEachPowerLevel. The default values will be restored.", LogLevel.Warn);
				Config.PickaxeConfig.RadiusAtEachPowerLevel = new List<int>() { 1, 2, 3, 4 };
			}
			else if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Any(i => i < 0))
			{
				Monitor.Log("Illegal negative value for shockwave radius in configs.json PickaxeConfig.RadiusAtEachPowerLevel. Those values will be replaced with zero.", LogLevel.Warn);
				Config.PickaxeConfig.RadiusAtEachPowerLevel = Config.PickaxeConfig.RadiusAtEachPowerLevel.Select(x => x < 0 ? 0 : x).ToList();
			}

			if (Config.RequireModkey && !Config.Modkey.IsBound)
			{
				Monitor.Log("'RequireModkey' setting is set to true, but no Modkey is bound. Default keybind will be restored. To disable the Modkey, set this value to false.", LogLevel.Warn);
				Config.Modkey = KeybindList.ForSingle(SButton.LeftShift);
			}

			if (Config.StaminaCostMultiplier < 0)
			{
				Monitor.Log("'StaminaCostMultiplier' is set to a negative value in configs.json. This may cause game-breaking bugs.", LogLevel.Warn);
			}

			if (Config.ShockwaveDelay < 0)
			{
				Monitor.Log("Illegal negative value for 'ShockwaveDelay' in configs.json. The default value will be restored.", LogLevel.Warn);
				Config.ShockwaveDelay = 200;
			}

			if (Framework.Utility.HasHigherLevelToolMod(ModRegistry))
			{
				Monitor.Log("Prismatic or Radioactive Tools detected.", LogLevel.Info);

				if (Config.AxeConfig.RadiusAtEachPowerLevel.Count() < 5)
				{
					Monitor.Log("Adding default fifth radius value to Axe configurations.", LogLevel.Info);
					Config.AxeConfig.RadiusAtEachPowerLevel.Add(5);
				}
				else if (Config.AxeConfig.RadiusAtEachPowerLevel.Count() > 5)
				{
					Monitor.Log("Too many values in configs.json AxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.", LogLevel.Warn);
					Config.AxeConfig.RadiusAtEachPowerLevel = Config.AxeConfig.RadiusAtEachPowerLevel.Take(5).ToList();
				}

				if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Count() < 5)
				{
					Monitor.Log("Adding default fifth radius value to Pickaxe configurations.", LogLevel.Info);
					Config.PickaxeConfig.RadiusAtEachPowerLevel.Add(5);
				}
				else if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Count() > 5)
				{
					Monitor.Log("Too many values in configs.json PickaxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.", LogLevel.Warn);
					Config.PickaxeConfig.RadiusAtEachPowerLevel = Config.PickaxeConfig.RadiusAtEachPowerLevel.Take(5).ToList();
				}
			}
			else
			{
				if (Config.AxeConfig.RadiusAtEachPowerLevel.Count() > 4)
				{
					Monitor.Log("Too many values in configs.json AxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.", LogLevel.Warn);
					Config.AxeConfig.RadiusAtEachPowerLevel = Config.AxeConfig.RadiusAtEachPowerLevel.Take(4).ToList();
				}

				if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Count() > 4)
				{
					Monitor.Log("Too many values in configs.json PickaxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.", LogLevel.Warn);
					Config.PickaxeConfig.RadiusAtEachPowerLevel = Config.PickaxeConfig.RadiusAtEachPowerLevel.Take(4).ToList();
				}
			}

			Helper.WriteConfig(Config);
		}

		/// <summary>Set the upgrade level of all upgradeable tools in the player's inventory.</summary>
		/// <param name="command">The console command.</param>
		/// <param name="args">The supplied arguments.</param>
		private void SetToolsUpgrade(string command, string[] args)
		{
			if (args.Length < 1)
			{
				Monitor.Log("Missing argument." + GetCommandUsage(), LogLevel.Info);
				return;
			}

			int upgradeLevel = args[0] switch
			{
				"copper" => 1,
				"steel" => 2,
				"gold" => 3,
				"iridium" => 4,
				"prismatic" => 5,
				"radioactive" => 5,
				_ => -1
			};

			if (upgradeLevel < 0)
			{
				if (int.TryParse(args[0], out int i) && i <= 5)
				{
					upgradeLevel = i;
				}
				else
				{
					Monitor.Log("Invalid argument." + GetCommandUsage(), LogLevel.Info);
					return;
				}
			}

			if (upgradeLevel == 5 && !Framework.Utility.HasHigherLevelToolMod(ModRegistry))
			{
				Monitor.Log("You must have either 'Prismatic Tools' or 'Radioactive Tools' installed to set this upgrade level.", LogLevel.Warn);
				return;
			}

			foreach (Item item in Game1.player.Items)
			{
				if (item is Axe || item is Hoe || item is Pickaxe || item is WateringCan)
				{
					(item as Tool).UpgradeLevel = upgradeLevel;
				}
			}
		}

		/// <summary>Tell the dummies how to use the console command.</summary>
		private string GetCommandUsage()
		{
			string result = "\n\nUsage: player_upgradetools < level >\n - level: one of 'copper', 'steel', 'gold', 'iridium'";
			if (ModRegistry.IsLoaded("stokastic.PrismaticTools"))
			{
				result += ", 'prismatic'";
			}
			else if (ModRegistry.IsLoaded("kakashigr.RadioactiveTools"))
			{
				result += ", 'radioactive'";
			}

			return result;
		}
	}
}