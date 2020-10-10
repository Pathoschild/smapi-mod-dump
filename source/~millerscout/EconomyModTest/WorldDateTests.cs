/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using EconomyMod.Helpers;
using EconomyMod.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EconomyModTest
{
    [TestClass]
    public class WorldDateTests
    {
        [TestMethod]
        public void CheckIfDaysSeasonAndYearAreParsedCorrectly()
        {
            var FirstDayOfFirstYearOFSpring = 1.ToWorldDate();
            Assert.AreEqual(DayOfWeek.Monday, FirstDayOfFirstYearOFSpring.Day);
            Assert.AreEqual(EconomyMod.Model.Season.Spring, FirstDayOfFirstYearOFSpring.Season);
            Assert.AreEqual(1, FirstDayOfFirstYearOFSpring.Year);

            var ThursdayOnFirstYearOfSpring = 18.ToWorldDate();
            Assert.AreEqual(DayOfWeek.Thursday, ThursdayOnFirstYearOfSpring.Day);
            Assert.AreEqual(EconomyMod.Model.Season.Spring, ThursdayOnFirstYearOfSpring.Season);
            Assert.AreEqual(1, ThursdayOnFirstYearOfSpring.Year);

            var LastDayOfweekOnSpringOfFirstYear = 7.ToWorldDate();
            Assert.AreEqual(DayOfWeek.Sunday, LastDayOfweekOnSpringOfFirstYear.Day);
            Assert.AreEqual(EconomyMod.Model.Season.Spring, LastDayOfweekOnSpringOfFirstYear.Season);
            Assert.AreEqual(1, LastDayOfweekOnSpringOfFirstYear.Year);

            var SecondWeekMondayFirstYearFirst = 8.ToWorldDate();
            Assert.AreEqual(DayOfWeek.Monday, SecondWeekMondayFirstYearFirst.Day);
            Assert.AreEqual(EconomyMod.Model.Season.Spring, SecondWeekMondayFirstYearFirst.Season);
            Assert.AreEqual(1, SecondWeekMondayFirstYearFirst.Year);


            var FirstDayOfSecondSummer = 29.ToWorldDate();
            Assert.AreEqual(DayOfWeek.Monday, FirstDayOfSecondSummer.Day);
            Assert.AreEqual(EconomyMod.Model.Season.Summer, FirstDayOfSecondSummer.Season);
            Assert.AreEqual(1, FirstDayOfSecondSummer.Year);

            var LastDayOfYear = 112.ToWorldDate();
            Assert.AreEqual(DayOfWeek.Sunday, LastDayOfYear.Day);
            Assert.AreEqual(EconomyMod.Model.Season.Winter, LastDayOfYear.Season);
            Assert.AreEqual(1, LastDayOfYear.Year);

            var FirstDayOfSecondYear = 113.ToWorldDate();
            Assert.AreEqual(DayOfWeek.Monday, FirstDayOfSecondYear.Day);
            Assert.AreEqual(EconomyMod.Model.Season.Spring, FirstDayOfSecondYear.Season);
            Assert.AreEqual(2, FirstDayOfSecondYear.Year);


            var SummerAtSecondYear = 168.ToWorldDate();
            Assert.AreEqual(DayOfWeek.Sunday, SummerAtSecondYear.Day);
            Assert.AreEqual(EconomyMod.Model.Season.Summer, SummerAtSecondYear.Season);
            Assert.AreEqual(2, SummerAtSecondYear.Year);


            var LastDayOfSecondYear = 224.ToWorldDate();
            Assert.AreEqual(DayOfWeek.Sunday, LastDayOfSecondYear.Day);
            Assert.AreEqual(EconomyMod.Model.Season.Winter, LastDayOfSecondYear.Season);
            Assert.AreEqual(2, LastDayOfSecondYear.Year);

            var FirstDayOfThirdYear = 225.ToWorldDate();
            Assert.AreEqual(DayOfWeek.Monday, FirstDayOfThirdYear.Day);
            Assert.AreEqual(EconomyMod.Model.Season.Spring, FirstDayOfThirdYear.Season);
            Assert.AreEqual(3, FirstDayOfThirdYear.Year);

        }
        [TestMethod]
        public void CheckIfNextDayIsValid()
        {

            var scenarioOne = 1.ToWorldDate().Next(DayOfWeek.Monday);
            Assert.IsTrue(scenarioOne.DaysCount > 1);
            Assert.AreEqual(DayOfWeek.Monday, scenarioOne.Day);

            var nextday = DayOfWeek.Tuesday;
            var scenarioTwo = 1.ToWorldDate();
            Assert.AreNotEqual(nextday, scenarioTwo.Day);
            scenarioTwo.Next(DayOfWeek.Tuesday);

            Assert.IsTrue(scenarioTwo.DaysCount > 1);
            Assert.AreEqual(DayOfWeek.Tuesday, scenarioTwo.Day);


        }
        [TestMethod]
        public void CheckIfDaysOfMonthIsRight()
        {
            CheckScenario(1, 1);
            CheckScenario(28, 28);
            CheckScenario(29, 1);
            CheckScenario(35, 7);
            CheckScenario(18, 18);
            CheckScenario(113, 1);
            CheckScenario(141, 1);
            CheckScenario(140, 28);
            CheckScenario(1579, 11);

            for (var i = 0; i <= 1000000; i++)
            {
                var date = i.ToWorldDate();
                Assert.IsTrue(date.DayOfMonth <= 28);
            }


            EconomyMod.Model.CustomWorldDate CheckScenario(int day, int expected)
            {
                var scenarioOne = day.ToWorldDate();
                Assert.AreEqual(expected, scenarioOne.DayOfMonth);
                return scenarioOne;
            }
        }

        [TestMethod]
        public void CheckIfCalendarTaxBoolIsWorkingFine()
        {
            //108scenario
            //113scenario
            //1579scenario
            //569-580
            var rd = new Random();
            var index = rd.Next(0, 580);
            TestIfHasOnlyTwentyEightItems(index);

            var list = new List<TaxSchedule>() { };
            var resultScenarioOne = 1.ToWorldDate()
                .GenerateCalendarTaxBool(list);

            Assert.AreEqual(28, resultScenarioOne.Count(c => c.Value == false), "Should not exist any tax because list is empty!");


            list.Add(new TaxSchedule() { DayCount = 108 }); //24day
            list.Add(new TaxSchedule() { DayCount = 85 });//1day
            list.Add(new TaxSchedule() { DayCount = 112 });//28day
            var resultScenarioTwo = 108.ToWorldDate()
                .GenerateCalendarTaxBool(list);

            Assert.AreEqual(3, resultScenarioTwo.Where(c=>c.Value).Count(), "Should exists 3 tax registered");
            Assert.AreEqual(true, resultScenarioTwo.Any(c => c.Key == 108 && c.Value));
            Assert.AreEqual(true, resultScenarioTwo.Any(c => c.Key == 85 && c.Value));
            Assert.AreEqual(true, resultScenarioTwo.Any(c => c.Key == 112 && c.Value));


            void TestIfHasOnlyTwentyEightItems(int referenceIndex)
            {


                var referenceDate = new CustomWorldDate(referenceIndex);
                var taxes = GenerateSomeTaxSchedule(referenceIndex, referenceDate);
                referenceDate = new CustomWorldDate(referenceIndex);
                var dict = referenceDate.GenerateCalendarTaxBool(taxes);

                Assert.AreEqual(28, dict.Count);


            }

            List<TaxSchedule> GenerateSomeTaxSchedule(int referenceIndex, CustomWorldDate referenceDate)
            {
                var taxes = new List<TaxSchedule>();
                for (int i = referenceIndex; i <= referenceIndex + 80; i++)
                {
                    if (i != referenceIndex) referenceDate.AddDays(1);
                    taxes.Add(new TaxSchedule(referenceDate, new EconomyMod.TaxDetailed()));
                }
                return taxes;
            }
        }


    }
}
