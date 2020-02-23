using System;
using System.IO;
using System.Reflection;

namespace SpriteMaster.Resample {
	internal sealed class NTFS {
		internal static bool CompressDirectory(string path) {
			if (!Runtime.IsWindows)
				return false;
			try {
				var dir = new DirectoryInfo(path);
				if ((dir.Attributes & FileAttributes.Compressed) == 0) {
					var objectPath = $"Win32_Directory.Name='{dir.FullName.Replace("\\", @"\\").TrimEnd('\\')}'";

					// I am switching this to use reflection does it doesn't try to search for these assemblies on Unix.

					var managementAssembly = Assembly.Load("System.Management");
					var managementObjectClass = managementAssembly.GetType("System.Management.ManagementObject");
					var invokeMethod = managementObjectClass.GetMethod("InvokeMethod", new Type[] { typeof(string), typeof(object[]) });

					using var obj = (IDisposable)Activator.CreateInstance(managementObjectClass, new object[] { objectPath });
					using ((IDisposable)invokeMethod.Invoke(obj, new object[] { "Compress", new object[] { } })) {
						// I don't really care about the return value, 
						// if we enabled it great but it can also be done manually
						// if really needed
					}
				}
			}
			catch {
				return false;
			}
			try {
				var dir = new DirectoryInfo(path);
				return (dir.Attributes & FileAttributes.Compressed) != 0;
			}
			catch {
				return false;
			}
		}
	}
}
