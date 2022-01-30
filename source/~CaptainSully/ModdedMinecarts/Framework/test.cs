/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

using xTile.Dimensions;

namespace ModdedMinecarts.Framework
{
    internal class test
    {
	
	// Tile action
	//		GameLocation.performAction
	//			createQuestionDialogue
	//			drawQuestionDialogue
	//				GameLocation.answerDialogueAction
	//					warpFarmer

/*
		public virtual bool performAction(string action, Farmer who, Location tileLocation)
		{
			if (action != null && who.IsLocalPlayer)
			{
				string[] actionParams = action.Split(' ');
				switch (actionParams[0])
				{
					case "Warp_Sunroom_Door":
						if (who.getFriendshipHeartLevelForNPC("Caroline") >= 2)
						{
							this.playSoundAt("doorClose", new Vector2(tileLocation.X, tileLocation.Y));
							Game1.warpFarmer("Sunroom", 5, 13, flip: false);
						}
						else
						{
							Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Caroline_Sunroom_Door"));
						}
						break;
					case "HMTGF":
						if (who.ActiveObject != null && who.ActiveObject != null && !who.ActiveObject.bigCraftable && (int)who.ActiveObject.parentSheetIndex == 155)
						{
							Object reward = new Object(Vector2.Zero, 155);
							if (!Game1.player.couldInventoryAcceptThisItem(reward) && (int)Game1.player.ActiveObject.stack > 1)
							{
								Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
								break;
							}
							Game1.player.reduceActiveItemByOne();
							Game1.player.makeThisTheActiveObject(reward);
							this.localSound("discoverMineral");
							Game1.flashAlpha = 1f;
						}
						break;
					case "BuyBackpack":
						{
							Response purchase10001 = new Response("Purchase", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Response2000"));
							Response purchase10000 = new Response("Purchase", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Response10000"));
							Response notNow = new Response("Not", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_ResponseNo"));
							if ((int)Game1.player.maxItems == 12)
							{
								this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Question24"), new Response[2] { purchase10001, notNow }, "Backpack");
							}
							else if ((int)Game1.player.maxItems < 36)
							{
								this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Question36"), new Response[2] { purchase10000, notNow }, "Backpack");
							}
							break;
						}
					case "WarpGreenhouse":
						if (Game1.MasterPlayer.mailReceived.Contains("ccPantry"))
						{
							who.faceGeneralDirection(new Vector2(tileLocation.X, tileLocation.Y) * 64f);
							this.playSoundAt("doorClose", new Vector2(tileLocation.X, tileLocation.Y));
							GameLocation greenhouse = Game1.getLocationFromName("Greenhouse");
							int destination_x = 10;
							int destination_y = 23;
							if (greenhouse != null)
							{
								foreach (Warp warp in greenhouse.warps)
								{
									if (warp.TargetName == "Farm")
									{
										destination_x = warp.X;
										destination_y = warp.Y - 1;
										break;
									}
								}
							}
							Game1.warpFarmer("Greenhouse", destination_x, destination_y, flip: false);
						}
						else
						{
							Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Farm_GreenhouseRuins"));
						}
						break;
					case "Warp":
						who.faceGeneralDirection(new Vector2(tileLocation.X, tileLocation.Y) * 64f);
						Rumble.rumble(0.15f, 200f);
						if (actionParams.Length < 5)
						{
							this.playSoundAt("doorClose", new Vector2(tileLocation.X, tileLocation.Y));
						}
						Game1.warpFarmer(actionParams[3], Convert.ToInt32(actionParams[1]), Convert.ToInt32(actionParams[2]), flip: false);
						break;



					case "MinecartTransport":
						if (Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
						{
							this.createQuestionDialogue( answerChoices: (!Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom")) ? new Response[3]
								//ternary operator...
							
								//if true then
								{
								new Response("Town", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Town")),
								new Response("Bus", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_BusStop")),
								new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel"))
								}

								//else
								: new Response[4]
								{
								new Response("Town", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Town")),
								new Response("Bus", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_BusStop")),
								new Response("Quarry", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Quarry")),
								new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel"))
								}, question: Game1.content.LoadString("Strings\\Locations:MineCart_ChooseDestination"), dialogKey: "Minecart" );
						}
						else
						{
							Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MineCart_OutOfOrder"));
						}
						break;


					case "MineElevator":
						if (MineShaft.lowestLevelReached < 5)
						{
							Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Mines_MineElevator_NotWorking")));
						}
						else
						{
							Game1.activeClickableMenu = new MineElevatorMenu();
						}
						break;
					case "NextMineLevel":
					case "Mine":
						this.playSound("stairsdown");
						Game1.enterMine((actionParams.Length == 1) ? 1 : Convert.ToInt32(actionParams[1]));
						break;
					case "ExitMine":
						{
							Response[] responses = new Response[3]
							{
						new Response("Leave", Game1.content.LoadString("Strings\\Locations:Mines_LeaveMine")),
						new Response("Go", Game1.content.LoadString("Strings\\Locations:Mines_GoUp")),
						new Response("Do", Game1.content.LoadString("Strings\\Locations:Mines_DoNothing"))
							};
							this.createQuestionDialogue(" ", responses, "ExitMine");
							break;
						}
					default:
						return false;
				}
				return true;
			}
		}

		public virtual bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
		{
			if (questionAndAnswer == null)
			{
				return false;
			}
			switch (questionAndAnswer)
			{
				case "Mine_Yes":
					if (Game1.CurrentMineLevel > 120)
					{
						Game1.warpFarmer("SkullCave", 3, 4, 2);
					}
					else
					{
						Game1.warpFarmer("UndergroundMine", 16, 16, flip: false);
					}
					break;
				case "Mine_No":
					{
						Response[] noYesResponses = new Response[2]
						{
					new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")),
					new Response("Yes", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes"))
						};
						this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Mines_ResetMine")), noYesResponses, "ResetMine");
						break;
					}
				case "ExitMine_Yes":
				case "ExitMine_Leave":
					if (Game1.CurrentMineLevel == 77377)
					{
						Game1.warpFarmer("Mine", 67, 10, flip: true);
					}
					else if (Game1.CurrentMineLevel > 120)
					{
						Game1.warpFarmer("SkullCave", 3, 4, 2);
					}
					else
					{
						Game1.warpFarmer("Mine", 23, 8, flip: false);
					}
					Game1.changeMusicTrack("none");
					break;
				case "ExitMine_Go":
					Game1.enterMine(Game1.CurrentMineLevel - 1);
					break;
				case "Minecart_Mines":
					Game1.player.Halt();
					Game1.player.freezePause = 700;
					Game1.warpFarmer("Mine", 13, 9, 1);
					if (Game1.getMusicTrackName() == "springtown")
					{
						Game1.changeMusicTrack("none");
					}
					break;
				case "Minecart_Town":
					Game1.player.Halt();
					Game1.player.freezePause = 700;
					Game1.warpFarmer("Town", 105, 80, 1);
					break;
				case "Minecart_Quarry":
					Game1.player.Halt();
					Game1.player.freezePause = 700;
					Game1.warpFarmer("Mountain", 124, 12, 2);
					break;
				case "Minecart_Bus":
					Game1.player.Halt();
					Game1.player.freezePause = 700;
					Game1.warpFarmer("BusStop", 4, 4, 2);
					break;
				case "Backpack_Yes":
					this.tryToBuyNewBackpack();
					break;
				default:
					return false;
			}
			return true;
		}

*/
	}
}
