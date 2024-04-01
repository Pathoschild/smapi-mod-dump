/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/qixing-jk/QiXingAutoGrabTruffles
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using AutoGrabTruffles.Patches;
using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

namespace AutoGrabTruffles;

public class ModEntry : Mod
{
	public Pig Pig;

	public Truffle Truffle;

	private ModConfig Config;

	public override void Entry(IModHelper helper)
	{
		Config = ((Mod)this).Helper.ReadConfig<ModConfig>();
		helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		helper.Events.World.ObjectListChanged += OnObjectListChanged;
		helper.Events.GameLoop.TimeChanged += OnTimeChanged;
		helper.Events.GameLoop.DayEnding += OnDayEnding;
		Pig = new Pig();
		Truffle = new Truffle(Config);
		Harmony harmony = new Harmony(((Mod)this).ModManifest.UniqueID);
		new FarmAnimalPatcher(((Mod)this).Monitor, Config, Pig).Apply(harmony);
		if (((Mod)this).Helper.ModRegistry.IsLoaded("ferdaber.DeluxeGrabberRedux"))
		{
			new GenericObjectGrabberPatcher(((Mod)this).Monitor, Config).Apply(harmony);
		}
	}

	private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
	{
		IGenericModConfigMenuApi configMenu = ((Mod)this).Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
		if (configMenu != null)
		{
			configMenu.Register(((Mod)this).ModManifest, delegate
			{
				Config = new ModConfig();
			}, delegate
			{
				((Mod)this).Helper.WriteConfig<ModConfig>(Config);
			});
			configMenu.AddBoolOption(((Mod)this).ModManifest, () => Config.EnableAutoGrabTruffles, delegate(bool value)
			{
				Config.EnableAutoGrabTruffles = value;
			}, () => "Enable Auto-Grab Truffles", () => "Allows truffle collection with barn auto-grabbers.");
			configMenu.AddTextOption(((Mod)this).ModManifest, () => Config.Collection, delegate(string value)
			{
				Config.Collection = value;
			}, () => "Collection Frequency", () => "Determines when truffles are collected.\n - Instantly: As soon as truffle is found\n - Hourly: Every hour\n - Daily: Once a day when player goes to sleep", new string[3] { "Instantly", "Hourly", "Daily" });
			configMenu.AddBoolOption(((Mod)this).ModManifest, () => Config.GainExperience, delegate(bool value)
			{
				Config.GainExperience = value;
			}, () => "Gain Experience", () => "Enables foraging experience gain for auto-grabbing truffles.");
			configMenu.AddBoolOption(((Mod)this).ModManifest, () => Config.ApplyGathererBonus, delegate(bool value)
			{
				Config.ApplyGathererBonus = value;
			}, () => "Apply Gatherer Bonus", () => "Applies bonus of 20% chance for double harvest if player is Gatherer.");
			configMenu.AddBoolOption(((Mod)this).ModManifest, () => Config.ApplyBotanistBonus, delegate(bool value)
			{
				Config.ApplyBotanistBonus = value;
			}, () => "Apply Botanist Bonus", () => "Applies bonus for highest quality truffles if player is Botanist.");
		}
	}

	private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
	{
		if (Config.EnableAutoGrabTruffles && Pig.Current != null)
		{
			foreach (var (_, truffle) in e.Added.Where((KeyValuePair<Vector2, Object> o) => Truffle.IsValid(o.Value)))
			{
				Truffle.Enqueue(truffle);
			}
		}
		if (Config.Collection == "Instantly")
		{
			CollectTruffles();
		}
	}

	private void OnTimeChanged(object sender, TimeChangedEventArgs e)
	{
		if (Config.Collection == "Hourly" && e.NewTime % 100 == 0)
		{
			CollectTruffles();
		}
	}

	private void OnDayEnding(object sender, DayEndingEventArgs e)
	{
		if (Config.Collection == "Daily")
		{
			CollectTruffles();
		}
	}

	private void CollectTruffles()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!Config.EnableAutoGrabTruffles)
		{
			return;
		}
		List<FarmAnimal> tempPigs = new List<FarmAnimal>();
		List<Object> tempTruffles = new List<Object>();
		Truffle.InitializeSummary();
		FarmAnimal pig;
		Object truffle;
		while (Pig.TryDequeue(out pig) && Truffle.TryDequeue(out truffle))
		{
			if (!Game1.getFarm().Objects.ContainsKey(truffle.TileLocation))
			{
				continue;
			}
			truffle = Truffle.UpdateData(truffle);
			Item leftoverTruffle = truffle;
			int initialStack = truffle.Stack;
			foreach (KeyValuePair<Vector2, Object> item in ((IEnumerable<KeyValuePair<Vector2, Object>>)(object)pig.home.indoors.Value.Objects.Pairs).Where((KeyValuePair<Vector2, Object> o) => IsGrabber(o.Value)))
			{
				item.Deconstruct(out var _, out var value);
				Object grabber = value;
				leftoverTruffle = (grabber.heldObject.Value as Chest).addItem(leftoverTruffle);
				if ((grabber.heldObject.Value as Chest).Items.Any((Item i) => i != null))
				{
					grabber.showNextIndex.Value = true;
				}
				if (leftoverTruffle == null)
				{
					Truffle.Summary[(Truffle.Quality)truffle.Quality] += initialStack;
					GainExperience(2, 7, initialStack);
					Game1.getFarm().Objects.Remove(truffle.TileLocation);
					break;
				}
			}
			if (leftoverTruffle != null)
			{
				if (leftoverTruffle.Stack < initialStack)
				{
					Truffle.Summary[(Truffle.Quality)truffle.Quality] += initialStack - leftoverTruffle.Stack;
					GainExperience(2, 7, initialStack - leftoverTruffle.Stack);
				}
				truffle.Stack = leftoverTruffle.Stack;
				tempPigs.Add(pig);
				tempTruffles.Add(truffle);
			}
		}
		tempPigs.ForEach(Pig.Enqueue);
		tempTruffles.ForEach(Truffle.Enqueue);
	}

	public static bool IsGrabber(Object obj)
	{
		if (obj.ParentSheetIndex == 165 && obj.heldObject.Value != null)
		{
			return obj.heldObject.Value is Chest;
		}
		return false;
	}

	public void GainExperience(int skill, int exp, int stack)
	{
		if (Config.GainExperience)
		{
			for (int i = 0; i < stack; i++)
			{
				Game1.player.gainExperience(skill, exp);
			}
		}
	}
}
