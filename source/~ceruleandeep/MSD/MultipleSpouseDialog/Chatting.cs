/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

// a hack-up of: https://github.com/aedenthorn/StardewValleyMods/blob/master/MultipleSpouses/Kissing.cs

namespace MultipleSpouseDialog
{
    public class Chatting
    {
        private static readonly List<string> chattingSpouses = new();
        private static int lastChatTime;

        private static bool Eligible(NPC npc1, NPC npc2)
        {
            var M = ModEntry.PMonitor;
            if (npc1 is null || npc2 is null) return false;

            if (ModEntry.config.ExtraDebugOutput)
                M.Log($"Eligible(): checking for {npc1.Name} {npc2.Name}", LogLevel.Debug);

            var distance = Vector2.Distance(npc1.position, npc2.position);
            var roll = ModEntry.myRand.NextDouble();
            var lucky = roll < ModEntry.config.SpouseChatChance;
            var already = chattingSpouses.Contains(npc1.Name) || chattingSpouses.Contains(npc2.Name);
            var in_range = distance >= ModEntry.config.MinDistanceToChat &&
                           distance < ModEntry.config.MaxDistanceToChat;
            var married = npc1.getSpouse() != null && npc2.getSpouse() != null;
            var married_to_same = married && npc1.getSpouse().Name == npc2.getSpouse().Name;
            var too_soon = lastChatTime < ModEntry.config.MinSpouseChatInterval;
            var not_related = !ModEntry.config.PreventRelativesFromChatting ||
                              !Misc.AreSpousesRelated(npc1.Name, npc2.Name);
            var sleeping = npc1.isSleeping.Value || npc2.isSleeping.Value;

            if (ModEntry.config.ExtraDebugOutput)
            {
                if (!married)
                    M.Log($"    Ineligible: {npc1.Name} {npc2.Name}: not married", LogLevel.Debug);
                if (!married_to_same)
                    M.Log($"    Ineligible: {npc1.Name} {npc2.Name}: not married to same player", LogLevel.Debug);
                if (!in_range)
                    M.Log($"    Ineligible: {npc1.Name} {npc2.Name}: not in range", LogLevel.Debug);
                if (already)
                    M.Log($"    Ineligible: {npc1.Name} {npc2.Name}: NPCs already chatting", LogLevel.Debug);
                if (sleeping) 
                    M.Log($"    Ineligible: {npc1.Name} {npc2.Name}: sleeping", LogLevel.Debug);
                if (too_soon)
                    M.Log($"    Ineligible: {npc1.Name} {npc2.Name}: other chat recently", LogLevel.Debug);
                if (!lucky)
                    M.Log($"    Ineligible: {npc1.Name} {npc2.Name}: failed random roll", LogLevel.Debug);
                if (!not_related)
                    M.Log($"    Ineligible: {npc1.Name} {npc2.Name}: related", LogLevel.Debug);
            }
            

            var eligible = married
                           && married_to_same
                           && in_range
                           && !already
                           && !sleeping
                           && !too_soon
                           && lucky
                           && not_related;

            //if (ModEntry.config.ExtraDebugOutput) M.Log($"    Eligible() result: {npc1.Name} {npc2.Name}: {eligible}", LogLevel.Debug);
            return eligible;
        }

        private static bool Eligible(NPC npc1, Farmer npc2)
        {
            var distance = Vector2.Distance(npc1.position, npc2.position);
            var roll = ModEntry.myRand.NextDouble();
            var lucky = roll < ModEntry.config.SpouseChatChance;
            var already = chattingSpouses.Contains(npc1.Name) || chattingSpouses.Contains(npc2.Name);
            var in_range = distance >= ModEntry.config.MinDistanceToChat &&
                           distance < ModEntry.config.MaxDistanceToChat;
            var too_soon = lastChatTime < ModEntry.config.MinSpouseChatInterval;

            //if (ModEntry.config.ExtraDebugOutput) ModEntry.PMonitor.Log($"Eligible: married {npc1.getSpouse() != null} {npc2.getSpouse() != null}", LogLevel.Debug);
            //if (ModEntry.config.ExtraDebugOutput) ModEntry.PMonitor.Log($"Eligible: {in_range} {!already} {!npc1.isSleeping} {lastChatTime > ModEntry.config.MinSpouseChatInterval} {lucky}", LogLevel.Debug);

            return
                npc1.getSpouse() != null && npc2.getSpouse() != null
                                         && in_range
                                         && !already
                                         && !npc1.isSleeping.Value
                                         && !too_soon
                                         && lucky
                ;
        }

        public static void TryChat()
        {
            var location = Game1.currentLocation;
            if (location == null || !ReferenceEquals(location.GetType(), typeof(FarmHouse))) return;
            if (location.characters is null) return;

            lastChatTime++;
            if (lastChatTime >= ModEntry.config.MinSpouseChatInterval)
                chattingSpouses.Clear();

            var n_spouses = location.characters.ToList().Count;
            if (ModEntry.myRand.Next(n_spouses) == 0)
            {
                if (ModEntry.config.ChatWithPlayer) TryPlayerChat();
                TryNPCsChat();
            }
            else
            {
                TryNPCsChat();
                if (ModEntry.config.ChatWithPlayer) TryPlayerChat();
            }
        }

        private static void TryPlayerChat()
        {
            var location = Game1.currentLocation;
            var owner = (location as FarmHouse)?.owner;

            var list = location.characters.ToList();
            Misc.ShuffleList(ref list);

            foreach (var npc1 in from npc1 in list where owner == null || owner.friendshipData.ContainsKey(npc1.Name) where owner == null || owner.getFriendshipHeartLevelForNPC(npc1.Name) >= ModEntry.config.MinHeartsForChat where Eligible(npc1, owner) select npc1)
            {
                chattingSpouses.Add(npc1.Name);
                if (owner == null) continue;
                chattingSpouses.Add(owner.Name);
                if (ModEntry.config.ExtraDebugOutput)
                    ModEntry.PMonitor.Log(
                        $"Chatting: {npc1.Name} (faces {npc1.facingDirection}) {owner.Name} (faces {owner.facingDirection})",
                        LogLevel.Debug);
                lastChatTime = 0;
                int npc1face = npc1.facingDirection;
                int npc2face = owner.facingDirection;

                PerformChat(npc1, owner);

                Task.Run(async delegate
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    npc1.FacingDirection = npc1face;
                    owner.FacingDirection = npc2face;
                });
            }
        }

        private static void TryNPCsChat()
        {
            var location = Game1.currentLocation;
            var owner = (location as FarmHouse)?.owner;

            var list = location.characters.ToList();
            Misc.ShuffleList(ref list);

            foreach (var npc1 in list)
            {
                if (owner != null && !owner.friendshipData.ContainsKey(npc1.Name))
                    //if (ModEntry.config.ExtraDebugOutput) ModEntry.PMonitor.Log($"NPC {npc1.Name} is not friends with {owner.Name}", LogLevel.Debug);
                    continue;
                if (owner != null && owner.getFriendshipHeartLevelForNPC(npc1.Name) < ModEntry.config.MinHeartsForChat)
                    //if (ModEntry.config.ExtraDebugOutput) ModEntry.PMonitor.Log($"NPC {npc1.Name} has {owner.getFriendshipHeartLevelForNPC(npc1.Name)} hearts, needs {ModEntry.config.MinHeartsForChat}", LogLevel.Debug);
                    continue;

                foreach (var npc2 in list)
                {
                    if (npc1.Name == npc2.Name) continue;

                    if (owner != null && !owner.friendshipData.ContainsKey(npc2.Name))
                        //if (ModEntry.config.ExtraDebugOutput) ModEntry.PMonitor.Log($"NPC {npc2.Name} is not friends with {owner.Name}", LogLevel.Debug);
                        continue;
                    if (owner != null && owner.getFriendshipHeartLevelForNPC(npc2.Name) <
                            ModEntry.config.MinHeartsForChat)
                        //if (ModEntry.config.ExtraDebugOutput) ModEntry.PMonitor.Log($"NPC {npc1.Name} has {owner.getFriendshipHeartLevelForNPC(npc1.Name)} hearts, needs {ModEntry.config.MinHeartsForChat}", LogLevel.Debug);
                        continue;

                    if (ModEntry.config.ExtraDebugOutput) ModEntry.PMonitor.Log($"Considering NPCs for chat: {npc1.Name} {npc2.Name}", LogLevel.Debug);

                    if (Eligible(npc1, npc2))
                    {
                        chattingSpouses.Add(npc1.Name);
                        chattingSpouses.Add(npc2.Name);
                        if (ModEntry.config.ExtraDebugOutput)
                            ModEntry.PMonitor.Log(
                                $"Chatting: {npc1.Name} (faces {npc1.facingDirection}) {npc2.Name} (faces {npc2.facingDirection})", LogLevel.Debug);
                        lastChatTime = 0;
                        int npc1face = npc1.facingDirection;
                        int npc2face = npc2.facingDirection;

                        PerformChat(npc1, npc2);

                        Task.Run(async delegate
                        {
                            await Task.Delay(TimeSpan.FromSeconds(4));
                            npc1.FacingDirection = npc1face;
                            npc2.FacingDirection = npc2face;
                        });
                    }
                    else
                    {
                        if (ModEntry.config.ExtraDebugOutput) ModEntry.PMonitor.Log($"    * Ineligible: {npc1.Name} {npc2.Name}", LogLevel.Debug);
                    }
                }
            }
        }

        private static void PerformChat(NPC npc1, Character npc2)
        {
            var (x, _) = new Vector2((npc1.position.X + npc2.position.X) / 2, (npc1.position.Y + npc2.position.Y) / 2);
            NPC caller;
            Character responder;
            var caller_face_left = false;

            if (npc1.position.X < x)
            {
                caller = npc1;
                responder = npc2;
            }
            else if (npc2 is Farmer)
            {
                caller = npc1;
                responder = npc2;
                caller_face_left = true;
            }
            else
            {
                caller = npc2 as NPC;
                responder = npc1;
            }

            if (caller is null) return;

            ModEntry.PMonitor.Log($"PerformChat {caller.Name} (partner: {responder.Name})  Farmer? {npc2 is Farmer}");

            var d = Dialog.RandomDialog(caller.Name, responder.Name, npc2 is Farmer);
            var call = d.Call.Replace("@", responder.displayName);
            var response = d.Response.Replace("@", caller.displayName);

            if (ModEntry.config.ExtraDebugOutput)
                ModEntry.PMonitor.Log($"PerformChat {caller.Name}: {call} / {responder.Name}: {response}",
                    LogLevel.Debug);

            const int delay = 2000;

            caller.faceDirection(caller_face_left ? 3 : 1);
            caller.showTextAboveHead($"{call}", -1, 2, delay);
            caller.Sprite.UpdateSourceRect();

            if (responder is NPC rn)
            {
                rn.faceDirection(3);
                rn.showTextAboveHead($"{response}", -1, 2, delay, delay);
                rn.Sprite.UpdateSourceRect();

                caller.movementPause = delay * 2;
                rn.movementPause = delay * 2;
            }
            else
            {
                caller.movementPause = delay;
                ((Farmer) responder).doEmote(20);
            }
        }
    }
}