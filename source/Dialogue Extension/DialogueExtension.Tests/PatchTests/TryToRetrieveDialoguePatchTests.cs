/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using DialogueExtension.Patches.Parsing;
using DialogueExtension.Patches.Utility;
using DialogueExtension.Tests.MockWrappers;
using DialogueExtension.Tests.PatchTests.VanillaCode;
using LightInject;
using NUnit.Framework;
using SDV.Shared.Abstractions;
using SDV.Shared.Abstractions.Utility;
using StardewModdingAPI;
using StardewValley;
using DayOfWeek = DialogueExtension.Patches.Utility.DayOfWeek;

namespace DialogueExtension.Tests.PatchTests
{
  public class TryToRetrieveDialoguePatchTests
  {
    private IWrapperFactory _factory;
    private IMonitor _logger = new MockLogger();
    private static IServiceFactory _container;


    [Test]
    public void ComparisonRunTest()
    {
      _factory = new MockWrapperFactory();
      GetTestingConditions();
    }

    private void GetTestingConditions()
    {
      var i = 0;
      var weekDay = DayOfWeek.Mon;
      foreach (var npc in MockData.NpcNames)
      {
        foreach (var year in MockData.Years)
        {
          foreach (var season in MockData.Seasons)
          {
            foreach (var day in MockData.DaysOfMonth)
            {
              foreach (var spouse in MockData.MarriageNpcNames)
              {
                foreach (var hearts in MockData.Friendships)
                {
                  foreach (var cc in new[] { true, false })
                    foreach (var key in new[] { true, false })
                    {
                      if (npc == "Penny" && season == Season.Fall && weekDay == DayOfWeek.Mon) continue;
                      BuildConditions(npc, year, season, day, weekDay, spouse, hearts, cc, key);
                      i++;
                    }
                }
              }
              weekDay = weekDay.NextDay();
            }
          }
        }
      }
      Console.WriteLine($"Checked {i} scenarios");
    }

    private void BuildConditions(string name, int year,
      Season season, int day, DayOfWeek weekday, string spouse, int hearts, bool cc, bool key)
    {
      var exNpc = new ExtendNPC() {SetDialogue = MockData.CharacterDialogue(name) as Dictionary<string, string>};
      var npc = _factory.CreateInstance<INPCWrapper>(exNpc);
      npc.Name = name;
      ((MockNPCWrapper)npc).MockDialogue = MockData.CharacterDialogue(name);
      
      dynamic game = _factory.CreateInstance<IGameWrapper>();
      game.year = year;
      game.currentSeason = season == Season.Unknown ? string.Empty : season.ToString().ToLower();
      game.dayOfMonth = day;
      game.shortDayNameFromDayOfSeason = new Func<int, string>(i => weekday.ToString());
      game.player.spouse = spouse;
      game.player.friendshipData = new Dictionary<string, IFriendshipWrapper>();
      if (hearts > 0)
      {
        var friendship = _factory.CreateInstance<IFriendshipWrapper>();
        friendship.Points = hearts;
        ((Dictionary<string, IFriendshipWrapper>)game.player.friendshipData).Add(name, friendship);
      }

      game.isLocationAccessible = new Func<string, bool>(s => cc);
      game.player.HasTownKey = key;
      Game1.content = new MockLocalizedContentManager(new MockServiceProvider(), Path.Combine("..", "..", ".."));
      Game1.random = new Random();
      
      ((MockWrapperFactory)_factory).SetInstance<IGameWrapper>(objects => game);
      //Console.WriteLine($"{name} {year} {season} {day} {weekday} {spouse} {hearts} {cc} {key}");

      if (name == "Jodi" && year == 1 && season == Season.Unknown && day == 2 && weekday == DayOfWeek.Tue && spouse == "" && hearts == 250)
      {
        var x = 45;
      }

      var vanillaTest = new TestNPC {Game = game, NPC = npc};
      var vanillaResult = vanillaTest.tryToRetrieveDialogue(season == Season.Unknown ? string.Empty : season.ToString().ToLower() + "_", hearts / 250);
      var patchTest = new DialogueLogic(new ConditionRepository(), _logger, _factory);
      
      var patchResult = patchTest.GetDialogue(ref npc, season != Season.Unknown);

      if (vanillaResult == null)
      {
        if (patchResult != null)
        {
          Console.WriteLine(string.Join(",", patchResult.CurrentEmotion));
          Console.WriteLine($"{name} {year} {season} {day} {weekday} {spouse} {hearts} {cc} {key}");
        }
        Assert.AreEqual(null, patchResult);
        //Console.WriteLine("Is Null");
      }
      else
      {
        if (vanillaResult?.CurrentEmotion != patchResult?.CurrentEmotion)
        {
          Console.WriteLine(vanillaResult.CurrentEmotion);
          Console.WriteLine(patchResult?.CurrentEmotion);
          Console.WriteLine($"{name} {year} {season} {day} {weekday} {spouse} {hearts} {cc} {key}");
        }
        Assert.That(vanillaResult?.CurrentEmotion == patchResult?.CurrentEmotion);
      }
    }
  }
}
