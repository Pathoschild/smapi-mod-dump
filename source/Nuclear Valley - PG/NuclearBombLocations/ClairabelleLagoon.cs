/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ApryllForever/NuclearBombLocations
**
*************************************************/

using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using xTile;
using StardewValley.Characters;
using StardewValley.Network;
using StardewValley.Objects;
using System.Linq;
using xTile.Tiles;
using StardewValley.Locations;
using Object = StardewValley.Object;
using StardewValley.GameData;
using StardewValley.Menus;
using StardewValley.Extensions;

namespace NuclearBombLocations
{
    [XmlType("Mods_ApryllForever_NuclearBombLocations_ClairabelleLagoon")]
    public class ClairabelleLagoon : NuclearLocation
    {
		static IModHelper Helper;

		public static IMonitor Monitor;

		[XmlIgnore]
		public List<SuspensionBridge> suspensionBridges = new List<SuspensionBridge>();

		private readonly NetEvent1Field<int, NetInt> rumbleAndFadeEvent = new NetEvent1Field<int, NetInt>();

        public static string SwimQuestion = "Do you want to call the Prismatic Pod to meet up for a swim?";

        private static Multiplayer multiplayer;


		//Mermaid Related Code Below
		[XmlIgnore]
		public NetEvent0 sapphiremermaidPuzzleSuccess = new NetEvent0();
		public Vector2 scale;
		[XmlIgnore]
		public ICue internalSound;
		[XmlIgnore]
		public int shakeTimer;
		[XmlIgnore]
		public int lastNoteBlockSoundTime;
		[XmlElement("preservedParentSheetIndex")]
		public readonly NetInt preservedParentSheetIndex = new NetInt();

		[XmlIgnore]
		public Texture2D mermaidSprites;
		[XmlIgnore]
		public int[] mermaidIdle = new int[1];

		[XmlIgnore]
		public int[] mermaidWave = new int[4]
		{
			1,
			1,
			2,
			2
		};

		[XmlIgnore]
		public int lastPlayedNote = -1;

		[XmlIgnore]
		public int songIndex = -1;
		public int[] mermaidChill = new int[6]
{
			0, 0, 0, 0, 0, 0
		
};
		[XmlIgnore]
		public int[] mermaidDance = new int[6]
{
			5,
			5,
			5,
			6,
			6,
			6
};
		[XmlIgnore]
		public int[] mermaidReward = new int[7]
		{
			3,
			3,
			3,
			3,
			3,
			4,
			4
		};
		[XmlIgnore]
		public int[] mermaidLongDance = new int[13]
{
			5,
			1,
			5,
			1,
			5,
			6,
			5,
			6,
			5,
			6,
			5,
			1,
			2
};

		[XmlIgnore]
		public int mermaidFrameIndex;

		[XmlIgnore]
		public int[] currentMermaidAnimation;

		[XmlIgnore]
		public float mermaidFrameTimer;

		[XmlIgnore]
		public float mermaidDanceTime;
		[XmlIgnore]
		public float mermaidRewardTime;
		[XmlIgnore]
		public float mermaidWaveTime;
		[XmlIgnore]
		public float mermaidLongDanceTime;
		[XmlIgnore]
		public float mermaidChillTime;
		// Mermaid Related Code is Above

		[XmlIgnore]
		private float smokeTimer;

		[XmlIgnore]
		public float hissTime;
		[XmlIgnore]
		public float hissTimer;

		internal static void Setup(IModHelper Helper)
		{
			ClairabelleLagoon.Helper = Helper;
			//Helper.Events.GameLoop.DayStarted += OnDayStarted;
		}

		public ClairabelleLagoon() { }

		

		public ClairabelleLagoon(IModContentHelper content)
        : base(content, "ClairabelleLagoon", "ClairabelleLagoon")
        {


		}

		

		protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddField(rumbleAndFadeEvent); //genSeed
		}
        public override bool IsLocationSpecificPlacementRestriction(Vector2 tileLocation)
        {
            foreach (SuspensionBridge suspensionBridge in this.suspensionBridges)
            {
                if (suspensionBridge.CheckPlacementPrevention(tileLocation))
                {
                    return true;
                }
            }
            return base.IsLocationSpecificPlacementRestriction(tileLocation);
        }
        
        protected override void resetLocalState()
        {
			
			base.resetLocalState();

			//suspensionBridges.Clear();
			//SuspensionBridge bridge = new SuspensionBridge(44, 135);
			//suspensionBridges.Add(bridge);
			


			int numSeagulls = Game1.random.Next(6);
            foreach (Vector2 tile in Utility.getPositionsInClusterAroundThisTile(new Vector2(Game1.random.Next(base.map.DisplayWidth / 64), Game1.random.Next(12, base.map.DisplayHeight / 64)), numSeagulls))
            {
                if (!base.isTileOnMap(tile) || (!this.CanItemBePlacedHere(tile) && !base.isWaterTile((int)tile.X, (int)tile.Y)))
                {
                    continue;
                }
                int state;
                state = 3;
                if (base.isWaterTile((int)tile.X, (int)tile.Y) && this.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Passable", "Buildings") == null)
                {
                    state = 2;
                    if (Game1.random.NextDouble() < 0.7)
                    {
                        continue;
                    }
                }
                base.critters.Add(new Seagull(tile * 64f + new Vector2(32f, 32f), state));
            }



			mermaidSprites = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");

			if (Game1.player.Tile.Y == 1)
			{
                if (Game1.player.Tile.X == 1)
                {
					Game1.player.Position = new Vector2(25.5f * Game1.tileSize, 19 * Game1.tileSize);
				}
			}

            var v1 = new Vector2(53);
            var v2 = new Vector2(22);
            var v3 = new Vector2(34);
            var v4 = new Vector2(67);
            var v5 = new Vector2(76);
            var v6 = new Vector2(51);
            addCritter(new Crow((int)v1.X, (int)v2.Y));
            addCritter(new Crow((int)v3.X, (int)v4.Y));
            addCritter(new Crow((int)v5.X, (int)v6.Y));

            addCritter(new Crow(57, 20));
            addCritter(new Crow(74, 26));
            addCritter(new Crow(69, 26));
            addCritter(new Crow(40, 9));

            addCritter(new Crow(67, 39));
            addCritter(new Crow(52, 41));
            addCritter(new Crow(46, 55));
            addCritter(new Crow(63, 55));
            addCritter(new Crow(42, 61));
            addCritter(new Crow(42, 43));

            addCritter(new Crow(6, 13));
            addCritter(new Crow(11, 7));

            addCritter(new CrabCritter(new Vector2(48f, 5f) * 64f));
            addCritter(new CrabCritter(new Vector2(47f, 6f) * 64f));
            addCritter(new CrabCritter(new Vector2(76f, 5f) * 64f));

            addCritter(new CrabCritter(new Vector2(47f, 16f) * 64f));
            addCritter(new CrabCritter(new Vector2(47f, 28f) * 64f));
            addCritter(new CrabCritter(new Vector2(45f, 34f) * 64f));
            addCritter(new CrabCritter(new Vector2(41f, 45f) * 64f));
            addCritter(new CrabCritter(new Vector2(53f, 46f) * 64f));
            addCritter(new CrabCritter(new Vector2(55f, 72f) * 64f));
            addCritter(new CrabCritter(new Vector2(41f, 53f) * 64f));
            addCritter(new CrabCritter(new Vector2(30f, 56f) * 64f));

            addCritter(new CrabCritter(new Vector2(33f, 4f) * 64f));
            addCritter(new CrabCritter(new Vector2(34f, 4f) * 64f));
            addCritter(new CrabCritter(new Vector2(32f, 3f) * 64f));
            addCritter(new CrabCritter(new Vector2(33f, 5f) * 64f));

            addCritter(new CrabCritter(new Vector2(9f, 3f) * 64f));
            addCritter(new CrabCritter(new Vector2(13f, 12f) * 64f));
            addCritter(new CrabCritter(new Vector2(5f, 15f) * 64f));
            addCritter(new CrabCritter(new Vector2(9f, 18f) * 64f));

            addCritter(new CrabCritter(new Vector2(25f, 9f) * 64f));
            addCritter(new CrabCritter(new Vector2(27f, 12f) * 64f));
            addCritter(new CrabCritter(new Vector2(19f, 24f) * 64f));
            addCritter(new CrabCritter(new Vector2(18f, 27f) * 64f));
            addCritter(new CrabCritter(new Vector2(15f, 26f) * 64f));
            addCritter(new CrabCritter(new Vector2(17f, 19f) * 64f));

			addCritter(new CrabCritter(new Vector2(25f, 38f) * 64f));
            addCritter(new CrabCritter(new Vector2(24f, 32f) * 64f));
            addCritter(new CrabCritter(new Vector2(13f, 41f) * 64f));
            addCritter(new CrabCritter(new Vector2(8f, 49f) * 64f));
            addCritter(new CrabCritter(new Vector2(15f, 55f) * 64f));
            addCritter(new CrabCritter(new Vector2(19f, 61f) * 64f));

			addJumperFrog(new Vector2(42, 16));
            addJumperFrog(new Vector2(42, 32));
            addJumperFrog(new Vector2(45, 1));
            addJumperFrog(new Vector2(27, 55));

			//addBirdiesHere(1);
            /*
			{
				Point offset = new Point(0, 0); 
				Vector2 vector_offset = new Vector2(offset.X, offset.Y);
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(38f, 48f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(41f, 45f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(46f, 45f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(50f, 45f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(45f, 52f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(51f, 52f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(63f, 45f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(66f, 53f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(66f, 62f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(66f, 69f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(48f, 69f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(31f, 69f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(24f, 69f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(83f, 48f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(83f, 52f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(83f, 56f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(83f, 60f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(83f, 64f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(83f, 68f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(83f, 72f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(83f, 76f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(83f, 80f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(22f, 107f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(22f, 111f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(22f, 115f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(22f, 119f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(22f, 123f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(22f, 127f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(25f, 120f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(28f, 120f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(31f, 120f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(34f, 120f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
			}  */

        }

		public override void TransferDataFromSavedLocation(GameLocation l)
		{
			base.TransferDataFromSavedLocation(l);
		}


        public void addBirdiesHere(double chance, bool onlyIfOnScreen = false)
        {
            if (Game1.timeOfDay >= 2500 )
            {
                return;
            }
            Season season = GetSeason();

            while (Game1.random.NextDouble() < chance)
            {
                int num = Game1.random.Next(1, 4);
                bool flag = false;
                int num2 = 0;
                while (!flag && num2 < 5)
                {
                    Vector2 randomTile = getRandomTile();
                    if (!onlyIfOnScreen || !Utility.isOnScreen(randomTile * 64f, 64))
                    {
                        Microsoft.Xna.Framework.Rectangle area = new Microsoft.Xna.Framework.Rectangle((int)randomTile.X - 2, (int)randomTile.Y - 2, 5, 5);
                        if (isAreaClear(area))
                        {
                            List<Critter> list = new List<Critter>();
                            int startingIndex =  25;
                           

                            for (int i = 0; i < num; i++)
                            {
                                list.Add(new Birdie(-100, -100, startingIndex));
                            }

                            addCrittersStartingAtTile(randomTile, list);
                            flag = true;
                        }
                    }

                    num2++;
                }
            }
        }




        private void addCrittersStartingAtTile(Vector2 tile, List<Critter> crittersToAdd)
        {
            if (crittersToAdd == null)
            {
                return;
            }

            int num = 0;
            HashSet<Vector2> hashSet = new HashSet<Vector2>();
            while (crittersToAdd.Count > 0 && num < 20)
            {
                if (hashSet.Contains(tile))
                {
                    tile = Utility.getTranslatedVector2(tile, Game1.random.Next(4), 1f);
                }
                else
                {
                    if (CanItemBePlacedHere(tile))
                    {
                        Critter critter = crittersToAdd.Last();
                        critter.position = tile * 64f;
                        critter.startingPosition = tile * 64f;
                        critters.Add(critter);
                        crittersToAdd.RemoveAt(crittersToAdd.Count - 1);
                    }

                    tile = Utility.getTranslatedVector2(tile, Game1.random.Next(4), 1f);
                    hashSet.Add(tile);
                }

                num++;
            }
        }


        public override void DayUpdate(int dayOfMonth)
        {
            base.DayUpdate(dayOfMonth);
            Microsoft.Xna.Framework.Rectangle tidePools;
            tidePools = new Microsoft.Xna.Framework.Rectangle(0, 0, 47, 63);
            float chance;
            chance = 1f;
            while (Game1.random.NextDouble() < (double)chance)
            {
                string id;
                id = ((Game1.random.NextDouble() < 0.2) ? "(O)372" : "(O)394");
                Vector2 position;
                position = new Vector2(Game1.random.Next(tidePools.X, tidePools.Right), Game1.random.Next(tidePools.Y, tidePools.Bottom));
                if (this.CanItemBePlacedHere(position))
                {
                    this.dropObject(ItemRegistry.Create<Object>(id), position * 64f, Game1.viewport, initialPlacement: true);
                }
                chance /= 2f;
            }
            Microsoft.Xna.Framework.Rectangle seaweedShore;
            seaweedShore = new Microsoft.Xna.Framework.Rectangle(6, 2, 22, 60);
            chance = 0.25f;
            while (Game1.random.NextDouble() < (double)chance)
            {
                if (Game1.random.NextDouble() < 0.15)
                {
                    Vector2 position2;
                    position2 = new Vector2(Game1.random.Next(seaweedShore.X, seaweedShore.Right), Game1.random.Next(seaweedShore.Y, seaweedShore.Bottom));
                    if (this.CanItemBePlacedHere(position2))
                    {
                        this.dropObject(ItemRegistry.Create<Object>("(O)152"), position2 * 64f, Game1.viewport, initialPlacement: true);
                    }
                }
                chance /= 2f;
            }
           // if (!base.IsSummerHere() || Game1.dayOfMonth < 12 || Game1.dayOfMonth > 14)
           // {
           //     return;
           // }
            for (int i = 0; i < 11; i++)
            {
                this.spawnObjects();
            }
            chance = 1.5f;
            while (Game1.random.NextDouble() < (double)chance)
            {
                string id2;
                id2 = ((Game1.random.NextDouble() < 0.2) ? "(O)392" : "(O)393");
                Vector2 position3;
                position3 = base.getRandomTile();
                position3.Y /= 2f;
                string prop;
                prop = this.doesTileHaveProperty((int)position3.X, (int)position3.Y, "Type", "Back");
                if (this.CanItemBePlacedHere(position3) && (prop == null || !prop.Equals("Wood")))
                {
                    this.dropObject(ItemRegistry.Create<Object>(id2), position3 * 64f, Game1.viewport, initialPlacement: true);
                }
                chance /= 1.1f;
            }
        }

      public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
		{
			base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
			rumbleAndFadeEvent.Poll();
			if (!Game1.currentLocation.Equals(this))
			{
				
			}
		}
	
		public void mermaidDanceShow()
        {
			string mermaidDanceSpeech = "Fiona: Hey Lovelies! Thanks for coming to my performance!!!";
			Game1.drawObjectDialogue(mermaidDanceSpeech);
			mermaidChillTime = 5f;
			mermaidDanceTime = 9f;
			mermaidLongDanceTime = 13f;
			mermaidDanceTime = 9f;
			mermaidWaveTime = 3f;
			mermaidRewardTime = 4f;
			currentMermaidAnimation = mermaidIdle;
		}

		static string NuclearShopDialogue = "Hey they love! what may I do for you today?";

		public void cannonFire()
        {
			GameLocation location = new GameLocation();
			location.netAudio.StopPlaying("fuse");
		}

		public override bool performAction(string action, Farmer who, Location tileLocation)
		{
			TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite();
			GameLocation location = new GameLocation();
			GameTime time = new GameTime();
			if (action == "NuclearShop")
			{
				//createQuestionDialogue(NuclearShopDialogue, createYesNoResponses(), "SapphireVolcanoEntrance");
				carpentersk(tileLocation);
			}

			if (action == "RS.RCannon")
			{
				//int idNum = Game1.random.Next();
				//GameLocation location = Game1.currentLocation;
				//List < TemporaryAnimatedSprite > cannonSprites = new List<TemporaryAnimatedSprite>();
				//location.explode = 9f;


				//if (hissTime == 0f)
				//{
					//location.netAudio.StartPlaying("fuse");
					//Game1.pauseThenDoFunction(2000, cannonFire);
				Game1.flashAlpha = 1f;
				Game1.playSound("explosion");
				Game1.player.jump();
				Game1.player.xVelocity = -16f;

				//}
				//Game1.pauseThenDoFunction(100, cannonExplode);
				//int radius = 3;
				//RestStopLocations.Mod mod = new RestStopLocations.Mod();
				//var Game1_multiplayer = mod.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
				//multiplayer = Game1_multiplayer;
				//List<TemporaryAnimatedSprite> sprites = new List<TemporaryAnimatedSprite>();
				//var fuckMe = Game1.player.position.X;
				//Vector2 currentTile = new Vector2(Math.Min(map.Layers[0].LayerWidth - 1, Math.Max(0f, tileLocation.X - (float)radius)), Math.Min(map.Layers[0].LayerHeight - 1, Math.Max(0f, tileLocation.Y - (float)radius)));
				//sprites.Add(new TemporaryAnimatedSprite(23, 9999f, 6, 1, new Vector2(currentTile.X * 64f, currentTile.Y * 64f), flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false)
				//{
				//light = true,
				//lightRadius = radius,
				//lightcolor = Color.Black,
				//alphaFade = 0.03f - (float)radius * 0.003f,
				//Parent = this
				//});
				//Game1_multiplayer.broadcastSprites(location, sprites);


				Vector2 placementTile = new Vector2(Game1.player.position.X);
				Utility.addSmokePuff(location, (placementTile * 64f + new Vector2(3f, 0f) * 64f));
				smokeTimer -= (float)time.ElapsedGameTime.TotalMilliseconds;
				if (smokeTimer <= 0f)
				{
					Utility.addSmokePuff(this, new Vector2(25.6f, 5.7f) * 64f);
					Utility.addSmokePuff(this, new Vector2(34f, 7.2f) * 64f);
					smokeTimer = 9000f;
				}

				//{
					//location.temporarySprites.Add(new TemporaryAnimatedSprite(25, new Vector2(fuckMe + 3f, 0f *64F), Color.White, 8, flipped: false, 100f, 0, 64, 1f, 128));
				//}


					//new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, (placementTile * 64f + new Vector2(3f, 0f) * 64f), flicker: true, flipped: false, 5f, 0f, Color.Yellow, 4f, 0f, 0f, 0f, local: true);

			};
			if (action == "RS.LCannon")
			{
				Game1.flashAlpha = 1f;
				Game1.playSound("explosion");
				Game1.player.jump();
				Game1.player.xVelocity = 16f;
			}
				if (action == "RS.MermaidKiss")
			{
				Game1.currentSong = null;
				string mermaidDanceSpeech2 = "Fiona: Have a seat, Lovelies, the show is about to begin!";
				Game1.playSound("dwop");
				mermaidWaveTime = 3f;
				mermaidChillTime = 6f;
				Game1.player.jitterStrength = 8f;
				
				Game1.drawObjectDialogue(mermaidDanceSpeech2);
				Game1.screenOverlayTempSprites.AddRange(Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), 500, Color.White, 10, 2000));
				DelayedAction.playSoundAfterDelay("gusviolin", 2500);
				Game1.player.freezePause = 500;
				Game1.pauseTime = 60f;

				Game1.pauseThenDoFunction(6000, mermaidDanceShow);
				currentMermaidAnimation = mermaidIdle;
				//Game1.createItemDebris(new Object(372, 1), new Vector2(32f, 33f) * 64f, 0, this, 0); //Clam
				
			}
			if (action == "RS.MermaidReward")
			{

				Game1.playSound("seagulls");
				mermaidRewardTime = 8f;
				Game1.freezeControls = true;
				DelayedAction.playSoundAfterDelay("dwop", 300);
				DelayedAction.playSoundAfterDelay("dwop", 1000);
				Game1.screenOverlayTempSprites.AddRange(Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), 500, Color.White, 10, 2000));
				currentMermaidAnimation = mermaidIdle;
				Game1.createItemDebris (ItemRegistry.Create("(O)638"), new Vector2(32f, 33f) * 64f, 0, this, 0);
				Game1.freezeControls = false;

               


            }
			if (action == "RS.MermaidLoot")
			{
				Game1.playSound("bubbles");
				switch (Game1.random.Next(13))
				{
					case 0:
						Game1.createItemDebris(ItemRegistry.Create("(O)169"), new Vector2(91f, 115f) * 64f, 0, this, 0); //Driftwood
						break;
					case 1:
						Game1.createItemDebris(ItemRegistry.Create("(O)390"), new Vector2(93f, 115f) * 64f, 0, this, 0); //Stone
						break;
					case 2:
						Game1.createItemDebris(ItemRegistry.Create("(O)110"), new Vector2(98f, 115f) * 64f, 0, this, 0); //Spoon
						break;
					case 3:
						Game1.createItemDebris(ItemRegistry.Create("(O)105"), new Vector2(97f, 114f) * 64f, 0, this, 0); //Chewing Stick
						break;
					case 4:
						Game1.createItemDebris(ItemRegistry.Create("(O)372"), new Vector2(98f, 114f) * 64f, 0, this, 0); //Clam
						break;
					case 5:
						Game1.createItemDebris(ItemRegistry.Create("(O)392"), new Vector2(97f, 113f) * 64f, 0, this, 0); //Nautilus
						break;
					case 6:
						Game1.createItemDebris(ItemRegistry.Create("(O)372"), new Vector2(98f, 116f) * 64f, 0, this, 0); //Clam
						break;
					case 7:
						Game1.createItemDebris(ItemRegistry.Create("(O)372"), new Vector2(94f, 118f) * 64f, 0, this, 0); //Clam
						break;
					case 8:
						Game1.createItemDebris(ItemRegistry.Create("(O)723"), new Vector2(95f, 117f) * 64f, 0, this, 0); //Oyster
						break;
					case 9:
						Game1.createItemDebris(ItemRegistry.Create("(O)718"), new Vector2(96f, 115f) * 64f, 0, this, 0); //Cockle
						break;
					case 10:
						Game1.createItemDebris(ItemRegistry.Create("(O)719"), new Vector2(98f, 115f) * 64f, 0, this, 0); //Mussel
						break;
					case 11:
						Game1.createItemDebris(ItemRegistry.Create("(O)80"), new Vector2(95f, 113f) * 64f, 0, this, 0); //Quartz
						break;
					case 12:
						Game1.createItemDebris(ItemRegistry.Create("(O)100"), new Vector2(94f, 113f) * 64f, 0, this, 0); //Amphora
						break;
				}
			}

			else if (action == "RS.MermaidStore")
			{
				

            }
				return base.performAction(action, who, tileLocation);
        }



        public bool carpentersk(Location tileLocation)
        {
            foreach (NPC i in this.characters)
            {
                if (!i.Name.Equals("MermaidRangerAnabelle"))
                {
                    continue;
                }
               // if (Vector2.Distance(i.Tile, new Vector2(tileLocation.X, tileLocation.Y)) > 3f)
                //{
                 //   return false;
               // }
                //i.faceDirection(2);
                if ( !Game1.IsThereABuildingUnderConstruction())
                {
                    List<Response> options;
                    options = new List<Response>();
                    options.Add(new Response("Shop", Game1.content.LoadString("Strings\\Locations:Anabelle_Shop")));
                   
                    options.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:Anabelle_Construct")));
                    options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));
                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Anabelle_Greeting"), options.ToArray(), "Anabelle");
                }
                else
                {
                    Utility.TryOpenShopMenu("NuclearBomb.BikiniAtoll", "MermaidRangerAnabelle");
                }
                return true;
            }
			/*
            if (this.getCharacterFromName("Robin") == null && Game1.IsVisitingIslandToday("Robin"))
            {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_MoneyBox"));
                Game1.afterDialogues = delegate
                {
                    Utility.OpenShopMenu("Carpenter", (string)null);
                };
                return true;
            } */
           
            return false;
        }

        public override bool answerDialogue(Response answer)
        {


			if (lastQuestionKey != null && afterQuestion == null)
			{
				string qa = lastQuestionKey.Split(' ')[0] + "_" + answer.responseKey;
				switch (qa)
				{
					

                    case "Anabelle_Construct":
                        this.ShowConstructOptions("MermaidRangerAnabelle");
                        break;

                    case "Anabelle_Shop":
                        Game1.player.forceCanMove();
                        Utility.TryOpenShopMenu("NuclearBomb.BikiniAtoll", "MermaidRangerAnabelle");
                        break;

                    case "MariSwim_Yes":

                        Event MariSwimEvent = new Event((Game1.content.LoadString("Data\\Events\\Custom_ClairabelleLagoon:PrismaticPodSwimEvent", ArgUtility.EscapeQuotes(Game1.player.Name))));
                        this.startEvent(MariSwimEvent);

                        return true;
                }
			}

                return base.answerDialogue(answer);
        }
        
        public override bool SeedsIgnoreSeasonsHere()
        {
            return true;
        }

        public override bool CanPlantSeedsHere(string itemId, int tileX, int tileY, bool isGardenPot, out string deniedMessage)
        {
			deniedMessage = string.Empty;
            return true;
        }

        public override bool CanPlantTreesHere(string itemId, int tileX, int tileY, out string deniedMessage)
        {
			deniedMessage = string.Empty;
            return true;
        }

        public override void tryToAddCritters(bool onlyIfOnScreen = false)
        {
            if (Game1.random.NextDouble() < 0.3)
            {
                Vector2 origin2 = Vector2.Zero;
                origin2 = ((Game1.random.NextDouble() < 0.75) ? new Vector2((float)Game1.viewport.X + Utility.RandomFloat(0f, Game1.viewport.Width), Game1.viewport.Y - 64) : new Vector2(Game1.viewport.X + Game1.viewport.Width + 64, Utility.RandomFloat(0f, Game1.viewport.Height)));
                int parrots_to_spawn = 1;
                if (Game1.random.NextDouble() < 0.5)
                {
                    parrots_to_spawn++;
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    parrots_to_spawn++;
                }
                for (int i = 0; i < parrots_to_spawn; i++)
                {
                    addCritter(new OverheadParrot(origin2 + new Vector2(i * 64, -i * 64)));
                }
            }
            if (!Game1.IsRainingHere(this))
            {
                double mapArea = map.Layers[0].LayerWidth * map.Layers[0].LayerHeight;
                double butterflyChance = Math.Max(0.4, Math.Min(0.25, mapArea / 15000.0));
                addButterflies(butterflyChance, onlyIfOnScreen);
            }
            if (Game1.IsRainingHere(this))
            {
                double mapArea = map.Layers[0].LayerWidth * map.Layers[0].LayerHeight;
                double butterflyChance = Math.Max(0.5, Math.Min(0.25, mapArea / 1500.0));
                addButterflies(butterflyChance, onlyIfOnScreen);
            }
			if (Game1.IsRainingHere(this))
			{
				double mapArea = map.Layers[0].LayerWidth * map.Layers[0].LayerHeight;
				//double frogChance = Math.Max(0.5, Math.Min(0.25, mapArea / 1500.0));
				addFrog();
			}
			if (!Game1.IsRainingHere(this))
			{
				double mapArea = map.Layers[0].LayerWidth * map.Layers[0].LayerHeight;
				double butterflyChance = Math.Max(0.7, Math.Min(0.25, mapArea / 15000.0));
				addBirdies(butterflyChance, onlyIfOnScreen);
			}
			if (Game1.currentSeason == "winter" && Game1.isDarkOut(this))
			{
				addMoonlightJellies(50, new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame - 24917), new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0));
			}

           // double num = map.Layers[0].LayerWidth * map.Layers[0].LayerHeight;
           // double chance2;
            //double chance;
            //double num2 = (chance2 = (chance = Math.Max(0.15, Math.Min(0.5, num / 15000.0))));

			double groovery = .4;


            double chance3 = groovery / 2.0;
            double chance4 = groovery / 2.0;
            double chance5 = groovery ;
            //double num3 = num2 * 2.0;

            if (critters.Count <= 200)
            {
                
                
                addBunnies(chance3, onlyIfOnScreen);
                addSquirrels(chance4, onlyIfOnScreen);
                addWoodpecker(chance5, onlyIfOnScreen = false);
                if (Game1.isDarkOut(this) && Game1.random.NextDouble() < 0.01)
                {
                    addOwl();
                }
            }

        }

        public override void UpdateWhenCurrentLocation(GameTime time)
		{
			GameLocation location = new GameLocation();
			base.UpdateWhenCurrentLocation(time);
			foreach (SuspensionBridge suspensionBridge in suspensionBridges)
			{
				suspensionBridge.Update(time);
			}
			
			bool should_wave = false; 
			
				foreach (Farmer farmer in farmers)
				{
					Point point = farmer.TilePoint;
					if (point.X > 96 && point.Y > 114 || point.X < 90 && point.Y < 115)
					{
						should_wave = true;
						break;
					}
				}

			hissTime += (float)time.ElapsedGameTime.TotalSeconds;
			if (hissTime > 0f)
				{
				location.netAudio.StartPlaying("fuse");
				}
			if (hissTime < 0f)
				{
				location.netAudio.StopPlaying("fuse");
				}


			if (should_wave && (currentMermaidAnimation == null || currentMermaidAnimation == mermaidIdle))
			{
				currentMermaidAnimation = mermaidWave;
				mermaidFrameIndex = 0;
				mermaidFrameTimer = 0f;
			}
			if (mermaidDanceTime > 0f)
			{
				if (currentMermaidAnimation == null || currentMermaidAnimation == mermaidIdle)
				{
					currentMermaidAnimation = mermaidDance;
					mermaidFrameTimer = 0f;
				}
				mermaidDanceTime -= (float)time.ElapsedGameTime.TotalSeconds;
				if (mermaidDanceTime < 0f && currentMermaidAnimation == mermaidDance)
				{
					currentMermaidAnimation = mermaidIdle;
					mermaidFrameTimer = 0f;
				}
			}
			if (mermaidRewardTime > 0f)
			{
				if (currentMermaidAnimation == null || currentMermaidAnimation == mermaidIdle)
				{
					currentMermaidAnimation = mermaidReward;
					mermaidFrameTimer = 0f;
				}
				mermaidRewardTime -= (float)time.ElapsedGameTime.TotalSeconds;
				if (mermaidRewardTime < 0f && currentMermaidAnimation == mermaidReward)
				{
					currentMermaidAnimation = mermaidIdle;
					mermaidFrameTimer = 0f;
				}
			}
			if (mermaidWaveTime > 0f)
			{
				if (currentMermaidAnimation == null || currentMermaidAnimation == mermaidIdle)
				{
					currentMermaidAnimation = mermaidWave;
					mermaidFrameTimer = 0f;
				}
				mermaidWaveTime -= (float)time.ElapsedGameTime.TotalSeconds;
				if (mermaidWaveTime < 0f && currentMermaidAnimation == mermaidWave)
				{
					currentMermaidAnimation = mermaidIdle;
					mermaidFrameTimer = 0f;
				}
			}
			if (mermaidLongDanceTime > 0f)
			{
				if (currentMermaidAnimation == null || currentMermaidAnimation == mermaidIdle)
				{
					currentMermaidAnimation = mermaidLongDance;
					mermaidFrameTimer = 0f;
				}
				mermaidLongDanceTime -= (float)time.ElapsedGameTime.TotalSeconds;
				if (mermaidLongDanceTime < 0f && currentMermaidAnimation == mermaidLongDance)
				{
					currentMermaidAnimation = mermaidIdle;
					mermaidFrameTimer = 0f;
				}
			}
			if (mermaidChillTime > 0f)
			{
				if (currentMermaidAnimation == null || currentMermaidAnimation == mermaidIdle)
				{
					currentMermaidAnimation = mermaidLongDance;
					mermaidFrameTimer = 0f;
				}
				mermaidChillTime -= (float)time.ElapsedGameTime.TotalSeconds;
				if (mermaidChillTime < 0f && currentMermaidAnimation == mermaidChill)
				{
					currentMermaidAnimation = mermaidIdle;
					mermaidFrameTimer = 0f;
				}
			}

			mermaidFrameTimer += (float)time.ElapsedGameTime.TotalSeconds;
			if (!(mermaidFrameTimer > 0.25f))
			{
				return;
			}
			mermaidFrameTimer = 0f;
			mermaidFrameIndex++;
			if (currentMermaidAnimation == null)
			{
				mermaidFrameIndex = 0;
			}

			else
			{
				if (mermaidFrameIndex < currentMermaidAnimation.Length)
				{
					return;
				}
				mermaidFrameIndex = 0;
				if (currentMermaidAnimation == mermaidReward)
				{
					if (should_wave)
					{
						currentMermaidAnimation = mermaidWave;
					}
					else
					{
						currentMermaidAnimation = mermaidIdle;
					}
				}
				else if (!should_wave && currentMermaidAnimation == mermaidWave)
				{
					currentMermaidAnimation = mermaidIdle;
				}

			}

			}

		public bool MermaidIsHere()
		{
			return Game1.IsRainingHere(this);
		}

		public override void draw(SpriteBatch b)
		{

;
			base.draw(b);


			foreach (SuspensionBridge suspensionBridge in suspensionBridges)
			{
				suspensionBridge.Draw(b);
			}

			//if (MermaidIsHere())
			{
				int frame = 0;
				if (currentMermaidAnimation != null && mermaidFrameIndex < currentMermaidAnimation.Length)
				{
					frame = currentMermaidAnimation[mermaidFrameIndex];
				}
				b.Draw(mermaidSprites, Game1.GlobalToLocal(new Vector2(15f, 23f) * 64f + new Vector2(0f, -8f) * 4f), new Microsoft.Xna.Framework.Rectangle(304 + 28 * frame, 592, 28, 36), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0009f);
			}
			
			
		}

        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            switch (base.getTileIndexAt(tileLocation, "Buildings"))
            {
                
                case 958:
                case 1080:
                case 1081:
                    base.ShowMineCartMenu("Default","Custom_ClairabelleLagoon");
                    return true;
            }
			if (Game1.player.eventsSeen.Contains("NuclearMarisol14Heart"))

			{
                switch (base.getTileIndexAt(tileLocation, "Buildings"))
                {
                    case 1120:

                        createQuestionDialogue(SwimQuestion, createYesNoResponses(), "MariSwim");

                        return true;
                }

            }



                return base.checkAction(tileLocation, viewport, who);
        }


        public void farmerAdjacentAction(GameLocation location)
        {
            if (name.Equals("Flute Block") && (internalSound == null || ((int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds - lastNoteBlockSoundTime >= 1000 && !internalSound.IsPlaying)) && !Game1.dialogueUp)
            {
                if (Game1.soundBank != null)
                {
                    internalSound = Game1.soundBank.GetCue("flute");
                    internalSound.SetVariable("Pitch", preservedParentSheetIndex.Value);
                    internalSound.Play();
                }
                scale.Y = 1.3f;
                shakeTimer = 200;
                lastNoteBlockSoundTime = (int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
                if (location is ClairabelleLagoon)
                {
                    (location as ClairabelleLagoon).OnFlutePlayed(preservedParentSheetIndex.Value);
                }
            }

        }


        public virtual void OnFlutePlayed(int pitch)
		{
			
			if (songIndex == -1)
			{
				lastPlayedNote = pitch;
				songIndex = 0;
			}
			int relative_pitch = pitch - lastPlayedNote;
			if (relative_pitch == 900)
			{
				songIndex = 1;
				mermaidDanceTime = 5f;
			}
			else if (songIndex == 1)
			{
				if (relative_pitch == -200)
				{
					songIndex++;
					mermaidDanceTime = 5f;
				}
				else
				{
					songIndex = -1;
					mermaidDanceTime = 0f;
					currentMermaidAnimation = mermaidIdle;
				}
			}
			else if (songIndex == 2)
			{
				if (relative_pitch == -400)
				{
					songIndex++;
					mermaidDanceTime = 5f;
				}
				else
				{
					songIndex = -1;
					mermaidDanceTime = 0f;
					currentMermaidAnimation = mermaidIdle;
				}
			}
			else if (songIndex == 3)
			{
				if (relative_pitch == 200)
				{
					songIndex = 0;
					sapphiremermaidPuzzleSuccess.Fire();
					mermaidDanceTime = 0f;
				}
				else
				{
					songIndex = -1;
					mermaidDanceTime = 0f;

					currentMermaidAnimation = mermaidIdle;
				}
			}
			lastPlayedNote = pitch;
		}
		public override void checkForMusic(GameTime time)
		{
			if (base.IsOutdoors && Game1.isMusicContextActiveButNotPlaying() && !Game1.IsRainingHere(this) && !Game1.eventUp)
			{
				if (Game1.random.NextDouble() < 0.003 && Game1.timeOfDay < 2100)
				{
					localSound("seagulls");
				}
				else if (Game1.isDarkOut(this) && Game1.timeOfDay < 2500)
				{
					Game1.changeMusicTrack("spring_night_ambient", track_interruptable: true);
				}
			}

			base.checkForMusic(time);
		}
		public override void cleanupBeforePlayerExit()
		{
			base.cleanupBeforePlayerExit();
			
			
		}

	}
}