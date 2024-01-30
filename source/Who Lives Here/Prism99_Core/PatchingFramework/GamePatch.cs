/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/WhoLivesHere
**
*************************************************/

using System.Reflection;
using HarmonyLib;

namespace Prism99_Core.PatchingFramework
{
    class GamePatch
    {
        public bool IsPrefix { get; set; }
        public MethodBase Original { get; set; }
        public HarmonyMethod Target { get; set; }
        public string Description { get; set; }
        public bool Applied { get; set; }
        public bool Failed { get; set; }
        public string FailureDetails { get; set; }
        public int Priority { get; set; } = -1;
    }
}
