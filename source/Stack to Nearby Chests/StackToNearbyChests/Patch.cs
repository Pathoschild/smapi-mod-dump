using Harmony;
using System;
using System.Linq;
using System.Reflection;

namespace StackToNearbyChests
{
	abstract class Patch
	{
		/// <summary>
		/// Don't use typeof() or it won't work on Mac/Linux
		/// </summary>
		/// <returns></returns>
		public abstract Type GetTargetType();

		/// <summary>
		/// Null if constructor is desired
		/// </summary>
		public abstract string GetTargetMethodName();

		public abstract Type[] GetTargetMethodArguments();

		public void ApplyPatch(HarmonyInstance harmonyInstance)
		{
			MethodBase targetMethod = String.IsNullOrEmpty(GetTargetMethodName()) ? 
				(MethodBase)GetTargetType().GetConstructor(GetTargetMethodArguments()) : 
				targetMethod = GetTargetType().GetMethod(GetTargetMethodName(), GetTargetMethodArguments());
				
			harmonyInstance.Patch(targetMethod, new HarmonyMethod(GetType().GetMethod("Prefix")), new HarmonyMethod(GetType().GetMethod("Postfix")));
		}

		public static void PatchAll(HarmonyInstance harmonyInstance)
		{
			foreach (Type type in (from type in Assembly.GetExecutingAssembly().GetTypes()
								   where type.IsClass && type.BaseType == typeof(Patch)
								   select type))
				((Patch)Activator.CreateInstance(type)).ApplyPatch(harmonyInstance);
		}
	}
}
