/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using HarmonyLib;
using LinqFasterer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SpriteMaster.Experimental;

internal static class MethodImpl {
	internal enum MethodClassification : byte {
		mcIL = 0,           // IL
		mcFCall = 1,        // FCall (also includes tlbimped ctor, Delegate ctor)
		mcNDirect = 2,      // N/Direct
		mcEEImpl = 3,       // special method; implementation provided by EE (like Delegate Invoke)
		mcArray = 4,        // Array ECall
		mcInstantiated = 5, // Instantiated generic methods, including descriptors
		mcComInterop = 6,
		mcDynamic = 7,      // for method desc with no metadata behind
		mcCount,
		mcMask = mcCount - 1
	}

	[Flags]
	internal enum MethodDescClassification : ushort {
		mcdHasNonVtableSlot = 0x0008,
		mcdMethodImpl = 0x0010,
		mcdHasNativeCodeSlot = 0x0020,
		mcdHasComPlusCallInfo = 0x0040,
		mcdStatic = 0x0080,
		mcdDuplicate = 0x0400,
		mcdVerifiedState = 0x0800,
		mcdVerifiable = 0x1000,
		mcdNotInline = 0x2000,
		mcdSynchronized = 0x4000,
		mcdRequiresFullSlotNumber = 0x8000
	}

	[StructLayout(LayoutKind.Explicit, Pack = sizeof(ushort), Size = sizeof(ushort))]
	internal struct MethodDesc {
		[FieldOffset(0)]
		private ushort Value;

		internal MethodClassification Classification {
			get => (MethodClassification)(Value & (ushort)MethodClassification.mcMask);
			set => Value = (ushort)((Value & ~(ushort)MethodClassification.mcMask) | (ushort)value);
		}

		internal MethodDescClassification Flags {
			get => (MethodDescClassification)(Value & ~(ushort)MethodClassification.mcMask);
			set => Value = (ushort)((Value & (ushort)MethodClassification.mcMask) | (ushort)value);
		}
	}
}

internal static class Inlining {
	// https://github.com/MonoMod/MonoMod.Common/blob/7d799091ba6e740988b82fe233cb3fa00ef32611/RuntimeDetour/Platforms/Runtime/DetourRuntimeNETCore30Platform.cs
	private static unsafe void EnableInlining(MethodBase method) {
		RuntimeMethodHandle handle = method.MethodHandle;

		nint handlePtr = handle.Value;
		if (handlePtr == 0) {
			return;
		}

		// https://github.com/dotnet/runtime/blob/89965be3ad2be404dc82bd9e688d5dd2a04bcb5f/src/coreclr/src/vm/method.hpp#L178
		// References to RuntimeMethodHandle (CORINFO_METHOD_HANDLE) pointing to MethodDesc
		// can be traced as far back as https://ntcore.com/files/netint_injection.htm

		const int offset =
				2 // UINT16 m_wFlags3AndTokenRemainder
			+ 1 // BYTE m_chunkIndex
			+ 1 // BYTE m_chunkIndex
			+ 2 // WORD m_wSlotNumber
			;

		var descPtr = (MethodImpl.MethodDesc*)(handlePtr + offset);
		var desc = *descPtr;

		if (!desc.Flags.HasFlag(MethodImpl.MethodDescClassification.mcdNotInline)) {
			return;
		}

		desc.Flags &= ~MethodImpl.MethodDescClassification.mcdNotInline;

		*descPtr = desc;
	}

	[Flags]
	internal enum PatchType : uint {
		Finalizer = 1u << 0,
		Postfix = 1u << 1,
		Prefix = 1u << 2,
		Transpiler = 1u << 3
	}

	[MethodImpl(Runtime.MethodImpl.RunOnce)]
	internal static void Reenable(PatchType patchTypes = PatchType.Transpiler) {
#if false
		_ = Parallel.ForEach(Harmony.GetAllPatchedMethods(), patchedMethod => {
			var allPatches = Harmony.GetPatchInfo(patchedMethod);
			IList<Patch> inlinePatches = Array.Empty<Patch>();

			foreach ((var patchType, var patches) in new[] {
				(PatchType.Finalizer, allPatches.Finalizers),
				(PatchType.Postfix, allPatches.Postfixes),
				(PatchType.Prefix, allPatches.Prefixes),
				(PatchType.Transpiler, allPatches.Transpilers)
			}) {
				if (patchTypes.HasFlag(patchType)) {
					inlinePatches = inlinePatches.ConcatF(patches);
				}
			}

			foreach (var patch in inlinePatches) {
				if (patch.PatchMethod.DeclaringType?.Assembly != SpriteMaster.Assembly) {
					continue;
				}

				EnableInlining(patchedMethod);
				EnableInlining(patch.GetMethod(patchedMethod));
			}
		});
#endif
	}
}

#if false
internal static class Dynamic {
	[StructLayout(LayoutKind.Explicit, Pack = sizeof(ushort), Size = sizeof(ushort))]
	private struct MethodDesc {
		[FieldOffset(0)]
		private ushort Value;

		internal MethodClassification Classification {
			get => (MethodClassification)(Value & (ushort)MethodClassification.mcMask);
			set => Value = (ushort)((Value & ~(ushort)MethodClassification.mcMask) | (ushort)value);
		}

		internal MethodDescClassification Flags {
			get => (MethodDescClassification)(Value & ~(ushort)MethodClassification.mcMask);
			set => Value = (ushort)((Value & (ushort)MethodClassification.mcMask) | (ushort)value);
		}
	}

	internal static unsafe void Mark(MethodBase method) {
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

		var descPtr = (MethodDesc*)(handle.Value + offset);
		var desc = *descPtr;

		var descShort = *(ushort*)(handle.Value + offset);

		if (desc.Flags.HasFlag(MethodDescClassification.mcdVerifiedState)) {
			return;
		}

		desc.Flags |= MethodDescClassification.mcdVerifiedState;

		*descPtr = desc;
	}
}
#endif
