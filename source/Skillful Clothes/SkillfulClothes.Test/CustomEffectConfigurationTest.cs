/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillfulClothes.Configuration;
using SkillfulClothes.Effects;
using SkillfulClothes.Effects.Attributes;
using SkillfulClothes.Effects.Buffs;
using SkillfulClothes.Effects.SharedParameters;
using SkillfulClothes.Effects.Skills;
using SkillfulClothes.Effects.Special;
using SkillfulClothes.Types;
using StardewValley;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SkillfulClothes.Test
{
    [TestClass]
    public class CustomEffectConfigurationTest
    {
        [TestMethod]
        public void ParseIdentifiers_Test()
        {
            string json = @"{
	123: 'Foo',
    SailorShirt: 'Bar' }";

            CustomEffectConfigurationParser parser = new CustomEffectConfigurationParser();

            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var definitions = parser.Parse(mStream);

                Assert.AreEqual(2, definitions.Count);
                Assert.AreEqual("123", definitions[0].ItemIdentifier);
                Assert.AreEqual("SailorShirt", definitions[1].ItemIdentifier);
            }
        }

        [TestMethod]
        public void CreateDefaultEffectInstance_Test()
        {
            EffectLibrary library = EffectLibrary.Default;

            IEffect effect = library.CreateEffectInstance("IncreaseAttack");
            Assert.IsInstanceOfType(effect, typeof(IncreaseAttack));

            Assert.AreEqual(new AmountEffectParameters().Amount, ((IncreaseAttack)effect).Parameters.Amount);
        }

        [TestMethod]
        public void ParseConfigWithSingleEffectAndDefaultParameters_Test()
        {
            string json = @"{
	123: 'IncreaseAttack',
    SailorShirt: 'IncreaseMaxHealth' }";

            CustomEffectConfigurationParser parser = new CustomEffectConfigurationParser();

            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var definitions = parser.Parse(mStream);

                Assert.AreEqual(2, definitions.Count);
                Assert.AreEqual("123", definitions[0].ItemIdentifier);                
                Assert.IsInstanceOfType(definitions[0].Effect, typeof(IncreaseAttack));

                Assert.AreEqual("SailorShirt", definitions[1].ItemIdentifier);
                Assert.IsInstanceOfType(definitions[1].Effect, typeof(IncreaseMaxHealth));
            }            
        }

        [TestMethod]
        public void ParseConfigWithSingleEffectAndCustomParameters_Test()
        {
            string json = @"{
	123: {
        IncreaseAttack: {
            amount: 5
    }}}";

            CustomEffectConfigurationParser parser = new CustomEffectConfigurationParser();

            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var definitions = parser.Parse(mStream);

                Assert.AreEqual(1, definitions.Count);
                Assert.AreEqual("123", definitions[0].ItemIdentifier);
                Assert.IsInstanceOfType(definitions[0].Effect, typeof(IncreaseAttack));
                Assert.AreEqual(5, ((IncreaseAttack)definitions[0].Effect).Parameters.Amount);                
            }
        }

        [TestMethod]
        public void ParseConfigWithSingleEffectAndEnumParameters_Test()
        {
            string json = @"{
	123: {
        IncreaseSkillLevel: {
            amount: 2,
            skill: 'Foraging'
    }}}";

            CustomEffectConfigurationParser parser = new CustomEffectConfigurationParser();

            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var definitions = parser.Parse(mStream);

                Assert.AreEqual(1, definitions.Count);
                Assert.AreEqual("123", definitions[0].ItemIdentifier);
                Assert.IsInstanceOfType(definitions[0].Effect, typeof(IncreaseSkillLevel));
                Assert.AreEqual(2, ((IncreaseSkillLevel)definitions[0].Effect).Parameters.Amount);
                Assert.AreEqual(Skill.Foraging, ((IncreaseSkillLevel)definitions[0].Effect).Parameters.Skill);
            }
        }

        [TestMethod]
        public void ParseConfigWithNestedEffect_Test()
        {
            string json = @"{
	123: {
        Seasonal: {
            season: 'Summer',
            effect: 'IncreaseAttack'
    }}}";

            CustomEffectConfigurationParser parser = new CustomEffectConfigurationParser();

            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var definitions = parser.Parse(mStream);

                Assert.AreEqual(1, definitions.Count);
                Assert.AreEqual("123", definitions[0].ItemIdentifier);
                Assert.IsInstanceOfType(definitions[0].Effect, typeof(SeasonalEffect));
                Assert.AreEqual(Season.Summer, ((SeasonalEffect)definitions[0].Effect).Parameters.Season);
                Assert.IsInstanceOfType(((SeasonalEffect)definitions[0].Effect).Parameters.Effect, typeof(IncreaseAttack));                
            }
        }

        [TestMethod]
        public void ParseConfigWithMultipleNestedEffects_Test()
        {
            string json = @"{
	123: {
        Locational: {
            location: 'DesertPlaces',
            effect: [ 'IncreaseAttack', { IncreaseDefense: {amount: 5} } ]
    }}}";

            CustomEffectConfigurationParser parser = new CustomEffectConfigurationParser();

            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var definitions = parser.Parse(mStream);

                Assert.AreEqual(1, definitions.Count);
                Assert.AreEqual("123", definitions[0].ItemIdentifier);
                Assert.IsInstanceOfType(definitions[0].Effect, typeof(LocationalEffect));
                Assert.AreEqual(LocationGroup.DesertPlaces, ((LocationalEffect)definitions[0].Effect).Parameters.Location);
                Assert.IsInstanceOfType(((LocationalEffect)definitions[0].Effect).Parameters.Effect, typeof(EffectSet));

                var effects = ((EffectSet)((LocationalEffect)definitions[0].Effect).Parameters.Effect).Effects;
                Assert.AreEqual(2, effects.Length);

                Assert.IsInstanceOfType(effects[0], typeof(IncreaseAttack));
                Assert.IsInstanceOfType(effects[1], typeof(IncreaseDefense));
                Assert.AreEqual(5, ((IncreaseDefense)effects[1]).Parameters.Amount);
            }
        }

        [TestMethod]
        public void ParseConfigWithMultipleEffectsAndDefaultParameters_Test()
        {
            string json = @"{
	123: [
        'IncreaseAttack', 'IncreaseMaxHealth'
        ]}";

            CustomEffectConfigurationParser parser = new CustomEffectConfigurationParser();

            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var definitions = parser.Parse(mStream);

                Assert.AreEqual(1, definitions.Count);
                Assert.AreEqual("123", definitions[0].ItemIdentifier);
                Assert.IsInstanceOfType(definitions[0].Effect, typeof(EffectSet));

                var effects = new List<IEffect>(((EffectSet)definitions[0].Effect).Effects);
                Assert.IsInstanceOfType(effects[0], typeof(IncreaseAttack));
                Assert.IsInstanceOfType(effects[1], typeof(IncreaseMaxHealth));                
            }
        }

        [TestMethod]
        public void ParseConfigWithMultipleEffectsAndParameters_Test()
        {
            string json = @"{
	123: [
        { IncreaseAttack: {amount: 10} },
        { IncreaseMaxHealth: {amount: 25} }
        ]}";

            CustomEffectConfigurationParser parser = new CustomEffectConfigurationParser();

            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var definitions = parser.Parse(mStream);

                Assert.AreEqual(1, definitions.Count);
                Assert.AreEqual("123", definitions[0].ItemIdentifier);
                Assert.IsInstanceOfType(definitions[0].Effect, typeof(EffectSet));

                var effects = new List<IEffect>(((EffectSet)definitions[0].Effect).Effects);
                Assert.IsInstanceOfType(effects[0], typeof(IncreaseAttack));
                Assert.AreEqual(10, ((IncreaseAttack)effects[0]).Parameters.Amount);

                Assert.IsInstanceOfType(effects[1], typeof(IncreaseMaxHealth));
                Assert.AreEqual(25, ((IncreaseMaxHealth)effects[1]).Parameters.Amount);
            }
        }

        [TestMethod]
        public void ParseConfigWithMultipleEffectsAndMixedParameterStyle_Test()
        {
            string json = @"{
	123: [
        { IncreaseAttack: {amount: 10} },
        'IncreaseMaxHealth',
        { IncreaseFishingBarByCaughtFish: {} },
        { IncreaseDefense: {} }
        ]}";

            CustomEffectConfigurationParser parser = new CustomEffectConfigurationParser();

            using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var definitions = parser.Parse(mStream);

                Assert.AreEqual(1, definitions.Count);
                Assert.AreEqual("123", definitions[0].ItemIdentifier);
                Assert.IsInstanceOfType(definitions[0].Effect, typeof(EffectSet));
                var effects = new List<IEffect>(((EffectSet)definitions[0].Effect).Effects);

                Assert.AreEqual(4, effects.Count);
                
                Assert.IsInstanceOfType(effects[0], typeof(IncreaseAttack));
                Assert.AreEqual(10, ((IncreaseAttack)effects[0]).Parameters.Amount);

                Assert.IsInstanceOfType(effects[1], typeof(IncreaseMaxHealth));
                Assert.AreEqual(1, ((IncreaseMaxHealth)effects[1]).Parameters.Amount);

                Assert.IsInstanceOfType(effects[2], typeof(IncreaseFishingBarByCaughtFish));

                Assert.IsInstanceOfType(effects[3], typeof(IncreaseDefense));
                Assert.AreEqual(1, ((IncreaseDefense)effects[3]).Parameters.Amount);
            }
        }
    }
}
