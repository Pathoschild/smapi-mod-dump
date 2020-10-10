/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleRoyale
{
    class Game
	{
		public bool IsGameInProgress { get; private set; } = false;
		public IModHelper ModHelper { get; private set; }
		private readonly ModConfig modConfig;

		public DateTime TimeWhenGameStarted { get; private set; }
		private DateTime? WhenToStartNextRound = null;
		private bool waitingForNextRoundToStart = false;

		public List<long> alivePlayers = new List<long>();//make private
		
		public OverlayUI OverlayUI { get; private set; }

		private const int maxHealth = 100;

		private const int knockbackImmunityMilliseconds = 2500;

		//Used to replant trees after match
		List<TileLocation> trees = new List<TileLocation>();

		public Game(IModHelper helper, ModConfig modConfig)
		{
			this.ModHelper = helper;
			this.modConfig = modConfig;

			Spawns.LoadSpawnLocations(helper);
			Chests.LoadChestsSpawns(helper);
			Storm.LoadStormData(helper);
			
			//Ban people from hiding in the elevator building.

			ModEntry.Events.Display.MenuChanged += (o, e) =>
			{
				if (IsGameInProgress && e.NewMenu != null && e.NewMenu.GetType().Name == "ElevatorMenu")
					e.NewMenu.exitThisMenu(false);
			};
			
			ModEntry.Events.GameLoop.UpdateTicked += (o,e) => {
				if (IsGameInProgress && Game1.IsServer && e.IsMultipleOf(8))
					Storm.QuarterSecUpdate(alivePlayers);
			};
		}
		
		public void ServerStartGame()
		{
			if (IsGameInProgress)
			{
				Console.WriteLine("A game is already in progress!");
				return;
			}
			IsGameInProgress = true;

			Console.WriteLine("Server start game");

			
			NetworkUtility.SendChatMessageToAllPlayers("Game starting!");

			//Wipe objects
			foreach (GameLocation location in Game1.locations)
			{
				location.Objects.Clear();
				location.debris.Clear();
			}


			Game1.MasterPlayer.Money = 0;
			
			//Spawn chests 
			new Chests().SpawnAndFillChests();

			#region Store tree locations or replant
			Random random = new Random();
			if (trees.Count == 0)//Store info
			{
				foreach (GameLocation gameLocation in Game1.locations)
				{
					foreach (var pair in gameLocation.terrainFeatures.Pairs)
					{
						TerrainFeature feature = pair.Value;
						if(feature is Tree tree && tree.growthStage.Value >= 5)
						{
							Vector2 vector = pair.Key;
							trees.Add(new TileLocation(gameLocation.Name, (int)vector.X, (int)vector.Y));
						}
					}
				}
			}
			else//Replant
			{
				//Delete all trees
				foreach (GameLocation gameLocation in Game1.locations)
				{
					List<Vector2> toRemove = new List<Vector2>();
					foreach (Vector2 vector in gameLocation.terrainFeatures.Keys)
					{
						TerrainFeature feature = gameLocation.terrainFeatures[vector];
						if (feature is Tree tree && tree.growthStage.Value >= 5)
							toRemove.Add(vector);
					}

					foreach (Vector2 vector in toRemove)
						gameLocation.terrainFeatures.Remove(vector);
				}
				
				//Replant
				foreach (TileLocation treeLocation in trees)
				{
                    treeLocation.GetGameLocation().terrainFeatures.Remove(treeLocation.CreateVector2());
                    treeLocation.GetGameLocation().terrainFeatures.Add(treeLocation.CreateVector2(), new Tree(random.Next(1, 4), 100));
				}
			}
			#endregion

			//Remove resource clumps (e.g. boulders) other than stumps and hollow logs
			var rc = Game1.getFarm().resourceClumps;
			foreach (var a in rc.Where(a => a.parentSheetIndex.Value != 600 || a.parentSheetIndex.Value != 602).ToList())
			{
				rc.Remove(a);
			}

			//Remove small stones and weeds
			var farmObjects = Game1.getFarm().objects;
			var keys = farmObjects.Keys.ToList();
			for (int i = 0; i < keys.Count(); i++)
			{
				var key0 = keys[i];

				if (farmObjects[key0].name == "Stone" || farmObjects[key0].name == "Weeds")
					farmObjects.Remove(key0);
			}

			//Setup alive players
			alivePlayers.Clear();
			foreach (Farmer player in Game1.getOnlineFarmers())
				if (player != null && !(player == Game1.player && !modConfig.ShouldHostParticipate))
				{
					Console.WriteLine($"Adding {player.Name} to the game");
					alivePlayers.Add(player.UniqueMultiplayerID);
				}

			//Storm index
			int stormIndex = Storm.GetRandomStormIndex();

			//Spawn players in & Tell the clients to start game
			var chosenSpawns = new Spawns().ScatterPlayers(Game1.getOnlineFarmers());
			foreach (Farmer player in chosenSpawns.Keys)
			{
				if (player == Game1.player)
				{
					ClientStartGame(alivePlayers.Count, modConfig.ShouldHostParticipate, stormIndex);

					if (modConfig.ShouldHostParticipate)
					{
						NetworkUtility.WarpFarmer(player, chosenSpawns[player]);
					}else
					{
						player.warpFarmer(new TileLocation("Forest", 100, 20).CreateWarp());
					}
				}
				else
				{
					NetworkUtility.BroadcastGameStartToClient(player, alivePlayers.Count, modConfig.ShouldHostParticipate, stormIndex);
					NetworkUtility.WarpFarmer(player, chosenSpawns[player]);
				}

			}

			//Remove horses/NPCs
			foreach (GameLocation loc in Game1.locations)
			{
				foreach (NPC horse in loc.characters.Where(x => ModEntry.Config.KillAllNPCs || x is StardewValley.Characters.Horse).ToList())
				{
					loc.characters.Remove(horse);
				}
			}

			//Freeze time
			Game1.timeOfDay = 1400;
			FreezeTime.TimeFrozen = true;
		}

		public void ClientStartGame(int numberOfPlayers, bool isHostParticipating, int stormIndex)//string locationName, int tileX, int tileY)
		{
			Console.WriteLine($"Client start game");
			IsGameInProgress = true;

			//Prevent draw issues if they were holding a slingshot before round ended/started
			foreach (Farmer p in Game1.getAllFarmers())
			{
				p.forceCanMove();
			}

			//Setup alive players (server has already done this in ServerStartGame)
			if (!Game1.IsServer)
			{
				alivePlayers.Clear();
				foreach (Farmer player in Game1.getOnlineFarmers())
					if (player != null && !(player == Game1.MasterPlayer && !isHostParticipating))
					{
						Console.WriteLine($"Adding {player.Name} to the game");
						alivePlayers.Add(player.UniqueMultiplayerID);
					}
			}

			//Time
			TimeWhenGameStarted = DateTime.Now;

			//Freeze time
			Game1.timeOfDay = 1200;
			FreezeTime.TimeFrozen = true;

			//Wipe quests
			Game1.player.questLog.Clear();

			//Storm
			Storm.StartStorm(stormIndex);

			//Open locations
			var strings = new string[] { "OpenedSewer", "wizardJunimoNote", "ccDoorUnlock", "guildMember", "krobusUnseal" };
			foreach (var s in strings)
			{
				if (!Game1.player.mailReceived.Contains(s))
					Game1.player.mailReceived.Add(s);
			}

			Game1.player.hasDarkTalisman = true;//Removes the chest in bugland


			//Set NPC relations to 2 hearts so you can enter every room
			var namesToIgnore = new string[] { "Krobus", "Dwarf", "Sandy", "Kent" };//Because spoilers
			foreach (GameLocation gameLocation in Game1.locations)
			{
				foreach (NPC npc in gameLocation.characters.Where(x => !namesToIgnore.Contains(x.Name)))
				{
					Game1.player.hasPlayerTalkedToNPC(npc.Name);//Creates friendship entries if they have never met before
					if (Game1.player.friendshipData.ContainsKey(npc.Name))
						Game1.player.friendshipData[npc.Name].Points = 500;//Each heart is 250 points
				}

			}
			
			//Clear buffs
			Game1.player.removeBuffAttributes();
			Game1.player.maxHealth = maxHealth;
			Game1.player.health = maxHealth;
            Game1.player.Stamina = Game1.player.MaxStamina;
			Game1.player.swimming.Value = false;
            Game1.player.changeOutOfSwimSuit();

			//Items
			Game1.player.MaxItems = 36;
			Game1.player.leftRing.Set(null);
			Game1.player.rightRing.Set(null);
			Game1.player.boots.Set(null);
			Game1.getFarm().lastItemShipped = null;

			for (int i = 0; i < Game1.player.items.Count; i++)
			{
				if (!(Game1.player.items[i] is StardewValley.Objects.Hat))
					Game1.player.items[i] = null;
			}

			Game1.player.addItemToInventory(new StardewValley.Tools.Axe() { UpgradeLevel = 3 });//3 = Gold level
			//Game1.player.addItemToInventory(new StardewValley.Tools.Pickaxe() { UpgradeLevel = 3 }); Pickaxe lets them have too much slingshot ammo


			//Setup crafting recipes
			Game1.player.craftingRecipes.Clear();
			Game1.player.craftingRecipes.Add("Gate", 0);
			Game1.player.craftingRecipes.Add("Wood Fence", 0);
			Game1.player.craftingRecipes.Add("Hardwood Fence", 0);
			//Game1.player.craftingRecipes.Add("Stone Fence", 0); (Need pickaxe to break)
			Game1.player.craftingRecipes.Add("Chest", 0);

			//Clear mail
			Game1.mailbox?.Clear();
			
			//Dismount
			var horse = Game1.player.mount;
			if (horse != null)
			{
				horse.dismount();
				Game1.currentLocation.characters.Remove(horse);
				Game1.player.mount = null;
			}

			//Setup UI
			if (!(Game1.activeClickableMenu is CharacterCustomization))
                Game1.activeClickableMenu?.exitThisMenu(false);

			IClickableMenu toRemove = null;
			foreach (IClickableMenu menu in Game1.onScreenMenus)
			{
				if (menu is DayTimeMoneyBox)
				{
					toRemove = menu;
					break;
				}
			}
			Game1.onScreenMenus.Remove(toRemove);

			OverlayUI = new OverlayUI(numberOfPlayers);
			Game1.onScreenMenus.Add(OverlayUI);
		}

		public void ClientEndGame()
		{
			Console.WriteLine($"Received game end");
			IsGameInProgress = false;
			
			//Remove the overlay
			IClickableMenu toRemove = null;
			foreach (IClickableMenu menu in Game1.onScreenMenus)
			{
				if (menu is OverlayUI)
				{
					toRemove = menu;
					break;
				}
			}
			Game1.onScreenMenus.Remove(toRemove);
		}

		public void HandleDeath(Farmer whoDied, long? damagerID = null)
		{
			if (!IsGameInProgress)
				return;

            if (damagerID.HasValue)
            {
                foreach (Farmer killer in Game1.getOnlineFarmers())
                {
                    if (damagerID.Value == killer.UniqueMultiplayerID)
                    {
                        Console.WriteLine($"{whoDied.Name} has been killed by {killer.Name}");
                        //[124] is the sword e-mote icon
                        NetworkUtility.SendChatMessageToAllPlayers($"[peach]{killer.Name} [124] {whoDied.Name}");
                        break;
                    }
                }
            }
            else
            {
                NetworkUtility.SendChatMessageToAllPlayers($"[peach]{whoDied.Name} died");
                Console.WriteLine($"Received news that {whoDied.Name} was killed");
            }
			
			alivePlayers.Remove(whoDied.UniqueMultiplayerID);

			foreach (Farmer player in Game1.getAllFarmers())
				if (player != Game1.player && player != null)
					Game1.server?.sendMessage(player.UniqueMultiplayerID,
						new OutgoingMessage(NetworkUtility.uniqueMessageType, Game1.player, (int)NetworkUtility.MessageTypes.BROADCAST_ALIVE_COUNT, alivePlayers.Count));


			if (alivePlayers.Count == 1)
			{
				long winnerID = alivePlayers[0];
				foreach (Farmer player in Game1.getOnlineFarmers())
				{
					if (player != null && player.UniqueMultiplayerID == winnerID)
					{
						HandleWin(player, whoDied);
						return;
					}
				}
				HandleWin(null, whoDied);
			}
			else if (alivePlayers.Count == 0)
				HandleWin(null, whoDied);
			else
			{
				NetworkUtility.SendChatMessageToAllPlayers($"'{whoDied.Name}' has died! {alivePlayers.Count} players remain...");
			}
		}
		
		private void HandleWin(Farmer winner, Farmer whoDied)
		{
			if (whoDied == null && winner == null)
				NetworkUtility.SendChatMessageToAllPlayers($"Ending game to avoid eternal wait.");
			else if (winner == null)
				NetworkUtility.SendChatMessageToAllPlayers($"'{whoDied.Name}' died, but no one was present to claim the victory.");
			else
				NetworkUtility.SendChatMessageToAllPlayers($"[orange]#1 VALLEY ROYALE '{winner.Name.ToUpper()}'");

			IsGameInProgress = false;

			ClientEndGame();

			foreach (Farmer player in Game1.getOnlineFarmers())
				if (player != Game1.player && player != null)
					NetworkUtility.BroadcastGameEndToClient(player);

			int timeBetweenRound = modConfig.TimeInSecondsBetweenRounds;
			if (timeBetweenRound >= 0)
			{
				NetworkUtility.SendChatMessageToAllPlayers($"Starting new game in {timeBetweenRound} seconds...");

				waitingForNextRoundToStart = true;
				WhenToStartNextRound = DateTime.Now + new TimeSpan(hours: 0, minutes: 0, seconds: timeBetweenRound);
			}


			int hatID = 10;//Chicken hat
			if (winner != null && (winner.hat == null || winner.hat.Value == null || winner.hat.Value.ParentSheetIndex != hatID) && (winner.Items == null || !winner.Items.Any(x => x is StardewValley.Objects.Hat && x.ParentSheetIndex == hatID)))
			{
				if (Game1.player == winner)
					winner.addItemToInventory(new StardewValley.Objects.Hat(hatID));
				else
				{
					var message = new OutgoingMessage(NetworkUtility.uniqueMessageType, Game1.player, (int)NetworkUtility.MessageTypes.GIVE_HAT);
					Game1.server?.sendMessage(winner.UniqueMultiplayerID, message);
				}
				
			}
		}

		private DateTime knockbackImmunityTimeActivated = DateTime.MinValue;
		public void AddKnockbackImmunity()
		{
			knockbackImmunityTimeActivated = DateTime.Now;
		}

		private StardewValley.Monsters.Monster dummyMonster = null;
		public void TakeDamage(int damage, long? damagerID = null)
		{
			if (ModEntry.BRGame.IsGameInProgress)
			{
				dummyMonster = dummyMonster ?? new StardewValley.Monsters.Monster();

                try
                {
                    bool oldIsEating = Game1.player.isEating;
                    Game1.player.isEating = false;//Prevent invincibility
					
                    Game1.player.takeDamage(damage, false, dummyMonster);//If you pass in null as monster the player won't take damage

                    Game1.player.isEating = oldIsEating;
                }catch(Exception)
                {
                    Console.WriteLine("Ignoring exception in takeDamage");
                }
                
                Farmer damager = null;
				if (Game1.currentLocation != null && Game1.currentLocation.farmers != null)
				{
					foreach (Farmer player in Game1.currentLocation?.farmers)
					{
						//Hit shake timer / Invincibility frames
						if (player != Game1.player && player != null)
						{
							var objects = new object[] { (int)NetworkUtility.MessageTypes.TELL_PLAYER_HIT_SHAKE_TIMER, 1200 };//Hard coded in Farmer.MovePosition
							if (Game1.IsServer)
								Game1.server.sendMessage(player.UniqueMultiplayerID, new OutgoingMessage(NetworkUtility.uniqueMessageType, Game1.player, objects));
							else
								NetworkUtility.RelayMessageFromClientToAnotherPlayer(player.UniqueMultiplayerID, objects);

							if (player.UniqueMultiplayerID == damagerID)
							{
								damager = player;
							}
						}
					}
				}

                //Knockback (knock the player)
                if (damager != null && damage > 0 && (DateTime.Now - knockbackImmunityTimeActivated).TotalMilliseconds >= knockbackImmunityMilliseconds)
                {
                    double amount = 10 + 8 * (-1 + 2 / (1 + Math.Pow(Math.E, -0.03 * damage)));
					amount = Math.Min(18, amount);//Just in case
                    
                    Vector2 displacement = Vector2.Subtract(Game1.player.Position, damager.Position);
                    if (displacement.LengthSquared() != 0)
                    {
                        displacement.Normalize();

                        displacement.Y = displacement.Y * -1;
                        displacement = Vector2.Multiply(displacement, (float)amount);
                        
                        Console.WriteLine($"setting trajectory to {displacement.X},{displacement.Y}");

                        Game1.player.setTrajectory((int)displacement.X, (int)displacement.Y);
                    }
                }

                if (Game1.player.health <= 0)
				{
					//They are dead now
					Console.WriteLine($"I AM DEAD!");

					Random random = new Random();

					//Spawn their items onto the floor
					foreach (var item in Game1.player.Items)
					{
						if (item != null)
							if (!(item is Tool) || item is StardewValley.Tools.MeleeWeapon || item is StardewValley.Tools.Slingshot)
							{
								Debris debris = new Debris(item, Game1.player.getStandingPosition(), Game1.player.getStandingPosition() + new Microsoft.Xna.Framework.Vector2(64 * (float)(random.NextDouble() * 2 - 1), 64 * (float)(random.NextDouble() * 2 - 1)));
								Game1.currentLocation.debris.Add(debris);
							}
					}

					

					var oldLocation = Game1.player.currentLocation;
					var oldPosition = new xTile.Dimensions.Location(
								(int)Game1.player.Position.X - Game1.viewport.Width / 2,
								(int)Game1.player.Position.Y - Game1.viewport.Height / 2);

					Game1.player.clearBackpack();
					Game1.player.health = maxHealth;//Otherwise they will go the the Doctor on the next update tick
					//Game1.player.warpFarmer(new TileLocation("Forest", 100, 20).CreateWarp());//Marnie's cow pen

					SpectatorMode.EnterSpectatorMode(oldLocation, oldPosition);

					if (Game1.IsServer)
						ModEntry.BRGame.HandleDeath(Game1.player, damagerID);
					else
					{
                        if (damagerID.HasValue)
						    Game1.client.sendMessage(new OutgoingMessage(NetworkUtility.uniqueMessageType, Game1.player,
							    (int)NetworkUtility.MessageTypes.TELL_SERVER_I_DIED, Game1.player.UniqueMultiplayerID, damagerID.Value));
                        else
                            Game1.client.sendMessage(new OutgoingMessage(NetworkUtility.uniqueMessageType, Game1.player,
                                (int)NetworkUtility.MessageTypes.TELL_SERVER_I_DIED, Game1.player.UniqueMultiplayerID));
                    }

					
				}
			}
		}

		public void ProcessPlayerJoin(NetFarmerRoot farmerRoot)
		{
			Console.WriteLine($"(Game) Player joining...");

			Storm.SendPhasesData(farmerRoot.Value.UniqueMultiplayerID);

			//If you run immediately it does nothing
			Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.Sleep(1000);//TODO: reduce/increase maybe?
				NetworkUtility.WarpFarmer(farmerRoot.Value, new TileLocation("Forest", 100, 20));
			});

			if (IsGameInProgress && alivePlayers.Count <= 1)
			{
				//Restart the game, or nothing will ever happen
				HandleWin(null, null);
			}
		}

		public void Update(GameTime gameTime)
		{
			Game1.currentMinigame = null;
			Game1.player.Stamina = Game1.player.MaxStamina;

			if (Game1.currentLocation != null && Game1.currentLocation is StardewValley.Locations.Desert && Game1.player.CanMove)
			{
				if (Game1.player.position.X > 3020 && Game1.player.position.Y > 1350 && Game1.player.position.Y < 2100)
				{
					Console.WriteLine("Exit desert by road");
					Game1.warpFarmer("Backwoods", 25, 30, false);
				}
			}

			if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is StardewValley.Menus.ShopMenu)
			{
				Game1.activeClickableMenu = null;
			}

			if (waitingForNextRoundToStart && WhenToStartNextRound != null && Game1.IsServer && DateTime.Now >= WhenToStartNextRound)
			{
				waitingForNextRoundToStart = false;
				ServerStartGame();
			}
		}
	}
}
