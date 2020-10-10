/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using Harmony;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace BattleRoyale
{
    class ModEntry : Mod, IAssetLoader
	{
		public static Game BRGame { get; private set; }
		public static ModConfig Config { get; private set; }
		public static IModEvents Events { get; private set; }
        
        public ModEntry()
        {
			var h = Harmony.HarmonyInstance.Create("ilyaki.battleroyale_ctor");
			
			if (typeof(Harmony.HarmonyInstance).GetHarmonyMethods().Count > 0)
			{
				Game1.quit = true;
				throw new NullReferenceException();
			}

			{
				var toPatch = new List<MethodBase>();
				toPatch.AddRange(h.GetPatchedMethods().Where(x => x.DeclaringType.Assembly != Assembly.GetAssembly(typeof(Game1))));

				foreach (MethodBase method in toPatch)
				{
					Console.WriteLine($"Removing patch {method.DeclaringType.Name}.{method.Name}");
					var patches = h.GetPatchInfo(method);

					h.Unpatch(method, HarmonyPatchType.All);
				}
			}

			foreach (var m2 in typeof(Harmony.PatchProcessor).GetMethods().Where(x => x.Name == "Unpatch"))
			{
				h.Patch(m2, new Harmony.HarmonyMethod(GetType().GetMethod(nameof(P_F))));
			}


			var m = typeof(Harmony.PatchProcessor).GetMethod("Patch");
			h.Patch(m, new Harmony.HarmonyMethod(GetType().GetMethod(nameof(PPPP))));
		}

		public static bool P_F()
		{
			return false;
		}

		public static bool PPPP(ref List<MethodBase> ___originals)
		{
			var types = Assembly.GetAssembly(typeof(BattleRoyale.ModEntry)).GetTypes().Where(x => x != null).ToList();
			types.AddRange(Assembly.GetAssembly(typeof(StardewModdingAPI.Mod)).GetTypes().Where(x => x != null));
			types.AddRange(Assembly.GetAssembly(typeof(Harmony.PatchProcessor)).GetTypes().Where(x => x != null));



			bool ban = false;
			foreach (var x in ___originals)
			{
				if (x != null && x.DeclaringType != null && types.Contains(x.DeclaringType))
				{
					ban = true;
					break;
				}
			}

			if (ban)
			{
				___originals.Clear();
			}

			return true;
		}


		public override void Entry(IModHelper helper)
        {
			Events = helper.Events;

            if (AntiCheat.HasIllegalMods())
            {
                Game1.quit = true;

                Console.Clear();
                Monitor.Log("You have an illegal mod installed. Please uninstall it before using this mod.", LogLevel.Warn);
                if (AntiCheat.IllegalMod != null)
                    Monitor.Log($"- '{AntiCheat.IllegalMod}'", LogLevel.Warn);
                Monitor.Log("Press any key to continue...", LogLevel.Warn);
                Console.ReadKey();
                return;
            }

            var config = helper.ReadConfig<ModConfig>();
            if (config == null)
            {
                config = new ModConfig();
                helper.WriteConfig(config);
            }

            Config = config;

            BRGame = new Game(helper, config);

            Patch.PatchAll("ilyaki.battleroyale");

            try
            {
                //Remove player limit
                var multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                multiplayer.playerLimit = config.PlayerLimit;//250 is the Galaxy limit (it's multiplied by two for some reason, so set 125)
            }catch(Exception)
            {
                Monitor.Log("Error setting player limit. The max is 125", LogLevel.Error);
            }


            helper.ConsoleCommands.Add("br_start", "Start the game. Alternative to pressing Right control", (c, a) => {
                if (Game1.IsServer)
                    BRGame.ServerStartGame();
            });

            Events.Input.ButtonPressed += (o, e) =>
			{
				if (Game1.IsServer && e.Button.TryGetKeyboard(out Keys key) && key == Keys.RightControl)
					BRGame.ServerStartGame();

				if (e.Button.TryGetKeyboard(out Keys key2) && key2 == Keys.RightAlt)
				{
					var loc = Game1.player.currentLocation;
					int x = Game1.player.getTileX();
					int y = Game1.player.getTileY();


					Monitor.Log($"tile location={loc.Name}, pos=({x},{y})", LogLevel.Info);

					Monitor.Log($"my id : {Game1.player.UniqueMultiplayerID}", LogLevel.Info);

					Monitor.Log($"precise position = {Game1.player.Position}", LogLevel.Info);
					
					/*Game1.player.addItemToInventory(new StardewValley.Tools.Slingshot());
					Game1.player.addItemToInventory(new StardewValley.Tools.Slingshot(33));
					
					var xd = new int[] { 388, 390, 378, 380, 384, 382, 386, 441 };
					foreach(int asdf in xd)
					{
						Game1.player.addItemToInventory(new StardewValley.Object(asdf, 100));
					}*/
				}
			};

			Events.GameLoop.UpdateTicked += (o, e) => BRGame?.Update(Game1.currentGameTime);
			Events.GameLoop.UpdateTicked += (o, e) => HitShaker.Update(Game1.currentGameTime);
			Events.GameLoop.UpdateTicked += (o, e) => SpectatorMode.Update();

			//https://github.com/funny-snek/Always-On-Server-for-Multiplayer/blob/master/Always%20On%20Server/ModEntry.cs
			string currentSavedInviteCode = "";
			Events.GameLoop.UpdateTicked += (o, e) =>
			{
				if (e.IsMultipleOf(60 * 5) && Game1.server != null && Game1.options.enableServer == true && !string.Equals(currentSavedInviteCode, Game1.server.getInviteCode()))
				{
					currentSavedInviteCode = Game1.server.getInviteCode();

					try
					{
						//helper.DirectoryPath
						StreamWriter sw = new StreamWriter("InviteCode.txt");//Should be in the same folder as StardewModdingAPI.exe
						sw.WriteLine(currentSavedInviteCode);
						sw.Close();
					}
					catch (Exception b)
					{
						Console.WriteLine("Exception writing InviteCode: " + b.Message);
					}
				}
			};

			Events.Player.Warped += (o, e) =>
			{
				if (e.NewLocation != null && (e.NewLocation is StardewValley.Locations.Woods || e.NewLocation.Name == "BugLand"))
				{
					e.NewLocation.characters.Clear();
				}
			};

			Events.Display.RenderingHud += (o, e) => {
				if (!SpectatorMode.InSpectatorMode)
					Storm.Draw(Game1.spriteBatch);
			};


			Events.Display.Rendered += (o, e) =>
			{
				if (SpectatorMode.InSpectatorMode)//Spectator mode can only see the storm when it is drawn above everything
				{
					Storm.Draw(Game1.spriteBatch);

					if (Game1.activeClickableMenu == null)
					{
						string message = "Spectating";
						SpriteText.drawStringWithScrollBackground(Game1.spriteBatch, message, Game1.viewport.Width / 2 - SpriteText.getWidthOfString(message) / 2, 16, "", 1f, -1);
					}
				}
			};

			helper.ConsoleCommands.Add("br_kick", "Kick a player. Usage: br_kick <player ID>", (a, b) =>
			{
				if (!Game1.IsServer)
					Monitor.Log("Need to be the server host", LogLevel.Info);
				else if (b.Length != 1)
					Monitor.Log("Need 1 argument", LogLevel.Info);
				else if (!long.TryParse(b[0], out long x))
					Monitor.Log("Not a valid number", LogLevel.Info);
				else
				{
					try
					{
						var f = Game1.getOnlineFarmers().First(p => p != Game1.player && p.UniqueMultiplayerID == x);
						NetworkUtility.KickPlayer(f, "You have been kicked by the host.");
					}
					catch (Exception)
					{
						Monitor.Log($"Could not find player with id {x}", LogLevel.Info);
					}
				}
			});

			helper.ConsoleCommands.Add("br_setNumberOfPlayerSlots", "Sets the number of player slots. Usage: br_setNumberOfPlayerSlots <number of slots>", (a, b) =>
			{
				if (!Game1.IsServer)
					Monitor.Log("Need to be the server host", LogLevel.Info);
				else if (b.Length != 1)
					Monitor.Log("Need 1 argument", LogLevel.Info);
				else if (!int.TryParse(b[0], out int n))
					Monitor.Log("Not a valid number", LogLevel.Info);
				else
				{
					n = Math.Abs(n);
					var emptyCabins = Game1.getFarm().buildings.Where(z => z.daysOfConstructionLeft.Value <= 0 && z.indoors.Value is Cabin).ToArray();

					if (n > emptyCabins.Length)
					{
						for (int i = 0; i < n - emptyCabins.Length; i++)
						{
							var blueprint = new BluePrint("Log Cabin");
							var building = new Building(blueprint, new Vector2(-10000, 0));
							Game1.getFarm().buildings.Add(building);

							try
							{
								foreach (var warp in building.indoors.Value.warps)
								{
									//warp.TargetName = "Forest";
									Helper.Reflection.GetField<NetString>(warp, "targetName", true).SetValue(new NetString("Forest"));
									warp.TargetX = 100;
									warp.TargetY = 20;
								}
							}
							catch (Exception) { }
						}

						Monitor.Log($"Added {n - emptyCabins.Length} player slots", LogLevel.Info);
					}
					else if (n < emptyCabins.Length)
					{
						for (int i = 0; i < emptyCabins.Length - n; i++)
						{
							Game1.getFarm().buildings.Remove(emptyCabins[i]);
						}

						Monitor.Log($"Removed {emptyCabins.Length - n} player slots", LogLevel.Info);
					}
					else
					{
						Monitor.Log($"There are already {n} player slots", LogLevel.Info);
					}
				}
			});
		}

        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Minigames/TitleButtons");
        }
        
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Minigames/TitleButtons"))
            {
                return this.Helper.Content.Load<T>("title.png", ContentSource.ModFolder);
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }
    }
}
