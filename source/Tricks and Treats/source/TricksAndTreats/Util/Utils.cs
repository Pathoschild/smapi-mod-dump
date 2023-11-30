/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cl4r3/Halloween-Mod-Jam-2023
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using static TricksAndTreats.Globals;

namespace TricksAndTreats
{
    internal static class Log
    {
        internal static void Error(string msg) => Monitor.Log(msg, StardewModdingAPI.LogLevel.Error);
        internal static void Alert(string msg) => Monitor.Log(msg, StardewModdingAPI.LogLevel.Alert);
        internal static void Warn(string msg) => Monitor.Log(msg, StardewModdingAPI.LogLevel.Warn);
        internal static void Info(string msg) => Monitor.Log(msg, StardewModdingAPI.LogLevel.Info);
        internal static void Debug(string msg) => Monitor.Log(msg, StardewModdingAPI.LogLevel.Debug);
        internal static void Trace(string msg) => Monitor.Log(msg, StardewModdingAPI.LogLevel.Trace);
        internal static void Verbose(string msg) => Monitor.VerboseLog(msg);

    }

    internal class Utils
    {
        public static void Speak(NPC npc, string key, bool clear = true)
        {
            if (clear)
                npc.CurrentDialogue.Clear();
            if (!string.IsNullOrWhiteSpace(key) && npc.Dialogue.TryGetValue(key, out string dialogue))
                npc.CurrentDialogue.Push(new Dialogue(dialogue, npc));
            else
                npc.CurrentDialogue.Push(new Dialogue(Helper.Translation.Get("generic." + key), npc));
            Game1.drawDialogue(npc);
        }

        internal static void ValidateNPCData()
        {
            foreach (KeyValuePair<string, Celebrant> entry in NPCData)
            {
                // Check that NPC exists
                if (Game1.getCharacterFromName(entry.Key, false, false) is null)
                {
                    Log.Trace($"TaT: Entry {entry.Key} in Trick-or-Treat NPC Data does not appear to be a valid NPC.");
                    NPCData.Remove(entry.Key);
                    continue;
                }

                // Check roles
                var roles = Array.ConvertAll(entry.Value.Roles, d => d.ToLower());
                if (roles.Except(ValidRoles).ToArray().Length > 0)
                {
                    Log.Warn($"NPC {entry.Key} has an invalid Trick-or-Treat role listed: " + roles.Except(ValidRoles).ToList());
                }
                NPCData[entry.Key].Roles = roles;

                // Check candygiver role
                if (entry.Value.TreatsToGive is not null && entry.Value.TreatsToGive.Length > 0)
                {
                    if (!NPCData[entry.Key].Roles.Contains("candygiver"))
                    {
                        Log.Warn($"NPC {entry.Key} has treats to give listed even though they do not have the role \"candygiver\", meaning they do not give candy.");
                    }
                    entry.Value.TreatsToGive = entry.Value.TreatsToGive.Where(x => { return TreatData.ContainsKey(x); }).ToArray();
                    if (entry.Value.TreatsToGive.Length < 1)
                        entry.Value.Roles = entry.Value.Roles.Where(x => { return x != "candygiver"; }).ToArray();
                }
                else if (NPCData[entry.Key].Roles.Contains("candygiver"))
                {
                    var treat = Helper.ModRegistry.IsLoaded("ch20youk.TaTPelicanTown.CP") ? "TaT.candy-corn" : "Maple Bar";
                    NPCData[entry.Key].TreatsToGive = Array.Empty<string>().Append(treat).ToArray();
                }

                // Check trickster role (and tricks)
                if (entry.Value.PreferredTricks is not null)
                {
                    if (!NPCData[entry.Key].Roles.Contains("trickster") && entry.Value.PreferredTricks.Length > 0)
                    {
                        Log.Warn($"NPC {entry.Key} has preferred tricks listed even though they do not have the role \"trickster\", meaning they do not pull tricks.");
                    }
                    else
                    {
                        var tricks = Array.ConvertAll(entry.Value.PreferredTricks, d => d.ToLower());
                        if (tricks.Contains("all"))
                            tricks = Config.SmallTricks.Keys.ToArray();
                        else
                            tricks = tricks.Distinct().ToArray();
                        var invalid_tricks = tricks.Where(x => !Config.SmallTricks.Keys.Contains(x)).ToArray();
                        if (invalid_tricks.Length > 0)
                        {
                            Log.Warn($"NPC {entry.Key} has invalid trick type listed: " + invalid_tricks.ToList());
                        }
                        NPCData[entry.Key].PreferredTricks = tricks.Except(invalid_tricks).ToArray();
                    }
                }
                else if (NPCData[entry.Key].Roles.Contains("trickster"))
                {
                    Log.Trace($"NPC {entry.Key} has no preferred tricks listed... Setting to all enabled tricks.");
                    NPCData[entry.Key].PreferredTricks = Config.SmallTricks.Keys.Where((string val) => { return Config.SmallTricks[val]; }).ToArray(); ;
                }
            }
        }

        internal static void ValidateCostumeData()
        {
            foreach (KeyValuePair<string, Costume> entry in CostumeData)
            {
                int count = 0;
                if (entry.Value is null)
                {
                    Log.Warn($"Could not find any data for costume set {entry.Key}.");
                    CostumeData.Remove(entry.Key);
                    continue;
                }
                if (entry.Value.Hat is not null && entry.Value.Hat.Length > 0 && (JA.GetHatId(entry.Value.Hat) >= 0 || HatInfo.ContainsKey(entry.Value.Hat)))
                    count++;
                else { CostumeData[entry.Key].Hat = ""; }
                if (entry.Value.Top is not null && entry.Value.Top.Length > 0 && (JA.GetClothingId(entry.Value.Top) >= 0 || ClothingInfo.ContainsKey(entry.Value.Top)))
                    count++;
                else { CostumeData[entry.Key].Top = ""; }
                if (entry.Value.Bottom is not null && entry.Value.Bottom.Length > 0 && (JA.GetClothingId(entry.Value.Bottom) >= 0 || ClothingInfo.ContainsKey(entry.Value.Bottom)))
                    count++;
                else { CostumeData[entry.Key].Bottom = ""; }

                if (count < 2)
                {
                    Log.Warn($"TaT: Removed costume set {entry.Key} with hat {entry.Value.Hat}, top {entry.Value.Top}, bottom {entry.Value.Bottom}");
                    CostumeData.Remove(entry.Key);
                    continue;
                }
                Log.Trace($"TaT: Registered costume set {entry.Key} with hat {entry.Value.Hat}, top {entry.Value.Top}, bottom {entry.Value.Bottom}");
                CostumeData[entry.Key].NumPieces = count;
            }
        }

        internal static void ValidateTreatData()
        {
            foreach (string name in TreatData.Keys)
            {
                if (string.IsNullOrEmpty(name))
                {
                    Log.Warn($"TaT: Treat was null or empty.");
                    TreatData.Remove(name);
                }
                var ja_id = JA.GetObjectId(name);
                TreatData[name].ObjectId = ja_id > -1 ? ja_id : FoodInfo[name];
                Log.Trace($"TaT: {name} has JA id {ja_id}, object ID {TreatData[name].ObjectId}");
                if (TreatData[name].ObjectId is null || TreatData[name].ObjectId < 0)
                {
                    Log.Warn($"TaT: No valid object ID found for treat {name}.");
                    TreatData.Remove(name);
                }
            }
        }
    }

}
