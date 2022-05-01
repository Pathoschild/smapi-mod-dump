/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using HarmonyLib;
using SpriteMaster.Configuration;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpriteMaster.Experimental;

static class Inlining {
	// https://github.com/MonoMod/MonoMod.Common/blob/7d799091ba6e740988b82fe233cb3fa00ef32611/RuntimeDetour/Platforms/Runtime/DetourRuntimeNETCore30Platform.cs
	private static unsafe void EnableInlining(MethodBase method) {
		RuntimeMethodHandle handle = method.MethodHandle;

		// https://github.com/dotnet/runtime/blob/89965be3ad2be404dc82bd9e688d5dd2a04bcb5f/src/coreclr/src/vm/method.hpp#L178
		// mdcNotInline = 0x2000
		// References to RuntimeMethodHandle (CORINFO_METHOD_HANDLE) pointing to MethodDesc
		// can be traced as far back as https://ntcore.com/files/netint_injection.htm

		const int offset =
				2 // UINT16 m_wFlags3AndTokenRemainder
			+ 1 // BYTE m_chunkIndex
			+ 1 // BYTE m_chunkIndex
			+ 2 // WORD m_wSlotNumber
			;
		ushort* m_wFlags = (ushort*)(((byte*)handle.Value) + offset);
		ushort wFlags = *m_wFlags;
		if ((wFlags & 0x2000) == 0) {
			// inlining is already enabled
			return;
		}
		*m_wFlags = (ushort)(wFlags & unchecked((ushort)~0x2000));
	}

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	internal static void Reenable() {
		if (!Config.Extras.HarmonyInlining) {
			return;
		}

		var loopResult = Parallel.ForEach(Harmony.GetAllPatchedMethods(), method => {
			EnableInlining(method);
		});
	}
}
