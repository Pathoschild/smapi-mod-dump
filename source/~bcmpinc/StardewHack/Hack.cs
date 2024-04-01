/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using HarmonyLib;
using StardewHack.Library;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        public Harmony harmony { get; private set; }

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

        /// <summary>
        /// Maps the method being patched to the method doing said patching. 
        /// </summary>
        private Dictionary<MethodBase, MethodInfo> patchmap = new Dictionary<MethodBase, MethodInfo>();

        /// <summary>
        /// A stack to allow patches to trigger additional patches. 
        /// This is necessary when dealing with delegates. 
        /// </summary>
        private Stack<MethodBase> to_be_patched = new Stack<MethodBase>();

        private bool broken = false;

        abstract internal MethodInfo getApplyPatchProxy(string UniqueID);

        /// <summary>
        /// Applies the methods annotated with BytecodePatch defined in this class. 
        /// </summary>
        public override void Entry(IModHelper helper) {
            this.helper = helper;

            try {
                ModChecks.validateAssemblyVersion(this);
                ModChecks.checkIncompatible(this);

                // Use the Mod's UniqueID to create the harmony instance.
                string UniqueID = helper.ModRegistry.ModID;
                Monitor.Log($"Applying bytecode patches for {UniqueID}.", LogLevel.Debug);
                harmony = new Harmony(UniqueID);

                // Let the mod register its patches.
                HackEntry(helper);

                // Apply the registered patches.
                // Any patched that are added by calls to ChainPatch during patching will be applied as well.
                var apply = getApplyPatchProxy(UniqueID);
                while (to_be_patched.Count > 0) {
                    var method = to_be_patched.Pop();
                    try {
                        harmony.Patch(method, null, null, new HarmonyMethod(apply));
                    } catch (Exception err) {
                        MarkAsBroken(err);
                    }
                }
            } catch (Exception err) {
                Monitor.Log("The mod failed to initialize cleanly. To avoid further problems, the game will not load.", LogLevel.Error);
                Monitor.Log("The mod is either broken or incompatible with your version of Stardew Valley, or any of your other mods.", LogLevel.Error);
                Monitor.Log("Please try updating the mod, or otherwise removing it.", LogLevel.Error);
                Monitor.Log("Please upload your log file at https://log.smapi.io/ and report this bug at " + getReportUrl() + ".", LogLevel.Error);
                ModChecks.InitializationError(this);
                LogException(err);
            }
        }

        public abstract void HackEntry(IModHelper helper);

        /// <summary>
        /// Find the first occurance of the given sequence of instructions that follows this range.
        /// The performed matching depends on the type:
        ///  - String: is it contained in the string representation of the instruction
        ///  - MemberReference (including MethodDefinition): is the instruction's operand equal to this reference.
        ///  - OpCode: is this the instruction's OpCode.
        ///  - CodeInstruction: are the instruction's OpCode and Operand equal.
        ///  - null: always matches.
        /// </summary>
        public InstructionRange FindCode(params InstructionMatcher[] contains) {
            return new InstructionRange(instructions, contains);
        }

        /// <summary>
        /// Find the last occurance of the given sequence of instructions that follows this range.
        /// See FindCode() for how the matching is performed.
        /// </summary>
        public InstructionRange FindCodeLast(params InstructionMatcher[] contains) {
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
        
        public void LogException(Exception err, LogLevel level = LogLevel.Error) {
            while (err.InnerException != null) {
                err = err.InnerException;
            }
            Monitor.Log(err.ToString(), level);
        }

        internal MethodBase getMethodBase(LambdaExpression arg) {
            switch (arg.Body) {
                case MethodCallExpression m:
                    var r = m.Method;
                    // The Expression gets the method defined in the base class. For subclasses we have to get the 
                    // overriden definition of the method.
                    if (r.IsVirtual && r.DeclaringType != m.Object.Type) {
                        r = m.Object.Type.GetMethod(r.Name, r.GetParameters().Types());
                    }
                    return r;
                case NewExpression m:
                    return m.Constructor;
                default:
                    throw new ArgumentException("Expression body has unexpected type: " + arg.Body.Type.Name);
            }
        }

        void MarkAsBroken(Exception err) {
            if (!broken) {
                Monitor.Log("The patch failed to apply cleanly. Usually this means the mod needs to be updated.", LogLevel.Alert);
                Monitor.Log("As a result, this mod does not function properly or at all.", LogLevel.Alert);
                Monitor.Log("Please upload your log file at https://log.smapi.io/ and report this bug at " + getReportUrl() + ".", LogLevel.Alert);
                ModChecks.failedPatches(this);
                broken = true;
            }
            LogException(err);
        }

        public void Patch(Expression<Action> method, Action patch) => ChainPatch(getMethodBase(method), patch.Method);
        public void Patch<X>(Expression<Action<X>> method, Action patch) => ChainPatch(getMethodBase(method), patch.Method);

        public void Patch(Type type, String methodName, Action patch) {
            var method = AccessTools.DeclaredMethod(type, methodName);
            if (method == null) {
                throw new Exception($"Failed to find method \"{methodName}\" in {type.FullName}.");
            }
            ChainPatch(method, patch.Method);
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
        public IEnumerable<CodeInstruction> ApplyPatch(MethodBase original, ILGenerator generator, IEnumerable<CodeInstruction> instructions) {
            // Set the patch's references to this method's arguments.
            this.original = original;
            this.generator = generator;
            this.instructions = new List<CodeInstruction>(instructions);

            // Obtain the patch method
            var patch = patchmap[original];

            // Print info 
            string info = $"Applying patch {patch.Name} to {original} in {original.DeclaringType.FullName}.";
            this.Monitor.Log(info, LogLevel.Trace);

            // Apply the patch
            patch.Invoke(this, null);

            // Keep a reference to the resulting code.
            instructions = this.instructions;

            // Clear the patch's references to this method's arguments.
            this.original = null;
            this.generator = null;
            this.instructions = null;

            // Return the resulting code.
            return instructions;
        }
    }

#pragma warning disable RECS0108 // Warns about static fields in generic types
    // I 'love' generics. :P
    // This pattern is used to have a separate static instance variable per type T.
    public abstract class HackImpl<T> : HackBase where T : HackImpl<T>
    {
        internal readonly static ModuleBuilder ProxyModule = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("StardewHack.Proxies"), AssemblyBuilderAccess.Run).DefineDynamicModule("StardewHack.Proxies");

        /// <summary>
        /// A reference to this class's instance. 
        /// </summary>
        internal static T instance;

        protected HackImpl() {
            instance = (T)this;
        }

        /// Returns the used instance of this class.
        public static T getInstance() {
            return instance;
        }

        internal override MethodInfo getApplyPatchProxy(string UniqueID) {
            MethodInfo instance = AccessTools.Method(GetType(), nameof(getInstance));
            MethodInfo apply = AccessTools.Method(typeof(HackBase), nameof(ApplyPatch));
            string className = UniqueID.Replace('.', '_') + "_proxy";
            string methodName = "ApplyPatch";

            TypeBuilder typeBuilder = ProxyModule.DefineType(className);
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Static, apply.ReturnType, apply.GetParameters().Types());

            if (apply.GetParameters().Length != 3) throw new InvalidOperationException("StardewHack cannot build patch proxy.");
            ILGenerator il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Call, instance);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, apply);
            il.Emit(OpCodes.Ret);

            Type t = typeBuilder.CreateType();
            return AccessTools.Method(t, methodName);
        }
    }
#pragma warning restore RECS0108 // Warns about static fields in generic types

    public abstract class Hack<T> : HackImpl<T> where T : Hack<T> {
        public sealed override void Entry(IModHelper helper) {
            base.Entry(helper);
        }
    }

    public abstract class HackWithConfig<T, C> : HackImpl<T> where T : HackWithConfig<T, C> where C : class, new()
    {
        public C config;

        public static C getConfig() {
            return getInstance().config;
        }

        public sealed override void Entry(IModHelper helper) {
            config = helper.ReadConfig<C>();
            base.Entry(helper);
            Helper.Events.GameLoop.GameLaunched += onLaunched;
        }

        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenu.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null) {
                api.Register(
                    mod: ModManifest, 
                    reset: () => config = new C(), 
                    save: () => Helper.WriteConfig(config),
                    titleScreenOnly: false
                );
                InitializeApi(api);
            }
        }
        
        abstract protected void InitializeApi(GenericModConfigMenu.IGenericModConfigMenuApi api);
    }
}
