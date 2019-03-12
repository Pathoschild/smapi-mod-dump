using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

namespace Spawn_Monsters
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{

		/*********
		** Properties
		*********/
		/// <summary>The mod configuration from the player.</summary>
		public ModConfig config;

		/*********
        ** Public methods
        *********/
		public override void Entry(IModHelper helper) {
			helper.ConsoleCommands.Add("monster_spawn", "Spawns a Monster.\n\nUsage: monster_spawn <name> [posX] [posY] [amount]\n\nUses Farmer's coordinates if none or '~' was given.", this.SpawnEntity);
			helper.ConsoleCommands.Add("monster_list", "Shows a lists of all monsters available to spawn.", this.MonsterList);
			helper.ConsoleCommands.Add("monster_menu", "Shows a menu for spawning monsters", this.MonsterMenu);

			config = helper.ReadConfig<ModConfig>();

			helper.Events.Input.ButtonPressed += this.OnButtonPressed;
		}

		/*********
        ** Input Methods
        *********/
		private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
			if (!Context.IsPlayerFree)
				return;
			if (e.Button == this.config.MenuKey) {
				Game1.activeClickableMenu = new MonsterMenu();
			}
		}

		/*********
        ** Command Methods
        *********/
		public void SpawnEntity(string command, string[] args) {

			//We need a world to spawn monsters in, duh
			if (Context.IsWorldReady) {

				// Determine if we have arguments
				if (args.Length > 0) {

					//Set defaults
					NPC entity = null;
					int xTile = Game1.player.getTileX();
					int yTile = Game1.player.getTileY();
					int amount = 1;

					//Ensure provided coordinatees are actually coordinates
					try {
						//Determine X tile
						if (args.Length >= 2) { if (!args[1].Equals("~")) xTile = int.Parse(args[1]); }

						//Determine Y tile
						if (args.Length >= 3) { if (!args[1].Equals("~")) yTile = int.Parse(args[2]); }
					} catch (Exception) {
						Monitor.Log("Arguments 1 and 2 must be coordinates or '~' to use the Farmer's coordinates! Make sure you don't add any brackets!");
						return;
					}

					try { if (args.Length >= 4) { amount = int.Parse(args[3]); if (amount < 1) throw new Exception(); } } catch (Exception) { Monitor.Log("Argument 3 must be an amount larger than 0!"); return; }

					Vector2 pos = new Vector2(xTile, yTile);

					for (int i = 0; i < amount; i++) {
						// Determine the monster to spawn
						switch (args[0]) {

							case "greenSlime": entity = new GreenSlime(pos, 0); break;
							case "blueSlime": entity = new GreenSlime(pos, 40); break;
							case "redSlime": entity = new GreenSlime(pos, 80); break;
							case "purpleSlime": entity = new GreenSlime(pos, 121); break;
							case "yellowSlime": entity = new GreenSlime(pos, new Color(255, 255, 50)); break;
							case "blackSlime": Random r = new Random(); entity = new GreenSlime(pos, new Color(40 + r.Next(10), 40 + r.Next(10), 40 + r.Next(10))); break;

							case "bat": entity = new Bat(pos); break;                           //minelevel: 0 - 40 - 80 - 171 -> type
							case "frostBat": entity = new Bat(pos, 40); break;
							case "lavaBat": entity = new Bat(pos, 80); break;
							case "iridiumBat": entity = new Bat(pos, 171); break;

							case "bug": entity = new Bug(pos, 0); break;                        //available areatypes: 121 -> armored
							case "armoredBug": entity = new Bug(pos, 121); break;

							case "fly": entity = new Fly(pos); break;                           //hard -> mutant
							case "mutantFly": entity = new Fly(pos, true); break;

							case "ghost": entity = new Ghost(pos); break;                       //name -> carbon ghost
							case "carbonGhost": entity = new Ghost(pos, "Carbon Ghost"); break;

							case "grub": entity = new Grub(pos); break;                         //hard -> mutant
							case "mutantGrub": entity = new Grub(pos, true); break;

							case "rockCrab": entity = new RockCrab(pos); break;                 //name -> iridium crab
							case "lavaCrab": entity = new LavaCrab(pos); break;
							case "iridiumCrab": entity = new RockCrab(pos, "Iridium Crab"); break;

							case "metalHead": entity = new MetalHead(pos, 80); break;            //mineareas: 0, 40, 80 - seems to only spawn at 80+

							case "rockGolem": entity = new RockGolem(pos); break;               //mineareas: 0, 40, 80 - changes health and damage; difficultymod: 
							case "wildernessGolem": entity = new RockGolem(pos, 5); break;

							case "mummy": entity = new Mummy(pos); break;
							case "serpent": entity = new Serpent(pos); break;
							case "shadowBrute": entity = new ShadowBrute(pos); break;
							case "shadowShaman": entity = new ShadowShaman(pos); break;
							case "skeleton": entity = new Skeleton(pos); break;
							case "squidKid": entity = new SquidKid(pos); break;
							case "duggy": entity = new Duggy(pos); break;
							case "dustSpirit": entity = new DustSpirit(pos); break;
						}
						if (entity != null) {
							entity.currentLocation = Game1.currentLocation;
							entity.setTileLocation(new Vector2(xTile, yTile));
							Game1.currentLocation.addCharacter(entity);
						} else { Monitor.Log($"{args[0]} not found! Type monster_list to view a list of available monsters to spawn!"); return; }
					}
					Monitor.Log($"{amount} {entity.Name} added at {entity.currentLocation.Name} {entity.getTileX()},{entity.getTileY()}", LogLevel.Info);
				} else {
					Monitor.Log("Usage: monster_spawn <name> [posX] [posY] [amount]\n\nUses Farmer's coordinates if none or '~' was given.");
				}
			} else { Monitor.Log("Load a save first!"); }
		}

		public void MonsterList(string command, string[] args) {
			Monitor.Log("Monsters available to spawn:\n\n" +
				"Slimes:\n" +
				"	greenSlime\n" +
				"	blueSlime\n" +
				"	redSlime\n" +
				"	purpleSlime\n" +
				"	yellowSlime\n" +
				"	blackSlime\n\n" +

				"Bats:\n" +
				"	bat\n" +
				"	frostBat\n" +
				"	lavaBat\n" +
				"	iridiumBat\n\n" +

				"Bugs:\n" +
				"	bug\n" +
				"	armoredBug\n\n" +

				"Flies: \n" +
				"	fly\n" +
				"	grub\n" +
				"	mutantFly\n" +
				"	mutantGrub\n\n" +

				"Ghosts:\n" +
				"	ghost\n" +
				"	carbonGhost\n\n" +

				"Crabs:\n" +
				"	rockCrab\n" +
				"	lavaCrab\n" +
				"	iridiumCrab\n\n" +

				"Golems:\n" +
				"	rockGolem\n" +
				"	wildernessGolem\n\n" +

				"Other:\n" +
				"	duggy\n" +
				"	dustSpirit\n" +
				"	metalHead\n" +
				"	mummy\n" +
				"	serpent\n" +
				"	shadowBrute\n" +
				"	shadowShaman\n" +
				"	skeleton\n" +
				"	squidKid\n\n" +

				"Use these names with 'monster_spawn'\n" +
				"Keep in mind that some monsters don't work properly outside of the farm and the mines!", LogLevel.Info);
		}

		public void MonsterMenu(string command, string[] args) {
			if (Context.IsWorldReady) {
				Game1.activeClickableMenu = new MonsterMenu();
			} else {
				Monitor.Log("Load a save first!");
			}
		}

	}
}
