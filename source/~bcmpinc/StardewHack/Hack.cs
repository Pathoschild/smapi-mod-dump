using Harmony;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace StardewHack
{
    /// <summary>
    /// Common Hack code 
    /// </summary>
    public abstract class HackBase : Mod
    {
        /// <summary>
        /// Provides simplified API's for writing mods. 
        /// </summary>
        public IModHelper helper { get; private set; }

        /// <summary>
        /// The harmony instance used for patching. 
        /// </summary>
        public HarmonyInstance harmony { get; private set; }

        /// <summary>
        /// The method being patched. 
        /// Use only within methods annotated with BytecodePatch. 
        /// </summary>
        public MethodBase original { get; internal set; }

        /// <summary>
        /// The code that is being patched. 
        /// Use only within methods annotated with BytecodePatch. 
        /// </summary>
        public List<CodeInstruction> instructions { get; internal set; }

        /// <summary>
        /// The generator used for patching. 
        /// Use only within methods annotated with BytecodePatch. 
        /// </summary>
        public ILGenerator generator { get; internal set; }

        public override void Entry(IModHelper helper) {
            this.helper = helper;

            // Use the Mod's UniqueID to create the harmony instance.
            string UniqueID = helper.ModRegistry.ModID;
            Monitor.Log($"Applying bytecode patches for {UniqueID}.", LogLevel.Debug);
            harmony = HarmonyInstance.Create(UniqueID);
        }

        /// <summary>
        /// Find the first occurance of the given sequence of instructions that follows this range.
        /// The performed matching depends on the type:
        ///  - String: is it contained in the string representation of the instruction
        ///  - MemberReference (including MethodDefinition): is the instruction's operand equal to this reference.
        ///  - OpCode: is this the instruction's OpCode.
        ///  - CodeInstruction: are the instruction's OpCode and Operand equal.
        ///  - null: always matches.
        /// </summary>
        public InstructionRange FindCode(params Object[] contains) {
            return new InstructionRange(instructions, contains);
        }

        /// <summary>
        /// Find the last occurance of the given sequence of instructions that follows this range.
        /// See FindCode() for how the matching is performed.
        /// </summary>
        public InstructionRange FindCodeLast(params Object[] contains) {
            return new InstructionRange(instructions, contains, instructions.Count, -1);
        }

        public InstructionRange BeginCode() {
            return new InstructionRange(instructions, 0, 0);
        }

        public InstructionRange EndCode() {
            return new InstructionRange(instructions, instructions.Count, 0);
        }

        public InstructionRange AllCode() {
            return new InstructionRange(instructions, 0, instructions.Count);
        }

        public Label AttachLabel(CodeInstruction target) {
            var lbl = generator.DefineLabel();
            target.labels.Add(lbl);
            return lbl;
        }
        
        public string getReportUrl() {
            return "https://github.com/bcmpinc/StardewHack";
        }
    }

    // I 'love' generics. :P
    // Used to have a separate static instance variable per type T.
    public abstract class Hack<T> : HackBase where T : Hack<T>
    {
        /// <summary>
        /// A reference to this class's instance. 
        /// </summary>
        static T instance;

        /// <summary>
        /// Maps the method being patched to the method doing said patching. 
        /// </summary>
        static Dictionary<MethodBase, MethodInfo> patchmap = new Dictionary<MethodBase, MethodInfo>();

        /// <summary>
        /// A stack to allow patches to trigger additional patches. 
        /// This is necessary when dealing with delegates. 
        /// </summary>
        static Stack<MethodBase> to_be_patched = new Stack<MethodBase>();

        /// <summary>
        /// Applies the methods annotated with BytecodePatch defined in this class. 
        /// </summary>
        public override void Entry(IModHelper helper) {
            if (typeof(T) != this.GetType()) throw new Exception($"The type of this ({this.GetType()}) must be the same as the generic argument T ({typeof(T)}).");
            base.Entry(helper);
            instance = (T)this;

            // Iterate all methods in this class and search for those that have a BytecodePatch annotation.
            var methods = typeof(T).GetMethods(AccessTools.all);
            var apply = AccessTools.Method(typeof(Hack<T>), "ApplyPatch");
            var broken = false;
            foreach (MethodInfo patch in methods) {
                var bytecode_patches = patch.GetCustomAttributes<BytecodePatch>();
                foreach (var bp in bytecode_patches) {
                    if (bp.IsEnabled(this)) {
                        // Add the patch to the to_be_patched stack.
                        ChainPatch(bp.GetMethod(), patch);
                    }
                }
                // Apply the patch to the method specified in the annotation.
                while (to_be_patched.Count > 0) {
                    var method = to_be_patched.Pop();
                    try {
                        harmony.Patch(method, null, null, new HarmonyMethod(apply));
                    } catch (Exception err) {
                        if (!broken) {
                            Monitor.Log("The patch failed to apply cleanly. Usually this means the mod needs to be updated.", LogLevel.Alert);
                            Monitor.Log("As a result, this mod does not function properly or at all.", LogLevel.Alert);
                            Monitor.Log("Please upload your log file at https://log.smapi.io/ and report this bug at "+getReportUrl()+".", LogLevel.Alert);
                            StardewHack.Library.ModEntry.broken_mods.Add(helper.ModRegistry.ModID);
                            broken = true;
                        }
                        LogException(err);
                    }
                }
            }
        }
        
        public void LogException(Exception err, LogLevel level = LogLevel.Error) {
            while (err.InnerException != null) {
                err = err.InnerException;
            }
            Monitor.Log(err.Message + System.Environment.NewLine + err.StackTrace, level);
        }

        /// <summary>
        /// Applies the given patch to the given method. 
        /// This method can be called from within a patch method, for example to patch delegate functions. 
        /// </summary>
        public void ChainPatch(MethodBase method, MethodInfo patch) {
            if (patchmap.ContainsKey(method)) {
                // We allow chain patch to be called multiple times with the same arguments, which will be silently ignored,
                // because harmony can execute the patch method multiple times.
                // Different arguments however, are an error.
                if (!patchmap[method].Equals(patch)) {
                    throw new Exception($"StardewHack can't apply patch {patch} to {method}, because it is already patched by {patchmap[method]}.");
                }
            } else {
                patchmap[method] = patch;
                to_be_patched.Push(method);
            }
        }

        /// <summary>
        /// Called by harmony to apply a patch. 
        /// </summary> 
        private static IEnumerable<CodeInstruction> ApplyPatch(MethodBase original, ILGenerator generator, IEnumerable<CodeInstruction> instructions) {
            // Set the patch's references to this method's arguments.
            instance.original = original;
            instance.generator = generator;
            instance.instructions = new List<CodeInstruction>(instructions);

            // Obtain the patch method
            var patch = patchmap[original];

            // Print info 
            string info = $"Applying patch {patch.Name} to {original} in {original.DeclaringType.FullName}.";
            instance.Monitor.Log(info, LogLevel.Trace);

            // Apply the patch
            patch.Invoke(instance, null);

            // Keep a reference to the resulting code.
            instructions = instance.instructions;

            // Clear the patch's references to this method's arguments.
            instance.original = null;
            instance.generator = null;
            instance.instructions = null;

            // Return the resulting code.
            return instructions;
        }
    }

    public abstract class HackWithConfig<T, C> : Hack<T> where T : HackWithConfig<T, C> where C : class, new()
    {
        public C config;

        public override void Entry(IModHelper helper) {
            config = helper.ReadConfig<C>();
            base.Entry(helper);
        }
    }
}

