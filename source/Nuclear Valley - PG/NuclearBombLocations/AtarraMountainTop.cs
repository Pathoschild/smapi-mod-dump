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
using StardewValley.Monsters;
using System.Drawing;

namespace NuclearBombLocations
{
    [XmlType("Mods_ApryllForever_NuclearBombLocations_AtarraMountainTop")]
    public class AtarraMountainTop : NuclearLocation
    {
        private List<WeatherDebris> weatherDebris;

        private Texture2D submarineSprites;

        static IModHelper Helper;

		public static IMonitor Monitor;

        [XmlElement("addedMermaidSquidsToday")]
        private readonly NetBool addedMermaidSquidsToday = new NetBool();

        [XmlElement("addedMermaidGhostiesToday")]
        private readonly NetBool addedMermaidGhostiesToday = new NetBool();

        [XmlElement("addedMermaidCGhostiesToday")]
        private readonly NetBool addedMermaidCGhostiesToday = new NetBool();

        [XmlElement("addedMermaidPGhostiesToday")]
        private readonly NetBool addedMermaidPGhostiesToday = new NetBool();

        [XmlElement("addedMermaidZombieToday")]
        private readonly NetBool addedMermaidZombieToday = new NetBool();

        [XmlElement("addedMermaidLavaCrabsToday")]
        private readonly NetBool addedMermaidLavaCrabsToday = new NetBool();

        [XmlElement("addedMermaidIridiumCrabsToday")]
        private readonly NetBool addedMermaidIridiumCrabsToday = new NetBool();

        [XmlElement("addedMermaidSerpentToday")]
        private readonly NetBool addedMermaidSerpentToday = new NetBool();

        [XmlIgnore]
		public List<SuspensionBridge> suspensionBridges = new List<SuspensionBridge>();

		private readonly NetEvent1Field<int, NetInt> rumbleAndFadeEvent = new NetEvent1Field<int, NetInt>();

		public static string returnsub = "Do you wish to return to the submarine?";

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
            AtarraMountainTop.Helper = Helper;
			//Helper.Events.GameLoop.DayStarted += OnDayStarted;
		}

		public AtarraMountainTop() { }

		

		public AtarraMountainTop(IModContentHelper content)
        : base(content, "AtarraMountainTop", "AtarraMountainTop")
        {
            if (!Game1.IsMultiplayer)
            {
                base.ExtraMillisecondsPerInGameMinute = 1000;
            }

        }

		

		protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddField(rumbleAndFadeEvent);
            base.NetFields.AddField(this.addedMermaidSquidsToday);
            base.NetFields.AddField(this.addedMermaidGhostiesToday);
            base.NetFields.AddField(this.addedMermaidCGhostiesToday);
            base.NetFields.AddField(this.addedMermaidPGhostiesToday);
            base.NetFields.AddField(this.addedMermaidLavaCrabsToday);
            base.NetFields.AddField(this.addedMermaidIridiumCrabsToday);
            base.NetFields.AddField(this.addedMermaidZombieToday);
            base.NetFields.AddField(this.addedMermaidSerpentToday);

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
        //static string EnterDungeon = "Do you wish to enter this forboding gate?";
        protected override void resetLocalState()
        {
			//this.seasonOverride = "spring";
			base.resetLocalState();
            this.weatherDebris?.Clear();
            this.submarineSprites = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
            Game1.changeMusicTrack("junimoKart_whaleMusic");

            mermaidSprites = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");

			if (Game1.player.Tile.Y == 1)
			{
                if (Game1.player.Tile.X == 1)
                {
					Game1.player.Position = new Vector2(25.5f * Game1.tileSize, 19 * Game1.tileSize);
				}
			}


		
			{
				Microsoft.Xna.Framework.Point offset = new Microsoft.Xna.Framework.Point(0, 0); 
				Vector2 vector_offset = new Vector2(offset.X, offset.Y);
			
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(33f, 10f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(4, (new Vector2(34f, 10f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
				Game1.currentLightSources.Add(new LightSource(5, (new Vector2(40f, 16f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
                Game1.currentLightSources.Add(new LightSource(1, (new Vector2(37f, 16f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
                Game1.currentLightSources.Add(new LightSource(1, (new Vector2(43f, 16f) + vector_offset) * 64f, 1f, LightSource.LightContext.None, 0L));
            }  

        }

		public override void TransferDataFromSavedLocation(GameLocation l)
		{
			base.TransferDataFromSavedLocation(l);
		}


        protected override void resetSharedState()
        {
            base.resetSharedState();
            if ((bool)this.addedMermaidSquidsToday.Value && (bool)this.addedMermaidGhostiesToday.Value 
				&& (bool)this.addedMermaidCGhostiesToday.Value && (bool)this.addedMermaidPGhostiesToday.Value 
				&& (bool)this.addedMermaidZombieToday.Value && (bool)this.addedMermaidLavaCrabsToday.Value 
				&& (bool)this.addedMermaidIridiumCrabsToday.Value && (bool)this.addedMermaidSerpentToday.Value)
            {
                return;
            }
            this.addedMermaidSquidsToday.Value = true;
            Random rand;
            rand = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);
            Microsoft.Xna.Framework.Rectangle spawnArea;
            spawnArea = new Microsoft.Xna.Framework.Rectangle(4, 10, 50, 40);
            for (int tries = 34; tries > 0; tries--)
            {
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea, rand);
                if (this.CanItemBePlacedHere(tile))
                {
					
				                 
                    {
                        BlueSquid a;
                        a = new BlueSquid(tile * 64f);
                        base.characters.Add(a);
                        //return;
                    }

                    //GreenSlime i;
                    //i = new GreenSlime(tile * 64f, 0);
                    //i.makeTigerSlime();
                    //base.characters.Add(i);
                }
            }


            if ((bool)this.addedMermaidGhostiesToday.Value && (bool)this.addedMermaidCGhostiesToday.Value
                && (bool)this.addedMermaidPGhostiesToday.Value
                && (bool)this.addedMermaidZombieToday.Value && (bool)this.addedMermaidLavaCrabsToday.Value
                && (bool)this.addedMermaidIridiumCrabsToday.Value && (bool)this.addedMermaidSerpentToday.Value)
            {
                return;
            }
            this.addedMermaidGhostiesToday.Value = true;
            Random rando;
            rando = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);
            Microsoft.Xna.Framework.Rectangle spawnAreaa;
            spawnAreaa = new Microsoft.Xna.Framework.Rectangle(18, 25, 12, 10);
            for (int tries = 6; tries > 0; tries--)
            {
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnAreaa, rando);
                if (this.CanItemBePlacedHere(tile))
                {
                    {
                        Ghost a;
                        a = new Ghost(tile * 64f);
                        a.focusedOnFarmers = false;
                        base.characters.Add(a);
                        //return;
                    } 
                }
            }



            if  ((bool)this.addedMermaidCGhostiesToday.Value && (bool)this.addedMermaidPGhostiesToday.Value
                && (bool)this.addedMermaidZombieToday.Value && (bool)this.addedMermaidLavaCrabsToday.Value
                && (bool)this.addedMermaidIridiumCrabsToday.Value && (bool)this.addedMermaidSerpentToday.Value)
            {
                return;
            }
            this.addedMermaidCGhostiesToday.Value = true;
            Random rand1;
            rand1 = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);
            Microsoft.Xna.Framework.Rectangle spawnArea1;
            spawnArea1 = new Microsoft.Xna.Framework.Rectangle(18, 25, 12, 10);
            for (int tries = 6; tries > 0; tries--)
            {
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea1, rand1);
                if (this.CanItemBePlacedHere(tile))
                {  
                    {

                        Ghost a;
                        a = new Ghost(tile * 64f, "Carbon Ghost");
                        a.focusedOnFarmers = false;
                        base.characters.Add(a);
                    }
                }
            }



            if ( (bool)this.addedMermaidPGhostiesToday.Value
                && (bool)this.addedMermaidZombieToday.Value && (bool)this.addedMermaidLavaCrabsToday.Value
                && (bool)this.addedMermaidIridiumCrabsToday.Value && (bool)this.addedMermaidSerpentToday.Value)
            {
                return;
            }
            this.addedMermaidPGhostiesToday.Value = true;
            Random rand2;
            rand2 = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);
            Microsoft.Xna.Framework.Rectangle spawnArea2;
            spawnArea2 = new Microsoft.Xna.Framework.Rectangle(18, 25, 12, 10);
            for (int tries = 6; tries > 0; tries--)
            {
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea2, rand2);
                if (this.CanItemBePlacedHere(tile))
                {
                    {

                        Ghost a;
                        a = new Ghost(tile * 64f, "Putrid Ghost");
						a.focusedOnFarmers = false;
                        base.characters.Add(a);
                    }
                }
            }

            if ((bool)this.addedMermaidZombieToday.Value && (bool)this.addedMermaidLavaCrabsToday.Value
                && (bool)this.addedMermaidIridiumCrabsToday.Value && (bool)this.addedMermaidSerpentToday.Value)
            {
                return;
            }
            this.addedMermaidZombieToday.Value = true;
           
            rand2 = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);
           
            spawnArea2 = new Microsoft.Xna.Framework.Rectangle(18, 25, 12, 10);
            for (int tries = 37; tries > 0; tries--)
            {
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea2, rand2);
                if (this.CanItemBePlacedHere(tile))
                {
                    {
                        RockGolem a;
                        a = new RockGolem(tile * 64f, 37);
                        base.characters.Add(a);
                    }
                }
            }

            if ( (bool)this.addedMermaidLavaCrabsToday.Value
               && (bool)this.addedMermaidIridiumCrabsToday.Value && (bool)this.addedMermaidSerpentToday.Value)
            {
                return;
            }
            this.addedMermaidLavaCrabsToday.Value = true;

            rand2 = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);
            for (int tries = 24; tries > 0; tries--)
            {
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea, rand);
                if (this.CanItemBePlacedHere(tile))
                {
                    {
                        RockCrab a;
                        a = new RockCrab(tile * 64f, "Lava Crab");
                        base.characters.Add(a);
                        //return;
                    }
                }
            }


            if ((bool)this.addedMermaidIridiumCrabsToday.Value && (bool)this.addedMermaidSerpentToday.Value)
            {
                return;
            }
            this.addedMermaidIridiumCrabsToday.Value = true;

            rand2 = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);
            for (int tries = 14; tries > 0; tries--)
            {
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea, rand);
                if (this.CanItemBePlacedHere(tile))
                {
                    {
                        RockCrab a;
                        a = new RockCrab(tile * 64f, "Iridium Crab");
                        base.characters.Add(a);
                        //return;
                    }
                }
            }

            if ((bool)this.addedMermaidSerpentToday.Value)
            {
                return;
            }
            this.addedMermaidSerpentToday.Value = true;

            rand2 = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);
            for (int tries = 14; tries > 0; tries--)
            {
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea, rand);
                if (this.CanItemBePlacedHere(tile))
                {
                    {
                        Serpent a;
                        a = new Serpent(tile * 64f);
                        base.characters.Add(a);
                        //return;
                    }
                }
            }







        }


        public override void DayUpdate(int dayOfMonth)
        {
            base.DayUpdate(dayOfMonth);

            for (int j = 0; j < base.characters.Count; j++)
            {
                if (base.characters[j] is Monster)
                {
                    base.characters.RemoveAt(j);
                    j--;
                }
            }
            this.addedMermaidSquidsToday.Value = false;
            this.addedMermaidGhostiesToday.Value = false;
            this.addedMermaidCGhostiesToday.Value = false;
            this.addedMermaidPGhostiesToday.Value = false;
            this.addedMermaidZombieToday.Value = false;
            this.addedMermaidLavaCrabsToday.Value = false;
            this.addedMermaidIridiumCrabsToday.Value = false;
            this.addedMermaidSerpentToday.Value = false;

            Microsoft.Xna.Framework.Rectangle tidePools;
            tidePools = new Microsoft.Xna.Framework.Rectangle(4, 5, 48, 48);
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
				else if (Game1.random.NextDouble() < 0.37)
				{

                    Vector2 position2;
                    position2 = new Vector2(Game1.random.Next(seaweedShore.X, seaweedShore.Right), Game1.random.Next(seaweedShore.Y, seaweedShore.Bottom));
                    if (this.CanItemBePlacedHere(position2))
                    {
                        this.dropObject(ItemRegistry.Create<Object>("(O)169"), position2 * 64f, Game1.viewport, initialPlacement: true);
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

			chance = 0.13f;
            while (Game1.random.NextDouble() < (double)chance)
            {
                string id2;
                id2 = ((Game1.random.NextDouble() < 0.2) ? "(O)797" : "(O)393");
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





       
		private void rumbleAndFade(int milliseconds)
		{
			rumbleAndFadeEvent.Fire(milliseconds);
		}

		private void performRumbleAndFade(int milliseconds)
		{
			if (Game1.currentLocation == this)
			{
				Rumble.rumbleAndFade(1f, milliseconds);
			}
		}


        public override void performTenMinuteUpdate(int timeOfDay)
        {
            base.performTenMinuteUpdate(timeOfDay);
            if (!(Game1.random.NextDouble() < 0.3))
            {
                return;
            }
            int numsprites;
            numsprites = 0;
            foreach (NPC character in base.characters)
            {
                if (character is Serpent)
                {
                    numsprites++;
                }
            }
            if (numsprites < base.farmers.Count * 13)
            {
                this.spawnFlyingMonsterOffScreen();
            }
        }

        public void spawnFlyingMonsterOffScreen()
        {
            Vector2 spawnLocation;
            spawnLocation = Vector2.Zero;
            switch (Game1.random.Next(4))
            {
                case 0:
                    spawnLocation.X = Game1.random.Next(base.map.Layers[0].LayerWidth);
                    break;
                case 3:
                    spawnLocation.Y = Game1.random.Next(base.map.Layers[0].LayerHeight);
                    break;
                case 1:
                    spawnLocation.X = base.map.Layers[0].LayerWidth - 1;
                    spawnLocation.Y = Game1.random.Next(base.map.Layers[0].LayerHeight);
                    break;
                case 2:
                    spawnLocation.Y = base.map.Layers[0].LayerHeight - 1;
                    spawnLocation.X = Game1.random.Next(base.map.Layers[0].LayerWidth);
                    break;
            }
            base.playSound("serpentDie");
            base.characters.Add(new Serpent(spawnLocation)
            {
                focusedOnFarmers = true
            });
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
				Game1.screenOverlayTempSprites.AddRange(Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), 500, Microsoft.Xna.Framework.Color.White, 10, 2000));
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
				Game1.screenOverlayTempSprites.AddRange(Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), 500, Microsoft.Xna.Framework.Color.White, 10, 2000));
				currentMermaidAnimation = mermaidIdle;
				Game1.createItemDebris (ItemRegistry.Create("(O)638"), new Vector2(32f, 33f) * 64f, 0, this, 0);
				Game1.freezeControls = false;

               


            }
			

			else if (action == "RS.MermaidStore")
			{
				

            }
				return base.performAction(action, who, tileLocation);
        }






        public override bool answerDialogue(Response answer)
        {


			if (lastQuestionKey != null && afterQuestion == null)
			{
				string qa = lastQuestionKey.Split(' ')[0] + "_" + answer.responseKey;
				switch (qa)
				{
					

                    case "ReturnSub_Yes":
                        performTouchAction("Warp " + "Custom_NuclearSubmarinePen 27 19", Game1.player.Tile);
                        break;

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
            return false;
        }

        public override bool CanPlantTreesHere(string itemId, int tileX, int tileY, out string deniedMessage)
        {
			deniedMessage = string.Empty;
            return false;
        }



        public override void UpdateWhenCurrentLocation(GameTime time)
		{
			GameLocation location = new GameLocation();
			base.UpdateWhenCurrentLocation(time);
			foreach (SuspensionBridge suspensionBridge in suspensionBridges)
			{
				suspensionBridge.Update(time);
			}
            /*
            if (Game1.random.NextDouble() < 0.9)
            {
                Vector2 pos5;
                pos5 = new Vector2(Game1.random.Next(12, base.map.DisplayWidth - 64), ( 64 * 60));
                int which3;
                which3 = Game1.random.Next(3);
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite
                {
                    motion = new Vector2(0f, (-3f + (float)which3)),
                    yStopCoordinate = (60 * 64),
                    texture = this.submarineSprites,
                    sourceRect = new Microsoft.Xna.Framework.Rectangle(132 + which3 * 8, 20, 8, 8),
                    xPeriodic = true,
                    xPeriodicLoopTime = 1500f,
                    xPeriodicRange = 12f,
                    initialPosition = pos5,
                    animationLength = 1,
                    interval = 5000f,
                    position = pos5,
                    scale = 4f,
                    
                    
                });
            }


            if (Game1.random.NextDouble() < 0.9)
            {
                Vector2 pos4;
                pos4 = new Vector2(Game1.random.Next(36, base.map.DisplayWidth - 256), (64 * 60));
                int which3;
                which3 = Game1.random.Next(3);
                this.temporarySprites.Add(new TemporaryAnimatedSprite
                {
                    motion = new Vector2(0f, (-3f + (float)which3)),
                    yStopCoordinate = (60 * 64),
                    texture = this.submarineSprites,
                    sourceRect = new Microsoft.Xna.Framework.Rectangle(132 + which3 * 8, 20, 8, 8),
                    xPeriodic = true,
                    xPeriodicLoopTime = 1500f,
                    xPeriodicRange = 12f,
                    initialPosition = pos4,
                    animationLength = 1,
                    interval = 18000f,
                    position = pos4,
                    scale = 4f
                });
            } */

            if (Game1.random.NextDouble() < 0.5)
            {
                Vector2 pos4;
                pos4 = new Vector2(Game1.random.Next(0, base.map.DisplayWidth - 64), 3800f);
                int which2;
                which2 = Game1.random.Next(3);
                this.temporarySprites.Add(new TemporaryAnimatedSprite
                {
                    motion = new Vector2(0f, -1f + (float)which2 * 0.2f),
                    yStopCoordinate = 33000,
                    texture = this.submarineSprites,
                    sourceRect = new Microsoft.Xna.Framework.Rectangle(132 + which2 * 8, 20, 8, 8),
                    animationLength = 1,
                    interval = 200000f,
                    xPeriodic = true,
                    xPeriodicLoopTime = 1500f,
                    xPeriodicRange = 12f,
                    initialPosition = pos4,
                    position = pos4,
                    scale = 2f
                });
            }

            bool should_wave = false; 
			
				foreach (Farmer farmer in farmers)
				{
					Microsoft.Xna.Framework.Point point = farmer.TilePoint;
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
            /*
			//if (MermaidIsHere())
			{
				int frame = 0;
				if (currentMermaidAnimation != null && mermaidFrameIndex < currentMermaidAnimation.Length)
				{
					frame = currentMermaidAnimation[mermaidFrameIndex];
				}
				b.Draw(mermaidSprites, Game1.GlobalToLocal(new Vector2(15f, 23f) * 64f + new Vector2(0f, -8f) * 4f), new Microsoft.Xna.Framework.Rectangle(304 + 28 * frame, 592, 28, 36), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0009f);
			}
			*/
			
		}

        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            switch (base.getTileIndexAt(tileLocation, "Buildings"))
            {
                
                
                case 1975:
                    createQuestionDialogue(returnsub, createYesNoResponses(), "ReturnSub");
                    return true;
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
                if (location is AtarraMountainTop)
                {
                    (location as AtarraMountainTop).OnFlutePlayed(preservedParentSheetIndex.Value);
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
				if (Game1.random.NextDouble() < 0.003)
				{
					localSound("bubbles");
				}
				else if (Game1.random.NextDouble() < 0.03)
				{
					Game1.changeMusicTrack("waterSlosh", track_interruptable: true);
				}
			}

			base.checkForMusic(time);
		}
        /*
        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            int x = this.map.GetLayer("AlwaysFront").LayerHeight;
            int y = this.map.GetLayer("AlwaysFront").LayerWidth;

            base.drawAboveAlwaysFrontLayer(b);
            //bool num = y == this.map.GetLayer("AlwaysFront2").LayerHeight - 1;
          //  bool flag = y == 0;
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(0, 0 )), new Microsoft.Xna.Framework.Rectangle(0 * 64, 2064, 64, 64), Microsoft.Xna.Framework.Color.BlueViolet, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.56f);
            //if (num)
            {
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y + 1) * 64 - (int)waterPosition)), new Microsoft.Xna.Framework.Rectangle(waterAnimationIndex * 64, 2064 + (((x + (y + 1)) % 2 != 0) ? ((!waterTileFlip) ? 128 : 0) : (waterTileFlip ? 128 : 0)), 64, 64 - (int)(64f - waterPosition) - 1), Microsoft.Xna.Framework.Color.BlueViolet, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.56f);
            }
        }*/


        public override void cleanupBeforePlayerExit()
		{
			base.cleanupBeforePlayerExit();
			
			
		}

	}
}