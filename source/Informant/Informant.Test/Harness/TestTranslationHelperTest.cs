/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System;
using System.Linq;
using NUnit.Framework;

namespace StardewTests.Harness; 

[TestFixture]
public class TestTranslationHelperTest {

    private TestTranslationHelper _classUnderTest = new(TestContext.CurrentContext.TestDirectory);

    [SetUp]
    public void SetUp() {
        _classUnderTest = new(TestContext.CurrentContext.TestDirectory);
    }

    [Test]
    public void ModId() {
        Assert.NotNull(_classUnderTest.ModID);
    }
    
    [Test]
    public void Locale() {
        Assert.NotNull(_classUnderTest.Locale);
        Assert.AreEqual("en",_classUnderTest.Locale);
    }
    
    [Test]
    public void Locale_Changed() {
        _classUnderTest.LocaleEnum = LocalizedContentManager.LanguageCode.fr;
        
        Assert.NotNull(_classUnderTest.Locale);
        Assert.AreEqual("fr",_classUnderTest.Locale);
    }
    
    [Test]
    public void LocaleEnum() {
        Assert.NotNull(_classUnderTest.LocaleEnum);
        Assert.AreEqual(LocalizedContentManager.LanguageCode.en,_classUnderTest.LocaleEnum);
    }
    
    [Test]
    public void Get() {
        var translation = _classUnderTest.Get("Key");
        Assert.AreEqual("Key" , translation.Key);
        Assert.AreEqual("Value" , translation.ToString());
    }
    
    [Test]
    public void Get_OtherLocale() {
        var translation = _classUnderTest.Get("Key");
        Assert.AreEqual("Key" , translation.Key);
        Assert.AreEqual("Value" , translation.ToString());
        
        _classUnderTest.LocaleEnum = LocalizedContentManager.LanguageCode.de;

        translation = _classUnderTest.Get("Key");
        Assert.AreEqual("Key" , translation.Key);
        Assert.AreEqual("Wert" , translation.ToString());
    }
    
    [Test]
    public void GetWithTokens() {
        var translation = _classUnderTest.Get("WithTokens", new { Token = "2" });
        Assert.AreEqual("WithTokens" , translation.Key);
        Assert.AreEqual("One, 2, Three" , translation.ToString());
    }
    
    [Test]
    public void GetWithTokens_OtherLocale() {
        _classUnderTest.LocaleEnum = LocalizedContentManager.LanguageCode.de;
        
        var translation = _classUnderTest.Get("WithTokens", new { Token = "2" });
        Assert.AreEqual("WithTokens" , translation.Key);
        Assert.AreEqual("Eins, 2, Drei" , translation.ToString());
    }
    
    [Test]
    public void GetTranslations() {
        var translations = _classUnderTest.GetTranslations().OrderBy(t => t.Key).ToList();
        Assert.NotNull(translations);
        Assert.AreEqual(2, translations.Count);

        var translation = translations[0];
        Assert.AreEqual("Key" , translation.Key);
        Assert.AreEqual("Value" , translation.ToString());
        
        translation = translations[1];
        Assert.AreEqual("WithTokens" , translation.Key);
        Assert.AreEqual("One, {{Token}}, Three" , translation.ToString());
    }
    
    [Test]
    public void GetTranslations_OtherLocale() {
        _classUnderTest.LocaleEnum = LocalizedContentManager.LanguageCode.de;
        
        var translations = _classUnderTest.GetTranslations().OrderBy(t => t.Key).ToList();
        Assert.NotNull(translations);
        Assert.AreEqual(2, translations.Count);

        var translation = translations[0];
        Assert.AreEqual("Key" , translation.Key);
        Assert.AreEqual("Wert" , translation.ToString());
        
        translation = translations[1];
        Assert.AreEqual("WithTokens" , translation.Key);
        Assert.AreEqual("Eins, {{Token}}, Drei" , translation.ToString());
    }
    
    [Test]
    public void GetInAllLocales() {
        var translations = _classUnderTest.GetInAllLocales("Key");
        Assert.NotNull(translations);
        Assert.AreEqual(1, translations.Count);
        Assert.AreEqual(new[] { "de" }, translations.Keys.ToArray());

        var translation = translations["de"];
        Assert.AreEqual("Key" , translation.Key);
        Assert.AreEqual("Wert" , translation.ToString());
    }
    
    [Test]
    public void GetInAllLocales_WithFallbackTrue() {
        var translations = _classUnderTest.GetInAllLocales("Key", true);
        Assert.NotNull(translations);
        Assert.AreEqual(2, translations.Count);
        Assert.AreEqual(new[] { "de", "en" }, translations.Keys.OrderBy(k => k).ToArray());

        var translation = translations["de"];
        Assert.AreEqual("Key" , translation.Key);
        Assert.AreEqual("Wert" , translation.ToString());
        
        translation = translations["en"];
        Assert.AreEqual("Key" , translation.Key);
        Assert.AreEqual("Value" , translation.ToString());
    }
    
    [Test]
    public void ValidateTranslations_True() {
        _classUnderTest.ValidateTranslations = true;

        try {
            _classUnderTest.Get("Unknown");
            Assert.Fail("Oh no!");
        } catch (Exception e) {
            Assert.Pass();
        }
    }
    
    [Test]
    public void ValidateTranslations_False() {
        _classUnderTest.ValidateTranslations = false;
        
        _classUnderTest.Get("Unknown");
        Assert.Pass();
    }
}