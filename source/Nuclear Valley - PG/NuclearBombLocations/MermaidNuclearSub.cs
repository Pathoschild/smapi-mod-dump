/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ApryllForever/NuclearBombLocations
**
*************************************************/

using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley.Tools;
using xTile.Dimensions;
using StardewValley.Locations;
using StardewValley;
using xTile.Layers;

namespace NuclearBombLocations
{
    [XmlType("Mods_ApryllForever_RestStopLocations_MermaidNuclearSub")]
    public class MermaidNuclearSub : NuclearLocation
	{
		public const float submergeTime = 20000f;

		[XmlElement("submerged")]
		public readonly NetBool submerged = new NetBool();

		[XmlElement("ascending")]
		public readonly NetBool ascending = new NetBool();

		private Texture2D submarineSprites;

		private float curtainMovement;

		private float curtainOpenPercent;

		private float submergeTimer;

		private Color ambientLightTargetColor;

		private bool hasLitSubmergeLight;

		private bool hasLitAscendLight;

		private bool doneUntilReset;

		private bool localAscending;

		public MermaidNuclearSub()
		{
		}

		public MermaidNuclearSub(IModContentHelper content)
        : base(content, "MermaidNuclearSub", "MermaidNuclearSub")
        {
		}

		protected override void initNetFields()
		{
			base.initNetFields();
			base.NetFields.AddField(this.submerged, "submerged").AddField(this.ascending, "ascending");
		}

		public override void draw(SpriteBatch b)
		{
			base.draw(b);

			// I think this is the doors and the light

			//b.Draw(this.submarineSprites, Game1.GlobalToLocal(new Vector2(9f, 7f) * 64f) + new Vector2(0f, -2f) * 4f, new Microsoft.Xna.Framework.Rectangle((int)(257f + 100f * this.curtainOpenPercent), 0, (int)(100f * (1f - this.curtainOpenPercent)), 80), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
			//b.Draw(this.submarineSprites, Game1.GlobalToLocal(new Vector2(15f, 7f) * 64f + new Vector2(-3f, -2f) * 4f + new Vector2(100f * this.curtainOpenPercent, 0f) * 4f), new Microsoft.Xna.Framework.Rectangle(357, 0, (int)(100f * (1f - this.curtainOpenPercent)), 80), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
			//b.Draw(this.submarineSprites, Game1.GlobalToLocal(new Vector2(82f, 123f) * 4f + new Vector2(0f, (this.submerged.Value && !this.doneUntilReset) ? (104f * (1f - this.submergeTimer / 20000f)) : 0f)), new Microsoft.Xna.Framework.Rectangle(457, 0, 9, 4), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
		
		
		
		
		
		
		}

		public override void DayUpdate(int dayOfMonth)
		{
			base.DayUpdate(dayOfMonth);
			this.hasLitSubmergeLight = false;
			this.curtainOpenPercent = 0f;
			this.curtainMovement = 0f;
			this.submergeTimer = 0f;
			this.submerged.Value = false;
			this.hasLitAscendLight = false;
			this.doneUntilReset = false;
			if ((bool)this.submerged)
			{
				this.submerged.Value = false;
			}
			if ((bool)this.ascending)
			{
				this.ascending.Value = false;
			}
			Game1.netWorldState.Value.IsSubmarineLocked = false;
		}

		public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
		{
			if (base.getTileIndexAt(tileLocation, "Buildings") == 645) //Captain
			{
				if (this.doneUntilReset)
				{
					Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MermaidNuclearSubmarine_Done"));
					return false;
				}
				if (!this.submerged.Value)
				{
					base.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:MermaidNuclearSubmarine_SubmergeQuestion"), base.createYesNoResponses(), "SubmergeQuestion");
				}
				else if (this.submergeTimer <= 0f && this.curtainOpenPercent >= 1f)
				{
					base.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:MermaidNuclearSubmarine_AscendQuestion"), base.createYesNoResponses(), "AscendQuestion");
				}
				return true;
			}
            if (base.getTileIndexAt(tileLocation, "Buildings") == 941) //Exit to Shore
            {
                if (this.doneUntilReset || !this.submerged.Value)
                {
                    base.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:MermaidNuclearSubmarine_Leave"), base.createYesNoResponses(), "MermaidNuclearSubmarineLeaveQuestion");
                    return false;
                }
                if (this.submerged.Value)
                {
                    //Cannot exit to shore
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MermaidNuclearSubmarine_CannotLeave"));
                }
                return true;
            }
            if (base.getTileIndexAt(tileLocation, "Buildings") == 425) //Exit to Ocean Mountain Top
            {
                if (this.doneUntilReset)
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AtarraMountainSubmarine_Done"));
                    return false;
                }
                if (this.submerged.Value)
                {
                    base.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:AtarraMountains_Exit"), base.createYesNoResponses(), "AtarraExit");
                }
                else if (this.submergeTimer >= 0f && this.curtainOpenPercent <= 1f)
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AtarraMountains_NotYet"));
                }
                return true;
            }
            if (base.getTileIndexAt(tileLocation, "Buildings") == 703) //Sonar Ping
			{
				Game1.playSound("ApryllForever.NuclearBomb_SonarPing");

                return true;

            }
            if (base.getTileIndexAt(tileLocation, "Buildings") == 663) //Sonar Ping
            {
                Game1.playSound("ApryllForever.NuclearBomb_SonarPing");

                return true;

            }


            return base.checkAction(tileLocation, viewport, who);
		}

		public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
		{
			if (questionAndAnswer == null)
			{
				return false;
			}
			if (!(questionAndAnswer == "SubmergeQuestion_Yes"))
			{
				if (questionAndAnswer == "AscendQuestion_Yes")
				{
		
					this.ascending.Value = true;
					this.localAscending = true;
				}
			}
			if (questionAndAnswer == "SubmergeQuestion_Yes" && Game1.player.Money < 1000)
			{
				Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
			}
			if (questionAndAnswer == "SubmergeQuestion_Yes")
            {
				Game1.player.Money -= 1000;
				this.submerged.Value = true;
				Game1.netWorldState.Value.IsSubmarineLocked = true;

			}
			if (questionAndAnswer == "MermaidNuclearSubmarineLeaveQuestion_Yes")
			{
                performTouchAction("Warp " + "Custom_NuclearSubmarinePen 28 19", Game1.player.Tile);
            }
            if (questionAndAnswer == "AtarraExit_Yes")
            {
                performTouchAction("Warp " + "Custom_AtarraMountainTop 36 10", Game1.player.Tile);
            }



            return base.answerDialogueAction(questionAndAnswer, questionParams);
		}

		private void changeSubmergeLight(bool red, bool clear = false)
		{


			/*
			if (clear)
			{
				base.setMapTileIndex(3, 4, 98, "Buildings");
				base.setMapTileIndex(4, 4, 99, "Buildings");
				base.setMapTileIndex(3, 5, 122, "Buildings");
				base.setMapTileIndex(4, 5, 123, "Buildings");
			}
			else if (red)
			{
				base.setMapTileIndex(3, 4, 425, "Buildings");
				base.setMapTileIndex(4, 4, 426, "Buildings");
				base.setMapTileIndex(3, 5, 449, "Buildings");
				base.setMapTileIndex(4, 5, 450, "Buildings");
			}
			else
			{
				base.setMapTileIndex(3, 4, 427, "Buildings");
				base.setMapTileIndex(4, 4, 428, "Buildings");
				base.setMapTileIndex(3, 5, 451, "Buildings");
				base.setMapTileIndex(4, 5, 452, "Buildings");
			}*/
		}

		protected override void resetSharedState()
		{
			base.resetSharedState();
			this.submerged.Value = false;
			this.ascending.Value = false;
			Game1.netWorldState.Value.IsSubmarineLocked = false;
		}

		protected override void resetLocalState()
		{
			base.resetLocalState();
			this.submarineSprites = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
			Game1.ambientLight = Color.Black;
			this.ambientLightTargetColor = Color.Black;
			this.hasLitSubmergeLight = false;
			Game1.background = new Background(this, new Color(0, 50, 255), onlyMapBG: true);
			this.curtainOpenPercent = 0f;
			this.curtainMovement = 0f;
			this.submergeTimer = 0f;
			this.hasLitAscendLight = false;
			this.doneUntilReset = false;
			this.localAscending = false;
		}

		public override bool performAction(string actionStr, Farmer who, xTile.Dimensions.Location tileLocation)
		{
			string[] split = actionStr.Split(' ');
			string action = split[0];
			int tx = tileLocation.X;
			int ty = tileLocation.Y;
			Layer layer = Map.GetLayer("Buildings");


			if (action == "NuclearMermaidSubDive")
			{
				this.answerDialogueAction("SubmergeQuestion_Yes", LegacyShimsEvil.EmptyArray<string>());




			}


            return base.performAction(action, who, tileLocation);
        }


        public override void performTenMinuteUpdate(int timeOfDay)
		{
			base.performTenMinuteUpdate(timeOfDay);
			Random r;
			r = Utility.CreateDaySaveRandom(timeOfDay);
			if (base.fishSplashPoint.Value.Equals(Point.Zero) && r.NextDouble() < 1.0 && this.curtainOpenPercent >= 1f)
			{
				for (int tries = 0; tries < 2; tries++)
				{
					Point p;
					p = new Point(r.Next(9, 21), r.Next(7, 12));
					if (!base.isOpenWater(p.X, p.Y))
					{
						continue;
					}
					int toLand;
					toLand = FishingRod.distanceToLand(p.X, p.Y, this);
					if (toLand > 1 && toLand < 5)
					{
						if (Game1.player.currentLocation.Equals(this))
						{
							base.playSound("waterSlosh");
						}
						base.fishSplashPoint.Value = p;
						break;
					}
				}
			}
			else if (!base.fishSplashPoint.Value.Equals(Point.Zero) && r.NextDouble() < 0.25)
			{
				base.fishSplashPoint.Value = Point.Zero;
			}
		}

		public override void UpdateWhenCurrentLocation(GameTime time)
		{
			base.UpdateWhenCurrentLocation(time);
			if (!Game1.player.currentLocation.Equals(this)) //|| !Game1.shouldTimePass()
            {
				return;
			}
			if (this.curtainMovement != 0f)
			{
				float old;
				old = this.curtainOpenPercent;
				this.curtainOpenPercent = Math.Max(0f, Math.Min(1f, this.curtainOpenPercent + this.curtainMovement * (float)time.ElapsedGameTime.Milliseconds));
				if (this.curtainOpenPercent >= 1f && old < 1f)
				{
					this.curtainMovement = 0f;
					this.changeSubmergeLight(red: false);
					this.ambientLightTargetColor = new Color(200, 150, 100);
					Game1.playSound("newArtifact");
					Game1.changeMusicTrack("submarine_song");
				}
			}
			if (this.submerged.Value && !this.hasLitSubmergeLight)
			{
				this.changeSubmergeLight(red: true);
				DelayedAction.playSoundAfterDelay("cowboy_monsterhit", 200);
				DelayedAction.playSoundAfterDelay("cowboy_monsterhit", 400);

                DelayedAction.playSoundAfterDelay("ApryllForever.NuclearBomb_DiveKlaxon", 1000);

                Game1.changeMusicTrack("Hospital_Ambient");
				this.submergeTimer = 20000f;
				this.hasLitSubmergeLight = true;
				base.ignoreWarps = true;
				base.temporarySprites.Add(new TemporaryAnimatedSprite
				{

					//THis is the Lever Actoin

					/*
					texture = this.submarineSprites,
					sourceRectStartingPos = new Vector2(457f, 11f),
					sourceRect = new Microsoft.Xna.Framework.Rectangle(457, 11, 14, 18),
					initialPosition = new Vector2(21f, 143f) * 4f,
					animationLength = 3,
					pingPong = true,
					position = new Vector2(21f, 143f) * 4f,
					scale = 4f
					*/
				});
			}
			if (this.ascending.Value && !this.hasLitAscendLight)
			{
				this.changeSubmergeLight(red: true);
				DelayedAction.playSoundAfterDelay("cowboy_monsterhit", 200);
				DelayedAction.playSoundAfterDelay("cowboy_monsterhit", 400);
				Game1.changeMusicTrack("Hospital_Ambient");
				this.submergeTimer = 1f;
				this.hasLitAscendLight = true;
				this.curtainMovement = -0.0002f;
				Game1.playSound("submarine_landing");
				base.temporarySprites.Add(new TemporaryAnimatedSprite
				{
					/*
					texture = this.submarineSprites,
					sourceRectStartingPos = new Vector2(457f, 11f),
					sourceRect = new Microsoft.Xna.Framework.Rectangle(457, 11, 14, 18),
					initialPosition = new Vector2(21f, 143f) * 4f,
					animationLength = 3,
					pingPong = true,
					position = new Vector2(21f, 143f) * 4f,
					scale = 4f*/
				});
				if (Game1.IsMasterGame)
				{
					base.fishSplashPoint.Value = Point.Zero;
				}
				if (Game1.activeClickableMenu is BobberBar)
				{
					Game1.activeClickableMenu.emergencyShutDown();
				}
				if (Game1.player.UsingTool && Game1.player.CurrentTool is FishingRod rod)
				{
					rod.doneFishing(Game1.player);
				}
				Game1.player.completelyStopAnimatingOrDoingAction();
				foreach (TemporaryAnimatedSprite tempSprite in Game1.background.tempSprites)
				{
					tempSprite.yStopCoordinate = ((tempSprite.position.X > 320f) ? 320 : 896);
					tempSprite.motion = new Vector2(0f, 2f);
					tempSprite.yPeriodic = false;
				}
			}
			if (this.submergeTimer > 0f)
			{
				if ((bool)this.ascending && !this.localAscending)
				{
					this.localAscending = true;
				}
				this.submergeTimer -= ((!this.localAscending) ? 1 : (-1)) * time.ElapsedGameTime.Milliseconds;
				Game1.background.c.B = (byte)(Math.Max(this.submergeTimer / 20000f, 0.2f) * 255f);
				Game1.background.c.G = (byte)(Math.Max(this.submergeTimer / 20000f, 0f) * 50f);
				if (this.submergeTimer <= 0f)
				{
					this.curtainMovement = 0.0002f;
					Game1.changeMusicTrack("none");
					Game1.playSound("submarine_landing");
					Game1.background.tempSprites.Add(new TemporaryAnimatedSprite    // THIS IS THE PLANTS BACKGROUND!!!
					{
						motion = new Vector2(0f, -1f),
						yStopCoordinate = 120,
						texture = this.submarineSprites,
						sourceRect = new Microsoft.Xna.Framework.Rectangle(257, 98, 182, 25),
						animationLength = 1,
						interval = 999999f,
						position = new Vector2(148f, 66f) * 4f,
						scale = 4f
					});
					Game1.background.tempSprites.Add(new TemporaryAnimatedSprite  //another plants bg
					{
						motion = new Vector2(0f, -1f),
						yStopCoordinate = 460,
						texture = this.submarineSprites,
						sourceRect = new Microsoft.Xna.Framework.Rectangle(441, 86, 66, 37),
						animationLength = 1,
						interval = 999999f,
						position = new Vector2(18f, 149f) * 4f,
						scale = 4f
					});
				}
				else
				{
					this.ambientLightTargetColor = new Color((byte)(250f - this.submergeTimer / 20000f * 250f), (byte)(200f - this.submergeTimer / 20000f * 200f), (byte)(150f - this.submergeTimer / 20000f * 150f));
					if (Game1.random.NextDouble() < 0.11)
					{
						Vector2 pos5;
						pos5 = new Vector2(Game1.random.Next(12, base.map.DisplayWidth - 64), this.ascending.Value ? 1 : 640);
						int which3;
						which3 = Game1.random.Next(3);
						Game1.background.tempSprites.Add(new TemporaryAnimatedSprite
						{
							motion = new Vector2(0f, (float)((!this.ascending.Value) ? 1 : (-1)) * (-3f + (float)which3)),
							yStopCoordinate = ((!this.ascending.Value) ? 1 : 832),
							texture = this.submarineSprites,
							sourceRect = new Microsoft.Xna.Framework.Rectangle(132 + which3 * 8, 20, 8, 8),
							xPeriodic = true,
							xPeriodicLoopTime = 1500f,
							xPeriodicRange = 12f,
							initialPosition = pos5,
							animationLength = 1,
							interval = 5000f,
							position = pos5,
							scale = 4f
						});
					}
				}
				if (this.submergeTimer >= 20000f)
				{
					Game1.changeMusicTrack("night_market");
					base.ignoreWarps = false;
					this.changeSubmergeLight(red: true, clear: true);
					Game1.playSound("pullItemFromWater");
					Game1.ambientLight = Color.Black;
					this.ambientLightTargetColor = Color.Black;
					this.hasLitSubmergeLight = false;
					Game1.background = new Background(this, new Color(0, 50, 255), onlyMapBG: true);
					this.curtainOpenPercent = 0f;
					this.curtainMovement = 0f;
					this.submergeTimer = 0f;
					this.submerged.Value = false;
					this.ascending.Value = false;
					Game1.netWorldState.Value.IsSubmarineLocked = false;
					this.hasLitAscendLight = false;
					this.doneUntilReset = false;
					this.localAscending = false;
				}
			}
			else if (this.submerged.Value && !this.doneUntilReset)
			{
				if (Game1.random.NextDouble() < 0.01)
				{
					Vector2 pos4;
					pos4 = new Vector2(Game1.random.Next(384, base.map.DisplayWidth - 64), 320f);
					int which2;
					which2 = Game1.random.Next(3);
					Game1.background.tempSprites.Add(new TemporaryAnimatedSprite
					{
						motion = new Vector2(0f, -1f + (float)which2 * 0.2f),
						yStopCoordinate = 1,
						texture = this.submarineSprites,
						sourceRect = new Microsoft.Xna.Framework.Rectangle(132 + which2 * 8, 20, 8, 8),
						animationLength = 1,
						interval = 20000f,
						xPeriodic = true,
						xPeriodicLoopTime = 1500f,
						xPeriodicRange = 12f,
						initialPosition = pos4,
						position = pos4,
						scale = 4f
					});
				}
				if (Game1.random.NextDouble() < 0.001)
				{
					Vector2 pos3;
					pos3 = new Vector2(1344f, Game1.random.Next(448, 704));
					Game1.background.tempSprites.Add(new TemporaryAnimatedSprite
					{
						motion = new Vector2(-0.5f, 0f),
						xStopCoordinate = 448,
						texture = this.submarineSprites,
						sourceRect = new Microsoft.Xna.Framework.Rectangle(3, 194, 16, 16),
						animationLength = 1,
						interval = 50000f,
						alpha = 0.5f,
						yPeriodic = true,
						yPeriodicLoopTime = 5500f,
						yPeriodicRange = 32f,
						initialPosition = pos3,
						position = pos3,
						scale = 4f
					});
				}
				if (Game1.random.NextDouble() < 0.001)
				{
					Game1.background.tempSprites.Insert(0, new TemporaryAnimatedSprite
					{
						texture = this.submarineSprites,
						sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 146, 16, 13),
						animationLength = 9,
						interval = 100f,
						position = new Vector2(Game1.random.Next(96, 381) * 4, Game1.random.Next(24, 66) * 4),
						scale = 4f
					});
				}
				if (Game1.random.NextDouble() < 0.0001)
				{
                    Vector2 pos2;
					pos2 = new Vector2(13f, 5f) * 64f;
					Game1.background.tempSprites.Add(new TemporaryAnimatedSprite
					{
						// THE MERMAID SPRITE

						motion = new Vector2(-0f, -1f),
						color = new Color(0, 50, 150),
						yStopCoordinate = 64,
						texture = this.submarineSprites,
						sourceRectStartingPos = new Vector2(67f, 189f),
						sourceRect = new Microsoft.Xna.Framework.Rectangle(67, 189, 24, 53),
						totalNumberOfLoops = 50,
						animationLength = 3,
						pingPong = true,
						interval = 192f,
						xPeriodic = true,
						xPeriodicLoopTime = 3500f,
						xPeriodicRange = 12f,
						initialPosition = pos2,
						position = pos2,
						scale = 4f
					});
				}
                if (Game1.random.NextDouble() < 0.0003)
                {
                    Vector2 pos2;
                    pos2 = new Vector2(10f, 5f) * 64f;
                    Game1.background.tempSprites.Add(new TemporaryAnimatedSprite
                    {
                        // THE MERMAID SPRITE

                        motion = new Vector2(-0f, -1f),
                        color = new Color(0, 50, 150),
                        yStopCoordinate = 256,
                        texture = this.submarineSprites,
                        sourceRectStartingPos = new Vector2(67f, 189f),
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(67, 189, 24, 53),
                        totalNumberOfLoops = 50,
                        animationLength = 3,
                        pingPong = true,
                        interval = 192f,
                        xPeriodic = true,
                        xPeriodicLoopTime = 3500f,
                        xPeriodicRange = 12f,
                        initialPosition = pos2,
                        position = pos2,
                        scale = 4f
                    });
                }
                if (Game1.random.NextDouble() < 0.0003)
                {
                    Vector2 pos2;
                    pos2 = new Vector2(19f, 5f) * 64f;
                    Game1.background.tempSprites.Add(new TemporaryAnimatedSprite
                    {
                        // THE MERMAID SPRITE

                        motion = new Vector2(-0f, -1f),
                        color = new Color(0, 50, 150),
                        yStopCoordinate = 64,
                        texture = this.submarineSprites,
                        sourceRectStartingPos = new Vector2(67f, 189f),
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(67, 189, 24, 53),
                        totalNumberOfLoops = 50,
                        animationLength = 3,
                        pingPong = true,
                        interval = 192f,
                        xPeriodic = true,
                        xPeriodicLoopTime = 3500f,
                        xPeriodicRange = 12f,
                        initialPosition = pos2,
                        position = pos2,
                        scale = 4f
                    });
                }
                if (Game1.random.NextDouble() < 0.00035)
				{
					Vector2 pos;
					pos = new Vector2(24f, 2f) * 64f;
					int which;
					which = Game1.random.Next(3);
					Game1.background.tempSprites.Add(new TemporaryAnimatedSprite
					{
						motion = new Vector2(-0.5f, 0f),
						xStopCoordinate = 64,
						texture = this.submarineSprites,
						sourceRectStartingPos = new Vector2(257 + which * 48, 81f),
						sourceRect = new Microsoft.Xna.Framework.Rectangle(257 + which * 48, 81, 16, 16),
						totalNumberOfLoops = 250,
						animationLength = 3,
						interval = 200f,
						pingPong = true,
						yPeriodic = true,
						yPeriodicLoopTime = 3500f,
						yPeriodicRange = 12f,
						initialPosition = pos,
						position = pos,
						scale = 4f
					});
				}
			}
			if (!Game1.ambientLight.Equals(this.ambientLightTargetColor))
			{
				if (Game1.ambientLight.R < this.ambientLightTargetColor.R)
				{
					Game1.ambientLight.R++;
				}
				else if (Game1.ambientLight.R > this.ambientLightTargetColor.R)
				{
					Game1.ambientLight.R--;
				}
				if (Game1.ambientLight.G < this.ambientLightTargetColor.G)
				{
					Game1.ambientLight.G++;
				}
				else if (Game1.ambientLight.G > this.ambientLightTargetColor.G)
				{
					Game1.ambientLight.G--;
				}
				if (Game1.ambientLight.B < this.ambientLightTargetColor.B)
				{
					Game1.ambientLight.B++;
				}
				else if (Game1.ambientLight.B > this.ambientLightTargetColor.B)
				{
					Game1.ambientLight.B--;
				}
			}
		}

		public override void cleanupBeforePlayerExit()
		{
			base.cleanupBeforePlayerExit();
			Game1.background = null;
		}
	}
}
