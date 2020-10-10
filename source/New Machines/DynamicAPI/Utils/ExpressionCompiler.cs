/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CSharp;

namespace Igorious.StardewValley.DynamicAPI.Utils
{
    public static class ExpressionCompiler
    {
        #region Private Data

        private static IDictionary<string, object> CachedFunctions { get; } = new Dictionary<string, object>();

        #endregion

        #region	Public Methods

        public static TDelegate CompileExpression<TDelegate>(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return default(TDelegate);

            var delegateInfo = typeof(TDelegate).GetMethod("Invoke");
            var delegateParameters = delegateInfo.GetParameters();
            var args = delegateParameters.Select(p => p.Name).ToList();

            var key = $"{typeof(TDelegate).Name}: {string.Join(", ", args)} => {body}";
            object value;
            if (CachedFunctions.TryGetValue(key, out value)) return (TDelegate)value;

            var argTypes = delegateParameters.Select(p => p.ParameterType).ToArray();
            var resultType = delegateInfo.ReturnType;

            TDelegate expression;
            int intValue;
            if (int.TryParse(body, out intValue))
            {
                var dynamicMethod = new DynamicMethod("", resultType, argTypes);
                var il = dynamicMethod.GetILGenerator();
                il.Emit(OpCodes.Ldc_I4, intValue);
                il.Emit(OpCodes.Ret);
                expression = (TDelegate)(object)dynamicMethod.CreateDelegate(typeof(TDelegate));
            }
            else
            {
                var methodInfo = CompileMethod(resultType, delegateParameters, body);
                expression = (TDelegate)(object)Delegate.CreateDelegate(typeof(TDelegate), methodInfo);
            }
          
            CachedFunctions.Add(key, expression);
            return expression;
        }

        #endregion

        #region	Auxiliary Methods

        private static MethodInfo CompileMethod(Type result, IEnumerable<ParameterInfo> parameters, string body)
        {
            var code = @"             
            public static class DynamicFunction
            {                
                public static $TypeResult Function($Arguments)
                {
                    return $Body;
                }
            }"
            .Replace("$TypeResult", result.FullName)
            .Replace("$Arguments", string.Join(", ", parameters.Select(p => $"{p.ParameterType.FullName} {p.Name}")))
            .Replace("$Body", body);

            var provider = new CSharpCodeProvider();
            var compilerParameters = new CompilerParameters {GenerateInMemory = true, IncludeDebugInformation = true };
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            var results = provider.CompileAssemblyFromSource(compilerParameters, code);
            if (results.Errors.HasErrors) return null;

            var dynamicFunction = results.CompiledAssembly.GetType("DynamicFunction");
            return dynamicFunction.GetMethod("Function");
        }

        #endregion
    }
}