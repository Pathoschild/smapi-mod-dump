/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

// TODO: AssertIs?
// Label stuff?
// MAKE SURE THE LABEL COUNTS ARE RIGHT. Inserting codes should add to the Important Labels! Check **any time** labels are removed.
// Insert should probably just have a pattern that moves over the labels....
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

using AtraBase.Collections;

using CommunityToolkit.Diagnostics;

using HarmonyLib;

namespace AtraShared.Utils.HarmonyHelper;

/// <summary>
/// Helper class for transpilers.
/// </summary>
public sealed class ILHelper
{
    // All locals.
    private readonly SortedList<int, LocalVariableInfo> locals = new();

    private readonly Counter<Label> importantLabels = new();

    private Label? label = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ILHelper"/> class.
    /// </summary>
    /// <param name="original">Original method's MethodBase.</param>
    /// <param name="codes">IEnumerable of codes.</param>
    /// <param name="monitor">Logger.</param>
    /// <param name="generator">ILGenerator.</param>
    public ILHelper(MethodBase original, IEnumerable<CodeInstruction> codes, IMonitor monitor, ILGenerator generator)
    {
        // scan method body and get the original variables.
        if (original.GetMethodBody() is MethodBody body)
        {
            foreach (LocalVariableInfo loc in body.LocalVariables)
            {
                this.locals[loc.LocalIndex] = loc;
            }
        }
        else
        {
            ThrowHelper.ThrowInvalidOperationException($"Attempted to transpile a method without a body: {original.FullDescription()}");
        }

        this.Original = original;
        this.Codes = codes.ToList();
        this.Generator = generator;
        this.Monitor = monitor;

        // update the variables with the ones mods have added
        // and also take count of the labels.
        foreach (CodeInstruction code in this.Codes)
        {
            if (code.operand is LocalBuilder builder)
            {
                this.locals.TryAdd(builder.LocalIndex, builder); // LocalBuilder inherits from LocalVariableInfo
            }
            if (code.Branches(out Label? label))
            {
                this.importantLabels[label!.Value]++;
            }
            if (code.opcode == OpCodes.Switch)
            {
                foreach (Label switchLabel in (Label[])code.operand)
                {
                    this.importantLabels[switchLabel]++;
                }
            }
        }
    }

    #region properties

    /// <summary>
    /// Gets the original MethodBase.
    /// </summary>
    public MethodBase Original { get; init; }

    /// <summary>
    /// Gets the list of codes.
    /// </summary>
    /// <remarks>Try not to use this.</remarks>
    public List<CodeInstruction> Codes { get; init; }

    /// <summary>
    /// Gets the ILGenerator.
    /// </summary>
    public ILGenerator Generator { get; init; }

    /// <summary>
    /// Gets the current instruction pointer stack.
    /// </summary>
    public Stack<int> PointerStack { get; private set; } = new();

    /// <summary>
    /// Points to the current location in the instructions list.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors", Justification = "Reviewed.")]
    public int Pointer { get; private set; } = -1;

    /// <summary>
    /// Gets the current instruction.
    /// </summary>
    public CodeInstruction CurrentInstruction
    {
        get => this.Codes[this.Pointer];
        private set => this.Codes[this.Pointer] = value;
    }

    /// <summary>
    /// Gets the logger for this instance.
    /// </summary>
    private IMonitor Monitor { get; init; }

    #endregion

    #region static methods

    /// <summary>
    /// Gets the instruction for loading a local at the index.
    /// </summary>
    /// <param name="localindex">Index of the local to get.</param>
    /// <returns>The proper local instruction.</returns>
    public static CodeInstruction GetLdLoc(int localindex)
    {
        return localindex switch
        {
            0 => new(OpCodes.Ldloc_0),
            1 => new(OpCodes.Ldloc_1),
            2 => new(OpCodes.Ldloc_2),
            3 => new(OpCodes.Ldloc_3),
            _ => new(OpCodes.Ldloc, localindex)
        };
    }

    /// <summary>
    /// Gets the instruction for storing to a local at the index.
    /// </summary>
    /// <param name="localindex">Index of the local to get.</param>
    /// <returns>The proper local instruction.</returns>
    public static CodeInstruction GetStLoc(int localindex)
    {
        return localindex switch
        {
            0 => new(OpCodes.Stloc_0),
            1 => new(OpCodes.Stloc_1),
            2 => new(OpCodes.Stloc_2),
            3 => new(OpCodes.Stloc_3),
            _ => new(OpCodes.Stloc, localindex)
        };
    }

    #endregion

    #region jumps

    /// <summary>
    /// Pushes the pointer onto the pointer stack.
    /// </summary>
    /// <returns>this.</returns>
    public ILHelper Push()
    {
        this.PointerStack.Push(this.Pointer);
        return this;
    }

    /// <summary>
    /// Pops the a pointer from the pointer stack.
    /// </summary>
    /// <returns>this.</returns>
    public ILHelper Pop()
    {
        this.Pointer = this.PointerStack.Pop();
        return this;
    }

    /// <summary>
    /// Moves the pointer to a specific index.
    /// </summary>
    /// <param name="index">Index to move to.</param>
    /// <returns>this.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Tried to move to an invalid location.</exception>
    public ILHelper JumpTo(int index)
    {
        Guard.IsBetweenOrEqualTo(index, 0, this.Codes.Count - 1);
        this.Pointer = index;
        return this;
    }

    /// <summary>
    /// Moves the pointer forward the number of steps.
    /// </summary>
    /// <param name="steps">Number of steps.</param>
    /// <returns>this.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Pointer tried to move to an invalid location.</exception>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public ILHelper Advance(int steps)
    {
        this.Pointer += steps;
        Guard.IsBetweenOrEqualTo(this.Pointer, 0, this.Codes.Count - 1);
        return this;
    }

    #endregion

    // TODO: Consider doing basic stack checking here.

    /// <summary>
    /// The list of codes as an enumerable.
    /// </summary>
    /// <returns>Returns the list of codes as an enumerable.</returns>
    public IEnumerable<CodeInstruction> Render() =>
        this.Codes.AsEnumerable();

    /// <summary>
    /// Prints out the current codes to console.
    /// Only works in DEBUG.
    /// </summary>
    [DebuggerHidden]
    [Conditional("DEBUG")]
    public void Print()
    {
        StringBuilder sb = new();
        sb.Append("ILHelper for: ").AppendLine(this.Original.FullDescription());
        sb.Append("With locals: ").AppendJoin(", ", this.locals.Values.Select((LocalVariableInfo loc) => $"{loc.LocalIndex}+{loc.LocalType.Name}"));
        for (int i = 0; i < this.Codes.Count; i++)
        {
            sb.AppendLine().Append(this.Codes[i]);
            if (this.Pointer == i)
            {
                sb.Append("       <----");
            }
            if (this.PointerStack.Contains(i))
            {
                sb.Append("       <----- stack point.");
            }
        }
        this.Monitor.Log(sb.ToString(), LogLevel.Info);
    }

    #region search

    /// <summary>
    /// Finds the first occurrence of the following pattern between the indexes given.
    /// </summary>
    /// <param name="instructions">Instructions to search for.</param>
    /// <param name="startindex">Index to start searching at (inclusive).</param>
    /// <param name="intendedendindex">Index to end search (exclusive). Null for "end of instruction list".</param>
    /// <returns>this.</returns>
    /// <exception cref="ArgumentException">StartIndex or EndIndex are invalid.</exception>
    /// <exception cref="InvalidOperationException">No match found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public ILHelper FindFirst(CodeInstructionWrapper[] instructions, int startindex = 0, int? intendedendindex = null)
    {
        int endindex = intendedendindex ?? this.Codes.Count;
        if (startindex >= (endindex - instructions.Length) || startindex < 0 || endindex > this.Codes.Count)
        {
            return ThrowHelper.ThrowArgumentException<ILHelper>($"Either the start index {startindex} or the end index {endindex} are invalid.");
        }

        for (int i = startindex; i < endindex - instructions.Length + 1; i++)
        {
            for (int j = 0; j < instructions.Length; j++)
            {
                if (!this.IsMatch(instructions[j], this.Codes[i + j]))
                {
                    goto ContinueSearchForward;
                }
            }
            this.Pointer = i;
            return this;
ContinueSearchForward:
            ;
        }
        this.Monitor.Log($"The desired pattern wasn't found:\n\n" + string.Join('\n', instructions.Select(i => i.ToString())), LogLevel.Error);
        return ThrowHelper.ThrowInvalidOperationException<ILHelper>();
    }

    /// <summary>
    /// Finds the first occurrence of the following pattern between the indexes given.
    /// </summary>
    /// <param name="instruction">Instruction to search for.</param>
    /// <param name="startindex">Index to start searching at (inclusive).</param>
    /// <param name="intendedendindex">Index to end search (exclusive). Null for "end of instruction list".</param>
    /// <returns>this.</returns>
    /// <exception cref="ArgumentException">StartIndex or EndIndex are invalid.</exception>
    /// <exception cref="InvalidOperationException">No match found.</exception>
    public ILHelper FindFirst(CodeInstructionWrapper instruction, int startindex = 0, int? intendedendindex = null)
    {
        int endindex = intendedendindex ?? this.Codes.Count;
        if (startindex >= (endindex - 1) || startindex < 0 || endindex > this.Codes.Count)
        {
            return ThrowHelper.ThrowArgumentException<ILHelper>($"Either the start index {startindex} or the end index {endindex} are invalid.");
        }
        for (int i = startindex; i < endindex; i++)
        {
            if (this.IsMatch(instruction, this.Codes[i]))
            {
                this.Pointer = i;
                return this;
            }
        }
        this.Monitor.Log($"The desired pattern wasn't found: {instruction}", LogLevel.Error);
        return ThrowHelper.ThrowInvalidOperationException<ILHelper>();
    }

    /// <summary>
    /// Finds the next occurrence of the code instruction.
    /// </summary>
    /// <param name="instructions">Instructions to search for.</param>
    /// <returns>this.</returns>
    /// <exception cref="ArgumentException">Fewer codes remain than the length of the instructions to search for.</exception>
    /// <exception cref="InvalidOperationException">No match found.</exception>
    public ILHelper FindNext(CodeInstructionWrapper[] instructions)
        => this.FindFirst(instructions, this.Pointer + 1, this.Codes.Count);

    /// <summary>
    /// Finds the last occurrence of the following pattern between the indexes given.
    /// </summary>
    /// <param name="instructions">Instructions to search for.</param>
    /// <param name="startindex">Index to start searching at (inclusive).</param>
    /// <param name="intendedendindex">Index to end search (exclusive). Leave null to mean "last code".</param>
    /// <returns>this.</returns>
    /// <exception cref="ArgumentException">StartIndex or EndIndex are invalid.</exception>
    /// <exception cref="InvalidOperationException">No match found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public ILHelper FindLast(CodeInstructionWrapper[] instructions, int startindex = 0, int? intendedendindex = null)
    {
        int endindex = intendedendindex ?? this.Codes.Count;
        if (startindex >= endindex - instructions.Length || startindex < 0 || endindex > this.Codes.Count)
        {
            return ThrowHelper.ThrowArgumentException<ILHelper>($"Either {nameof(startindex)} {startindex} or {nameof(endindex)} {endindex} are invalid. ");
        }
        for (int i = endindex - instructions.Length - 1; i >= startindex; i--)
        {
            for (int j = 0; j < instructions.Length; j++)
            {
                if (!this.IsMatch(instructions[j], this.Codes[i + j]))
                {
                    goto ContinueSearchBackwards;
                }
            }
            this.Pointer = i;
            return this;
ContinueSearchBackwards:
            ;
        }
        this.Monitor.Log($"The desired pattern wasn't found:\n\n" + string.Join('\n', instructions.Select(i => i.ToString())), LogLevel.Error);
        return ThrowHelper.ThrowInvalidOperationException<ILHelper>();
    }

    /// <summary>
    /// Finds the previous occurrence of the code instruction.
    /// </summary>
    /// <param name="instructions">Instructions to search for.</param>
    /// <returns>this.</returns>
    /// <exception cref="ArgumentException">Fewer codes remain than the length of the instructions to search for.</exception>
    /// <exception cref="InvalidOperationException">No match found.</exception>
    public ILHelper FindPrev(CodeInstructionWrapper[] instructions)
        => this.FindLast(instructions, 0, this.Pointer);

    #endregion

    #region manipulation

    /// <summary>
    /// Inserts the following code instructions at this location.
    /// </summary>
    /// <param name="instructions">Instructions to insert.</param>
    /// <param name="withLabels">Labels to attach to the first instruction.</param>
    /// <returns>this.</returns>
    public ILHelper Insert(CodeInstruction[] instructions, IList<Label>? withLabels = null)
    {
        this.Codes.InsertRange(this.Pointer, instructions);
        if (withLabels is not null)
        {
            this.CurrentInstruction.labels.AddRange(withLabels);
        }
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.Branches(out Label? label))
            {
                this.importantLabels[label!.Value]++;
            }
        }
        this.Pointer += instructions.Length;
        return this;
    }

    /// <summary>
    /// Removes the following number of instructions, starting from and including the current instruction.
    /// </summary>
    /// <param name="count">Number to remove.</param>
    /// <returns>this.</returns>
    /// <exception cref="InvalidOperationException">Attempted to remove an important label, stopping.</exception>
    public ILHelper Remove(int count)
    {
        for (int i = this.Pointer; i < this.Pointer + count; i++)
        {
            if (this.Codes[i].Branches(out Label? label))
            {
                this.importantLabels[label!.Value]--;
            }
            else if (this.Codes[i].opcode == OpCodes.Switch)
            {
                foreach (Label switchLabel in (Label[])this.Codes[i].operand)
                {
                    this.importantLabels[switchLabel]--;
                }
            }
        }
        this.importantLabels.RemoveZeros();
        for (int i = this.Pointer; i < this.Pointer + count; i++)
        {
            if (this.Codes[i].labels.Intersect(this.importantLabels.Keys).Any())
            {
                StringBuilder sb = new();
                sb.Append("Attempted to remove an important label!\n\nThis code's labels: ")
                    .AppendJoin(", ", this.Codes[i].labels.Select(l => l.ToString()))
                    .AppendLine().Append("Important labels: ")
                    .AppendJoin(", ", this.importantLabels.Select(l => l.ToString()));
                this.Monitor.Log(sb.ToString(), LogLevel.Error);
                return ThrowHelper.ThrowInvalidOperationException<ILHelper>();
            }
        }
        this.Codes.RemoveRange(this.Pointer, count);
        return this;
    }

    /// <summary>
    /// Removes instructions until it encounters the *first* instruction of a specific pattern.
    /// </summary>
    /// <param name="instructions">List of instructions to search for.</param>
    /// <returns>this.</returns>
    /// <exception cref="InvalidOperationException">Attempted to remove an important label, stopping.</exception>
    public ILHelper RemoveUntil(CodeInstructionWrapper[] instructions)
    {
        this.Push();
        this.FindNext(instructions);
        int finalpos = this.Pointer;
        this.Pop();
        this.Remove(finalpos - this.Pointer);
        return this;
    }

    /// <summary>
    /// Removes instructions, up to and including the match pattern.
    /// </summary>
    /// <param name="instructions">List of instructions to search for.</param>
    /// <returns>this.</returns>
    /// <exception cref="InvalidOperationException">Attempted to remove an important label, stopping.</exception>
    public ILHelper RemoveIncluding(CodeInstructionWrapper[] instructions)
    {
        this.Push();
        this.FindNext(instructions);
        int finalpos = this.Pointer + instructions.Length;
        this.Pop();
        this.Remove(finalpos - this.Pointer);
        return this;
    }

    /// <summary>
    /// Grabs a copy of the instructions, starting at the startpos.
    /// </summary>
    /// <param name="startpos">Starting position (inclusive).</param>
    /// <param name="count">Number of codes to grab.</param>
    /// <param name="codes">The codes.</param>
    /// <returns>this.</returns>
    /// <remarks>All labels and exception blocks are removed.</remarks>
    public ILHelper Copy(int startpos, int count, out IEnumerable<CodeInstruction> codes)
    {
        codes = this.Codes.GetRange(startpos, count).Select((code) => code.Clone());
        return this;
    }

    /// <summary>
    /// Grabs a copy of the instructions, starting at the current pointer location (inclusive).
    /// </summary>
    /// <param name="count">Number of codes to grab.</param>
    /// <param name="codes">The codes.</param>
    /// <returns>this.</returns>
    /// <remarks>All labels and exception blocks are removed.</remarks>
    public ILHelper Copy(int count, out IEnumerable<CodeInstruction> codes)
        => this.Copy(this.Pointer, count, out codes);

    /// <summary>
    /// Grabs a copy of the instructions, starting at the current pointer location, and going to the search pattern.
    /// Does not include the search pattern.
    /// </summary>
    /// <param name="instructions">Pattern to search for.</param>
    /// <param name="codes">Copied codes.</param>
    /// <returns>this.</returns>
    /// <remarks>All labels and exception blocks are removed.</remarks>
    public ILHelper CopyUntil(CodeInstructionWrapper[] instructions, out IEnumerable<CodeInstruction> codes)
    {
        this.Push();
        this.FindNext(instructions);
        int finalpos = this.Pointer;
        this.Pop();
        this.Copy(finalpos - this.Pointer, out codes);
        return this;
    }

    /// <summary>
    /// Copies a block that starts from the current instruction and includes the search instructions.
    /// </summary>
    /// <param name="instructions">Instructions to search for.</param>
    /// <param name="codes">Copied codes.</param>
    /// <returns>this.</returns>
    /// <remarks>All labels and exception blocks are removed.</remarks>
    public ILHelper CopyIncluding(CodeInstructionWrapper[] instructions, out IEnumerable<CodeInstruction> codes)
    {
        this.Push();
        this.FindNext(instructions);
        int finalpos = this.Pointer + instructions.Length;
        this.Pop();
        this.Copy(finalpos - this.Pointer, out codes);
        return this;
    }

    /// <summary>
    /// Replace the current instruction with the given instruction.
    /// </summary>
    /// <param name="instruction">Instruction to replace.</param>
    /// <param name="withLabels">Labels to attach, if any.</param>
    /// <param name="keepLabels">Whether or not to keep the original labels. Default: true.</param>
    /// <returns>this.</returns>
    public ILHelper ReplaceInstruction(CodeInstruction instruction, Label[] withLabels, bool keepLabels = true)
    {
        this.ReplaceInstruction(instruction, keepLabels);
        this.CurrentInstruction.labels.AddRange(withLabels);
        return this;
    }

    /// <summary>
    /// Replaces the current instruction with the given instruction.
    /// </summary>
    /// <param name="opcode">Opcode.</param>
    /// <param name="operand">Operand.</param>
    /// <param name="withLabels">Labels to attach.</param>
    /// <param name="keepLabels">Whether or not to keep the original labels. Default: true.</param>
    /// <returns>this.</returns>
    public ILHelper ReplaceInstruction(OpCode opcode, object operand, Label[] withLabels, bool keepLabels = true)
        => this.ReplaceInstruction(new CodeInstruction(opcode, operand), withLabels, keepLabels);

    /// <summary>
    /// Replaces an instruction with a different instruction.
    /// </summary>
    /// <param name="instruction">Instruction to replace with.</param>
    /// <param name="keepLabels">Whether or not to keep the labels.</param>
    /// <returns>this.</returns>
    /// <exception cref="InvalidOperationException">Tried to remove an important label.</exception>
    public ILHelper ReplaceInstruction(CodeInstruction instruction, bool keepLabels = true)
    {
        if (keepLabels)
        {
            instruction.labels.AddRange(this.CurrentInstruction.labels);
        }

        // removed a branch, so un-track those labels.
        if (this.CurrentInstruction.Branches(out Label? currlabel))
        {
            this.importantLabels[currlabel!.Value]--;
        }
        else if (this.CurrentInstruction.opcode == OpCodes.Switch)
        {
            foreach (Label switchLabel in (Label[])this.CurrentInstruction.operand)
            {
                this.importantLabels[switchLabel]--;
            }
        }
        this.importantLabels.RemoveZeros();

        if (!keepLabels && this.CurrentInstruction.labels.Intersect(this.importantLabels.Keys).Any())
        {
            StringBuilder sb = new();
            sb.Append("Attempted to remove an important label!\n\nThis code's labels: ")
                .AppendJoin(", ", this.CurrentInstruction.labels.Select(l => l.ToString()))
                .AppendLine().Append("Important labels: ")
                .AppendJoin(", ", this.importantLabels.Select(l => l.ToString()));
            this.Monitor.Log(sb.ToString(), LogLevel.Error);
            return ThrowHelper.ThrowInvalidOperationException<ILHelper>();
        }

        // track new labels.
        if (instruction.Branches(out Label? label))
        {
            this.importantLabels[label!.Value]++;
        }
        else if (this.CurrentInstruction.opcode == OpCodes.Switch)
        {
            foreach (Label switchLabel in (Label[])this.CurrentInstruction.operand)
            {
                this.importantLabels[switchLabel]++;
            }
        }
        this.CurrentInstruction = instruction;
        return this;
    }

    /// <summary>
    /// Replaces an instruction with a different instruction.
    /// </summary>
    /// <param name="opcode">Opcode.</param>
    /// <param name="operand">Operand.</param>
    /// <param name="keepLabels">Whether or not to keep the labels.</param>
    /// <returns>this.</returns>
    /// <exception cref="InvalidOperationException">Tried to remove an important label.</exception>
    public ILHelper ReplaceInstruction(OpCode opcode, object? operand, bool keepLabels = true)
        => this.ReplaceInstruction(new CodeInstruction(opcode, operand), keepLabels);

    /// <summary>
    /// Replaces the operand.
    /// </summary>
    /// <param name="operand">New operand.</param>
    /// <returns>this.</returns>
    public ILHelper ReplaceOperand(object operand)
    {
        if (this.CurrentInstruction.Branches(out Label? label))
        {
            this.importantLabels[label!.Value]--;
        }
        else if (this.CurrentInstruction.opcode == OpCodes.Switch)
        {
            foreach (Label switchLabel in (Label[])this.CurrentInstruction.operand)
            {
                this.importantLabels[switchLabel]--;
            }
        }

        if (operand is Label newlabel)
        {
            this.importantLabels[newlabel]++;
        }
        else if (operand is Label[] newlabels)
        {
            foreach (Label newSwitchLabel in newlabels)
            {
                this.importantLabels[newSwitchLabel]++;
            }
        }
        this.importantLabels.RemoveZeros();
        this.CurrentInstruction.operand = operand;
        return this;
    }

    #endregion

    #region labels

    /// <summary>
    /// Grab branch destination.
    /// </summary>
    /// <param name="label">Label branches to.</param>
    /// <returns>this.</returns>
    /// <exception cref="InvalidOperationException">Attempted to call this not on a branch.</exception>
    public ILHelper GrabBranchDest(out Label? label)
    {
        if (!this.CurrentInstruction.Branches(out label))
        {
            return ThrowHelper.ThrowInvalidOperationException<ILHelper>($"Attempted to grab label from something that's not a branch.");
        }
        return this;
    }

    /// <summary>
    /// When called on a branch, stores the label branched to.
    /// </summary>
    /// <returns>this.</returns>
    public ILHelper StoreBranchDest()
        => this.GrabBranchDest(out this.label);

    /// <summary>
    /// Gets the labels from a certain instruction. (Primarily used for moving labels).
    /// </summary>
    /// <param name="labels">out labels.</param>
    /// <param name="clear">whether or not to clear the labels.</param>
    /// <returns>this.</returns>
    /// <remarks>DOES NOT CHECK LABELS! YOU SHOULD PROBABLY PUT THEM BACK SOMEWHERE if cleared.</remarks>
    public ILHelper GetLabels(out IList<Label> labels, bool clear = true)
    {
        labels = this.CurrentInstruction.labels.ToList();
        if (clear)
        {
            this.CurrentInstruction.labels.Clear();
        }
        return this;
    }

    /// <summary>
    /// Attaches the labels to the current instruction.
    /// </summary>
    /// <param name="labels">Labels to attach.</param>
    /// <returns>this.</returns>
    public ILHelper AttachLabel(params Label[] labels)
    {
        this.CurrentInstruction.labels.AddRange(labels);
        return this;
    }

    /// <summary>
    /// Attaches the labels to the current instruction.
    /// </summary>
    /// <param name="labels">Labels to attach.</param>
    /// <returns>this.</returns>
    public ILHelper AttachLabels(IEnumerable<Label> labels)
    {
        this.CurrentInstruction.labels.AddRange(labels);
        return this;
    }

    /// <summary>
    /// Defines a new label and attaches it to the current instruction.
    /// </summary>
    /// <param name="label">The label produced.</param>
    /// <returns>this.</returns>
    public ILHelper DefineAndAttachLabel(out Label label)
    {
        label = this.Generator.DefineLabel();
        this.CurrentInstruction.labels.Add(label);
        return this;
    }

    /// <summary>
    /// Finds the first usage of a label in the following section.
    /// </summary>
    /// <param name="label">Label to search for.</param>
    /// <param name="startindex">Index to start searching at (inclusive).</param>
    /// <param name="intendedendindex">Index to end search (exclusive). Leave null to mean "last code".</param>
    /// <returns>this.</returns>
    /// <exception cref="ArgumentException">StartIndex or EndIndex are invalid.</exception>
    /// <exception cref="InvalidOperationException">No match found.</exception>
    public ILHelper FindFirstLabel(Label label, int startindex = 0, int? intendedendindex = null)
    {
        int endindex = intendedendindex ?? this.Codes.Count;
        if (startindex >= endindex || startindex < 0 || endindex > this.Codes.Count)
        {
            return ThrowHelper.ThrowArgumentException<ILHelper>($"Either {nameof(startindex)} {startindex} or {nameof(endindex)} {endindex} are invalid.");
        }
        for (int i = startindex; i < endindex; i++)
        {
            if (this.Codes[i].labels.Contains(label))
            {
                this.Pointer = i;
                return this;
            }
        }
        return ThrowHelper.ThrowInvalidOperationException<ILHelper>($"label {label} could not be found between {startindex} and {endindex}");
    }

    /// <summary>
    /// Moves pointer forward to the label.
    /// </summary>
    /// <param name="label">Label to search for.</param>
    /// <returns>this.</returns>
    /// <exception cref="IndexOutOfRangeException">No match found.</exception>
    public ILHelper AdvanceToLabel(Label label)
        => this.FindFirstLabel(label, this.Pointer + 1, this.Codes.Count);

    /// <summary>
    /// Advances to the stored label.
    /// </summary>
    /// <returns>this.</returns>
    /// <exception cref="InvalidOperationException">No label stored.</exception>
    /// <exception cref="IndexOutOfRangeException">No match found.</exception>
    public ILHelper AdvanceToStoredLabel()
    {
        if (this.label is null)
        {
            return ThrowHelper.ThrowInvalidOperationException<ILHelper>("Attempted to advance to label, but there is not one stored!");
        }
        return this.AdvanceToLabel(this.label.Value);
    }

    /// <summary>
    /// Finds the last usage of a label in the following section.
    /// </summary>
    /// <param name="label">Label to search for.</param>
    /// <param name="startindex">Index to start searching at (inclusive).</param>
    /// <param name="intendedendindex">Index to end search (exclusive). Leave null to mean "last code".</param>
    /// <returns>this.</returns>
    /// <exception cref="ArgumentException">StartIndex or EndIndex are invalid.</exception>
    /// <exception cref="IndexOutOfRangeException">No match found.</exception>
    public ILHelper FindLastLabel(Label label, int startindex = 0, int? intendedendindex = null)
    {
        int endindex = intendedendindex ?? this.Codes.Count;
        if (startindex >= endindex || startindex < 0 || endindex > this.Codes.Count)
        {
            return ThrowHelper.ThrowArgumentException<ILHelper>($"Either {nameof(startindex)} {startindex} or {nameof(endindex)} {endindex} are invalid.");
        }
        for (int i = endindex - 1; i >= startindex; i--)
        {
            if (this.Codes[i].labels.Contains(label))
            {
                this.Pointer = i;
                return this;
            }
        }
        return ThrowHelper.ThrowInvalidOperationException<ILHelper>($"label {label} could not be found between {startindex} and {endindex}");
    }

    /// <summary>
    /// Moves pointer backwards to the label.
    /// </summary>
    /// <param name="label">Label to search for.</param>
    /// <returns>this.</returns>
    /// <exception cref="IndexOutOfRangeException">No match found.</exception>
    public ILHelper RetreatToLabel(Label label)
        => this.FindLastLabel(label, 0, this.Pointer);

    /// <summary>
    /// Retreat to the stored label.
    /// </summary>
    /// <returns>this.</returns>
    /// <exception cref="InvalidOperationException">No label stored.</exception>
    /// <exception cref="IndexOutOfRangeException">No match found.</exception>
    public ILHelper RetreatToStoredLabel()
    {
        if (this.label is null)
        {
            return ThrowHelper.ThrowInvalidOperationException<ILHelper>("Attempted to advance to label, but there is not one stored!");
        }
        return this.RetreatToLabel(this.label.Value);
    }

    #endregion

    #region locals

    /// <summary>
    /// Declares a local and adds it to the list to be tracked.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="local">Out param - the local.</param>
    /// <param name="pinned">Whether to pin the local or not.</param>
    /// <returns>this.</returns>
    public ILHelper DeclareLocal(Type type, out LocalBuilder local, bool pinned = false)
    {
        local = this.Generator.DeclareLocal(type, pinned);
        this.locals.Add(local.LocalIndex, local);
        return this;
    }

    /// <summary>
    /// Gets the index of the local of a specific type.
    /// </summary>
    /// <param name="type">Type to search for.</param>
    /// <param name="which">If there's multiple locals of a single type, which one.</param>
    /// <returns>Index of the local, -1 if not found.</returns>
    public int GetIndexOfLocal(Type type, int which = 1)
    {
        int counter = 0;
        foreach ((int key, LocalVariableInfo local) in this.locals)
        {
            if (local.LocalType == type && ++counter == which)
            {
                return local.LocalIndex;
            }
        }
        return -1;
    }

    #endregion

    // transformer should return true to continue and false to stop?
    // and throw errors if it runs into issues.
    // TODO: consider checking the state of the stack. Transformers should match pops and pushes...

    /// <summary>
    /// For each match found, run the transformer given.
    /// Transformer should take the helper as the sole argument and either return true (to continue matching) or false (to end).
    /// Be careful with the pointer-stack and make sure to restore it.
    /// </summary>
    /// <param name="instructions">Instruction set to match against.</param>
    /// <param name="transformer">
    /// Lambda to apply after each match. The lambda should take in the helper as the only parameter.
    /// It can return true to continue or return false to end this command.
    /// </param>
    /// <param name="startindex">Index to start at.</param>
    /// <param name="intendedendindex">Index to end search at. Leave as null to mean "end of instructions".</param>
    /// <param name="maxCount">Maximum number of times to apply the lambda.</param>
    /// <returns>this.</returns>
    /// <exception cref="ArgumentException">Indexes are out of bounds.</exception>
    public ILHelper ForEachMatch(
        CodeInstructionWrapper[] instructions,
        Func<ILHelper, bool> transformer,
        int startindex = 0,
        int? intendedendindex = null,
        int maxCount = int.MaxValue) // if I have to repeat more than int.MaxValue times something has gone very wrong.
    {
        int count = 0;
        int endindex = intendedendindex ?? this.Codes.Count;
        if (startindex >= endindex - instructions.Length || startindex < 0 || endindex > this.Codes.Count)
        {
            return ThrowHelper.ThrowArgumentException<ILHelper>($"Either {nameof(startindex)} {startindex} or {nameof(endindex)} {endindex} are invalid.");
        }
        this.Push();
        for (int i = startindex; i < endindex; i++)
        {
            for (int j = 0; j < instructions.Length; j++)
            {
                if (!this.IsMatch(instructions[j], this.Codes[i + j]))
                {
                    goto ContinueSearch;
                }
            }
            this.Pointer = i;
            if (!transformer(this) || ++count >= maxCount)
            {
                break;
            }
ContinueSearch:
            ;
        }
        this.Monitor.Log($"ForEachMatch found {count} occurrences for {string.Join(", ", instructions.Select(i => i.ToString()))} for {this.Original.FullDescription()}.", LogLevel.Trace);
        this.Pop();
        return this;
    }

    /// <summary>
    /// Handles matching wrappers against instructions. Has special handling for the first three locals.
    /// </summary>
    /// <param name="wrapper">The CodeInstructionWrapper.</param>
    /// <param name="instruction">The instruction to match against.</param>
    /// <returns>True if matches, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private bool IsMatch(CodeInstructionWrapper wrapper, CodeInstruction instruction)
    {
        if (wrapper.Matches(instruction))
        {
            return true;
        }
        else if (wrapper.LocalType is not null)
        {
            if (instruction.opcode == OpCodes.Ldloc_0 || instruction.opcode == OpCodes.Stloc_0)
            {
                return this.locals.TryGetValue(0, out LocalVariableInfo? loc) && loc.LocalType == wrapper.LocalType;
            }
            else if (instruction.opcode == OpCodes.Ldloc_1 || instruction.opcode == OpCodes.Stloc_1)
            {
                return this.locals.TryGetValue(1, out LocalVariableInfo? loc) && loc.LocalType == wrapper.LocalType;
            }
            else if (instruction.opcode == OpCodes.Ldloc_2 || instruction.opcode == OpCodes.Stloc_2)
            {
                return this.locals.TryGetValue(2, out LocalVariableInfo? loc) && loc.LocalType == wrapper.LocalType;
            }
            else if (instruction.opcode == OpCodes.Ldloc_3 || instruction.opcode == OpCodes.Stloc_3)
            {
                return this.locals.TryGetValue(3, out LocalVariableInfo? loc) && loc.LocalType == wrapper.LocalType;
            }
            else
            { // wrapper Matches would have matched it if it matched.
                return false;
            }
        }
        return false;
    }
}