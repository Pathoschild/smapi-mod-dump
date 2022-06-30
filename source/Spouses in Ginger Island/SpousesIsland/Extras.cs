/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using SpousesIsland.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpousesIsland
{
    public class Debugging
    {
        internal static void Reset(ModEntry me, string[] args, ModConfig c)
        {
            if (!Context.IsWorldReady && !args.Contains("debug"))
            {
                ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get("CLI.nosaveloaded"), LogLevel.Error);
                return;
            }
            else if (args.Count<string>() > 1 && args[1] is not "debug")
            {
                ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get($"CLI.TooManyArguments"), LogLevel.Info);
            }
            else if (!args.Any() || args[0] is "help" || args[0] is "h")
            {
                ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get("CLI.reset.description"), LogLevel.Info);
            }
            else
            {
                if (args[0] is "schedules" || args[0] is "s")
                {
                    int ResetCounter = 0;
                    foreach (string str in ModEntry.CustomSchedule.Keys)
                    {
                        ModEntry.ModHelper.GameContent.InvalidateCache($"Characters/schedules/{str}");
                        ResetCounter++;
                    }
                    ModEntry.ModMonitor.Log($"Reloaded {ResetCounter} schedules.", LogLevel.Info);
                }
                else if (args[0] is "dialogues" || args[0] is "d")
                {
                    int ResetCounter = 0;
                    int TlCounter = 0;
                    foreach (ContentPackData cpd in ModEntry.CustomSchedule.Values)
                    {
                        ModEntry.ModHelper.GameContent.InvalidateCache($"Characters/Dialogue/{cpd.Spousename}");
                        foreach (DialogueTranslation tl in cpd.Translations)
                        {
                            ModEntry.ModHelper.GameContent.InvalidateCache($"Characters/schedules/{cpd?.Spousename}{Commands.ParseLangCode(tl?.Key)}");
                            TlCounter++;
                        }
                        ResetCounter++;
                    }
                    ModEntry.ModMonitor.Log($"Reloaded {ResetCounter} default dialogues and {TlCounter} translations.", LogLevel.Info);
                }
                else if (args[0] is "packs" || args[0] is "p")
                {
                    ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get($"CLI.MustReset"), LogLevel.Warn);
                }
                else if (args[0] is "spouse" || args[0] is "spouses" || args[0] is "sp")
                {
                    ModEntry.ModMonitor.Log("Doing this may cause issues. It's recommended to restart the game instead to avoid bugs or crashes.", LogLevel.Warn);
                    ModEntry.ModMonitor.Log("Do you still want to continue? (click enter first, then type yes/no)", LogLevel.Info);
                    var keychosen = Console.ReadLine();
                    if (keychosen.ToLower() is "y" || keychosen.ToLower() is "yes")
                    {
                        ModEntry.ModMonitor.Log("Clearing Spouse info from mod...", LogLevel.Info);
                        me.CanEditSpouse.Clear();

                        ModEntry.ModMonitor.Log("Updating spouses list...", LogLevel.Info);
                        foreach (string s in me.IntegratedSpouses)
                        {
                            bool _LoadThisSpouse = SGIData.IsSpouseEnabled(s, c);
                            NPC SN = Game1.getCharacterFromName(s, false, false);
                            if (SN is not null)
                            {
                                bool _IsMarried;
                                if (SN.isMarriedOrEngaged() is true || SN.isRoommate() is true)
                                {
                                    _IsMarried = true;
                                }
                                else
                                {
                                    _IsMarried = false;
                                }

                                if (_IsMarried is true && _LoadThisSpouse is true)
                                {
                                    me.CanEditSpouse.Add(s, true);
                                    if (c.Verbose == true)
                                    {
                                        ModEntry.ModMonitor.Log($"Added {s} = true to CanEditSpouse dict");
                                    }
                                }
                                else
                                {
                                    me.CanEditSpouse.Add(s, false);
                                    if (c.Verbose == true)
                                    {
                                        ModEntry.ModMonitor.Log($"Added {s} = false to CanEditSpouse dict");
                                    }

                                }
                            }
                            else
                            {
                                me.CanEditSpouse.Add(s, false);
                                if (c.Verbose == true)
                                {
                                    ModEntry.ModMonitor.Log($"Added {s} = false to CanEditSpouse dict");
                                }

                            }
                        }
                        ModEntry.ModMonitor.Log("Done!", LogLevel.Info);
                    }
                    else
                    {
                        ModEntry.ModMonitor.Log("Operation cancelled.", LogLevel.Info);
                    }
                }
                else
                {
                    ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get($"CLI.InvalidValue"), LogLevel.Error);
                }
            }
        }
        internal static void List(ModEntry me, string[] args, ModConfig Config)
        {
            if (!Context.IsWorldReady && !args.Contains("debug"))
            {
                ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get("CLI.nosaveloaded"), LogLevel.Error);
                return;
            }
            else if (args.Count<string>() > 1 && args[1] is not "debug")
            {
                ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get($"CLI.TooManyArguments"), LogLevel.Info);
            }
            else if (!args.Any() || args[0] is "help" || args[0] is null)
            {
                ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get("CLI.list.description"), LogLevel.Info);
            }
            else
            {
                if (args[0] is "schedules" || args[0] is "s")
                {
                    string tempsched = "";
                    foreach (string s in me.SchedulesEdited)
                    {
                        tempsched = tempsched + $"\n   {s}";
                    }
                    ModEntry.ModMonitor.Log($"\n{ModEntry.ModHelper.Translation.Get("CLI.get.schedules")}\n{tempsched}", LogLevel.Info);
                }
                else if (args[0] is "dialogues" || args[0] is "d")
                {
                    string tempdial = "";
                    foreach (string s in me.DialoguesEdited)
                    {
                        tempdial = tempdial + $"\n   {s}";
                    }
                    ModEntry.ModMonitor.Log($"\n{ModEntry.ModHelper.Translation.Get("CLI.get.dialogues")}\n{tempdial}", LogLevel.Info);
                }
                else if (args[0] is "packs" || args[0] is "p")
                {
                    string tempkeys = "";
                    foreach (string s in ModEntry.CustomSchedule.Keys)
                    {
                        tempkeys = tempkeys + $"\n   {s}";
                    }
                    ModEntry.ModMonitor.Log($"{ModEntry.ModHelper.Translation.Get("CLI.get.packs")}\n{tempkeys}", LogLevel.Info);
                }
                else if (args[0] is "translations" || args[0] is "translation" || args[0] is "tl")
                {
                    string temptl = "";
                    foreach (string s in me.TranslationsAdded)
                    {
                        temptl = temptl + $"\n   {s}";
                    }
                    ModEntry.ModMonitor.Log($"{ModEntry.ModHelper.Translation.Get("CLI.get.packs")}\n{temptl}", LogLevel.Info);
                }
                else if (args[0] is "internal" || args[0] is "i")
                {
                    ModEntry.ModMonitor.Log($"Internal info: \n\n IsLeahMarried = {me.IsLeahMarried}; \n\n IsElliottMarried = {me.IsElliottMarried}; \n\n IsKrobusRoommate = {me.IsKrobusRoommate}; \n\n PreviousDayRandom = {me.PreviousDayRandom}; \n\n CCC = {me.CCC}; \n\n SawDevan4H = {me.SawDevan4H}; \n\n HasSVE = {me.HasSVE}; \n\n HasC2N = {me.HasC2N}; \n\n HasExGIM = {me.HasExGIM}; \n\n", LogLevel.Info);
                }
                else if (args[0] is "married" || args[0] is "m" || args[0] is "im")
                {
                    string tempM = "";
                    foreach (KeyValuePair<string, bool> kvp in me.CanEditSpouse)
                    {
                        tempM = tempM + $"\n   {kvp.Key}: {kvp.Value}";
                    }
                    ModEntry.ModMonitor.Log($"Is this character married?: \n{tempM}", LogLevel.Info);
                }
                else if (args[0] is "playerconfig" || args[0] is "pc")
                {
                    ModEntry.ModMonitor.Log($"Config: \n\n CustomChance: {Config.CustomChance}; \n\n ScheduleRandom = {Config.ScheduleRandom}; \n\n CustomRoom = {Config.CustomRoom}; \n\n BOOLSHERE \n\n Childbedcolor = {Config.Childbedcolor}; \n\n NPCDevan = {Config.NPCDevan}; \n\n Allow_Children = {Config.Allow_Children};", LogLevel.Info);
                }
                else
                {
                    ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get($"CLI.InvalidValue"), LogLevel.Error);
                }
            }
        }
        internal static void Chance(ModEntry me, string[] args, ModConfig Config)
        {
            if (!args.Any())
            {
                ModEntry.ModMonitor.Log($"{me.RandomizedInt}", LogLevel.Info);
            }
            else if (args.Contains<string>("debug"))
            {
                if (!Context.IsWorldReady)
                {
                    ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get("CLI.nosaveloaded"), LogLevel.Error);
                    return;
                }
                else
                {
                    ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get("CLI.Day0") + $": {me.PreviousDayRandom}", LogLevel.Info);
                    ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get("CLI.Day1") + $": {me.RandomizedInt}", LogLevel.Info);
                }
            }
            else if (args[0] is "set" && args[1].All(char.IsDigit))
            {
                var value = int.Parse(args[1]);
                if (value >= 0 && value <= 100)
                {
                    Config.CustomChance = value;
                    ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get($"CLI.ChangingCC") + Config.CustomChance + "%...", LogLevel.Info);
                }
                else
                {
                    ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get($"CLI.InvalidValue.CC"), LogLevel.Error);
                }
            }
            else
            {
                ModEntry.ModMonitor.Log(ModEntry.ModHelper.Translation.Get($"CLI.InvalidValue"), LogLevel.Error);
            }
        }
    }
    public class Titles
    {
        internal static string SpouseT()
        {
            var SpousesGrlTitle = "SDV";
            return SpousesGrlTitle;
        }
        internal static string SVET()
        {
            var sve = "SVE";
            return sve;
        }
        internal static string Debug()
        {
            var db = "Debug";
            return db;
        }
    }
}