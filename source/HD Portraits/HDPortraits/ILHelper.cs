/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HDPortraits
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HDPortraits
{
    class ILHelper
    {
        public readonly List<LocalBuilder> boxes;
        public readonly string name;
        public ILGenerator Generator
        {
            get { return currentGenerator; }
        }

        private enum ActionType { None, SkipTo, Add, AddF, Remove, RemoveTo, RemoveAt, RemoveL, Finish, Transform, InsertBefore };
        public delegate IEnumerable<CodeInstruction> Transformer(IList<CodeInstruction> instructions);

        private readonly List<(ActionType action, object arg)> actionQueue = new();
        private IEnumerable<CodeInstruction> instructions;
        private IEnumerator<CodeInstruction> cursor;
        private int actionIndex = 0;
        private bool hasErrored = false;
        private ILGenerator currentGenerator = null;

        public ILHelper(string Name)
        {
            name = Name;
            boxes = new();
        }
        /// <summary>Skips ahead</summary>
        /// <param name="count">Number of instructions to skip</param>
        /// <returns>this</returns>
        public ILHelper Skip(int count = 1)
        {
            actionQueue.Add((ActionType.None, count));
            return this;
        }
        /// <summary>Skips ahead, to after a set of marker instructions</summary>
        /// <param name="markers">Marker Instructions</param>
        /// <returns>this</returns>
        public ILHelper SkipTo(IList<CodeInstruction> markers)
        {
            actionQueue.Add((ActionType.SkipTo, markers));
            return this;
        }
        /// <summary>Adds some instructions</summary>
        /// <param name="codes">The instructions to add</param>
        /// <returns>this</returns>
        public ILHelper Add(IList<CodeInstruction> codes)
        {
            actionQueue.Add((ActionType.Add, codes));
            return this;
        }
        /// <summary>Adds an instruction</summary>
        /// <param name="code">The instruction to add</param>
        /// <returns>this</returns>
        public ILHelper Add(CodeInstruction code)
        {
            return Add(new CodeInstruction[] { code });
        }
        /// <summary>Adds some dynamically-generated instructions</summary>
        /// <param name="func">The instruction generator.</param>
        /// <returns>this</returns>
        public ILHelper Add(Func<IList<LocalBuilder>, IEnumerable<CodeInstruction>> func)
        {
            actionQueue.Add((ActionType.AddF, func));
            return this;
        }
        /// <summary>Removes some instructions</summary>
        /// <param name="count">How many to remove</param>
        /// <returns>this</returns>
        public ILHelper Remove(int count = 1)
        {
            actionQueue.Add((ActionType.Remove, count));
            return this;
        }
        /// <summary>Removes some instructions</summary>
        /// <param name="markers">A list of instructions to remove</param>
        /// <returns>this</returns>
        public ILHelper Remove(IList<CodeInstruction> markers)
        {
            actionQueue.Add((ActionType.RemoveL, markers));
            return this;
        }
        /// <summary>Remove up to, but not including, this set of instructions</summary>
        /// <param name="markers">The marker instructions</param>
        /// <returns>this</returns>
        public ILHelper RemoveTo(IList<CodeInstruction> markers)
        {
            actionQueue.Add((ActionType.RemoveTo, markers));
            return this;
        }
        /// <summary>Removes up to, and including, this set of instructions</summary>
        /// <param name="markers">The marker instructions</param>
        /// <returns>this</returns>
        public ILHelper RemoveAt(IList<CodeInstruction> markers)
        {
            actionQueue.Add((ActionType.RemoveAt, markers));
            return this;
        }
        /// <summary>Adds the rest of the original instructions as-is</summary>
        /// <returns>this</returns>
        public ILHelper Finish()
        {
            actionQueue.Add((ActionType.Finish, null));
            return this;
        }
        /// <summary>Allows dynamically replacing a set of instructions with new ones</summary>
        /// <param name="markers">The marker instructions</param>
        /// <param name="transformer">The method used to transform the markers into a new instruction set</param>
        /// <returns>this</returns>
        public ILHelper Transform(IList<CodeInstruction> markers, Transformer transformer)
        {
            actionQueue.Add((ActionType.Transform, (markers, transformer)));
            return this;
        }
        /// <summary>Inserts an instruction before the marker instructions</summary>
        /// <param name="code">The instruction to insert</param>
        /// <param name="markers">The marker instructions</param>
        /// <returns>this</returns>
        public ILHelper InsertBefore(CodeInstruction code, IList<CodeInstruction> markers)
        {
            return InsertBefore(new[] { code }, markers);
        }
        /// <summary>Inserts a set of instructions before the marker instructions</summary>
        /// <param name="codes">The instructions to insert</param>
        /// <param name="markers">The marker instructions</param>
        /// <returns>this</returns>
        public ILHelper InsertBefore(IList<CodeInstruction> codes, IList<CodeInstruction> markers)
        {
            actionQueue.Add((ActionType.InsertBefore, (codes, markers)));
            return this;
        }
        /// <summary>Resets the helper to a default state. Clears all defined actions. Do not use while patching, ever.</summary>
        public void Reset()
        {
            actionQueue.Clear();
            boxes.Clear();
        }
        /// <summary>Applies the patch to a given instruction set</summary>
        /// <param name="Instructions">The original instruction set</param>
        /// <param name="generator">the ILGenerator for this patch, if needed.</param>
        /// <returns>The applied patch.</returns>
        public IEnumerable<CodeInstruction> Run(IEnumerable<CodeInstruction> Instructions, ILGenerator generator = null)
        {
            ModEntry.monitor.Log("Now applying patch '" + name + "'...", LogLevel.Debug);
            currentGenerator = generator;
            instructions = Instructions;
            cursor = instructions.GetEnumerator();
            actionIndex = 0;
            hasErrored = false;
            foreach (var item in actionQueue)
            {
                int c = 0;
                int count = 0;
                switch (item.action)
                {
                    case ActionType.None:
                        count = (int)item.arg;
                        while (c < count && cursor.MoveNext())
                        {
                            yield return cursor.Current;
                            c++;
                        }
                        break;
                    case ActionType.SkipTo:
                        foreach (var code in skipTo((IList<CodeInstruction>)item.arg))
                            yield return code;
                        break;
                    case ActionType.Add:
                        foreach (var code in (IList<CodeInstruction>)item.arg)
                            yield return code;
                        break;
                    case ActionType.AddF:
                        foreach (var code in ((Func<IList<LocalBuilder>, IEnumerable<CodeInstruction>>)item.arg)(boxes))
                            yield return code;
                        break;
                    case ActionType.Remove:
                        count = (int)item.arg;
                        while (c < count && cursor.MoveNext())
                            c++;
                        break;
                    case ActionType.RemoveTo:
                        foreach (var code in removeTo((IList<CodeInstruction>)item.arg))
                            yield return code;
                        break;
                    case ActionType.RemoveAt:
                        foreach (var code in removeAt((IList<CodeInstruction>)item.arg))
                            yield return code;
                        break;
                    case ActionType.RemoveL:
                        foreach (var code in removeChunk((IList<CodeInstruction>)item.arg))
                            yield return code;
                        break;
                    case ActionType.Finish:
                        while (cursor.MoveNext())
                            yield return cursor.Current;
                        break;
                    case ActionType.Transform:
                        (var markers, var transformer) = (ValueTuple<IList<CodeInstruction>, Transformer>)item.arg;
                        foreach (var code in transform(markers, transformer))
                            yield return code;
                        break;
                    case ActionType.InsertBefore:
                        (var codes, var marker) = (ValueTuple<IList<CodeInstruction>, IList<CodeInstruction>>)item.arg;
                        foreach (var code in insertBefore(codes, marker))
                            yield return code;
                        break;
                }

                if (hasErrored)
                    break;
                actionIndex++;
            }
            currentGenerator = null;
            if (hasErrored)
                ModEntry.monitor.Log("Failed to correctly apply patch '" + name + "'! May cause problems!", LogLevel.Error);
            else
                ModEntry.monitor.Log("Successfully applied patch '" + name + "'.", LogLevel.Debug);
        }
        private IEnumerable<CodeInstruction> insertBefore(IList<CodeInstruction> codes, IList<CodeInstruction> Anchors)
        {
            int marker = 0;
            var saved = new CodeInstruction[Anchors.Count];
            while (cursor.MoveNext())
            {
                var s = Anchors[marker];
                var code = cursor.Current;
                if (code.opcode == s.opcode && CompareOperands(code.operand, s.operand))
                {
                    saved[marker] = code;
                    marker++;
                }
                else
                {
                    for (int i = 0; i < marker; i++)
                    {
                        yield return saved[i];
                        saved[i] = null;
                    }
                    yield return code;
                    marker = 0;
                }
                if (marker >= Anchors.Count)
                {
                    ModEntry.monitor.Log("Found markers for '" + name + "':" + actionIndex.ToString(), LogLevel.Trace);
                    foreach (var item in codes)
                        yield return item;
                    foreach (var item in saved)
                        yield return item;
                    yield break;
                }
            }
            hasErrored = true;
            ModEntry.monitor.Log("Failed to apply patch component '" + name + "':" + actionIndex.ToString() + " ; Marker instructions not found!", LogLevel.Error);
        }
        private IEnumerable<CodeInstruction> transform(IList<CodeInstruction> Anchors, Transformer transformer)
        {
            int marker = 0;
            var saved = new CodeInstruction[Anchors.Count];
            while (cursor.MoveNext())
            {
                var s = Anchors[marker];
                var code = cursor.Current;
                if (code.opcode == s.opcode && CompareOperands(code.operand, s.operand))
                {
                    saved[marker] = code;
                    marker++;
                }
                else
                {
                    for (int i = 0; i < marker; i++)
                    {
                        yield return saved[i];
                        saved[i] = null;
                    }
                    yield return code;
                    marker = 0;
                }
                if (marker >= Anchors.Count)
                {
                    ModEntry.monitor.Log("Found markers for '" + name + "':" + actionIndex.ToString(), LogLevel.Trace);
                    foreach (var item in transformer(saved))
                        yield return item;
                    yield break;
                }
            }
            hasErrored = true;
            ModEntry.monitor.Log("Failed to apply patch component '" + name + "':" + actionIndex.ToString() + " ; Marker instructions not found!", LogLevel.Error);
        }
        private IEnumerable<CodeInstruction> skipTo(IList<CodeInstruction> Anchors)
        {
            int marker = 0;
            while (cursor.MoveNext())
            {
                var s = Anchors[marker];
                var code = cursor.Current;
                if (code.opcode == s.opcode && CompareOperands(code.operand, s.operand))
                {
                    marker++;
                    if (code.operand is LocalBuilder b)
                        boxes.Add(b);
                }
                else
                {
                    boxes.Clear();
                    marker = 0;
                }
                yield return code;

                if (marker >= Anchors.Count)
                {
                    ModEntry.monitor.Log("Found markers for '" + name + "':" + actionIndex.ToString(), LogLevel.Trace);
                    yield break;
                }
            }
            hasErrored = true;
            ModEntry.monitor.Log("Failed to apply patch component '" + name + "':" + actionIndex.ToString() + " ; Marker instructions not found!", LogLevel.Error);
        }
        private IEnumerable<CodeInstruction> removeTo(IList<CodeInstruction> Anchors)
        {
            int marker = 0;
            var saved = new CodeInstruction[Anchors.Count];
            while (cursor.MoveNext())
            {
                var s = Anchors[marker];
                var code = cursor.Current;
                if (code.opcode == s.opcode && CompareOperands(code.operand, s.operand))
                {
                    saved[marker] = code;
                    marker++;
                    if (code.operand is LocalBuilder b)
                        boxes.Add(b);
                }
                else
                {
                    boxes.Clear();
                    marker = 0;
                }
                if (marker >= Anchors.Count)
                {
                    foreach (var inst in saved)
                    {
                        yield return inst;
                    }
                    ModEntry.monitor.Log("Found markers for '" + name + "':" + actionIndex.ToString(), LogLevel.Trace);
                    yield break;
                }
            }
            hasErrored = true;
            ModEntry.monitor.Log("Failed to apply patch component '" + name + "':" + actionIndex.ToString() + " ; Marker instructions not found!", LogLevel.Error);
        }
        private IEnumerable<CodeInstruction> removeAt(IList<CodeInstruction> Anchors)
        {
            int marker = 0;
            while (cursor.MoveNext())
            {
                var s = Anchors[marker];
                var code = cursor.Current;
                if (code.opcode == s.opcode && CompareOperands(code.operand, s.operand))
                {
                    marker++;
                    if (code.operand is LocalBuilder b)
                        boxes.Add(b);
                }
                else
                {
                    boxes.Clear();
                    marker = 0;
                }
                if (marker >= Anchors.Count)
                {
                    ModEntry.monitor.Log("Found markers for '" + name + "':" + actionIndex.ToString(), LogLevel.Trace);
                    yield break;
                }
            }
            hasErrored = true;
            ModEntry.monitor.Log("Failed to apply patch component '" + name + "':" + actionIndex.ToString() + " ; Marker instructions not found!", LogLevel.Error);
        }
        private IEnumerable<CodeInstruction> removeChunk(IList<CodeInstruction> Anchors)
        {
            int marker = 0;
            var saved = new CodeInstruction[Anchors.Count];
            while (cursor.MoveNext())
            {
                var s = Anchors[marker];
                var code = cursor.Current;
                if (code.opcode == s.opcode && CompareOperands(code.operand, s.operand))
                {
                    saved[marker] = code;
                    marker++;
                    if (code.operand is LocalBuilder b)
                        boxes.Add(b);
                }
                else
                {
                    boxes.Clear();
                    marker = 0;
                    for (int i = 0; i < marker; i++)
                    {
                        yield return saved[i];
                        saved[i] = null;
                    }
                    yield return code;
                }
                if (marker >= Anchors.Count)
                {
                    ModEntry.monitor.Log("Found markers for '" + name + "':" + actionIndex.ToString(), LogLevel.Trace);
                    yield break;
                }
            }
            hasErrored = true;
            ModEntry.monitor.Log("Failed to apply patch component '" + name + "':" + actionIndex.ToString() + " ; Marker instructions not found!", LogLevel.Error);
        }
        public Label? FindAddress(CodeInstruction[] Anchors)
        {
            int marker = 0;
            foreach (var code in instructions)
            {
                var s = Anchors[marker];
                if (code.opcode == s.opcode && CompareOperands(code.operand, s.operand))
                {
                    marker++;
                }
                else
                {
                    marker = 0;
                }
                if (marker >= Anchors.Length)
                {
                    return (code.labels.Count > 0) ? code.labels[0] : null;
                }
            }
            return null;
        }
        public static bool CompareOperands(object op1, object op2)
        {
            if (op1 == null || op1.Equals(op2))
                return true;

            if (op1 is sbyte bt && op2 is int bt2)
                return Convert.ToInt32(bt) == bt2;

            if (op1 is LocalBuilder oper1 && op2 is ValueTuple<int, Type> oper2)
            {
                return (oper2.Item1 < 0 || oper1.LocalIndex == oper2.Item1) && (oper2.Item2 == null || oper1.LocalType == oper2.Item2);
            }

            return false;
        }
    }
}
