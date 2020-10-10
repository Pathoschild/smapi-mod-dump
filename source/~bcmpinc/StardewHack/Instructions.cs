/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using Harmony;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace StardewHack
{
    public static class Instructions
    {
        // A
        public static CodeInstruction Add() => new CodeInstruction(OpCodes.Add);

        // B
        // Use Instructions.Br(AttachLabel(CodeInstruction)) to create branch to the given code instruction.
        public static CodeInstruction Bge    (Label target) => new CodeInstruction(OpCodes.Bge,     target);
        public static CodeInstruction Bgt    (Label target) => new CodeInstruction(OpCodes.Bgt,     target);
        public static CodeInstruction Ble    (Label target) => new CodeInstruction(OpCodes.Ble,     target);
        public static CodeInstruction Blt    (Label target) => new CodeInstruction(OpCodes.Blt,     target);
        public static CodeInstruction Beq    (Label target) => new CodeInstruction(OpCodes.Beq,     target);
        public static CodeInstruction Bne_Un (Label target) => new CodeInstruction(OpCodes.Bne_Un,  target);
        public static CodeInstruction Br     (Label target) => new CodeInstruction(OpCodes.Br,      target);
        public static CodeInstruction Brfalse(Label target) => new CodeInstruction(OpCodes.Brfalse, target);
        public static CodeInstruction Brtrue (Label target) => new CodeInstruction(OpCodes.Brtrue,  target);

        // C
        public static CodeInstruction Call        (Type type, string method, params Type[] parameters) => new CodeInstruction(OpCodes.Call,     GetMethod(type, method, parameters));
        public static CodeInstruction Callvirt    (Type type, string method, params Type[] parameters) => new CodeInstruction(OpCodes.Callvirt, GetMethod(type, method, parameters));
        public static CodeInstruction Call_get    (Type type, string method) => new CodeInstruction(OpCodes.Call,     GetProperty(type, method).GetGetMethod());
        public static CodeInstruction Callvirt_get(Type type, string method) => new CodeInstruction(OpCodes.Callvirt, GetProperty(type, method).GetGetMethod());
        public static CodeInstruction Call_set    (Type type, string method) => new CodeInstruction(OpCodes.Call,     GetProperty(type, method).GetSetMethod());
        public static CodeInstruction Callvirt_set(Type type, string method) => new CodeInstruction(OpCodes.Callvirt, GetProperty(type, method).GetSetMethod());
        public static CodeInstruction Conv_I() => new CodeInstruction(OpCodes.Conv_I);
        public static CodeInstruction Conv_U() => new CodeInstruction(OpCodes.Conv_U);
        public static CodeInstruction Conv_I1() => new CodeInstruction(OpCodes.Conv_I1);
        public static CodeInstruction Conv_I2() => new CodeInstruction(OpCodes.Conv_I2);
        public static CodeInstruction Conv_I4() => new CodeInstruction(OpCodes.Conv_I4);
        public static CodeInstruction Conv_I8() => new CodeInstruction(OpCodes.Conv_I8);
        public static CodeInstruction Conv_R4() => new CodeInstruction(OpCodes.Conv_R4);
        public static CodeInstruction Conv_R8() => new CodeInstruction(OpCodes.Conv_R8);
        public static CodeInstruction Conv_U1() => new CodeInstruction(OpCodes.Conv_U1);
        public static CodeInstruction Conv_U2() => new CodeInstruction(OpCodes.Conv_U2);
        public static CodeInstruction Conv_U4() => new CodeInstruction(OpCodes.Conv_U4);
        public static CodeInstruction Conv_U8() => new CodeInstruction(OpCodes.Conv_U8);

        // D
        public static CodeInstruction Dup() => new CodeInstruction(OpCodes.Dup);

        // I
        public static CodeInstruction Isinst(Type type) => new CodeInstruction(OpCodes.Isinst, type);

        // L
        public static CodeInstruction Ldarg_0() => new CodeInstruction(OpCodes.Ldarg_0);
        public static CodeInstruction Ldarg_1() => new CodeInstruction(OpCodes.Ldarg_1);
        public static CodeInstruction Ldarg_2() => new CodeInstruction(OpCodes.Ldarg_2);
        public static CodeInstruction Ldarg_3() => new CodeInstruction(OpCodes.Ldarg_3);
        public static CodeInstruction Ldarg_S(byte index) => new CodeInstruction(OpCodes.Ldarg_S, index);

        public static CodeInstruction Ldc_I4_0() => new CodeInstruction(OpCodes.Ldc_I4_0);
        public static CodeInstruction Ldc_I4_1() => new CodeInstruction(OpCodes.Ldc_I4_1);
        public static CodeInstruction Ldc_I4_2() => new CodeInstruction(OpCodes.Ldc_I4_2);
        public static CodeInstruction Ldc_I4_3() => new CodeInstruction(OpCodes.Ldc_I4_3);
        public static CodeInstruction Ldc_I4_4() => new CodeInstruction(OpCodes.Ldc_I4_4);
        public static CodeInstruction Ldc_I4_5() => new CodeInstruction(OpCodes.Ldc_I4_5);
        public static CodeInstruction Ldc_I4_6() => new CodeInstruction(OpCodes.Ldc_I4_6);
        public static CodeInstruction Ldc_I4_7() => new CodeInstruction(OpCodes.Ldc_I4_7);
        public static CodeInstruction Ldc_I4_8() => new CodeInstruction(OpCodes.Ldc_I4_8);
        public static CodeInstruction Ldc_I4_M1() => new CodeInstruction(OpCodes.Ldc_I4_M1);
        public static CodeInstruction Ldc_I4_S(byte value) => new CodeInstruction(OpCodes.Ldc_I4_S, value);
        public static CodeInstruction Ldc_I4(int value) => new CodeInstruction(OpCodes.Ldc_I4, value);
        public static CodeInstruction Ldc_R4(float value) => new CodeInstruction(OpCodes.Ldc_R4, value);
        public static CodeInstruction Ldc_R8(double value) => new CodeInstruction(OpCodes.Ldc_R8, value);

        public static CodeInstruction Ldfld (Type type, string field) => new CodeInstruction(OpCodes.Ldfld,  GetField(type, field));
        public static CodeInstruction Ldsfld(Type type, string field) => new CodeInstruction(OpCodes.Ldsfld, GetField(type, field));
        public static CodeInstruction Ldloc_0() => new CodeInstruction(OpCodes.Ldloc_0);
        public static CodeInstruction Ldloc_1() => new CodeInstruction(OpCodes.Ldloc_1);
        public static CodeInstruction Ldloc_2() => new CodeInstruction(OpCodes.Ldloc_2);
        public static CodeInstruction Ldloc_3() => new CodeInstruction(OpCodes.Ldloc_3);
        public static CodeInstruction Ldloc   (LocalBuilder local) => new CodeInstruction(OpCodes.Ldloc,    local);
        public static CodeInstruction Ldloc_S (LocalBuilder local) => new CodeInstruction(OpCodes.Ldloc_S,  local);
        public static CodeInstruction Ldloca  (LocalBuilder local) => new CodeInstruction(OpCodes.Ldloca,   local);
        public static CodeInstruction Ldloca_S(LocalBuilder local) => new CodeInstruction(OpCodes.Ldloca_S, local);
        public static CodeInstruction Ldnull() => new CodeInstruction(OpCodes.Ldnull);
        public static CodeInstruction Ldstr(string text) => new CodeInstruction(OpCodes.Ldstr, text);

        // M
        public static CodeInstruction Mul() => new CodeInstruction(OpCodes.Mul);

        // N
        public static CodeInstruction Nop() => new CodeInstruction(OpCodes.Nop);

        // P
        public static CodeInstruction Pop() => new CodeInstruction(OpCodes.Pop);

        // R
        public static CodeInstruction Ret() => new CodeInstruction(OpCodes.Ret);
        
        // S
        public static CodeInstruction Starg_S(byte index) => new CodeInstruction(OpCodes.Starg_S, index);

        public static CodeInstruction Stfld (Type type, string field) => new CodeInstruction(OpCodes.Stfld,  GetField(type, field));
        public static CodeInstruction Stsfld(Type type, string field) => new CodeInstruction(OpCodes.Stsfld, GetField(type, field));
        public static CodeInstruction Stloc_0() => new CodeInstruction(OpCodes.Stloc_0);
        public static CodeInstruction Stloc_1() => new CodeInstruction(OpCodes.Stloc_1);
        public static CodeInstruction Stloc_2() => new CodeInstruction(OpCodes.Stloc_2);
        public static CodeInstruction Stloc_3() => new CodeInstruction(OpCodes.Stloc_3);
        public static CodeInstruction Stloc_S(LocalBuilder local) => new CodeInstruction(OpCodes.Stloc_S, local);

        public static CodeInstruction Sub() => new CodeInstruction(OpCodes.Sub);


        /// <summary>
        /// Retrieves the field definition with the specified name.
        /// </summary>
        internal static FieldInfo GetField(Type type, string field) {
            var res = AccessTools.Field(type, field);
            if (res == null) {
                throw new MissingFieldException($"ERROR: field {type}.{field} not found.");
            }
            return res;
        }

        /// <summary>
        /// Retrieves the property definition with the specified name. 
        /// </summary>
        internal static PropertyInfo GetProperty(Type type, string property) {
            var res = AccessTools.Property(type, property);
            if (res == null) {
                throw new MissingMemberException($"ERROR: property {type}.{property} not found.");
            }
            return res;
        }

        /// <summary>
        /// Retrieves the type definition with the specified name.
        /// </summary>
        internal static MethodInfo GetMethod(Type type, string method, Type[] parameters) {
            var res = AccessTools.Method(type, method, parameters);
            if (res == null) {
                string args = String.Join(",", (object[])parameters);
                throw new MissingMemberException($"ERROR: member {type}.{method}({args}) not found.");
            }
            return res;
        }
    }
}

