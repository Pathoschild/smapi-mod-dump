using Harmony;
using System;
using System.Reflection;

namespace StardewHack
{
    /** Indicates that this is a transpiler for the given method.
     * Can be used multiple times to patch multiple methods.
     */
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]  
    public class BytecodePatch : System.Attribute  
    {
        string sig;
        string enabled;

        public bool IsEnabled(HackBase hack)
        {
            if (enabled == null) return true;
            var method = AccessTools.Method(hack.GetType(), enabled);
            return (bool)method.Invoke(hack, null);
        }

        public BytecodePatch(string sig, string enabled=null)
        {
            this.sig = sig;
            this.enabled = enabled;
        }

        public MethodInfo GetMethod() 
        {
            string[] a = sig.Split(':','(',',',')');
            // Hack.Log("Signature: " + String.Join(";", a));

            // Get type
            Type type = GetType(a[0]);
            if (a.Length < 3 || a[1].Length > 0) throw new Exception($"Expected \"::\" between class and method name, but got \"{sig}\".");

            // Get name
            string name = a[2];

            // Check for arguments.
            Type[] parameters = null;
            if (a.Length > 3) {
                if (a[3].Trim().Length == 0) {
                    // Empty argument list
                    if (a.Length != 5) throw new Exception($"Expected \"()\" for method without arguments, but got \"{sig}\".");
                    parameters = new Type[0];
                } else {
                    if (a[a.Length-1].Length > 0) throw new Exception($"Expected parentheses around method arguments, but got \"{sig}\".");
                    parameters = new Type[a.Length-4];
                    for (int i=0; i<parameters.Length; i++) {
                        parameters[i] = GetType(a[i+3]);
                    }
                }
            }
            var method = AccessTools.Method(type, name, parameters);
            if (method == null) {
                throw new Exception("Failed to find method {sig}.");
            }
            return method;
        }

        /** Retrieves the type definition with the specified name. */
        internal static Type GetType(string type) {
            var res = AccessTools.TypeByName(type.Trim());
            if (res == null) {
                throw new TypeAccessException($"ERROR: type \"{type}\" not found.");
            }
            return res;
        }
    }
}

