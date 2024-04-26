/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JudeRV/Stardew-ReportMobCounts
**
*************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

namespace ReportMobCounts
{
    class ModEntry : Mod
    {
        ModConfig? Config;

        KeybindList? toggleKey;
        bool displayNotifications;
        int notificationLength;

        bool printInConsole;
        bool printInChat;
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            toggleKey = Config.DisplayReportButton;
            displayNotifications = Config.DisplayReportsAsNotifications;
            notificationLength = Config.HowLongToDisplayNotificationsInMilliseconds;
            printInConsole = Config.PrintReportsToConsole;
            printInChat = Config.PrintReportsToInGameChat;

            Helper.Events.Player.Warped += OnPlayerWarped;
            Helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (toggleKey.JustPressed())
            {
                
            }
        }

        private void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            bool? isQuarryArea = null;
            try
            {
                isQuarryArea = Helper.Reflection.GetProperty<bool>(e.NewLocation as MineShaft, "isQuarryArea").GetValue();
            }
            catch (ArgumentNullException) { }
            Dictionary<string, int> monsterTypes = new();
            foreach (Monster monster in e.NewLocation.characters.OfType<Monster>())
            {
                if (monsterTypes.ContainsKey(monster.Name))
                {
                    monsterTypes[monster.Name]++;
                }
                else
                {
                    monsterTypes.Add(monster.Name, 1);
                }
            }
            foreach (KeyValuePair<string, int> kvp in new Dictionary<string, int>(monsterTypes))
            {
                if (kvp.Key == "Sludge")
                {
                    int value = kvp.Value;
                    monsterTypes.Remove("Sludge");
                    if (e.NewLocation is MineShaft ms)
                    {
                        if (isQuarryArea ?? false)
                        {
                            monsterTypes.Add("Slime", value);
                        }
                        else if (ms.mineLevel < 120)
                        {
                            monsterTypes.Add("Red Slime", value);
                        }
                        else
                        {
                            monsterTypes.Add("Purple Slime", value);
                        }
                    }
                }
                if (kvp.Key == "Green Slime")
                {
                    int value = kvp.Value;
                    monsterTypes.Remove("Green Slime");
                    monsterTypes.Add("Slime", value);
                }
                if (kvp.Key == "Prismatic Slime")
                {
                    PrintInGame($"{kvp.Value} Prismatic Slime detected!", Color.Purple);
                    monsterTypes.Remove("Prismatic Slime");
                    Game1.playSound("newRecord");
                }
            }
            foreach (KeyValuePair<string, int> kvp in monsterTypes)
            {
                PrintInGame($"{kvp.Value} {kvp.Key}{(kvp.Value > 1 ? "s" : "")} detected!");
            }
            if (monsterTypes.Count > 0)
            {
                PrintInGame("-----");
            }
        }

        private void PrintInGame(string msg, Color? color = null)
        {
            if (printInChat)
            {
                Game1.chatBox.addMessage(msg, color ?? Color.White);
            }
            if (printInConsole)
            {
                Monitor.Log(msg, LogLevel.Info);
            }
            if (displayNotifications)
            {
                Game1.addHUDMessage(new HUDMessage(msg, notificationLength));
            }
        }
    }

    class ModConfig
    {
        public KeybindList DisplayReportButton { get; set; } = KeybindList.Parse("Y");
        public bool DisplayReportsAsNotifications { get; set; } = true;
        public int HowLongToDisplayNotificationsInMilliseconds { get; set; } = 5000;
        public bool PrintReportsToInGameChat { get; set; } = false;
        public bool PrintReportsToConsole { get; set; } = false;
    }
}