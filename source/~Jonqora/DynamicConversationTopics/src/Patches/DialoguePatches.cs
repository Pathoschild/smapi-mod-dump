using Harmony;
using CIL = Harmony.CodeInstruction;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Net;

namespace DynamicConversationTopics
{
    /// <summary>The class for patching methods related to NPC dialogue handling.</summary>
    public class DialoguePatches
    {
        /*********
        ** Accessors
        *********/
        private static IModHelper Helper => ModEntry.Instance.Helper;
        private static IMonitor Monitor => ModEntry.Instance.Monitor;
        private static HarmonyInstance Harmony => ModEntry.Instance.Harmony;

        internal protected static ModConfig Config => ModConfig.Instance;


        /*********
        ** Fields
        *********/

        //Criteria matching the section of CodeInstructions where we should exit the loop if HasSpokenRecently is true.
        //Desired index is 0 (the start of the function).
        static List<Func<CIL, bool>> skipCriteria = new List<Func<CIL, bool>>() { };

        //Criteria matching the section of CodeInstructions where we should add a call to AddToRecentTopicSpeakers.
        //Desired index is right after the Brtrue_S, so index 4 in this list.
        static List<Func<CIL, bool>> trackCriteria = new List<Func<CIL, bool>>()
        {
            //(C#)
            // if (!s.Contains("dumped"))
			// {
			//     Game1.player.mailReceived.Add(base.Name + "_" + eventMessageKey);
		    // }

            //(CIL)
            //ldloc.s s
            //ldstr "dumped"
            //callvirt instance bool [mscorlib]System.String::Contains(string)
            //brtrue.s IL_00c9
            //call class StardewValley.Farmer StardewValley.Game1::get_player()
            //ldfld class [Netcode]Netcode.NetStringList StardewValley.Farmer::mailReceived
            //ldarg.0
            //call instance string StardewValley.Character::get_Name()
            //ldstr "_"
            //ldloc.0
            //call string [mscorlib]System.String::Concat(string, string, string)
            //callvirt instance void class [Netcode]Netcode.NetList`2<string, class [Netcode]Netcode.NetString>::Add(!0)

            x => new List<OpCode>(){ OpCodes.Ldloc_0, OpCodes.Ldloc_1, OpCodes.Ldloc_2, OpCodes.Ldloc_3,
            OpCodes.Ldloc, OpCodes.Ldloc_S, }.Contains(x.opcode),
            x => x.opcode.Equals(OpCodes.Ldstr) && 
                x.operand.Equals("dumped"),
            x => x.opcode.Equals(OpCodes.Callvirt) && 
                x.operand.Equals(typeof(string).GetMethod("Contains", new Type[]{ typeof(string) })),
            x => new List<OpCode>(){ OpCodes.Brtrue, OpCodes.Brtrue_S }.Contains(x.opcode),
            x => x.opcode.Equals(OpCodes.Call) && 
                x.operand.Equals(typeof(Game1).GetMethod("get_player")),
            x => x.opcode.Equals(OpCodes.Ldfld) &&
                x.operand.Equals(typeof(Farmer).GetField("mailReceived")),
            x => new List<OpCode>(){ OpCodes.Ldarg_0, OpCodes.Ldarg_1, OpCodes.Ldarg_2, OpCodes.Ldarg_3,
            OpCodes.Ldarg_S, OpCodes.Ldarg }.Contains(x.opcode),
            x => x.opcode.Equals(OpCodes.Call) && 
                x.operand.Equals(typeof(Character).GetMethod("get_Name")),
            x => x.opcode.Equals(OpCodes.Ldstr) &&
                x.operand.Equals("_"),
            x => new List<OpCode>(){ OpCodes.Ldloc_0, OpCodes.Ldloc_1, OpCodes.Ldloc_2, OpCodes.Ldloc_3,
            OpCodes.Ldloc, OpCodes.Ldloca, OpCodes.Ldloc_S, OpCodes.Ldloca_S }.Contains(x.opcode),
            x => x.opcode.Equals(OpCodes.Call) &&
                x.operand.Equals(typeof(string).GetMethod("Concat", new Type[]{ typeof(string), typeof(string), typeof(string) })),
            x => x.opcode.Equals(OpCodes.Callvirt) &&
                x.operand.Equals(typeof(Netcode.NetList<string, Netcode.NetString>).GetMethod("Add", new Type[] { typeof(string)}))
        };

        //Criteria matching the CodeInstructions where we should identify the Leave operand label and use it to exit the loop
        //Desired index is Leave_S, so index 3 in this list.
        static List<Func<CIL, bool>> targetCriteria = new List<Func<CIL, bool>>()
        {
            //ldloca.s 2
            //call instance bool valuetype [Netcode]Netcode.NetDictionary`5/KeysCollection/Enumerator<string, int32, class [Netcode]Netcode.NetInt, class StardewValley.SerializableDictionary`2<string, int32>, class StardewValley.Network.NetStringDictionary`2<int32, class [Netcode]Netcode.NetInt>>::MoveNext()
            //brtrue IL_0023
            //leave.s IL_00ed

            x => new List<OpCode>(){ OpCodes.Ldloca, OpCodes.Ldloca_S }.Contains(x.opcode),
            x => x.opcode.Equals(OpCodes.Call) &&
                x.operand.Equals(typeof(Netcode.NetDictionary<string, int, Netcode.NetInt, SerializableDictionary<string, int>, StardewValley.Network.NetStringDictionary<int, Netcode.NetInt>>.KeysCollection.Enumerator).GetMethod("MoveNext")),
            x => new List<OpCode>(){ OpCodes.Brtrue, OpCodes.Brtrue_S }.Contains(x.opcode),
            x => new List<OpCode>(){ OpCodes.Leave, OpCodes.Leave_S }.Contains(x.opcode)
        };


        /*********
        ** Public methods
        *********/
        /// <summary>
        /// Applies the harmony patches defined in this class.
        /// </summary>
        public static void Apply()
        {
            // Add checks to limit conversation topic dialogues so they aren't all displayed one after the other.
            Harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkForNewCurrentDialogue)),
                transpiler: new HarmonyMethod(AccessTools.Method(
                    typeof(DialoguePatches),
                    nameof(DialoguePatches.checkForNewCurrentDialogue_Transpiler)
                    ))
                );
        }

        /// <summary>
        /// Finds any sequence matching target code, then adds conditional skip logic and tracking to conversation topic dialogue checker.
        /// </summary>
        /// <param name="instructions">Harmony-provided CodeInstruction enumerable for the original checkForNewCurrentDialogue method</param>
        /// <returns>Altered CodeInstruction enumerable if a location was found and patches applied; else returns original codes</returns>
        public static IEnumerable<CIL> checkForNewCurrentDialogue_Transpiler(IEnumerable<CIL> instructions)
        {
            try
            {
                var codes = new List<CIL>(instructions);

                int? insertSkipLocation = Utilities.findSublist(codes, skipCriteria);
                int? insertTrackLocation = insertSkipLocation == null ? null : Utilities.findSublist(codes, trackCriteria, insertSkipLocation.Value, 4);
                int? findTargetLocation = insertTrackLocation == null ? null : Utilities.findSublist(codes, targetCriteria, insertTrackLocation.Value, 3);

                if (insertSkipLocation != null &&
                    insertTrackLocation != null &&
                    findTargetLocation != null)
                {
                    // Debug output
                    Monitor.Log($"Found patch location for {nameof(checkForNewCurrentDialogue_Transpiler)}:\n" +
                        $"insertSkipLocation = {insertSkipLocation}\n" +
                        $"insertTrackLocation = {insertTrackLocation}\n" +
                        $"findTargetLocation = {findTargetLocation}", Config.DebugMode ? LogLevel.Debug : LogLevel.Trace);

                    // Identify the target label needed for our new conditional branch
                    Label skipTargetLabel = (Label)codes[findTargetLocation.Value].operand;

                    // Compose new instructions for tracking recent NPCs who gave conversation topic dialogue
                    // AddToRecentTopicSpeakers(this)
                    var trackCodesToInsert = new List<CIL>
                    {
                        new CIL(OpCodes.Ldarg_0),
                        new CIL(OpCodes.Call, Helper.Reflection.GetMethod(
                            typeof(DialoguePatches),nameof(AddToRecentTopicSpeakers)).MethodInfo)
                    };

                    // Compose new instructions to skip checking for conversation topic dialogue if spoken recently
                    // if (!HasSpokenRecently(this))
                    // {
                    //   foreach (string s in Game1.player.activeDialogueEvents.Keys)
                    //   {
                    //     ...
                    //   }
                    // }
                    // ...
                    var skipCodesToInsert = new List<CIL>
                    {
                        new CIL(OpCodes.Ldarg_0),
                        new CIL(OpCodes.Call, Helper.Reflection.GetMethod(
                            typeof(DialoguePatches),nameof(HasSpokenRecently)).MethodInfo),
                        new CIL(OpCodes.Brtrue, skipTargetLabel)
                    };

                    // Inject the instruction sets (last section to first to avoid index mixups)
                    codes.InsertRange(insertTrackLocation.Value, trackCodesToInsert);
                    codes.InsertRange(insertSkipLocation.Value, skipCodesToInsert);

                    Monitor.LogOnce($"Applied harmony patch to class NPC: {nameof(checkForNewCurrentDialogue_Transpiler)}", LogLevel.Trace);
                    return codes.AsEnumerable();
                }
                else
                {
                    // Debug output
                    Monitor.Log($"Failed to find patch location for {nameof(checkForNewCurrentDialogue_Transpiler)}:\n" +
                        $"insertSkipLocation = {insertSkipLocation}\n" +
                        $"insertTrackLocation = {insertTrackLocation}\n" +
                        $"findTargetLocation = {findTargetLocation}", Config.DebugMode ? LogLevel.Debug : LogLevel.Trace);

                    Monitor.Log($"Couldn't apply harmony patch to class NPC: {nameof(checkForNewCurrentDialogue_Transpiler)}\n" +
                    $"The quality-of-life feature to space out topic dialogues will be inactive, but all main features of this mod should continue to work correctly.\n" +
                    $"You don't need to worry about this error in your game, but please report it to the mod author anyway for bugtesting.", LogLevel.Warn);
                    return instructions; // use original code if could not apply both edits correctly
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(checkForNewCurrentDialogue_Transpiler)}:\n{ex}", LogLevel.Error);
                return instructions; // use original code
            }
        }

        /// <summary>
        /// Checks to see if the player has already seen conversation topic dialogue from an NPC recently.
        /// </summary>
        /// <param name="npc">NPC to check</param>
        /// <returns>true if NPC found in RecentTopicSpeakers</returns>
        public static bool HasSpokenRecently(NPC npc)
        {
            try
            {
                return ModEntry.Instance.RecentTopicSpeakers.ContainsKey(npc.Name);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(HasSpokenRecently)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        /// <summary>
        /// Adds an NPC to RecentTopicSpeakers with the current integer game time.
        /// </summary>
        /// <param name="npc">NPC to add</param>
        public static void AddToRecentTopicSpeakers(NPC npc)
        {
            try
            {
                string name = npc.Name;
                int gametime = Game1.timeOfDay;
                ModEntry.Instance.RecentTopicSpeakers[name] = gametime;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(AddToRecentTopicSpeakers)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
