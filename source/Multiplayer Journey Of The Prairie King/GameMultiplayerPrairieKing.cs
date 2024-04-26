/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/scayze/multiprairie
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MultiplayerPrairieKing;
using MultiplayerPrairieKing.Components;
using MultiplayerPrairieKing.Entities;
using MultiplayerPrairieKing.Entities.Enemies;
using MultiplayerPrairieKing.Utility;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using StardewValley.Minigames;
using StardewValley.SDKs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml.Serialization;
using static MultiplayerPrairieKing.Utility.Serialization;

namespace MultiPlayerPrairie
{
	[XmlInclude(typeof(AbigailGame.JOTPKProgress))]
	[InstanceStatics]
	public class GameMultiplayerPrairieKing : IMinigame
	{
		public delegate void behaviorAfterMotionPause();

		public void NETskipLevel(int toLevel = -1)
		{
			//Cant skip level as non host, i think
			if (!IsHost)
			{
				return;
			}

			foreach (Enemy monster in monsters)
			{
				PK_EnemyKilled enemyKilled = new()
				{
					id = monster.id
				};
				modInstance.SyncMessage(enemyKilled);
			}

			monsters.Clear();
			

			OnCompleteLevel(toLevel);
			PK_CompleteLevel completeLevel = new()
			{
				toLevel = toLevel
			};
			modInstance.SyncMessage(completeLevel);
		}

		public void NETspawnBullet(bool friendly, Point position, Point motion, int damage)
		{
			Bullet bullet = new(this, friendly, true, position, motion, damage);
			bullets.Add(bullet);

			//NET Spawn Bullet
			PK_BulletSpawn mBullet = new()
			{
				id = bullet.id,
				position = position,
				motion = motion,
				damage = damage,
				isFriendly = friendly
			};
			modInstance.SyncMessage(mBullet);
		}

		public void NETspawnBullet(bool friendly, Point position, int direction, int damage)
		{
			Bullet bullet = new(this, friendly, true, position, direction, damage);
			bullets.Add(bullet);

			//NET Spawn Bullet
			PK_BulletSpawn mBullet = new()
			{
				id = bullet.id,
				position = position,
				motion = bullet.motion,
				damage = damage,
				isFriendly = friendly
			};
			modInstance.SyncMessage(mBullet);
		}

		public void NETmovePlayer(Vector2 pos)
		{
			PK_PlayerMove message = new()
			{
				playerId = modInstance.playerID.Value,
				position = pos,
				id = Game1.player.UniqueMultiplayerID,
				shootingDirections = player.shootingDirections,
				movementDirections = player.movementDirections
			};
			modInstance.SyncMessage(message);
		}

		public void NETspawnPowerup(POWERUP_TYPE type, Point position, int duration = 7500)
		{
			Powerup powerup = new(this,type,position,duration);
			powerups.Add(powerup);

			PK_PowerupSpawn mPowerup = new()
			{
				id = powerup.id,
				position = position,
				which = (int)type,
				duration = duration
			};
			modInstance.SyncMessage(mPowerup);

			modInstance.Monitor.Log("spawning " + mPowerup.which.ToString() + " with id " + mPowerup.id, LogLevel.Debug);
		}

		public ModMultiPlayerPrairieKing modInstance;

		public UI ui;

        public Cutscene Cutscene { get; set; }

        //Save state that was transmitted by the host to start this match. Seperate from the own savestate that is saved in the mod instance
        public SaveState multiplayerSaveState;

        public bool IsHost { get; set; }

        public const int mapWidth = 16;

		public const int mapHeight = 16;

		public const int pixelZoom = 3;

		public const int baseTileSize = 16;

		public const int playerMotionDelay = 100;

		public const int deathDelay = 3000;

		public static Texture2D shopBubbleTexture;

		public DIFFICULTY difficulty = DIFFICULTY.NORMAL;

		public Dictionary<long, BasePlayer> playerList = new();

		public Player player;

		public int newGamePlus;

		public const int waveDuration = 80000;

		public const int betweenWaveDuration = 5000;

		public List<Enemy> monsters = new();

		public Rectangle merchantBox;

		public Rectangle noPickUpBox;

		public int motionPause;

		public int lives = 3;

		public int Coins { get; set; }

		int score;

		public List<Bullet> bullets = new();

		public Map map;

		Map nextMap;

		List<SpawnTask>[] spawnQueue = new List<SpawnTask>[4];

		public static Vector2 topLeftScreenCoordinate;

		public float cactusDanceTimer;

		public behaviorAfterMotionPause behaviorAfterPause;

		List<Vector2> monsterChances = new()
		{
			new Vector2(0.014f, 0.4f),
			Vector2.Zero,
			Vector2.Zero,
			Vector2.Zero,
			Vector2.Zero,
			Vector2.Zero,
			Vector2.Zero
		};

		public Rectangle shoppingCarpetNoPickup;

		public Dictionary<POWERUP_TYPE, int> activePowerups = new();

		public List<Powerup> powerups = new();

		public List<TemporaryAnimatedSprite> temporarySprites = new();


		public MAP_TYPE world = MAP_TYPE.desert;

		public int gamerestartTimer;

		public int waveTimer = 80000;

		public int betweenWaveTimer = 5000;

		public int currentLevel;

		public int monsterConfusionTimer;

		public int zombieModeTimer;

		int shoppingTimer;

		public int newMapPosition;

		public int screenFlash;

		int gopherTrainPosition;

		bool shopping;

		public bool gopherRunning;

		bool merchantArriving;

		bool merchantShopOpen;

		bool waitingForPlayerToMoveDownAMap;

		public bool scrollingMap;

		bool hasGopherAppeared;

		public bool shootoutLevel;

		public bool gopherTrain;

		bool playerJumped;

		public bool endCutscene;

		public bool gameOver;

		readonly Dictionary<Rectangle, ITEM_TYPE> storeItems = new();

		public bool quit;

		public bool died;

		public Rectangle gopherBox;

		Point gopherMotion;

		//Music
		public static ICue overworldSong;

		public static ICue outlawSong;

		public static ICue zombieSong;

		//Input States
		readonly HashSet<GameKeys> _buttonHeldState = new();

		readonly Dictionary<GameKeys, int> _buttonHeldFrames = new();

		public static int TileSize => 48;

        public bool IsEnterButtonAssignmentFlipped;

		public GameMultiplayerPrairieKing(ModMultiPlayerPrairieKing mod, bool isHost)
		{
			ui = new(this);
			Cutscene = new(this);

			for (int k = 0; k < 11; k++)
			{
				_buttonHeldFrames[(GameKeys)k] = 0;
			}

			this.modInstance = mod;
			this.IsHost = isHost;
			Reset(gameLooped: false, gameRestarted: false);

			//Access SDKHelper by reflection. Hacky but otherwise controller controls are fucked
			Type type = typeof(Program);
			FieldInfo info = type.GetField("_sdk", BindingFlags.NonPublic | BindingFlags.Static);
			SDKHelper sdkHelper = (SDKHelper)info.GetValue(null);
			IsEnterButtonAssignmentFlipped = sdkHelper.IsEnterButtonAssignmentFlipped;
		}

		public bool LoadGame()
		{
			SaveState saveState;

            if (IsHost) saveState = modInstance.GetSaveState();
			else saveState = multiplayerSaveState;
			

            if (saveState == null)
            {
                modInstance.Monitor.Log("Couldnt Load PrairieKing saveState", LogLevel.Debug);
                return false;
            }

            foreach (PlayerSaveState playerSaveState in saveState.playerSaveStates)
			{
				modInstance.Monitor.Log("Loading player: " + playerSaveState.PlayerID, LogLevel.Debug);
				BasePlayer referencedPlayer = playerList[playerSaveState.PlayerID];
				
                referencedPlayer.ammoLevel = playerSaveState.AmmoLevel;
                referencedPlayer.runSpeedLevel = playerSaveState.RunSpeedLevel;
                referencedPlayer.fireSpeedLevel = playerSaveState.FireSpeedLevel;
                referencedPlayer.bulletDamage = playerSaveState.BulletDamage;
                referencedPlayer.spreadPistol = playerSaveState.SpreadPistol;

                if (playerSaveState.HeldItem != -100)
                {
                    referencedPlayer.heldItem = new Powerup(this, (POWERUP_TYPE)playerSaveState.HeldItem, Point.Zero, 9999);
                }
            }

			Coins = saveState.Coins;
			died = saveState.Died;
			lives = saveState.Lives;
			score = saveState.Score;
			newGamePlus = saveState.WhichRound;
			currentLevel = saveState.WhichWave;
			waveTimer = saveState.WaveTimer;
			world = (MAP_TYPE)saveState.World;
			
			monsterChances = ConvertFromSVector2(saveState.MonsterChances);
			ApplyLevelSpecificStates();
			if (shootoutLevel)
			{
				player.position = new Vector2(8 * TileSize, 3 * TileSize);
			}
            modInstance.Monitor.Log("Loaded gamestate", LogLevel.Info);
            return true;
		}

		public void SaveGame()
		{
			SaveState saveState = new();
			saveState.playerSaveStates = new();

			foreach(long playerID in playerList.Keys)
			{
				BasePlayer bp = playerList[playerID];

				PlayerSaveState playerSaveState = new()
				{
					PlayerID = playerID,
					PlayerName = bp.playerName,
                    AmmoLevel = bp.ammoLevel,
                    RunSpeedLevel = bp.runSpeedLevel,
                    FireSpeedLevel = bp.fireSpeedLevel,
                    BulletDamage = bp.bulletDamage,
                    SpreadPistol = bp.spreadPistol
                };

                if (bp.heldItem == null)
                {
                    playerSaveState.HeldItem = -100;
                }
                else
                {
                    playerSaveState.HeldItem = (int)bp.heldItem.type;
                }

                modInstance.Monitor.Log("Saving player: " + playerSaveState.PlayerID, LogLevel.Debug);
                saveState.playerSaveStates.Add(playerSaveState);
            }

            saveState.Coins = Coins;
            saveState.Died = died;
            saveState.Lives = lives;
            saveState.Score = score;
            saveState.WhichRound = newGamePlus;
            saveState.WhichWave = currentLevel;
            saveState.WaveTimer = waveTimer;
            saveState.World = (int)world;
			saveState.MonsterChances = ConvertToSVector2(monsterChances);

			modInstance.UpdateSaveState(saveState);

            
        }

		public void InstantiatePlayers()
		{
			//Let players start in a square
			List<Vector2> startPositions = new()
			{
				new Vector2(352f, 352f) + new Vector2(0,0),
				new Vector2(352f, 352f) + new Vector2(64,0),
				new Vector2(352f, 352f) + new Vector2(0,64),
				new Vector2(352f, 352f) + new Vector2(64,64),
            };

			playerList.Clear();
			for(int i=0; i<modInstance.playerList.Value.Count; i++)
			{
				long pid = modInstance.playerList.Value[i];

				BasePlayer p;

				if (pid == modInstance.playerID.Value)
				{
					player = new Player(this);
					p = player;
				}
				else
				{
					p = new PlayerSlave(this);
				}
				p.textureBase = new Vector2(i * 64, 0);
				p.position = startPositions[i];
                p.playerName = Game1.getFarmer(pid).Name;


                modInstance.Monitor.Log("playerName set to: " + p.playerName, LogLevel.Info);
				playerList.Add(pid, p);
			}

			//NET Player Move
			NETmovePlayer(player.position);

			player.boundingBox.X = (int)player.position.X + TileSize / 4;
			player.boundingBox.Y = (int)player.position.Y + TileSize / 4;
			player.boundingBox.Width = TileSize / 2;
			player.boundingBox.Height = TileSize / 2;

            if (LoadGame())
            {
                map = MapLoader.GetMap(currentLevel);
            }
        }

		public void Reset(bool gameLooped, bool gameRestarted)
		{
			//Reset false only if its an full reset and not loop
			if(!gameLooped) died = false;

			if(gameLooped)
			{
				//Actual apply new game plus
				monsterChances[0] = new Vector2(0.014f + newGamePlus * 0.005f, 0.41f + newGamePlus * 0.05f);
				monsterChances[4] = new Vector2(0.002f, 0.1f);
			}

            //Only show the start menu when its a fresh new game
            ui.onStartMenu = false;
            if (!gameLooped && !gameRestarted)
			{
				ui.onStartMenu = true;
			}

			//Updates the minigame viewport / topLeftScreenCoordinate
			changeScreenSize();

			merchantArriving = false;
			merchantShopOpen = false;
			noPickUpBox = new Rectangle(0, 0, TileSize, TileSize);
			merchantBox = new Rectangle(8 * TileSize, 0, TileSize, TileSize);
			shopping = false;

			zombieModeTimer = 0;
			monsterConfusionTimer = 0;
			monsters.Clear();

			world = 0;
			currentLevel = 0;
			map = MapLoader.GetDefaultMap();
			newMapPosition = 16 * TileSize;
			scrollingMap = false;
			
			temporarySprites.Clear();
			waitingForPlayerToMoveDownAMap = false;
			waveTimer = 80000;
			
			shootoutLevel = false;
			betweenWaveTimer = 2500;
			gopherRunning = false;
			hasGopherAppeared = false;

			endCutscene = false;
			Cutscene.Reset();

			gameOver = false;
			
			powerups.Clear();

            overworldSong?.Stop(AudioStopOptions.Immediate);
            outlawSong?.Stop(AudioStopOptions.Immediate);
            zombieSong?.Stop(AudioStopOptions.Immediate);

            outlawSong = null;
			overworldSong = null;
			
			Game1.changeMusicTrack("none", track_interruptable: false, MusicContext.MiniGame);

			for (int j = 0; j < 4; j++)
			{
				spawnQueue[j] = new List<SpawnTask>();
			}
		}

		public bool overrideFreeMouseMovement()
		{
			return Game1.options.SnappyMenus;
		}

		public void StartGopherTrain(ITEM_TYPE item = ITEM_TYPE.NONE)
		{
			foreach (BasePlayer p in playerList.Values)
			{
				p.HoldItem(item, 2000);
			}

			Game1.playSound("Cowboy_Secret");
			gopherTrain = true;
			gopherTrainPosition = -TileSize * 2;
		}

		public void AddGuts(Point position, MONSTER_TYPE whichGuts)
		{
			switch (whichGuts)
			{
				case MONSTER_TYPE.orc:
				case MONSTER_TYPE.ogre:
				case MONSTER_TYPE.mushroom:
				case MONSTER_TYPE.spikey:
				case MONSTER_TYPE.dracula:
					temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(512, 1696, 16, 16), 80f, 6, 0, topLeftScreenCoordinate + new Vector2(position.X, position.Y), flicker: false, Game1.random.NextDouble() < 0.5, 0.001f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true));
					temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(592, 1696, 16, 16), 10000f, 1, 0, topLeftScreenCoordinate + new Vector2(position.X, position.Y), flicker: false, Game1.random.NextDouble() < 0.5, 0.001f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
					{
						delayBeforeAnimationStart = 480
					});
					break;
				case MONSTER_TYPE.mummy:
					temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 80f, 5, 0, topLeftScreenCoordinate + new Vector2(position.X, position.Y), flicker: false, Game1.random.NextDouble() < 0.5, 0.001f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true));
					break;
				case MONSTER_TYPE.ghost:
				case MONSTER_TYPE.devil:
					temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(544, 1728, 16, 16), 80f, 4, 0, topLeftScreenCoordinate + new Vector2(position.X, position.Y), flicker: false, Game1.random.NextDouble() < 0.5, 0.001f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true));
					break;
			}
		}

		public void EndOfGopherAnimationBehavior2(int extraInfo)
		{
			Game1.playSound("cowboy_gopher");
			if (Math.Abs(gopherBox.X - 8 * TileSize) > Math.Abs(gopherBox.Y - 8 * TileSize))
			{
				if (gopherBox.X > 8 * TileSize)
				{
					gopherMotion = new Point(-2, 0);
				}
				else
				{
					gopherMotion = new Point(2, 0);
				}
			}
			else if (gopherBox.Y > 8 * TileSize)
			{
				gopherMotion = new Point(0, -2);
			}
			else
			{
				gopherMotion = new Point(0, 2);
			}
			gopherRunning = true;
		}

		public void EndOfGopherAnimationBehavior(int extrainfo)
		{
			temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(384, 1792, 16, 16), 120f, 4, 2, topLeftScreenCoordinate + new Vector2(gopherBox.X + TileSize / 2, gopherBox.Y + TileSize / 2), flicker: false, flipped: false, (float)gopherBox.Y / 10000f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
			{
				endFunction = EndOfGopherAnimationBehavior2
			});
			Game1.playSound("cowboy_gopher");
		}

		public void UpdateBullets(GameTime time)
		{
			for (int m = bullets.Count - 1; m >= 0; m--)
			{
				bullets[m].Update();
				if(bullets[m].queuedForDeletion)
				{
					//NET Despawn Bullet
					PK_BulletDespawned mBulletDespawned = new()
					{
						id = bullets[m].id
					};	
					modInstance.SyncMessage(mBulletDespawned);
					bullets.RemoveAt(m);
				}
			}
		}

		public void PlayerDie()
		{
			gopherRunning = false;
			hasGopherAppeared = false;
			spawnQueue = new List<SpawnTask>[4];
			for (int i = 0; i < 4; i++)
			{
				spawnQueue[i] = new List<SpawnTask>();
			}

			//enemyBullets.Clear(); Still needed? Might even look cool, and not scary because of invincibility timer

			if (!shootoutLevel)
			{
				powerups.Clear();
				monsters.Clear();
			}
			died = true;
			activePowerups.Clear();

			if (overworldSong != null && overworldSong.IsPlaying)
			{
				overworldSong.Stop(AudioStopOptions.Immediate);
			}
			temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1808, 16, 16), 120f, 5, 0, player.position + topLeftScreenCoordinate, flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true));
			waveTimer = Math.Min(80000, waveTimer + 10000);
			betweenWaveTimer = 4000;
			lives--;

			foreach(BasePlayer p in playerList.Values)
			{
				p.Die();
			}

			if (lives < 0)
			{
				temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1808, 16, 16), 550f, 5, 0, player.position + topLeftScreenCoordinate, flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
				{
					alpha = 0.001f,
					endFunction = AfterPlayerDeathFunction
				});

				foreach (BasePlayer p in playerList.Values)
				{
					p.deathTimer *= 3f;
				}

				modInstance.UpdateSaveState(null);
				
			}
			else if (!shootoutLevel)
			{
				SaveGame();
			}
		}

		public void AfterPlayerDeathFunction(int extra)
		{
			if (lives < 0)
			{
				gameOver = true;
				overworldSong?.Stop(AudioStopOptions.Immediate);
                outlawSong?.Stop(AudioStopOptions.Immediate);
                zombieSong?.Stop(AudioStopOptions.Immediate);
                monsters.Clear();
				powerups.Clear();
				died = false;
				Game1.playSound("Cowboy_monsterDie");
			}
		}

		public void StartNewRound()
		{
			gamerestartTimer = 2000;
			Game1.playSound("Cowboy_monsterDie");
			newGamePlus++;
		}

		public void StartLevelTransition()
		{
			SaveGame();
			shopping = false;
			merchantArriving = false;
			merchantShopOpen = false;
			merchantBox.Y = -TileSize;
			scrollingMap = true;
			nextMap = MapLoader.GetMap(currentLevel);
			newMapPosition = 16 * TileSize;
			temporarySprites.Clear();
			powerups.Clear();
		}

		protected void UpdateInput()
		{
			if (Game1.options.gamepadControls)
			{
				GamePadState pad_state = Game1.input.GetGamePadState();
				ButtonCollection button_collection = new(ref pad_state);

				if (pad_state.ThumbSticks.Left.X < -0.2) _buttonHeldState.Add(GameKeys.MoveLeft);
				if (pad_state.ThumbSticks.Left.X > 0.2) _buttonHeldState.Add(GameKeys.MoveRight);
				if (pad_state.ThumbSticks.Left.Y < -0.2) _buttonHeldState.Add(GameKeys.MoveDown);
				if (pad_state.ThumbSticks.Left.Y > 0.2) _buttonHeldState.Add(GameKeys.MoveUp);
				if (pad_state.ThumbSticks.Right.X < -0.2) _buttonHeldState.Add(GameKeys.ShootLeft);
				if (pad_state.ThumbSticks.Right.X > 0.2) _buttonHeldState.Add(GameKeys.ShootRight);
				if (pad_state.ThumbSticks.Right.Y < -0.2) _buttonHeldState.Add(GameKeys.ShootDown);
				if (pad_state.ThumbSticks.Right.Y > 0.2) _buttonHeldState.Add(GameKeys.ShootUp);

				ButtonCollection.ButtonEnumerator enumerator = button_collection.GetEnumerator();
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
						case Buttons.A:
							if (gameOver) _buttonHeldState.Add(GameKeys.SelectOption); 
							else if (IsEnterButtonAssignmentFlipped) _buttonHeldState.Add(GameKeys.ShootRight);
							else _buttonHeldState.Add(GameKeys.ShootDown);
							break;
						case Buttons.Y:
							_buttonHeldState.Add(GameKeys.ShootUp);
							break;
						case Buttons.X:
							_buttonHeldState.Add(GameKeys.ShootLeft);
							break;
						case Buttons.B:
							if (gameOver) _buttonHeldState.Add(GameKeys.Exit);
							else if (IsEnterButtonAssignmentFlipped) _buttonHeldState.Add(GameKeys.ShootDown);
							else _buttonHeldState.Add(GameKeys.ShootRight);
							break;
						case Buttons.DPadUp:
							_buttonHeldState.Add(GameKeys.MoveUp);
							break;
						case Buttons.DPadDown:
							_buttonHeldState.Add(GameKeys.MoveDown);
							break;
						case Buttons.DPadLeft:
							_buttonHeldState.Add(GameKeys.MoveLeft);
							break;
						case Buttons.DPadRight:
							_buttonHeldState.Add(GameKeys.MoveRight);
							break;
						case Buttons.Start:
						case Buttons.LeftShoulder:
						case Buttons.RightShoulder:
						case Buttons.RightTrigger:
						case Buttons.LeftTrigger:
							_buttonHeldState.Add(GameKeys.UsePowerup);
							break;
						case Buttons.Back:
							_buttonHeldState.Add(GameKeys.Exit);
							break;
					}
				}
			}
			if (Game1.input.GetKeyboardState().IsKeyDown(Keys.W)) _buttonHeldState.Add(GameKeys.MoveUp);
			if (Game1.input.GetKeyboardState().IsKeyDown(Keys.S)) _buttonHeldState.Add(GameKeys.MoveDown);
			if (Game1.input.GetKeyboardState().IsKeyDown(Keys.A)) _buttonHeldState.Add(GameKeys.MoveLeft);
			if (Game1.input.GetKeyboardState().IsKeyDown(Keys.D)) _buttonHeldState.Add(GameKeys.MoveRight);
			if (Game1.input.GetKeyboardState().IsKeyDown(Keys.Up))
			{
				if (gameOver) _buttonHeldState.Add(GameKeys.MoveUp);
				else _buttonHeldState.Add(GameKeys.ShootUp);
			}
			if (Game1.input.GetKeyboardState().IsKeyDown(Keys.Down))
			{
				if (gameOver) _buttonHeldState.Add(GameKeys.MoveDown);
				else _buttonHeldState.Add(GameKeys.ShootDown);
			}
			if (Game1.input.GetKeyboardState().IsKeyDown(Keys.Left)) _buttonHeldState.Add(GameKeys.ShootLeft);
			if (Game1.input.GetKeyboardState().IsKeyDown(Keys.Right)) _buttonHeldState.Add(GameKeys.ShootRight);
			if (Game1.input.GetKeyboardState().IsKeyDown(Keys.X) || Game1.input.GetKeyboardState().IsKeyDown(Keys.Enter) || Game1.input.GetKeyboardState().IsKeyDown(Keys.Space))
			{
				if (gameOver) _buttonHeldState.Add(GameKeys.SelectOption);
				else _buttonHeldState.Add(GameKeys.UsePowerup);
			}
			if (Game1.input.GetKeyboardState().IsKeyDown(Keys.Escape)) _buttonHeldState.Add(GameKeys.Exit);
		}

		public bool tick(GameTime time)
		{
			//Pause the game for everyone if ALL players are ingame
			if (modInstance.playerList.Value.Count == Game1.numberOfPlayers())
			{
				Game1.gameTimeInterval = 0;
			}
			
			//Update and process inputs
			_buttonHeldState.Clear();
			UpdateInput();

			for (int l = 0; l < 11; l++)
			{
				if (_buttonHeldState.Contains((GameKeys)l))
				{
					_buttonHeldFrames[(GameKeys)l]++;
				}
				else
				{
					_buttonHeldFrames[(GameKeys)l] = 0;
				}
			}

			ProcessInputs();

			// Dont process the game anymore if on menu on quitting
			if (quit)
			{
				Game1.stopMusicTrack(MusicContext.MiniGame);
				return true;
			}

			if (gameOver) return false;
			if (ui.onStartMenu) return false;

			if (gamerestartTimer > 0)
			{
				gamerestartTimer -= time.ElapsedGameTime.Milliseconds;
				if (gamerestartTimer <= 0)
				{
					if (newGamePlus == 0 || !endCutscene)
					{
						Reset(gameLooped: false, gameRestarted: true);
					}
					else
					{
						Reset(gameLooped: true, gameRestarted: false);
					}
				}
			}

			//Process cutscene
			if (endCutscene)
			{
				Cutscene.Tick(time);
				return false;
			}
			if (motionPause > 0)
			{
				motionPause -= time.ElapsedGameTime.Milliseconds;
				if (motionPause <= 0 && behaviorAfterPause != null)
				{
					behaviorAfterPause();
					behaviorAfterPause = null;
				}
			}
			else if (monsterConfusionTimer > 0)
			{
				monsterConfusionTimer -= time.ElapsedGameTime.Milliseconds;
			}
			if (zombieModeTimer > 0)
			{
				zombieModeTimer -= time.ElapsedGameTime.Milliseconds;
			}

			foreach(BasePlayer p in playerList.Values)
			{
				p.Tick(time);
			}

			//Dont stop if the player is still holding up the item?
			if (player.IsHoldingItem()) return false;

			
			//Screen flash timer
			if (screenFlash > 0)
			{
				screenFlash -= time.ElapsedGameTime.Milliseconds;
			}

			//Weird gopher train shit
			if (gopherTrain)
			{
				gopherTrainPosition += 3;
				if (gopherTrainPosition % 30 == 0)
				{
					Game1.playSound("Cowboy_Footstep");
				}
				if (playerJumped)
				{
					player.position.Y += 3f;
				}
				if (Math.Abs(player.position.Y - (float)(gopherTrainPosition - TileSize)) <= 16f)
				{
					playerJumped = true;
					player.position.Y = gopherTrainPosition - TileSize;
				}
				if (gopherTrainPosition > 16 * TileSize + TileSize)
				{
					gopherTrain = false;
					playerJumped = false;
					currentLevel++;
					map = MapLoader.GetMap(currentLevel);
					player.position = new Vector2(8 * TileSize, 8 * TileSize);

					//NET Player Move
					NETmovePlayer(player.position);

					world = ((world != MAP_TYPE.desert) ? MAP_TYPE.graveyard : MAP_TYPE.woods);
					waveTimer = 80000;
					betweenWaveTimer = 5000;
					waitingForPlayerToMoveDownAMap = false;
					shootoutLevel = false;
					SaveGame();
				}
			}

			// Shopping lady moving to her place
			if ((shopping || merchantArriving || waitingForPlayerToMoveDownAMap) && !player.IsHoldingItem())
			{
				int oldTimer = shoppingTimer;
				shoppingTimer += time.ElapsedGameTime.Milliseconds;
				shoppingTimer %= 500;
				if (!merchantShopOpen && merchantArriving && shopping && ((oldTimer < 250 && shoppingTimer >= 250) || oldTimer > shoppingTimer))
				{
					Game1.playSound("Cowboy_Footstep");
				}
			}

			//Move palyers along when moving down to the next map
			if (scrollingMap)
			{
				newMapPosition -= TileSize / 8;

				foreach (BasePlayer p in playerList.Values)
				{
					p.position.Y -= (float)TileSize / 8;
					p.position.Y += 3f;
					p.boundingBox.X = (int)player.position.X + TileSize / 4;
					p.boundingBox.Y = (int)player.position.Y + TileSize / 4;
					p.boundingBox.Width = TileSize / 2;
					p.boundingBox.Height = TileSize / 2;
					p.movementDirections = new List<int> { 2 };
					p.motionAnimationTimer += time.ElapsedGameTime.Milliseconds;
					p.motionAnimationTimer %= 400;
				}

				//Swap to the next map once the map is loaded
				if (newMapPosition <= 0)
				{
					scrollingMap = false;
					map = nextMap;
					newMapPosition = 16 * TileSize;
					shopping = false;
					betweenWaveTimer = 5000;
					waitingForPlayerToMoveDownAMap = false;
					player.movementDirections.Clear();
					ApplyLevelSpecificStates();
				}
			}
			if (gopherRunning)
			{
				gopherBox.X += gopherMotion.X;
				gopherBox.Y += gopherMotion.Y;
				for (int m = monsters.Count - 1; m >= 0; m--)
				{
					if (gopherBox.Intersects(monsters[m].position))
					{
						//Net EnemyKilled
						PK_EnemyKilled message = new()
						{
							id = monsters[m].id
						};
						modInstance.SyncMessage(message);

						AddGuts(monsters[m].position.Location, monsters[m].type);
						monsters.RemoveAt(m);
						Game1.playSound("Cowboy_monsterDie");
					}
				}
				if (gopherBox.X < 0 || gopherBox.Y < 0 || gopherBox.X > 16 * TileSize || gopherBox.Y > 16 * TileSize)
				{
					gopherRunning = false;
				}
			}

			//Remove temporary sprites whos time have run out (?)
			for (int n = temporarySprites.Count - 1; n >= 0; n--)
			{
				if (temporarySprites[n].update(time))
				{
					temporarySprites.RemoveAt(n);
				}
			}

			//Move powerups again?
			if (motionPause <= 0)
			{
				for (int i = powerups.Count - 1; i >= 0; i--)
				{
					float distance = Utility.distance(player.boundingBox.Center.X, powerups[i].position.X + TileSize / 2, player.boundingBox.Center.Y, powerups[i].position.Y + TileSize / 2);

					if ( distance <= (float)(TileSize + 3) && (powerups[i].position.X < TileSize || powerups[i].position.X >= 16 * TileSize - TileSize || powerups[i].position.Y < TileSize || powerups[i].position.Y >= 16 * TileSize - TileSize))
					{
						if (powerups[i].position.X + TileSize / 2 < player.boundingBox.Center.X)
						{
							powerups[i].position.X++;
						}
						if (powerups[i].position.X + TileSize / 2 > player.boundingBox.Center.X)
						{
							powerups[i].position.X--;
						}
						if (powerups[i].position.Y + TileSize / 2 < player.boundingBox.Center.Y)
						{
							powerups[i].position.Y++;
						}
						if (powerups[i].position.Y + TileSize / 2 > player.boundingBox.Center.Y)
						{
							powerups[i].position.Y--;
						}
					}

					//Remove powerups that are queed for deletion
					if (powerups[i].queuedForDeletion)
					{
						powerups.RemoveAt(i);
					}
				}
				for (int i = activePowerups.Count - 1; i >= 0; i--)
				{
					activePowerups[activePowerups.ElementAt(i).Key] -= time.ElapsedGameTime.Milliseconds;
					if (activePowerups[activePowerups.ElementAt(i).Key] <= 0)
					{
						activePowerups.Remove(activePowerups.ElementAt(i).Key);
					}
				}

				//As Host, move down to the level if the level is finished and both players are at the bottom of the screen
				if (waitingForPlayerToMoveDownAMap && IsHost)
				{
					float bottomBorder = 16 * TileSize - TileSize / 4;
					bool readyToRumble = true;
					foreach(BasePlayer p in playerList.Values)
					{
						if (p.boundingBox.Bottom < bottomBorder)
						{
							readyToRumble = false;
						}
					}

					if(readyToRumble)
					{
						PK_StartLevelTransition message = new();
						modInstance.SyncMessage(message); 
						StartLevelTransition();
					}
				}
				if (shopping)
				{
					if (merchantBox.Y < 8 * TileSize - TileSize * 3 && merchantArriving)
					{
						merchantBox.Y += 2;
						if (merchantBox.Y >= 8 * TileSize - TileSize * 3)
						{
							merchantShopOpen = true;
							merchantArriving = false;
							Game1.playSound("cowboy_monsterhit");
							map[7, 15] = MAP_TILE.SAND;
							map[8, 15] = MAP_TILE.SAND;
							map[9, 15] = MAP_TILE.SAND;
							map[7, 14] = MAP_TILE.SAND;
							map[8, 14] = MAP_TILE.SAND;
							map[9, 14] = MAP_TILE.SAND;
							shoppingCarpetNoPickup = new Rectangle(merchantBox.X - TileSize, merchantBox.Y + TileSize, TileSize * 3, TileSize * 2);
						}
					}
					else if (merchantShopOpen)
					{
						for (int i = storeItems.Count - 1; i >= 0; i--)
						{
							if (!player.boundingBox.Intersects(shoppingCarpetNoPickup) && player.boundingBox.Intersects(storeItems.ElementAt(i).Key) && Coins >= GetPriceForItem(storeItems.ElementAt(i).Value))
							{
								Game1.playSound("Cowboy_Secret");
								motionPause = 2500;
								ITEM_TYPE boughtItem = storeItems.ElementAt(i).Value;
								Coins -= GetPriceForItem(boughtItem);

								player.HoldItem(boughtItem, 2500);

								merchantArriving = false;
								merchantShopOpen = false;

								PK_BuyItem message = new()
								{
									playerId = modInstance.playerID.Value,
									type = (int)boughtItem
								};
								modInstance.SyncMessage(message);

								BuyItem(player, boughtItem);
                            }
						}
					}
				}
				cactusDanceTimer += time.ElapsedGameTime.Milliseconds;
				cactusDanceTimer %= 1600f;

				UpdateBullets(time);

				//Update powerups
				foreach (Powerup powerup in powerups)
				{
					powerup.Tick(time);
				}

				if (waveTimer > 0 && betweenWaveTimer <= 0 && zombieModeTimer <= 0 && !shootoutLevel && !gameOver && gamerestartTimer <= 0 && (overworldSong == null || !overworldSong.IsPlaying))
				{
                    Game1.playSound("Cowboy_OVERWORLD", out overworldSong);
                    Game1.musicPlayerVolume = Game1.options.musicVolumeLevel;
					Game1.musicCategory.SetVolume(Game1.musicPlayerVolume);
				}
				if (player.deathTimer > 0f)
				{
					player.deathTimer -= time.ElapsedGameTime.Milliseconds;
				}
				if (betweenWaveTimer > 0 && monsters.Count == 0 && IsSpawnQueueEmpty() && !shopping && !waitingForPlayerToMoveDownAMap)
				{
					betweenWaveTimer -= time.ElapsedGameTime.Milliseconds;
				}
				else if (player.deathTimer <= 0f && !waitingForPlayerToMoveDownAMap && !shopping && !shootoutLevel)
				{
					if (waveTimer > 0)
					{
						waveTimer -= time.ElapsedGameTime.Milliseconds;

						int u = 0;
						foreach (Vector2 v in monsterChances)
						{
							if (Game1.random.NextDouble() < (v.X * ((monsters.Count != 0) ? 1 : 2)))
							{
								int numMonsters = 1;
								while (Game1.random.NextDouble() < v.Y && numMonsters < 15)
								{
									numMonsters++;
								}
								spawnQueue[(currentLevel == 11) ? (Game1.random.Next(1, 3) * 2 - 1) : Game1.random.Next(4)].Add(new SpawnTask((MONSTER_TYPE)u, numMonsters));
							}
							u++;
						}
						if (!hasGopherAppeared && monsters.Count > 6 && Game1.random.NextDouble() < 0.0004 && waveTimer > 7000 && waveTimer < 50000)
						{
							hasGopherAppeared = true;
							gopherBox = new Rectangle(Game1.random.Next(16 * TileSize), Game1.random.Next(16 * TileSize), TileSize, TileSize);
							int tries2 = 0;
							while ((map.IsCollidingWithMap(gopherBox) || map.IsCollidingWithMonster(gopherBox, this) || Math.Abs(gopherBox.X - player.position.X) < (TileSize * 6) || Math.Abs(gopherBox.Y - player.position.Y) < (float)(TileSize * 6) || Math.Abs(gopherBox.X - 8 * TileSize) < TileSize * 4 || Math.Abs(gopherBox.Y - 8 * TileSize) < TileSize * 4) && tries2 < 10)
							{
								gopherBox.X = Game1.random.Next(16 * TileSize);
								gopherBox.Y = Game1.random.Next(16 * TileSize);
								tries2++;
							}
							if (tries2 < 10)
							{
								temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(256, 1664, 16, 32), 80f, 5, 0, topLeftScreenCoordinate + new Vector2(gopherBox.X + TileSize / 2, gopherBox.Y - TileSize + TileSize / 2), flicker: false, flipped: false, (float)gopherBox.Y / 10000f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
								{
									endFunction = EndOfGopherAnimationBehavior
								});
							}
						}
					}
					for (int p = 0; p < 4; p++)
					{
						if (spawnQueue[p].Count <= 0)
						{
							continue;
						}
						if (spawnQueue[p][0].type == MONSTER_TYPE.ghost || spawnQueue[p][0].type == MONSTER_TYPE.devil)
						{
							List<Vector2> border = Utility.getBorderOfThisRectangle(new Rectangle(0, 0, 16, 16));
							Vector2 tile = border[Game1.random.Next(border.Count)];
							int tries = 0;
							while (map.IsCollidingWithMonster(new Rectangle((int)tile.X * TileSize, (int)tile.Y * TileSize, TileSize, TileSize), this) && tries < 10)
							{
								tile = border[Game1.random.Next(border.Count)];
								tries++;
							}
							if (tries < 10)
							{
								if(IsHost)
									monsters.Add(new Enemy(this,spawnQueue[p][0].type, new Point((int)tile.X * TileSize, (int)tile.Y * TileSize)));

								spawnQueue[p][0] = new SpawnTask(spawnQueue[p][0].type, spawnQueue[p][0].Y - 1);
								if (spawnQueue[p][0].Y <= 0)
								{
									spawnQueue[p].RemoveAt(0);
								}
							}
							continue;
						}
						switch (p)
						{
							case 0:
								{
									for (int x = 7; x < 10; x++)
									{
										if (Game1.random.NextDouble() < 0.5 && !map.IsCollidingWithMonster(new Rectangle(x * 16 * 3, 0, 48, 48),this))
										{
											if (IsHost)
												monsters.Add(new Enemy(this, spawnQueue[p][0].type, new Point(x * TileSize, 0)));

											spawnQueue[p][0] = new SpawnTask(spawnQueue[p][0].type, spawnQueue[p][0].Y - 1);
											if (spawnQueue[p][0].Y <= 0)
											{
												spawnQueue[p].RemoveAt(0);
											}
											break;
										}
									}
									break;
								}
							case 1:
								{
									for (int y = 7; y < 10; y++)
									{
										if (Game1.random.NextDouble() < 0.5 && !map.IsCollidingWithMonster(new Rectangle(720, y * TileSize, 48, 48), this))
										{
											if (IsHost)
												monsters.Add(new Enemy(this, spawnQueue[p][0].type, new Point(15 * TileSize, y * TileSize)));

											spawnQueue[p][0] = new SpawnTask(spawnQueue[p][0].type, spawnQueue[p][0].Y - 1);
											if (spawnQueue[p][0].Y <= 0)
											{
												spawnQueue[p].RemoveAt(0);
											}
											break;
										}
									}
									break;
								}
							case 2:
								{
									for (int x2 = 7; x2 < 10; x2++)
									{
										if (Game1.random.NextDouble() < 0.5 && !map.IsCollidingWithMonster(new Rectangle(x2 * 16 * 3, 15 * TileSize, 48, 48), this))
										{
											if (IsHost)
												monsters.Add(new Enemy(this,spawnQueue[p][0].type, new Point(x2 * TileSize, 15 * TileSize)));

											spawnQueue[p][0] = new SpawnTask(spawnQueue[p][0].type, spawnQueue[p][0].Y - 1);
											if (spawnQueue[p][0].Y <= 0)
											{
												spawnQueue[p].RemoveAt(0);
											}
											break;
										}
									}
									break;
								}
							case 3:
								{
									for (int y2 = 7; y2 < 10; y2++)
									{
										if (Game1.random.NextDouble() < 0.5 && !map.IsCollidingWithMonster(new Rectangle(0, y2 * TileSize, 48, 48), this))
										{
											if (IsHost)
												monsters.Add(new Enemy(this, spawnQueue[p][0].type, new Point(0, y2 * TileSize)));

											spawnQueue[p][0] = new SpawnTask(spawnQueue[p][0].type, spawnQueue[p][0].Y - 1);
											if (spawnQueue[p][0].Y <= 0)
											{
												spawnQueue[p].RemoveAt(0);
											}
											break;
										}
									}
									break;
								}
						}
					}

					//When only spikeys are left, and no monsters spawn anymore, set all of their health to one
					//TODO: Sync?
					if (waveTimer <= 0 && monsters.Count > 0 && IsSpawnQueueEmpty())
					{
						bool onlySpikeys = true;
						foreach (Enemy monster in monsters)
						{
							if (monster.type != MONSTER_TYPE.spikey)
							{
								onlySpikeys = false;
								break;
							}
						}
						if (onlySpikeys)
						{
							foreach (Enemy monster2 in monsters)
							{
								monster2.health = 1;
							}
						}
					}

					//If finished the level
					if (waveTimer <= 0 && monsters.Count == 0 && IsSpawnQueueEmpty())
					{

						//NET Complete Level
						if(IsHost)
						{
							PK_CompleteLevel message = new();
							modInstance.SyncMessage(message);
							OnCompleteLevel();
						}
						
					}
				}

				for (int i = monsters.Count - 1; i >= 0; i--)
				{
					// Target the closest player
					float minimumDist = float.MaxValue;
					Vector2 targetPosition = Vector2.Zero;

					foreach(BasePlayer p in playerList.Values)
					{
						float dist = (p.position - monsters[i].position.Location.ToVector2()).LengthSquared();
						if (dist < minimumDist)
						{
							minimumDist = dist;
							targetPosition = p.position;
						}
					}

					monsters[i].Move(targetPosition, time);

					if (i < monsters.Count && monsters[i].position.Intersects(player.boundingBox) && !player.IsInvincible())
					{
						if (zombieModeTimer <= 0)
						{

							PlayerDie();

							//NET player death
							PK_PlayerDeath message = new()
							{
								id = Game1.player.UniqueMultiplayerID
							};
							modInstance.SyncMessage(message);

							break;
						}
						if (monsters[i].type != MONSTER_TYPE.dracula)
						{
							//NET EnemyKilled
							PK_EnemyKilled message = new()
							{
								id = monsters[i].id
							};
							modInstance.SyncMessage(message);

							AddGuts(monsters[i].position.Location, monsters[i].type);
							monsters.RemoveAt(i);
							Game1.playSound("Cowboy_monsterDie");
						}
					}
				}
			}


			//NET EnemyPositions
			if(IsHost)
			{
				PK_EnemyPositions message = new()
				{
					positions = new Dictionary<long, Point>()
				};
				foreach (Enemy m in monsters)
				{
					message.positions.Add(m.id, m.position.Location);
				}
				modInstance.SyncMessage(message);
			}

			return false;
		}

		public void BuyItem(BasePlayer basePlayer, ITEM_TYPE boughtItem)
		{
            switch (boughtItem)
            {
                case ITEM_TYPE.AMMO1:
                case ITEM_TYPE.AMMO2:
                case ITEM_TYPE.AMMO3:
                    basePlayer.ammoLevel++;
                    basePlayer.bulletDamage++;
                    break;
                case ITEM_TYPE.FIRESPEED1:
                case ITEM_TYPE.FIRESPEED2:
                case ITEM_TYPE.FIRESPEED3:
                    basePlayer.fireSpeedLevel++;
                    break;
                case ITEM_TYPE.RUNSPEED1:
                case ITEM_TYPE.RUNSPEED2:
                    basePlayer.runSpeedLevel++;
                    break;
                case ITEM_TYPE.LIFE:
                    lives++;
                    break;
                case ITEM_TYPE.SPREADPISTOL:
                    basePlayer.spreadPistol = true;
                    break;
                case ITEM_TYPE.STAR:
                    basePlayer.heldItem = new Powerup(this, POWERUP_TYPE.SHERRIFF, Point.Zero, 9999);
                    break;
            }
        }

		public void OnCompleteLevel(int level = -1)
		{
			hasGopherAppeared = false;
			waveTimer = 80000;
			betweenWaveTimer = 3333;
			if(level == -1)
			{
				currentLevel++;
			}
			else
			{
				currentLevel = level;
			}
			

			switch (currentLevel)
			{
				case 1:
				case 2:
				case 3:
					monsterChances[0] = new Vector2(monsterChances[0].X + 0.001f, monsterChances[0].Y + 0.02f);
					if (currentLevel > 1)
					{
						monsterChances[2] = new Vector2(monsterChances[2].X + 0.001f, monsterChances[2].Y + 0.01f);
					}
					monsterChances[6] = new Vector2(monsterChances[6].X + 0.001f, monsterChances[6].Y + 0.01f);
					if (newGamePlus > 0)
					{
						monsterChances[4] = new Vector2(0.002f, 0.1f);
					}
					break;
				case 4:
				case 5:
				case 6:
				case 7:
					if (monsterChances[5].Equals(Vector2.Zero))
					{
						monsterChances[5] = new Vector2(0.01f, 0.15f);
						if (newGamePlus > 0)
						{
							monsterChances[5] = new Vector2(0.01f + newGamePlus * 0.004f, 0.15f + newGamePlus * 0.04f);
						}
					}
					monsterChances[0] = Vector2.Zero;
					monsterChances[6] = Vector2.Zero;
					monsterChances[2] = new Vector2(monsterChances[2].X + 0.002f, monsterChances[2].Y + 0.02f);
					monsterChances[5] = new Vector2(monsterChances[5].X + 0.001f, monsterChances[5].Y + 0.02f);
					monsterChances[1] = new Vector2(monsterChances[1].X + 0.0018f, monsterChances[1].Y + 0.08f);
					if (newGamePlus > 0)
					{
						monsterChances[4] = new Vector2(0.001f, 0.1f);
					}
					break;
				case 8:
				case 9:
				case 10:
				case 11:
					monsterChances[5] = Vector2.Zero;
					monsterChances[1] = Vector2.Zero;
					monsterChances[2] = Vector2.Zero;
					if (monsterChances[3].Equals(Vector2.Zero))
					{
						monsterChances[3] = new Vector2(0.012f, 0.4f);
						if (newGamePlus > 0)
						{
							monsterChances[3] = new Vector2(0.012f + (float)newGamePlus * 0.005f, 0.4f + (float)newGamePlus * 0.075f);
						}
					}
					if (monsterChances[4].Equals(Vector2.Zero))
					{
						monsterChances[4] = new Vector2(0.003f, 0.1f);
					}
					monsterChances[3] = new Vector2(monsterChances[3].X + 0.002f, monsterChances[3].Y + 0.05f);
					monsterChances[4] = new Vector2(monsterChances[4].X + 0.0015f, monsterChances[4].Y + 0.04f);
					if (currentLevel == 11)
					{
						monsterChances[4] = new Vector2(monsterChances[4].X + 0.01f, monsterChances[4].Y + 0.04f);
						monsterChances[3] = new Vector2(monsterChances[3].X - 0.01f, monsterChances[3].Y + 0.04f);
					}
					break;
			}
			//Slightly ramp up chanced for each round completed
			if (newGamePlus > 0)
			{
				for (int j = 0; j < monsterChances.Count; j++)
				{
					monsterChances[j] *= 1.1f;
				}
			}
			//Every second level shall be a shopping level (? probably overwritten by boss level then?)
			if (currentLevel > 0 && currentLevel % 2 == 0)
			{
				StartShoppingLevel();
			}
			else if (currentLevel > 0)
			{
				waitingForPlayerToMoveDownAMap = true;
				map[7, 15] = MAP_TILE.SAND;
				map[8, 15] = MAP_TILE.SAND;
				map[9, 15] = MAP_TILE.SAND;
			}
		}

		protected void ProcessInputs()
		{

			if (_buttonHeldFrames[GameKeys.Exit] == 1)
			{
				quit = true;
			}

			//Start the game after startmenu
			ui.ProcessInputs(_buttonHeldFrames);
			if (ui.onStartMenu) return;

			player.ProcessInputs(_buttonHeldFrames);

			if (_buttonHeldFrames[GameKeys.UsePowerup] == 1 && !ui.onStartMenu && !gameOver && player.heldItem != null && player.deathTimer <= 0f && zombieModeTimer <= 0)
			{
				player.UsePowerup(player.heldItem.type);
				player.heldItem = null;
			}

		}

		public virtual void ApplyLevelSpecificStates()
		{
			if (currentLevel == 12)
			{
				shootoutLevel = true;

				if(IsHost)
					monsters.Add(new Dracula(this));

			}
			else if (currentLevel > 0 && currentLevel % 4 == 0)
			{
				shootoutLevel = true;

				if(IsHost)
					monsters.Add(new Outlaw(this, new Point(8 * TileSize, 13 * TileSize)));

                Game1.playSound("cowboy_outlawsong", out outlawSong);
            }
		}

		// Empty
		public void receiveLeftClick(int x, int y, bool playSound = true)
		{
		}

		// Empty
		public void leftClickHeld(int x, int y)
		{
		}

		// Empty
		public void receiveRightClick(int x, int y, bool playSound = true)
		{
		}

		// Empty
		public void releaseLeftClick(int x, int y)
		{
		}

		// Empty
		public void releaseRightClick(int x, int y)
		{
		}

		public bool IsSpawnQueueEmpty()
		{
			for (int i = 0; i < 4; i++)
			{
				if (spawnQueue[i].Count > 0)
				{
					return false;
				}
			}
			return true;
		}

		public void StartShoppingLevel()
		{
			merchantBox.Y = -TileSize;
			shopping = true;
			merchantArriving = true;
			merchantShopOpen = false;
            overworldSong?.Stop(AudioStopOptions.Immediate);
            monsters.Clear();
			waitingForPlayerToMoveDownAMap = true;
			storeItems.Clear();

			//Fill store with the next upgrade items
			ITEM_TYPE runSpeedItem;
			if (player.runSpeedLevel == 0) runSpeedItem = ITEM_TYPE.RUNSPEED1;
			else if (player.runSpeedLevel == 1) runSpeedItem = ITEM_TYPE.RUNSPEED2;
			else runSpeedItem = ITEM_TYPE.LIFE;

			ITEM_TYPE fireSpeedItem;
			if (player.fireSpeedLevel == 0) fireSpeedItem = ITEM_TYPE.FIRESPEED1;
			else if (player.fireSpeedLevel == 1) fireSpeedItem = ITEM_TYPE.FIRESPEED2;
			else if (player.fireSpeedLevel == 2) fireSpeedItem = ITEM_TYPE.FIRESPEED3;
			else if (player.ammoLevel >= 3 && !player.spreadPistol) fireSpeedItem = ITEM_TYPE.SPREADPISTOL;
			else fireSpeedItem = ITEM_TYPE.STAR;

			ITEM_TYPE ammoItem;
			if (player.ammoLevel == 0) ammoItem = ITEM_TYPE.AMMO1;
			else if (player.ammoLevel == 1) ammoItem = ITEM_TYPE.AMMO2;
			else if (player.ammoLevel == 2) ammoItem = ITEM_TYPE.AMMO3;
			else ammoItem = ITEM_TYPE.STAR;

			storeItems.Add(new Rectangle(7 * TileSize + 12, 8 * TileSize - TileSize * 2, TileSize, TileSize), runSpeedItem);
			storeItems.Add(new Rectangle(8 * TileSize + 24, 8 * TileSize - TileSize * 2, TileSize, TileSize), fireSpeedItem);
			storeItems.Add(new Rectangle(9 * TileSize + 36, 8 * TileSize - TileSize * 2, TileSize, TileSize), ammoItem);

		}

		public void receiveKeyPress(Keys k)
		{

		}

		public void receiveKeyRelease(Keys k)
		{
		}

		public static int GetPriceForItem(ITEM_TYPE whichItem)
		{
			return whichItem switch
			{
				ITEM_TYPE.AMMO1 => 15,
				ITEM_TYPE.AMMO2 => 30,
				ITEM_TYPE.AMMO3 => 45,
				ITEM_TYPE.FIRESPEED1 => 10,
				ITEM_TYPE.FIRESPEED2 => 20,
				ITEM_TYPE.FIRESPEED3 => 30,
				ITEM_TYPE.LIFE => 10,
				ITEM_TYPE.RUNSPEED1 => 8,
				ITEM_TYPE.RUNSPEED2 => 20,
				ITEM_TYPE.SPREADPISTOL => 99,
				ITEM_TYPE.STAR => 10,
				_ => 5,
			};
		}

		public void draw(SpriteBatch b)
		{
			//Start drawing
			b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			//Draw the UI
			ui.Draw(b);
			//Draw game over screen
			if(ui.onStartMenu)
			{

			}
			else if ((gameOver || gamerestartTimer > 0) && !endCutscene)
			{

			}
			else if (endCutscene) //Draw the final cutscene
			{
				Cutscene.Draw(b);
			}
			else
			{
				//Lightning animation for zombie powerup
				if (zombieModeTimer > 8200)
				{
					//Draw the flashing characters
					foreach (BasePlayer p in playerList.Values)
					{
						b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + p.position, new Rectangle(384 + ((zombieModeTimer / 200 % 2 == 0) ? 16 : 0), 1760, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
					}

					//Draw the lighning above the players
					foreach(BasePlayer p in playerList.Values)
					{
						for (int y = (int)(p.position.Y - TileSize); y > -TileSize; y -= TileSize)
						{
							b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(p.position.X, y), new Rectangle(368 + ((y / TileSize % 3 == 0) ? 16 : 0), 1744, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
						}
					}

					b.End();
					return;
				}

				//Draw the Map
				for (int x = 0; x < 16; x++)
				{
					for (int y = 0; y < 16; y++)
					{
						b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(x, y) * 16f * 3f + new Vector2(0f, newMapPosition - 16 * TileSize), new Rectangle(464 + 16 * (int)map[x, y] + ((map[x, y] == MAP_TILE.CACTUS && cactusDanceTimer > 800f) ? 16 : 0), 1680 - (int)world * 16, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
					}
				}
				//Also draw the next map if transitioning between levels
				if (scrollingMap)
				{
					for (int x = 0; x < 16; x++)
					{
						for (int y = 0; y < 16; y++)
						{
							b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(x, y) * 16f * 3f + new Vector2(0f, newMapPosition), new Rectangle(464 + 16 * (int)nextMap[x, y] + ((nextMap[x, y] == MAP_TILE.CACTUS && cactusDanceTimer > 800f) ? 16 : 0), 1680 - (int)world * 16, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
						}
					}
					// Not quite sure?
					b.Draw(Game1.staminaRect, new Rectangle((int)topLeftScreenCoordinate.X, -1, 16 * TileSize, (int)topLeftScreenCoordinate.Y), Game1.staminaRect.Bounds, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
					b.Draw(Game1.staminaRect, new Rectangle((int)topLeftScreenCoordinate.X, (int)topLeftScreenCoordinate.Y + 16 * TileSize, 16 * TileSize, (int)topLeftScreenCoordinate.Y + 2), Game1.staminaRect.Bounds, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
				}

				//Draw all temporary sprites
				foreach (TemporaryAnimatedSprite temporarySprite in temporarySprites)
				{
					temporarySprite.draw(b, localPosition: true);
				}

				//Draw all powerups
				foreach (Powerup powerup in powerups)
				{
					powerup.Draw(b);
				}

				//Draw all bullets
				foreach (Bullet bullet in bullets)
				{
					bullet.Draw(b);
				}

				//Draw shop
				if (shopping)
				{
					if (merchantArriving && !merchantShopOpen)
					{
						b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(merchantBox.Location.X, merchantBox.Location.Y), new Rectangle(464 + ((shoppingTimer / 100 % 2 == 0) ? 16 : 0), 1728, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, (float)merchantBox.Y / 10000f + 0.001f);
					}
					else
					{
						int whichFrame = (player.boundingBox.X - merchantBox.X > TileSize) ? 2 : ((merchantBox.X - player.boundingBox.X > TileSize) ? 1 : 0);
						b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(merchantBox.Location.X, merchantBox.Location.Y), new Rectangle(496 + whichFrame * 16, 1728, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, (float)merchantBox.Y / 10000f + 0.001f);
						b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(merchantBox.Location.X - TileSize, merchantBox.Location.Y + TileSize), new Rectangle(529, 1744, 63, 32), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, (float)merchantBox.Y / 10000f + 0.001f);
						foreach (KeyValuePair<Rectangle, ITEM_TYPE> v in storeItems)
						{
							b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(v.Key.Location.X, v.Key.Location.Y), new Rectangle(320 + (int)v.Value * 16, 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, (float)v.Key.Location.Y / 10000f);

							//Draw three times with slight offset to obtain a thicker font i suppose
							b.DrawString(Game1.smallFont, string.Concat(GetPriceForItem(v.Value)), topLeftScreenCoordinate + new Vector2((v.Key.Location.X + TileSize / 2) - Game1.smallFont.MeasureString(string.Concat(GetPriceForItem(v.Value))).X / 2f, v.Key.Location.Y + TileSize + 3), new Color(88, 29, 43), 0f, Vector2.Zero, 1f, SpriteEffects.None, v.Key.Location.Y / 10000f + 0.002f);
							b.DrawString(Game1.smallFont, string.Concat(GetPriceForItem(v.Value)), topLeftScreenCoordinate + new Vector2((v.Key.Location.X + TileSize / 2) - Game1.smallFont.MeasureString(string.Concat(GetPriceForItem(v.Value))).X / 2f - 1f, v.Key.Location.Y + TileSize + 3), new Color(88, 29, 43), 0f, Vector2.Zero, 1f, SpriteEffects.None, v.Key.Location.Y / 10000f + 0.002f);
							b.DrawString(Game1.smallFont, string.Concat(GetPriceForItem(v.Value)), topLeftScreenCoordinate + new Vector2((v.Key.Location.X + TileSize / 2) - Game1.smallFont.MeasureString(string.Concat(GetPriceForItem(v.Value))).X / 2f + 1f, v.Key.Location.Y + TileSize + 3), new Color(88, 29, 43), 0f, Vector2.Zero, 1f, SpriteEffects.None, v.Key.Location.Y / 10000f + 0.002f);
						}

						//Draw Speech bubble
						b.Draw(
							shopBubbleTexture,
							topLeftScreenCoordinate + new Vector2(merchantBox.Location.X - TileSize / 2, merchantBox.Location.Y - TileSize * 2),
							new Rectangle(0, 0, 32, 32),
							Color.White,
							0f,
							Vector2.Zero,
							3f,
							SpriteEffects.None,
							merchantBox.Location.Y / 10000f + 0.001f
						);
					}
				}

				//Draw the arrow pointing down to the next map if the level is finished
				if (waitingForPlayerToMoveDownAMap && (merchantShopOpen || !shopping) && shoppingTimer < 250)
				{
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(8.5f, 15f) * TileSize + new Vector2(-12f, 0f), new Rectangle(355, 1750, 8, 8), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.001f);
				}

				//Draw Players
				foreach(BasePlayer p in playerList.Values)
				{
					p.Draw(b);
				}

				// Draw all monsters
				foreach (Enemy monster in monsters)
				{
					monster.Draw(b);
				}

				// Draw the single gopher that runs randomly hihi
				if (gopherRunning)
				{
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(gopherBox.X, gopherBox.Y), new Rectangle(320 + waveTimer / 100 % 4 * 16, 1792, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, (float)gopherBox.Y / 10000f + 0.001f);
				}

				//Draw the animation when transitioning to the next level
				if (gopherTrain && gopherTrainPosition > -TileSize)
				{
					b.Draw(Game1.staminaRect, new Rectangle((int)topLeftScreenCoordinate.X, (int)topLeftScreenCoordinate.Y, 16 * TileSize, 16 * TileSize), Game1.staminaRect.Bounds, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 0.95f);
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(player.position.X - (float)(TileSize / 2), gopherTrainPosition), new Rectangle(384 + gopherTrainPosition / 30 % 4 * 16, 1792, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.96f);
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(player.position.X + (float)(TileSize / 2), gopherTrainPosition), new Rectangle(384 + gopherTrainPosition / 30 % 4 * 16, 1792, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.96f);
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(player.position.X, gopherTrainPosition - TileSize * 3), new Rectangle(320 + gopherTrainPosition / 30 % 4 * 16, 1792, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.96f);
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(player.position.X - (float)(TileSize / 2), gopherTrainPosition - TileSize), new Rectangle(400, 1728, 32, 32), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.97f);
					if (player.IsHoldingItem())
					{
						b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + player.position + new Vector2(0f, -TileSize / 4), new Rectangle(384, 1760, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.98f);
						b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + player.position + new Vector2(0f, -TileSize * 2 / 3) + new Vector2(0f, -TileSize / 4), new Rectangle(320 + (int)player.GetHeldItem() * 16, 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.99f);
					}
					else
					{
						b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + player.position + new Vector2(0f, -TileSize / 4), new Rectangle(464, 1760, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.98f);
					}
				}

				//Draw Screen Flash
				if (screenFlash > 0)
				{
					b.Draw(Game1.staminaRect, new Rectangle((int)topLeftScreenCoordinate.X, (int)topLeftScreenCoordinate.Y, 16 * TileSize, 16 * TileSize), Game1.staminaRect.Bounds, new Color(255, 214, 168), 0f, Vector2.Zero, SpriteEffects.None, 1f);
				}
			}
			b.End();
		}

		public void changeScreenSize()
		{
			topLeftScreenCoordinate = new Vector2(Game1.viewport.Width / 2 - 384, Game1.viewport.Height / 2 - 384);
		}

		public void unload()
		{

			overworldSong?.Stop(AudioStopOptions.Immediate);
			outlawSong?.Stop(AudioStopOptions.Immediate);
			zombieSong?.Stop(AudioStopOptions.Immediate);

			lives = 3;
			Game1.stopMusicTrack(MusicContext.MiniGame);

			IsHost = false;
			modInstance.isHost.Value = false;
			modInstance.isHostAvailable = false;

			PK_ExitGame message = new();
			modInstance.SyncMessage(message);
		}

		public void receiveEventPoke(int data)
		{
		}

		public string minigameId()
		{
			return "PrairieKing";
		}

		public bool doMainGameUpdates()
		{
			return false;
		}

		public bool forceQuit()
		{
			unload();
			return true;
		}
	}
}
