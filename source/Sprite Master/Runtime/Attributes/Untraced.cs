using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace SpriteMaster.Attributes {
	// https://stackoverflow.com/a/11898531
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class UntracedAttribute : Attribute { }

	public static class DebugExtensions {
		[DebuggerStepThrough, DebuggerHidden(), Untraced]
		public static bool IsUntraced (this MethodBase method) {
			if (method == null) {
				return false;
			}
			return method.IsDefined(typeof(UntracedAttribute), true);
		}

		[DebuggerStepThrough, DebuggerHidden(), Untraced]
		public static string GetStackTrace (this Exception e) {
			var tracedFrames = new List<StackFrame>();
			foreach (var frame in new StackTrace(e, true).GetFrames()) {
				if (!frame.GetMethod().IsUntraced()) {
					tracedFrames.Add(frame);
				}
			}

			var tracedStrings = new List<string>();
			foreach (var frame in tracedFrames) {
				tracedStrings.Add(new StackTrace(frame).ToString());
			}

			return string.Concat(tracedStrings);
		}
	}
}
