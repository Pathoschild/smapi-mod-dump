/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/SplitScreen
**
*************************************************/

using System;
using System.Diagnostics;
using System.Linq;

namespace SplitScreen
{
	public static class AffinitySetter
	{
		private static IntPtr GetAffinityForSelectProcessors(params int[] processors) => (IntPtr)processors.ToList().Aggregate(0, (a, b) => a | (1 << b - 1));//Not zero based
			
		/// <summary>
		/// NOT ZERO BASED
		/// </summary>
		/// <param name="processor"></param>
		public static void SetDesignatedProcessor(int processor)
		{
			Process process = Process.GetCurrentProcess();
			process.ProcessorAffinity = GetAffinityForSelectProcessors(processor);
		}
	}
}
