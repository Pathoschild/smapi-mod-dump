/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DeadRobotDev/StardewMods
**
*************************************************/

using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SkipStart;

internal sealed class ModEntry : Mod
{
	private ModConfig _config;

	private IReflectedField<NetBool>? _landslide;

	public override void Entry(IModHelper helper)
	{
		_config = helper.ReadConfig<ModConfig>();

		helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
		helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

		if (_config.MineOpen)
		{
			helper.Events.Player.Warped += Player_Warped;
		}
	}

	private void GameLoop_SaveLoaded(object? sender, SaveLoadedEventArgs e)
	{
		if (_config.IntroductionQuestSkip)
		{
			SkipIntroductionQuest();
		}

		if (_config.Willy == "skip")
		{
			Willy_Skip();
		}

		if (_config.CommunityCenter != "")
		{
			CommunityCenter_Open();

			if (_config.CommunityCenter == "wizard")
			{
				CommunityCenter_Wizard();
			}
		}
	}

	private void GameLoop_DayStarted(object? sender, DayStartedEventArgs e)
	{
		if (SDate.Now() > new SDate(5, "spring", 1))
		{
			if (_config.MineOpen)
			{
				Helper.Events.Player.Warped -= Player_Warped;
			}

			Helper.Events.GameLoop.DayStarted -= GameLoop_DayStarted;
		}

		switch (Game1.dayOfMonth)
		{
			case 1:
				if (_config.Willy == "day1")
				{
					// SV looks for "spring_2_1" in mailReceived to unlock the Fish Shop.
					Game1.player.mailbox.Add("spring_2_1");
				}

				break;
			case 2:
				if (_config.Willy != "")
				{
					Game1.player.mailbox.Remove("spring_2_1");
				}

				break;
			case 5:
				if (_config.MineOpen)
				{
					Game1.player.mailbox.Remove("landslideDone");
				}

				break;
		}
	}

	private void Player_Warped(object? sender, WarpedEventArgs e)
	{
		if (e.NewLocation.Name != "Mountain")
		{
			return;
		}

		if (_landslide == null)
		{
			_landslide = Helper.Reflection.GetField<NetBool>(
				(StardewValley.Locations.Mountain)e.NewLocation, "landslide");
		}

		_landslide.SetValue(new NetBool(false));
	}

	private static IEnumerable<string> GetIntroductionQuestCharacters()
	{
		return Game1.characterData.Where(pair =>
		{
			var characterData = pair.Value;
			if (characterData.IntroductionsQuest == false)
			{
				return false;
			}

			return characterData.HomeRegion == "Town";
		}).Select(pair => pair.Key);
	}

	private static void SkipIntroductionQuest()
	{
		if (!Game1.player.hasQuest("9"))
		{
			return;
		}

		Game1.player.completeQuest("9");

		foreach (var character in GetIntroductionQuestCharacters())
		{
			if (Game1.player.hasPlayerTalkedToNPC(character))
			{
				continue;
			}

			if (Game1.player.friendshipData.TryGetValue(character, out _))
			{
				continue;
			}

			var friendship = new Friendship();

			Game1.player.friendshipData.Add(character, friendship);
		}
	}

	private static void Willy_Skip()
	{
		if (Game1.player.eventsSeen.Contains("739330"))
		{
			Game1.player.mailReceived.Add("spring_2_1");
			return;
		}

		if (!Game1.player.hasQuest("13"))
		{
			Game1.player.mailReceived.Add("spring_2_1");
			Game1.player.addQuest("13");
		}

		Game1.player.eventsSeen.Add("739330");
		Game1.player.completeQuest("13");
		Game1.player.mailReceived.Add("NOQUEST_13");
		Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(T)BambooPole"));
	}

	private static void CommunityCenter_Open()
	{
		if (!Game1.player.eventsSeen.Add("611439"))
		{
			return;
		}

		Game1.player.mailReceived.Add("ccDoorUnlock");
		Game1.player.addQuest("26");
	}

	private static void CommunityCenter_Wizard()
	{
		if (!Game1.player.hasQuest("26"))
		{
			return;
		}

		Game1.player.completeQuest("26");
		Game1.player.mailReceived.Add("seenJunimoNote");

		Game1.player.mailReceived.Add("wizardJunimoNote");
		Game1.player.addQuest("1");

		Game1.player.completeQuest("1");
		Game1.player.eventsSeen.Add("112");
		Game1.player.activeDialogueEvents.Add("cc_Begin", 4);
		Game1.player.mailReceived.Add("canReadJunimoText");
	}
}
