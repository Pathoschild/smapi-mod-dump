/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

using Harmony;
using System;
using System.Linq;
using System.Reflection;

namespace BattleRoyale
{
	abstract class Patch
	{
		protected abstract PatchDescriptor GetPatchDescriptor();

		//Remember Prefix/Postfix should be public and static! Do not use lambdas

		protected class PatchDescriptor
		{
			public Type targetType;
			public string targetMethodName;
			public Type[] targetMethodArguments;

			/// <param name="targetType">Use typeof()</param>
			/// <param name="targetMethodName">Null if constructor is desired</param>
			/// <param name="targetMethodArguments">Null if no method abiguity</param>
			public PatchDescriptor(Type targetType, string targetMethodName, Type[] targetMethodArguments = null)
			{
				this.targetType = targetType;
				this.targetMethodName = targetMethodName;
				this.targetMethodArguments = targetMethodArguments;
			}
		}
		

		private void ApplyPatch(HarmonyInstance harmonyInstance)
		{
			var patchDescriptor = GetPatchDescriptor();
			
			MethodBase targetMethod = String.IsNullOrEmpty(patchDescriptor.targetMethodName) ?

				(MethodBase)patchDescriptor.targetType.GetConstructor(patchDescriptor.targetMethodArguments ?? new Type[0]) :

				targetMethod = patchDescriptor.targetMethodArguments != null ?
					patchDescriptor.targetType.GetMethod(patchDescriptor.targetMethodName, patchDescriptor.targetMethodArguments)
					: patchDescriptor.targetType.GetMethod(patchDescriptor.targetMethodName, ((BindingFlags)62));

			try
			{
				harmonyInstance.Patch(targetMethod,
					new HarmonyMethod(GetType().GetMethod("Prefix")),
					new HarmonyMethod(GetType().GetMethod("Postfix")),
					new HarmonyMethod(GetType().GetMethod("Transpiler") ?? GetType().GetMethod("Transpile")));
			}catch(Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public static void PatchAll(string id)
		{
			HarmonyInstance harmonyInstance = HarmonyInstance.Create(id);

			foreach (Type type in (from type in Assembly.GetExecutingAssembly().GetTypes()
								   where type.IsClass && type.BaseType == typeof(Patch)
								   select type))
				((Patch)Activator.CreateInstance(type)).ApplyPatch(harmonyInstance);
			
		}
	}
}
