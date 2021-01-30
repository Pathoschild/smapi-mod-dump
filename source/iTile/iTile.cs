/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0074 // Use compound assignment

using iTile.Core;
using StardewModdingAPI;

namespace iTile
{
    public class iTile : Mod
    {
        public static iTile Instance { get; protected set; }
        public static string ModID => _Helper.ModRegistry.ModID;

        public static IModHelper _Helper => Instance.Helper;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            CoreManager.Instance.Init();
        }

        public static void LogDebug(string msg)
        {
            Instance.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}