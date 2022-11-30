/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using InformantTest.Implementation;
using NUnit.Framework;
using StardewTests.Harness.Test;

namespace InformantTest; 

[TestFixture]
public class I18NTest {

    [Test]
    [TestCase(LocalizedContentManager.LanguageCode.de)] 
    [TestCase(LocalizedContentManager.LanguageCode.ko)] 
    [TestCase(LocalizedContentManager.LanguageCode.tr)] 
    [TestCase(LocalizedContentManager.LanguageCode.zh)] 
    [TestCase(LocalizedContentManager.LanguageCode.fr)] 
    public void ValidateLocales(LocalizedContentManager.LanguageCode locale) {
        I18NTestUtil.Assert18NCorrect(TestUtils.ModFolder, locale);
    }
}