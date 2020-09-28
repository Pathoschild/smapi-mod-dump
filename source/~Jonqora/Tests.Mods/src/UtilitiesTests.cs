using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicConversationTopics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using CIL = Harmony.CodeInstruction;
using System.Reflection.Emit;

namespace Tests.Mods.DynamicConversationTopics
{
    [TestClass()]
    public class UtilitiesTests
    {
        [TestMethod()]
        public void findListMatch_Start()
        {
            var target1 = new List<int>() { 1, 2, 3, 4, 5 };
            var match1 = new List<int>() { 1, 2, 3 };
            var got = Utilities.findListMatch(target1, match1);
            Assert.AreEqual(0, got);
        }

        [TestMethod()]
        public void findListMatch_Middle()
        {
            var target1 = new List<int>() { 1, 2, 3, 4, 5 };
            var match1 = new List<int>() { 3, 4 };
            var got = Utilities.findListMatch(target1, match1);
            Assert.AreEqual(2, got);
        }

        [TestMethod()]
        public void findListMatch_End()
        {
            var target1 = new List<int>() { 1, 2, 3, 4, 5 };
            var match1 = new List<int>() { 3, 4, 5 };
            var got = Utilities.findListMatch(target1, match1);
            Assert.AreEqual(2, got);
        }

        [TestMethod()]
        public void findListMatch_NoMatch()
        {
            var target1 = new List<int>() { 1, 2, 3, 4, 5 };
            var match1 = new List<int>() { 1, 4 };
            var got = Utilities.findListMatch(target1, match1);
            Assert.IsNull(got);
        }

        [TestMethod()]
        public void findListMatch_WithStart()
        {
            var target1 = new List<int>() { 1, 2, 3, 4, 5 };
            var match1 = new List<int>() { 4, 5 };
            var got = Utilities.findListMatch(target1, match1, 2);
            Assert.AreEqual(3, got);
        }

        [TestMethod()]
        public void findListMatch_WithReturnOffset()
        {
            var target1 = new List<int>() { 1, 2, 3, 4, 5 };
            var match1 = new List<int>() { 2, 3, 4 };
            var got = Utilities.findListMatch(target1, match1, 0, 2);
            Assert.AreEqual(3, got);
        }

        [TestMethod()]
        public void findListMatch_StartPosTooHigh()
        {
            var target1 = new List<int>() { 1, 2, 3, 4, 5 };
            var match1 = new List<int>() { 2, 3, 4 };
            var got = Utilities.findListMatch(target1, match1, 3);
            Assert.IsNull(got);
        }

        [TestMethod()]
        public void findListMatch_MatchLongerThanTarget()
        {
            var target1 = new List<int>() { 1, 2, 3, 4, 5 };
            var match1 = new List<int>() { 1, 2, 3, 4, 5, 6 };
            var got = Utilities.findListMatch(target1, match1);
            Assert.IsNull(got);
        }

        [TestMethod()]
        public void findListMatch_TargetIsEmpty()
        {
            var target1 = new List<int>() { };
            var match1 = new List<int>() { 1, 2, 3 };
            var got = Utilities.findListMatch(target1, match1);
            Assert.IsNull(got);
        }

        [TestMethod()]
        public void findListMatch_MatchIsEmpty()
        {
            var target1 = new List<int>() { 1, 2, 3 };
            var match1 = new List<int>() { };
            var got = Utilities.findListMatch(target1, match1);
            Assert.AreEqual(0, got);
        }

        [TestMethod()]
        public void findListMatch_MatchEqualsTarget()
        {
            var target1 = new List<int>() { 1, 2, 3 };
            var match1 = new List<int>() { 1, 2, 3 };
            var got = Utilities.findListMatch(target1, match1);
            Assert.AreEqual(0, got);
        }

        [TestMethod()]
        public void findListMatch_CodeInstructions()
        {
            var target1 = new List<CIL>()
            {
                new CIL(OpCodes.Brtrue_S, 6),
                new CIL(OpCodes.Ldarg_0),
                new CIL(OpCodes.Call, typeof(DialoguePatches).GetMethod("HasNotSpokenRecently")),
                new CIL(OpCodes.Brfalse, 12),
                new CIL(OpCodes.Ldstr, "hello"),
                new CIL(OpCodes.Call, typeof(DialoguePatches).GetMethod("AddToRecentTopicSpeakers"))
            };
            var match1 = new List<CIL>()
            {
                new CIL(OpCodes.Call, typeof(DialoguePatches).GetMethod("HasNotSpokenRecently")),
                new CIL(OpCodes.Brfalse, 12),
                new CIL(OpCodes.Ldstr, "hello")
            };
            var got = Utilities.findListMatch(target1, match1, 1, 2);
            Assert.AreEqual(4, got);
        }

        [TestMethod()]
        public void findListMatch_NoMatchCodeInstructions()
        {
            var target1 = new List<CIL>()
            {
                new CIL(OpCodes.Brtrue_S, 6),
                new CIL(OpCodes.Ldarg_0),
                new CIL(OpCodes.Call, typeof(DialoguePatches).GetMethod("HasNotSpokenRecently")),
                new CIL(OpCodes.Brfalse, 12),
                new CIL(OpCodes.Ldstr, "hello"),
                new CIL(OpCodes.Call, typeof(DialoguePatches).GetMethod("AddToRecentTopicSpeakers"))
            };
            var match1 = new List<CIL>()
            {
                new CIL(OpCodes.Call, typeof(DialoguePatches).GetMethod("AddToRecentTopicSpeakers")),
                new CIL(OpCodes.Brfalse, 12),
                new CIL(OpCodes.Ldstr, "hello")
            };
            var got = Utilities.findListMatch(target1, match1, 1, 2);
            Assert.IsNull(got);
        }

        [TestMethod()]
        public void findListMatch_NullMatchCodeInstructions()
        {
            var target1 = new List<CIL>()
            {
                new CIL(OpCodes.Brtrue_S, 6),
                new CIL(OpCodes.Ldarg_0),
                new CIL(OpCodes.Call, typeof(DialoguePatches).GetMethod("HasNotSpokenRecently")),
                new CIL(OpCodes.Brfalse, 12),
                new CIL(OpCodes.Ldstr, "hello"),
                new CIL(OpCodes.Call, typeof(DialoguePatches).GetMethod("AddToRecentTopicSpeakers"))
            };
            var match1 = new List<CIL>()
            {
                new CIL(OpCodes.Call, null),
                new CIL(OpCodes.Brfalse, 12),
                new CIL(OpCodes.Ldstr, null)
            };
            var got = Utilities.findListMatch(target1, match1, 1, 2);
            Assert.AreEqual(4, got);
        }
    }
}