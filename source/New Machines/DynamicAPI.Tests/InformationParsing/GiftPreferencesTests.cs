/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Igorious.StardewValley.DynamicAPI.Tests.InformationParsing
{
    [TestClass]
    public sealed class GiftPreferencesTests
    {
        [TestMethod]
        public void ParseCharacterGiftPreferences()
        {
            TestParsing(" Oh! This is exactly what I wanted! Thank you!/196 200 348 606 651 650 426 430/This is a really nice gift! Thank you!/-5 -6 -79 -81 18 402 169 406 408 418 86/Hmm... I guess everone has different tastes./-2 2 152 330 221 223 229 232 233 241 209 194/This is a pretty terrible gift, isn't it?/305 211 210 206 216/Thank you.// ");
        }  
        
        private static void TestParsing(string value)
        {
            var characterGiftPreference = CharacterGiftPreferences.Parse(value);
            Assert.AreEqual(value, characterGiftPreference.ToString());
        } 
    }
}
