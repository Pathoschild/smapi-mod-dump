/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Generics;
using AeroCore.Utils;
using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AeroCore
{
    public class ILHelper : IEnumerable<CodeInstruction>
    {
        #region head
        public delegate IList<CodeInstruction> Transformer(ILEnumerator enumer);
        public delegate IEnumerable<CodeInstruction> ComplexTransformer(ILEnumerator enumer);
        private readonly List<(int action, object arg)> actionQueue = new();
        public bool Debug = false;
        private readonly string name;
        private readonly IMonitor monitor;
        public ILGenerator generator;
        private IEnumerable<CodeInstruction> instructions;

        public IEnumerator<CodeInstruction> GetEnumerator() => new ILEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new ILEnumerator(this);

        /// <summary>Create a new IL Helper</summary>
        /// <param name="Monitor">Your mod's monitor</param>
        /// <param name="Name">The name of this patch. (Only used for logging.)</param>
        public ILHelper(IMonitor Monitor, string Name, bool debug = false)
        {
            monitor = Monitor;
            name = Name;
            Debug = debug;
        }

        /// <summary>Sets up the helper to run the patch</summary>
        /// <param name="instructions">The original instruction set</param>
        /// <param name="Generator">The associated <see cref="ILGenerator"/>. 
        /// Required if creating <see cref="Label"/>s or <see cref="LocalBuilder"/>s, otherwise optional.</param>
        /// <returns>This</returns>
        public ILHelper Run(IEnumerable<CodeInstruction> instructions, ILGenerator Generator = null)
        {
            this.instructions = instructions;
            generator = Generator;
            monitor.Log($"Patch '{name}' is now being applied...", LogLevel.Trace);
            return this;
        }

        /// <summary>Throughly compare the operands of two <see cref="CodeInstruction"/>s. Use (<see cref="int"/>, <see cref="Type"/>) tuples for locals.</summary>
        /// <param name="op1">Source operand</param>
        /// <param name="op2">Specified operand</param>
        public static bool CompareOperands(object op1, object op2)
        {
            if (op2 is null || op2.Equals(op1))
                return true;

            if (op1 is sbyte sb && Convert.ToInt32(sb).Equals(op2))
                return true;

            if (op1 is LocalBuilder oper1)
            {
                if (op2 is ValueTuple<int, Type> oper2)
                    return (oper2.Item1 < 0 || oper1.LocalIndex == oper2.Item1) && (oper2.Item2 == null || oper1.LocalType == oper2.Item2);
                else if (op2 is int val)
                    return val < 0 || oper1.LocalIndex == val;
            }
            return false;
        }

        /// <summary>Uses <see cref="CompareOperands(object, object)"/> to compare operands, and also compares operators and checks null</summary>
        /// <param name="src">Source instruction</param>
        /// <param name="def">Specified instruction</param>
        public static bool CompareInstructions(CodeInstruction src, CodeInstruction def)
        {
            return def is null || (def.opcode.Equals(src.opcode) && CompareOperands(src.operand, def.operand));
        }

        #endregion head
        #region queue
        /// <summary>Returns the remainder of the original method unaltered.</summary>
        public ILHelper Finish()
        {
            actionQueue.Add((0, null));
            return this;
        }

        /// <summary>Skip a number of instructions</summary>
        /// <param name="count">How many to skip</param>
        public ILHelper Skip(int count)
        {
            actionQueue.Add((1, count));
            return this;
        }

        /// <summary>Move forward to a specific instruction</summary>
        /// <param name="marker">The instruction</param>
        public ILHelper SkipTo(CodeInstruction marker) => SkipTo(new[] { marker });

        /// <summary>Move forward to the first of a specific set of instructions</summary>
        /// <param name="markers">The instructions</param>
        public ILHelper SkipTo(IList<CodeInstruction> markers)
        {
            actionQueue.Add((2, markers));
            return this;
        }

        /// <summary>Remove a certain number of instructions</summary>
        /// <param name="count">How many to remove</param>
        public ILHelper Remove(int count)
        {
            actionQueue.Add((3, count));
            return this;
        }

        /// <summary>Remove all instructions from the current point until a specific instruction</summary>
        /// <param name="marker">The instruction to stop at</param>
        public ILHelper RemoveTo(CodeInstruction marker) => RemoveTo(new[] { marker });

        /// <summary>Remove all instructions from the current point until a specific set of instructions</summary>
        /// <param name="marker">The instructions to stop just at</param>
        public ILHelper RemoveTo(IList<CodeInstruction> markers)
        {
            actionQueue.Add((4, markers));
            return this;
        }

        /// <summary>Add an instruction</summary>
        /// <param name="instruction">The instruction to add</param>
        public ILHelper Add(CodeInstruction instruction) => Add(new[]{ instruction });

        /// <summary>Add a set of instructions</summary>
        /// <param name="instructions">The instructions to add</param>
        public ILHelper Add(IList<CodeInstruction> instructions)
        {
            actionQueue.Add((5, instructions));
            return this;
        }

        /// <summary>
        /// Directly manipulate the instructions at the current point.
        /// The delegate accepts the cursor in its current state, and outputs a list of instructions to add.
        /// </summary>
        /// <param name="transformer">Transformation delegate</param>
        public ILHelper Transform(Transformer transformer)
        {
            actionQueue.Add((6, transformer));
            return this;
        }
        /// <summary>Add a named <see cref="Label"/> to the current instruction. See <see cref="ILEnumerator.CreateLabel"/></summary>
        /// <param name="id">The name of the previously-created <see cref="Label"/></param>
        public ILHelper AddLabel(string id) => AddLabels(new[] { id });

        /// <summary>Add a list of named <see cref="Label"/>s to the current instruction. See <see cref="ILEnumerator.CreateLabel"/></summary>
        /// <param name="ids">The list of names of previously-created <see cref="Label"/>s</param>
        public ILHelper AddLabels(IList<string> ids)
        {
            actionQueue.Add((7, ids));
            return this;
        }

        /// <summary>Creates a jump instruction pointing to a named label</summary>
        /// <param name="opcode">The <see cref="OpCode"/>. MUST be some kind of BR opcode, or the IL will be invalid</param>
        /// <param name="where">The name of the label to jump to</param>
        public ILHelper AddJump(OpCode opcode, string where)
        {
            actionQueue.Add((8, (opcode, where)));
            return this;
        }

        /// <summary>Add a set of instructions, and attach a set of labels to the first one</summary>
        /// <param name="instructions">The instructions to add</param>
        /// <param name="labels">The labels to attach</param>
        public ILHelper AddWithLabels(IList<CodeInstruction> instructions, IList<string> labels)
        {
            actionQueue.Add((9, (instructions, labels)));
            return this;
        }

        /// <summary>Add a set of instructions, and attach a label to the first one</summary>
        /// <param name="instructions">The instructions to add</param>
        /// <param name="label">The label to attach</param>
        public ILHelper AddWithLabels(IList<CodeInstruction> instructions, string label)
            => AddWithLabels(instructions, new[] { label });

        /// <summary>Add an instruction, and attach a label to it</summary>
        /// <param name="instruction">The instruction to add</param>
        /// <param name="label">The label to attach</param>
        public ILHelper AddWithLabels(CodeInstruction instruction, string label)
            => AddWithLabels(new[] {instruction}, new[] {label});

        /// <summary>Add an instruction, and attach a set of labels to it</summary>
        /// <param name="instruction">The instruction to add</param>
        /// <param name="labels">The labels to attach</param>
        public ILHelper AddWithLabels(CodeInstruction instruction, IList<string> labels)
            => AddWithLabels(new[] {instruction}, labels);

        /// <summary>Adds an instruction that stores to a named local</summary>
        /// <param name="Name">The name of the local</param>
        /// <param name="type">The type of the local</param>
        public ILHelper StoreLocal(string Name, Type type)
        {
            actionQueue.Add((10, (Name, type)));
            return this;
        }

        /// <summary>Adds an instruction that loads from a named local</summary>
        /// <param name="Name">The name of the local</param>
        public ILHelper LoadLocal(string Name, bool reference = false)
        {
            actionQueue.Add((11, (Name, reference)));
            return this;
        }

        /// <summary>
        /// Directly manipulate the instructions at the current point.
        /// The delegate accepts the cursor in its current state, and outputs a list of instructions to add.
        /// </summary>
        /// <param name="transformer">Transformation delegate</param>
        public ILHelper Transform(ComplexTransformer transformer)
        {
            actionQueue.Add((12, transformer));
            return this;
        }

        #endregion queue

        public class ILEnumerator : IEnumerator<CodeInstruction>
        {
            private delegate bool Mode(ILEnumerator e, ref CodeInstruction result);
            private static readonly Mode[] modes = 
                {Finish, Skip, SkipTo, Remove, RemoveTo, Add, Add, AddLabels, AddJump, AddWithLabels, StoreLocal, LoadLocal, TransformEnum};

            private bool disposedValue;
            public readonly BufferedEnumerator<CodeInstruction> source;
            private readonly ILHelper owner;
            public readonly ILGenerator gen;
            private readonly Dictionary<string, Label> labels = new();
            private readonly Dictionary<string, LocalBuilder> locals = new();

            private CodeInstruction current = null;
            private bool isSetup = false;
            private Mode mode;
            private int modeIndex = 0;
            private bool hasErrored = false;

            private int marker = 0;
            private IList<CodeInstruction> anchors;
            private CodeInstruction[] matched;
            private IList<string> labelsToAdd;
            private bool isLastItem = false;
            private OpCode jumpCode;
            private Type localtype;
            private IEnumerator<CodeInstruction> tenum;

            public CodeInstruction Current => current;
            object IEnumerator.Current => current;

            internal ILEnumerator(ILHelper Owner)
            {
                owner = Owner;
                gen = owner.generator;
                source = new(Owner.instructions.GetEnumerator());
            }

            public bool MoveNext()
            {
                if (isLastItem)
                {
                    owner.monitor.Log($"Patch '{owner.name}' was successfully applied.", LogLevel.Debug);
                    return false;
                }

                if (!isSetup)
                {
                    if (!gotoNextMode())
                    {
                        owner.monitor.Log($"Patch '{owner.name}' contains no operations! This will result in an empty output!", LogLevel.Error);
                        return false;
                    }
                    if (!source.MoveNext())
                    {
                        owner.monitor.Log($"Patch '{owner.name}' source instructions empty! Did you forget to Run()?", LogLevel.Error);
                        return false;
                    }
                    current = source.Current;
                    isSetup = true;
                }

                bool r = !hasErrored;
                if (r)
                {
                    while (mode.Invoke(this, ref current) && !hasErrored)
                        if (!gotoNextMode())
                        {
                            isLastItem = true;
                            break;
                        }
                    if (hasErrored)
                        owner.monitor.Log($"Patch '{owner.name}' was not applied correctly!", LogLevel.Error);
                    r = !hasErrored;
                }
                return r;
            }
            public void Reset()
            {
                source.Reset();
                modeIndex = 0;
                hasErrored = false;
                isSetup = false;
                labels.Clear();
                current = null;
                isLastItem = false;
            }

            /// <summary>Get a named <see cref="Label"/>, or create it if it does not exist.</summary>
            /// <param name="id">The name of the <see cref="Label"/></param>
            public Label GetOrCreateLabel(string id)
            {
                if (labels.TryGetValue(id, out var l))
                    return l;
                else
                    return CreateLabel(id);
            }

            /// <summary>Get a named <see cref="LocalBuilder"/>, or create it if it does not exist.</summary>
            /// <param name="id">The name of the <see cref="LocalBuilder"/></param>
            /// <param name="type">The type of the <see cref="LocalBuilder"/></param>
            public LocalBuilder GetOrCreateLocal(string id, Type type)
            {
                if (locals.TryGetValue(id, out var l))
                    return l;
                else
                    return CreateLocal(type, id);
            }

            /// <summary>Get a named <see cref="LocalBuilder"/></summary>
            /// <param name="id">The name of the <see cref="LocalBuilder"/> to get</param>
            /// <returns>The <see cref="LocalBuilder"/></returns>
            public LocalBuilder GetLocal(string id)
            {
                if (locals.TryGetValue(id, out var l))
                    return l;

                error($"Local with id '{id}' has not been created and does not exist");
                return default;
            }

            /// <summary>Create a new <see cref="LocalBuilder"/>. Can be named. Requires an <see cref="ILGenerator"/> to be provided in <see cref="Run"/></summary>
            /// <param name="type">The type of the <see cref="LocalBuilder"/></param>
            /// <param name="id">If included, the name of the <see cref="LocalBuilder"/></param>
            public LocalBuilder CreateLocal(Type type, string id = null)
            {
                if (gen is null)
                {
                    error("ILGenerator is required to create locals, but was not provided");
                    return default;
                }

                if (locals.TryGetValue(id, out var l))
                {
                    error($"Local with ID '{id}' already exists");
                    return l;
                }

                LocalBuilder local = gen.DeclareLocal(type);
                if (id is not null)
                    locals.Add(id, local);
                return local;
            }

            /// <summary>Create a new <see cref="Label"/>. Can be named. Requires an <see cref="ILGenerator"/> to be provided in <see cref="Run"/></summary>
            /// <param name="id">If included, the name of the <see cref="Label"/></param>
            public Label CreateLabel(string id = null)
            {
                if (gen is null)
                {
                    error("ILGenerator is required to create labels, but was not provided");
                    return default;
                }

                if (labels.TryGetValue(id, out var l))
                {
                    error($"Label with ID '{id}' already exists");
                    return l;
                }

                Label label = gen.DefineLabel();
                if (id is not null)
                    labels.Add(id, label);
                return label;
            }

            /// <summary>Get a named <see cref="Label"/></summary>
            /// <param name="id">The name of the <see cref="Label"/> to get</param>
            /// <returns>The <see cref="Label"/></returns>
            public Label GetLabel(string id)
            {
                if (labels.TryGetValue(id, out var l))
                    return l;
                else
                    error($"Label with id '{id}' has not been created and does not exist");
                return default;
            }
            private bool gotoNextMode()
            {
                if (modeIndex >= owner.actionQueue.Count)
                    return false;
                var (action, arg) = owner.actionQueue[modeIndex];
                mode = modes[action];
                marker = 0;

                switch (action)
                {
                    case 1 or 3: marker = (int)arg; break;
                    case 2 or 4:
                        anchors = (IList<CodeInstruction>)arg;
                        matched = new CodeInstruction[anchors.Count];
                        break;
                    case 5: anchors = (IList<CodeInstruction>)arg; break;
                    case 6: anchors = ((Transformer)arg).Invoke(this); break;
                    case 7: labelsToAdd = (IList<string>)arg; break;
                    case 8:
                        var (what, where) = ((OpCode, string))arg;
                        jumpCode = what;
                        labelsToAdd = new string[] {where};
                        break;
                    case 9:
                        var (codes, labels) = ((IList<CodeInstruction>, IList<string>))arg;
                        labelsToAdd = labels;
                        anchors = codes;
                        break;
                    case 10:
                        var (local, type) = ((string, Type))arg;
                        labelsToAdd = new[] {local};
                        localtype = type;
                        break;
                    case 11: 
                        var (name, isRef) = ((string, bool))arg;
                        labelsToAdd = new[] {name};
                        jumpCode = isRef ? OpCodes.Ldarga_S : OpCodes.Ldarg_S;
                        break;
                    case 12: tenum = ((ComplexTransformer)arg)(this).GetEnumerator(); break;
                }

                modeIndex++;
                return true;
            }
            private bool matchSequence()
            {
                int i = 0;
                bool r = true;
                bool ret = false;
                while(i < anchors.Count && r)
                {
                    matched[i] = source.Current;

                    if (owner.Debug && i > 0)
                        owner.monitor.Log(source.Current.ToString(), LogLevel.Debug);

                    i++;
                    if (!CompareInstructions(source.Current, anchors[i - 1]))
                    {
                        if (i > 1 && owner.Debug)
                            owner.monitor.Log("-------------------", LogLevel.Debug);
                        break;
                    }
                    ret = i >= anchors.Count;

                    if (owner.Debug && i == 1)
                        owner.monitor.Log(source.Current.ToString(), LogLevel.Debug);

                    if (i < anchors.Count)
                        r = source.MoveNext();
                }
                while(i > 0)
                {
                    i--;
                    source.Push(matched[i]);
                }
                source.MoveNext();
                return ret;
            }
            private void error(string reason)
            {
                hasErrored = true;
                owner.monitor.Log($"{reason}! @'{owner.name}':{modeIndex}", LogLevel.Error);
            }
            #region Modes
            private static bool Finish(ILEnumerator inst, ref CodeInstruction result)
            {
                result = inst.source.Current;
                return !inst.source.MoveNext();
            }
            private static bool Skip(ILEnumerator inst, ref CodeInstruction result)
            {
                result = inst.source.Current;
                if (inst.marker <= 0 || !inst.source.MoveNext())
                    return true;
                inst.marker--;
                return false;
            }
            private static bool SkipTo(ILEnumerator inst, ref CodeInstruction result)
            {
                bool v = inst.matchSequence();
                result = inst.source.Current;
                if (!v)
                {
                    if (!inst.source.MoveNext())
                    {
                        inst.error("Could not find marker instructions");
                        return true;
                    }
                }
                return v;
            }
            private static bool Remove(ILEnumerator inst, ref CodeInstruction result)
            {
                while (inst.marker > 0 && inst.source.MoveNext())
                    inst.marker--;
                result = inst.source.Current;
                return true;
            }
            private static bool RemoveTo(ILEnumerator inst, ref CodeInstruction result)
            {
                var v = true;
                while (v && !inst.matchSequence())
                    v = inst.source.MoveNext();
                if (!v)
                    inst.error("Could not find marker instructions");
                else
                    result = inst.source.Current;
                return true;
            }
            private static bool Add(ILEnumerator inst, ref CodeInstruction result)
            {
                if (inst.marker >= inst.anchors.Count)
                    return true;
                result = inst.anchors[inst.marker];
                inst.marker++;
                return false;
            }
            private static bool AddWithLabels(ILEnumerator inst, ref CodeInstruction result)
            {
                if (inst.marker == 0)
                {
                    if (inst.anchors.Count <= 0)
                        return Add(inst, ref result);
                    var what = inst.anchors[inst.marker];
                    foreach (string id in inst.labelsToAdd)
                        what.labels.Add(inst.GetOrCreateLabel(id));
                }
                return Add(inst, ref result);
            }
            private static bool AddLabels(ILEnumerator inst, ref CodeInstruction result)
            {
                if (result is null)
                    inst.error("Tried to add labels to null instruction");
                else
                    foreach (string id in inst.labelsToAdd)
                        result.labels.Add(inst.GetOrCreateLabel(id));
                return true;

            }
            private static bool AddJump(ILEnumerator inst, ref CodeInstruction result)
            {
                if (inst.marker > 0)
                    return true;

                inst.marker++;
                result = new(inst.jumpCode, inst.GetOrCreateLabel(inst.labelsToAdd[0]));
                return false;
            }
            private static bool StoreLocal(ILEnumerator inst, ref CodeInstruction result)
            {
                if (inst.marker > 0)
                    return true;

                inst.marker++;
                result = new(OpCodes.Stloc_S, inst.GetOrCreateLocal(inst.labelsToAdd[0], inst.localtype));
                return false;
            }
            private static bool LoadLocal(ILEnumerator inst, ref CodeInstruction result)
            {
                if (inst.marker > 0)
                    return true;

                inst.marker++;
                result = new(OpCodes.Ldloc_S, inst.GetLocal(inst.labelsToAdd[0]));
                return false;
            }
            private static bool TransformEnum(ILEnumerator inst, ref CodeInstruction result)
            {
                if (!inst.tenum.MoveNext())
                {
                    inst.tenum.Dispose();
                    inst.tenum = null;
                    return true;
                }
                result = inst.tenum.Current;
                return false;
            }
            #endregion Modes
            #region dispose
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }
                    disposedValue = true;
                }
            }
            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
            #endregion dispose
        }
    }
}
