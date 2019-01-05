using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace Spawn_Monsters
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		/*********
        ** Public methods
        *********/
		public override void Entry(IModHelper helper) {
			helper.ConsoleCommands.Add("monster_spawn", "Spawns a Monster.\n\nUsage: monster_spawn <name> [posX] [posY] [amount]\n\nUses Farmer's coordinates if none or '~' was given.", this.SpawnEntity);
			helper.ConsoleCommands.Add("monster_list", "Shows a lists of all monsters available to spawn.", this.MonsterList);
			helper.ConsoleCommands.Add("monster_menu", "Shows a menu for spawning monsters", this.MonsterMenu);
		}


		/*********
        ** Private methods
        *********/
		private void SpawnEntity(string command, string[] args) {

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
						Monitor.Log("Arguments 1 and 2 must be coordinates or '~' to use the Farmer's coordinates!");
						return;
					}

					try { if (args.Length >= 4) { amount = int.Parse(args[3]); if (amount < 1) throw new Exception(); } } catch (Exception) { Monitor.Log("Argument 3 must be an amount larger than 0!"); return; }

					Vector2 pos = new Vector2(xTile, yTile);

					for (int i = 0; i < amount; i++) {
						// Determine the monster to spawn
						switch (args[0]) {
							case "slime": entity = new GreenSlime(pos); break;

							case "bat": entity = new Bat(pos); break;
							case "bug": entity = new Bug(pos, 0); break;                 //available areatypes: 121 -> armored
							case "duggy": entity = new Duggy(pos); break;
							case "dustSpirit": entity = new DustSpirit(pos); break;
							case "fly": entity = new Fly(pos); break;                   //hard flies?
							case "ghost": entity = new Ghost(pos); break;               //also has constructor with name
							case "grub": entity = new Grub(pos); break;                 //hard grubs?
							case "lavaCrab": entity = new LavaCrab(pos); break;
							case "metalHead": entity = new MetalHead(pos, 0); break;    //mineareas can be 0, 40 or 80; changes color and health accordingly
							case "mummy": entity = new Mummy(pos); break;
							case "rockCrab": entity = new RockCrab(pos); break;         //has constructor with name
							case "rockGolem": entity = new RockGolem(pos); break;       //mineareas: <40, 40-80, >80; difficultymod? alreadyspawned?
							case "serpent": entity = new Serpent(pos); break;
							case "shadowBrute": entity = new ShadowBrute(pos); break;
							case "shadowShaman": entity = new ShadowShaman(pos); break;
							case "skeleton": entity = new Skeleton(pos); break;
							case "squidKid": entity = new SquidKid(pos); break;
						}
						if (entity != null) {
							entity.currentLocation = Game1.currentLocation;
							entity.setTileLocation(new Vector2(xTile, yTile));
							Game1.currentLocation.addCharacter(entity);
						} else { Monitor.Log($"{args[0]} not found! Type monster_list to view a list of available entities to spawn!"); return; }
					}
					Monitor.Log($"{amount} {entity.Name} added at {entity.currentLocation.Name} {entity.getTileX()},{entity.getTileY()}", LogLevel.Info);
				} else {
					Monitor.Log("You need to provide arguments.");
				}
			} else { Monitor.Log("Load a save first!"); }
		}

		private void MonsterList(string command, string[] args) {
			Monitor.Log("Monsters available to spawn:\n\n" +
				"slime\n" +
				"bat\n" +
				"bug\n" +
				"duggy\n" +
				"dustSpirit\n" +
				"fly\n" +
				"ghost\n" +
				"grub\n" +
				"lavaCrab\n" +
				"metalHead\n" +
				"mummy\n" +
				"rockCrab\n" +
				"rockGolem\n" +
				"serpent\n" +
				"shadowBrute\n" +
				"shadowShaman\n" +
				"skeleton\n" +
				"squidKid\n", LogLevel.Info);
		}

		private void MonsterMenu(string command, string[] args) {
			if (Context.IsWorldReady) {
				Game1.activeClickableMenu = new MonsterMenu();
			} else {
				Monitor.Log("Load a save first!");
			}
		}

	}
}
