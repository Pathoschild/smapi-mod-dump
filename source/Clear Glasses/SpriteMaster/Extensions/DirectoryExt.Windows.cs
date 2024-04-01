/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace SpriteMaster.Extensions;

[SupportedOSPlatform("windows")]
internal static class DirectoryExtWindows {
	private delegate object? InvokeMethodDelegate(IDisposable managementObject, string methodName, params object[] args);

	private static readonly Type? ManagementObjectType = null;
	private static readonly InvokeMethodDelegate? ManagementInvokeMethod = null;

	[MemberNotNullWhen(true, "ManagementInvokeMethod", "ManagementObjectType")]
	private static bool Supported { get; } = false;

	static DirectoryExtWindows() {
		try {
			var managementAssembly = Assembly.Load("System.Management");
			ManagementObjectType = managementAssembly.GetType("System.Management.ManagementObject");

			if (ManagementObjectType is null) {
				return;
			}

			var managementInvokeMethodInfo = ManagementObjectType.GetMethod("InvokeMethod", new[] { typeof(string), typeof(object[]) });

			ManagementInvokeMethod = managementInvokeMethodInfo?.CreateDelegate<InvokeMethodDelegate>();

			Supported = ManagementInvokeMethod is not null;
		}
		catch {
			// ignored
		}
	}

	internal static bool CompressDirectory(string path, bool force) {
		var info = new DirectoryInfo(path);

		if (!info.Exists) {
			return false;
		}

		if (Supported) {
			try {
				if (!force && info.Attributes.HasFlag(FileAttributes.Compressed)) {
					return true;
				}

				var objectPath = $"Win32_Directory.Name='{info.FullName.Replace("\\", @"\\").TrimEnd('\\')}'";

				// I am switching this to use reflection does it doesn't try to search for these assemblies on Unix.

				using var obj = (IDisposable?)Activator.CreateInstance(ManagementObjectType, objectPath);
				if (obj is null) {
					return false;
				}

				_ = ManagementInvokeMethod.Invoke(obj, "Compress");
			}
			catch {
				return false;
			}
		}

		try {
			// Attempt to use LZX compression, if possible
			var processInfo = new ProcessStartInfo("compact.exe") {
				CreateNoWindow = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				RedirectStandardInput = true
			};
			processInfo.ArgumentList.AddRange("/C", "/I", "/F", $"/S:{path}", "/EXE:LZX");

			var process = Process.Start(processInfo);
			if (process is not null) {
				Task.Run(
					async () => {
						using var disposableProcess = process;
						await disposableProcess.WaitForExitAsync();
					}
				);
			}
		}
		catch {
			return false;
		}
		try {
			info.Refresh();
			return info.Attributes.HasFlag(FileAttributes.Compressed);
		}
		catch {
			return false;
		}
	}
}
