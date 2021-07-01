/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-silo-size/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */


/* PatcherHelper for SDV Patching using Harmony
 * ============================================
 * 
 * This file is a self-contained "helper bundle" to _assist_ modders in using the Harmony framework.
 * 
 * Why is it called a "helper bundle" not a "framework"? First, we must understand the difference
 * between the two.
 * 
 * A "framework" provides a complete, often opinionated, system to hide the underlying mechanism.
 * A "framework" is usually complex, and you need to follow its own well-defined procedure. If a
 * feature/facility does not exist, you usually have to either extend the framework yourself, or
 * submit an Issue report to the maker so they can implement the facility in a way that meshes
 * with the overall concept/design of the "framework".
 * 
 * A clear -- and relevant -- example is the Harmony framework itself. You _can_ perform your own patching,
 * but Harmony hides all that behind its framework, so you never have to interact with the .Net IL
 * interpreter itself.
 * 
 * A "helper bundle" is, on the other hand, just a collection of _optional_ tools to help you use the
 * underlying mechanism. A little veneer might exist to provide some 'convenience methods' in using the
 * underlying mechanism, but overall you're still nearly directly using the underlying mechanism.
 * 
 * For example, the InstructionsWalker class provided in this helper bundle. It does not hide how
 * Harmony works; rather, it just provides a convenient way to search an array of instructions, replacing
 * CIL ops easily using convenient methods, and so on.
 * 
 * Even the HarmonyPatcherHelper class does not hide the mechanism of Harmony; rather, it simplifies
 * all the boilerplate code such as the try..catch protection, instantiation of MethodInfo and HarmonyMethod
 * objects required by HarmonyInstance.Patch(), and so on.
 * 
 * A benefit of making this a "helper bundle" instead of a "framework", is that this strategy allows simple
 * inclusion in your project(s): Just copy the file over, include it in the Project, and add the statement
 * "using PatcherHelper;" on top of *your* patcher code ... and that's it. No need to install binaries,
 * or exotic NuGet packages, and definitely -- most importantly, IMO -- no chance whatsoever of experiencing
 * DLL hell due to different binary version being requested by different mods!
 * 
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

using Harmony;
using StardewModdingAPI;

namespace PatcherHelper
    {
    public static class Meta
        {
        public static string Revision = "R61";
        }

    /// <summary>
    /// <para>If you install the NuGet package "RequireNamedArgs", this class enables you to mark methods with the
    /// <c>[RequireNamedArgs]</c> attribute, resulting in VS forcing you to specify parameters using keywords.</para>
    /// <para>See <see href="https://github.com/mykolav/require-named-args-fs"/> for more info.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RequireNamedArgsAttribute : Attribute { }

    /// <summary>
    /// <para>Type of patch. Mainly to assist <c>HarmonyPatcherQueue.QueuePatchA</c>.</para>
    /// <para>See also the <seealso cref="PatchTypeAttribute.FromPatch(HarmonyMethod)"/> method.</para>
    /// </summary>
    public enum PatchType
        {
        Prefix,
        Postfix,
        Transpiler,
        UNKNOWN
        }

    /// <summary>
    /// <para>A custom attribute to specify the type of a patch. Mainly to assist <c>HarmonyPatcherQueue.QueuePatchA</c> methods</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PatchTypeAttribute : Attribute
        {
        public static readonly Dictionary<string, PatchType> TypeFromSuffix = new Dictionary<string, PatchType>() {
                {"prefix", PatchType.Prefix},
                {"pre", PatchType.Prefix},
                {"postfix", PatchType.Postfix },
                {"post", PatchType.Postfix },
                {"transpiler", PatchType.Transpiler },
                {"transp", PatchType.Transpiler },
            };

        public PatchType PType { get; private set; }

        public PatchTypeAttribute(PatchType ptype) {
            PType = ptype;
            }

        /// <summary>
        /// Given a HarmonyMethod, try to determine type of patch based on attribute,
        /// but if attribute not set, try to deduce based on method's prefix.
        /// </summary>
        /// <param name="patch">A HarmonyMethod to inspect</param>
        /// <returns>The type of the patch, or <c>PatchType.UNKNOWN</c> if function cannot deduce the type</returns>
        public static PatchType FromPatch(HarmonyMethod patch) {
            PatchTypeAttribute pt_attr = patch.method.GetCustomAttribute<PatchTypeAttribute>();
            if (pt_attr != null) return pt_attr.PType;
            string suffix = patch.method.Name.Split('_').Last();
            if (TypeFromSuffix.TryGetValue(suffix, out PatchType pt))
                return pt;
            else
                return PatchType.UNKNOWN;
            }
        }

    /// <summary>
    /// Extension methods for CodeInstruction.
    /// <para>These are mostly used as mutators</para>
    /// </summary>
    public static class CodeInstructionExtensions
        {
        /// <summary>
        /// Fluent-ly set label for instruction
        /// </summary>
        /// <param name="instruction">CodeInstruction to mutate</param>
        /// <param name="newLabels">New labels to apply</param>
        /// <returns>Mutated <c>instruction</c> to allow Fluent chaining</returns>
        public static CodeInstruction SetLabel(this CodeInstruction instruction, List<Label> newLabels) {
            if (!(newLabels is null)) instruction.labels = newLabels;
            return instruction;
            }
        public static string Repr(this CodeInstruction instruction) {
            OpCode opc = instruction.opcode;
            object opr = instruction.operand;
            Type oprt = opr?.GetType();
            string oprt_s = oprt is null ? "null" : oprt.ToString();
            return $"{opc} | ({oprt_s}) {opr}";
            }
        }

    /// <summary>
    /// Custom exception that can be raised by HarmonyPatcherHelper
    /// </summary>
    public class PatchingFailureException : Exception
        {
        public PatchingFailureException() : base() { }
        public PatchingFailureException(string message) : base(message) { }
        public PatchingFailureException(string message, Exception innerException) : base(message, innerException) { }
        }

    /// <summary>
    /// <para>This convenience class allows you to first gather a whole bunch of parameters for HarmonyPatcherHelper.TryPatching,
    /// and execute them in one go.</para>
    /// <para>This allows you to determine how many patches will take place before actually doing them.</para>
    /// </summary>
    public class HarmonyPatcherQueue
        {
        public readonly string UniqueID;
        public readonly IMonitor Monitor;
        public HarmonyPatcherHelper HarPH;

        /// <summary>
        /// <para>If true (default), then immediately abort queue processing upon patching failure.</para>
        /// <para>If false, then continue patching even if a patch failed, gathering the failures into an output dictionary.</para>
        /// </summary>
        public bool FailFast { get; set; } = true;

        public bool? ExceptionOnFail = null;
        public bool Silent = false;
        public int Stats_Success = 0;

        public class QueueEntry
            {
            public MethodBase original = null;
            public HarmonyMethod prefix = null;
            public HarmonyMethod postfix = null;
            public HarmonyMethod transpiler = null;
            public bool enabled = true;
            }

        public List<QueueEntry> PatchQueue = new List<QueueEntry>();

        public HarmonyPatcherQueue(string uniqueID, IMonitor monitor, bool failFast = true) : this(new HarmonyPatcherHelper(uniqueID, monitor), failFast) { }
        public HarmonyPatcherQueue(HarmonyPatcherHelper patcherHelper, bool failFast = true) {
            Monitor = patcherHelper.Monitor;
            HarPH = patcherHelper;
            FailFast = failFast;
            }

        /// <summary>
        /// <para>Release potentially expensive references.</para>
        /// <para><b>WARNING:</b> Once you invoke this method, the instance will no longer be usable!</para>
        /// </summary>
        public void Cleanup() {
            PatchQueue.Clear();
            PatchQueue = null;
            HarPH = null;
            }

        public int Length { get => PatchQueue.Count; }

        public int Count { get => PatchQueue.Count; }

        /// <summary>
        /// A count of how many queued patches are enabled
        /// </summary>
        public int Enableds { get => PatchQueue.Where(i => i.enabled).Count(); }
        public void Clear() {
            PatchQueue.Clear();
            }
        /// <summary>
        /// Perform patching
        /// </summary>
        /// <param name="failures">Outputs a dictionary of failed patches</param>
        /// <returns>The number of successful patches</returns>
        public int Execute(out Dictionary<string, Exception> failures) {
            failures = new Dictionary<string, Exception>();
            Stats_Success = 0;
            string culprit;
            foreach (var i in PatchQueue.Where(i => i.enabled)) {
                try {
                    if (HarPH.TryPatching(i.original, i.prefix, i.postfix, i.transpiler, ExceptionOnFail, Silent))
                        Stats_Success++;
                    }
                catch (Exception ex) {
                    if (FailFast) throw;
                    culprit = (i.prefix ?? i.postfix ?? i.transpiler).methodName;
                    failures[culprit] = ex;
                    }
                }
            return Stats_Success;
            }

        /// <summary>
        /// Perform patching
        /// </summary>
        /// <returns>The number of successful patches</returns>
        public int Execute() => Execute(out _);

        /// <summary>
        /// Queue exactly one patch
        /// </summary>
        /// <param name="original">The method to patch</param>
        /// <param name="prefix">Prefix-type patch</param>
        /// <param name="postfix">Postfix-type patch</param>
        /// <param name="transpiler">Transpiler-type patch</param>
        /// <param name="enabled">Whether to actually perform the patch when .Execute is invoked</param>
        /// <remarks><b>Note:</b> One (and only one!) of prefix, postfix, or transpiler must be provided.</remarks>
        public void QueuePatch(
            MethodBase original,
            HarmonyMethod prefix = null,
            HarmonyMethod postfix = null,
            HarmonyMethod transpiler = null,
            bool enabled = true
            ) {
            if (original is null)
                throw new ArgumentNullException("original");
            if (Util.CountNull(prefix, postfix, transpiler) == 3)
                throw new PatchingFailureException("At least one of prefix/postfix/transpiler must be specified!");
            PatchQueue.Add(new QueueEntry { original = original, prefix = prefix, postfix = postfix, transpiler = transpiler, enabled = enabled });
            }

        /// <summary>
        /// Queue patching against several methods using the same patcher
        /// </summary>
        /// <param name="originals">The original methods to patch in the same way</param>
        /// <param name="prefix">Prefix-type patch</param>
        /// <param name="postfix">Postfix-type patch</param>
        /// <param name="transpiler">Transpiler-type patch</param>
        /// <param name="enabled">Whether to actually perform the patch when .Execute is invoked</param>
        /// <remarks><b>Note:</b> One (and only one!) of prefix, postfix, or transpiler must be provided.</remarks>
        public void QueuePatch(
            IEnumerable<MethodBase> originals,
            HarmonyMethod prefix = null,
            HarmonyMethod postfix = null,
            HarmonyMethod transpiler = null,
            bool enabled = true
            ) {
            foreach (var mb in originals) QueuePatch(mb, prefix, postfix, transpiler, enabled);
            }

        /// <summary>
        /// Queue exactly one patch. MethodInfo will be retrieved using originalType and originalName
        /// </summary>
        /// <param name="originalType">The Type containing the method to patch</param>
        /// <param name="originalName">The name of the method to patch</param>
        /// <param name="prefix">Prefix-type patch</param>
        /// <param name="postfix">Postfix-type patch</param>
        /// <param name="transpiler">Transpiler-type patch</param>
        /// <param name="targetParams">(Optional) used to select between methods of the same name but different signatures</param>
        /// <param name="targetGenerics">(Optional) used to select between generic methods of the same signature but different Generic</param>
        /// <param name="enabled">Whether to actually perform the patch when .Execute is invoked</param>
        /// <remarks><b>Note:</b> One (and only one!) of prefix, postfix, or transpiler must be provided.</remarks>
        public void QueuePatch(
            Type originalType,
            string originalName,
            HarmonyMethod prefix = null,
            HarmonyMethod postfix = null,
            HarmonyMethod transpiler = null,
            Type[] targetParams = null,
            Type[] targetGenerics = null,
            bool enabled = true
            ) {
            MethodInfo original = AccessTools.Method(originalType, originalName, targetParams, targetGenerics);
            if (original is null) throw new PatchingFailureException("Unable to retrieve method to patch");
            QueuePatch(original, prefix, postfix, transpiler, enabled);
            }

        /// <summary>
        /// Queue exactly one patch, type of patch will be auto-deduced using <c>PatchType.FromPatch</c> static method.
        /// </summary>
        /// <param name="originalType">The Type containing the method to patch</param>
        /// <param name="originalName">The name of the method to patch</param>
        /// <param name="patch">The patch to queue</param>
        /// <param name="enabled">Whether to actually perform the patch when .Execute is invoked</param>
        public void QueuePatchA(
            Type originalType,
            string originalName,
            HarmonyMethod patch,
            bool enabled = true
            ) {
            HarmonyMethod prefix = null;
            HarmonyMethod postfix = null;
            HarmonyMethod transpiler = null;

            switch (PatchTypeAttribute.FromPatch(patch)) {
                case PatchType.Prefix:
                    prefix = patch;
                    break;
                case PatchType.Postfix:
                    postfix = patch;
                    break;
                case PatchType.Transpiler:
                    transpiler = patch;
                    break;
                default:
                    throw new PatchingFailureException($"Cannot determine patching method for {patch.method.Name}");
                }
            QueuePatch(originalType, originalName, prefix, postfix, transpiler, enabled: enabled);
            }

        /// <summary>
        /// Queue several patches using the same patch, type of patch will be auto-deduced using <c>PatchType.FromPatch</c> static method.
        /// </summary>
        /// <param name="originals">A collection of methods to patch</param>
        /// <param name="patch">The patch method</param>
        /// <param name="enabled">Whether to actually perform the patch when .Execute is invoked</param>
        public void QueuePatchA(
            IEnumerable<MethodBase> originals,
            HarmonyMethod patch,
            bool enabled = true
            ) {
            HarmonyMethod prefix = null;
            HarmonyMethod postfix = null;
            HarmonyMethod transpiler = null;

            switch (PatchTypeAttribute.FromPatch(patch)) {
                case PatchType.Prefix:
                    prefix = patch;
                    break;
                case PatchType.Postfix:
                    postfix = patch;
                    break;
                case PatchType.Transpiler:
                    transpiler = patch;
                    break;
                default:
                    throw new PatchingFailureException($"Cannot determine patching method for {patch.method.Name}");
                }
            QueuePatch(originals, prefix, postfix, transpiler, enabled: enabled);
            }
        }

    /// <summary>
    /// Provides some boilerplating to make patching less tedious
    /// </summary>
    public class HarmonyPatcherHelper
        {
        public static LogLevel BeginPatchingLogLevel = LogLevel.Trace;
        public static LogLevel SuccessLogLevel = LogLevel.Trace;

        public readonly IMonitor Monitor;
        public readonly HarmonyInstance harmony;
        public bool DefaultExceptionOnFail = true;
        public int Stats_Success = 0;
        public int Stats_Fail = 0;
        public Type PatcherClass = null;

        private HarmonyPatcherHelper() { }
        public HarmonyPatcherHelper(string uniqueID, IMonitor monitor) {
            Monitor = monitor;
            harmony = HarmonyInstance.Create(uniqueID);
            }
        public HarmonyPatcherHelper(Mod mod) : this(mod.ModManifest.UniqueID, mod.Monitor) { }

        /// <summary>
        /// This constructor 'caches' the patcherClass static class so that you can use the third override of <c>TryPatching()</c>,
        /// in which you don't have to build your <c>HarmonyMethod</c> object yourself, but simply refer to the patcher
        /// method you want to use.
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <param name="monitor"></param>
        /// <param name="patcherClass"></param>
        public HarmonyPatcherHelper(string uniqueID, IMonitor monitor, Type patcherClass) : this(uniqueID, monitor) {
            PatcherClass = patcherClass;
            }

        public bool TryPatching(
            MethodBase original,
            HarmonyMethod prefix = null,
            HarmonyMethod postfix = null,
            HarmonyMethod transpiler = null,
            bool? exception_on_fail = null,
            bool silent = false
            ) {
            if (original is null) {
                Monitor.Log("original cannot be null!", LogLevel.Error);
                throw new ArgumentNullException("original");
                }
            string poi = $"{original.DeclaringType.Namespace}.{original.DeclaringType.Name}.{original.Name}";
            try {
                string patch_type = prefix is null ? (postfix is null ? (transpiler is null ? "null!": "transpiler") : "postfix") : "prefix";
                if (!silent) Monitor.Log($"Patching {poi} [{patch_type}]...", BeginPatchingLogLevel);
                harmony.Patch(original: original, prefix: prefix, postfix: postfix, transpiler: transpiler);
                Stats_Success++;
                if (!silent) Monitor.Log($"Succesful patching {poi}", SuccessLogLevel);
                return true;
                }
            catch (Exception ex) {
                Stats_Fail++;
                string msg = $"Error patching {poi}:\n{ex}";
                Monitor.Log(msg, LogLevel.Error);
                if (exception_on_fail ?? DefaultExceptionOnFail) throw new PatchingFailureException(msg, ex);
                return false;
                }
            }

        public bool TryPatching(
            Type originalType,
            string originalName,
            HarmonyMethod prefix = null,
            HarmonyMethod postfix = null,
            HarmonyMethod transpiler = null,
            bool? exception_on_fail = null,
            bool silent = false,
            Type[] targetParams = null,
            Type[] targetGenerics = null
            ) {
            MethodInfo original = AccessTools.Method(originalType, originalName, targetParams, targetGenerics);
            if (original is null) throw new PatchingFailureException("Unable to retrieve method to patch");
            return TryPatching(original, prefix, postfix, transpiler, exception_on_fail, silent);
            }

        /// <summary>
        /// The 3rd override of TryPatching allows you to simply use the patcher method's name, and behind
        /// the scenes it will instantiate the require <c>HarmonyMethod</c> object for you.
        /// You are required to set the value of the <c>PatcherClass</c> property first, though.
        /// This can be set at instantiation time by using the 3rd constructor.
        /// </summary>
        /// <param name="originalType"></param>
        /// <param name="originalName"></param>
        /// <param name="prefix">Method name of prefix patcher</param>
        /// <param name="postfix">Method name of postfix patcher</param>
        /// <param name="transpiler">Method name of transpiler patcher</param>
        /// <param name="exception_on_fail"></param>
        /// <param name="silent"></param>
        /// <param name="targetParams"></param>
        /// <param name="targetGenerics"></param>
        /// <returns></returns>
        public bool TryPatching(
            Type originalType,
            string originalName,
            string prefix = null,
            string postfix = null,
            string transpiler = null,
            bool? exception_on_fail = null,
            bool silent = false,
            Type[] targetParams = null,
            Type[] targetGenerics = null
            ) {
            HarmonyMethod mprefix = !(prefix is null) ? new HarmonyMethod(PatcherClass, prefix) : null;
            HarmonyMethod mpostfix = !(postfix is null) ? new HarmonyMethod(PatcherClass, postfix) : null;
            HarmonyMethod mtransp = !(transpiler is null) ? new HarmonyMethod(PatcherClass, transpiler) : null;
            return TryPatching(originalType, originalName, mprefix, mpostfix, mtransp, exception_on_fail, silent, targetParams, targetGenerics);
            }
        }

    /// <summary>
    /// Custom exception that can be raised by HarmonyPatcherHelper
    /// </summary>
    public class AssertionFailure : Exception
        {
        public AssertionFailure() : base() { }
        public AssertionFailure(string message) : base(message) { }
        public AssertionFailure(string message, Exception innerException) : base(message, innerException) { }
        }

    /// <summary>
    /// Used by some methods of InstructionsWalker to determine what op attribute(s) to copy.
    /// <para>This is a <b><c>Flags</c></b> enum, so to combine you can use the <c>|</c> (binary OR) operator</para>
    /// </summary>
    [Flags]
    public enum OpCopy
        {
        Labels = 1,
        Blocks = 2
        }

    /// <summary>
    /// <para>'Walks' over an array of <c>CodeInstruction[]</c> by maintaining an internal pointer.
    /// Speeds up searching for opcode patterns by not having to search from the start again,
    /// and simplifies coding by not having to maintain a pointer yourself.
    /// Use this in conjunction with the <b><c>WantOp</c></b> class.</para>
    /// </summary>
    public class InstructionsWalker
        {
        public readonly CodeInstruction[] Instructions;
        public int CurrentLocation { get; private set; } = 0;
        public readonly Dictionary<string, int> Bookmarks = new Dictionary<string, int>();

        public InstructionsWalker(IEnumerable<CodeInstruction> instructions) {
            Instructions = instructions.ToArray();
            }

        public CodeInstruction CurrentOp(WantOp assert_op = null) {
            CodeInstruction op = Instructions[CurrentLocation];
            if (!(assert_op is null))
                if (assert_op.NotMatches(op))
                    throw new AssertionFailure($"CurrentOp <{op}> does not match assertion {assert_op}");
            return op;
            }

        /// <summary>
        /// Fetch the current location, but do NOT advance
        /// </summary>
        /// <param name="fetchedCurLoc">Variable to hold the fetched CurrentLocation</param>
        /// <param name="offset">Add this to the fetched location</param>
        /// <param name="assert_op">If given, asserts that instruction at location+offset matches a <c>WantOp</c> criteria</param>
        /// <returns><c>this</c> for Fluent chaining.</returns>
        public InstructionsWalker FetchLocation(out int fetchedCurLoc, int offset = 0, WantOp assert_op = null) {
            fetchedCurLoc = CurrentLocation + offset;
            if (!(assert_op is null)) {
                CodeInstruction op = Instructions[fetchedCurLoc];
                if (assert_op.NotMatches(Instructions[fetchedCurLoc]))
                    throw new AssertionFailure($"Op at {fetchedCurLoc} is <{op}> does not match assertion {assert_op}");
                }
            return this;
            }

        /// <summary>
        /// Search forward in Instructions array for an op that matches <c><paramref name="to_match"/></c>
        /// <para>Does <b>NOT</b> advance instruction pointer.</para>
        /// </summary>
        /// <param name="where">Variable to hold the match position. If not found, will be set to <c>-1</c></param>
        /// <param name="to_match">WantOp to search for</param>
        /// <param name="relative_pos">If <c>true</c> (default), outputs the offset compared to CurrentLocation.
        /// <para>If <c>false</c>, outputs the absolute position within the Instructions array.</para></param>
        /// <returns><c>this</c> for Fluent chaining</returns>
        public InstructionsWalker FindNextMatch(out int where, WantOp to_match, bool relative_pos = true, int? assert_where = null) {
            int? findloc = null;
            for (int i = CurrentLocation; i < Instructions.Length; i++) {
                if (to_match.Matches(Instructions[i])) {
                    findloc = i;
                    break;
                    }
                }
            if (findloc is null)
                where = -1;
            else
                where = relative_pos ? findloc.Value - CurrentLocation : findloc.Value;
            if (!(assert_where is null))
                if (where != assert_where)
                    throw new AssertionFailure($"{to_match} found at {where} does not match assertion {assert_where}");
            return this;
            }

        /// <summary>
        /// Save the current location as bookmark. Will NOT advance pointer.
        /// </summary>
        /// <param name="bookmarkName">Bookmark's name</param>
        /// <returns><c>this</c> for Fluent chaining</returns>
        public InstructionsWalker Mark(string bookmarkName) {
            Bookmarks[bookmarkName] = CurrentLocation;
            return this;
            }

        /// <summary>
        /// Fetch the instruction currently pointed at, and advance.
        /// </summary>
        /// <param name="fetchedOp">A <c>CodeInstruction</c> variable to store the fetched instruction</param>
        /// <param name="assert_op">Assert that instruction matches a <c>WantOp</c> criteria</param>
        /// <param name="move_pos">If <c>false</c>, don't advance pointer after fetching</param>
        /// <returns><c>this</c> for Fluent chaining</returns>
        public InstructionsWalker GoFetchOp(out CodeInstruction fetchedOp, WantOp assert_op = null, bool move_pos = true) {
            fetchedOp = CurrentOp(assert_op);
            if (move_pos) CurrentLocation++;
            return this;
            }

        /// <summary>
        /// Advance pointer forward by N. If N is negative, move backwards instead.
        /// </summary>
        /// <param name="N">How many instructions to advance. Can be negative.</param>
        /// <param name="assert_op">If given, then will assert that instruction at destination matches provided WantOp</param>
        /// <returns><c>this</c> to allow Fluent chaining.</returns>
        public InstructionsWalker GoForward(int N, WantOp assert_op = null) {
            int newpos = CurrentLocation + N;
            if (!(assert_op is null)) {
                CodeInstruction op = Instructions[newpos];
                if (assert_op.NotMatches(Instructions[newpos]))
                    throw new AssertionFailure($"Op at {newpos} is <{op}> does not match assertion {assert_op}");
                }
            CurrentLocation = newpos;
            return this;
            }

        /// <summary>
        /// Sets the pointer to a certain position
        /// </summary>
        /// <param name="pos">New position</param>
        /// <param name="assert_op">If given, then will assert that instruction at location 'pos' matches provided WantOp</param>
        /// <returns><c>this</c> to allow Fluent chaining.</returns>
        /// <remarks>Some code (especially within switch..case structure) can get shuffled around by the compiler during optimization.
        /// <para>This method allows returning to a certain point in the instruction chain,
        /// e.g., to the start of a switch structure.</para></remarks>
        public InstructionsWalker GoTo(int pos, WantOp assert_op = null) {
            if (!(assert_op is null)) {
                CodeInstruction op = Instructions[pos];
                if (assert_op.NotMatches(Instructions[pos]))
                    throw new AssertionFailure($"Op at {pos} is <{op}> does not match assertion {assert_op}");
                }
            CurrentLocation = pos;
            return this;
            }

        /// <summary>
        /// <para>Move pointer to a specified bookmark (created beforehand using <c>Mark()</c>).</para>
        /// </summary>
        /// <param name="bookmark">Bookmark name to move to</param>
        /// <returns><c>this</c> for Fluent chaining</returns>
        public InstructionsWalker GoTo(string bookmark, WantOp assert_op = null) => GoTo(Bookmarks[bookmark], assert_op);

        /// <summary>
        /// Restart walker from the very beginning (i.e., set pointer to 0)
        /// </summary>
        /// <returns><c>this</c> to allow Fluent chaining.</returns>
        public InstructionsWalker Restart() {
            return GoTo(0);
            }

        /// <summary>
        /// Given an array of WantOp, this method tries to find where that array is inside the bigger array of CodeInstruction
        /// </summary>
        /// <param name="wantOps">Array of WantOp's to search for</param>
        /// <param name="pos_after">If false (default), CurrentLocation will point to first op that matches the array.
        /// <para>If true (explicit), CurrentLocation will point to the first op after the found array.</para></param>
        /// <param name="exception_on_notfound">Raise exception instead of returning null if not found.</param>
        /// <returns><c>null</c> if <c>wantOps</c> is not found (and <c>exception_on_notfound = false</c>).
        /// <para>If <c>wantOps</c> is found, it will return <c>this</c> to allow Fluent chaining of commands.</para>
        /// <para>Finding position can be retrieved using <c>CurrentLocation</c></para></returns>
        public InstructionsWalker GoFind(WantOp[] wantOps, bool pos_after = false, bool exception_on_notfound = true) {
            int maxLoc = Instructions.Length - wantOps.Length;
            int? foundAt = null;
            for (int i = CurrentLocation; i <= maxLoc; ++i) {
                foundAt = i;
                for (int j = 0; j < wantOps.Length; ++j) {
                    if (wantOps[j].NotMatches(Instructions[i + j])) {
                        foundAt = null;
                        break;
                        }
                    }
                if (foundAt.HasValue)
                    break;
                }
            if (foundAt is null) {
                if (exception_on_notfound)
                    throw new EntryPointNotFoundException("wantOps not found!");
                else
                    return null;
                }
            CurrentLocation = foundAt.Value;
            if (pos_after) CurrentLocation += wantOps.Length;
            return this;
            }

        /// <summary>
        /// Given a single WantOp, this method tries to move the pointer to the first occurrence of that WantOp on or after the current location
        /// </summary>
        /// <param name="wantop">A WantOp to search for</param>
        /// <param name="pos_after">If false (default), CurrentLocation will point to first op that matches the array.
        /// <para>If true (explicit), CurrentLocation will point to the first op after the found array.</para></param>
        /// <param name="exception_on_notfound">Raise exception instead of returning null if not found.</param>
        /// <returns><c>null</c> if <c>wantOps</c> is not found (and <c>exception_on_notfound = false</c>).
        /// <para>If <c>wantOps</c> is found, it will return <c>this</c> to allow Fluent chaining of commands.</para>
        /// <para>Finding position can be retrieved using <c>CurrentLocation</c></para></returns>
        public InstructionsWalker GoFind(WantOp wantop, bool pos_after = false, bool exception_on_notfound = true) =>
            GoFind(new WantOp[] { wantop }, pos_after, exception_on_notfound);

        /// <summary>
        /// Given two arrays of WantOp[], this method tries to find where the positions of the 1st array and the second array
        /// </summary>
        /// <param name="label"></param>
        /// <param name="beginOps"></param>
        /// <param name="endOps"></param>
        /// <param name="beginOffset"></param>
        /// <param name="endOffset"></param>
        /// <returns>A Tuple(int, int), Item1 is the startpos of 1st array, Item2 is the startpos of 2nd array</returns>
        public Tuple<int, int> GoFindBeginEnd(
          string label,
          WantOp[] beginOps,
          WantOp[] endOps
            ) {
            _ = GoFind(beginOps, exception_on_notfound: false) ?? throw new EntryPointNotFoundException($"Cannot find begin point of '{label}'");
            int beginpos = CurrentLocation;
            CurrentLocation += beginOps.Length;
            _ = GoFind(endOps, exception_on_notfound: false) ?? throw new EntryPointNotFoundException($"Cannot find ending point of '{label}'");
            int endpos = CurrentLocation;
            return new Tuple<int, int>(beginpos, endpos);
            }

        /// <summary>
        /// Replace a range of CodeInstruction's to NOP
        /// </summary>
        /// <param name="absolute_begin_pos">Where to begin NOPification. If null, start from CurrentLocation</param>
        /// <param name="relative_begin_pos">Add this to the starting point of NOPification</param>
        /// <param name="absolute_end_pos">Where to stop NOPification. Instruction at this position <b>will NOT</b> be NOPified. <para><b>See Note on method description.</b></para></param>
        /// <param name="relative_end_pos">Where to stop NOPification, relative to CurrentLocation. Identical to 'length' if begin location is CurrentLocation. <b>See Note on method description.</b></param>
        /// <param name="length">How many instructions to NOPify. <b>See Note on method description.</b></param>
        /// <param name="move_pos">If true (explicit), then move CurrentPos to the instruction following the sequence of NOPs</param>
        /// <param name="copy">Combination of OpCopy flags to specify what metaop to copy</param>
        /// <returns><c>this</c> for Fluent chaining</returns>
        /// <exception cref="ArgumentException">Thrown if none or more than one of {<c>absolute_end_pos</c> or <c>relative_end_pos</c> or <c>length</c>} are specified</exception>
        /// <remarks><b>NOTE:</b> ONE and ONLY ONE of <c>absolute_end_pos</c> or <c>relative_end_pos</c> or <c>length</c> must be specified!</remarks>
        public InstructionsWalker NOPify(
            int? absolute_begin_pos = null,
            int relative_begin_pos = 0,
            int? absolute_end_pos = null,
            int? relative_end_pos = null,
            int? length = null,
            bool move_pos = true,
            OpCopy copy = 0
            ) {
            bool copy_labels = copy.HasFlag(OpCopy.Labels);
            if (Util.CountNull(absolute_end_pos, relative_end_pos, length) != 2)
                throw new ArgumentException("ONE AND ONLY ONE OF absolute_end_pos or relative_end_pos or length must be given!");
            int begin_pos = absolute_begin_pos ?? CurrentLocation;
            begin_pos += relative_begin_pos;
            int end_pos = begin_pos;
            if (!(length is null))
                end_pos += length.Value;
            else if (!(relative_end_pos is null))
                end_pos += CurrentLocation + relative_end_pos.Value;
            else if (!(absolute_end_pos is null))
                end_pos = absolute_end_pos.Value;
            if (end_pos <= begin_pos) throw new IndexOutOfRangeException("end_pos <= begin_pos");
            List<Label> orig_labels = null;
            for (int i = begin_pos; i < end_pos; i++) {
                if (copy_labels) orig_labels = Instructions[i].labels;
                Instructions[i] = new CodeInstruction(OpCodes.Nop);
                if (copy_labels) Instructions[i].labels = orig_labels;
                }
            if (move_pos) CurrentLocation = end_pos;
            return this;
            }

        /// <summary>
        /// Replace instructions from CurrentLocation with NOPs until we find a certain WantOp (which itself will not be replaced)
        /// </summary>
        /// <param name="until">Stop NOPifying when this WantOp is encountered (itself won't be replaced)</param>
        /// <param name="move_pos">If true (explicit), then move CurrentPos to the instruction following the sequence of NOPs</param>
        /// <param name="copy">Combination of OpCopy flags to specify what metaop to copy</param>
        /// <param name="assert_count">Assert how many replacements were made</param>
        /// <param name="assert_next">Assert the next op following the NOPified block</param>
        /// <returns><c>this</c> for Fluent chaining</returns>
        /// <exception cref="AssertionFailure">Thrown if any of the assertions (if provided) fails</exception>
        public InstructionsWalker NOPify(WantOp until, bool move_pos = true, OpCopy copy = 0, int? assert_count = null, WantOp assert_next = null) {
            FindNextMatch(out int stop_pos, relative_pos: false, to_match: until);
            if (stop_pos < 0) {
                throw new PatchingFailureException($"Cannot find {until} in rest of Instructions");
                }
            bool copy_labels = copy.HasFlag(OpCopy.Labels);
            CodeInstruction orig_op, new_op;
            int count = 0;
            for (int loc = CurrentLocation; loc < stop_pos; loc++) {
                orig_op = Instructions[loc];
                new_op = new CodeInstruction(OpCodes.Nop);
                if (copy_labels) new_op.labels = orig_op.labels;
                Instructions[loc] = new_op;
                count++;
                }
            if (assert_count.HasValue && count != assert_count.Value)
                throw new AssertionFailure($"Performed {count} NOPing does not match assertion {assert_count}");
            if (!(assert_next is null) && assert_next.NotMatches(Instructions[stop_pos]))
                throw new AssertionFailure($"Next op after NOPified <{Instructions[stop_pos]}> does not match assertion {assert_next}");
            if (move_pos)
                CurrentLocation = stop_pos;
            return this;
            }

        /// <summary>
        /// Replace CodeInstruction at a position with a new CodeInstruction.
        /// <para>Position is calculated as absolute_pos + relative_pos (see param description on null handling)</para>
        /// <para><b>WARNING:</b> Pointer will advance to 1 instruction after the replaced instruction! (Unless <c>move_pos = false</c>)</para>
        /// </summary>
        /// <param name="absolute_pos">Location to replace. If null, use CurrentLocation</param>
        /// <param name="relative_pos">Add this to wanted location</param>
        /// <param name="with">CodeInstruction that will replace</param>
        /// <param name="mutator">(Optional) mutate CodeInstruction before replacing</param>
        /// <param name="assert_previous">If given, assert that the to-be-replaced op is the given WantOp</param>
        /// <param name="copy">Combination of OpCopy flags to specify what metaop to copy</param>
        /// <param name="move_pos">If false, do not advance pointer after replacement.</param>
        public InstructionsWalker ReplaceAt(
            int? absolute_pos = null,
            int relative_pos = 0,
            CodeInstruction with = null,
            Func<CodeInstruction, CodeInstruction> mutator = null,
            OpCopy copy = 0,
            bool move_pos = true,
            WantOp assert_previous = null
            ) {
            if (with is null)
                throw new ArgumentNullException(nameof(with));
            int pos = (absolute_pos ?? CurrentLocation) + relative_pos;
            CodeInstruction orig_op = Instructions[pos];
            if (assert_previous?.NotMatches(orig_op) ?? false)
                throw new AssertionFailure($"Previous op is <{orig_op}> does not match assertion {assert_previous}");
            CodeInstruction replacer_op = (mutator is null) ? with : mutator(with);
            if (copy.HasFlag(OpCopy.Labels)) replacer_op.labels = orig_op.labels;
            if (copy.HasFlag(OpCopy.Blocks)) replacer_op.blocks = orig_op.blocks;
            Instructions[pos] = replacer_op;
            if (move_pos) CurrentLocation = pos + 1;
            return this;
            }

        /// <summary>
        /// Set the <c>labels</c> property of the op at pointer location
        /// </summary>
        /// <param name="newLabels">Labels to apply</param>
        /// <param name="move_pos">If true (default), advances pointer after setting label</param>
        /// <param name="assert_op">If given, assert that the to-be-mutated op is the given WantOp</param>
        /// <returns></returns>
        public InstructionsWalker SetLabels(List<Label> newLabels, bool move_pos = true, WantOp assert_op = null) {
            CodeInstruction op = Instructions[CurrentLocation];
            if (!(assert_op is null))
                if (assert_op.NotMatches(op))
                    throw new AssertionFailure($"Op <{op.Repr()}> does not match assertion {assert_op}");
            Instructions[CurrentLocation].labels = newLabels;
            if (move_pos) CurrentLocation++;
            return this;
            }


        /// <summary>
        /// Fluently dumps several instructions into their Repr() representation.
        /// </summary>
        /// <param name="result">A <c>List&lt;string&gt;</c> variable to contain the dump result</param>
        /// <param name="N">How many instructions to Repr</param>
        /// <param name="move_pos">If true, advance pointer by <c>N</c> after dumping</param>
        /// <returns><c>this</c> for Fluent chaining</returns>
        public InstructionsWalker GoDump(out List<string> result, int N, bool move_pos = true) {
            result = new List<string>(
                Instructions
                    .Skip(CurrentLocation)
                    .Take(N)
                    .Select(i => i.Repr())
                );
            if (move_pos) CurrentLocation += N;
            return this;
            }
        }

    internal static class Util
        {
        internal static int CountNull(params object[] args) => args.Where(i => i is null).Count();
        }

    /// <summary>
    /// This is a convenience class that can be compared directly to CodeInstruction when transpiling.
    /// </summary>
    public class WantOp
        {
        public static IMonitor Monitor;

        public readonly OpCode? OriginalOpcode;
        public readonly HashSet<OpCode> OpcodeSet = new HashSet<OpCode>();
        public readonly object OperandValue;
        public Type OperandType { get; private set; } = null;

        private static readonly Dictionary<OpCode, Type> OpcodeToOperandType = new Dictionary<OpCode, Type> {
            {OpCodes.Ldfld,    typeof(FieldInfo) },
            {OpCodes.Ldflda,   typeof(FieldInfo) },
            {OpCodes.Call,     typeof(MethodInfo) },
            {OpCodes.Callvirt, typeof(MethodInfo) },
            {OpCodes.Ldstr,    typeof(string) },
            {OpCodes.Ldsfld,   typeof(FieldInfo) },
            {OpCodes.Ldc_I4,   typeof(int) },
            {OpCodes.Ldc_I4_S, typeof(sbyte) },
            {OpCodes.Ldc_R4,   typeof(float) },
            {OpCodes.Newobj,   typeof(ConstructorInfo) },
            {OpCodes.Ldarg_S,  typeof(byte) },
            {OpCodes.Stfld,    typeof(FieldInfo) },
            {OpCodes.Stsfld,   typeof(FieldInfo) },
            };

        /// <summary>
        /// Some equivalent opcodes.
        /// <para><b>dotPeek</b> sometomes uses the ".s" variants of the opcode, but the IL decompiler in Harmony will produce the non-".s" variant.
        /// Likely because dotPeek sees the <em>optimized</em> bytecode, while Harmony tries to make it 'portable'.</para>
        /// </summary>
        private static readonly List<HashSet<OpCode>> Equivalents = new List<HashSet<OpCode>> {
            new HashSet<OpCode>() {OpCodes.Ble, OpCodes.Ble_S},
            new HashSet<OpCode>() {OpCodes.Br, OpCodes.Br_S},
            new HashSet<OpCode>() {OpCodes.Brtrue, OpCodes.Brtrue_S},
            new HashSet<OpCode>() {OpCodes.Brfalse, OpCodes.Brfalse_S},
            new HashSet<OpCode>() {OpCodes.Stloc, OpCodes.Brfalse_S}
            };

        private protected WantOp() {
            }

        /// <summary>
        /// Instantiates a WantOp object. It will try to automagically determine some important stuff for matching purposes.
        /// </summary>
        /// <param name="opcode">If null (explicit), then will match any OpCode.</param>
        /// <param name="value">If null (default), means ignore the operand</param>
        /// <param name="use_equivalents">If false, then don't try to find equivalent opcodes</param>
        /// <param name="explicit_type">If null (default), then auto-deduce OperandType. Will be ignored if value is null.</param>
        public WantOp(OpCode? opcode, object value = null, bool use_equivalents = true, Type explicit_type = null) {
            OriginalOpcode = opcode;
            if (use_equivalents) {
                foreach (HashSet<OpCode> s in Equivalents) {
                    if (s.Contains(opcode.Value)) {
                        OpcodeSet = s;
                        break;
                        }
                    }
                }
            if (OpcodeSet.Count == 0)
                OpcodeSet.Add(opcode.Value);
            OperandValue = value;
            if (!(value is null))
                OperandType = explicit_type ?? OperandTypeForOpcode(opcode.Value);
            }

        private static readonly HashSet<Type> NativeTypes = new HashSet<Type> { typeof(sbyte), typeof(int), typeof(string) };


        /// <summary>
        /// Try to make a suitable WantOp from a given CodeInstruction
        /// </summary>
        /// <param name="op">CodeInstruction to 'genericize' as a WantOp</param>
        /// <param name="ignore_operand">Ignore operand, so the resulting WantOp matches solely on opcode</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0045:Convert to conditional expression", Justification = "Maintain ladder structure for readability")]
        public WantOp(CodeInstruction op, bool ignore_operand = false) : this(op.opcode, null) {
            if (ignore_operand) return;
            // If we can't auto-deduce the operand's type, then we'll ignore the operand
            if (!OpcodeToOperandType.TryGetValue(op.opcode, out Type oprt)) return;
            OperandType = oprt;
            if (NativeTypes.Contains(oprt))
                OperandValue = op.operand;
            else if (oprt == typeof(FieldInfo))
                OperandValue = (op.operand as FieldInfo).Name;
            else if (oprt == typeof(MethodInfo))
                OperandValue = (op.operand as MethodInfo).Name;
            else // if (oprt == typeof(ConstructorInfo))
                OperandValue = (op.operand as ConstructorInfo).DeclaringType.ToString();
            }

        public override string ToString() =>
            $"<WantOp>({OriginalOpcode?.ToString() ?? "null"}, ({OperandType?.Name ?? "null"}){OperandValue?.ToString() ?? "null"})";

        public Type OperandTypeForOpcode(OpCode opc) {
            if (!OpcodeToOperandType.ContainsKey(opc))
                throw new NotImplementedException($"Please add expected OperandType for '{opc}' to TypeFromOpcode!");
            return OpcodeToOperandType[opc];
            }

        /// <summary>
        /// Compare against a CodeInstruction object and determine match based on some hierarchical logic
        /// </summary>
        public bool Matches(CodeInstruction inst) {
            if (OriginalOpcode is null)
                return true;
            if (!OpcodeSet.Contains(inst.opcode))
                return false;
            if (OperandValue is null)
                return true;
            // Using .IsValueType + .ChangeType does NOT work, I don't know why.
            // So please do not try to 'optimize' this because I've been there, done that, got the exception
            //if (OperandType.IsValueType)
            //    return Convert.ChangeType(inst.operand, OperandType) == Convert.ChangeType(OperandValue, OperandType);
            if (OperandType == typeof(sbyte))
                return (sbyte)inst.operand == (sbyte)OperandValue;
            if (OperandType == typeof(int))
                return (int)inst.operand == (int)OperandValue;
            if (OperandType == typeof(float))
                return (float)inst.operand == (float)OperandValue;
            string operandStr = (string)OperandValue;
            if (OperandType == typeof(string))
                return (inst.operand as string) == operandStr;
            if (OperandType == typeof(FieldInfo))
                return (inst.operand as FieldInfo)?.Name == operandStr;
            if (OperandType == typeof(MethodInfo))
                return (inst.operand as MethodInfo)?.Name == operandStr;
            if (OperandType == typeof(ConstructorInfo)) {
                var cinfo = inst.operand as ConstructorInfo;
                return cinfo.DeclaringType.ToString() == operandStr || cinfo.DeclaringType.Name == operandStr;
                }
            return false;
            }

        /// <summary>
        /// Negation of Matches(). Just to make things easier to read
        /// </summary>
        public bool NotMatches(CodeInstruction inst) => !Matches(inst);

        #region convenience static methods for common OpCodes
        internal static WantOp Call(string target) => new WantOp(OpCodes.Call, target);
        internal static WantOp CallVirt(string target) => new WantOp(OpCodes.Callvirt, target);
        internal static WantOp Ldfld(string fieldname) => new WantOp(OpCodes.Ldfld, fieldname);
        internal static WantOp Ldsfld(string fieldname) => new WantOp(OpCodes.Ldsfld, fieldname);
        internal static WantOp Ldc_I4(int value) => new WantOp(OpCodes.Ldc_I4, value);
        internal static WantOp Ldc_I4_S(sbyte value) => new WantOp(OpCodes.Ldc_I4_S, value);
        internal static WantOp Ldc_R4(float value) => new WantOp(OpCodes.Ldc_R4, value);
        internal static WantOp Ldstr(string value) => new WantOp(OpCodes.Ldstr, value);
        internal static WantOp Stfld(string fieldname) => new WantOp(OpCodes.Stfld, fieldname);
        #endregion
        }

    /// <summary>
    /// Instantiate this class, and use the properties as 'cached' instantiations of WantOp.
    /// <para>This should speed up your transpiling, and also saves a lot of memory.</para>
    /// <para>When you're done with all the patching, you can simply remove all refs to the instantiated object, and let GC cleanup.</para>
    /// </summary>
    internal class WantOpCache
        {
        internal WantOp

            // null operand
            box = new WantOp(OpCodes.Box),
            ble_s = new WantOp(OpCodes.Ble_S),
            br = new WantOp(OpCodes.Br),
            br_s = new WantOp(OpCodes.Br_S),
            brfalse_s = new WantOp(OpCodes.Brfalse_S),
            brtrue_s = new WantOp(OpCodes.Brtrue_S),
            clt = new WantOp(OpCodes.Clt),
            conv_i2 = new WantOp(OpCodes.Conv_I2),
            conv_i4 = new WantOp(OpCodes.Conv_I4),
            conv_i8 = new WantOp(OpCodes.Conv_I8),
            conv_r4 = new WantOp(OpCodes.Conv_R4),
            conv_r_un = new WantOp(OpCodes.Conv_R_Un),
            div_un = new WantOp(OpCodes.Div_Un),
            isinst = new WantOp(OpCodes.Isinst),
            ldarg_0 = new WantOp(OpCodes.Ldarg_0),
            ldarg_1 = new WantOp(OpCodes.Ldarg_1),
            ldarg_2 = new WantOp(OpCodes.Ldarg_2),
            ldarg_3 = new WantOp(OpCodes.Ldarg_3),
            ldarg_s = new WantOp(OpCodes.Ldarg_S),
            ldarga_s = new WantOp(OpCodes.Ldarga_S),
            ldc_i4_0 = new WantOp(OpCodes.Ldc_I4_0),
            ldc_i4_1 = new WantOp(OpCodes.Ldc_I4_1),
            ldc_i4_2 = new WantOp(OpCodes.Ldc_I4_2),
            ldc_i4_3 = new WantOp(OpCodes.Ldc_I4_3),
            ldc_i4_4 = new WantOp(OpCodes.Ldc_I4_4),
            ldc_i4_5 = new WantOp(OpCodes.Ldc_I4_5),
            ldc_i4_7 = new WantOp(OpCodes.Ldc_I4_7),
            ldc_i4_s = new WantOp(OpCodes.Ldc_I4_S),
            ldc_r4 = new WantOp(OpCodes.Ldc_R4),
            ldloc_0 = new WantOp(OpCodes.Ldloc_0),
            ldloc_1 = new WantOp(OpCodes.Ldloc_1),
            ldloc_2 = new WantOp(OpCodes.Ldloc_2),
            ldloc_3 = new WantOp(OpCodes.Ldloc_3),
            ldloc_s = new WantOp(OpCodes.Ldloc_S),
            ldloca_s = new WantOp(OpCodes.Ldloca_S),
            ldelem_ref = new WantOp(OpCodes.Ldelem_Ref),
            ldfld = new WantOp(OpCodes.Ldfld),
            ldflda = new WantOp(OpCodes.Ldflda),
            ldftn = new WantOp(OpCodes.Ldftn),
            ldlen = new WantOp(OpCodes.Ldlen),
            ldnull = new WantOp(OpCodes.Ldnull),
            ldsfld = new WantOp(OpCodes.Ldsfld),
            stelem_i2 = new WantOp(OpCodes.Stelem_I2),
            stelem_ref = new WantOp(OpCodes.Stelem_Ref),
            stfld = new WantOp(OpCodes.Stfld),
            stsfld = new WantOp(OpCodes.Stsfld),
            stloc_0 = new WantOp(OpCodes.Stloc_0),
            stloc_1 = new WantOp(OpCodes.Stloc_1),
            stloc_2 = new WantOp(OpCodes.Stloc_2),
            stloc_3 = new WantOp(OpCodes.Stloc_3),
            stloc_s = new WantOp(OpCodes.Stloc_S),
            @switch = new WantOp(OpCodes.Switch),
            add = new WantOp(OpCodes.Add),
            div = new WantOp(OpCodes.Div),
            dup = new WantOp(OpCodes.Dup),
            mul = new WantOp(OpCodes.Mul),
            nop = new WantOp(OpCodes.Nop),
            pop = new WantOp(OpCodes.Pop),
            rem = new WantOp(OpCodes.Rem),
            ret = new WantOp(OpCodes.Ret),
            xor = new WantOp(OpCodes.Xor),
            newarr = new WantOp(OpCodes.Newarr),
            newobj = new WantOp(OpCodes.Newobj),
            call = new WantOp(OpCodes.Call),
            callvirt = new WantOp(OpCodes.Callvirt),

            // Non-null operand
            call_AppendEx = WantOp.Call("AppendEx"),
            call_getminelevel = WantOp.Call("get_mineLevel"),
            call_getstats = WantOp.Call("get_stats"),
            call_op_implicit = WantOp.Call("op_Implicit"),
            call_ToInt32 = WantOp.Call("ToInt32"),
            call_ToString = WantOp.Call("ToString"),
            callv_Append = WantOp.CallVirt("Append"),
            callv_Clear = WantOp.CallVirt("Clear"),
            callv_getCount = WantOp.CallVirt("get_Count"),
            callv_getDaysPlayed = WantOp.CallVirt("get_DaysPlayed"),
            callv_getX = WantOp.CallVirt("get_X"),
            callv_getY = WantOp.CallVirt("get_Y"),
            callv_Next = WantOp.CallVirt("Next"),
            callv_NextDouble = WantOp.CallVirt("NextDouble"),
            fld_uidThisGame = WantOp.Ldsfld("uniqueIDForThisGame"),
            fld_myID = WantOp.Ldfld("myID"),
            fld__temp = WantOp.Ldfld("_temp"),
            fld_timeOfDay = WantOp.Ldsfld("timeOfDay"),
            fld_tileLocation = WantOp.Ldfld("tileLocation"),
            fld_tileX = WantOp.Ldfld("tileX"),
            fld_tileY = WantOp.Ldfld("tileY"),
            fld_X = WantOp.Ldfld("X"),
            fld_Y = WantOp.Ldfld("Y"),
            new_random = new WantOp(OpCodes.Newobj, "System.Random")
            ;
        }

    }
